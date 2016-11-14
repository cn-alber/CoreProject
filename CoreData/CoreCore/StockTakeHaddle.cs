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
using System.Linq;
using System.Threading.Tasks;

namespace CoreData.CoreCore
{

    public static class StockTakeHaddle
    {
        #region 库存盘点-查询-主表
        public static DataResult GetStockTakeMain(string CoID)
        {
            var result = new DataResult(1, null);
            var main = new Sfc_main_query();
            string sql = @"SELECT ID,WhID,Remark,Status,Creator,CreateDate FROM sfc_main WHERE CoID = @CoID AND Type = 2";
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    var mainLst = conn.Query<Sfc_main_view>(sql, new { CoID = CoID }).AsList();
                    if (mainLst.Count > 0)
                    {
                        var WhIDLst = mainLst.Select(a => a.WhID).Distinct().AsList();
                        var res = CommHaddle.GetWhViewByID(CoID, WhIDLst);
                        main.DicWh = res.d as Dictionary<string, object>;//获取仓库List资料
                        main.MainLst = mainLst;
                    }
                    result.d = main;
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
            string sql = @"SELECT ID,Skuautoid,Qty,InvQty,ParentID,Type FROM sfc_item WHERE CoID = @CoID AND ParentID=@ParentID AND Type = 2";
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    var ItemLst = conn.Query<Sfc_item_view>(sql, new { CoID = CoID, ParentID = ParentID }).AsList();
                    if (ItemLst.Count > 0)
                    {
                        var SkuIDLst = ItemLst.Select(a => a.Skuautoid).Distinct().AsList();
                        var res = CommHaddle.GetSkuViewByID(CoID, SkuIDLst);
                        Item.DicSku = res.d as Dictionary<string, object>;//获取商品Sku资料
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
        public static DataResult InsertStockTakeMain(string WhID, string Parent_WhID, string CoID, string UserName)
        {
            var result = new DataResult(1, null);
            var conn = new MySqlConnection(DbBase.CoreConnectString);
            conn.Open();
            var Trans = conn.BeginTransaction();
            try
            {
                var main = new Sfc_main();
                main.CoID = CoID;
                main.Creator = UserName;
                main.CreateDate = DateTime.Now.ToString();
                main.Type = 2;
                main.WhID = WhID;
                main.Parent_WhID = Parent_WhID;
                conn.Execute(AddSfcMainSql(), main, Trans);
                Trans.Commit();
                CoreUser.LogComm.InsertUserLog("新增库存盘点", "sfc_main", "新增盘点单" + WhID.ToString(), main.Creator, int.Parse(CoID), DateTime.Now);
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
        public static DataResult InsertStockTakeItem(string ParentID, List<int> SkuIDLst, string CoID, string UserName)
        {
            var result = new DataResult(1, null);
            var conn = new MySqlConnection(DbBase.CoreConnectString);
            conn.Open();
            var Trans = conn.BeginTransaction();
            try
            {
                string sql = @"SELECT WhID,Parent_WhID FROM sfc_main WHERE CoID=@CoID AND ParentID=@ParentID";
                var main = conn.QueryFirst<Sfc_main>(sql, new { CoID = CoID, ParentID = ParentID });
                var ItemLst = SkuIDLst.Select(a => new Sfc_item
                {
                    WhID = main.WhID,
                    ParentID = ParentID,
                    Parent_WhID = main.Parent_WhID,
                    Skuautoid = a.ToString(),
                    Type = 2,
                    CoID = CoID,
                    Creator = UserName,
                    CreateDate = DateTime.Now.ToString()
                }).AsList();
                conn.Execute(AddSfcItemSql(), ItemLst, Trans);
                Trans.Commit();
                CoreUser.LogComm.InsertUserLog("新增库存盘点明细", "sfc_main", "新增盘点" + string.Join(",", SkuIDLst.ToArray()), main.Creator, int.Parse(CoID), DateTime.Now);
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
        public static DataResult SaveTakeQty(string ID, string InvQty, string CoID, string UserName)
        {
            var result = new DataResult(1, null);
            var conn = new MySqlConnection(DbBase.CoreConnectString);
            conn.Open();
            var Trans = conn.BeginTransaction();
            try
            {
                string sql = "UPDATE sfc_item SET InvQty = @InvQty,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE CoID=@CoID AND ID=@ID";
                conn.Execute(sql, new { CoID = CoID, ID = ID, Modifier = UserName, ModifyDate = DateTime.Now.ToString() }, Trans);
                Trans.Commit();
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
            var Trans = conn.BeginTransaction();
            try
            {
                string sql = "UPDATE sfc_main SET Status = 1 WHERE CoID=@CoID AND ID=@ID";//(0:待确认;1:生效;2.作废)
                string querymainsql = @"SELECT ID,WhID,Parent_WhID FROM Sfc_main WHERE ID=@ID";
                string querysql = @"SELECT ID,Skuautoid,WhID,InvQty,Parent_WhID FROM sfc_item WHERE CoID=@CoID AND ParentID=@ID";
                string InvQuerySql = @"SELECT ID,Skuautoid,StockQty FROM Inventory WHERE CoID=@CoID AND WarehouseID=@WarehouseID AND Skuautoid in @SkuIDLst";
                var main = conn.QueryFirst<Sfc_main>(querymainsql, new { CoID = CoID, ID = ID });
                var itemLst = conn.Query<Sfc_item>(querysql, new { CoID = CoID, ID = ID }).AsList();
                if (itemLst.Count > 0)
                {
                    string Parent_WhID = main.Parent_WhID;//主仓ID
                    string WhID = main.WhID;//分仓ID
                    var SkuIDLst = itemLst.Select(a => a.Skuautoid).AsList();
                    var InvSkuLst = conn.Query<Sfc_InvStock>(InvQuerySql, new { CoID = CoID, @WarehouseID = Parent_WhID, SkuIDLst = SkuIDLst }).AsList();//读取现有库存         
                    var RecordID = "INV" + CommHaddle.GetRecordID(int.Parse(CoID));
                    int Type = 1401;
                    string CusType = "盘点";
                    //交易主表         
                    var inv = new Invinout();
                    inv.RecordID = RecordID;
                    inv.Type = Type;
                    inv.CusType = CusType;
                    inv.Status = 1;//(0:待审核;1.审核通过;2.作废)
                    inv.WhID = Parent_WhID;
                    inv.LinkWhID = WhID;
                    inv.Creator = UserName;
                    inv.CreateDate = DateTime.Now.ToString();
                    inv.CoID = CoID;
                    conn.Execute(InventoryHaddle.AddInvinoutSql(), inv, Trans);
                    //交易明细
                    var invitem = (from a in itemLst
                                   join b in InvSkuLst on a.Skuautoid equals b.Skuautoid into result1
                                   from c in result1.DefaultIfEmpty()
                                   select new Invinoutitem
                                   {
                                       IoID = RecordID,
                                       CusType = CusType,
                                       Skuautoid = c.Skuautoid,
                                       WhID = Parent_WhID,
                                       LinkWhID=WhID,
                                       Qty = (c.Skuautoid == null ? int.Parse(a.Qty) : (int.Parse(a.Qty) - int.Parse(c.StockQty))),
                                       CoID = CoID,
                                       Creator = UserName,
                                       CreateDate = DateTime.Now.ToString()
                                   }).ToList<Invinoutitem>();
                    //库存sku新增
                    var NewInvLst = itemLst.Where(a => !InvSkuLst
                                                        .Select(b => b.Skuautoid)
                                                        .Contains(a.Skuautoid))
                                        .Select(a => new Inventory
                                        {
                                            Skuautoid = a.Skuautoid,
                                            WarehouseID = a.Parent_WhID,
                                            StockQty = int.Parse(a.Qty),
                                            CoID = CoID
                                        }).AsList();
                    //库存更新
                    foreach(var v in InvSkuLst)
                    {
                        v.StockQty=itemLst.Where(a=>a.Skuautoid.Contains(v.Skuautoid)).Select(a=>a.InvQty).First();
                    }
                   

                }



                conn.Execute(sql, new { CoID = CoID, ID = ID }, Trans);
                Trans.Commit();
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