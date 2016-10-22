using System.Collections.Generic;
using System.Linq;
using CoreModels;
using CoreModels.XyUser;
using CoreModels.XyComm;
using CoreModels.XyCore;
using CoreModels.WmsApi;
using CoreData.CoreComm;
using Dapper;
using MySql.Data.MySqlClient;
using System;

namespace CoreData.CoreWmsApi
{
    public static class APurHaddle
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
                IParam.WmsboxLst.AddRange(ContactBox(IParam.BarCodeLst, IParam.CoID));//组合箱码&件码
            res = GetBoxByCode(IParam);//获取WmsPile 1.新增 2.修改
            if (res.s == 1)
            {
                string RecordID = "RE" + CommHaddle.GetRecordID(IParam.CoID);
                IParam.RecordID = RecordID;
                var CoreConn = new MySqlConnection(DbBase.CoreConnectString);
                var MsgConn = new MySqlConnection(DbBase.MsgConnectString);
                CoreConn.Open();
                MsgConn.Open();
                var CoreTrans = CoreConn.BeginTransaction();
                var MsgTrans = MsgConn.BeginTransaction();
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
                            //int count = CoreConn.Execute("Update ");
                        }
                        #region 采购收入
                        IParam.invType = 1;
                        IParam.Status = "审核通过";
                        var PurRec = new PurchaseReceive();
                        if (IParam.IsPur)
                        {
                            res = GetPurDetail(IParam);
                            IParam.APurDetailLst = res.d as List<PurchaseDetail>;
                            IParam.ApurDetail = IParam.APurDetailLst[0];
                            IParam.CusType = "采购收入";
                            IParam.Contents = "采购收入";
                            IParam.Apur = CoreConn.QueryFirst<Purchase>("SELECT * FROM WHERE CoID=@CoID AND ID=@ID",new{CoID=IParam.CoID,ID=IParam.ApurDetail.purchaseid});
                            var count = CoreConn.Execute(AddPurRecSql(),NewPurRec(IParam),CoreTrans);//新增收料单
                            if(count>0)
                            {                               
                                IParam.PurRecID=CoreConn.QueryFirst<int>("select LAST_INSERT_ID()");

                            }

                        }

                        #endregion
                    }
                }
                catch (Exception e)
                {
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
        public static List<AWmsBox> ContactBox(List<string> BarCodeLst, int CoID)
        {
            var BoxLst = new List<AWmsBox>();
            foreach (var BarCode in BarCodeLst)
            {
                var SkuID = BarCode.Substring(0, BarCode.Length - 6);
                var wbox = new AWmsBox();
                wbox.BarCode = BarCode;
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
            rec.coid = IParam.CoID;
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
            rec.norm = IParam.Norm;
            // rec.SkuID = IParam.Apur.SkuID;
            // rec.SkuName = IParam.Apur.SkuName;
            // rec.Norm = IParam.Apur.Norm;
            // rec.GoodsCode = IParam.Apur.GoodsCode;
            // rec.WarehouseID = IParam.PWID;
            // rec.WarehouseName = IParam.PWName;
            // rec.RecQty = IParam.RecQty;
            // rec.Creator = IParam.Creator;
            // rec.CreateDate = DateTime.Now;
            // rec.CoID = IParam.CoID;
            // rec.PurchaseID = IParam.Apur.PurchaseID;
            // rec.Status = IParam.Status;
            return rec;
        }
        #endregion
    }
}