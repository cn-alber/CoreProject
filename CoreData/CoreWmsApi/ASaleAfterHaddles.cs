using System.Collections.Generic;
using System.Linq;
using System.Data;
using CoreModels;
// using CoreModels.XyUser;
using CoreModels.Enum;
using CoreModels.XyComm;
using CoreModels.XyCore;
using CoreModels.WmsApi;
using CoreData.CoreComm;
using CoreData.CoreCore;
using Dapper;
using MySql.Data.MySqlClient;
using System;
// using System.Text;
namespace CoreData.CoreWmsApi
{
    public static class ASaleAfterHaddles
    {
        #region 售后退回-获取退货信息
        public static DataResult AfterScanSku(ASaleAfterParam IParam)
        {
            var result = new DataResult(1, null);
            using (var CoreConn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    var data = new ASaleAfter();
                    var cp = new ASkuScanParam();
                    cp.CoID = IParam.CoID;
                    cp.BarCode = IParam.BarCode;
                    result = ASkuScanHaddles.GetType(cp);
                    if (result.s > 0)
                    {
                        var asku = result.d as ASkuScan;
                        string sql = @"SELECT ID,
                                    BatchID,
                                    OID,
                                    SoID
                                FROM batchpicked
                                WHERE CoID=@CoID ";
                        if (asku.SkuType == 0)
                        {
                            sql = sql + " AND BarCode=@BarCode";
                        }
                        else if (asku.SkuType == 2)
                        {
                            sql = sql + " AND BoxCode=@BarCode";
                        }
                        else if (asku.SkuType == 1)
                        {
                            sql = sql + " AND SkuID=@BarCode";
                        }
                        var p = new DynamicParameters();
                        p.Add("@CoID", IParam.CoID);
                        p.Add("@BarCode", IParam.BarCode);
                        var PickLst = CoreConn.Query<ABatchPicked>(sql, p).AsList();
                        if (PickLst.Count <= 0)
                        {
                            result.s = -6023;
                        }
                        else
                        {
                            IParam.OID = PickLst[0].OID;
                            IParam.SoID = PickLst[0].SoID;
                            if (asku.SkuType == 0)//检查售后是否重复
                            {
                                string iSql = @"SELECT ID FROM aftersaleitem WHERE CoID=@CoID AND OID=@OID AND BarCode=@BarCode";
                                var itemLst = CoreConn.Query<int>(iSql, new { CoID = IParam.CoID, BarCode = IParam.BarCode, OID = IParam.OID }).AsList();
                                if (itemLst.Count > 0)
                                {
                                    string msql = @"SELECT ID,IssueType,GoodsStatus FROM aftersale WHERE CoID=@CoID AND OID=@OID";
                                    var Safter = CoreConn.Query<AfterSale>(msql, new { CoID = IParam.CoID, OID = IParam.OID }).AsList();
                                    if (Safter.Count > 0)
                                    {
                                        if (Safter[0].GoodsStatus == "卖家已收到退货")
                                        {
                                            result.s = -6024;//重复收获
                                        }
                                        else
                                        {
                                            IParam.issueName = Enum.GetName(typeof(OrderE.IssueType), Safter[0].IssueType);//问题原因
                                            IParam.ASID = Safter[0].ID;
                                        }
                                    }
                                }
                            }
                        }
                        data.SkuAuto = asku;
                        data.OID = IParam.OID;
                        data.SoID = IParam.SoID;
                        data.ASID = IParam.ASID;
                        data.IssueName = IParam.issueName;
                        result.d = data;
                    }
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d = e.Message;
                }
            }
            return result;
        }
        #endregion

        #region 售后退回-产生收货单&更新库存
        public static DataResult SetSOAfter(ASaleAfterParam IParam)
        {
            var result = new DataResult(1, null);
            var CoreConn = new MySqlConnection(DbBase.CoreConnectString);
            CoreConn.Open();
            var CoreTrans = CoreConn.BeginTransaction();
            try
            {
                int inv_type = 1201;
                string CusType = "销售退货";
                //获取入库仓库
                result = CommHaddle.GetWhViewAll(IParam.CoID.ToString());
                var WhViewLst = result.d as List<Warehouse_view>;
                WhViewLst = WhViewLst.Where(a => a.Type == IParam.Type.ToString()).AsList();
                IParam.WhID = int.Parse(WhViewLst[0].ID);
                IParam.WhName = WhViewLst[0].WhName;
                //退货单
                var RecordID = "RT" + CommHaddle.GetRecordID(IParam.CoID);
                if (IParam.ASID > 0)
                {
                    CoreConn.Execute(UptAfterStatuSql(), new { CoID = IParam.CoID, ID = IParam.ASID, Modifier = IParam.Creator, ModifyDate = IParam.CreateDate }, CoreTrans);
                }
                else
                {
                    string psql = @"SELECT RealPrice FROM saleoutitem WHERE CoID=@CoID AND OID=@OID AND Skuautoid=@SkuID";
                    var Price = CoreConn.Query<decimal>(psql, new { CoID = IParam.CoID, OID = IParam.OID, SkuID = IParam.SkuAuto.Skuautoid }).AsList();
                    IParam.Price = Price[0];
                    var Amain = AddAfterSale(IParam, CoreTrans, CoreConn);
                    CoreConn.Execute(AddAfterSaleSql(), Amain, CoreTrans);
                    IParam.ASID = CoreConn.QueryFirst<int>("select LAST_INSERT_ID()", CoreTrans);
                    var AItem = AddAfterSaleItem(IParam);
                    CoreConn.Execute(AddAfterSaleItemSql(), AItem, CoreTrans);
                }
                if (result.s > 0)
                {

                    //更新WmsPile
                    string pilesql = "SELECT * FROM wmspile WHERE CoID = @CoID AND Skuautoid in @SkuID AND Type=4 AND WarehouseID=@WhID";
                    var pileLst = CoreConn.Query<AWmsPile>(pilesql, new { CoID = IParam.CoID, SkuID = IParam.SkuAuto.Skuautoid, WhID = IParam.WhID }, CoreTrans).AsList();
                    if (pileLst.Count > 0)
                    {
                        pileLst[0].Qty = pileLst[0].Qty + IParam.SkuAuto.Qty;
                        CoreConn.Execute("UPDATE wmspile SET Qty=@Qty WHERE CoID=@CoID AND ID=@ID", pileLst[0], CoreTrans);
                    }
                    else
                    {
                        var NewPile = new AWmsPile();
                        NewPile.Skuautoid = IParam.SkuAuto.Skuautoid;
                        NewPile.SkuID = IParam.SkuAuto.SkuID;
                        NewPile.PCode = "";
                        NewPile.WarehouseID = IParam.WhID;
                        NewPile.WarehouseName = WhViewLst[0].WhName;
                        NewPile.Type = int.Parse(WhViewLst[0].Type);
                        NewPile.Qty = IParam.SkuAuto.Qty;
                        NewPile.Creator = IParam.Creator;
                        NewPile.CreateDate = IParam.CreateDate;
                        NewPile.CoID = IParam.CoID;
                        CoreConn.Execute(APurHaddles.AddWmsPile(), NewPile, CoreTrans);
                    }
                    //新增操作记录
                    var log = new AWmslog();
                    log.BarCode = IParam.BarCode;
                    log.Skuautoid = IParam.SkuAuto.Skuautoid;
                    log.SkuID = IParam.SkuAuto.SkuID;
                    log.WarehouseID = IParam.WhID;
                    log.Qty = IParam.SkuAuto.Qty;
                    log.Contents = CusType;
                    log.Type = int.Parse(WhViewLst[0].Type);
                    log.RecordID = RecordID;
                    log.CoID = IParam.CoID;
                    log.Creator = IParam.Creator;
                    log.CreateDate = IParam.CreateDate;
                    CoreConn.Execute(APurHaddles.AddWmsLogSql(), log, CoreTrans);
                    //交易主表
                    var inv = new Invinout();
                    inv.RefID = IParam.ASID.ToString();
                    inv.OID = IParam.OID.ToString();
                    inv.RecordID = RecordID;
                    inv.Type = inv_type;
                    inv.CusType = CusType;
                    inv.Status = 1;
                    inv.WhID = WhViewLst[0].ParentID;
                    inv.LinkWhID = WhViewLst[0].ID;
                    inv.Creator = IParam.Creator;
                    inv.CreateDate = IParam.CreateDate;
                    inv.CoID = IParam.CoID.ToString();
                    CoreConn.Execute(InventoryHaddle.AddInvinoutSql(), inv, CoreTrans);
                    //交易子表
                    var inv_item = new Invinoutitem();
                    inv_item.RefID = IParam.ASID.ToString();
                    inv_item.IoID = RecordID;
                    inv_item.Type = inv_type;
                    inv_item.CusType = CusType;
                    inv_item.Status = 1;
                    inv_item.WhID = WhViewLst[0].ParentID;
                    inv_item.LinkWhID = WhViewLst[0].ID;
                    inv_item.Skuautoid = IParam.SkuAuto.Skuautoid;
                    inv_item.Qty = IParam.SkuAuto.Qty;//交易数量 
                    inv_item.Creator = IParam.Creator;
                    inv_item.CreateDate = IParam.CreateDate;
                    inv_item.CoID = IParam.CoID.ToString();
                    CoreConn.Execute(InventoryHaddle.AddInvinoutitemSql(), inv_item, CoreTrans);
                    var SkuIDLst = new List<int>();
                    SkuIDLst.Add(IParam.SkuAuto.Skuautoid);
                    //更新库存数量                                                                              
                    CoreConn.Execute(ASaleOutHaddles.UptInvSaleQtySql(), new { CoID = IParam.CoID, SkuIDLst = SkuIDLst, Modifier = IParam.Creator, ModifyDate = IParam.CreateDate }, CoreTrans);
                    //更新总库存数量
                    var res = CommHaddle.GetWareCoidList(IParam.CoID.ToString());
                    var CoIDLst = res.d as List<string>;
                    CoreConn.Execute(ASaleOutHaddles.UptInvMainSaleQtySql(), new { CoID = IParam.CoID, CoIDLst = CoIDLst, SkuIDLst = SkuIDLst, Modifier = IParam.Creator, ModifyDate = IParam.CreateDate }, CoreTrans);
                    CoreTrans.Commit();
                    CoreUser.LogComm.InsertUserLog(CusType, "saleout", "单据ID" + IParam.ASID, IParam.Creator, IParam.CoID, DateTime.Now);
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
            return result;
        }
        #endregion

        #region 更新售后单状态
        public static string UptAfterStatuSql()
        {
            string sql = @"UPDATE aftersale
                        SET GoodsStatus = '卖家已收到退货',
                        Modifier =@Modifier,
                        ModifyDate =@ModifyDate
                        WHERE
                            CoID =@CoID
                        AND ID =@ID";
            return sql;
        }
        #endregion
        #region 新增售后主表
        public static AfterSale AddAfterSale(ASaleAfterParam IParam, IDbTransaction Trans, MySqlConnection conn)
        {
            string odersql = @"SELECT ID,
                            SoID,
                            BuyerShopID,
                            RecName,
                            RecPhone,
                            ShopName,
                            Type,
                            ShopStatus,
                            Distributor
                            FROM order WHERE CoID=@CoID AND ID=@ID";
            var oderLst = conn.Query<Order>(odersql, new { CoID = IParam.CoID, ID = IParam.OID }).AsList();
            var after = new AfterSale();
            if (oderLst.Count > 0)
            {
                var oder = oderLst[0];
                after.CoID = IParam.CoID;
                after.OID = oder.ID;
                after.SoID = oder.SoID;
                after.RegisterDate = DateTime.Now;
                after.BuyerShopID = oder.BuyerShopID;
                after.RecName = oder.RecName;
                after.Type = oder.Type;
                after.RecTel = oder.RecTel;
                after.RecPhone = oder.RecPhone;
                after.SalerReturnAmt = IParam.Price;
                after.RealReturnAmt = IParam.Price;
                after.ShopID = oder.ShopID;
                after.ShopName = oder.ShopName;
                after.WarehouseID = IParam.WhID;
                after.RecWarehouse = IParam.WhName;
                after.IssueType = (int)Enum.Parse(typeof(OrderE.IssueType), IParam.issueName, true);
                after.OrdType = oder.Type;
                after.Status = 0;
                after.ShopStatus = oder.ShopStatus;
                after.GoodsStatus = "卖家已收到退货";
                after.ExCode = IParam.ExCode;
                after.Distributor = oder.Distributor;
                after.Creator = IParam.Creator;
                after.CreateDate = DateTime.Now;
            }
            return after;
        }
        #endregion

        #region 
        public static string AddAfterSaleSql()
        {
            string sql = @" INSERT INTO 
                                (OID,
                                SoID,
                                RegisterDate,
                                BuyerShopID,
                                RecName,
                                Type,
                                RecTel,
                                RecPhone,
                                SalerReturnAmt,
                                RealReturnAmt,
                                ShopID,
                                ShopName,
                                WarehouseID,
                                RecWarehouse,
                                IssueType,
                                OrdType,
                                `Status`,
                                ShopStatus,
                                GoodsStatus,
                                RefundStatus,
                                Result,
                                Express,
                                ExCode,
                                Distributor,
                                CoID,
                                Creator,
                                CreateDate) 
                            VALUES
                            (
                                @OID,
                                @SoID,
                                @RegisterDate,
                                @BuyerShopID,
                                @RecName,
                                @Type,
                                @RecTel,
                                @RecPhone,
                                @SalerReturnAmt,
                                @RealReturnAmt,
                                @ShopID,
                                @ShopName,
                                @WarehouseID,
                                @RecWarehouse,
                                @IssueType,
                                @OrdType,
                                @Status,
                                @ShopStatus,
                                @GoodsStatus,
                                @RefundStatus,
                                @Result,
                                @Express,
                                @ExCode,
                                @Distributor,
                                @CoID,
                                @Creator,
                                @CreateDate
                            )";
            return sql;
        }
        #endregion

        #region 新增售后子表
        public static ASaleAfterItem AddAfterSaleItem(ASaleAfterParam IParam)
        {
            var Aitem = new ASaleAfterItem();
            Aitem.CoID = IParam.CoID;
            Aitem.OID = IParam.OID;
            Aitem.SoID = IParam.SoID;
            Aitem.ReturnType = 0;
            Aitem.BarCode = IParam.SkuAuto.BarCode;
            Aitem.SkuID = IParam.SkuAuto.SkuID;
            Aitem.SkuName = IParam.SkuAuto.SkuName;
            Aitem.Norm = IParam.SkuAuto.Norm;
            Aitem.GoodsCode = IParam.SkuAuto.GoodsCode;
            Aitem.RegisterQty = 1;
            Aitem.ReturnQty = 1;
            Aitem.Price = IParam.Price;
            Aitem.Amount = IParam.Price;
            Aitem.Creator = IParam.Creator;
            Aitem.CreateDate = IParam.CreateDate;
            Aitem.RID = IParam.ASID;
            return Aitem;
        }

        public static string AddAfterSaleItemSql()
        {
            string sql = @"INSERT INTO aftersaleitem
                                (RID,
                                ReturnType,
                                BarCode,
                                SkuAutoID,
                                SkuID,
                                SkuName,
                                Norm,
                                GoodsCode,
                                RegisterQty,
                                ReturnQty,
                                Price,
                                Amount,
                                CoID,
                                Creator,
                                CreateDate) VALUES (
                                            @RID,
                                            @ReturnType,
                                            @BarCode,
                                            @SkuAutoID,
                                            @SkuID,
                                            @SkuName,
                                            @Norm,
                                            @GoodsCode,
                                            @RegisterQty,
                                            @ReturnQty,
                                            @Price,
                                            @Amount,
                                            @CoID,
                                            @Creator,
                                            @CreateDate
                                            )";
            return sql;
        }
        #endregion
    }
}