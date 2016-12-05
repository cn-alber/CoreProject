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
    public static class AShelvesHaddles
    {
        /// <summary>
        /// 扫条码获取Sku信息及默认货位
        /// </summary>
        public static DataResult GetUpBoxSku(AShelfParam IParam)
        {
            var result = new DataResult(1, null);
            var data = new AShelfData();
            var cp = new ASkuScanParam();
            cp.CoID = IParam.CoID;
            cp.BarCode = IParam.BoxCode;
            result = ASkuScanHaddles.GetType(cp);
            if (result.s > 0)
            {
                data.SkuAuto = result.d as ASkuScan;
                IParam.Skuautoid = data.SkuAuto.Skuautoid;
                IParam.Qty = data.SkuAuto.Qty;
            }
            result = GetUpPCode(IParam);
            if (result.s > 0)
            {
                data.PileAuto = result.d as AWmsPileAuto;
            }
            if (result.s > 0)
            {
                result.d = data;
            }
            return result;
        }

        /// <summary>
        /// 上架货位,默认货位,货物MaxQty
        /// </summary>
        public static DataResult GetUpPCode(AShelfParam IParam)
        {
            var result = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    var PCodeSql1 = @" SELECT
                                            wmspile.WarehouseID,
                                            wmspile.SkuID,
                                            wmspile.PCode,
                                            wmspile.MaxQty,
                                            wmspile.Qty,
                                            wmspile.Type,
                                            wmspile.ID,
                                            wmspile.Order
                                        FROM
                                            wmspile
                                        WHERE                                            
                                            wmspile.Skuautoid = @SkuID
                                        AND wmspile.CoID = @CoID
                                        AND wmspile.maxqty - wmspile.qty >= @BoxQty
                                        AND (
                                            wmspile.WarehouseID = @WarehouseID
                                            AND @WarehouseID > 0
                                            OR @WarehouseID = 0
                                        )
                                        AND (
                                            wmspile.PCode = @PCode
                                            AND IFNULL(@PCode, '') <> ''
                                            OR IFNULL(@PCode, '') = ''
                                        )";
                    var PCodeSql2 = @" SELECT
                                                wmspile.WarehouseID,
                                                wmspile.SkuID,
                                                wmspile.PCode,
                                                wmspile.MaxQty,
                                                wmspile.Qty,
                                                wmspile.Type,
                                                wmspile.ID,
                                                wmspile.Order
                                            FROM
                                                wmspile
                                            WHERE
                                                wmspile.Skuautoid = 0
                                            AND wmspile.CoID = @CoID
                                            AND wmspile.maxqty - wmspile.qty >= @BoxQty
                                            AND (
                                                wmspile.WarehouseID = @WarehouseID
                                                AND @WarehouseID > 0
                                                OR @WarehouseID = 0
                                            )
                                            AND (
                                                wmspile.PCode = @PCode
                                                AND IFNULL(@PCode, '') <> ''
                                                OR IFNULL(@PCode, '') = ''
                                            )
                      ";
                    var p = new DynamicParameters();
                    p.Add("@SkuID", IParam.Skuautoid);
                    p.Add("@CoID", IParam.CoID);
                    p.Add("@WarehouseID", IParam.WarehouseID);
                    p.Add("@BoxQty", IParam.Qty);
                    p.Add("@PCode", IParam.PCode);
                    if (IParam.Type > 0)
                    {
                        PCodeSql1 = PCodeSql1 + " AND wmspile.Type=@Type";
                        PCodeSql2 = PCodeSql2 + " AND wmspile.Type=@Type";
                        p.Add("@Type", IParam.Type);
                    }
                    if (IParam.TypeLst.Count > 0)
                    {
                        PCodeSql1 = PCodeSql1 + " AND wmspile.Type in @TypeLst";
                        PCodeSql2 = PCodeSql2 + " AND wmspile.Type in @TypeLst";
                        p.Add("@TypeLst", IParam.TypeLst);
                    }
                    var PCodeSql = @"SELECT A.WarehouseID,A.SkuID,A.PCode,A.maxqty,A.Type,A.ID
                                    FROM
                                        ( " + PCodeSql1 + " UNION " + PCodeSql2 + " ) AS A ORDER BY A.Order";
                    var PCLst = conn.Query<AWmsPileAuto>(PCodeSql, p).AsList();
                    if (PCLst.Count > 0)
                    {
                        result.d = PCLst[0];
                    }
                    else
                    {
                        result.s = -6002;
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

        /// <summary>
        /// Sku上架
        /// </summary>
        public static DataResult SetUpShelfPile(AShelfData IParam)
        {
            var result = new DataResult(1, null);
            var CoreConn = new MySqlConnection(DbBase.CoreConnectString);
            CoreConn.Open();
            var CoreTrans = CoreConn.BeginTransaction();
            try
            {
                var asku = IParam.SkuAuto;
                //更新WmsPile
                string SubPileSql = "SELECT CoID,ID,Qty,WarehouseID,Type FROM wmspile WHERE CoID = @CoID AND Skuautoid = @Skuautoid AND Type=4";
                string AddPileSql = "SELECT CoID,ID,Qty,WarehouseID,Type FROM wmspile WHERE CoID = @CoID AND ID=@ID";
                var subpileLst = CoreConn.Query<AWmsPileAuto>(SubPileSql, new { CoID = IParam.CoID, Skuautoid = asku.Skuautoid }, CoreTrans).AsList();
                var addpileLst = CoreConn.Query<AWmsPileAuto>(AddPileSql, new { CoID = IParam.CoID, ID = IParam.PileID }, CoreTrans).AsList();
                if (subpileLst.Count <= 0 || addpileLst.Count <= 0)
                {
                    result.s = -6003;//无此SKU库存信息
                }
                else
                {
                    subpileLst[0].Qty = subpileLst[0].Qty - asku.Qty;
                    addpileLst[0].Qty = addpileLst[0].Qty + asku.Qty;
                    var UptLst = new List<AWmsPileAuto>();
                    UptLst.Add(subpileLst[0]);
                    UptLst.Add(addpileLst[0]);
                    CoreConn.Execute(UptPileQtySql(), UptLst, CoreTrans);
                    //添加上架操作记录
                    string Contents = "货品上架";
                    var logLst = new List<AWmslog>();
                    if (asku.SkuType < 2)
                    {
                        var log = new AWmslog();
                        log.BarCode = asku.BarCode;
                        log.Skuautoid = asku.Skuautoid;
                        log.SkuID = asku.SkuID;
                        log.WarehouseID = addpileLst[0].WarehouseID;
                        log.Qty = asku.Qty;
                        log.Contents = Contents;
                        log.Type = addpileLst[0].Type;
                        log.CoID = IParam.CoID;
                        log.Creator = IParam.Creator;
                        log.CreateDate = IParam.CreateDate;
                        logLst.Add(log);
                    }
                    else
                    {
                        string boxsql = "SELECT BarCode,Skuautoid,SkuID,BoxCode,Qty FROM wmsbox WHERE CoID=@CoID AND BoxCode = @BoxCode";
                        var BoxSkuLst = CoreConn.Query<AWmsBox>(boxsql, new { CoID = IParam.CoID, BoxCode = asku.BarCode }).AsList();
                        if (BoxSkuLst.Count > 0)
                        {
                            var logLstA = BoxSkuLst.Select(a => new AWmslog
                            {
                                BarCode = a.BarCode,
                                Skuautoid = a.Skuautoid,
                                SkuID = a.SkuID,
                                BoxCode = a.BoxCode,
                                WarehouseID = addpileLst[0].WarehouseID,
                                Qty = a.Qty,
                                Contents = Contents,
                                Type = addpileLst[0].Type,
                                CoID = IParam.CoID,
                                Creator = IParam.Creator,
                                CreateDate = IParam.CreateDate
                            }).AsList();
                            logLst.AddRange(logLstA);
                        }
                        if (logLst.Count > 0)
                        {
                            CoreConn.Execute(APurHaddles.AddWmsLogSql(), logLst, CoreTrans);
                        }
                        CoreTrans.Commit();
                    }
                    //更新库存数量      
                    var SkuIDLst = new List<int>();
                    SkuIDLst.Add(asku.Skuautoid);
                    CoreConn.Execute(UptInvWaitInQtySql(), new { CoID = IParam.CoID, SkuIDLst = SkuIDLst, Modifier = IParam.Creator, ModifyDate = IParam.CreateDate }, CoreTrans);
                    //更新总库存数量
                    var res = CommHaddle.GetWareCoidList(IParam.CoID.ToString());
                    var CoIDLst = res.d as List<string>;
                    CoreConn.Execute(UptInvMainWaitInQtySql(), new { CoID = IParam.CoID, CoIDLst = CoIDLst, SkuIDLst = SkuIDLst, Modifier = IParam.Creator, ModifyDate = IParam.CreateDate }, CoreTrans);
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


        /// <summary>
        /// 检查货位库存
        /// </summary>
        // public static DataResult CheckWhPCode(AShelfParam IParam)
        // {
        //     var result = new DataResult(1, null);
        //     using (var conn = new MySqlConnection(DbBase.CoreConnectString))
        //     {
        //         try
        //         {
        //             var res = GetUpPCode(IParam);
        //             if (res.s < 0)
        //             {
        //                 result.s = res.s;
        //                 result.d = res.d;
        //             }
        //             else
        //             {
        //                 var dic = new Dictionary<string, string>();

        //             }
        //         }
        //         catch (Exception e)
        //         {
        //             result.s = -1;
        //             result.d = e.Message;
        //         }
        //     }
        //     return result;
        // }



        public static string UptPileQtySql()
        {
            string sql = "UPDATE wmspile SET Qty=@Qty WHERE CoID=@CoID AND ID=@ID";
            return sql;
        }
        #region 更新仓库待入库存
        public static string UptInvWaitInQtySql()
        {
            string sql = @"UPDATE inventory 
                            SET WaitInQty = (SELECT IFNULL(SUM(Qty),0)
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
        public static string UptInvMainWaitInQtySql()
        {
            string sql = @"UPDATE inventory_sale
                            SET inventory_sale.WaitInQty = (
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


