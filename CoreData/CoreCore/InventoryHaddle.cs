using CoreModels;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using System.Data;
using CoreModels.XyCore;
using CoreModels.XyComm;
using CoreModels.Enum;
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
                    //获取本公司商品Skuautoid - 提供资料过滤
                    StringBuilder queryskusql = new StringBuilder();
                    StringBuilder querycount = new StringBuilder();
                    StringBuilder querysql = new StringBuilder();
                    bool is_subsql = false;
                    string skusql = @"SELECT ID FROM coresku WHERE CoID=@CoID AND Type<2 ";
                    queryskusql.Append(skusql);
                    var p = new DynamicParameters();
                    p.Add("@CoID", IParam.CoID);
                    if (!string.IsNullOrEmpty(IParam.GoodsCode))//款式编号
                    {
                        queryskusql.Append(" AND coresku.GoodsCode like @GoodsCode");
                        p.Add("@GoodsCode", "%" + IParam.GoodsCode + "%");
                        is_subsql = true;
                    }
                    if (!string.IsNullOrEmpty(IParam.SkuID))//商品编号
                    {
                        queryskusql.Append(" AND coresku.SkuID like @SkuID");
                        p.Add("@SkuID", "%" + IParam.SkuID + "%");
                        is_subsql = true;
                    }
                    if (!string.IsNullOrEmpty(IParam.SkuName))//商品名称
                    {
                        queryskusql.Append(" AND coresku.SkuName like @SkuName");
                        p.Add("@SkuName", "%" + IParam.SkuName + "%");
                        is_subsql = true;
                    }
                    if (!string.IsNullOrEmpty(IParam.Norm))//商品名称
                    {
                        queryskusql.Append(" AND coresku.Norm like @Norm");
                        p.Add("@Norm", "%" + IParam.Norm + "%");
                        is_subsql = true;
                    }
                    string countsql = @"SELECT
                                            count(ID)
                                        FROM
                                            inventory_sale
                                        WHERE
                                            CoID = @CoID";
                    string sql = @"SELECT
                                        ID,
                                        Skuautoid,
                                        StockQty,
                                        InvLockQty,
                                        LockQty,
                                        PickQty,
                                        WaitInQty,
                                        SaleRetuQty,
                                        SafeQty,
                                        UpSafeQty,
                                        DefectiveQty,
                                        VirtualQty,
                                        PurchaseQty,
                                        CoID
                                    FROM
                                        inventory_sale
                                    WHERE
                                        CoID = @CoID AND IsDelete=0";
                    querycount.Append(countsql);
                    querysql.Append(sql);
                    if (IParam.StockQtyb < IParam.StockQtye && IParam.StockQtyb > 0)
                    {
                        querycount.Append(" AND inventory_sale.StockQty > @StockQtyb AND inventory_sale.StockQty < @StockQtye");
                        querysql.Append(" AND inventory_sale.StockQty > @StockQtyb AND inventory_sale.StockQty < @StockQtye");
                        p.Add("@StockQtyb", IParam.StockQtyb);
                        p.Add("@StockQtye", IParam.StockQtye);
                    }
                    if (IParam.Status > 0)//库存状态:0.全部,1.充足,2.预警
                    {
                        if (IParam.Status == 1)
                        {
                            querycount.Append(" AND inventory_sale.StockQty>=inventory_sale.SafeQty");
                            querysql.Append(" AND inventory_sale.StockQty>=inventory_sale.SafeQty");
                        }
                        else
                        {
                            querycount.Append(" AND inventory_sale.StockQty<inventory_sale.SafeQty");
                            querysql.Append(" AND inventory_sale.StockQty<inventory_sale.SafeQty");
                        }
                    }
                    if (is_subsql)
                    {
                        querycount.Append(" AND Skuautoid in (" + queryskusql + ") ");
                        querysql.Append(" AND Skuautoid in (" + queryskusql + ") ");
                    }
                    //排序
                    if (!string.IsNullOrEmpty(IParam.SortField) && !string.IsNullOrEmpty(IParam.SortDirection))//排序
                    {
                        querysql.Append(" ORDER BY " + IParam.SortField + " " + IParam.SortDirection);
                    }
                    var DataCount = conn.QueryFirst<int>(querycount.ToString(), p);
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
                        var InvLst = conn.Query<Inventory_sale>(querysql.ToString(), p).AsList();
                        if (InvLst.Count > 0)
                        {
                            var SkuIDLst = InvLst.Select(a => a.Skuautoid).Distinct().AsList();
                            var res1 = CommHaddle.GetSkuViewByID(IParam.CoID.ToString(), SkuIDLst);
                            var SkuViewLst = res1.d as List<CoreSkuView>;//获取商品Sku资料,拼接Lst显示
                            InvLst = (from a in InvLst
                                      join b in SkuViewLst on a.Skuautoid equals b.ID into data
                                      from c in data.DefaultIfEmpty()
                                      select new Inventory_sale
                                      {
                                          ID = a.ID,
                                          Skuautoid = a.Skuautoid,
                                          StockQty = a.StockQty,
                                          SafeQty = a.SafeQty,
                                          UpSafeQty = a.UpSafeQty,
                                          LockQty = a.LockQty,
                                          SaleRetuQty = a.SaleRetuQty,
                                          WaitInQty = a.WaitInQty,
                                          VirtualQty = a.VirtualQty,
                                          CoID = a.CoID,
                                          GoodsCode = c == null ? "" : c.GoodsCode,
                                          SkuID = c == null ? "" : c.SkuID,
                                          Name = c == null ? "" : c.SkuName,
                                          Norm = c == null ? "" : c.Norm,
                                          Img = c == null ? "" : c.Img,
                                      }).AsList();
                        }
                        inv.InvMainLst = InvLst;
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
        #region 商品库存查询(分仓)
        public static DataResult GetInvQueryByWh(InvQueryParam IParam)
        {
            var inv = new InventoryData();
            var result = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    //获取本公司商品Skuautoid - 提供资料过滤
                    StringBuilder queryskusql = new StringBuilder();
                    StringBuilder querycount = new StringBuilder();
                    StringBuilder querysql = new StringBuilder();
                    string skusql = @"SELECT ID FROM coresku WHERE CoID=@CoID AND Type<2 ";
                    queryskusql.Append(skusql);
                    var p = new DynamicParameters();
                    p.Add("@CoID", IParam.CoID);
                    if (!string.IsNullOrEmpty(IParam.GoodsCode))//款式编号
                    {
                        queryskusql.Append(" AND coresku.GoodsCode like @GoodsCode");
                        p.Add("@GoodsCode", "%" + IParam.GoodsCode + "%");
                    }
                    if (!string.IsNullOrEmpty(IParam.SkuID))//商品编号
                    {
                        queryskusql.Append(" AND coresku.SkuID like @SkuID");
                        p.Add("@SkuID", "%" + IParam.SkuID + "%");
                    }
                    if (!string.IsNullOrEmpty(IParam.SkuName))//商品名称
                    {
                        queryskusql.Append(" AND coresku.SkuName like @SkuName");
                        p.Add("@SkuName", "%" + IParam.SkuName + "%");
                    }
                    if (!string.IsNullOrEmpty(IParam.Norm))//商品名称
                    {
                        queryskusql.Append(" AND coresku.Norm like @Norm");
                        p.Add("@Norm", "%" + IParam.Norm + "%");
                    }
                    // var SkuautoidLst = conn.Query(queryskusql+" ORDER BY ID", p1).AsList();//本仓商品autoid
                    string countsql = @"SELECT count(ID) FROM inventory WHERE CoID in @CoIDLst AND IsDelete=0 AND Skuautoid in (" + queryskusql + " ORDER BY ID)";
                    string sql = @"SELECT
                                    inventory.ID,
                                    inventory.Skuautoid,
                                    inventory.StockQty,
                                    inventory.PickQty,
                                    inventory.WaitInQty,
                                    inventory.SaleRetuQty,
                                    inventory.SafeQty,
                                    inventory.DefectiveQty,
                                    inventory.VirtualQty,
                                    inventory.PurchaseQty,
                                    Inventory.WarehouseID,
                                    inventory.CoID
                                   FROM
                                    inventory
                                   WHERE CoID in @CoIDLst AND IsDelete=0 AND Skuautoid in (" + queryskusql + " ORDER BY ID)";
                    querycount.Append(countsql);
                    querysql.Append(sql);
                    //获取第三方仓公司ID
                    var res = CommHaddle.GetWareCoidList(IParam.CoID.ToString());
                    var CoIDLst = res.d as List<string>;
                    if (CoIDLst.Count > 0)
                    {
                        // var p2 = new DynamicParameters();
                        p.Add("@CoIDLst", CoIDLst);
                        if (IParam.StockQtyb < IParam.StockQtye && IParam.StockQtyb > 0)
                        {
                            querycount.Append(" AND inventory.StockQty > @StockQtyb AND inventory.StockQty < @StockQtye");
                            querysql.Append(" AND inventory.StockQty > @StockQtyb AND inventory.StockQty < @StockQtye");
                            p.Add("@StockQtyb", IParam.StockQtyb);
                            p.Add("@StockQtye", IParam.StockQtye);
                        }
                        if (IParam.WarehouseID > 0)
                        {
                            querycount.Append(" AND inventory.CoID = @WarehouseID");
                            querysql.Append(" AND inventory.CoID = @WarehouseID");
                            p.Add("@WarehouseID", IParam.WarehouseID);
                        }
                        if (IParam.Status > 0)//库存状态:0.全部,1.充足,2.预警
                        {
                            if (IParam.Status == 1)
                            {
                                querycount.Append(" AND inventory.StockQty>=inventory.SafeQty");
                                querysql.Append(" AND inventory.StockQty>=inventory.SafeQty");
                            }
                            else
                            {
                                querycount.Append(" AND inventory.StockQty<inventory.SafeQty");
                                querysql.Append(" AND inventory.StockQty<inventory.SafeQty");
                            }
                        }
                        if (!string.IsNullOrEmpty(IParam.SortField) && !string.IsNullOrEmpty(IParam.SortDirection))//排序
                        {
                            querycount.Append(" ORDER BY " + IParam.SortField + " " + IParam.SortDirection);
                            querysql.Append(" ORDER BY " + IParam.SortField + " " + IParam.SortDirection);
                        }
                        var DataCount = conn.QueryFirst<int>(querycount.ToString(), p);
                        if (DataCount < 0)
                        {
                            result.s = -3001;
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
                            var InvLst = conn.Query<Inventory>(querysql.ToString(), p).AsList();
                            if (InvLst.Count > 0)
                            {
                                var SkuIDLst = InvLst.Select(a => a.Skuautoid).Distinct().AsList();
                                var res1 = CommHaddle.GetSkuViewByID(IParam.CoID.ToString(), SkuIDLst);
                                var res2 = CommHaddle.GetWareThirdList(IParam.CoID.ToString()); ;
                                var WhLst = res2.d as List<Warehouse_view>;
                                var SkuViewLst = res1.d as List<CoreSkuView>;//获取商品Sku资料,拼接Lst显示
                                InvLst = (from a in InvLst
                                          join b in SkuViewLst on a.Skuautoid equals b.ID into data
                                          from c in data.DefaultIfEmpty()
                                          join w in WhLst on a.CoID equals w.CoID into enddata
                                          from e in enddata.DefaultIfEmpty()
                                          select new Inventory
                                          {
                                              ID = a.ID,
                                              Skuautoid = a.Skuautoid,
                                              StockQty = a.StockQty,
                                              SafeQty = a.SafeQty,
                                              SaleRetuQty = a.SaleRetuQty,
                                              WaitInQty = a.WaitInQty,
                                              VirtualQty = a.VirtualQty,
                                              WarehouseID = e == null ? "" : e.CoID,
                                              WarehouseName = e == null ? "" : e.WhName,
                                              CoID = a.CoID,
                                              GoodsCode = c == null ? "" : c.GoodsCode,
                                              SkuID = c == null ? "" : c.SkuID,
                                              Name = c == null ? "" : c.SkuName,
                                              Norm = c == null ? "" : c.Norm,
                                              Img = c == null ? "" : c.Img
                                          }).AsList();
                            }
                            inv.InvLst = InvLst;
                            result.d = inv;
                        }
                    }
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d = e.Message;
                }
                finally
                {
                    conn.Dispose();
                }
            }
            return result;
        }
        #endregion

        #region 商品库存查询 - 商品交易查询
        public static DataResult GetInvDetailQuery(InvQueryParam IParam)
        {
            var inv = new InvItemData();
            var result = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    StringBuilder querycount = new StringBuilder();
                    StringBuilder querysql = new StringBuilder();
                    var p = new DynamicParameters();
                    string countsql = @"SELECT count(ID) FROM invinoutitem WHERE CoID IN @CoIDLst AND Status = 1";
                    string sql = @"
                                   SELECT
                                        Invinoutitem.ID,
                                        invinoutitem.IoID,
                                        invinoutitem.CoID,
                                        invinoutitem.Skuautoid,
                                        invinoutitem.Qty,
                                        invinoutitem.Creator,
                                        invinoutitem.CreateDate,
                                        invinoutitem.Type,
                                        invinoutitem.CusType,
                                        invinoutitem.WhID,
                                        invinoutitem.LinkWhID,
                                        invinoutitem.`Status`,
                                        invinoutitem.RefID,
                                        invinoutitem.OID
                                   FROM
                                        invinoutitem
                                   WHERE CoID IN @CoIDLst AND Status = 1";
                    querycount.Append(countsql);
                    querysql.Append(sql);
                    // p.Add("@CoID", IParam.CoID);
                    //获取第三方仓公司ID
                    var res = CommHaddle.GetWareCoidList(IParam.CoID.ToString());
                    var CoIDLst = res.d as List<string>;
                    p.Add("@CoIDLst", CoIDLst);

                    if (!string.IsNullOrEmpty(IParam.SkuID))//商品编号
                    {
                        querycount.Append(" AND Skuautoid = @SkuID");
                        querysql.Append(" AND Skuautoid = @SkuID");
                        p.Add("@SkuID", IParam.SkuID);
                    }
                    if (IParam.WarehouseID > 0)//仓库编号
                    {
                        querycount.Append(" AND CoID = @WarehouseID");
                        querysql.Append(" AND CoID = @WarehouseID");
                        p.Add("@WarehouseID", IParam.WarehouseID);
                    }
                    if (!string.IsNullOrEmpty(IParam.DocType))//单据类型
                    {
                        querycount.Append(" AND Type = @DocType");
                        querysql.Append(" AND Type = @DocType");
                        p.Add("@DocType", IParam.DocType);
                    }
                    // if (!string.IsNullOrEmpty(IParam.RecordID))
                    // {
                    //     querycount.Append(" AND IoID = @IoID");
                    //     querysql.Append(" AND IoID = @IoID");
                    //     p.Add("@IoID", IParam.RecordID);
                    // }
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
                        result.s = -3001;
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
                        result.d = inv;
                    }
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d = e.Message;
                }
                finally
                {
                    conn.Dispose();
                }
            }
            return result;
        }
        #endregion

        #region 商品库存查询 - 修改现有库存-查询单笔库存明细
        public static DataResult GetInventorySingle(int ID, string CoID)
        {
            var result = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    var res = WarehouseHaddle.wareSettingGet(CoID);
                    if (res.s == 1)
                    {
                        var ware = res.d as ware_m_setting;
                        if (ware.IsPositionAccurate == "1")//库存精确化管理
                        {
                            result.s = -3011;
                        }
                        else
                        {
                            StringBuilder querysql = new StringBuilder();
                            string sql = @"SELECT ID,Skuautoid,StockQty FROM inventory WHERE CoID=@CoID AND ID=@ID";
                            var p = new DynamicParameters();
                            querysql.Append(sql);
                            p.Add("@CoID", CoID);
                            p.Add("@ID", ID);
                            var invLst = CoreData.DbBase.CoreDB.Query<Sfc_InvStock>(querysql.ToString(), p).AsList();
                            if (invLst.Count == 0)
                            {
                                result.s = -3001;
                            }
                            else
                            {
                                string skuid = conn.QueryFirst<string>("SELECT SkuID FROM coresku WHERE CoID=@CoID AND ID = @ID", new { CoID = CoID, ID = invLst[0].Skuautoid });
                                invLst[0].SkuID = skuid;
                                result.d = invLst[0];
                            }
                        }
                    }
                    else
                    {
                        result.s = res.s;
                        result.d = res.d;
                    }
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d = e.Message;
                }
                finally
                {
                    conn.Dispose();
                }
            }
            return result;
        }
        #endregion


        #region 商品库存查询 - 修改现有库存-产生盘点交易(分仓)
        public static DataResult SetTakeStockQty(int ID, int sfc_type, int inv_type, int InvQty, string CoID, string UserName)
        {
            var result = new DataResult(1, null);
            var conn = new MySqlConnection(DbBase.CoreConnectString);
            conn.Open();
            string sfc_TypeName = Enum.GetName(typeof(InvE.SfcMainTypeE), sfc_type).ToString();//单据类型
            string CusType = Enum.GetName(typeof(InvE.InvType), inv_type).ToString();//交易类型
            var RecordID = "INV" + CommHaddle.GetRecordID(int.Parse(CoID));
            int Status = 1;//(0:待审核;1.审核通过;2.作废)       
            var Trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
            try
            {
                var res = WarehouseHaddle.wareSettingGet(CoID);
                if (res.s == 1)
                {
                    var ware = res.d as ware_m_setting;
                    if (ware.IsPositionAccurate == "1")//库存精确化管理
                    {
                        result.s = -3011;
                    }
                    else
                    {
                        StringBuilder querysql = new StringBuilder();
                        string sql = @"SELECT ID,Skuautoid,StockQty,WarehouseID FROM inventory WHERE CoID=@CoID AND ID=@ID";
                        var p = new DynamicParameters();
                        querysql.Append(sql);
                        p.Add("@CoID", CoID);
                        p.Add("@ID", ID);
                        var wh_inv = conn.QueryFirst<Inventory>(querysql.ToString(), p);
                        //盘点主表
                        var main = new Sfc_main();
                        main.CoID = CoID;
                        main.Creator = UserName;
                        main.CreateDate = DateTime.Now.ToString();
                        main.Type = sfc_type;
                        main.Status = Status;//已确认生效
                        main.WhID = wh_inv.WarehouseID;
                        main.Parent_WhID = wh_inv.WarehouseID;
                        conn.Execute(StockTakeHaddle.AddSfcMainSql(), main, Trans);
                        var ParentID = conn.QueryFirst<string>("select LAST_INSERT_ID()", Trans);
                        //盘点子表
                        var item = new Sfc_item();
                        item.ParentID = ParentID;
                        item.CoID = CoID;
                        item.Creator = UserName;
                        item.CreateDate = DateTime.Now.ToString();
                        item.Type = sfc_type;
                        item.WhID = wh_inv.WarehouseID;
                        item.Parent_WhID = wh_inv.WarehouseID;
                        item.Skuautoid = wh_inv.Skuautoid;
                        item.InvQty = InvQty;//盘点数量
                        item.Qty = InvQty - Convert.ToInt32(wh_inv.StockQty);//交易数量                       
                        conn.Execute(StockTakeHaddle.AddSfcItemSql(), item, Trans);
                        //交易主表
                        var inv = new Invinout();
                        inv.RefID = ParentID;
                        inv.RecordID = RecordID;
                        inv.Type = inv_type;
                        inv.CusType = CusType;
                        inv.Status = Status;
                        inv.WhID = wh_inv.WarehouseID;
                        inv.LinkWhID = wh_inv.WarehouseID;
                        inv.Creator = UserName;
                        inv.CreateDate = DateTime.Now.ToString();
                        inv.CoID = CoID;
                        conn.Execute(AddInvinoutSql(), inv, Trans);
                        //交易子表
                        var inv_item = new Invinoutitem();
                        inv_item.RefID = ParentID;
                        inv_item.IoID = RecordID;
                        inv_item.Type = inv_type;
                        inv_item.CusType = CusType;
                        inv_item.Status = Status;
                        inv_item.WhID = wh_inv.WarehouseID;
                        inv_item.LinkWhID = wh_inv.WarehouseID;
                        inv_item.Skuautoid = wh_inv.Skuautoid;
                        inv_item.Qty = InvQty - Convert.ToInt32(wh_inv.StockQty);//交易数量 
                        inv_item.Creator = UserName;
                        inv_item.CreateDate = DateTime.Now.ToString();
                        inv_item.CoID = CoID;
                        conn.Execute(AddInvinoutitemSql(), inv_item, Trans);
                        //更新库存数量                   
                        var SkuIDLst = new List<int>();
                        SkuIDLst.Add(inv_item.Skuautoid);
                        conn.Execute(InventoryHaddle.UptInvStockQtySql(), new { CoID = CoID, SkuIDLst = SkuIDLst, Modifier = UserName, ModifyDate = DateTime.Now.ToString() }, Trans);
                        //获取第三方仓公司ID
                        res = CommHaddle.GetWareCoidList(CoID);
                        var CoIDLst = res.d as List<string>;
                        conn.Execute(InventoryHaddle.UptInvMainStockQtySql(), new { CoID = CoID, CoIDLst = CoIDLst, SkuIDLst = SkuIDLst, Modifier = UserName, ModifyDate = DateTime.Now.ToString() }, Trans);
                        Trans.Commit();
                        CoreUser.LogComm.InsertUserLog("修改库存数量-盘点数量", "Inventory", "单据ID" + ParentID, UserName, int.Parse(CoID), DateTime.Now);
                    }
                }
                else
                {
                    result.s = res.s;
                    result.d = res.d;
                }
            }
            catch (Exception e)
            {
                Trans.Rollback();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                Trans.Dispose();
                conn.Dispose();
                conn.Close();
            }
            return result;
        }
        #endregion


        #region 商品库存查询 - 库存清零(分仓)
        public static DataResult ClearInvQty(List<int> IDLst, int sfc_type, int inv_type, string CoID, string UserName)
        {
            var result = new DataResult(1, null);
            var conn = new MySqlConnection(DbBase.CoreConnectString);
            conn.Open();
            //参数定义
            string sfc_TypeName = Enum.GetName(typeof(InvE.SfcMainTypeE), sfc_type).ToString();//单据类型
            string CusType = Enum.GetName(typeof(InvE.InvType), inv_type).ToString();//交易类型
            var RecordID = "INV" + CommHaddle.GetRecordID(int.Parse(CoID));
            int Status = 1;//(0:待审核;1.审核通过;2.作废)      
            var Trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
            try
            {
                var res = WarehouseHaddle.wareSettingGet(CoID);
                if (res.s == 1)
                {
                    var ware = res.d as ware_m_setting;
                    if (ware.IsPositionAccurate == "1")//库存精确化管理
                    {
                        result.s = -3011;
                    }
                    else
                    {
                        StringBuilder querysql = new StringBuilder();
                        string sql = @"SELECT ID,Skuautoid,StockQty,WarehouseID FROM inventory WHERE CoID=@CoID AND IsDelete=0 AND StockQty>0";
                        var p = new DynamicParameters();
                        querysql.Append(sql);
                        p.Add("@CoID", CoID);
                        if (IDLst.Count > 0)
                        {
                            querysql.Append(" AND ID in @IDLst");
                            p.Add("@IDLst", IDLst);
                        }
                        var wh_invLst = conn.Query<Inventory>(querysql.ToString(), p).AsList();
                        if (wh_invLst.Count > 0)
                        {
                            //盘点主表
                            var main = new Sfc_main();
                            main.CoID = CoID;
                            main.Creator = UserName;
                            main.CreateDate = DateTime.Now.ToString();
                            main.Type = sfc_type;
                            main.Status = Status;//已确认生效
                            main.WhID = wh_invLst[0].WarehouseID;
                            main.Parent_WhID = wh_invLst[0].WarehouseID;
                            conn.Execute(StockTakeHaddle.AddSfcMainSql(), main, Trans);
                            var ParentID = conn.QueryFirst<string>("select LAST_INSERT_ID()", Trans);
                            //盘点子表
                            var itemLst = new List<Sfc_item>();
                            foreach (var w in wh_invLst)
                            {
                                var item = new Sfc_item();
                                item.ParentID = ParentID;
                                item.CoID = CoID;
                                item.Creator = UserName;
                                item.CreateDate = DateTime.Now.ToString();
                                item.Type = sfc_type;
                                item.WhID = w.WarehouseID;
                                item.Parent_WhID = w.WarehouseID;
                                item.Skuautoid = w.Skuautoid;
                                item.InvQty = 0;//盘点数量
                                item.Qty = 0 - Convert.ToInt32(w.StockQty);//交易数量   
                                itemLst.Add(item);
                            }
                            conn.Execute(StockTakeHaddle.AddSfcItemSql(), itemLst, Trans);
                            //交易主表
                            var inv = new Invinout();
                            inv.RefID = ParentID;
                            inv.RecordID = RecordID;
                            inv.Type = inv_type;
                            inv.CusType = CusType;
                            inv.Status = Status;
                            inv.WhID = wh_invLst[0].WarehouseID;
                            inv.LinkWhID = wh_invLst[0].WarehouseID;
                            inv.Creator = UserName;
                            inv.CreateDate = DateTime.Now.ToString();
                            inv.CoID = CoID;
                            conn.Execute(AddInvinoutSql(), inv, Trans);
                            //交易子表
                            var inv_itemLst = new List<Invinoutitem>();
                            foreach (var w in wh_invLst)
                            {
                                var inv_item = new Invinoutitem();
                                inv_item.RefID = ParentID;
                                inv_item.IoID = RecordID;
                                inv_item.Type = inv_type;
                                inv_item.CusType = CusType;
                                inv_item.Status = Status;
                                inv_item.WhID = w.WarehouseID;
                                inv_item.LinkWhID = w.WarehouseID;
                                inv_item.Skuautoid = w.Skuautoid;
                                inv_item.Qty = 0 - Convert.ToInt32(w.StockQty);//交易数量 
                                inv_item.Creator = UserName;
                                inv_item.CreateDate = DateTime.Now.ToString();
                                inv_item.CoID = CoID;
                                inv_itemLst.Add(inv_item);
                            }
                            conn.Execute(AddInvinoutitemSql(), inv_itemLst, Trans);
                            //更新库存数量       
                            var WhIDLst = wh_invLst.Select(a => a.ID).AsList();
                            string clearsql = @"UPDATE inventory SET StockQty = 0,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE CoID = @CoID AND IsDelete=0 AND ID in @IDLst";
                            int count3 = conn.Execute(clearsql, new { CoID = CoID, Modifier = UserName, ModifyDate = DateTime.Now.ToString(), IDLst = WhIDLst });
                            var SkuIDLst = wh_invLst.Select(a => a.Skuautoid).AsList();
                            //获取第三方仓公司ID
                            res = CommHaddle.GetWareCoidList(CoID);
                            var CoIDLst = res.d as List<string>;
                            conn.Execute(InventoryHaddle.UptInvMainStockQtySql(), new { CoID = CoID, CoIDLst = CoIDLst, SkuIDLst = SkuIDLst, Modifier = UserName, ModifyDate = DateTime.Now.ToString() }, Trans);
                            Trans.Commit();
                            CoreUser.LogComm.InsertUserLog("清空库存数量-盘点数量", "Inventory", "单据ID" + ParentID, UserName, int.Parse(CoID), DateTime.Now);
                        }
                    }
                }
                else
                {
                    result.s = res.s;
                    result.d = res.d;
                }
            }
            catch (Exception e)
            {
                Trans.Rollback();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                Trans.Dispose();
                conn.Dispose();
                conn.Close();
            }
            return result;
        }
        #endregion

        #region 清除零库存资料(分仓|主仓共用)
        public static DataResult DelZeroSku(List<int> IDLst, int whType, string CoID, string UserName)
        {
            var result = new DataResult(1, null);
            var conn = new MySqlConnection(DbBase.CoreConnectString);
            conn.Open();
            var Trans = conn.BeginTransaction();
            try
            {
                string content = string.Empty;
                StringBuilder querysql = new StringBuilder();
                string sql = string.Empty;
                var p = new DynamicParameters();
                p.Add("@CoID", CoID);
                p.Add("@Modifier", UserName);
                p.Add("@ModifyDate", DateTime.Now.ToString());
                if (IDLst.Count > 0)
                {
                    querysql.Append(" AND ID in @IDLst");
                    p.Add("@IDLst", IDLst);
                }
                if (whType == 1)//1.分仓，2.主仓
                {
                    content = "清除本仓零库存";
                    sql = "UPDATE inventory SET IsDelete=1,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE CoID=@CoID AND IsDelete=0 AND StockQty=0";
                }
                else if (whType == 2)
                {
                    content = "清除主仓零库存";
                    sql = "UPDATE inventory_sale SET IsDelete=1,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE CoID=@CoID AND IsDelete=0 AND StockQty=0";
                }
                querysql.Append(sql);
                conn.Execute(sql, new { CoID = CoID, Modifier = UserName, ModifyDate = DateTime.Now.ToString() }, Trans);
                Trans.Commit();
                CoreUser.LogComm.InsertUserLog("清除零库存资料", "Inventory", content, UserName, int.Parse(CoID), DateTime.Now);
            }
            catch (Exception e)
            {
                Trans.Rollback();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                Trans.Dispose();
                conn.Dispose();
                conn.Close();
            }
            return result;
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

        #region 商品库存查询 - 修改安全库存 - 单笔(分仓)      
        //Type: 0.全部更新，1.安全库存下限，2.安全库存上限
        public static DataResult UptSafeQty(InventParams inv)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                conn.Open();
                var Trans = conn.BeginTransaction();
                try
                {
                    string sql = string.Empty;
                    string content = string.Empty;
                    var invOld = conn.QueryFirst<InventParams>("SELECT ID,SafeQty FROM inventory WHERE CoID = @CoID AND ID=@ID", new { CoID = inv.CoID, ID = inv.ID });
                    if (invOld != null)
                    {
                        if (inv.SafeQty != invOld.SafeQty)
                        {
                            content = "安全库存：" + invOld.SafeQty + "=>" + inv.SafeQty;
                            sql = @"UPDATE inventory SET SafeQty = @SafeQty,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE CoID=@CoID AND ID = @ID";
                            conn.Execute(sql, inv, Trans);
                            Trans.Commit();
                            CoreUser.LogComm.InsertUserLog("修改安全库存", "inventory", content, inv.Modifier, int.Parse(inv.CoID), DateTime.Now);
                        }                       
                    }
                }
                catch (Exception e)
                {
                    Trans.Rollback();
                    res.s = -1;
                    res.d = e.Message;
                }
                finally
                {
                    Trans.Dispose();
                    conn.Dispose();
                    conn.Close();
                }
            }

            return res;
        }

        #endregion

        #region 商品库存查询 - 修改安全库存 - 单笔(主仓)      
        //Type: 0.全部更新，1.安全库存下限，2.安全库存上限
        public static DataResult UptMainSafeQty(InventParams inv)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                conn.Open();
                var Trans = conn.BeginTransaction();
                try
                {
                    string sql = string.Empty;
                    string content = string.Empty;
                    // if (inv.Type == 0)
                    // {
                    //     sql = @"UPDATE inventory SET SafeQty = @SafeQty,UpSaveQty=@UpSaveQty,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE CoID=@CoID AND ID = @ID";
                    // }
                    // else 
                    var invOld = conn.QueryFirst<InventParams>("SELECT ID,SafeQty,UpSafeQty FROM inventory_sale WHERE CoID = @CoID AND ID=@ID", new { CoID = inv.CoID, ID = inv.ID });
                    if (invOld != null)
                    {
                        if (inv.Type == 1 && inv.SafeQty != invOld.SafeQty)
                        {
                            content = "安全库存下限：" + invOld.SafeQty + "=>" + inv.SafeQty;
                            sql = @"UPDATE inventory_sale SET SafeQty = @SafeQty,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE CoID=@CoID AND ID = @ID";
                        }
                        else if (inv.Type == 2 && inv.UpSafeQty != invOld.UpSafeQty)
                        {
                            content = "安全库存上限：" + invOld.UpSafeQty + "=>" + inv.UpSafeQty;
                            sql = @"UPDATE inventory_sale SET UpSafeQty=@UpSafeQty,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE CoID=@CoID AND ID = @ID";
                        }
                        if (!string.IsNullOrEmpty(sql))
                        {
                            conn.Execute(sql, inv, Trans);
                            Trans.Commit();
                            CoreUser.LogComm.InsertUserLog("修改安全库存", "inventory_sale", content, inv.Modifier, int.Parse(inv.CoID), DateTime.Now);
                        }
                    }

                }
                catch (Exception e)
                {
                    Trans.Rollback();
                    res.s = -1;
                    res.d = e.Message;
                }
                finally
                {
                    Trans.Dispose();
                    conn.Dispose();
                    conn.Close();
                }
            }

            return res;
        }

        #endregion

        #region 商品库存查询 - 修改安全库存 - 多笔(主仓)  
        public static DataResult UptMainLstSafeQty(List<InventParams> InvLst, string CoID, string UserName)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                conn.Open();
                var Trans = conn.BeginTransaction();
                try
                {
                    InvLst = InvLst.Select(a => new InventParams
                    {
                        ID = a.ID,
                        SafeQty = a.SafeQty,
                        UpSafeQty = a.UpSafeQty,
                        Modifier = UserName,
                        ModifyDate = DateTime.Now.ToString(),
                        CoID = CoID
                    }).AsList();
                    if (InvLst.Count > 0)
                    {
                        string sql = @"UPDATE inventory_sale SET SafeQty = @SafeQty,UpSafeQty=@UpSafeQty,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE CoID=@CoID AND ID = @ID";
                        conn.Execute(sql, InvLst, Trans);
                        Trans.Commit();
                        var InvIDLst = InvLst.Select(a => a.ID).AsList();
                        CoreUser.LogComm.InsertUserLog("修改安全库存", "inventory_sale", "库存ID：" + string.Join(",", InvIDLst.ToArray()), UserName, int.Parse(CoID), DateTime.Now);
                    }
                }
                catch (Exception e)
                {
                    Trans.Rollback();
                    res.s = -1;
                    res.d = e.Message;
                }
                finally
                {
                    Trans.Dispose();
                    conn.Dispose();
                    conn.Close();
                }
            }

            return res;
        }
        #endregion


        #region 商品库存查询 - 清空安全库存 - 所有
        public static DataResult ClearSafeQty(string CoID, string UserName)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                conn.Open();
                var Trans = conn.BeginTransaction();
                try
                {
                    string sql = @"UPDATE inventory_sale SET SafeQty = 0,UpSafeQty=0,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE CoID=@CoID";
                    conn.Execute(sql, new { CoID = CoID, Modifier = UserName, ModifyDate = DateTime.Now.ToString() }, Trans);
                    Trans.Commit();
                    CoreUser.LogComm.InsertUserLog("修改安全库存", "inventory_sale", "清空所有商品安全库存", UserName, int.Parse(CoID), DateTime.Now);
                }
                catch (Exception e)
                {
                    Trans.Rollback();
                    res.s = -1;
                    res.d = e.Message;
                }
                finally
                {
                    Trans.Dispose();
                    conn.Dispose();
                    conn.Close();
                }
            }
            return res;
        }

        #endregion

        #region  商品库存查询 - 修改虚拟库存 - 更新虚拟库存(单笔)
        public static DataResult UptVirtualQty(InventParams inv)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                conn.Open();
                var Trans = conn.BeginTransaction();
                try
                {
                    string sql = string.Empty;
                    string content = string.Empty;
                    var invOld = conn.QueryFirst<InventParams>("SELECT ID,VirtualQty FROM inventory_sale WHERE CoID = @CoID AND ID=@ID", new { CoID = inv.CoID, ID = inv.ID });
                    if (invOld != null)
                    {
                        if (inv.VirtualQty != invOld.VirtualQty)
                        {
                            content = "虚拟库存：" + invOld.VirtualQty + "=>" + inv.VirtualQty;
                            sql = @"UPDATE inventory_sale SET VirtualQty = @VirtualQty,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE CoID=@CoID AND ID = @ID";
                        }
                        if (!string.IsNullOrEmpty(sql))
                        {
                            conn.Execute(sql, inv, Trans);
                            Trans.Commit();
                            CoreUser.LogComm.InsertUserLog("修改虚拟库存", "inventory_sale", content, inv.Modifier, int.Parse(inv.CoID), DateTime.Now);
                        }
                    }
                }
                catch (Exception e)
                {
                    Trans.Rollback();
                    res.s = -1;
                    res.d = e.Message;
                }
                finally
                {
                    Trans.Dispose();
                    conn.Dispose();
                    conn.Close();
                }
            }

            return res;
        }
        #endregion

        #region  商品库存查询 - 修改虚拟库存 - 更新虚拟库存(多笔)
        public static DataResult UptLstVirtualQty(List<int> IDLst,string VirtualQty, string CoID, string UserName)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                conn.Open();
                var Trans = conn.BeginTransaction();
                try
                {                   
                    if (IDLst.Count > 0)
                    {
                        string sql = @"UPDATE inventory_sale SET VirtualQty = @VirtualQty,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE CoID=@CoID AND ID in @IDLst";
                        var p = new DynamicParameters();
                        p.Add("@CoID",CoID);                        
                        p.Add("@IDLst",IDLst);                       
                        p.Add("@VirtualQty",VirtualQty);                       
                        p.Add("@Modifier",UserName);                       
                        p.Add("@ModifyDate",DateTime.Now.ToString());
                        conn.Execute(sql, p, Trans);
                        Trans.Commit();
                        CoreUser.LogComm.InsertUserLog("修改虚拟库存", "inventory_sale", "库存ID：" + string.Join(",", IDLst.ToArray()), UserName, int.Parse(CoID), DateTime.Now);
                    }
                }
                catch (Exception e)
                {
                    Trans.Rollback();
                    res.s = -1;
                    res.d = e.Message;
                }
                finally
                {
                    Trans.Dispose();
                    conn.Dispose();
                    conn.Close();
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
                        CoreUser.LogComm.InsertUserLog("更新商品名称", "Inventory", "更新商品名称", UserName, CoID, DateTime.Now);
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

        #region 商品库存盘点 - 从EXCEL模板导入(自动保存) -Create临时表导入盘点库存
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
                    int recount = DbBase.CoreDB.Execute(sql, TransCore);//新建临时表
                    string trsql = @"truncate TABLE TmpInvTable";
                    DbBase.CoreDB.Execute(trsql, TransCore);
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
                                int i = DbBase.CoreDB.Execute(InsertInvSql, p, TransCore);
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
                                    int j = DbBase.CoreDB.Execute(InsertInvItemSql, p1, TransCore);
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
                                int i = DbBase.CoreDB.Execute(InsertInvSql, p, TransCore);
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
                                    int j = DbBase.CoreDB.Execute(InsertInvItemSql, p1, TransCore);
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
                            int s = DbBase.CoreDB.Execute(UpdateInvsql, new { CoID = CoID, WhIDLst = WhIDLst }, TransCore);
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

        #region 商品期初库存 - 从EXCEL模板导入(自动保存) -Create临时表导入期初库存
        public static DataResult CreateInvInitTemp(string RblType, List<InitInvQtyExcel> InvLst, int WarehouseID, string WarehouseName, int CoID, string UserName)
        {
            var res = new DataResult(1, null);
            var ErLst = new List<InitInvQtyExcel>();
            Boolean upttype = false;
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                conn.Open();
                var TransCore = conn.BeginTransaction();
                try
                {
                    #region 新建临时表
                    string sql = @" CREATE TEMPORARY TABLE IF NOT EXISTS `TmpInvTable` (
                            `GoodsCode`  varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '货品编号' ,
                            `GoodsName`  varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '货品名称' ,
                            `SkuID`  varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '商品编码' ,
                            `Name`  varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '商品名称' ,
                            `ColorName`  varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '颜色名称' ,
                            `ParentSize`  varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '尺码组' ,
                            `SizeName`  varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '尺码名称' ,
                            `Qty`  decimal(11,2) NULL DEFAULT 0.00 COMMENT '库存'  ,
                            `CoID`  int(11) NULL DEFAULT NULL COMMENT '公司ID' 
                            )";
                    int recount = DbBase.CoreDB.Execute(sql, TransCore);//新建临时表
                    string trsql = @"truncate TABLE TmpInvTable";
                    DbBase.CoreDB.Execute(trsql, TransCore);
                    #endregion
                    #region 消除重复项
                    //取出重复list
                    var NKeylst = from l in InvLst
                                  group l by
                                  new { l.SkuID }
                                  into g
                                  where g.Count() > 1
                                  select g.Key;
                    var NLst = InvLst.Where(
                           a => NKeylst.Select(b => b.SkuID)
                               .Contains(a.SkuID)
                               )
                               .ToList();
                    //排除重复项
                    var DLst = InvLst.Where(
                            a => !NKeylst.Select(b => b.SkuID)
                                .Contains(a.SkuID)
                                )
                                .ToList();
                    if (NLst.Count > 0)
                    {

                        var label = new InitInvQtyExcel();
                        label.GoodsCode = "期初资料重复:";
                        ErLst.Add(label);
                        ErLst.AddRange(NLst);
                    }
                    #endregion
                    if (DLst.Count > 0)
                    {
                        #region 数据插入临时表
                        var sqltest = new StringBuilder();
                        foreach (var inv in DLst)
                        {
                            sqltest.Append("('" + inv.GoodsCode + "','" + inv.GoodsName + "','" + inv.SkuID + "','" + inv.SkuName
                                 + "','" + inv.ColorName + "','" + inv.ParentSize + "','" + inv.SizeName
                                  + "'," + inv.StockQty + "," + CoID + "),");
                        }
                        string insql = @"INSERT INTO TmpInvTable(`GoodsCode`,`GoodsName`,`SkuID`,`Name`,`ColorName`,`ParentSize`,`SizeName`,`Qty`,`CoID`) 
                                 VALUES"
                                + sqltest.ToString().Substring(0, sqltest.ToString().Length - 1);
                        int tcount = DbBase.CoreDB.Execute(insql);//插入临时数据
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
                        //SKUID 不存在->PASS
                        var SkuLst = DbBase.CoreDB.Query<InitInvQtyExcel>(skusql, new { CoID = CoID }).AsList();
                        if (SkuLst.Count > 0)
                        {
                            var label = new InitInvQtyExcel();
                            label.GoodsCode = "商品编码不存在:";
                            ErLst.Add(label);
                            ErLst.AddRange(SkuLst);
                            DbBase.CoreDB.Execute(skudelsql, new { CoID = CoID });
                        }
                        #endregion
                        #region 检查颜色表,ColorName不存在->PASS

                        string corsql = @"SELECT
	                                    *
                                    FROM
	                                    TmpInvTable
                                    WHERE
	                                    TmpInvTable.CoID =@CoID
                                    AND TmpInvTable.ColorName NOT IN (
	                                    SELECT
		                                    xygoods.corecolor. NAME
	                                    FROM
		                                    xygoods.corecolor
	                                    WHERE
		                                    xygoods.corecolor.CoID =@CoID
                                    )";
                        string cordelsql = @"DELETE
                                        FROM
	                                        TmpInvTable
                                        WHERE
	                                        TmpInvTable.CoID =@CoID
                                        AND TmpInvTable.ColorName NOT IN (
	                                        SELECT
		                                        xygoods.corecolor. NAME
	                                        FROM
		                                        xygoods.corecolor
	                                        WHERE
		                                        xygoods.corecolor.CoID =@CoID
                                        )";
                        var CorLst = DbBase.CoreDB.Query<InitInvQtyExcel>(corsql, new { CoID = CoID }).AsList();
                        if (CorLst.Count > 0)//ColorName不存在->PASS
                        {
                            var label = new InitInvQtyExcel();
                            label.GoodsCode = "颜色不存在:";
                            ErLst.Add(label);
                            ErLst.AddRange(CorLst);
                            DbBase.CoreDB.Execute(cordelsql);
                        }
                        #endregion
                        #region 检查尺码表, SizeName不存在->PASS
                        string szsql = @"SELECT
	                                *
                                FROM
	                                TmpInvTable
                                WHERE
	                                TmpInvTable.CoID =@CoID
                                AND TmpInvTable.SizeName NOT IN (
	                                SELECT
		                                xygoods.coresize. NAME
	                                FROM
		                                xygoods.coresize
	                                WHERE
		                                xygoods.coresize.CoID =@CoID
                                )";
                        string szdelsql = @"DELETE
                                        FROM
	                                        TmpInvTable
                                        WHERE
	                                        TmpInvTable.CoID =@CoID
                                        AND TmpInvTable.SizeName NOT IN (
	                                        SELECT
		                                        xygoods.coresize. NAME
	                                        FROM
		                                        xygoods.coresize
	                                        WHERE
		                                        xygoods.coresize.CoID =@CoID
                                        )";
                        var SzLst = DbBase.CoreDB.Query<InitInvQtyExcel>(szsql, new { CoID = CoID }).AsList();
                        if (SzLst.Count > 0)//SizeName不存在->PASS
                        {
                            var label = new InitInvQtyExcel();
                            label.GoodsCode = "尺码不存在:";
                            ErLst.Add(label);
                            ErLst.AddRange(SzLst);
                            DbBase.CoreDB.Execute(szdelsql);
                        }
                        #endregion
                        #region 检查期初是否已存在
                        string invsql = @"SELECT
	                                    TmpInvTable.*
                                    FROM
	                                    invinoutitem,
	                                    TmpInvTable
                                    WHERE
	                                    invinoutitem.SkuID = TmpInvTable.SkuID
                                    AND invinoutitem.WhID = @WarehouseID
                                    AND invinoutitem.CoID = TmpInvTable.CoID
                                    AND TmpInvTable.CoID =@CoID
                                    AND invinoutitem.CusType = '期初'";
                        var invLst = DbBase.CoreDB.Query<InitInvQtyExcel>(invsql, new { CoID = CoID, WarehouseID = WarehouseID }).AsList();
                        if (invLst.Count > 0) //期初已存在-->判断1.覆盖or 0.忽略 & 删除已存在临时数据
                        {
                            if (RblType == "1")
                            {
                                string oversql = @"UPDATE invinoutitem,
                                             TmpInvTable
                                            SET invinoutitem.Qty = TmpInvTable.Qty
                                            WHERE
	                                            invinoutitem.SkuID = TmpInvTable.SkuID
                                            AND invinoutitem.WhID = @WarehouseID
                                            AND invinoutitem.CoID = TmpInvTable.CoID
                                            AND TmpInvTable.CoID =@CoID
                                            AND invinoutitem.CusType = '期初'";
                                DbBase.CoreDB.Execute(oversql, new { CoID = CoID, WarehouseID = WarehouseID });
                                upttype = true;
                            }
                            var label = new InitInvQtyExcel();
                            label.GoodsCode = "存在期初的资料已被" + (RblType == "1" ? "覆盖!" : "忽略!");
                            ErLst.Add(label);
                            ErLst.AddRange(invLst);
                            string invdelsql = @"DELETE TmpInvTable.*
                                            FROM
	                                            TmpInvTable,
	                                            invinoutitem
                                            WHERE
	                                            invinoutitem.SkuID = TmpInvTable.SkuID
                                            AND invinoutitem.WhID = @WarehouseID
                                            AND invinoutitem.CoID = TmpInvTable.CoID
                                            AND TmpInvTable.CoID =@CoID
                                            AND invinoutitem.CusType = '期初'";
                            DbBase.CoreDB.Execute(invdelsql, new { CoID = CoID, WarehouseID = WarehouseID });
                        }
                        #endregion
                        #region 生成期初明细&库存
                        string selectsql = @"select * from TmpInvTable where CoID=@CoID";
                        var lst = DbBase.CoreDB.Query<InitInvQtyExcel>(selectsql, new { CoID = CoID }).AsList();
                        if (lst.Count > 0)
                        {
                            #region 期初主表新增
                            var Rd = CommHaddle.GetRecordID(CoID);
                            string RecordID = "BIN" + (Convert.ToInt64(Rd)).ToString();//单据编号  
                            string InsertInvSql = @"insert into invinout(RecordID,Type,CusType,Status,
                                                                WhID,WhName,IsExport,Creator,
                                                                CreateDate,CoID)
                                                         values (@RecordID,1,'期初','审核通过',
                                                                @WarehouseID,@WarehouseName,0,@Creator,
                                                                @CreateDate,@CoID)";
                            var p = new DynamicParameters();
                            p.Add("@CoID", CoID);
                            p.Add("@RecordID", RecordID);
                            p.Add("@WarehouseID", WarehouseID);
                            p.Add("@WarehouseName", WarehouseName);
                            p.Add("@Creator", UserName);
                            p.Add("@CreateDate", DateTime.Now);
                            int i = DbBase.CoreDB.Execute(InsertInvSql, p, TransCore);
                            if (i > 0)
                            {
                                #region 期初明细新增
                                string InsertInvItemSql = @"INSERT INTO invinoutitem (
	                                                                            IoID,CoID,SkuID,SkuName,
	                                                                            CusType,Norm,WhID,WhName,
	                                                                            Creator,CreateDate,Qty
                                                                            )(
	                                                                            SELECT
		                                                                            @RecordID,CoID,SkuID,NAME,
		                                                                            '期初',CONCAT(ColorName,';',SizeName),@WarehouseID,@WarehouseName,
		                                                                            @Creator ,@CreateDate,Qty
	                                                                            FROM
		                                                                            TmpInvTable
	                                                                            WHERE
		                                                                            TmpInvTable.CoID =@CoID
                                                                            )";
                                int j = DbBase.CoreDB.Execute(InsertInvItemSql, p, TransCore);
                                #endregion
                                #region
                                string InsertInvtorySql = @"INSERT INTO inventory (
	                                                                        GoodsCode,SkuID,NAME,Norm,
	                                                                        WarehouseID,WarehouseName,StockQty,CoID
                                                                        )(
	                                                                        SELECT
		                                                                        GoodsCode,SkuID,NAME,CONCAT(ColorName,';',SizeName),
		                                                                        @WarehouseID,@WarehouseName,Qty,CoID
	                                                                        FROM
		                                                                        TmpInvTable
	                                                                        WHERE
		                                                                        TmpInvTable.CoID =@CoID
	                                                                        AND CONCAT(
		                                                                        TmpInvTable.SkuID,
		                                                                        ',',
		                                                                        @WarehouseName
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
	                                                                        )
                                                                        )";
                                var p1 = new DynamicParameters();
                                p1.Add("@CoID", CoID);
                                p1.Add("@WarehouseID", WarehouseID);
                                p1.Add("@WarehouseName", WarehouseName);
                                int k = DbBase.CoreDB.Execute(InsertInvtorySql, p1, TransCore);
                                #endregion
                                upttype = true;
                            }
                            #endregion
                            #region 更新庫存
                            if (upttype)
                            {
                                string UpdateInvsql = @"UPDATE inventory
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
	                                            inventory.CoID =@CoID
                                            AND inventory.WarehouseID = @WarehouseID";
                                int s = DbBase.CoreDB.Execute(UpdateInvsql, new { CoID = CoID, WarehouseID = WarehouseID }, TransCore);
                                TransCore.Commit();
                            }
                            #endregion
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
                                LinkWhID,
                                IsExport,
                                RecID,
                                InvoiceID,
                                Creator,
                                CreateDate,
                                CoID,
                                RefID ) VALUES(
                                @RecordID,
                                @Type,
                                @CusType,
                                @Status,
                                @WhID,
                                @WhName,
                                @LinkWhID,
                                @IsExport,
                                @RecID,
                                @InvoiceID,
                                @Creator,
                                @CreateDate,
                                @CoID,
                                @RefID   )";
            return sql;
        }
        public static string AddInvinoutitemSql()
        {
            string sql = @"INSERT INTO Invinoutitem
                            (
                                IoID,
                                CoID,
                                Skuautoid,
                                SkuID,
                                SkuName,
                                Norm,
                                Qty,
                                WhID,
                                WhName,
                                LinkWhID,
                                Creator,
                                CreateDate,
                                Type,
                                CusType,
                                Status,
                                RefID ) VALUES(
                                @IoID,
                                @CoID,
                                @Skuautoid,
                                @SkuID,
                                @SkuName,
                                @Norm,
                                @Qty,
                                @WhID,
                                @WhName,
                                @LinkWhID,
                                @Creator,
                                @CreateDate,
                                @Type,
                                @CusType,
                                @Status,
                                @RefID)";
            return sql;
        }
        public static string AddInventorySql()
        {
            string sql = @"INSERT INTO inventory
                        (
                            Skuautoid,
                            StockQty,
                            PickQty,
                            WaitInQty,
                            SaleRetuQty,
                            SafeQty,
                            DefectiveQty,
                            VirtualQty,
                            PurchaseQty,
                            CoID,
                            Creator,
                            CreateDate,
                            WarehouseID
                        ) VALUES(
                            @Skuautoid,
                            @StockQty,
                            @PickQty,
                            @WaitInQty,
                            @SaleRetuQty,
                            @SafeQty,
                            @DefectiveQty,
                            @VirtualQty,
                            @PurchaseQty,
                            @CoID,
                            @Creator,
                            @CreateDate,
                            @WarehouseID
                        )";
            return sql;
        }

        public static string AddInventorySaleSql()
        {
            string sql = @"INSERT INTO inventory_sale
                        (
                            Skuautoid,
                            StockQty,
                            LockQty,
                            PickQty,
                            WaitInQty,
                            SaleRetuQty,
                            SafeQty,
                            DefectiveQty,
                            VirtualQty,
                            PurchaseQty,
                            CoID,
                            Creator,
                            CreateDate
                        ) VALUES(
                            @Skuautoid,
                            @StockQty,
                            @LockQty,
                            @PickQty,
                            @WaitInQty,
                            @SaleRetuQty,
                            @SafeQty,
                            @DefectiveQty,
                            @VirtualQty,
                            @PurchaseQty,
                            @CoID,
                            @Creator,
                            @CreateDate
                        )";
            return sql;
        }
        #region 更新总库存数量
        public static string UptInvStockQtySql()
        {
            string sql = @"UPDATE inventory 
                            SET StockQty= (SELECT IFNULL(sum(Qty),0)
	                                            FROM
		                                            invinoutitem
	                                            WHERE
		                                            invinoutitem.Skuautoid = inventory.Skuautoid
	                                            AND invinoutitem.CoID = inventory.CoID
                                                AND invinoutitem.`Status` = 1
                                                AND invinoutitem.Skuautoid in @SkuIDLst
                                            ),IsDelete=0,Modifier=@Modifier,ModifyDate=@ModifyDate
                                            WHERE
	                                            inventory.CoID =@CoID
                                            AND Inventory.Skuautoid in @SkuIDLst";
            return sql;
        }
        #endregion

        #region 更新仓库待发库存
        public static string UptInvMainPickQtySql()
        {
            string sql = @"UPDATE inventory_sale
                            SET inventory_sale.PickQty = (
                                SELECT
                                    IFNULL(SUM(PickQty),0)
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

        #region 更新仓库安全库存
        public static string UptInvMainSafeQtySql()
        {
            string sql = @"UPDATE inventory_sale
                            SET inventory_sale.SafeQty = (
                                SELECT                                
                                    IFNULL(SUM(SafeQty),0)
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

        #region 更新仓库安全库存
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

        #region 更新销退仓库存
        public static string UptInvMainSaleRetuQtySql()
        {
            string sql = @"UPDATE inventory_sale
                            SET inventory_sale.SaleRetuQty = (
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

        #region 更新采购仓库存
        public static string UptInvMainPurchaseQtySql()
        {
            string sql = @"UPDATE inventory_sale
                            SET inventory_sale.PurchaseQty = (
                                SELECT
                                    IFNULL(SUM(PurchaseQty),0)
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

        #region 
        public static string UptInvMainStockQtySql()
        {
            string sql = @"UPDATE inventory_sale
                            SET inventory_sale.StockQty = (
                                SELECT
                                    SUM(StockQty)
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
        public static Invinout AddInvinout(InvinoutAuto IOauto)
        {
            var inout = new Invinout();
            inout.CoID = IOauto.CoID.ToString();
            inout.Status = 1;
            inout.CusType = IOauto.CusType;
            inout.Type = IOauto.Type;
            inout.RecordID = IOauto.RecordID;
            inout.WhID = IOauto.inv.WarehouseID.ToString();
            inout.WhName = IOauto.inv.WarehouseName;
            inout.Creator = IOauto.UserName;
            inout.CreateDate = DateTime.Now.ToString();
            inout.IsExport = false;
            return inout;
        }

        public static Invinoutitem AddInvinoutitem(InvinoutAuto IOauto)
        {
            var item = new Invinoutitem();
            item.CoID = IOauto.CoID;
            item.CusType = IOauto.CusType;
            item.IoID = IOauto.RecordID;
            item.WhID = IOauto.inv.WarehouseID.ToString();
            item.WhName = IOauto.inv.WarehouseName;
            item.SkuID = IOauto.inv.SkuID;
            item.SkuName = IOauto.inv.Name;
            item.Qty = Convert.ToInt32(IOauto.Qty);
            // item.Unit = "件";
            item.Creator = IOauto.UserName;
            item.CreateDate = DateTime.Now.ToString();
            return item;
        }

        public static List<Invinout> AddInvinoutLst(InvinoutAuto IOauto)
        {
            var inoutLst = new List<Invinout>();
            foreach (var Inv in IOauto.InvLst)
            {
                var inout = new Invinout();
                inout.CoID = IOauto.CoID.ToString();
                inout.Status = 1;
                inout.CusType = IOauto.CusType;
                inout.Type = IOauto.Type;
                inout.RecordID = IOauto.RecordID;
                inout.WhID = IOauto.inv.WarehouseID.ToString();
                inout.WhName = IOauto.inv.WarehouseName;
                inout.Creator = IOauto.UserName;
                inout.CreateDate = DateTime.Now.ToString();
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
                item.WhID = IOauto.inv.WarehouseID.ToString();
                item.WhName = IOauto.inv.WarehouseName;
                item.SkuID = IOauto.inv.SkuID;
                item.SkuName = IOauto.inv.Name;
                item.Qty = Convert.ToInt32(0 - IOauto.inv.StockQty);
                // item.Unit = "件";
                item.Creator = IOauto.UserName;
                item.CreateDate = DateTime.Now.ToString();
                ItemLst.Add(item);
            }
            return ItemLst;
        }
        #endregion        
    }
}