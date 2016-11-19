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
// using System.Linq;
// using System.Threading.Tasks;

namespace CoreData.CoreCore
{

    public static class StockTakeHaddle
    {
        #region 库存盘点-查询-主表
        public static DataResult GetStockTakeMain(Sfc_item_param IParam)
        {
            var result = new DataResult(1, null);
            var cs = new Sfc_main_query();
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    StringBuilder querysql = new StringBuilder();
                    StringBuilder querycount = new StringBuilder();
                    querycount.Append("SELECT count(ID) FROM sfc_main WHERE Type = 2");
                    querysql.Append("SELECT ID,WhID,Remark,Status,Creator,CreateDate FROM sfc_main WHERE Type = 2");
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
                    if (!string.IsNullOrEmpty(IParam.WhID))
                    {
                        querycount.Append(" AND WhID = @WhID");
                        querysql.Append(" AND WhID = @WhID");
                        p.Add("@WhID", IParam.WhID);
                    }
                    if (!string.IsNullOrEmpty(IParam.DateF))
                    {
                        querycount.Append(" AND DateF = @DateF");
                        querysql.Append(" AND DateF = @DateF");
                        p.Add("@DateF", IParam.DateF);
                    }
                    if (!string.IsNullOrEmpty(IParam.DateT))
                    {
                        querycount.Append(" AND DateT = @DateT");
                        querysql.Append(" AND DateT = @DateT");
                        p.Add("@DateT", IParam.DateT);
                    }
                    if (!string.IsNullOrEmpty(IParam.Skuautoid))
                    {
                        querycount.Append(" AND ID in (SELECT distinct ParentID FROM sfc_item WHERE sfc_item.CoID= sfc_main.CoID AND sfc_item.ParentID=sfc_main.ID AND sfc_item.Skuautoid LIKE @Skuautoid ))");
                        querysql.Append(" AND ID in (SELECT distinct ParentID FROM sfc_item WHERE sfc_item.CoID= sfc_main.CoID AND sfc_item.ParentID=sfc_main.ID AND sfc_item.Skuautoid LIKE @Skuautoid ))");
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

        #region 库存盘点-查询-子表
        public static DataResult GetStockTakeItem(string CoID, string ParentID)
        {
            var result = new DataResult(1, null);
            var Item = new Sfc_item_query();
            string sql = @"SELECT ID,Skuautoid,Qty,InvQty FROM sfc_item WHERE CoID = @CoID AND ParentID=@ParentID AND Type = 2";
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    var ItemLst = conn.Query<Sfc_item_view>(sql, new { CoID = CoID, ParentID = ParentID }).AsList();
                    if (ItemLst.Count > 0)
                    {
                        var SkuIDLst = ItemLst.Select(a => a.Skuautoid).Distinct().AsList();
                        var res = CommHaddle.GetSkuViewByID(CoID, SkuIDLst);
                        var SkuViewLst = res.d as List<CoreSkuView>;//获取商品Sku资料,拼接Lst显示
                        ItemLst = (from a in ItemLst
                                   join b in SkuViewLst on a.Skuautoid equals b.ID into data
                                   from c in data.DefaultIfEmpty()
                                   select new Sfc_item_view
                                   {
                                       ID = a.ID,
                                       Skuautoid = a.Skuautoid,
                                       InvQty = a.InvQty,
                                       Qty = a.Qty,
                                       SkuID = c == null ? "" : c.SkuID,
                                       SkuName = c == null ? "" : c.SkuName,
                                       Norm = c == null ? "" : c.Norm,
                                       Img = c == null ? "" : c.Img
                                   }).AsList();
                        // ItemLst = (from a in ItemLst
                        //            join b in SkuViewLst                                   
                        //    on new { Skuautoid = a.Skuautoid } equals new { Skuautoid = b.ID }
                        //    group new { a, b } by new
                        //    {
                        //        a.ID,
                        //        a.Skuautoid,
                        //        a.InvQty,
                        //        a.Qty,
                        //        b.SkuID,
                        //        b.SkuName,
                        //        b.Norm,
                        //        b.Img
                        //    } into c
                        //    select new Sfc_item_view
                        //    {
                        //        ID = c.Key.ID,
                        //        Skuautoid = c.Key.Skuautoid,
                        //        InvQty = c.Key.InvQty,
                        //        Qty = c.Key.Qty,
                        //        SkuID = c.Key.SkuID,
                        //        SkuName = c.Key.SkuName,
                        //        Norm = c.Key.Norm,
                        //        Img = c.Key.Img
                        //    }).AsList();
                        Item.ItemLst = ItemLst;
                    }
                    result.d = Item;
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

        #region 库存盘点-新增-盘点主表
        ///<summary>
        ///库存盘点-新增-盘点主表
        ///</summary>
        public static DataResult InsertStockTakeMain(string WhID, string Parent_WhID, int Type, string CoID, string UserName)
        {
            var result = new DataResult(1, null);
            var conn = new MySqlConnection(DbBase.CoreConnectString);
            conn.Open();
            string TypeName = Enum.GetName(typeof(InvE.SfcMainTypeE), Type).ToString();
            var Trans = conn.BeginTransaction();
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
                        var main = new Sfc_main();
                        main.CoID = CoID;
                        main.Creator = UserName;
                        main.CreateDate = DateTime.Now.ToString();
                        main.Type = Type;
                        main.WhID = WhID;
                        main.Parent_WhID = Parent_WhID;
                        conn.Execute(AddSfcMainSql(), main, Trans);
                        Trans.Commit();
                        CoreUser.LogComm.InsertUserLog("新增库存" + TypeName, "sfc_main", "新增" + TypeName + ":" + WhID.ToString(), main.Creator, int.Parse(CoID), DateTime.Now);
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

        #region 库存盘点-新增-盘点子表
        public static DataResult InsertStockTakeItem(string ParentID, List<int> SkuIDLst, int Type, string CoID, string UserName)
        {
            var result = new DataResult(1, null);
            var conn = new MySqlConnection(DbBase.CoreConnectString);
            conn.Open();
            string TypeName = Enum.GetName(typeof(InvE.SfcMainTypeE), Type).ToString();
            var Trans = conn.BeginTransaction();
            try
            {
                string sql = @"SELECT WhID,Parent_WhID FROM sfc_main WHERE CoID=@CoID AND ID=@ParentID AND Type=@Type";
                string itemsql = @"SELECT Skuautoid FROM sfc_item WHERE CoID=@CoID AND ParentID=@ParentID AND Type=@Type";
                var main = conn.QueryFirst<Sfc_main>(sql, new { CoID = CoID, ParentID = ParentID, Type = Type });
                var Old_SkuIDLst = conn.Query<int>(itemsql, new { CoID = CoID, ParentID = ParentID, Type = Type }).AsList();
                var ItemLst = SkuIDLst.Where(a => !Old_SkuIDLst.Contains(a)).Select(a => new Sfc_item
                {
                    WhID = main.WhID,
                    ParentID = ParentID,
                    Parent_WhID = main.Parent_WhID,
                    Skuautoid = a.ToString(),
                    Type = Type,
                    CoID = CoID,
                    Creator = UserName,
                    CreateDate = DateTime.Now.ToString()
                }).AsList();
                if (ItemLst.Count > 0)
                {
                    conn.Execute(AddSfcItemSql(), ItemLst, Trans);
                    Trans.Commit();
                    CoreUser.LogComm.InsertUserLog("新增库存" + TypeName + "明细", "sfc_main", "新增" + TypeName + string.Join(",", SkuIDLst.ToArray()), main.Creator, int.Parse(CoID), DateTime.Now);
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

        #region 库存盘点 - 修改保存盘点数量
        public static DataResult SaveStockTakeQty(string ID, string InvQty, string CoID, string UserName)
        {
            var result = new DataResult(1, null);
            var conn = new MySqlConnection(DbBase.CoreConnectString);
            conn.Open();
            var Trans = conn.BeginTransaction();
            try
            {
                string contents = string.Empty;
                var itemOld = conn.QueryFirst<Sfc_item_Init_view>("SELECT ID,Skuautoid,InvQty,Price FROM sfc_item WHERE CoID=@CoID AND ID=@ID", new { CoID = CoID, ID = ID });
                if (int.Parse(itemOld.InvQty) != int.Parse(InvQty))
                {
                    contents = contents + "数量:" + itemOld.InvQty + "=>" + InvQty + ";";
                }
                if (string.IsNullOrEmpty(contents))
                {
                    string sql = "UPDATE sfc_item SET InvQty = @InvQty,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE CoID=@CoID AND ID=@ID";
                    conn.Execute(sql, new { InvQty = InvQty, CoID = CoID, ID = ID, Modifier = UserName, ModifyDate = DateTime.Now.ToString() }, Trans);
                    Trans.Commit();
                    CoreUser.LogComm.InsertUserLog("修改期初明细", "sfc_item", "商品ID" + itemOld.Skuautoid + " " + contents, UserName, int.Parse(CoID), DateTime.Now);
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

        #region 库存盘点 - 确认生效
        public static DataResult CheckStockTake(string ID, string CoID, string UserName)
        {
            var result = new DataResult(1, null);
            var conn = new MySqlConnection(DbBase.CoreConnectString);
            conn.Open();
            var Trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
            try
            {
                string checksql = "UPDATE sfc_main SET Status = 1,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE CoID=@CoID AND ID=@ID";//(0:待确认;1:生效;2.作废)
                string querymainsql = @"SELECT ID,WhID,Parent_WhID,Status FROM sfc_main WHERE CoID=@CoID AND ID=@ID";
                string querysql = @"SELECT ID,Skuautoid,WhID,InvQty,Parent_WhID FROM sfc_item WHERE CoID=@CoID AND ParentID=@ID";
                string InvQuerySql = @"SELECT ID,Skuautoid,StockQty FROM Inventory WHERE CoID=@CoID AND WarehouseID=@WarehouseID AND Skuautoid in @SkuIDLst";
                string InvMQuerySql = @"SELECT ID,Skuautoid,StockQty FROM Inventory WHERE CoID=@CoID AND WarehouseID=0 AND Skuautoid in @SkuIDLst";
                var main = conn.QueryFirst<Sfc_main>(querymainsql, new { CoID = CoID, ID = ID });
                var itemLst = conn.Query<Sfc_item>(querysql, new { CoID = CoID, ID = ID }).AsList();
                if (main.Status == 0)
                {
                    if (itemLst.Count > 0)
                    {
                        string Parent_WhID = main.Parent_WhID;//主仓ID
                        string WhID = main.WhID;//分仓ID
                        var SkuIDLst = itemLst.Select(a => a.Skuautoid).AsList();
                        var InvSkuLst = conn.Query<Sfc_InvStock>(InvQuerySql, new { CoID = CoID, @WarehouseID = Parent_WhID, SkuIDLst = SkuIDLst }).AsList();//读取现有库存
                        var MainInvSkuLst = conn.Query<Sfc_InvStock>(InvMQuerySql, new { CoID = CoID, SkuIDLst = SkuIDLst }).AsList();//读取现有主仓库存   
                        var RecordID = "INV" + CommHaddle.GetRecordID(int.Parse(CoID));
                        int Type = 1401;
                        string CusType = "盘点";
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
                        var invitemLst = new List<Invinoutitem>();
                        if (InvSkuLst.Count > 0)
                        {
                            var UptLst = (from a in itemLst
                                          join b in InvSkuLst
                                          on new { Skuautoid = a.Skuautoid } equals new { Skuautoid = b.Skuautoid }
                                          group new { a, b } by new
                                          {
                                              a.Skuautoid,
                                              a.InvQty,
                                              a.WhID,
                                              a.Parent_WhID,
                                              b.StockQty
                                          } into c
                                          select new Invinoutitem
                                          {
                                              RefID = ID,
                                              IoID = RecordID,
                                              Type = Type,
                                              Status = Status,
                                              CusType = CusType,
                                              Skuautoid = c.Key.Skuautoid,
                                              WhID = c.Key.Parent_WhID,
                                              LinkWhID = c.Key.WhID,
                                              Qty = c.Key.InvQty - c.Key.StockQty,
                                              CoID = CoID,
                                              Creator = UserName,
                                              CreateDate = DateTime.Now.ToString()
                                          }).AsList();
                            invitemLst.AddRange(UptLst);
                        }
                        var NewLst = itemLst.Where(a => !InvSkuLst
                                                            .Select(b => b.Skuautoid)
                                                            .Contains(a.Skuautoid))
                                               .Select(a => new Invinoutitem
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
                        invitemLst.AddRange(NewLst);
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
                                                Creator=UserName,
                                                CreateDate = DateTime.Now.ToString()
                                            }).AsList();
                        var NewMainInvLst = itemLst.Where(a => !InvSkuLst
                                                            .Select(b => b.Skuautoid)
                                                            .Contains(a.Skuautoid))
                                            .Select(a => new Inventory
                                            {
                                                Skuautoid = a.Skuautoid,
                                                WarehouseID = "0",
                                                StockQty = a.InvQty,
                                                CoID = CoID
                                            }).AsList();
                        foreach (var item in itemLst)
                        {
                            int qty = 0;
                            qty = invitemLst.Where(a => a.Skuautoid == item.Skuautoid).Select(a => a.Qty).First();
                            item.Qty = qty;
                        }
                        //新增交易表头
                        conn.Execute(InventoryHaddle.AddInvinoutSql(), inv, Trans);
                        //新增交易明细
                        conn.Execute(InventoryHaddle.AddInvinoutitemSql(), invitemLst, Trans);
                        //Sku库存新增
                        if (NewInvLst.Count > 0)
                        {
                            conn.Execute(InventoryHaddle.AddInventorySql(), NewInvLst, Trans);
                        }
                        if (NewMainInvLst.Count > 0)
                        {
                            conn.Execute(InventoryHaddle.AddInventorySql(), NewMainInvLst, Trans);//Sku库存新增
                        }
                        //更新确认生效标记&更新库存数量&回填盘点差异
                        conn.Execute(checksql, new { CoID = CoID, ID = ID, Modifier = UserName, ModifyDate = DateTime.Now.ToString() }, Trans);//确认生效
                        //更新库存数量                                                                              
                        conn.Execute(InventoryHaddle.UptInvStockQtySql(), new { CoID = CoID, WarehouseID = Parent_WhID, SkuIDLst = SkuIDLst , Modifier = UserName, ModifyDate = DateTime.Now.ToString() }, Trans);
                        conn.Execute(InventoryHaddle.UptInvMainStockQtySql(), new { CoID = CoID, WarehouseID = Parent_WhID, SkuIDLst = SkuIDLst , Modifier = UserName, ModifyDate = DateTime.Now.ToString() }, Trans);
                        conn.Execute("UPDATE sfc_item SET Qty=@Qty WHERE ID = @ID", itemLst, Trans);
                        Trans.Commit();
                        CoreUser.LogComm.InsertUserLog("盘点单据-确认生效", "sfc_item", "单据ID" + ID, UserName, int.Parse(CoID), DateTime.Now);
                    }
                    else
                    {
                        result.s = -1;
                        result.d = "请先录入盘点商品";
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

        #region 库存盘点 - 作废盘点单
        public static DataResult UnCheckStockTake(string ID, int Type, string CoID, string UserName)
        {
            var result = new DataResult(1, null);
            var conn = new MySqlConnection(DbBase.CoreConnectString);
            conn.Open();
            string TypeName = Enum.GetName(typeof(InvE.SfcMainTypeE), Type).ToString();
            var Trans = conn.BeginTransaction();
            try
            {
                string WhID = conn.QueryFirst<string>("SELECT Parent_WhID FROM sfc_main WHERE CoID=@CoID AND ID=@ID", new { CoID = CoID, ID = ID });
                var SkuIDLst = conn.Query<string>("SELECT Skuautoid FROM sfc_item WHERE CoID=@CoID AND ParentID=@ID", new { CoID = CoID, ID = ID }).AsList();
                if (SkuIDLst.Count > 0)
                {
                    //更新单据状态&交易状态：2.作废
                    string mainsql = "UPDATE sfc_main SET Status = 2,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE CoID=@CoID AND ID=@ID";//(0:待确认;1:生效;2.作废)
                    string invinoutsql = "UPDATE Invinout SET Status = 2,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE CoID=@CoID AND RefID=@ID";
                    string invinoutitemsql = "UPDATE Invinoutitem SET Status = 2,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE CoID=@CoID AND RefID=@ID";
                    var p = new DynamicParameters();
                    p.Add("@CoID", CoID);
                    p.Add("@ID", ID);
                    p.Add("@Modifier", UserName);
                    p.Add("@ModifyDate", DateTime.Now.ToString());
                    conn.Execute(mainsql, p, Trans);
                    conn.Execute(invinoutsql, p, Trans);
                    conn.Execute(invinoutitemsql, p, Trans);
                    //更新仓库库存
                    conn.Execute(InventoryHaddle.UptInvStockQtySql(), new { CoID = CoID, WarehouseID = WhID, SkuIDLst = SkuIDLst , Modifier = UserName, ModifyDate = DateTime.Now.ToString() }, Trans);
                    conn.Execute(InventoryHaddle.UptInvMainStockQtySql(), new { CoID = CoID, WarehouseID = WhID, SkuIDLst = SkuIDLst, Modifier = UserName, ModifyDate = DateTime.Now.ToString() }, Trans);
                    Trans.Commit();
                    CoreUser.LogComm.InsertUserLog(TypeName + "单据-作废", "sfc_item", "单据ID" + ID, UserName, int.Parse(CoID), DateTime.Now);

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

        #region Sql-新增-盘点单
        public static string AddSfcMainSql()
        {
            string sql = @"INSERT INTO sfc_main 
            (
                WhID,
                Remark,
                `Status`,
                Parent_WhID,
                Type,
                Creator,
                CreateDate,
                CoID
            )
            VALUES
            (
                @WhID,
                @Remark,
                @Status,
                @Parent_WhID,
                @Type,
                @Creator,
                @CreateDate,
                @CoID
            )";
            return sql;
        }
        #endregion

        #region Sql-新增-盘点明细
        public static string AddSfcItemSql()
        {
            string sql = @"INSERT INTO sfc_item
                        (
                            Skuautoid,
                            Qty,
                            InvQty,
                            WhID,
                            Parent_WhID,
                            ParentID,
                            Type,
                            Creator,
                            CreateDate,
                            CoID
                        )
                        VALUES
                        (
                            @Skuautoid,
                            @Qty,
                            @InvQty,
                            @WhID,
                            @Parent_WhID,
                            @ParentID,
                            @Type,
                            @Creator,
                            @CreateDate,
                            @CoID
                        )";
            return sql;
        }
        #endregion

    }

}