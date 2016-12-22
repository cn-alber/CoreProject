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
                {//3.正在拣货，0.一单一件                    
                    string bsql = @"SELECT ID FROM batch WHERE CoID=@CoID AND Status=3 AND Type = 0";
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
                                FROM batchpicked
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
                                result = ASkuScanHaddles.GetType(cp);
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
                var invLst = CoreConn.Query<int>(invsql, new { CoID = IParam.CoID, SkuID = asku.Skuautoid }, CoreTrans).AsList();
                if (invLst.Count <= 0)
                {
                    result.s = -6017;//库存不足
                }
                else if (invLst[0] < asku.Qty)
                {
                    result.s = -6017;//库存不足
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
                    p.Add("@CoID", IParam.CoID);
                    p.Add("@ID", IParam.ID);
                    p.Add("@OutID", aout.OutID);
                    p.Add("@OID", aout.OID);
                    p.Add("@SoID", aout.SoID);
                    p.Add("@ExCode", aout.ExCode);
                    p.Add("@Express", aout.ExpName);
                    int c = CoreConn.Execute(picksql, p, CoreTrans);
                    //获取未出货笔数                 
                    int bchNout = CoreConn.Query<int>(GetBatchNOutSql(), new { CoID = IParam.CoID, BatchID = aout.BatchID }).First();
                    if (bchNout == 0)
                    {//6.已完成
                        CoreConn.Execute(SetBatchFnSql(), new { CoID = IParam.CoID, BatchID = aout.BatchID }, CoreTrans);
                    }
                    //更新发货状态
                    CoreConn.Execute(SetSaleOutFnSql(), new { CoID = IParam.CoID, OutID = aout.OutID }, CoreTrans);
                    //更新订单状态
                    CoreConn.Execute(SetOrderFnSql(), new { CoID = IParam.CoID, OID = aout.OID }, CoreTrans);
                    //新增出库交易&更新库存
                    result = CommHaddle.GetWhViewAll(IParam.CoID.ToString());
                    var WhViewLst = result.d as List<Warehouse_view>;
                    WhViewLst = WhViewLst.Where(a => a.Type == "3").AsList();//销退仓 - 零数仓                    
                    int Status = 1;
                    int Type = 1202;
                    var inv = new Invinout();
                    inv.RefID = aout.OutID.ToString();
                    inv.OID = aout.SoID.ToString();
                    inv.RecordID = RecordID;
                    inv.Type = Type;
                    inv.CusType = Enum.GetName(typeof(InvE.InvType), Type).ToString();//交易类型           ;
                    inv.Status = Status;
                    inv.WhID = WhViewLst[0].ParentID;
                    inv.LinkWhID = WhViewLst[0].ID;
                    inv.Creator = IParam.Creator;
                    inv.CreateDate = IParam.CreateDate;
                    inv.CoID = IParam.CoID.ToString();
                    CoreConn.Execute(InventoryHaddle.AddInvinoutSql(), inv, CoreTrans);
                    //交易子表
                    var inv_item = new Invinoutitem();
                    inv_item.RefID = aout.OutID.ToString();
                    inv_item.OID = aout.OID.ToString();
                    inv_item.IoID = RecordID;
                    inv_item.Type = Type;
                    inv_item.CusType = Enum.GetName(typeof(InvE.InvType), Type).ToString();//交易类型     ;
                    inv_item.Status = Status;
                    inv_item.WhID = WhViewLst[0].ParentID;
                    inv_item.LinkWhID = WhViewLst[0].ID;
                    inv_item.Skuautoid = asku.Skuautoid;
                    inv_item.Qty = asku.Qty;//交易数量 
                    inv_item.Creator = IParam.Creator;
                    inv_item.CreateDate = IParam.CreateDate;
                    inv_item.CoID = IParam.CoID.ToString();
                    CoreConn.Execute(InventoryHaddle.AddInvinoutitemSql(), inv_item, CoreTrans);
                    //更新库存数量     
                    var SkuIDLst = new List<int>();
                    SkuIDLst.Add(inv_item.Skuautoid);
                    CoreConn.Execute(UptInvSaleQtySql(), new { CoID = IParam.CoID, SkuIDLst = SkuIDLst, Modifier = IParam.Creator, ModifyDate = IParam.CreateDate }, CoreTrans);
                    //更新总库存数量
                    var res = CommHaddle.GetWareCoidList(IParam.CoID.ToString());
                    var CoIDLst = res.d as List<string>;
                    CoreConn.Execute(UptInvMainSaleQtySql(), new { CoID = IParam.CoID, CoIDLst = CoIDLst, SkuIDLst = SkuIDLst, Modifier = IParam.Creator, ModifyDate = IParam.CreateDate }, CoreTrans);
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


        #region 多件发货 - 扫描格号
        public static DataResult GetScanOutSortCode(ASaleParams IParam)
        {
            var result = new DataResult(1, null);
            using (var CoreConn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    string sql = @"SELECT BatchID,
                                        ID AS OutID,
                                        OID,
                                        ExpName,
                                        ExCode,
                                        OrdQty,
                                        SortCode
                                    FROM saleout
                                    WHERE CoID=@CoID
                                    AND SortCode=@SortCode
                                    AND `Status` = 0";
                    var aout = CoreConn.Query<OutItemBatch>(sql, new { CoID = IParam.CoID, SortCode = IParam.SortCode }).AsList();
                    if (aout.Count <= 0)
                    {
                        result.s = -6000;
                    }
                    else
                    {
                        result.d = aout[0];
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

        #region 多件发货-扫描件码
        public static DataResult GetSaleOutSkuMulti(ASaleParams IParam)
        {
            var result = new DataResult(1, null);
            var data = new ASaleOutData();
            using (var CoreConn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    string sql = @"SELECT ID,
                                    BatchID,
                                    OID,
                                    SoID,
                                    BarCode,
                                    Skuautoid,
                                    OutID
                                    BatchtaskID,
                                    SortCode 
                                FROM batchpicked
                                WHERE CoID=@CoID 
                                AND BarCode=@BarCode
                                AND SortCode in @SortCode";
                    var pickLst = CoreConn.Query<ABatchPicked>(sql, new { CoID = IParam.CoID, SortCode = IParam.SortCode }).AsList();
                    if (pickLst.Count <= 0)
                    {
                        result.s = -6018;//件码与出货订单不符
                    }
                    else
                    {
                        var pLst = pickLst.Where(a => a.OutID == 0).AsList();
                        if (pLst.Count <= 0)
                        {
                            result.s = -6019;//扫描件码已出货
                        }
                        else
                        {
                            var cp = new ASkuScanParam();
                            cp.CoID = IParam.CoID;
                            cp.BarCode = IParam.BarCode;
                            result = ASkuScanHaddles.GetType(cp);
                            if (result.s > 0)
                            {
                                data.SkuAuto = result.d as ASkuScan; //扫描件码
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

        #region 多件发货-更新库存资料
        public static DataResult SaleOutMulti(ASaleOutSet IParam)
        {
            var result = new DataResult(1, null);
            var RecordID = "SL" + CommHaddle.GetRecordID(IParam.CoID);
            var CoreConn = new MySqlConnection(DbBase.CoreConnectString);
            CoreConn.Open();
            var CoreTrans = CoreConn.BeginTransaction();
            try
            {
                var aout = IParam.OItemAuto;
                // IParam.OID = aout.OID;
                //库存检查
                result = CheckMultiInvQty(IParam, CoreTrans, CoreConn);
                var itemLst = result.d as List<ASaleOutQty>;
                if (result.s > 0)
                {
                    //更新拣货资料
                    string picksql = @"UPDATE batchpicked
                                        SET OutID =@OutID,                                            
                                            ExCode =@ExCode,
                                            Express =@Express,
                                            SortCode = '',
                                            `Status`=2
                                        WHERE CoID=@CoID
                                        AND OID=@OID";
                    var p = new DynamicParameters();
                    p.Add("@CoID", IParam.CoID);
                    p.Add("@OutID", aout.OutID);
                    p.Add("@OID", aout.OID);
                    p.Add("@ExCode", aout.ExCode);
                    p.Add("@Express", aout.ExpName);
                    int c = CoreConn.Execute(picksql, p, CoreTrans);
                    //获取未出货笔数                 
                    int bchNout = CoreConn.Query<int>(GetBatchNOutSql(), new { CoID = IParam.CoID, BatchID = aout.BatchID }).First();
                    if (bchNout == 0)
                    {//6.已完成
                        CoreConn.Execute(SetBatchFnSql(), new { CoID = IParam.CoID, BatchID = aout.BatchID }, CoreTrans);
                    }
                    //更新发货状态
                    CoreConn.Execute(SetSaleOutFnSql(), new { CoID = IParam.CoID, OutID = aout.OutID }, CoreTrans);
                    //更新订单状态
                    CoreConn.Execute(SetOrderFnSql(), new { CoID = IParam.CoID, OID = aout.OID }, CoreTrans);
                    //新增出库交易&更新库存
                    result = CommHaddle.GetWhViewAll(IParam.CoID.ToString());
                    var WhViewLst = result.d as List<Warehouse_view>;
                    WhViewLst = WhViewLst.Where(a => a.Type == "3").AsList();//销退仓 - 零数仓                    
                    int Status = 1;
                    int Type = 1202;
                    var inv = new Invinout();
                    inv.RefID = aout.OutID.ToString();
                    inv.OID = aout.SoID.ToString();
                    inv.RecordID = RecordID;
                    inv.Type = Type;
                    inv.CusType = Enum.GetName(typeof(InvE.InvType), Type).ToString();//交易类型
                    inv.Status = Status;
                    inv.WhID = WhViewLst[0].ParentID;
                    inv.LinkWhID = WhViewLst[0].ID;
                    inv.Creator = IParam.Creator;
                    inv.CreateDate = IParam.CreateDate;
                    inv.CoID = IParam.CoID.ToString();
                    CoreConn.Execute(InventoryHaddle.AddInvinoutSql(), inv, CoreTrans);
                    //交易子表
                    var inv_itemLst = itemLst.Select(a => new Invinoutitem
                    {
                        RefID = aout.OutID.ToString(),
                        OID = aout.OID.ToString(),
                        IoID = RecordID,
                        Type = Type,
                        CusType = Enum.GetName(typeof(InvE.InvType), Type).ToString(),
                        Status = Status,
                        WhID = WhViewLst[0].ParentID,
                        LinkWhID = WhViewLst[0].ID,
                        Skuautoid = a.Skuautoid,
                        Qty = a.Qty,
                        Creator = IParam.Creator,
                        CreateDate = IParam.CreateDate,
                        CoID = IParam.CoID.ToString()
                    }).AsList();
                    CoreConn.Execute(InventoryHaddle.AddInvinoutitemSql(), inv_itemLst, CoreTrans);
                    //更新库存数量     
                    var SkuIDLst = inv_itemLst.Select(a => a.Skuautoid).AsList();
                    CoreConn.Execute(UptInvSaleQtySql(), new { CoID = IParam.CoID, SkuIDLst = SkuIDLst, Modifier = IParam.Creator, ModifyDate = IParam.CreateDate }, CoreTrans);
                    //更新总库存数量
                    var res = CommHaddle.GetWareCoidList(IParam.CoID.ToString());
                    var CoIDLst = res.d as List<string>;
                    CoreConn.Execute(UptInvMainSaleQtySql(), new { CoID = IParam.CoID, CoIDLst = CoIDLst, SkuIDLst = SkuIDLst, Modifier = IParam.Creator, ModifyDate = IParam.CreateDate }, CoreTrans);
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

        #region 多件发货 - 检查库存是否充足
        public static DataResult CheckMultiInvQty(ASaleOutSet IParam, IDbTransaction Trans, MySqlConnection conn)
        {
            var result = new DataResult(1, null);
            var item=IParam.OItemAuto;
            string itemsql = @"SELECT @CoID AS CoID,Skuautoid,Qty FROM saleoutitem WHERE CoID=@CoID AND OID=@OID";
            string invsql = @"SELECT IFNULL(Count(ID),0) FROM inventory WHERE CoID=@CoID AND Skuautoid =@Skuautoid AND StockQty<@Qty";
            var itemLst = conn.Query<ASaleOutQty>(itemsql, new { CoID = IParam.CoID, OID = item.OID }, Trans).AsList();
            var count = conn.QueryFirst<int>(invsql, itemLst);
            if (count > 0)
            {
                result.s = -6017;//库存不足
            }
            else
            {
                result.d = itemLst;
            }
            return result;
        }
        #endregion

        #region 大单发货 - 获取批次信息
        public static DataResult GetSaleOutByBatch(ASaleParams IParam)
        {
            var result = new DataResult(1, null);
            var data = new ASaleOutData();
            using (var CoreConn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    string sql = @"SELECT BatchID,
                                        ID AS OutID,
                                        OID,
                                        ExpName,
                                        ExCode,
                                        OrdQty,
                                        SortCode
                                    FROM saleout
                                    WHERE CoID=@CoID
                                    AND BatchID=@BatchID
                                    AND `Status` = 0";
                    var p = new DynamicParameters();
                    p.Add("@CoID", IParam.CoID);
                    p.Add("@BatchID", IParam.BatchID);
                    var aout = CoreConn.Query<OutItemBatch>(sql, p).AsList();
                    if (aout.Count <= 0)
                    {
                        result.s = -6000;
                    }
                    else
                    {
                        string qSql = @"SELECT
                                            IFNULL(SUM(Qty), 0)
                                        FROM
                                            saleoutitemp
                                        WHERE
                                            CoID =@CoID
                                        AND BatchID =@BatchID";
                        int OutQty = CoreConn.Query<int>(qSql, p).First();
                        aout[0].OutQty = OutQty;
                        result.d = aout[0];
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

        #region 大单发货 - 条码扫描获取条码信息
        public static DataResult GetSaleOutSkuBig(ASaleParams IParam)
        {
            var result = new DataResult(1, null);
            var data = new ASaleOutData();
            using (var CoreConn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    var cp = new ASkuScanParam();
                    cp.CoID = IParam.CoID;
                    cp.BarCode = IParam.BarCode;
                    result = ASkuScanHaddles.GetType(cp);
                    if (result.s > 0)
                    {
                        var asku = result.d as ASkuScan;
                        if (asku.SkuType != 1)
                        {
                            string sql = @"SELECT IFNULL(SUM(Qty),0) FROM saleoutitemp WHERE CoID=@CoID";
                            if (asku.SkuType == 0)
                            {
                                sql = sql + " AND BarCode=@BarCode";
                            }
                            else if (asku.SkuType == 2)
                            {
                                sql = sql + " AND BoxCode=@BarCode";
                            }
                            var p = new DynamicParameters();
                            p.Add("@CoID", IParam.CoID);
                            p.Add("@BarCode", IParam.BarCode);
                            var temp = CoreConn.Query<int>(sql, p).First();
                            if (temp > 0)
                            {
                                result.s = -6016;//重复扫描出货
                            }
                        }
                        string psql = @"SELECT IFNULL(SUM(Qty),0)  
                                FROM batchpicked                                
                                WHERE CoID=@CoID 
                                AND BarCode=@BarCode
                                AND BatchID = @BatchID";
                        var PickQty = CoreConn.Query<int>(psql, new { CoID = IParam.CoID, BarCode = IParam.BarCode, BatchID = IParam.BatchID }).First();
                        if (PickQty <= 0)
                        {
                            result.s = -6020;//请先执行拣货任务
                        }
                        else
                        {
                            result.d = asku;
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

        #region 大单发货 - 更新库存资料
        public static DataResult SaleOutBig(ASaleOutSet IParam)
        {
            var result = new DataResult(1, null);
            var RecordID = "SL" + CommHaddle.GetRecordID(IParam.CoID);
            var CoreConn = new MySqlConnection(DbBase.CoreConnectString);
            CoreConn.Open();
            var CoreTrans = CoreConn.BeginTransaction();
            try
            {
                string Osql = @"SELECT `Status`
                                FROM saleout
                                WHERE CoID=@CoID AND OID=@OID ";
                var Olst = CoreConn.Query<int>(Osql, new { CoID = IParam.CoID, OID = IParam.OItemAuto.OID }).AsList();
                if (Olst.Count <= 0)
                {
                    result.s = -6000;
                }
                else if (Olst[0] > 0)
                {
                    result.s = -6022;
                }
                else
                {
                    //检查库存是否充足
                    result = CheckMultiInvQty(IParam, CoreTrans, CoreConn);
                    var itemLst = result.d as List<ASaleOutQty>;
                    if (result.s > 0)
                    {

                        //更新出货单状态
                        string UptOsql = @"UPDATE saleout
                                    SET `Status` = 1,
                                        IsDeliver = 1
                                    WHERE
                                        CoID =@CoID
                                    AND OID =@OID";
                        CoreConn.Execute(UptOsql, new { CoID = IParam.CoID, OID = IParam.OItemAuto.OID }, CoreTrans);

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


        #region 更新销退仓库存
        public static string UptInvSaleQtySql()
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
                            SaleRetuQty = (SELECT IFNULL(SUM(Qty),0)
                                                FROM wmspile
                                                WHERE wmspile.Skuautoid = inventory.Skuautoid
                                                AND wmspile.CoID = inventory.CoID
                                                AND wmspile.Skuautoid in @SkuIDLst
                                                AND wmspile.Type = 3),
                                            IsDelete=0,Modifier=@Modifier,ModifyDate=@ModifyDate
                                            WHERE
	                                            inventory.CoID =@CoID
                                            AND Inventory.Skuautoid in @SkuIDLst";
            return sql;
        }

        public static string UptInvMainSaleQtySql()
        {
            string sql = @"UPDATE inventory_sale
                            SET Inventory_sale.StockQty = (
                                SELECT          
                                    IFNULL(SUM(StockQty),0)
                                FROM
                                    inventory
                                WHERE
                                    inventory.CoID IN @CoIDLst
                                AND inventory.Skuautoid = inventory_sale.Skuautoid),
                            inventory_sale.SaleRetuQty = (
                                SELECT
                                    IFNULL(SUM(SaleRetuQty),0)
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
        #region 获取批次未出货笔数
        public static string GetBatchNOutSql()
        {
            string bnsql = @"SELECT
                                        IFNULL(SUM(Qty), 0)
                                    FROM
                                        batchpicked
                                    WHERE
                                        CoID =@CoID
                                    AND BatchID =@BatchID
                                    AND OutID=0";
            return bnsql;
        }
        #endregion

        #region 更新批次状态-已完成
        public static string SetBatchFnSql()
        {
            string sql = @"UPDATE batch
                            SET `Status` = 6
                            WHERE
                                CoID =@CoID
                            AND BatchID =@BatchID";
            return sql;
        }
        #endregion

        #region 更新发货状态 - 已发货
        public static string SetSaleOutFnSql()
        {
            string sql = @"UPDATE saleout
                            SET `Status` = 1,
                                IsDeliver = 1
                            WHERE
                                CoID =@CoID
                            AND ID =@OutID";
            return sql;
        }
        #endregion

        #region 更新订单状态 - 已发货
        public static string SetOrderFnSql()
        {
            string sql = @"UPDATE saleout
                        SET `Status` = 4
                        WHERE
                            CoID =@CoID
                        AND ID =@OID";
            return sql;
        }
        #endregion
    }
}