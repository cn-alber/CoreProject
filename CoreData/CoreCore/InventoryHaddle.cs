using CoreModels;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using CoreModels.XyCore;
using CoreData.CoreComm;
using MySql.Data.MySqlClient;

namespace CoreData.CoreCore
{
    public static class InventoryHaddle
    {
        #region 商品库存查询
        public static DataResult GetInvQuery(InvQueryParam IParam)
        {
            var inv = new InventoryData();
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    StringBuilder querycount = new StringBuilder();
                    StringBuilder querysql = new StringBuilder();
                    var p = new DynamicParameters();
                    string countsql = @"SELECT count(ID) FROM inventory WHERE CoID = @CoID";
                    string sql = @"SELECT
                                    inventory.ID,
                                    inventory.GoodsCode,
                                    inventory.SkuID,
                                    inventory.`Name`,
                                    inventory.Norm,
                                    inventory.WarehouseID,
                                    inventory.WarehouseName,
                                    inventory.StockQty,
                                    inventory.LockQty,
                                    inventory.PickQty,
                                    inventory.WaitInQty,
                                    inventory.ReturnQty,
                                    inventory.SafeQty,
                                    inventory.DefectiveQty,
                                    inventory.VirtualQty,
                                    inventory.PurchaseQty,
                                    inventory.Pic,
                                    inventory.CoID
                                   FROM
                                    inventory
                                   WHERE CoID = @CoID";
                    querycount.Append(countsql);
                    querysql.Append(sql);
                    p.Add("@CoID", IParam.CoID);
                    if (!string.IsNullOrEmpty(IParam.SkuID))//商品编号
                    {
                        querycount.Append(" AND inventory.SkuID like @SkuID");
                        querysql.Append(" AND inventory.SkuID like @SkuID");
                        p.Add("@SkuID", "%" + IParam.SkuID + "%");
                    }
                    if (!string.IsNullOrEmpty(IParam.SkuName))//商品名称
                    {
                        querycount.Append(" AND inventory.SkuName like @SkuName");
                        querysql.Append(" AND inventory.SkuName like @SkuName");
                        p.Add("@SkuName", "%" + IParam.SkuName + "%");
                    }
                    if (IParam.StockQtyb < IParam.StockQtye && IParam.StockQtyb > 0)
                    {
                        querycount.Append(" AND inventory.StockQty > @StockQtyb AND inventory.StockQty < @StockQtye");
                        querysql.Append(" AND inventory.StockQty > @StockQtyb AND inventory.StockQty < @StockQtye");
                        p.Add("@StockQtyb", IParam.StockQtyb);
                        p.Add("@StockQtye", IParam.StockQtye);
                    }
                    if (IParam.WarehouseID > 0)
                    {
                        querycount.Append(" AND inventory.WarehouseID = @WarehouseID");
                        querysql.Append(" AND inventory.WarehouseID = @WarehouseID");
                        p.Add("@WarehouseID", IParam.WarehouseID);
                    }
                    if (IParam.Status == 1)
                    {
                        querycount.Append(" AND inventory.StockQty >= inventory.SafeQty");
                    }
                    else if (IParam.Status == 2)
                    {
                        querycount.Append(" AND inventory.StockQty < inventory.SafeQty");
                    }
                    if (!string.IsNullOrEmpty(IParam.SortField) && !string.IsNullOrEmpty(IParam.SortDirection))//排序
                    {
                        querycount.Append(" ORDER BY " + IParam.SortField + " " + IParam.SortDirection);
                        querysql.Append(" ORDER BY " + IParam.SortField + " " + IParam.SortDirection);
                    }
                    var DataCount = CoreData.DbBase.CoreDB.QueryFirst<int>(querycount.ToString(), p);
                    if (DataCount < 0)
                    {
                        res.s = -3001;
                    }
                    else
                    {
                        inv.DataCount = DataCount;
                        decimal pagecnt = Math.Ceiling(decimal.Parse(inv.DataCount.ToString()) / decimal.Parse(IParam.PageSize.ToString()));
                        inv.PageCount = Convert.ToInt32(pagecnt);
                        int dataindex = (IParam.PageIndex - 1) * IParam.PageSize;
                        querysql.Append(" LIMIT @ls , @le");
                        p.Add("@ls", dataindex);
                        p.Add("@le", IParam.PageSize);
                        var InvLst = CoreData.DbBase.CoreDB.Query<Inventory>(querysql.ToString(), p).AsList();
                        inv.InvLst = InvLst;
                        res.d = inv;
                    }
                }
                catch (Exception e)
                {
                    res.s = -1;
                    res.d = e.Message;
                }
                finally
                {
                    conn.Dispose();
                }
            }
            return res;
        }
        #endregion

        #region 商品库存查询 - 商品交易查询
        public static DataResult GetInvDetailQuery(InvQueryParam IParam)
        {
            var inv = new InvItemData();
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    StringBuilder querycount = new StringBuilder();
                    StringBuilder querysql = new StringBuilder();
                    var p = new DynamicParameters();
                    string countsql = @"SELECT count(ID) FROM inventory WHERE CoID = @CoID";
                    string sql = @"
                                   SELECT
                                        invinoutitem.IoID,
                                        invinoutitem.CoID,
                                        invinoutitem.SkuID,
                                        invinoutitem.SkuName,
                                        invinoutitem.Qty,
                                        invinoutitem.Creator,
                                        invinoutitem.CreateDate,
                                        invinoutitem.CusType,
                                        invinoutitem.Norm,
                                        invinoutitem.Unit,
                                        invinoutitem.WhID,
                                        invinoutitem.WhName
                                   FROM
                                        invinoutitem
                                   WHERE CoID = @CoID";
                    querycount.Append(countsql);
                    querysql.Append(sql);
                    p.Add("@CoID", IParam.CoID);
                    if (!string.IsNullOrEmpty(IParam.SkuID))//商品编号
                    {
                        querycount.Append(" AND SkuID = @SkuID");
                        querysql.Append(" AND SkuID = @SkuID");
                        p.Add("@SkuID", IParam.SkuID);
                    }
                    if (IParam.WarehouseID > 0)//商品名称
                    {
                        querycount.Append(" AND WarehouseID = @WarehouseID");
                        querysql.Append(" AND WarehouseID = @WarehouseID");
                        p.Add("@WarehouseID", IParam.WarehouseID);
                    }
                    if (!string.IsNullOrEmpty(IParam.DocType))//单据类型
                    {
                        querycount.Append(" AND DocType = @DocType");
                        querysql.Append(" AND DocType = @DocType");
                        p.Add("@DocType", IParam.DocType);
                    }
                    if (!string.IsNullOrEmpty(IParam.RecordID))
                    {
                        querycount.Append(" AND IoID = @IoID");
                        querysql.Append(" AND IoID = @IoID");
                        p.Add("@IoID", IParam.RecordID);
                    }
                    if (IParam.DocDateB > Convert.ToDateTime("1999/01/01") & IParam.DocDateE > Convert.ToDateTime("1999/01/01") & IParam.DocDateB < IParam.DocDateE)
                    {
                        querycount.Append(" AND CreateDate >= @DocDateB AND CreateDate <=@DocDateE");
                        p.Add("@DocDateB", IParam.DocDateB);
                        p.Add("@DocDateE", IParam.DocDateE);
                    }
                    if (!string.IsNullOrEmpty(IParam.SortField) && !string.IsNullOrEmpty(IParam.SortDirection))//排序
                    {
                        querycount.Append(" ORDER BY " + IParam.SortField + " " + IParam.SortDirection);
                        querysql.Append(" ORDER BY " + IParam.SortField + " " + IParam.SortDirection);
                    }
                    var DataCount = CoreData.DbBase.CoreDB.QueryFirst<int>(querycount.ToString(), p);
                    if (DataCount < 0)
                    {
                        res.s = -3001;
                    }
                    else
                    {
                        inv.DataCount = DataCount;
                        decimal pagecnt = Math.Ceiling(decimal.Parse(inv.DataCount.ToString()) / decimal.Parse(IParam.PageSize.ToString()));
                        inv.PageCount = Convert.ToInt32(pagecnt);
                        int dataindex = (IParam.PageIndex - 1) * IParam.PageSize;
                        querysql.Append(" LIMIT @ls , @le");
                        p.Add("@ls", dataindex);
                        p.Add("@le", IParam.PageSize);
                        var InvItemLst = CoreData.DbBase.CoreDB.Query<Invinoutitem>(querysql.ToString(), p).AsList();
                        inv.InvitemLst = InvItemLst;
                        res.d = inv;
                    }
                }
                catch (Exception e)
                {
                    res.s = -1;
                    res.d = e.Message;
                }
                finally
                {
                    conn.Dispose();
                }
            }
            return res;
        }
        #endregion

        #region 商品库存查询 - 修改现有库存-查询单笔库存明细
        public static DataResult GetInventorySingle(int ID, int CoID)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    StringBuilder querysql = new StringBuilder();
                    string sql = @"SELECT * FROM inventory WHERE CoID=@CoID AND ID=@ID";
                    var p = new DynamicParameters();
                    querysql.Append(sql);
                    p.Add("@CoID", CoID);
                    p.Add("@ID", ID);
                    // if (!string.IsNullOrEmpty(SkuID))//商品编号
                    // {
                    //     querysql.Append(" AND SkuID = @SkuID");
                    //     p.Add("@SkuID", SkuID);
                    // }
                    // if (WarehouseID > 0)//商品名称
                    // {
                    //     querysql.Append(" AND WarehouseID = @WarehouseID");
                    //     p.Add("@WarehouseID", WarehouseID);
                    // }
                    var inv = CoreData.DbBase.CoreDB.QueryFirst<Inventory>(querysql.ToString(), p);
                    if (inv == null)
                    {
                        res.s = -3001;
                    }
                    else
                    {
                        res.d = inv;
                    }
                }
                catch (Exception e)
                {
                    res.s = -1;
                    res.d = e.Message;
                }
                finally
                {
                    conn.Dispose();
                }
            }
            return res;
        }
        #endregion

        #region 商品库存查询 - 修改现有库存-产生盘点交易
        public static DataResult SetStockQtySingle(int ID, decimal SetQty, int CoID, string UserName)
        {
            var res = new DataResult(1, null);
            var inoutAuto = new InvinoutAuto();
            inoutAuto.CoID = CoID;
            inoutAuto.UserName = UserName;
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                conn.Open();
                var TransCore = conn.BeginTransaction();
                try
                {
                    var invsql = "SELECT * FROM inventory WHERE ID = @ID";
                    var args = new { ID = ID };
                    var inv = conn.QueryFirst<Inventory>(invsql, args);
                    if (inv == null)
                    {
                        res.s = -1;
                        res.d = "获取库存失败";
                    }
                    else
                    {
                        inoutAuto.inv = inv;
                        inoutAuto.Qty = SetQty - inv.StockQty;
                        if (inoutAuto.Qty > 0)
                        {
                            inoutAuto.CusType = "盘盈";
                            inoutAuto.Type = 1;
                        }
                        else
                        {
                            inoutAuto.CusType = "盘亏";
                            inoutAuto.Type = 2;
                        }
                        inoutAuto.RecordID = CommHaddle.GetRecordID(CoID);
                        int count1 = conn.Execute(AddInvinoutSql(), AddInvinout(inoutAuto), TransCore);
                        int count2 = conn.Execute(AddInvinoutitemSql(), AddInvinoutitem(inoutAuto), TransCore);
                        string sql = @"UPDATE inventory SET StockQty = @StockQty WHERE ID=@ID ";
                        int count3 = conn.Execute(sql, new { StockQty = SetQty, ID = ID });
                        if (count1 < 0 || count2 < 0 || count3 < 0)
                        {
                            res.s = -3002;//资料新增失败
                        }
                        else
                        {
                            TransCore.Commit();
                            CoreUser.LogComm.InsertUserLog("新增交易", "invinout", inoutAuto.CusType + inoutAuto.inv.SkuID + " " + inoutAuto.Qty.ToString(), UserName, CoID.ToString(), DateTime.Now);
                        }
                    }

                }
                catch (Exception e)
                {
                    TransCore.Rollback();
                    res.s = -1;
                    res.d = e.Message;
                }
                finally
                {
                    TransCore.Dispose();
                    conn.Dispose();
                }
            }
            return res;
        }
        #endregion

        #region 商品库存查询 - 修改安全库存 - 查询GoodsCode款式库存
        public static DataResult GetInvSafeQtyLst(string GoodsCode, int WarehouseID, int CoID, string UserName)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    StringBuilder querysql = new StringBuilder();
                    string sql = @"SELECT * FROM inventory WHERE CoID=@CoID";
                    var p = new DynamicParameters();
                    querysql.Append(sql);
                    p.Add("@CoID", CoID);
                    if (!string.IsNullOrEmpty(GoodsCode))//款式编号
                    {
                        querysql.Append(" AND GoodsCode = @GoodsCode");
                        p.Add("@GoodsCode", GoodsCode);
                    }
                    if (WarehouseID > 0)//商品名称
                    {
                        querysql.Append(" AND WarehouseID = @WarehouseID");
                        p.Add("@WarehouseID", WarehouseID);
                    }
                    var invLst = CoreData.DbBase.CoreDB.Query<Inventory>(querysql.ToString(), p).AsList();
                    if (invLst.Count < 0)
                    {
                        res.s = -3001;
                    }
                    else
                    {
                        res.d = invLst;
                    }
                }
                catch (Exception e)
                {
                    res.s = -1;
                    res.d = e.Message;
                }
                finally
                {
                    conn.Dispose();
                }
            }
            return res;
        }
        #endregion

        #region  商品库存查询 - 修改安全库存 - 更新安全库存
        public static DataResult UptInvSafeQty(List<InventParams> invLst, int CoID, string UserName)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                conn.Open();
                var TransCore = conn.BeginTransaction();
                try
                {
                    string sql = @"UPDATE inventory SET SafeQty = @SafeQty WHERE ID = @ID";
                    int Count = conn.Execute(sql, invLst, TransCore);
                    if (Count < 0)
                    {
                        res.s = -3003;
                    }
                    else
                    {
                        TransCore.Commit();
                        CoreUser.LogComm.InsertUserLog("更新安全库存", "Inventory", "", UserName, CoID.ToString(), DateTime.Now);
                    }
                }
                catch (Exception e)
                {
                    res.s = -1;
                    res.d = e.Message;
                }
                finally
                {
                    conn.Dispose();
                }
            }

            return res;
        }
        #endregion

        #region 商品库存查询 - 更新商品名称
        public static DataResult UptInvSkuName(int CoID, string UserName)
        {
            var res = new DataResult(1, null);
            string sql = @"UPDATE inventory,
                                    coresku
                            SET inventory.`Name` = coresku.SkuName
                            WHERE
                                inventory.CoID = coresku.CoID
                            AND inventory.SkuID = coresku.SkuID
                            AND inventory.CoID = @CoID";
            var args = new { CoID = CoID };
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                conn.Open();
                var TransCore = conn.BeginTransaction();
                try
                {
                    int count = DbBase.CoreDB.Execute(sql, args);
                    if (count < 0)
                    {
                        res.s = -3003;
                    }
                    else
                    {
                        TransCore.Commit();
                        CoreUser.LogComm.InsertUserLog("更新商品名称", "Inventory", "更新商品名称", UserName, CoID.ToString(), DateTime.Now);
                    }
                }
                catch (Exception e)
                {
                    res.s = -1;
                    res.d = e.Message;
                }
                finally
                {
                    conn.Dispose();
                }
            }
            return res;
        }
        #endregion

        #region 商品库存查询 - 库存清零
        public static DataResult ClearSku(List<int> IDLst, int CoID, string UserName)
        {
            var res = new DataResult(1, null);
            var InvLst = DbBase.CoreDB.Query<Inventory>("SELECT * FROM inventory WHERE ID IN @IDLst", new { IDLst = IDLst }).AsList();
            // var cp = new InvinoutAuto();
            // cp.CoID = CoID;
            // cp.RecordID = CommHaddle.GetRecordID(CoID);
            string CusType = "库存清零";
            // cp.InvLst = InvLst;
            // var inoutLst = AddInvinoutLst(cp);
            // var itemLst = AddInvinoutitemLst(cp);
            var InoutLst = new List<Invinout>();
            var ItemLst = new List<Invinoutitem>();
            List<string> RecIDLst = new List<string>();
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                conn.Open();
                var TransCore = conn.BeginTransaction();
                try
                {
                    foreach (var inv in InvLst)
                    {
                        string RecordID = CommHaddle.GetRecordID(CoID);
                        var inout = new Invinout();
                        var item = new Invinoutitem();
                        inout.CoID = inv.CoID;
                        inout.Status = "审核通过";
                        inout.CusType = CusType;
                        if (inv.StockQty > 0)
                        {
                            inout.Type = 2;
                        }
                        else
                        {
                            inout.Type = 1;
                        }
                        inout.RecordID = RecordID;
                        inout.WhID = inv.WarehouseID;
                        inout.WhName = inv.WarehouseName;
                        inout.Creator = UserName;
                        inout.CreateDate = DateTime.Now;
                        inout.IsExport = false;
                        item.CoID = CoID;
                        item.CusType = CusType;
                        item.IoID = RecordID;
                        item.WhID = inv.WarehouseID;
                        item.WhName = inv.WarehouseName;
                        item.SkuID = inv.SkuID;
                        item.SkuName = inv.Name;
                        item.Qty = Convert.ToInt32(0 - inv.StockQty);
                        item.Unit = "件";
                        item.Creator = UserName;
                        item.CreateDate = DateTime.Now;
                        InoutLst.Add(inout);
                        ItemLst.Add(item);
                        RecIDLst.Add(RecordID);
                    }
                    int count1 = conn.Execute(AddInvinoutSql(), InoutLst, TransCore);
                    int count2 = conn.Execute(AddInvinoutitemSql(), ItemLst, TransCore);
                    string sql = @"UPDATE inventory SET StockQty = 0 WHERE ID in @IDLst ";
                    int count3 = conn.Execute(sql, new { IDLst = IDLst });
                    if (count1 < 0 || count2 < 0 || count3 < 0)
                    {
                        res.s = -1;
                        res.d = "库存清空失败";
                    }
                    else
                    {
                        TransCore.Commit();
                        CoreUser.LogComm.InsertUserLog("新增库存清零交易", "invinout", "单据编号:" + string.Join(",", RecIDLst.ToArray()), UserName, CoID.ToString(), DateTime.Now);
                    }
                }
                catch (Exception e)
                {
                    res.s = -1;
                    res.d = e.Message;
                }
                finally
                {
                    conn.Dispose();
                }
                return res;
            }
        }
        #endregion

        #region 商品库存盘点 - 从EXCEL模板导入(自动保存)
        public static DataResult CreateInvSetTemp(List<SetInvQtyExcel> InvLst, int CoID, string UserName)
        {
            var res = new DataResult(1, null);
            var ErLst = new List<SetInvQtyExcel>();
            int row = 0;
            Boolean upttype = false;
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                conn.Open();
                var TransCore = conn.BeginTransaction();
                try
                {
                    #region 新建临时表
                    string sql = @" CREATE TEMPORARY TABLE IF NOT EXISTS `TmpInvTable` (
                            `WarehouseID`  int(11) NULL DEFAULT NULL COMMENT '仓库编号' ,
                            `WarehouseName`  varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '仓库名称' ,
                            `SkuID`  varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '商品编码' ,
                            `Name`  varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '商品名称' ,
                            `Norm`  varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '规格' ,
                            `StockQty`  decimal(11,2) NULL DEFAULT 0.00 COMMENT '系统库存' ,
                            `SetQty`  decimal(11,2) NULL DEFAULT 0.00 COMMENT '库存' ,
                            `CoID`  int(11) NULL DEFAULT NULL COMMENT '公司ID' 
                            )";
                    int recount = DbBase.CoreDB.Execute(sql,TransCore);//新建临时表
                    string trsql = @"truncate TABLE TmpInvTable";
                    DbBase.CoreDB.Execute(trsql,TransCore);
                    #endregion

                    #region 消除重复项
                    var NKeylst = from l in InvLst
                                  group l by
                                  new { l.SkuID, l.WarehouseID }
                    into g
                                  where g.Count() > 1
                                  select g.Key;
                    var NLst = InvLst.Where(
                       a => NKeylst.Select(b => b.SkuID + b.WarehouseID.ToString())
                           .Contains(a.SkuID + a.WarehouseID.ToString())
                           )
                           .ToList();
                    //排除重复项
                    var DLst = InvLst.Where(
                            a => !NKeylst.Select(b => b.SkuID + b.WarehouseID.ToString())
                                .Contains(a.SkuID + a.WarehouseID.ToString())
                                )
                                .ToList();
                    if (NLst.Count > 0)
                    {

                        var label = new SetInvQtyExcel();
                        label.WarehouseName = "盘点资料重复:";
                        ErLst.Add(label);
                        ErLst.AddRange(NLst);
                    }
                    #endregion
                    if (DLst.Count > 0)
                    {
                        #region 插入临时数据
                        var sqltest = new StringBuilder();
                        foreach (var inv in DLst)
                        {
                            sqltest.Append("('" + inv.SkuID + "','" + inv.Name + "','" + inv.Norm
                                  + "'," + inv.WarehouseID + ",'" + inv.WarehouseName
                                  + "'," + inv.StockQty + "," + inv.SetQty + "," + CoID + "),");
                        }
                        string insql = @"INSERT INTO TmpInvTable(`SkuID`,`Name`,`Norm`,`WarehouseID`,`WarehouseName`,`StockQty`,`SetQty`,`CoID`) 
                                 VALUES"
                                + sqltest.ToString().Substring(0, sqltest.ToString().Length - 1);
                        int tcount = DbBase.CoreDB.Execute(insql);//插入临时数据
                        // var templst = DbBase.CoreDB.Query<SetInvQtyExcel>("SELECT * FROM TmpInvTable").AsList();

                        #endregion
                        #region 检查商品编码,SKUID 不存在->PASS
                        string skusql = @"SELECT
	                                    *
                                    FROM
	                                    TmpInvTable
                                    WHERE
	                                    TmpInvTable.CoID =@CoID
                                    AND TmpInvTable.SkuID NOT IN (
	                                    SELECT
		                                    coresku.SkuID
	                                    FROM
		                                    coresku
	                                    WHERE
		                                    coresku.CoID =@CoID
                                    )";
                        string skudelsql = @"DELETE
                                        FROM
	                                        TmpInvTable
                                        WHERE
	                                        TmpInvTable.CoID =@CoID
                                        AND TmpInvTable.SkuID NOT IN (
	                                        SELECT
		                                        coresku.SkuID
	                                        FROM
		                                        coresku
	                                        WHERE
		                                        coresku.CoID =@CoID)";
                        var SkuLst = DbBase.CoreDB.Query<SetInvQtyExcel>(skusql, new { CoID = CoID }).AsList();
                        if (SkuLst.Count > 0)
                        {
                            var label = new SetInvQtyExcel();
                            label.WarehouseName = "商品资料不存在";
                            ErLst.Add(label);
                            ErLst.AddRange(SkuLst);
                            DbBase.CoreDB.Execute(skudelsql, new { CoID = CoID });
                        }
                        #endregion
                        #region 检查商品是否存在库存,不存在->PASS
                        string ChkInvSql = @" SELECT *
                                         FROM
                                            TmpInvTable
                                        WHERE
                                            TmpInvTable.CoID = @CoID
                                        AND CONCAT(
                                            TmpInvTable.SkuID,
                                            ',',
                                            CONCAT(TmpInvTable.WarehouseID,'')
	                                    ) NOT IN (
                                            SELECT
                                                CONCAT(
                                                    inventory.SkuID,
                                                    ',',
                                                    CONCAT(inventory.WarehouseID,'')
                                                )
                                            FROM
                                                inventory
                                            WHERE
                                                inventory.CoID = @CoID
	                                    )";
                        string DelInvSql = @"Delete FROM
                                            TmpInvTable
                                        WHERE
                                            TmpInvTable.CoID = @CoID
                                        AND CONCAT(
                                            TmpInvTable.SkuID,
                                            ',',
                                            TmpInvTable.WarehouseName
	                                    ) NOT IN (
                                            SELECT
                                                CONCAT(
                                                    inventory.SkuID,
                                                    ',',
                                                    inventory.WarehouseName
                                                )
                                            FROM
                                                inventory
                                            WHERE
                                                inventory.CoID = @CoID
	                                    )";
                        var Invlst = DbBase.CoreDB.Query<SetInvQtyExcel>(ChkInvSql, new { CoID = CoID }).AsList();
                        if (Invlst.Count > 0)
                        {
                            var label = new SetInvQtyExcel();
                            label.WarehouseName = "商品库存不存在:";
                            ErLst.Add(label);
                            ErLst.AddRange(Invlst);
                            DbBase.CoreDB.Execute(DelInvSql, new { CoID = CoID });
                        }
                        #endregion
                        #region 生成盘点明细-盘盈
                        // templst = DbBase.CoreDB.Query<SetInvQtyExcel>("SELECT * FROM TmpInvTable").AsList();
                        string gaddsql = @"select distinct TmpInvTable.WarehouseID,TmpInvTable.WarehouseName 
                                    from TmpInvTable,Inventory
                                    where TmpInvTable.SkuID=Inventory.SkuID
                                    and TmpInvTable.WarehouseID=Inventory.WarehouseID
                                    and TmpInvTable.CoID=Inventory.CoID
                                    and TmpInvTable.SetQty>Inventory.StockQty
                                    and TmpInvTable.CoID = @CoID";
                        var GaddLst = DbBase.CoreDB.Query<SetInvQtyExcel>(gaddsql, new { CoID = CoID }).AsList();
                        if (GaddLst.Count > 0)
                        {
                            upttype = true;
                            var Rd = CommHaddle.GetRecordID(CoID);
                            foreach (var m in GaddLst)
                            {
                                row = row + 1;
                                string RecordID = "INV" + (Convert.ToInt64(Rd) + row + 1).ToString();//单据编号  

                                #region 期初主表新增
                                string InsertInvSql = @"insert into invinout(RecordID,Type,CusType,Status,
                                                                WhID,WhName,IsExport,Creator,
                                                                CreateDate,CoID)
                                                         values (@RecordID,1,'盘盈','审核通过',
                                                                @WarehouseID,@WarehouseName,0,@Creator,
                                                                @CreateDate,@CoID)";
                                var p = new DynamicParameters();
                                p.Add("@CoID", CoID);
                                p.Add("@RecordID", RecordID);
                                p.Add("@WarehouseID", m.WarehouseID);
                                p.Add("@WarehouseName", m.WarehouseName);
                                p.Add("@Creator", UserName);
                                p.Add("@CreateDate", DateTime.Now);
                                int i = DbBase.CoreDB.Execute(InsertInvSql, p);
                                #endregion
                                if (i > 0)
                                {
                                    #region 盘点明细新增
                                    string InsertInvItemSql = @"INSERT INTO invinoutitem (
	                                                                            IoID,CoID,SkuID,SkuName,
	                                                                            CusType,Norm,WhID,WhName,
	                                                                            Creator,CreateDate,Qty
                                                                            )(
	                                                                            SELECT
		                                                                            @RecordID,a.CoID,b.SkuID,b.NAME,
		                                                                            '盘盈',b.Norm,a.WarehouseID,b.WarehouseName,
		                                                                            @Creator ,@CreateDate,(a.SetQty - b.StockQty)
	                                                                            from TmpInvTable AS a,
                                                                                     Inventory AS b 
                                                                                where a.SkuID = b.SkuID
                                                                                and a.WarehouseID = b.WarehouseID
                                                                                and a.CoID = b.CoID
                                                                                and a.CoID=@CoID
                                                                                and a.WarehouseID = @WarehouseID
                                                                                and a.SetQty>b.StockQty
                                                                            )";
                                    var p1 = new DynamicParameters();
                                    p1.Add("@CoID", CoID);
                                    p1.Add("@RecordID", RecordID);
                                    p1.Add("@WarehouseID", m.WarehouseID);
                                    p1.Add("@Creator", UserName);
                                    p1.Add("@CreateDate", DateTime.Now);
                                    int j = DbBase.CoreDB.Execute(InsertInvItemSql, p1);
                                    #endregion
                                }
                            }
                        }
                        #endregion
                        #region 生成盘点明细-盘亏
                        string gsubsql = @"select distinct TmpInvTable.WarehouseID,TmpInvTable.WarehouseName 
                                    from TmpInvTable,Inventory
                                    where TmpInvTable.SkuID=Inventory.SkuID
                                    and TmpInvTable.WarehouseID=Inventory.WarehouseID
                                    and TmpInvTable.CoID=Inventory.CoID
                                    and TmpInvTable.SetQty<Inventory.StockQty
                                    and TmpInvTable.CoID = @CoID";
                        var GsubLst = DbBase.CoreDB.Query<SetInvQtyExcel>(gsubsql, new { CoID = CoID }).AsList();
                        if (GsubLst.Count > 0)
                        {
                            upttype = true;
                            var Rd = CommHaddle.GetRecordID(CoID);
                            foreach (var m in GsubLst)
                            {
                                row = row + 1;
                                string RecordID = "INV" + (Convert.ToInt64(Rd) + row + 1).ToString();//单据编号  
                                #region 期初主表新增
                                string InsertInvSql = @"insert into invinout(RecordID,Type,CusType,Status,
                                                                WhID,WhName,IsExport,Creator,
                                                                CreateDate,CoID)
                                                         values (@RecordID,1,'盘亏','审核通过',
                                                                @WarehouseID,@WarehouseName,0,@Creator,
                                                                @CreateDate,@CoID)";
                                var p = new DynamicParameters();
                                p.Add("@CoID", CoID);
                                p.Add("@RecordID", RecordID);
                                p.Add("@WarehouseID", m.WarehouseID);
                                p.Add("@WarehouseName", m.WarehouseName);
                                p.Add("@Creator", UserName);
                                p.Add("@CreateDate", DateTime.Now);
                                int i = DbBase.CoreDB.Execute(InsertInvSql, p);
                                #endregion
                                if (i > 0)
                                {
                                    #region 盘点明细新增
                                    string InsertInvItemSql = @"INSERT INTO invinoutitem (
	                                                                            IoID,CoID,SkuID,SkuName,
	                                                                            CusType,Norm,WhID,WhName,
	                                                                            Creator,CreateDate,Qty
                                                                            )(
	                                                                            SELECT
		                                                                            @RecordID,a.CoID,b.SkuID,b.NAME,
		                                                                            '盘亏',b.Norm,a.WarehouseID,b.WarehouseName,
		                                                                            @Creator ,@CreateDate,(a.SetQty - b.StockQty)
	                                                                            from TmpInvTable AS a,
                                                                                     Inventory AS b 
                                                                                where a.SkuID = b.SkuID
                                                                                and a.WarehouseID = b.WarehouseID
                                                                                and a.CoID = b.CoID
                                                                                and a.CoID=@CoID
                                                                                and a.WarehouseID = @WarehouseID
                                                                                and a.SetQty<b.StockQty
                                                                            )";
                                    var p1 = new DynamicParameters();
                                    p1.Add("@CoID", CoID);
                                    p1.Add("@RecordID", RecordID);
                                    p1.Add("@WarehouseID", m.WarehouseID);
                                    p1.Add("@Creator", UserName);
                                    p1.Add("@CreateDate", DateTime.Now);
                                    int j = DbBase.CoreDB.Execute(InsertInvItemSql, p1);
                                    #endregion
                                }
                            }
                        }
                        #endregion
                        #region 库存更新
                        if (upttype)
                        {
                            string UpdateInvsql = @"UPDATE inventory,TmpInvTable
                                            SET inventory.StockQty = (
	                                            SELECT
		                                            sum(Qty)
	                                            FROM
		                                            invinoutitem
	                                            WHERE
		                                            invinoutitem.SkuID = inventory.SkuID
	                                            AND invinoutitem.WhID = inventory.WarehouseID
	                                            AND invinoutitem.CoID = inventory.CoID
                                                AND invinoutitem.IsUpdate = 1
                                            )
                                            WHERE
                                                TmpInvTable.SkuID = inventory.SkuID
	                                            AND TmpInvTable.WarehouseID = inventory.WarehouseID
	                                            AND TmpInvTable.CoID = inventory.CoID
	                                            AND inventory.CoID = @CoID
                                                AND inventory.WarehouseID IN @WhIDLst";
                            var WhIDLst = DLst.Select(a => a.WarehouseID).Distinct().ToList();
                            int s = DbBase.CoreDB.Execute(UpdateInvsql, new { CoID = CoID, WhIDLst = WhIDLst });
                            TransCore.Commit();
                        }
                        #endregion
                    }
                }
                catch (Exception e)
                {
                    TransCore.Rollback();
                    res.s = -1;
                    res.d = e.Message;
                }
                finally
                {
                    conn.Dispose();
                    TransCore.Dispose();
                }
            }
            return res;
        }
        #endregion










        #region 新增库存交易
        public static string AddInvinoutSql()
        {
            string sql = @"INSERT INTO invinout
                            (   
                                RecordID,
                                Type,
                                CusType,
                                `Status`,
                                WhID,
                                WhName,
                                IsExport,
                                RecID,
                                InvoiceID,
                                Creator,
                                CreateDate,
                                CoID ) VALUES(
                                @RecordID,
                                @Type,
                                @CusType,
                                @Status,
                                @WhID,
                                @WhName,
                                @IsExport,
                                @RecID,
                                @InvoiceID,
                                @Creator,
                                @CreateDate,
                                @CoID   )";
            return sql;
        }
        public static string AddInvinoutitemSql()
        {
            string sql = @"INSERT INTO Invinoutitem
                            (
                                IoID,
                                CoID,
                                SkuID,
                                SkuName,
                                Norm,
                                Qty,
                                Unit,
                                WhID,
                                WhName,
                                Creator,
                                CreateDate,
                                CusType ) VALUES(
                                @IoID,
                                @CoID,
                                @SkuID,
                                @SkuName,
                                @Norm,
                                @Qty,
                                @Unit,
                                @WhID,
                                @WhName,
                                @Creator,
                                @CreateDate,
                                @CusType)";
            return sql;
        }
        public static Invinout AddInvinout(InvinoutAuto IOauto)
        {
            var inout = new Invinout();
            inout.CoID = IOauto.CoID;
            inout.Status = "审核通过";
            inout.CusType = IOauto.CusType;
            inout.Type = IOauto.Type;
            inout.RecordID = IOauto.RecordID;
            inout.WhID = IOauto.inv.WarehouseID;
            inout.WhName = IOauto.inv.WarehouseName;
            inout.Creator = IOauto.UserName;
            inout.CreateDate = DateTime.Now;
            inout.IsExport = false;
            return inout;
        }

        public static Invinoutitem AddInvinoutitem(InvinoutAuto IOauto)
        {
            var item = new Invinoutitem();
            item.CoID = IOauto.CoID;
            item.CusType = IOauto.CusType;
            item.IoID = IOauto.RecordID;
            item.WhID = IOauto.inv.WarehouseID;
            item.WhName = IOauto.inv.WarehouseName;
            item.SkuID = IOauto.inv.SkuID;
            item.SkuName = IOauto.inv.Name;
            item.Qty = Convert.ToInt32(IOauto.Qty);
            item.Unit = "件";
            item.Creator = IOauto.UserName;
            item.CreateDate = DateTime.Now;
            return item;
        }

        public static List<Invinout> AddInvinoutLst(InvinoutAuto IOauto)
        {
            var inoutLst = new List<Invinout>();
            foreach (var Inv in IOauto.InvLst)
            {
                var inout = new Invinout();
                inout.CoID = IOauto.CoID;
                inout.Status = "审核通过";
                inout.CusType = IOauto.CusType;
                inout.Type = IOauto.Type;
                inout.RecordID = IOauto.RecordID;
                inout.WhID = IOauto.inv.WarehouseID;
                inout.WhName = IOauto.inv.WarehouseName;
                inout.Creator = IOauto.UserName;
                inout.CreateDate = DateTime.Now;
                inout.IsExport = false;
                inoutLst.Add(inout);
            }
            return inoutLst;
        }
        public static List<Invinoutitem> AddInvinoutitemLst(InvinoutAuto IOauto)
        {
            var ItemLst = new List<Invinoutitem>();
            foreach (var Inv in IOauto.InvLst)
            {
                var item = new Invinoutitem();
                item.CoID = IOauto.CoID;
                item.CusType = IOauto.CusType;
                item.IoID = IOauto.RecordID;
                item.WhID = IOauto.inv.WarehouseID;
                item.WhName = IOauto.inv.WarehouseName;
                item.SkuID = IOauto.inv.SkuID;
                item.SkuName = IOauto.inv.Name;
                item.Qty = Convert.ToInt32(0 - IOauto.inv.StockQty);
                item.Unit = "件";
                item.Creator = IOauto.UserName;
                item.CreateDate = DateTime.Now;
                ItemLst.Add(item);
            }
            return ItemLst;
        }
        #endregion        
    }
}