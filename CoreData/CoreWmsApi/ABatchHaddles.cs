using System.Collections.Generic;
using System.Linq;
using System.Data;
using CoreModels;
// using CoreModels.XyUser;
// using CoreModels.Enum;
// using CoreModels.XyComm;
using CoreModels.XyCore;
using CoreModels.WmsApi;
// using CoreData.CoreComm;
// using CoreData.CoreCore;
using Dapper;
using MySql.Data.MySqlClient;
using System;
// using System.Text;
namespace CoreData.CoreWmsApi
{
    public static class ABatchHaddles
    {
        #region 获取批次任务
        public static DataResult GetBatchTask(ABatchParams IParam)
        {
            var result = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    List<int> ABatchLst = new List<int>();
                    var batchsql = @"
                            SELECT
	                            DISTINCT batchtask.BatchID
                            FROM
	                            batch,
	                            batchtask
                            WHERE
	                            batch.CoID = batchtask.CoID
                            AND batch.ID = batchtask.BatchID
                            AND (batch.status=0 OR batch.status=3)
                            AND batchtask.Qty>(batchtask.PickQty+batchtask.NoQty)
                            AND batch.CoID = @CoID
                            AND batch.Type = @Type
                            AND batch.PickorID = @Pickor
                            ";
                    var p = new DynamicParameters();
                    p.Add("@CoID", IParam.CoID);
                    p.Add("@Type", IParam.Type);
                    p.Add("@Pickor", IParam.Pickor);
                    ABatchLst = conn.Query<int>(batchsql, p).AsList();
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

        #region 根据任务ID获取批次任务
        public static DataResult GetBatchTaskByID(ABatchParams IParam)
        {
            var result = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    var batchsql = @"SELECT
                                            ID,
                                            WarehouseID,
                                            WarehouseName
                                        FROM
                                            batch
                                        WHERE
                                            CoID =@CoID
                                        AND Type =@Type
                                        AND PickorID = @Pickor
                                        AND (`Status` = 0 OR `Status` = 3)
                                        AND (
                                            batch.ID = @BID
                                            AND @BID > 0
                                            OR @BID = 0
                                        )
                                    ";
                    var batchLst = conn.Query<ABatch>(batchsql, new { CoID = IParam.CoID, Type = IParam.Type, Picker = IParam.Pickor, BID = IParam.BatchID }).AsList();
                    if (batchLst.Count > 0)
                    {
                        var BatchIDLst = batchLst.Select(a => a.ID).AsList();
                        var tasksql = @" SELECT
                                            ID,
                                            BatchID,
                                            Skuautoid,
                                            PCode,
                                            Qty,
                                            PickQty,
                                            NoQty
                                        FROM
                                            batchtask
                                        WHERE
                                            CoID =@CoID
                                        AND Qty > (PickQty + NoQty)
                                        AND BatchID IN BatchIDLst
                                        ORDER BY ID
                                        LIMIT 1";
                        var taskLst = conn.Query<ABatchTask>(tasksql, new { CoID = IParam.CoID, BatchIDLst = BatchIDLst }).AsList();
                        if (taskLst.Count > 0)
                        {
                            var task = taskLst[0];
                            task.WarehouseID = batchLst[0].WarehouseID;
                            task.WarehouseName = batchLst[0].WarehouseName;
                            var pileLst = conn.Query<ASkuPileQty>("SELECT SkuID,PCode,Qty FROM wmspile WHERE CoID=@CoID AND PCode=@PCode", new { CoID = IParam.CoID, PCode = task.PCode }).AsList();
                            var skuLst = conn.Query<ASkuScan>("SELECT Skuautoid,SkuID,SkuName,GoodsCode,Norm FROM coresku WHERE CoID=@CoID AND @Skuautoid=@Skuautoid", new { CoID = IParam.CoID, Skuautoid = task.Skuautoid }).AsList();
                            if (pileLst.Count > 0 && skuLst.Count > 0)
                            {
                                task.GoodsCode = skuLst[0].GoodsCode;
                                task.Sku = skuLst[0].SkuID;
                                task.SkuName = skuLst[0].SkuName;
                                task.Norm = skuLst[0].Norm;
                                task.PCodeSku = pileLst[0].SkuID;
                                task.PCodeQty = pileLst[0].Qty;
                            }
                            result.d = task;
                        }
                        else
                        {
                            result.s = -1;
                        }
                    }
                    else
                    {
                        result.s = -1;
                    }
                    if (result.s < 0)
                    {
                        if (IParam.BatchID > 0)
                        {
                            result.s = -6006;//任务已完成
                        }
                        else
                        {
                            result.s = -6005;//获取拣货任务失败
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

        #region 获取批次任务数量
        public static DataResult GetBatchNum(ABatchParams IParam)
        {
            var result = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    var batchsql = @"SELECT batch.Type,
                                            COUNT(DISTINCT batch.ID) AS Num
                                        FROM
                                            batch
                                        WHERE batch.CoID = @CoID
                                        AND (batch.status=0 OR batch.status=3)
                                        AND batch.Qty>(batch.PickQty+batch.NoQty)
                                        AND batch.PickorID = @Pickor
                                        GROUP BY
                                            batch.Type
                                        ";
                    var TypeNumLst = conn.Query<TypeNum>(batchsql, new { CoID = IParam.CoID, Pickor = IParam.Pickor }).AsList();
                    if (TypeNumLst.Count > 0)
                    {
                        result.d = TypeNumLst;
                    }
                    else
                    {
                        result.s = -6005;//未获取拣货任务
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

        #region 拣货下架&更新状态
        public static DataResult BatchOffShelf(AShelfSet IParam, IDbTransaction Trans, MySqlConnection conn)
        {
            var res = new DataResult(1, null);
            var p = new DynamicParameters();
            p.Add("@CoID", IParam.CoID);
            p.Add("@BatchID", IParam.BatchID);
            string batsql = @"SELECT
                                batch.ID,
                                batch.`Status`
                            FROM
                                batch
                            WHERE
                                CoID =@CoID
                            AND ID =@BatchID";
            var batchLst = conn.Query<ABatchAuto>(batsql, p, Trans).AsList();
            if (batchLst.Count > 0)
            {
                if (batchLst[0].Status == 7)
                {
                    res.s = -6008;//任务已终止,拣货失败                    
                }
                else
                {
                    //回填拣货数量
                    string upttasksql = @"UPDATE batchtask
                                SET PickQty = PickQty +@Qty
                                WHERE
                                    CoID =@CoID
                                AND ID =@ID";
                    int count = conn.Execute(upttasksql, new { CoID = IParam.CoID, Qty = IParam.SkuAuto.Qty }, Trans);
                    if (count <= 0)
                    {
                        res.s = -6007;//拣货失败
                    }
                    else
                    {

                        var autoLst = conn.Query<ABatchAuto>(BatchAutoSql(), new { CoID = IParam.CoID, BatchID = IParam.BatchID }, Trans).AsList();
                        if (autoLst.Count > 0)
                        {
                            var bth = autoLst[0];
                            if (bth.Qty > bth.PickQty + bth.NoQty)
                            {
                                bth.Status = 3;//1.正在拣货
                            }
                            else
                            {
                                if (IParam.BatchType != 1)//1.一单多件
                                {
                                    bth.Status = 8;//等待发货
                                }
                                // if (IParam.BatchType == 1)//1.一单多件
                                //     bth.Status = 1;//2:等待分拣
                                // else
                                //     bth.Status = 2;//4:等待发货
                            }
                            string uptsql2 = @"UPDATE batch
                                            SET PickedQty =@PickQty,
                                                NoQty =@NoQty,
                                                `Status` =@Status
                                            WHERE
                                                CoID =@CoID
                                            AND BatchID =@BatchID";
                            conn.Execute(uptsql2, bth, Trans);
                        }
                    }
                }
            }
            else
            {
                res.s = -6005;//未获取拣货任务
            }
            return res;
        }
        #endregion

        #region 新增拣货记录
        public static DataResult AddBatchPicked(AShelfSet IParam, List<AWmslog> LogLst, IDbTransaction Trans, MySqlConnection conn)
        {
            var res = new DataResult(1, null);
            var PinckLst = new List<ABatchPicked>();
            foreach (var log in LogLst)
            {
                var p = new ABatchPicked();
                p.CoID = log.CoID;
                p.BarCode = log.BarCode;
                p.Skuautoid = log.Skuautoid;
                p.Sku = log.SkuID;
                p.BatchID = IParam.BatchID;
                p.BatchtaskID = IParam.BatchtaskID;
                p.Creator = IParam.Creator;
                p.CreatDate = IParam.CreateDate;
                PinckLst.Add(p);
            }
            if (conn.Execute(AddBatchPickSql(), PinckLst, Trans) <= 0)
            {
                res.s = -6007;
            }
            return res;
        }
        #endregion

        #region 手动结单&标记缺货
        public static DataResult SetBatchNoQty(ABatchParams IParam)
        {
            var result = new DataResult(1, null);
            var CoreConn = new MySqlConnection(DbBase.CoreConnectString);
            CoreConn.Open();
            var CoreTrans = CoreConn.BeginTransaction();
            try
            {
                var p = new DynamicParameters();
                p.Add("@CoID", IParam.CoID);
                p.Add("@BatchID", IParam.BatchID);
                string batsql = @"SELECT
                                batch.ID,
                                batch.`Status`
                            FROM
                                batch
                            WHERE
                                CoID =@CoID
                            AND ID =@BatchID";
                var batchLst = CoreConn.Query<ABatchAuto>(batsql, p, CoreTrans).AsList();
                if (batchLst.Count > 0)
                {
                    if (batchLst[0].Status == 7)
                    {
                        result.s = -6008;//任务已终止,拣货失败                    
                    }
                    else
                    {

                        //拣货任务明细更新
                        string UptSql = @"UPDATE batchtask
                                SET NoQty = Qty - PickQty
                                WHERE
                                    CoID =@CoID
                                AND ID =@ID";

                        int count = CoreConn.Execute(UptSql, new { CoID = IParam.CoID, ID = IParam.ID }, CoreTrans);
                        if (count <= 0)
                        {
                            result.s = -6010;//任务终止失败
                        }
                        else
                        {
                            var autoLst = CoreConn.Query<ABatchAuto>(BatchAutoSql(), new { CoID = IParam.CoID, BatchID = IParam.BatchID }, CoreTrans).AsList();
                            if (autoLst.Count > 0)
                            {
                                var bth = autoLst[0];
                                if (bth.Qty > bth.PickQty + bth.NoQty)
                                {
                                    bth.Status = 3;//1.正在拣货
                                }
                                else
                                {
                                    if (IParam.Type != 1)//1.一单多件
                                    {
                                        bth.Status = 8;//等待发货
                                    }
                                    // if (IParam.Type == 1)//1.一单多件
                                    //     bth.Status = 2;//2:等待分拣
                                    // else
                                    //     bth.Status = 4;//4:等待发货
                                }
                                string uptsql2 = @"UPDATE batch
                                            SET PickedQty =@PickQty,
                                                NoQty =@NoQty,
                                                `Status` =@Status
                                            WHERE
                                                CoID =@CoID
                                            AND BatchID =@BatchID";
                                CoreConn.Execute(uptsql2, bth, CoreTrans);
                                CoreTrans.Commit();
                            }
                        }
                    }
                }
                else
                {
                    result.s = -6005;//未获取拣货任务
                }
                return result;
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

        #region 订单分解-获取件信息&推荐格号
        public static DataResult GetSortCode(ABatchParams IParam)
        {
            var result = new DataResult(1, null);
            var data = new ABatchPickData();
            using (var CoreConn = new MySqlConnection(DbBase.CoreConnectString))
            {
                string bsql = @"SELECT ID FROM batch WHERE CoID=@CoID AND Status=3 AND Type = 1";
                var BatchIDLst = CoreConn.Query<int>(bsql).AsList();
                if (BatchIDLst.Count <= 0)
                {
                    result.s = -6011;//无分拣任务
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
                    var PickLst = CoreConn.Query<ABatchPicked>(sql, new { CoID = IParam.CoID, BarCode = IParam.BarCode, BatchIDLst = BatchIDLst }).AsList();
                    if (PickLst.Count <= 0)
                    {
                        result.s = -6000;//无效条码
                    }
                    else
                    {
                        var PLst = PickLst.Where(a => a.Status == 0).AsList();
                        if (PLst.Count <= 0)
                        {
                            result.s = -6012;//已分配格号
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
                                result = GetPickedOrder(IParam);
                                if (result.s > 0)
                                {
                                    data.OItemAuto = result.d as OrderItemBatch;//匹配订单分拣信息
                                    data.ID = PLst[0].ID;
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }
        #endregion

        #region 订单分拣-更新分拣格 
        public static DataResult SetSortCode(ABatchParams IParam)
        {
            var result = new DataResult(1, null);
            var CoreConn = new MySqlConnection(DbBase.CoreConnectString);
            CoreConn.Open();
            var CoreTrans = CoreConn.BeginTransaction();
            try
            {
                string sql = @"SELECT ID,
                                    BatchID,
                                    OID,
                                    SoID,
                                    `Status`,
                                    SortCode,
                                    CoID 
                                FROM batchpicked
                                WHERE CoID=@CoID 
                                AND ID=@ID";
                var PickLst = CoreConn.Query<ABatchPicked>(sql, new { CoID = IParam.CoID, ID = IParam.ID }, CoreTrans).AsList();
                if (!string.IsNullOrEmpty(PickLst[0].SortCode))
                {
                    result.s = -6012;//已分配格号
                }
                else
                {
                    //出货主表绑定分拣格
                    PickLst[0].OID = IParam.OID;
                    PickLst[0].SoID = IParam.SoID;
                    PickLst[0].SortCode = IParam.SortCode;
                    PickLst[0].Status = 1;
                    string psql = @"UPDATE batchpicked
                                    SET OID =@OID,
                                        SoID =@SoID,
                                        SortCode =@SortCode
                                    WHERE
                                        CoID =@CoID
                                    AND ID =@ID";
                    int count = CoreConn.Execute(psql, PickLst[0], CoreTrans);
                    if (count < 0)
                    {
                        result.s = -6013;
                    }
                    else
                    {
                        CoreTrans.Commit();
                    }
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


        #region 获取绑定订单(for 订单分拣)
        public static DataResult GetPickedOrder(ABatchParams IParam)
        {
            var result = new DataResult(1, null);
            using (var CoreConn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    string FSql = @"SELECT
                                        saleout.BatchID,
                                        saleout.OID,
                                        saleout.SoID,
                                        saleout.SortCode,
                                        saleoutitem.SkuID,
                                        SUM(saleoutitem.Qty) ItemQty,
                                        (
                                            SELECT
                                                IFNULL(SUM(batchpicked.Qty), 0)
                                            FROM
                                                batchpicked
                                            WHERE
                                                batchpicked.BatchID = saleout.BatchID
                                            AND batchpicked.Skuautoid = @SkuID
                                            AND batchpicked.CoID = @CoID
                                            AND batchpicked.OID = saleout.OID
                                        ) AS SortQty
                                    FROM
                                        saleout,
                                        saleoutitem,
                                        batch
                                    WHERE
                                        saleout.CoID = saleoutitem.CoID
                                    AND saleout.OID = saleoutitem.OID
                                    AND saleout.BatchID = saleoutitem.BatchID
                                    AND saleout.BatchID = batch.ID
                                    AND batch.Type = 1
                                    AND saleoutitem.Skuautoid = @SkuID
                                    AND saleout.CoID = @CoID
                                    AND saleout.BatchID = @BatchID
                                    GROUP BY
                                        saleout.BatchID,
                                        saleout.OID,
                                        saleout.SoID,
                                        saleout.BatchID,
                                        saleoutitem.SkuID
                                    HAVING
                                        SUM(saleoutitem.Qty) > (
                                            SELECT
                                                IFNULL(SUM(batchpicked.Qty), 0)
                                            FROM
                                                batchpicked
                                            WHERE
                                                batchpicked.BatchID = @BatchID
                                            AND batchpicked.Skuautoid = @SkuID
                                            AND batchpicked.CoID = @CoID
                                            AND batchpicked.OID = saleout.OID
                                        )
                                    LIMIT 1";
                    var p = new DynamicParameters();
                    p.Add("@CoID", IParam.CoID);
                    p.Add("@SkuID", IParam.Skuautoid);
                    p.Add("@BatchID", IParam.BatchID);
                    var itemLst = CoreConn.Query<OrderItemBatch>(FSql, p).AsList();
                    if (itemLst.Count > 0)
                    {
                        result.d = itemLst[0];
                    }
                    else
                    {
                        result.s = -6011;//无分拣任务
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

        #region 分拣格解绑
        public static DataResult SetUnLock(ABatchParams IParam)
        {
            var result = new DataResult(1, null);
            var CoreConn = new MySqlConnection(DbBase.CoreConnectString);
            CoreConn.Open();
            var CoreTrans = CoreConn.BeginTransaction();
            try
            {

                string psql = @"UPDATE batchpicked
                            SET SoID = 0,
                                OID = 0,
                                SortCode = '',
                                `Status` = 0
                            WHERE
                                CoID =@CoID
                            AND SortCode =@SortCode";
                string osql = @"UPDATE saleout
                                SET SortCode = ''
                                WHERE
                                    CoID =@CoID
                                AND SortCode =@SortCode";
                var p = new DynamicParameters();
                p.Add("@CoID", IParam.CoID);
                p.Add("@SortCode", IParam.SortCode);
                int c1 = CoreConn.Execute(psql, p, CoreTrans);
                int c2 = CoreConn.Execute(osql, p, CoreTrans);
                if (c1 <= 0 || c2 <= 0)
                {
                    result.s = 6014;
                }
                else
                {
                    CoreTrans.Commit();
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


        #region 新增拣货记录Sql        
        public static string AddBatchPickSql()
        {
            string sql = @"INSERT INTO batchpicked
                            {
                                BatchID,
                                CoID,
                                BarCode,
                                Skuautoid,
                                Sku,
                                BatchtaskID,
                                Creator,
                                CreateDate
                            }
                            VALUES
                            {
                                @BatchID,
                                @CoID,
                                @BarCode,
                                @Skuautoid,
                                @Sku,
                                @BatchtaskID,
                                @Creator,
                                @CreateDate
                            }";
            return sql;
        }
        #endregion

        #region 获取拣货数量auto
        public static string BatchAutoSql()
        {
            string sql = @"SELECT
                                batchtask.BatchID,
                                IFNULL(SUM(Qty), 0) AS Qty,
                                IFNULL(SUM(PickQty), 0) AS PickQty,
                                IFNULL(SUM(NoQty), 0) AS NoQty
                            FROM
                                batchtask
                            WHERE
                                CoID = @CoID
                            AND BatchID = @BatchID";
            return sql;
        }
        #endregion
    }
}