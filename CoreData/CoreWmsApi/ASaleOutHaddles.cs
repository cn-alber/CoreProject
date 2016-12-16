using System.Collections.Generic;
using System.Linq;
using System.Data;
using CoreModels;
// using CoreModels.XyUser;
// using CoreModels.Enum;
// using CoreModels.XyComm;
using CoreModels.XyCore;
using CoreModels.WmsApi;
using CoreData.CoreComm;
// using CoreData.CoreCore;
using Dapper;
using MySql.Data.MySqlClient;
using System;
// using System.Text;
namespace CoreData.CoreWmsApi
{
    public static class ASaleOutHaddles
    {
        #region 获取单件发货-批次任务
        public static DataResult OutBatch(ASaleParams IParam)
        {
            var result = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    string sql = @"SELECT
                                        batch.ID
                                    FROM
                                        batch
                                    WHERE
                                        CoID =@CoID
                                    AND `Status` = 8
                                    AND Type =@Type";
                    var BatchIDLst = conn.Query<int>(sql, new { CoID = IParam.CoID, Type = IParam.BatchType }).AsList();
                    if (BatchIDLst.Count <= 0)
                    {
                        result.s = -6015;//未获取待出货批次
                    }
                    else
                    {
                        result.d = BatchIDLst;
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


        #region 一单一件-扫描件码-获取待发货任务Sku
        public static DataResult SaleOutScanBySku(ASaleParams IParam)
        {
            var result = new DataResult(1, null);
            var data = new ASaleOutData();
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {//8.等待发货，0.一单一件                    
                    string bsql = @"SELECT ID FROM batch WHERE CoID=@CoID AND Status=8 AND Type = 0";
                    var p = new DynamicParameters();
                    p.Add("@CoID", IParam.CoID);
                    if (IParam.BatchID > 0)
                    {
                        bsql = bsql + " AND ID=@BatchID";
                        p.Add("@BatchID", IParam.BatchID);
                    }
                    var BatchIDLst = conn.Query<int>(bsql).AsList();
                    if (BatchIDLst.Count <= 0)
                    {
                        result.s = -6015;//未获取待出货批次
                    }
                    else
                    {
                        string sql = @"SELECT ID,
                                    BatchID,
                                    OID,
                                    SoID,
                                    BarCode,
                                    Skuautoid,
                                    Qty,
                                    `Status`,
                                    OutID
                                    BatchtaskID,
                                    SortCode 
                                WHERE CoID=@CoID 
                                AND BarCode=@BarCode
                                AND BatchID in @BatchIDLst";
                        var PickLst = conn.Query<ABatchPicked>(sql, new { CoID = IParam.CoID, BarCode = IParam.BarCode, BatchIDLst = BatchIDLst }).AsList();
                        if (PickLst.Count <= 0)
                        {
                            result.s = -6000;//无效条码
                        }
                        else
                        {
                            var PLst = PickLst.Where(a => a.OutID == 0).AsList();
                            if (PLst.Count <= 0)
                            {
                                result.s = -6016;//重复发货
                            }
                            else
                            {
                                IParam.BatchID = PLst[0].BatchID;//批次任务
                                var cp = new ASkuScanParam();
                                cp.CoID = IParam.CoID;
                                cp.BarCode = IParam.BarCode;
                                ASkuScanHaddles.GetType(cp);
                                if (result.s > 0)
                                {
                                    data.SkuAuto = result.d as ASkuScan; //扫描件码
                                    IParam.Skuautoid = data.SkuAuto.Skuautoid;
                                    result = GetOutOrder(IParam);
                                    if (result.s > 0)
                                    {
                                        data.OItemAuto = result.d as OutItemBatch;//匹配订单出货信息
                                        data.ID = PLst[0].ID;
                                    }
                                }
                            }
                        }
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

        #region 匹配批次内Sku发货明细
        public static DataResult GetOutOrder(ASaleParams IParam)
        {
            var result = new DataResult(1, null);
            using (var CoreConn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    var FSql = @"SELECT
                                    saleout.BatchID,
                                    saleout.ID AS OutID,
                                    saleoutitem.OID,
                                    saleoutitem.SoID,
                                    saleoutitem.SkuID,
                                    saleoutitem.Qty AS ItemQty,
                                    saleout.ExCode,
                                    saleout.ExpName,
                                    saleout.IsExpPrint
                                FROM
                                    saleout,
                                    saleoutitem,
                                    batch
                                WHERE
                                    saleout.CoID = saleoutitem.CoID
                                AND saleout.OID = saleoutitem.OID
                                AND saleout.`Status` = 0
                                AND saleoutitem.SkuAutoID = @SkuID
                                AND saleoutitem.CoID = @CoID
                                AND (
                                    (
                                        saleout.BatchID = @BatchID
                                        AND @BatchID > 0
                                    )
                                    OR @BatchID = 0
                                )
                                AND saleout.CoID = batch.CoID
                                AND saleout.BatchID = batch.ID
                                AND batch.Type = 0
                                AND 1 > (
                                    SELECT
                                        IFNULL(SUM(batchpicked.Qty),0)
                                    FROM
                                        batchpicked
                                    WHERE
                                        batchpicked.CoID = saleoutitem.CoID
                                    AND batchpicked.Skuautoid = saleoutitem.Skuautoid
                                    AND batchpicked.BatchID = saleout.BatchID
                                    AND batchpicked.OutID = saleout.ID
                                )
                                ORDER BY
                                    saleoutitem.ID
                                LIMIT 1";
                    var p = new DynamicParameters();
                    p.Add("@CoID", IParam.CoID);
                    p.Add("@SkuID", IParam.Skuautoid);
                    p.Add("@BatchID", IParam.BatchID);
                    var itemLst = CoreConn.Query<OutItemBatch>(FSql, p).AsList();
                    if (itemLst.Count > 0)
                    {
                        result.d = itemLst[0];
                    }
                    else
                    {
                        result.s = -6015;//未获取待出货任务
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

        #region 单件发货-更新库存
        public static DataResult SaleOutSingle(ASaleOutSet IParam)
        {
            var result = new DataResult(1, null);
            var RecordID = "SL" + CommHaddle.GetRecordID(IParam.CoID);
            var CoreConn = new MySqlConnection(DbBase.CoreConnectString);
            CoreConn.Open();
            var CoreTrans = CoreConn.BeginTransaction();
            try
            {
                var asku = IParam.SkuAuto;
                var aout = IParam.OItemAuto;
                string invsql = @"SELECT IFNULL(StockQty,0) FROM inventory WHERE CoID=@CoID AND Skuautoid=@SkuID AND StockQty>0";
                var invLst = CoreConn.Query<int>(invsql, new { CoID = IParam.CoID, SkuID = asku.Skuautoid },CoreTrans).AsList();
                if(invLst.Count<=0)
                {
                    result.s=-6017;//库存不足
                }
                else if(invLst[0]<asku.Qty)
                {
                    result.s=-6017;//库存不足
                }
                else
                {                    
                    //更新拣货资料
                    string picksql = @"UPDATE batchpicked
                                        SET OutID =@OutID,
                                            OID =@OID,
                                            SoID =@SoID,
                                            ExCode =@ExCode,
                                            Express =@Express,
                                            `Status`=2
                                        WHERE CoID=@CoID
                                        AND ID=@ID";
                    var p = new DynamicParameters();
                    p.Add("@CoID",IParam.CoID);
                    p.Add("@ID",IParam.ID);
                    p.Add("@OutID",aout.OutID);
                    p.Add("@OID",aout.OID);
                    p.Add("@SoID",aout.SoID);
                    p.Add("@ExCode",aout.ExCode);
                    p.Add("@Express",aout.ExpName);
                    int c = CoreConn.Execute(picksql,p,CoreTrans);
                    // string bosql=@"SELECT
                    //                     IFNULL(SUM(Qty), 0)
                    //                 FROM
                    //                     batchpicked
                    //                 WHERE
                    //                     CoID =@CoID
                    //                 AND BatchID =@BatchID
                    //                 AND OutID>0";
                    string bnsql=@"SELECT
                                        IFNULL(SUM(Qty), 0)
                                    FROM
                                        batchpicked
                                    WHERE
                                        CoID =@CoID
                                    AND BatchID =@BatchID
                                    AND OutID=0";
                    // int bchout = CoreConn.Query<int>(bosql,new{CoID=IParam.CoID,BatchID=aout.BatchID}).First();
                    int bchNout = CoreConn.Query<int>(bnsql,new{CoID=IParam.CoID,BatchID=aout.BatchID}).First();
                    if(bchNout==0)
                    {//6.已完成
                        CoreConn.Execute("UPDATE batch SET `Status`=6 WHERE CoID=@CoID AND BatchID=@BatchID",new{CoID=IParam.CoID,BatchID=aout.BatchID},CoreTrans);
                    }
                    //更新发货状态
                    string osql=@"UPDATE saleout SET `Status`=1 WHERE CoID=@CoID AND ID=@OutID";
                    CoreConn.Execute(osql,new{CoID=IParam.CoID,OutID=aout.OutID},CoreTrans);
                    //更新订单状态
                    string dsql = @"UPDATE saleout SET `Status`=4 WHERE CoID=@CoID AND ID=@OID";
                    CoreConn.Execute(dsql,new{CoID=IParam.CoID,OID=aout.OID},CoreTrans);
                    
                }
            }
            catch (Exception e)
            {
                result.s = -1;
                result.d = e.Message;
            }
            return result;
        }
        #endregion


    }
}