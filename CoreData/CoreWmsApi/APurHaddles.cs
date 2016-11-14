using System.Collections.Generic;
using System.Linq;
using System.Data;
using CoreModels;
using CoreModels.XyUser;
using CoreModels.XyComm;
using CoreModels.XyCore;
using CoreModels.WmsApi;
using CoreData.CoreComm;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Text;

namespace CoreData.CoreWmsApi
{
    public static class APurHaddles
    {
        #region 普通收货作业-获取Sku详情
        public static DataResult GetBoxSku(WmsBoxParams IParam)
        {
            var res = new DataResult(1, null);
            try
            {
                IParam.IsBox = IsBoxTF(IParam.SkuID, IParam.CoID);//检查参数是箱码or件码
                if (IsExist(IParam.IsBox, IParam.Type, IParam.BoxCode, IParam.CoID))//判断箱体是否已收入or已上架
                {
                    res.s = -1;
                    res.d = "重复扫描";
                }
                else
                {
                    res = GetBoxSkuDetail(IParam);//获取Sku信息      
                }
            }
            catch (Exception e)
            {
                res.s = -1;
                res.d = e.Message;
            }
            finally
            {
                DbBase.GoodsDB.Close();
                DbBase.CoreDB.Close();
            }
            return res;
        }
        #endregion

        #region 新增收料单
        public static DataResult SetPurRecDetail(APurParams IParam)
        {
            var res = new DataResult(1, null);
            var BoxCodeLst = IParam.BPLst.Where(a => a.IsBox == true).ToList().Select(b => b.BoxCode).AsList();//装箱Sku
            var PieceLst = IParam.BPLst.Where(a => a.IsBox == false).ToList().Select(b => b.BoxCode).AsList();//单件Sku            
            var BoxLst = DbBase.GoodsDB.Query<AWmsBox>(@"SELECT * FROM wmsbox WHERE CoID=@CoID AND BoxCode IN @BoxCodeLst", new { CoID = IParam.CoID, BoxCodeLst = BoxCodeLst }).AsList();
            IParam.WmsboxLst = BoxLst;
            IParam.BarCodeLst = PieceLst;
            if (IParam.BarCodeLst.Count > 0)
                IParam.WmsboxLst.AddRange(ContactBox(IParam.BarCodeLst,IParam.Skuautoid,int.Parse(IParam.CoID)));//组合箱码&件码
            IParam.RecQty = IParam.WmsboxLst.Count();
            res = GetBoxByCode(IParam);//获取WmsPile 1.新增 2.修改
            if (res.s == 1)
            {
                string RecordID = "RE" + CommHaddle.GetRecordID(int.Parse(IParam.CoID));
                IParam.RecordID = RecordID;
                var CoreConn = new MySqlConnection(DbBase.CoreConnectString);
                var MsgConn = new MySqlConnection(DbBase.MsgConnectString);
                CoreConn.Open();
                MsgConn.Open();
                var CoreTrans = CoreConn.BeginTransaction(IsolationLevel.ReadUncommitted);
                var MsgTrans = MsgConn.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    var Lst = res.d as List<WmsPileAuto>;
                    if (Lst.Count > 0)
                    {
                        var NewLst = Lst.Where(a => a.status == 1).ToList();
                        var UptLst = Lst.Where(a => a.status == 2).ToList();
                        if (NewLst.Count > 0)
                        {
                            int count = CoreConn.Execute(AddWmsPile(), NewLst, CoreTrans);//新增暂存WmsPile
                        }
                        if (UptLst.Count > 0)
                        {
                            string uptpilesql = "UPDATE wmspile SET qty=qty+@RecQty WHERE CoID=@CoID AND Type=@Type AND skuautoid=@Skuautoid";
                            var args = new{RecQty=UptLst[0].Qty,CoID=IParam.CoID,Type=IParam.Type,Skuautoid=IParam.Skuautoid};
                            int count = CoreConn.Execute(uptpilesql,args,CoreTrans);
                        }
                        IParam.invType = 1;
                        IParam.Status = 1;
                        #region 采购收料
                        if (IParam.IsPur)
                        {
                            var PurRec = new PurchaseReceive();
                            res = GetPurDetail(IParam);
                            IParam.APurDetailLst = res.d as List<PurchaseDetail>;
                            IParam.ApurDetail = IParam.APurDetailLst[0];
                            IParam.CusType = "采购收入";
                            IParam.Contents = "采购收入";
                            IParam.Apur = CoreConn.QueryFirst<Purchase>("SELECT * FROM WHERE CoID=@CoID AND ID=@ID", new { CoID = IParam.CoID, ID = IParam.ApurDetail.purchaseid });
                            var count = CoreConn.Execute(AddPurRecSql(), NewPurRec(IParam), CoreTrans);//新增收料单
                            if (count > 0)
                            {
                                IParam.PurRecID = CoreConn.QueryFirst<int>("select LAST_INSERT_ID()");
                                count = CoreConn.Execute(AddPurRecDetailSql(), NewPurRecDetail(IParam), CoreTrans);//新增收料明细
                            }
                            #region 根据收料数量更新采购状态(1:已确认=>2.已完成)
                            string sql = @"SELECT
                                                purchasereceive.PurchaseID,
                                                purchaserecdetail.Skuautoid,
                                                SUM(purchaserecdetail.RecQty) AS recqty
                                            FROM
                                                purchaserecdetail ,
                                                purchasereceive
                                            WHERE purchaserecdetail.RecID=purchasereceive.ID
                                                AND purchasereceive.PurchaseID=@PurID
                                                AND purchaserecdetail.Skuautoid=@Skuautoid
                                                AND purchasereceive.`Status`=1
                                                AND purchasereceive.CoID=@CoID
                                            GROUP BY
                                                purchasereceive.PurchaseID,
                                                purchaserecdetail.Skuautoid ";
                            var args = new DynamicParameters();
                            args.Add("@PurID", IParam.Apur.id);
                            args.Add("@Skuautoid", IParam.ApurDetail.skuautoid);
                            var PurDt = CoreConn.QueryFirst<APurchaseDetail>(sql, args);
                            string UptPurDtSql = "UPDATE purchasedetail SET RecQty=@RecQty,DetailStatus=@DetailStatus WHERE PurchaseID=@PurID AND Skuautoid=@Skuautoid AND CoID=@CoID";
                            int DetailStatus = 1;//1:已确认;2.已完成;
                            if (PurDt.recqty >= int.Parse(IParam.ApurDetail.purqty))
                                DetailStatus = 2;
                            CoreConn.Execute(UptPurDtSql, new { RecQty = PurDt.recqty, DetailStatus = DetailStatus, PurID = IParam.Apur.id, Skuautoid = IParam.ApurDetail.skuautoid, CoID = IParam.CoID });//更新明细状态
                            sql = @"SELECT
                                        purchasedetail.PurchaseID,
                                        SUM(purchasedetail.PurQty) AS purqty,
                                        SUM(purchasedetail.RecQty) AS recqty
                                    FROM
                                        purchasedetail
                                    WHERE purchasedetail.PurchaseID=@PurID
                                    GROUP BY purchasedetail.PurchaseID";
                            var Pur = CoreConn.QueryFirst<APurchaseDetail>(sql, new { PurID = IParam.Apur.id });
                            if (Pur.purqty <= Pur.recqty)
                            {
                                CoreConn.Execute("UPDATE purchase SET Status=2 WHERE CoID=@CoID AND ID=@PurID", new { CoID = IParam.CoID, PurID = IParam.Apur.id });
                            }
                            #endregion
                        }
                        else
                        {
                            IParam.CusType = "普通收入";
                            IParam.Contents = "普通收入";
                        }
                        #endregion
                        #region 新增库存交易
                        var InvIO = NewPurInvInOut(IParam);
                        var InvItem = NewPurInvInOutItem(IParam);
                        CoreConn.Execute(CoreData.CoreCore.InventoryHaddle.AddInvinoutSql(), InvIO, CoreTrans);
                        CoreConn.Execute(CoreData.CoreCore.InventoryHaddle.AddInvinoutitemSql(), InvItem, CoreTrans);
                        #endregion
                        #region 操作记录worklog
                        StringBuilder sqltest = new StringBuilder();
                        foreach (var Log in IParam.WmsboxLst)
                        {
                            sqltest.Append("('" + Log.BarCode + "','" + Log.SkuID + "','" + Log.BoxCode
                                  + "',0,-1,0"
                                  + "," + Log.CoID + ",'" + IParam.Creator + "','" + DateTime.Now.ToString()
                                  + "','" + IParam.Contents + "','" + IParam.RecordID + "'),");
                        }
                        foreach (var Log in IParam.WmsboxLst)
                        {
                            sqltest.Append("('" + Log.BarCode + "','" + Log.SkuID + "','" + Log.BoxCode
                                  + "'," + IParam.WarehouseID + ",1," + IParam.Type
                                  + "," + Log.CoID + ",'" + IParam.Creator + "','" + DateTime.Now.ToString()
                                  + "','" + IParam.Contents + "','" + IParam.RecordID + "'),");
                        }
                        string ISql = @"INSERT INTO xymessage.worklog (
                         xymessage.worklog.BarCode,
                         xymessage.worklog.SkuID,
                         xymessage.worklog.BoxCode,
                         xymessage.worklog.WarehouseID,
                         xymessage.worklog.qty,
                         xymessage.worklog.Type,
                         xymessage.worklog.CoID,
                         xymessage.worklog.Creator,
                         xymessage.worklog.CreateDate,
                         xymessage.worklog.Contents,
                         xymessage.worklog.RecordID
                        )  VALUES"
                                + sqltest.ToString().Substring(0, sqltest.ToString().Length - 1);
                        CoreConn.Execute(ISql,MsgTrans);//新增操作记录
                        #endregion
                        var invLst = NewPurInventory(IParam);
                        if(invLst.Count>0)
                        {
                            CoreConn.Execute(AddInventorySql(),invLst[0],CoreTrans);
                        }
                        else
                        {
                            string UpdateInvsql = @"UPDATE inventory
                                        SET inventory.StockQty = (
	                                        SELECT
		                                        sum(Qty)
	                                        FROM
		                                        invinoutitem
	                                        WHERE
		                                        invinoutitem.skuautoid = inventory.skuautoid
	                                        AND invinoutitem.WhID = inventory.WarehouseID
	                                        AND invinoutitem.CoID = inventory.CoID
                                        )
                                        WHERE
	                                        inventory.CoID =@CoID
                                        AND inventory.WarehouseID = @WarehouseID
                                        AND inventory.skuautoid = @skuautoid";
                            CoreConn.Execute(UpdateInvsql,new{CoID=IParam.CoID,WarehouseID=IParam.WarehouseID,skuautoid=IParam.ApurDetail.skuautoid},CoreTrans);
                        }
                        CoreTrans.Commit();
                        MsgTrans.Commit();
                    }
                }
                catch (Exception e)
                {
                    CoreTrans.Rollback();
                    MsgTrans.Rollback();
                    res.s = -1;
                    res.d = e.Message;
                }
                finally
                {
                    CoreTrans.Dispose();
                    MsgTrans.Dispose();
                    CoreConn.Close();
                    MsgConn.Close();
                }
            }


            return res;
        }
        #endregion


        #region 组合箱码&件码
        public static List<AWmsBox> ContactBox(List<string> BarCodeLst,int Skuautoid, int CoID)
        {
            var BoxLst = new List<AWmsBox>();
            foreach (var BarCode in BarCodeLst)
            {
                var SkuID = BarCode.Substring(0, BarCode.Length - 6);
                var wbox = new AWmsBox();
                wbox.BarCode = BarCode;
                wbox.Skuautoid = Skuautoid;
                wbox.SkuID = SkuID;
                wbox.CoID = CoID;
                BoxLst.Add(wbox);
            }
            return BoxLst;
        }
        #endregion

        #region 根据装箱单号返回收入暂存依据
        public static DataResult GetBoxByCode(APurParams IParam)
        {
            var res = new DataResult(1, null);
            var SkuIDLst = IParam.WmsboxLst.Select(a => a.SkuID.ToUpper()).Distinct().ToList<string>();
            var SkuSql = @"SELECT ID,SkuID,GoodsCode,ColorName,SizeName FROM coresku WHERE CoID=@CoID AND SkuID IN @SkuIDLst";
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    var SkuYLst = new List<ACoreSku>();
                    string sql = @"SELECT Distinct SkuID FROM wmspile WHERE CoID=@CoID AND Type=@Type AND SkuID IN @SkuIDLst";
                    var p = new { CoID = IParam.CoID, Type = IParam.Type, SkuIDLst = SkuIDLst };
                    var PileSkuLst = conn.Query<string>(sql, p).AsList();//Pile已存在资料
                    if (PileSkuLst.Count > 0)
                    {
                        SkuYLst = conn.Query<ACoreSku>(SkuSql, new { CoID = IParam.CoID, SkuIDLst = PileSkuLst }).AsList();
                        foreach (var sku in SkuYLst)
                        {
                            if (SkuIDLst.Contains(sku.SkuID.ToUpper()))
                                SkuIDLst.Remove(sku.SkuID.ToUpper());
                        }
                    }
                    var SkuNLst = conn.Query<ACoreSku>(sql, new { CoID = IParam.CoID, SkuIDLst = SkuIDLst }).AsList();
                    var Lst = (from b in IParam.WmsboxLst
                               join s in SkuYLst
                               on new { SkuID = b.SkuID.ToUpper() } equals new { SkuID = s.SkuID.ToUpper() }
                               group new { b, s } by new { s.SkuID, s.SkuName, s.Norm, s.GoodsCode } into c
                               select new
                               {
                                   CoID = IParam.CoID,
                                   GoodsCode = c.Key.GoodsCode,
                                   SkuID = c.Key.SkuID,
                                   SkuName = c.Key.SkuName,
                                   Norm = c.Key.Norm,
                                   WarehouseID = IParam.WarehouseID,
                                   WarehouseName = IParam.WarehouseName,
                                   Creator = IParam.Creator,
                                   Qty = c.Count(),
                                   Type = IParam.Type,
                                   Status = 2//修改
                               })
                      .Union(from b in IParam.WmsboxLst
                             join s in SkuNLst
                             on new { SkuID = b.SkuID.ToUpper() } equals new { SkuID = s.SkuID.ToUpper() }
                             group new { b, s } by new { s.SkuID, s.SkuName, s.Norm, s.GoodsCode } into c
                             select new
                             {
                                 CoID = IParam.CoID,
                                 GoodsCode = c.Key.GoodsCode,
                                 SkuID = c.Key.SkuID,
                                 SkuName = c.Key.SkuName,
                                 Norm = c.Key.Norm,
                                 WarehouseID = IParam.WarehouseID,
                                 WarehouseName = IParam.WarehouseName,
                                 Creator = IParam.Creator,
                                 Qty = c.Count(),
                                 Type = IParam.Type,
                                 Status = 1//新增
                             })
                      .ToList();
                    var WmsPileLst = new List<WmsPileAuto>();
                    if (Lst.Count > 0)
                    {
                        foreach (var l in Lst)
                        {
                            var WmsPile = new WmsPileAuto();
                            WmsPile.CoID = l.CoID;
                            WmsPile.GoodsCode = l.GoodsCode;
                            // WmsPile.Skuautoid = 
                            WmsPile.SkuID = l.SkuID;
                            WmsPile.SkuName = l.SkuName;
                            WmsPile.Norm = l.Norm;
                            WmsPile.WarehouseID = l.WarehouseID;
                            WmsPile.WarehouseName = l.WarehouseName;
                            WmsPile.Creator = l.Creator;
                            WmsPile.Qty = l.Qty;
                            WmsPile.status = l.Status;
                            WmsPile.Type = l.Type;
                            WmsPileLst.Add(WmsPile);
                        }
                    }
                    res.d = WmsPileLst;
                }
                catch (Exception e)
                {
                    conn.Close();
                    res.s = -1;
                    res.d = e.Message;
                }
            }
            return res;
        }
        #endregion


        #region 检查参数是箱码or件码
        public static Boolean IsBoxTF(string SkuID, int CoID)
        {
            Boolean IsBox = true;
            int count = DbBase.CoreDB.QueryFirst<int>("SELECT count(ID) FROM coresku WHERE SkuID=@SkuID AND CoID=@CoID", new { SkuID = SkuID, CoID = CoID });
            if (count > 0)
                IsBox = false;
            return IsBox;
        }
        #endregion
        #region 判断箱体是否已收入or已上架
        public static Boolean IsExist(Boolean IsBox, int Type, string BoxCode, int CoID)
        {
            Boolean IsExist = true;
            string sql = "SELECT IFNULL(SUM(qty),0) FROM worklog WHERE CoID=@CoID AND Type=@Type";
            if (IsBox)
            {
                sql = sql + " AND BoxCode=@BoxCode";
            }
            else
            {
                sql = sql + " AND BarCode=@BoxCode";
            }
            var p = new DynamicParameters();
            p.Add("@CoID", CoID);
            p.Add("@Type", Type);
            p.Add("@BoxCode", BoxCode);
            var Qty = DbBase.MsgDB.QueryFirst<int>(sql, p);
            if (Qty <= 0)
                IsExist = false;
            return IsExist;
        }
        #endregion
        #region 根据箱号取Sku
        public static DataResult GetBoxSkuDetail(WmsBoxParams IParam)
        {
            var res = new DataResult(1, null);
            int Qty = 1;
            try
            {
                if (IParam.IsBox)
                {
                    Qty = DbBase.GoodsDB.QueryFirst<int>("SELECT count(ID) FROM wmsbox WHERE CoID=@CoID AND BoxCode=@BoxCode", new { CoID = IParam.CoID, BoxCode = IParam.BoxCode });
                    IParam.SkuID = DbBase.GoodsDB.QueryFirst<string>("SELECT SkuID FROM wmsbox WHERE CoID=@CoID AND BoxCode=@BoxCode LIMIT 1", new { CoID = IParam.CoID, BoxCode = IParam.BoxCode });
                }
                string sql = @"SELECT ID,                                    
                                    SkuID,
                                    GoodsCode,
                                    ColorName,
                                    SizeName
                               FROM coresku 
                               WHERE CoID=@CoID AND SkuID=@SkuID";
                var ACore = DbBase.CoreDB.QueryFirst<ACoreSku>(sql, new { CoID = IParam.CoID, SkuID = IParam.SkuID });
                if (ACore != null)
                {
                    ACore.Qty = Qty;
                    ACore.IsBox = IParam.IsBox;
                    res.d = ACore;
                }
                else
                {
                    res.s = -1;
                    res.d = "无效Sku";
                }
            }
            catch (Exception e)
            {
                res.s = -1;
                res.d = e.Message;
            }
            finally
            {
                DbBase.GoodsDB.Close();
                DbBase.CoreDB.Close();
            }
            return res;
        }
        #endregion

        #region 新增暂存档wmspile
        public static string AddWmsPile()
        {
            string sql = @"INSERT INTO wmspile(
                                PCode,
                                SkuID,
                                WarehouseID,
                                WarehouseName,
                                Type,
                                PCType,
                                `Order`,
                                qty,
                                lockqty,
                                `Enable`,
                                Creator,
                                CreateDate,
                                CoID,
                                maxqty ) 
                            VALUES (
                                @PCode,
                                @SkuID,
                                @WarehouseID,
                                @WarehouseName,
                                @Type,
                                @PCType,
                                @Order,
                                @qty,
                                @lockqty,
                                @Enable,
                                @Creator,
                                @CreateDate,
                                @CoID,
                                @maxqty )";
            return sql;
        }
        #endregion

        #region 获取采购资料
        public static DataResult GetPurDetail(APurParams IParam)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    string sql = "SELECT * FROM purchasedetail WHERE CoID=@CoID AND DetailStatus=2";//2.已确认
                    var p = new DynamicParameters();
                    p.Add("@CoID", IParam.CoID);
                    if (IParam.PurID > 0)
                    {
                        sql = sql + " AND ID=@PurID";
                        p.Add("@PurID", IParam.PurID);
                    }
                    var Lst = conn.Query<PurchaseDetail>(sql, p).AsList();
                    if (Lst.Count <= 0)
                    {
                        res.s = -1;
                        res.d = "获取采购信息失败";
                    }
                }
                catch (Exception e)
                {
                    res.s = -1;
                    res.d = e.Message;
                }
            }
            return res;
        }
        #endregion

        #region 新增采购收料
        public static string AddPurRecSql()
        {
            string sql = @" INSERT INTO purchasereceive(
                        ScoID,
                        ScoName,
                        PurchaseID,
                        CoID,
                        Creator,
                        CreateDate,
                        WarehouseID,
                        WarehouseName,
                        `Status`,
                        ReceiveDate
                        ) VALUES (
                        @ScoID,
                        @ScoName,
                        @PurchaseID,
                        @CoID,
                        @Creator,
                        @CreateDate,
                        @WarehouseID,
                        @WarehouseName,
                        1,
                        @ReceiveDate    
                        )
                        ";
            return sql;
        }
        public static string AddPurRecDetailSql()
        {
            string sql = @"
            INSERT INTO purchaserecdetail(
                                RecID,
                                Skuautoid,
                                SkuID,
                                SkuName,
                                Norm,
                                RecQty,
                                Price,
                                Amount,
                                GoodsCode,
                                CoID,
                                Creator,
                                CreateDate,
                                Remark )
                            VALUES (
                                @RecID,
                                @Skuautoid,
                                @SkuID,
                                @SkuName,
                                @Norm,
                                @RecQty,
                                @Price,
                                @Amount,
                                @GoodsCode,
                                @CoID,
                                @Creator,
                                @CreateDate,
                                @Remark    
                                )";
            return sql;
        }

        public static PurchaseReceive NewPurRec(APurParams IParam)
        {
            var rec = new PurchaseReceive();
            rec.scoid = IParam.Apur.scoid;
            rec.sconame = IParam.Apur.sconame;
            rec.purchaseid = IParam.Apur.id;
            rec.coid = int.Parse(IParam.CoID);
            rec.creator = IParam.Creator;
            rec.createdate = DateTime.Now;
            rec.warehouseid = IParam.WarehouseID;
            rec.warehousename = IParam.WarehouseName;
            rec.receivedate = DateTime.Now.ToString("yyyy-mm-dd");
            return rec;
        }
        public static PurchaseRecDetail NewPurRecDetail(APurParams IParam)
        {
            var rec = new PurchaseRecDetail();
            rec.recid = IParam.PurRecID;
            rec.skuautoid = IParam.ApurDetail.skuautoid;
            rec.skuid = IParam.ApurDetail.skuid;
            rec.norm = IParam.ApurDetail.norm;
            rec.skuname = IParam.ApurDetail.skuname;
            rec.recqty = IParam.RecQty.ToString();
            rec.price = IParam.ApurDetail.price;
            rec.amount = (IParam.RecQty * double.Parse(rec.price)).ToString();
            rec.goodscode = IParam.ApurDetail.goodscode;
            rec.creator = IParam.Creator;
            rec.createdate = DateTime.Now;
            rec.coid =  int.Parse(IParam.CoID);
            return rec;
        }
        #endregion

        #region 新增库存交易

        public static Invinout NewPurInvInOut(APurParams IParam)
        {
            var inv = new Invinout();
            inv.RecordID = IParam.RecordID;
            inv.Type = IParam.invType;
            inv.CusType = IParam.CusType;
            inv.Status = IParam.Status;
            inv.WhID = IParam.PWID.ToString();
            inv.WhName = IParam.PWName;
            inv.Creator = IParam.Creator;
            inv.CreateDate = DateTime.Now.ToString();
            inv.CoID = IParam.CoID.ToString();
            inv.RecID = IParam.PurRecID;
            return inv;
        }

        public static Invinoutitem NewPurInvInOutItem(APurParams IParam)
        {
            var inv = new Invinoutitem();
            inv.IoID = IParam.RecordID;
            inv.SkuID = IParam.ApurDetail.skuid;
            inv.Skuautoid = IParam.ApurDetail.skuautoid.ToString();
            inv.Norm = IParam.ApurDetail.norm;
            inv.Qty = IParam.RecQty;
            inv.Creator = IParam.Creator;
            inv.CreateDate = DateTime.Now.ToString();
            inv.CoID = IParam.CoID;
            inv.CusType = IParam.CusType;
            inv.WhID = IParam.WarehouseID.ToString();
            inv.WhName = IParam.WarehouseName;
            return inv;
        }
        #endregion
        #region 新增库存
        public static string AddInventorySql()
        {
            string sql=@"INSERT INTO inventory (
                                    GoodsCode,
                                    Skuautoid,
                                    SkuID,
                                    `Name`,
                                    Norm,
                                    WarehouseID,
                                    WarehouseName,
                                    CreateDate,
                                    StockQty,
                                    Creator)
                            VALUES (
                                        @GoodsCode,
                                    @Skuautoid,
                                    @SkuID,
                                    @Name,
                                    @Norm,
                                    @WarehouseID,
                                    @WarehouseName,
                                    @CreateDate,
                                    @StockQty,
                                    @Creator
                                    )
                                ";
            return sql;
        }
        public static List<Inventory> NewPurInventory(APurParams IParam)
        {
            var InvLst = new List<Inventory>();
            string sql = "SELECT Count(ID) FROM inventory WHERE CoID=@CoID AND WarehouseID=@WarehouseID AND skuautoid=@SkuAutoID";
            var args = new { CoID = IParam.CoID, WarehouseID = IParam.WarehouseID, SkuAutoID = IParam.ApurDetail.skuautoid };
            int count = DbBase.CoreDB.QueryFirst(sql, args);
            if (count <= 0)
            {
                var inv = new Inventory();
                inv.CoID =  IParam.CoID;
                inv.Skuautoid = IParam.ApurDetail.skuautoid.ToString();
                inv.SkuID = IParam.ApurDetail.skuid;
                inv.Name = IParam.ApurDetail.skuname;
                inv.Norm = IParam.ApurDetail.norm;
                inv.GoodsCode = IParam.ApurDetail.goodscode;
                inv.WarehouseID = IParam.WarehouseID.ToString();
                inv.WarehouseName = IParam.WarehouseName;
                InvLst.Add(inv);
            }
            return InvLst;
        }
        #endregion
    }
}