using CoreModels;
using Dapper;
using System.Collections.Generic;
// using System.Linq;
using System;
using System.Text;
using CoreModels.XyCore;
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

        #region 商品交易查询
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

        #region 修改现有库存-查询单笔库存明细
        public static DataResult GetInventorySingle(string SkuID, int WarehouseID, int CoID)
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
                    if (!string.IsNullOrEmpty(SkuID))//商品编号
                    {
                        querysql.Append(" AND SkuID = @SkuID");
                        p.Add("@SkuID", SkuID);
                    }
                    if (WarehouseID > 0)//商品名称
                    {
                        querysql.Append(" AND WarehouseID = @WarehouseID");
                        p.Add("@WarehouseID", WarehouseID);
                    }
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

        #region 修改现有库存-产生盘点交易
        public static DataResult SetStockQtySingle(int ID,decimal SetQty,int CoID,string UserName)
        {
            var res = new DataResult(1, null);
            var inout = new InvinoutAuto();       
            inout.CoID = CoID;
            inout.UserName = UserName;         
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    var invsql = "SELECT * FROM inventory WHERE ID = @ID";
                    var args = new {ID = ID};
                    var inv = conn.QueryFirst<Inventory>(invsql,args);
                    if(inv==null)
                    {
                        res.s = -1;
                        res.d = "获取库存失败";
                    }
                    inout.Qty = inv.StockQty - SetQty;
                    if(inout.Qty>0)
                    {
                        inout.CusType = "盘盈";
                        inout.Type = 1;
                    }
                    else
                    {
                        inout.CusType = "盘亏";
                        inout.Type = 2;
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

        #region 新增库存交易
        public static Invinout AddInvinout(InvinoutAuto IOauto)
        {
           var inout = new Invinout();
           inout.CoID = IOauto.CoID;
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

        // public static Invinoutitem AddInvinoutitem(InvinoutAuto IOauto)
        // {
        //     var item = Invinoutitem();
        //     item.
        //     return item;
        // }
        #endregion
    }
}