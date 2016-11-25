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
    public static class InvlockHaddle
    {
        #region 库存锁定查询 - 管理
        public static DataResult GetInvLockMainLst(InvLockParam IParam)
        {
            var inv = new InvlockData();
            var result = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    StringBuilder querycount = new StringBuilder();
                    StringBuilder querysql = new StringBuilder();
                    string countsql = @"SELECT COUNT(ID) FROM invlock_main WHERE CoID=@CoID";
                    string sql = @"SELECT
                                    ID,
                                    NAME,
                                    Type,
                                    DeadLine,
                                    AutoUnlock,
                                    UNLOCK
                                FROM
                                    invlock_main
                                WHERE
                                    CoID =@CoID";
                    querycount.Append(countsql);
                    querysql.Append(sql);
                    var p = new DynamicParameters();
                    p.Add("@CoID", IParam.CoID);
                    if (!string.IsNullOrEmpty(IParam.SkuID))//商品编号
                    {
                        string sql1 = @" AND invlock_main.ID IN (SELECT DISTINCT
                                                                        ParentID
                                                                    FROM
                                                                        invlock_item,
                                                                        coresku
                                                                    WHERE
                                                                        invlock_main.CoID = invlock_item.CoID
                                                                    AND invlock_main.ID = invlock_item.ParentID
                                                                    AND invlock_item.Skuautoid = coresku.ID
                                                                    AND coresku.SkuID LIKE @SkuID) ";
                        querysql.Append(sql1);
                        querycount.Append(sql1);
                        p.Add("@SkuID", "%" + IParam.SkuID + "%");
                    }
                    if (!string.IsNullOrEmpty(IParam.ShopType))//平台编号
                    {
                        string sql1 = @" AND invlock_main.ShopID IN (SELECT ID
                                                                    FROM
                                                                        Shop
                                                                    WHERE
                                                                        Shop.CoID = Shop.CoID
                                                                    AND Shop.ShopType LIKE @ShopType) ";
                        querysql.Append(sql1);
                        querycount.Append(sql1);
                        p.Add("@ShopType", "%" + IParam.ShopType + "%");
                    }
                    //排序
                    if (!string.IsNullOrEmpty(IParam.SortField) && !string.IsNullOrEmpty(IParam.SortDirection))//排序
                    {
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
                        var Lst = conn.Query<Invlock_main>(querysql.ToString(), p).AsList();
                        inv.LockMainLst = Lst;
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

        #region 锁定单 表头查询
        public static DataResult GetInvLockMain(string ID, string CoID)
        {
            var result = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    string sql = @"SELECT
                                    ID,
                                    NAME,
                                    Type,
                                    DeadLine,
                                    AutoUnlock,
                                    ShopID
                                FROM
                                    invlock_main
                                WHERE
                                    CoID =@CoID
                                AND ID =@ID";

                    var main = conn.Query<Invlock_main_view>(sql, new { CoID = CoID, ID = ID }).AsList();
                    if (main.Count < 0)
                    {
                        result.s = -3001;
                    }
                    else
                    {
                        var res = CommHaddle.GetShopNameByID(CoID, main[0].ShopID);
                        main[0].ShopName = res.d as string;
                        result.d = main[0];
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

        #region 锁定单 - 明细查询
        public static DataResult GetInvLockItem(string ParentID, string CoID)
        {
            var result = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    string sql = @"SELECT ID,Skuautoid,Qty FROM invlock_item WHERE ParentID = @ParentID AND CoID = @CoID";
                    var ItemLst = conn.Query<Invlock_item_view>(sql, new { ParentID = ParentID, CoID = CoID }).AsList();
                    if (ItemLst.Count > 0)
                    {
                        var SkuIDLst = ItemLst.Select(a => a.Skuautoid).AsList();
                        var res = CommHaddle.GetSkuViewByID(CoID, SkuIDLst);
                        var SkuViewLst = res.d as List<CoreSkuView>;//获取商品Sku资料,拼接Lst显示
                        ItemLst = (from a in ItemLst
                                   join b in SkuViewLst on a.Skuautoid equals b.ID into data
                                   from c in data.DefaultIfEmpty()
                                   select new Invlock_item_view
                                   {
                                       ID = a.ID,
                                       Skuautoid = a.Skuautoid,
                                       SkuID = c == null ? "" : c.SkuID,
                                       SkuName = c == null ? "" : c.SkuName,
                                       Norm = c == null ? "" : c.Norm,
                                       Qty = a.Qty
                                   }).AsList();
                    }
                    result.d = ItemLst;
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

        #region 锁定单 - 新增
        public static DataResult InsertLock(Invlock_main_view main, List<Invlock_item_view> itemLst, string CoID, string UserName)
        {
            var result = new DataResult(1, null);
            var conn = new MySqlConnection(DbBase.CoreConnectString);
            conn.Open();
            var Trans = conn.BeginTransaction();
            try
            {
                var main_new = new Invlock_main();
                main_new.Name = main.Name;
                main_new.Type = main.Type;
                main_new.DeadLine = main.DeadLine;
                main_new.AutoUnlock = main.AutoUnlock;
                main_new.ShopID = main.ShopID;
                main_new.CoID = CoID;
                main_new.Creator = UserName;
                main_new.CreateDate = DateTime.Now.ToString();
                conn.Execute(AddLockMainSql(), main_new, Trans);
                long MainID = conn.QueryFirst<long>("select LAST_INSERT_ID()", Trans);//获取新增id
                if (itemLst.Count > 0)
                {
                    var ItemLst_new = itemLst.Select(a => new Invlock_item
                    {
                        ParentID = int.Parse(MainID.ToString()),
                        Skuautoid = a.Skuautoid,
                        Qty = main.Type == 1 ? (a.StockQty * a.Qty / 100) : (main.Type == 2 ? a.Qty : 0),
                        Creator = UserName,
                        CreateDate = DateTime.Now.ToString()
                    }).AsList();
                    conn.Execute(AddLockItemSql(), ItemLst_new, Trans);
                }
                Trans.Commit();
                CoreUser.LogComm.InsertUserLog("新增锁定单", "Invlock_main", main.Type.ToString(), UserName, int.Parse(CoID), DateTime.Now);
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

        #region 锁定单 - 手动解锁
        public static DataResult HandUnLock(string ParentID, string CoID, string UserName)
        {
            var result = new DataResult(1, null);
            var conn = new MySqlConnection(DbBase.CoreConnectString);
            conn.Open();
            var Trans = conn.BeginTransaction();
            try
            {
                string sql = @"UPDATE invlock_main
                                SET UNLOCK = 1,
                                Unlocker =@Modifier,
                                UnlockDate =@ModifyDate,
                                Modifier =@Modifier,
                                ModifyDate =@ModifyDate
                                WHERE
                                    CoID =@CoID
                                AND ID =@ID";
                var p = new DynamicParameters();
                p.Add("@CoID", CoID);
                p.Add("@ID", ParentID);
                p.Add("@Modifier", UserName);
                p.Add("@ModifyDate", DateTime.Now.ToString());
                p.Add("@Unlocker", UserName);
                p.Add("@UnlockDate", DateTime.Now.ToString());
                conn.Execute(sql, p, Trans);
                Trans.Commit();
                CoreUser.LogComm.InsertUserLog("修改锁定单", "Invlock_main", "手动解锁-锁定单:" + ParentID.ToString(), UserName, int.Parse(CoID), DateTime.Now);
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

        #region 锁定单 - 检查可用库存
        public static DataResult CheckQty(List<string> SkuautoLst, string CoID)
        {
            var result = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    
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

        #region 锁定单 - 新增sql
        public static string AddLockMainSql()
        {
            string sql = @"INSERT INTO invlock_main(
                `Name`,
                Type,
                DeadLine,
                AutoUnlock,
                ShopID,
                `Unlock`,
                Creator,
                CreateDate,
                CoID
            ) VALUES (
                Name,
                Type,
                DeadLine,
                AutoUnlock,
                ShopID,
                Unlock,
                Creator,
                CreateDate,
                CoID
            )";
            return sql;
        }
        public static string AddLockItemSql()
        {
            string sql = @"INSERT INTO invlock_item(
                    @ParentID,
                    @Skuautoid,
                    @Qty,
                    @Creator,
                    @CreateDate,
                    @CoID
                ) VALUES (
                    ParentID,
                    Skuautoid,
                    Qty,
                    Creator,
                    CreateDate,
                    CoID )";
            return sql;
        }
        #endregion



    }
}