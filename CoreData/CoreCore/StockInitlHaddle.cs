using CoreModels;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using System.Data;
using CoreModels.XyCore;
using CoreModels.XyComm;
using CoreData.CoreComm;
using MySql.Data.MySqlClient;
// using System.Linq;
// using System.Threading.Tasks;
namespace CoreData.CoreCore
{
    public static class StockInitHaddle
    {
        #region 库存期初 - 查询 - 主表
        public static DataResult GetStockInitMain(Sfc_item_param IParam)
        {
            var result = new DataResult(1, null);
            var cs = new Sfc_main_query();
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    StringBuilder querysql = new StringBuilder();
                    StringBuilder querycount = new StringBuilder();
                    querycount.Append("SELECT count(ID) FROM sfc_main WHERE Type = 1 AND `Status`<>2");//单据类型(1.期初，2.盘点，3.调拨)
                    querysql.Append("SELECT ID,WhID,Remark,Status,Creator,CreateDate FROM sfc_main WHERE Type = 1 AND `Status`<>2");
                    var p = new DynamicParameters();
                    if (IParam.CoID != "1")
                    {
                        querycount.Append(" AND CoID = @CoID");
                        querysql.Append(" AND CoID = @CoID");
                        p.Add("@CoID", IParam.CoID);
                    }
                    if (!string.IsNullOrEmpty(IParam.Status))//状态：(0:待确认;1:生效;2.作废)
                    {
                        querycount.Append(" AND Status = @Status");
                        querysql.Append(" AND Status = @Status");
                        p.Add("@Status", IParam.Status);
                    }
                    if (!string.IsNullOrEmpty(IParam.Skuautoid))
                    {
                        querycount.Append(" AND ID in (SELECT distinct ParentID FROM sfc_item WHERE sfc_item.CoID= sfc_main.CoID AND sfc_item.ParentID=sfc_main.ID AND sfc_item.Skuautoid LIKE @Skuautoid )");
                        querysql.Append(" AND ID in (SELECT distinct ParentID FROM sfc_item WHERE sfc_item.CoID= sfc_main.CoID AND sfc_item.ParentID=sfc_main.ID AND sfc_item.Skuautoid LIKE @Skuautoid )");
                        p.Add("@Skuautoid", "%" + IParam.Skuautoid + "%");
                    }
                    if (!string.IsNullOrEmpty(IParam.SortField) && !string.IsNullOrEmpty(IParam.SortDirection))//排序
                    {
                        querysql.Append(" ORDER BY " + IParam.SortField + " " + IParam.SortDirection);
                    }
                    var DataCount = conn.QueryFirst<int>(querycount.ToString(), p);
                    if (DataCount < 0)
                    {
                        result.s = -3001;
                    }
                    else
                    {
                        cs.DataCount = DataCount;
                        decimal pagecnt = Math.Ceiling(decimal.Parse(cs.DataCount.ToString()) / decimal.Parse(IParam.PageSize.ToString()));
                        cs.PageCount = Convert.ToInt32(pagecnt);
                        int dataindex = (IParam.PageIndex - 1) * IParam.PageSize;
                        querysql.Append(" LIMIT @ls , @le");
                        p.Add("@ls", dataindex);
                        p.Add("@le", IParam.PageSize);
                        var mainLst = conn.Query<Sfc_main_view>(querysql.ToString(), p).AsList();
                        if (mainLst.Count > 0)
                        {
                            var WhIDLst = mainLst.Select(a => a.WhID).Distinct().AsList();
                            var res = CommHaddle.GetWhViewLstByID(IParam.CoID, WhIDLst);
                            var WhViewLst = res.d as List<Warehouse_view>;
                            // cs.DicWh = res.d as Dictionary<string, object>;//获取仓库List资料
                            mainLst = (from a in mainLst
                                       join b in WhViewLst on a.WhID equals b.ID into data
                                       from c in data.DefaultIfEmpty()
                                       select new Sfc_main_view
                                       {
                                           ID = a.ID,
                                           WhID = a.WhID,
                                           WhName = c == null ? "" : c.WhName,
                                           Remark = a.Remark,
                                           Status = a.Status,
                                           Creator = a.Creator,
                                           CreateDate = a.CreateDate
                                       }).AsList();
                            // ID,WhID,Remark,Status,Creator,CreateDate
                            cs.MainLst = mainLst;
                        }
                        result.d = cs;
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


        #region 库存期初-查询-子表
        public static DataResult GetStockInitItem(Sfc_item_param IParam)
        {
            var result = new DataResult(1, null);
            var cs = new Sfc_item_query();
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    StringBuilder querysql = new StringBuilder();
                    StringBuilder querycount = new StringBuilder();
                    querycount.Append("SELECT count(ID) FROM sfc_item WHERE CoID = @CoID AND ParentID=@ParentID AND Type = @Type");//单据类型(1.期初，2.盘点，3.调拨)
                    querysql.Append("SELECT ID,Skuautoid,InvQty,Price,Amount FROM sfc_item WHERE CoID = @CoID AND ParentID=@ParentID AND Type = @Type");
                    var p = new DynamicParameters();
                    p.Add("@CoID", IParam.CoID);
                    p.Add("@Type", IParam.Type);
                    p.Add("@ParentID", IParam.ParentID);
                    if (!string.IsNullOrEmpty(IParam.SortField) && !string.IsNullOrEmpty(IParam.SortDirection))//排序
                    {
                        querysql.Append(" ORDER BY " + IParam.SortField + " " + IParam.SortDirection);
                    }
                    var DataCount = conn.QueryFirst<int>(querycount.ToString(), p);
                    if (DataCount < 0)
                    {
                        result.s = -3001;
                    }
                    else
                    {
                        cs.DataCount = DataCount;
                        decimal pagecnt = Math.Ceiling(decimal.Parse(cs.DataCount.ToString()) / decimal.Parse(IParam.PageSize.ToString()));
                        cs.PageCount = Convert.ToInt32(pagecnt);
                        int dataindex = (IParam.PageIndex - 1) * IParam.PageSize;
                        querysql.Append(" LIMIT @ls , @le");
                        p.Add("@ls", dataindex);
                        p.Add("@le", IParam.PageSize);
                        var ItemLst = conn.Query<Sfc_item_Init_view>(querysql.ToString(), p).AsList();
                        if (ItemLst.Count > 0)
                        {
                            var SkuIDLst = ItemLst.Select(a => a.Skuautoid).Distinct().AsList();
                            var res = CommHaddle.GetSkuViewByID(IParam.CoID, SkuIDLst);
                            var SkuViewLst = res.d as List<CoreSkuView>;//获取商品Sku资料
                            ItemLst = (from a in ItemLst
                                       join b in SkuViewLst on a.Skuautoid equals b.ID into data
                                       from c in data.DefaultIfEmpty()
                                       select new Sfc_item_Init_view
                                       {
                                           ID = a.ID,
                                           Skuautoid = a.Skuautoid,
                                           InvQty = a.InvQty,
                                           Price = a.Price,
                                           Amount = a.Amount,
                                           SkuID = c == null ? "" : c.SkuID,
                                           SkuName = c == null ? "" : c.SkuName,
                                           Norm = c == null ? "" : c.Norm,
                                           Img = c == null ? "" : c.Img
                                       }).AsList();
                            //   on new { Skuautoid = a.Skuautoid } equals new { Skuautoid = b.ID } 
                            //   group new {a,b} by new
                            //   {
                            //       a.ID,
                            //       a.Skuautoid,
                            //       a.InvQty,
                            //       a.Price,
                            //       a.Amount,
                            //       b.SkuID,
                            //       b.SkuName,
                            //       b.Norm,
                            //       b.Img
                            //   } into c
                            //   select new Sfc_item_Init_view{
                            //         ID = c.Key.ID,
                            //       Skuautoid = c.Key.Skuautoid,
                            //       InvQty = c.Key.InvQty,
                            //       Price = c.Key.Price,
                            //       Amount = c.Key.Amount,
                            //       SkuID = c.Key.SkuID,
                            //       SkuName = c.Key.SkuName,
                            //       Norm = c.Key.Norm,
                            //       Img = c.Key.Img
                            //   }).AsList();
                            cs.InitItemLst = ItemLst;
                        }
                        result.d = cs;
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


        #region 库存期初 - 修改保存 - 期初数量
        public static DataResult SaveStockInitQty(string ID, int InvQty, string CoID, string UserName)
        {
            var result = new DataResult(1, null);
            var conn = new MySqlConnection(DbBase.CoreConnectString);
            conn.Open();
            var Trans = conn.BeginTransaction();
            try
            {
                string contents = string.Empty;
                var itemOld = conn.QueryFirst<Sfc_item_Init_view>("SELECT ID,Skuautoid,InvQty,Price,ParentID FROM sfc_item WHERE CoID=@CoID AND ID=@ID", new { CoID = CoID, ID = ID });
                if (itemOld != null)
                {
                    var main = conn.QueryFirst<Sfc_main>("SELECT `Status`,Parent_WhID FROM sfc_main WHERE CoID=@CoID AND ID=@ID", new { CoID = CoID, ID = itemOld.ParentID });
                    if (int.Parse(itemOld.InvQty) != InvQty)
                    {
                        contents = contents + "数量:" + itemOld.InvQty + "=>" + InvQty + ";";
                    }
                    if (!string.IsNullOrEmpty(contents))
                    {
                        string sql = @"UPDATE sfc_item
                                        SET InvQty = @InvQty,
                                        Amount =@InvQty * Price,
                                        Modifier =@Modifier,
                                        ModifyDate =@ModifyDate
                                        WHERE
                                            CoID =@CoID
                                        AND ID =@ID";
                        conn.Execute(sql, new { InvQty = InvQty, CoID = CoID, ID = ID, Modifier = UserName, ModifyDate = DateTime.Now.ToString() }, Trans);
                        if (main.Status == 1)//对于已生效的期初单据，需要重新计算库存数量
                        {
                            //更新交易
                            string uptiosql = @"UPDATE Invinoutitem
                                                SET Qty =@Qty
                                                WHERE
                                                    CoID =@CoID
                                                AND RefID =@RefID
                                                AND Skuautoid =@Skuautoid,
                                                Modifier =@Modifier,
                                                ModifyDate =@ModifyDate";
                            conn.Execute(uptiosql, new { CoID = CoID, RefID = itemOld.ParentID, Skuautoid = itemOld.Skuautoid, Modifier = UserName, ModifyDate = DateTime.Now.ToString() }, Trans);

                            //重算库存
                            var SkuIDLst = new List<string>();
                            SkuIDLst.Add(itemOld.Skuautoid);
                            conn.Execute(InventoryHaddle.UptInvStockQtySql(), new { CoID = CoID, WarehouseID = main.Parent_WhID, SkuIDLst = SkuIDLst, Modifier = UserName, ModifyDate = DateTime.Now.ToString() }, Trans);
                            conn.Execute(InventoryHaddle.UptInvMainStockQtySql(), new { CoID = CoID, WarehouseID = main.Parent_WhID, SkuIDLst = SkuIDLst, Modifier = UserName, ModifyDate = DateTime.Now.ToString() }, Trans);
                        }
                        Trans.Commit();
                        CoreUser.LogComm.InsertUserLog("修改期初明细", "sfc_item", "商品ID" + itemOld.Skuautoid + " " + contents, UserName, int.Parse(CoID), DateTime.Now);
                    }
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
        #region 库存期初 - 修改保存 - 成本单价
        public static DataResult SaveStockInitPrice(string ID, Decimal Price, string CoID, string UserName)
        {
            var result = new DataResult(1, null);
            var conn = new MySqlConnection(DbBase.CoreConnectString);
            conn.Open();
            var Trans = conn.BeginTransaction();
            try
            {
                string contents = string.Empty;
                var itemOld = conn.QueryFirst<Sfc_item_Init_view>("SELECT ID,Skuautoid,InvQty,Price FROM sfc_item WHERE CoID=@CoID AND ID=@ID", new { CoID = CoID, ID = ID });
                if (itemOld != null)
                {
                    if (Convert.ToDecimal(itemOld.Price) != Price)
                    {
                        contents = contents + "单价:" + itemOld.Price + "=>" + Price + ";";
                    }
                    if (!string.IsNullOrEmpty(contents))
                    {
                        string sql = "UPDATE sfc_item SET Price=@Price,Amount=InvQty*@Price,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE CoID=@CoID AND ID=@ID";
                        conn.Execute(sql, new { Price = Price, CoID = CoID, ID = ID, Modifier = UserName, ModifyDate = DateTime.Now.ToString() }, Trans);
                        Trans.Commit();
                        CoreUser.LogComm.InsertUserLog("修改期初明细", "sfc_item", "商品ID" + itemOld.Skuautoid + " " + contents, UserName, int.Parse(CoID), DateTime.Now);
                    }
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

        #region 库存期初 - 确认生效
        public static DataResult CheckStockInit(string ID, string CoID, string UserName)
        {
            var result = new DataResult(1, null);
            var conn = new MySqlConnection(DbBase.CoreConnectString);
            conn.Open();
            var Trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
            try
            {
                string checksql = "UPDATE sfc_main SET Status = 1,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE CoID=@CoID AND ID=@ID";//(0:待确认;1:生效;2.作废)
                string querymainsql = @"SELECT ID,WhID,Parent_WhID,Status,Type FROM sfc_main WHERE CoID=@CoID AND ID=@ID";
                string querysql = @"SELECT ID,Skuautoid,WhID,InvQty,Parent_WhID FROM sfc_item WHERE CoID=@CoID AND ParentID=@ID";
                string InvQuerySql = @"SELECT ID,Skuautoid,StockQty FROM Inventory WHERE CoID=@CoID AND Skuautoid in @SkuIDLst";
                string InvMQuerySql = @"SELECT ID,Skuautoid,StockQty FROM Inventory_sale WHERE CoID=@CoID AND Skuautoid in @SkuIDLst";
                var main = conn.QueryFirst<Sfc_main>(querymainsql, new { CoID = CoID, ID = ID });
                var itemLst = conn.Query<Sfc_item>(querysql, new { CoID = CoID, ID = ID }).AsList();
                if (main.Status == 0 && main.Type == 1)//1.期初 2.盘点 3.调拨
                {
                    if (itemLst.Count > 0)
                    {
                        string Parent_WhID = main.Parent_WhID;//主仓ID
                        string WhID = main.WhID;//分仓ID
                        var SkuIDLst = itemLst.Select(a => a.Skuautoid).AsList();
                        var InvSkuLst = conn.Query<Sfc_InvStock>(InvQuerySql, new { CoID = CoID, @WarehouseID = Parent_WhID, SkuIDLst = SkuIDLst }).AsList();//读取现有库存
                        var MainInvSkuLst = conn.Query<Sfc_InvStock>(InvMQuerySql, new { CoID = CoID, SkuIDLst = SkuIDLst }).AsList();//读取现有主仓库存   
                        var RecordID = "BIN" + CommHaddle.GetRecordID(int.Parse(CoID));
                        int Type = 1701;
                        string CusType = "期初";
                        int Status = 1;//(0:待审核;1.审核通过;2.作废)
                        //交易主表         
                        var inv = new Invinout();
                        inv.RefID = ID;
                        inv.RecordID = RecordID;
                        inv.Type = Type;
                        inv.CusType = CusType;
                        inv.Status = Status;
                        inv.WhID = Parent_WhID;
                        inv.LinkWhID = WhID;
                        inv.Creator = UserName;
                        inv.CreateDate = DateTime.Now.ToString();
                        inv.CoID = CoID;
                        //交易明细             
                        var InvinoutitemLst = itemLst.Select(a => new Invinoutitem
                        {
                            RefID = ID,
                            IoID = RecordID,
                            Type = Type,
                            Status = Status,
                            CusType = CusType,
                            Skuautoid = a.Skuautoid,
                            WhID = Parent_WhID,
                            LinkWhID = WhID,
                            Qty = a.InvQty,
                            CoID = CoID,
                            Creator = UserName,
                            CreateDate = DateTime.Now.ToString()
                        }).AsList();
                        //库存sku新增
                        var NewInvLst = itemLst.Where(a => !InvSkuLst
                                                            .Select(b => b.Skuautoid)
                                                            .Contains(a.Skuautoid))
                                            .Select(a => new Inventory
                                            {
                                                Skuautoid = a.Skuautoid,
                                                WarehouseID = a.Parent_WhID,
                                                StockQty = a.InvQty,
                                                CoID = CoID,
                                                Creator = UserName,
                                                CreateDate = DateTime.Now.ToString()
                                            }).AsList();
                        var NewMainInvLst = itemLst.Where(a => !InvSkuLst
                                                            .Select(b => b.Skuautoid)
                                                            .Contains(a.Skuautoid))
                                            .Select(a => new Inventory_sale
                                            {
                                                Skuautoid = a.Skuautoid,
                                                // WarehouseID = "0",
                                                StockQty = a.InvQty,
                                                CoID = CoID,
                                                Creator = UserName,
                                                CreateDate = DateTime.Now.ToString()
                                            }).AsList();
                        //新增交易表头
                        conn.Execute(InventoryHaddle.AddInvinoutSql(), inv, Trans);
                        //新增交易明细
                        conn.Execute(InventoryHaddle.AddInvinoutitemSql(), InvinoutitemLst, Trans);
                        //Sku库存新增
                        if (NewInvLst.Count > 0)
                        {
                            conn.Execute(InventoryHaddle.AddInventorySql(), NewInvLst, Trans);
                        }
                        if (NewMainInvLst.Count > 0)
                        {
                            conn.Execute(InventoryHaddle.AddInventorySaleSql(), NewMainInvLst, Trans);//Sku库存新增
                        }
                        //更新确认生效标记&更新库存数量
                        conn.Execute(checksql, new { CoID = CoID, ID = ID, Modifier = UserName, ModifyDate = DateTime.Now.ToString() }, Trans);//确认生效
                        //更新库存数量                                                                              
                        conn.Execute(InventoryHaddle.UptInvStockQtySql(), new { CoID = CoID, SkuIDLst = SkuIDLst, Modifier = UserName, ModifyDate = DateTime.Now.ToString() }, Trans);
                        //更新总库存数量
                        var res = CommHaddle.GetWareCoidList(CoID);
                        var CoIDLst = res.d as List<string>;
                        conn.Execute(InventoryHaddle.UptInvMainStockQtySql(), new { CoID = CoID, CoIDLst = CoIDLst, SkuIDLst = SkuIDLst, Modifier = UserName, ModifyDate = DateTime.Now.ToString() }, Trans);
                        Trans.Commit();
                        CoreUser.LogComm.InsertUserLog("期初单据-确认生效", "sfc_item", "单据ID" + ID, UserName, int.Parse(CoID), DateTime.Now);
                    }
                    else
                    {
                        result.s = -1;
                        result.d = "请先录入期初明细";
                    }
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

    }
}