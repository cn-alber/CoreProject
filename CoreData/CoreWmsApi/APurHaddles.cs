using System.Collections.Generic;
using System.Linq;
using System.Data;
using CoreModels;
using CoreModels.XyUser;
using CoreModels.XyComm;
using CoreModels.XyCore;
using CoreModels.WmsApi;
using CoreData.CoreComm;
using CoreData.CoreCore;
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
                //获取条码资料
                // IParam.IsBox = IsBoxTF(IParam.SkuID, IParam.CoID);//检查参数是箱码or件码
                // if (IsExist(IParam.IsBox, IParam.Type, IParam.BoxCode, IParam.CoID))//判断箱体是否已收入or已上架
                // {
                //     res.s = -1;
                //     res.d = "重复扫描";
                // }
                // else
                // {
                //     res = GetBoxSkuDetail(IParam);//获取Sku信息      
                // }
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

        #region 新增采购收料单
        public static DataResult SetPurRecDetail(ApiRecParam IParam)
        {
            var result = new DataResult(1, null);
            var CoreConn = new MySqlConnection(DbBase.CoreConnectString);
            CoreConn.Open();
            var CoreTrans = CoreConn.BeginTransaction(IsolationLevel.ReadUncommitted);
            try
            {
                //获取仓库基本资料
                string ParentID = string.Empty;
                int inv_type = 1101;
                string CusType = "采购进货";
                var WhIDLst = new List<string>();
                WhIDLst.Add(IParam.WhID.ToString());
                var res = CommHaddle.GetWhViewLstByID(IParam.CoID.ToString(), WhIDLst);
                var WhViewLst = res.d as List<Warehouse_view>;
                var RecSkuLst = (from a in IParam.RecSkuLst
                                 group a by new { a.Skuautoid, a.SkuID } into g
                                 select new ARecSkuSum
                                 {
                                     Skuautoid = g.Key.Skuautoid,
                                     SkuID = g.Key.SkuID,
                                     Qty = g.Sum(a => a.Qty)
                                 }).AsList();
                var SkuIDLst = IParam.RecSkuLst.Select(a => a.Skuautoid).Distinct().AsList();
                res = CommHaddle.GetSkuViewByID(IParam.CoID.ToString(), SkuIDLst);
                var SkuViewLst = res.d as List<CoreSkuView>;//获取商品Sku资料
                //获取采购资料
                res = GetPurDetail(IParam.PurID, IParam.CoID);
                if (res.s == 1)
                {
                    string RecordID = "RE" + CommHaddle.GetRecordID(IParam.CoID);
                    var PurD = res.d as List<APurchaseDetail>;
                    var PurM = CoreConn.Query<Purchase>("SELECT * FROM purchase WHERE CoID = @CoID AND ID =@ID", new { CoID = IParam.CoID, ID = PurD[0].PurchaseID }).AsList();
                    //新增采购收料主表
                    var RecM = new APurchaseReceive();
                    RecM.SCoID = PurM[0].scoid;
                    RecM.SCoName = PurM[0].sconame;
                    RecM.PurchaseID = PurM[0].id;
                    RecM.CoID = IParam.CoID;
                    RecM.Status = 1;
                    RecM.Creator = IParam.Creator;
                    RecM.CreateDate = IParam.CreateDate;
                    RecM.WarehouseID = int.Parse(WhViewLst[0].ID);
                    RecM.WarehouseName = WhViewLst[0].WhName;
                    RecM.ReceiveDate = DateTime.Now.ToString("yyyy-MM-dd");
                    CoreConn.Execute(AddPurRecSql(), RecM, CoreTrans);
                    var RecID = CoreConn.QueryFirst<int>("select LAST_INSERT_ID()", CoreTrans);
                    ParentID = RecID.ToString();
                    //新增采购收料明细
                    var RecDLst = (from a in RecSkuLst
                                   join b in SkuViewLst on a.Skuautoid equals b.ID into data
                                   from c in data.DefaultIfEmpty()
                                   select new APurchaseRecDetail
                                   {
                                       RecID = RecID,
                                       Skuautoid = a.Skuautoid,
                                       GoodsCode = c == null ? "" : c.GoodsCode,
                                       SkuID = c == null ? "" : c.SkuID,
                                       SkuName = c == null ? "" : c.SkuName,
                                       Norm = c == null ? "" : c.Norm,
                                       RecQty = a.Qty,
                                       Price = PurD[0].Price,
                                       Amount = PurD[0].Price * a.Qty,
                                       Creator = IParam.Creator,
                                       CreateDate = IParam.CreateDate,
                                       CoID = IParam.CoID
                                   }).AsList();
                    CoreConn.Execute(AddPurRecDetailSql(), RecDLst, CoreTrans);

                    #region 根据收料数量更新采购状态(1:已确认=>2.已完成)
                    string sql = @"SELECT
                                        IFNULL(SUM(purchaserecdetail.RecQty),0) 
                                    FROM
                                        purchaserecdetail ,
                                        purchasereceive
                                    WHERE purchaserecdetail.RecID=purchasereceive.ID
                                        AND purchasereceive.CoID=@CoID
                                        AND purchasereceive.PurchaseID=@PurID
                                        AND purchaserecdetail.Skuautoid=@Skuautoid
                                        AND purchasereceive.`Status`=1";
                    var args = new DynamicParameters();
                    args.Add("@CoID", IParam.CoID);
                    args.Add("@PurID", PurM[0].id);
                    args.Add("@Skuautoid", SkuIDLst[0]);
                    var RecQty = CoreConn.QueryFirst<int>(sql, args, CoreTrans);
                    string UptPurDtSql = "UPDATE purchasedetail SET RecQty=@RecQty,RecieveDate=@RecieveDate,DetailStatus=@DetailStatus WHERE ID=@PurID AND Skuautoid=@Skuautoid AND CoID=@CoID";
                    int DetailStatus = 1;//1:已确认;2.已完成;                    
                    if (RecQty >= PurD[0].PurQty)
                        DetailStatus = 2;
                    CoreConn.Execute(UptPurDtSql, new { RecQty = RecQty, RecieveDate = DateTime.Now.ToString("yyyy-MM-dd"), DetailStatus = DetailStatus, PurID = IParam.PurID, Skuautoid = SkuIDLst[0], CoID = IParam.CoID }, CoreTrans);//更新明细状态
                    sql = @"SELECT
                                purchasedetail.PurchaseID,
                                IFNULL(SUM(purchasedetail.PurQty),0) AS PurQty,
                                IFNULL(SUM(purchasedetail.RecQty),0) AS RecQty
                            FROM
                                purchasedetail
                            WHERE purchasedetail.PurchaseID=@PurID
                            AND purchasedetail.CoID = @CoID";
                    var Pur = CoreConn.Query<APurchaseDetail>(sql, new { CoID = IParam.CoID, PurID = PurM[0].id }, CoreTrans).AsList();
                    if (Pur[0].PurQty <= Pur[0].RecQty)
                    {
                        CoreConn.Execute("UPDATE purchase SET Status=2 WHERE CoID=@CoID AND ID=@PurID", new { CoID = IParam.CoID, PurID = PurM[0].id }, CoreTrans);
                    }
                    #endregion

                    #region 添加WmsPile库存&Wmslog操作记录
                    //更新WmsPile
                    string pilesql = "SELECT * FROM wmspile WHERE CoID = @CoID AND Skuautoid in @SkuIDLst AND Type=4 AND WarehouseID=@WhID";
                    var pileLst = CoreConn.Query<AWmsPile>(pilesql, new { CoID = IParam.CoID, SkuIDLst = SkuIDLst, WhID = IParam.WhID }, CoreTrans).AsList();
                    var NewPileLst = RecSkuLst.Where(a => !pileLst
                                                        .Select(b => b.Skuautoid)
                                                        .Contains(a.Skuautoid))
                                            .Select(a => new AWmsPile
                                            {
                                                Skuautoid = a.Skuautoid,
                                                SkuID = a.SkuID,
                                                PCode = "",
                                                WarehouseID = IParam.WhID,
                                                WarehouseName = WhViewLst[0].WhName,
                                                Type = int.Parse(WhViewLst[0].Type),
                                                Qty = a.Qty,
                                                Creator = IParam.Creator,
                                                CreateDate = IParam.CreateDate,
                                                CoID = IParam.CoID,
                                            }).AsList();
                    if (NewPileLst.Count > 0)
                    {
                        CoreConn.Execute(AddWmsPile(), NewPileLst, CoreTrans);
                    }
                    if (pileLst.Count > 0)
                    {
                        var UptPileLst = (from a in pileLst
                                          join b in RecSkuLst on a.Skuautoid equals b.Skuautoid
                                          select new AWmsPile
                                          {
                                              CoID = IParam.CoID,
                                              ID = a.ID,
                                              Qty = a.Qty + b.Qty
                                          }).AsList();
                        CoreConn.Execute("UPDATE wmspile SET Qty=@Qty WHERE CoID=@CoID AND ID=@ID", UptPileLst, CoreTrans);
                    }
                    //新增Log
                    var BoxLst = IParam.RecSkuLst.Where(a => a.SkuType == 2).ToList();//装箱Sku
                    var PieceLst = IParam.RecSkuLst.Where(a => a.SkuType < 2).ToList();//单件Sku   
                    var logLst = new List<AWmslog>();
                    if (BoxLst.Count > 0)
                    {
                        string boxsql = "SELECT BarCode,Skuautoid,SkuID,BoxCode,Qty FROM wmsbox WHERE CoID=@CoID AND BoxCode in @BoxCodeLst";
                        var BoxCodeLst = BoxLst.Select(a => a.BarCode).AsList();
                        var BoxSkuLst = CoreConn.Query<AWmsBox>(boxsql, new { CoID = IParam.CoID, BoxCodeLst = BoxCodeLst });
                        var logLstA = BoxSkuLst.Select(a => new AWmslog
                        {
                            BarCode = a.BarCode,
                            Skuautoid = a.Skuautoid,
                            SkuID = a.SkuID,
                            BoxCode = a.BoxCode,
                            WarehouseID = IParam.WhID,
                            Qty = a.Qty,
                            Contents = CusType,
                            Type = int.Parse(WhViewLst[0].Type),
                            RecordID = ParentID,
                            CoID = IParam.CoID,
                            Creator = IParam.Creator,
                            CreateDate = IParam.CreateDate
                        }).AsList();
                        logLst.AddRange(logLstA);
                    }
                    if (PieceLst.Count > 0)
                    {
                        var logLstB = PieceLst.Select(a => new AWmslog
                        {
                            BarCode = a.BarCode,
                            Skuautoid = a.Skuautoid,
                            SkuID = a.SkuID,
                            WarehouseID = IParam.WhID,
                            Qty = a.Qty,
                            Contents = CusType,
                            Type = int.Parse(WhViewLst[0].Type),
                            RecordID = ParentID,
                            CoID = IParam.CoID,
                            Creator = IParam.Creator,
                            CreateDate = IParam.CreateDate
                        }).AsList();
                        logLst.AddRange(logLstB);
                    }
                    if (logLst.Count > 0)
                    {
                        CoreConn.Execute(AddWmsLogSql(), logLst, CoreTrans);
                    }
                    #endregion

                    #region 产生库存交易&更新库存
                    //交易主表
                    var inv = new Invinout();
                    inv.RefID = ParentID;
                    inv.RecordID = RecordID;
                    inv.Type = inv_type;
                    inv.CusType = CusType;
                    inv.Status = 1;
                    inv.WhID = WhViewLst[0].ID;
                    inv.LinkWhID = WhViewLst[0].ParentID;
                    inv.Creator = IParam.Creator;
                    inv.CreateDate = IParam.CreateDate;
                    inv.CoID = IParam.CoID.ToString();
                    // CoreConn.Execute(InventoryHaddle.AddInvinoutSql(), inv, CoreTrans);
                    var inv_itemLst = IParam.RecSkuLst.Select(a => new Invinoutitem
                    {
                        RefID = ParentID,
                        IoID = RecordID,
                        Type = inv_type,
                        CusType = CusType,
                        Status = 1,
                        Skuautoid = a.Skuautoid,
                        Qty = a.Qty,
                        WhID = WhViewLst[0].ID,
                        LinkWhID = WhViewLst[0].ParentID,
                        Creator = IParam.Creator,
                        CreateDate = IParam.CreateDate,
                        CoID = IParam.CoID.ToString()
                    }).AsList();
                    // CoreConn.Execute(InventoryHaddle.AddInvinoutitemSql(), inv_itemLst, CoreTrans);

                    #endregion
                    string InvQuerySql = @"SELECT ID,Skuautoid,StockQty FROM Inventory WHERE CoID=@CoID AND Skuautoid in @SkuIDLst";
                    string InvMQuerySql = @"SELECT ID,Skuautoid,StockQty FROM Inventory_sale WHERE CoID=@CoID AND Skuautoid in @SkuIDLst";
                    var InvSkuLst = CoreConn.Query<Sfc_InvStock>(InvQuerySql, new { CoID = IParam.CoID, @WarehouseID = WhViewLst[0].ParentID, SkuIDLst = SkuIDLst }).AsList();//读取现有库存
                    var MainInvSkuLst = CoreConn.Query<Sfc_InvStock>(InvMQuerySql, new { CoID = IParam.CoID, SkuIDLst = SkuIDLst }).AsList();//读取现有主仓库存   
                    var NewInvLst = RecSkuLst.Where(a => !InvSkuLst
                                                           .Select(b => b.Skuautoid)
                                                           .Contains(a.Skuautoid))
                                           .Select(a => new Inventory
                                           {
                                               Skuautoid = a.Skuautoid,
                                               WarehouseID = WhViewLst[0].ParentID,
                                               CoID = IParam.CoID.ToString(),
                                               Creator = IParam.Creator,
                                               CreateDate = IParam.CreateDate
                                           }).AsList();
                    var NewMainInvLst = RecSkuLst.Where(a => !MainInvSkuLst
                                                        .Select(b => b.Skuautoid)
                                                        .Contains(a.Skuautoid))
                                        .Select(a => new Inventory_sale
                                        {
                                            Skuautoid = a.Skuautoid,
                                            CoID = IParam.CoID.ToString(),
                                            Creator = IParam.Creator,
                                            CreateDate = IParam.CreateDate
                                        }).AsList();
                    //新增交易表头
                    CoreConn.Execute(InventoryHaddle.AddInvinoutSql(), inv, CoreTrans);
                    //新增交易明细
                    CoreConn.Execute(InventoryHaddle.AddInvinoutitemSql(), inv_itemLst, CoreTrans);
                    //Sku库存新增
                    if (NewInvLst.Count > 0)
                    {
                        CoreConn.Execute(InventoryHaddle.AddInventorySql(), NewInvLst, CoreTrans);
                    }
                    if (NewMainInvLst.Count > 0)
                    {
                        CoreConn.Execute(InventoryHaddle.AddInventorySaleSql(), NewMainInvLst, CoreTrans);//Sku库存新增
                    }
                    //更新库存数量                                                                              
                    CoreConn.Execute(UptInvWaitInQtySql(), new { CoID = IParam.CoID, SkuIDLst = SkuIDLst, Modifier = IParam.Creator, ModifyDate = IParam.CreateDate }, CoreTrans);
                    //更新总库存数量
                    res = CommHaddle.GetWareCoidList(IParam.CoID.ToString());
                    var CoIDLst = res.d as List<string>;
                    CoreConn.Execute(UptInvMWaitInQtySql(), new { CoID = IParam.CoID, CoIDLst = CoIDLst, SkuIDLst = SkuIDLst, Modifier = IParam.Creator, ModifyDate = IParam.CreateDate }, CoreTrans);
                    CoreTrans.Commit();
                    CoreUser.LogComm.InsertUserLog("采购收入", "purchasereceive", "单据ID" + ParentID, IParam.Creator, IParam.CoID, DateTime.Now);

                }
                else
                {
                    result.s = res.s;
                    result.d = res.d;
                }
            }
            catch (Exception e)
            {
                CoreTrans.Rollback();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                CoreTrans.Dispose();
                CoreConn.Close();
            }

            //                 #region 操作记录worklog
            //                 StringBuilder sqltest = new StringBuilder();
            //                 foreach (var Log in IParam.WmsboxLst)
            //                 {
            //                     sqltest.Append("('" + Log.BarCode + "','" + Log.SkuID + "','" + Log.BoxCode
            //                           + "',0,-1,0"
            //                           + "," + Log.CoID + ",'" + IParam.Creator + "','" + DateTime.Now.ToString()
            //                           + "','" + IParam.Contents + "','" + IParam.RecordID + "'),");
            //                 }
            //                 foreach (var Log in IParam.WmsboxLst)
            //                 {
            //                     sqltest.Append("('" + Log.BarCode + "','" + Log.SkuID + "','" + Log.BoxCode
            //                           + "'," + IParam.WarehouseID + ",1," + IParam.Type
            //                           + "," + Log.CoID + ",'" + IParam.Creator + "','" + DateTime.Now.ToString()
            //                           + "','" + IParam.Contents + "','" + IParam.RecordID + "'),");
            //                 }
            // string ISql = @"INSERT INTO xymessage.worklog (
            //  xymessage.worklog.BarCode,
            //  xymessage.worklog.SkuID,
            //  xymessage.worklog.BoxCode,
            //  xymessage.worklog.WarehouseID,
            //  xymessage.worklog.qty,
            //  xymessage.worklog.Type,
            //  xymessage.worklog.CoID,
            //  xymessage.worklog.Creator,
            //  xymessage.worklog.CreateDate,
            //  xymessage.worklog.Contents,
            //  xymessage.worklog.RecordID
            // )  VALUES"
            //                         + sqltest.ToString().Substring(0, sqltest.ToString().Length - 1);
            //                 CoreConn.Execute(ISql, MsgTrans);//新增操作记录
            //                 #endregion

            return result;
        }
        #endregion

        #region 新增普通收料单
        public static DataResult SetRecDetail(ApiRecParam IParam)
        {
            var result = new DataResult(1, null);
            var CoreConn = new MySqlConnection(DbBase.CoreConnectString);
            CoreConn.Open();
            var CoreTrans = CoreConn.BeginTransaction(IsolationLevel.ReadUncommitted);
            try
            {
                string RecordID = "RE" + CommHaddle.GetRecordID(IParam.CoID);
                //获取仓库基本资料
                string ParentID = string.Empty;
                int inv_type = 1801;
                string CusType = "其他进仓";
                var WhIDLst = new List<string>();
                WhIDLst.Add(IParam.WhID.ToString());
                var res = CommHaddle.GetWhViewLstByID(IParam.CoID.ToString(), WhIDLst);
                var WhViewLst = res.d as List<Warehouse_view>;
                var RecSkuLst = (from a in IParam.RecSkuLst
                                 group a by new { a.Skuautoid, a.SkuID } into g
                                 select new ARecSkuSum
                                 {
                                     Skuautoid = g.Key.Skuautoid,
                                     SkuID = g.Key.SkuID,
                                     Qty = g.Sum(a => a.Qty)
                                 }).AsList();
                var SkuIDLst = IParam.RecSkuLst.Select(a => a.Skuautoid).AsList();
                res = CommHaddle.GetSkuViewByID(IParam.CoID.ToString(), SkuIDLst);
                var SkuViewLst = res.d as List<CoreSkuView>;//获取商品Sku资料
                #region 添加WmsPile库存&Wmslog操作记录
                //更新WmsPile
                string pilesql = "SELECT * FROM wmspile WHERE CoID = @CoID AND Skuautoid in @SkuIDLst AND Type=4 AND WarehouseID=@WhID";
                var pileLst = CoreConn.Query<AWmsPile>(pilesql, new { CoID = IParam.CoID, SkuIDLst = SkuIDLst, WhID = IParam.WhID }, CoreTrans).AsList();
                var NewPileLst = RecSkuLst.Where(a => !pileLst
                                                    .Select(b => b.Skuautoid)
                                                    .Contains(a.Skuautoid))
                                        .Select(a => new AWmsPile
                                        {
                                            Skuautoid = a.Skuautoid,
                                            SkuID = a.SkuID,
                                            PCode = "",
                                            WarehouseID = IParam.WhID,
                                            WarehouseName = WhViewLst[0].WhName,
                                            Type = int.Parse(WhViewLst[0].Type),
                                            Qty = a.Qty,
                                            Creator = IParam.Creator,
                                            CreateDate = IParam.CreateDate,
                                            CoID = IParam.CoID,
                                        }).AsList();
                if (NewPileLst.Count > 0)
                {
                    CoreConn.Execute(AddWmsPile(), NewPileLst, CoreTrans);
                }
                if (pileLst.Count > 0)
                {
                    var UptPileLst = (from a in pileLst
                                      join b in RecSkuLst on a.Skuautoid equals b.Skuautoid
                                      select new AWmsPile
                                      {
                                          CoID = IParam.CoID,
                                          ID = a.ID,
                                          Qty = a.Qty + b.Qty
                                      }).AsList();
                    CoreConn.Execute("UPDATE wmspile SET Qty=@Qty WHERE CoID=@CoID AND ID=@ID", UptPileLst, CoreTrans);
                }
                //新增Log
                var BoxLst = IParam.RecSkuLst.Where(a => a.SkuType == 2).ToList();//装箱Sku
                var PieceLst = IParam.RecSkuLst.Where(a => a.SkuType < 2).ToList();//单件Sku   
                var logLst = new List<AWmslog>();
                if (BoxLst.Count > 0)
                {
                    string boxsql = "SELECT BarCode,Skuautoid,SkuID,BoxCode,Qty FROM wmsbox WHERE CoID=@CoID AND BoxCode in @BoxCodeLst";
                    var BoxCodeLst = BoxLst.Select(a => a.BarCode).AsList();
                    var BoxSkuLst = CoreConn.Query<AWmsBox>(boxsql, new { CoID = IParam.CoID, BoxCodeLst = BoxCodeLst });
                    var logLstA = BoxSkuLst.Select(a => new AWmslog
                    {
                        BarCode = a.BarCode,
                        Skuautoid = a.Skuautoid,
                        SkuID = a.SkuID,
                        BoxCode = a.BoxCode,
                        WarehouseID = IParam.WhID,
                        Qty = a.Qty,
                        Contents = CusType,
                        Type = int.Parse(WhViewLst[0].Type),
                        RecordID = RecordID,
                        CoID = IParam.CoID,
                        Creator = IParam.Creator,
                        CreateDate = IParam.CreateDate
                    }).AsList();
                    logLst.AddRange(logLstA);
                }
                if (PieceLst.Count > 0)
                {
                    var logLstB = PieceLst.Select(a => new AWmslog
                    {
                        BarCode = a.BarCode,
                        Skuautoid = a.Skuautoid,
                        SkuID = a.SkuID,
                        WarehouseID = IParam.WhID,
                        Qty = a.Qty,
                        Contents = CusType,
                        Type = int.Parse(WhViewLst[0].Type),
                        RecordID = RecordID,
                        CoID = IParam.CoID,
                        Creator = IParam.Creator,
                        CreateDate = IParam.CreateDate
                    }).AsList();
                    logLst.AddRange(logLstB);
                }
                if (logLst.Count > 0)
                {
                    CoreConn.Execute(AddWmsLogSql(), logLst, CoreTrans);
                }
                #endregion

                #region 产生库存交易&更新库存
                //交易主表
                var inv = new Invinout();
                inv.RefID = ParentID;
                inv.RecordID = RecordID;
                inv.Type = inv_type;
                inv.CusType = CusType;
                inv.Status = 1;
                inv.WhID = WhViewLst[0].ID;
                inv.LinkWhID = WhViewLst[0].ParentID;
                inv.Creator = IParam.Creator;
                inv.CreateDate = IParam.CreateDate;
                inv.CoID = IParam.CoID.ToString();
                // CoreConn.Execute(InventoryHaddle.AddInvinoutSql(), inv, CoreTrans);
                var inv_itemLst = IParam.RecSkuLst.Select(a => new Invinoutitem
                {
                    RefID = ParentID,
                    IoID = RecordID,
                    Type = inv_type,
                    CusType = CusType,
                    Status = 1,
                    Skuautoid = a.Skuautoid,
                    Qty = a.Qty,
                    WhID = WhViewLst[0].ID,
                    LinkWhID = WhViewLst[0].ParentID,
                    Creator = IParam.Creator,
                    CreateDate = IParam.CreateDate,
                    CoID = IParam.CoID.ToString()
                }).AsList();
                // CoreConn.Execute(InventoryHaddle.AddInvinoutitemSql(), inv_itemLst, CoreTrans);

                #endregion
                string InvQuerySql = @"SELECT ID,Skuautoid,StockQty FROM Inventory WHERE CoID=@CoID AND Skuautoid in @SkuIDLst";
                string InvMQuerySql = @"SELECT ID,Skuautoid,StockQty FROM Inventory_sale WHERE CoID=@CoID AND Skuautoid in @SkuIDLst";
                var InvSkuLst = CoreConn.Query<Sfc_InvStock>(InvQuerySql, new { CoID = IParam.CoID, @WarehouseID = WhViewLst[0].ParentID, SkuIDLst = SkuIDLst }).AsList();//读取现有库存
                var MainInvSkuLst = CoreConn.Query<Sfc_InvStock>(InvMQuerySql, new { CoID = IParam.CoID, SkuIDLst = SkuIDLst }).AsList();//读取现有主仓库存   
                var NewInvLst = RecSkuLst.Where(a => !InvSkuLst
                                                       .Select(b => b.Skuautoid)
                                                       .Contains(a.Skuautoid))
                                       .Select(a => new Inventory
                                       {
                                           Skuautoid = a.Skuautoid,
                                           WarehouseID = WhViewLst[0].ParentID,
                                           CoID = IParam.CoID.ToString(),
                                           Creator = IParam.Creator,
                                           CreateDate = IParam.CreateDate
                                       }).AsList();
                var NewMainInvLst = RecSkuLst.Where(a => !MainInvSkuLst
                                                    .Select(b => b.Skuautoid)
                                                    .Contains(a.Skuautoid))
                                    .Select(a => new Inventory
                                    {
                                        Skuautoid = a.Skuautoid,
                                        CoID = IParam.CoID.ToString(),
                                        Creator = IParam.Creator,
                                        CreateDate = IParam.CreateDate
                                    }).AsList();
                //新增交易表头
                CoreConn.Execute(InventoryHaddle.AddInvinoutSql(), inv, CoreTrans);
                //新增交易明细
                CoreConn.Execute(InventoryHaddle.AddInvinoutitemSql(), inv_itemLst, CoreTrans);
                //Sku库存新增
                if (NewInvLst.Count > 0)
                {
                    CoreConn.Execute(InventoryHaddle.AddInventorySql(), NewInvLst, CoreTrans);
                }
                if (NewMainInvLst.Count > 0)
                {
                    CoreConn.Execute(InventoryHaddle.AddInventorySaleSql(), NewMainInvLst, CoreTrans);//Sku库存新增
                }
                //更新库存数量                                                                              
                CoreConn.Execute(UptInvWaitInQtySql(), new { CoID = IParam.CoID, SkuIDLst = SkuIDLst, Modifier = IParam.Creator, ModifyDate = IParam.CreateDate }, CoreTrans);
                //更新总库存数量
                res = CommHaddle.GetWareCoidList(IParam.CoID.ToString());
                var CoIDLst = res.d as List<string>;
                CoreConn.Execute(UptInvMWaitInQtySql(), new { CoID = IParam.CoID, CoIDLst = CoIDLst, SkuIDLst = SkuIDLst, Modifier = IParam.Creator, ModifyDate = IParam.CreateDate }, CoreTrans);
                CoreTrans.Commit();
                CoreUser.LogComm.InsertUserLog(CusType, "Invinout", "交易ID" + RecordID, IParam.Creator, IParam.CoID, DateTime.Now);

            }
            catch (Exception e)
            {
                CoreTrans.Rollback();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                CoreTrans.Dispose();
                CoreConn.Close();
            }
            return result;
        }
        #endregion

        #region 组合箱码&件码
        public static List<AWmsBox> ContactBox(List<string> BarCodeLst, int Skuautoid, int CoID)
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
                    Qty = DbBase.GoodsDB.QueryFirst<int>("SELECT IFNULL(SUM(Qty),0) FROM wmsbox WHERE CoID=@CoID AND BoxCode=@BoxCode", new { CoID = IParam.CoID, BoxCode = IParam.BoxCode });
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
                                Skuautoid,
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
                                @Skuautoid,
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
        public static DataResult GetPurDetail(int PurID, int CoID)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    string sql = "SELECT * FROM purchasedetail WHERE CoID=@CoID AND DetailStatus=1";//明细状态(0:待审核;1:已确认;2.已完成;3.待发货;4.待收货;5:作废)
                    var p = new DynamicParameters();
                    p.Add("@CoID", CoID);
                    if (PurID > 0)
                    {
                        sql = sql + " AND ID=@PurID";
                        p.Add("@PurID", PurID);
                    }
                    var Lst = conn.Query<APurchaseDetail>(sql, p).AsList();
                    if (Lst.Count <= 0)
                    {
                        res.s = -1;
                        res.d = "获取采购信息失败";
                    }
                    else
                        res.d = Lst;
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


        #endregion

        #region 新增操作记录
        public static string AddWmsLogSql()
        {
            string sql = @"INSERT INTO wmslog (
                             BarCode,
                             Skuautoid,
                             SkuID,
                             BoxCode,
                             WarehouseID,
                             Qty,
                             Type,
                             CoID,
                             Creator,
                             CreateDate,
                             Contents,
                             RecordID
                            )  VALUES (
                             @BarCode,
                             @Skuautoid,
                             @SkuID,
                             @BoxCode,
                             @WarehouseID,
                             @Qty,
                             @Type,
                             @CoID,
                             @Creator,
                             @CreateDate,
                             @Contents,
                             @RecordID
                            )";
            return sql;
        }
        #endregion


        #region 新增库存
        public static string AddInventorySql()
        {
            string sql = @"INSERT INTO inventory (
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

        #endregion

        #region 更新库存数量&待入库数量(分仓)
        public static string UptInvWaitInQtySql()
        {
            string sql = @"UPDATE inventory 
                            SET StockQty= (SELECT IFNULL(sum(Qty),0)
	                                            FROM
		                                            invinoutitem
	                                            WHERE
		                                            invinoutitem.Skuautoid = inventory.Skuautoid
	                                            AND invinoutitem.CoID = inventory.CoID
                                                AND invinoutitem.`Status` = 1
                                                AND invinoutitem.Skuautoid in @SkuIDLst),
                                WaitInQty = (SELECT IFNULL(SUM(Qty),0)
                                                FROM wmspile
                                                WHERE wmspile.Skuautoid = inventory.Skuautoid
                                                AND wmspile.CoID = inventory.CoID
                                                AND wmspile.Skuautoid in @SkuIDLst
                                                AND wmspile.Type = 4),
                                            IsDelete=0,Modifier=@Modifier,ModifyDate=@ModifyDate
                                            WHERE
	                                            inventory.CoID =@CoID
                                            AND Inventory.Skuautoid in @SkuIDLst";
            return sql;
        }
        #endregion

        #region 更新总库存数量-待入库数量(主仓)
        public static string UptInvMWaitInQtySql()
        {
            string sql = @"UPDATE Inventory_sale
                            SET Inventory_sale.StockQty = (
                                SELECT          
                                    IFNULL(SUM(StockQty),0)
                                FROM
                                    inventory
                                WHERE
                                    inventory.CoID IN @CoIDLst
                                AND inventory.Skuautoid = inventory_sale.Skuautoid),
                            inventory_sale.WaitInQty = (
                                SELECT          
                                    IFNULL(SUM(WaitInQty),0)
                                FROM
                                    inventory
                                WHERE
                                    inventory.CoID IN @CoIDLst
                                AND inventory.Skuautoid = inventory_sale.Skuautoid
                            ),IsDelete=0,Modifier=@Modifier,ModifyDate=@ModifyDate
                            WHERE inventory_sale.CoID=@CoID
                            AND inventory_sale.Skuautoid in @SkuIDLst";
            return sql;
        }
        #endregion
    }
}