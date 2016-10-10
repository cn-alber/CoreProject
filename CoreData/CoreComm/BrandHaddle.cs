using System;
using System.Text;
using System.Collections.Generic;
// using System.Linq;
using CoreModels;
using CoreModels.XyComm;
using Dapper;
using MySql.Data.MySqlClient;
namespace CoreData.CoreComm
{
    public static class BrandHaddle
    {
        #region 品牌 - 资料查询
        public static DataResult GetBrandLst(BrandParam IParam)
        {
            var res = new DataResult(1, null);
            var info = new BrandData();
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {
                    StringBuilder querysql = new StringBuilder();
                    var p = new DynamicParameters();
                    string sql = @"SELECT
                                        brand.ID,
                                        brand.`Name`,
                                        brand.Intro,
                                        brand.Link,
                                        brand.`Enable`,
                                        brand.CoID,
                                        brand.Creator,
                                        brand.CreateDate
                                    FROM
                                        brand
                                    WHERE CoID = @CoID";
                    querysql.Append(sql);
                    p.Add("@CoID", IParam.CoID);
                    if (!string.IsNullOrEmpty(IParam.Enable) && IParam.Enable.ToUpper() != "ALL")//是否启用
                    {
                        querysql.Append(" AND Enable = @Enable");
                        p.Add("@Enable", IParam.Enable.ToUpper() == "TRUE" ? true : false);
                    }
                    if (!string.IsNullOrEmpty(IParam.Filter))
                    {
                        querysql.Append(" AND (Name like @Filter)");
                        p.Add("@Filter", "%" + IParam.Filter + "%");
                    }
                    if (!string.IsNullOrEmpty(IParam.SortField) && !string.IsNullOrEmpty(IParam.SortDirection))//排序
                    {
                        querysql.Append(" ORDER BY " + IParam.SortField + " " + IParam.SortDirection);
                    }
                    var BrandLst = DbBase.CommDB.Query<Brand>(querysql.ToString(), p).AsList();
                    if (BrandLst.Count == 0)
                    {
                        res.s = -3001;
                    }
                    else
                    {
                        info.DataCount = BrandLst.Count;
                        decimal pagecnt = Math.Ceiling(decimal.Parse(info.DataCount.ToString()) / decimal.Parse(IParam.PageSize.ToString()));
                        info.PageCount = Convert.ToInt32(pagecnt);
                        int dataindex = (IParam.PageIndex - 1) * IParam.PageSize;
                        querysql.Append(" LIMIT @ls , @le");
                        p.Add("@ls", dataindex);
                        p.Add("@le", IParam.PageSize);
                        BrandLst = CoreData.DbBase.CommDB.Query<Brand>(querysql.ToString(), p).AsList();
                        info.BrandLst = BrandLst;
                        res.d = info;
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

        #region 品牌 - 编辑|查询
        public static DataResult GetBrandEdit(string ID)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {
                    string querysql = @"SELECT
                                            brand.ID,
                                            brand.`Name`,
                                            brand.Intro,
                                            brand.Link,
                                            brand.`Enable`,
                                            brand.CoID,
                                            brand.Creator,
                                            brand.CreateDate
                                        FROM
                                            brand
                                        WHERE ID = @ID ";
                    var p = new { ID = ID };
                    var info = DbBase.CommDB.QueryFirst<Brand>(querysql, p);
                    if (info == null)
                    {
                        res.s = -3001;
                    }
                    res.d = info;
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

        #region 状态(停用|启用)
        public static DataResult UptBrandEnable(List<int> IDLst, string CoID, string UserName, bool Enable)
        {
            var res = new DataResult(1, null);
            var UserDBconn = new MySqlConnection(DbBase.UserConnectString);
            var CommDBconn = new MySqlConnection(DbBase.CommConnectString);
            UserDBconn.Open();
            CommDBconn.Open();
            var TransComm = CommDBconn.BeginTransaction();
            var TransUser = UserDBconn.BeginTransaction();
            try
            {
                string contents = string.Empty;
                string uptsql = @"update brand set Enable = @Enable where ID in @ID";
                var args = new { ID = IDLst, Enable = Enable };
                int count = CommDBconn.Execute(uptsql, args, TransComm);
                if (count < 0)
                {
                    res.s = -3003;
                }
                else
                {
                    if (Enable)
                    {
                        contents = "品牌状态启用：";
                        res.s = 3001;
                    }
                    else
                    {
                        contents = "品牌状态停用：";
                        res.s = 3002;
                    }
                    contents += string.Join(",", IDLst.ToArray());
                    CoreUser.LogComm.InsertUserLogTran(TransUser, "修改品牌状态", "Brand", contents, UserName, CoID, DateTime.Now);

                    if (res.s == 1)
                    {
                        TransComm.Commit();
                        TransUser.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                TransComm.Rollback();
                TransUser.Rollback();
                res.s = -1;
                res.d = e.Message;
            }
            finally
            {
                TransComm.Dispose();
                TransUser.Dispose();
                CommDBconn.Dispose();
                UserDBconn.Dispose();
            }
            return res;
        }
        #endregion

        #region 新增品牌
        public static DataResult SaveInsertBrand(Brand brand, int CoID, string UserName)
        {
            var res = new DataResult(1, null);
            string sql = @"INSERT INTO brand
            (
                `Name`,
                Intro,
                Link,
                `Enable`,
                CoID,
                Creator,
                CreateDate
            ) VALUES(
                @Name,
                @Intro,
                @Link,
                @Enable,
                @CoID,
                @Creator,
                @CreateDate
            )";
            var b = new Brand();
            b.CoID = CoID;
            b.Name = brand.Name;
            b.Intro = brand.Intro;
            b.Link = brand.Link;
            b.Enable = brand.Enable;
            b.Creator = UserName;
            b.CreateDate = DateTime.Now.ToString();
            var UserDBconn = new MySqlConnection(DbBase.UserConnectString);
            var CommDBconn = new MySqlConnection(DbBase.CommConnectString);
            UserDBconn.Open();
            CommDBconn.Open();
            var TransUser = UserDBconn.BeginTransaction();
            var TransComm = CommDBconn.BeginTransaction();
            try
            {
                int count = CommDBconn.Execute(sql, b, TransComm);
                if (count < 0)
                {
                    res.s = -3002;
                }
                else
                {
                    CoreUser.LogComm.InsertUserLogTran(TransUser, "新增品牌", "Brand", b.Name, UserName, CoID.ToString(), DateTime.Now);
                }
                if (res.s == 1)
                {
                    TransComm.Commit();
                    TransUser.Commit();
                }
            }
            catch (Exception e)
            {
                TransComm.Rollback();
                TransUser.Rollback();
                res.s = -1;
                res.d = e.Message;
            }
            finally
            {
                TransComm.Dispose();
                TransUser.Dispose();
                CommDBconn.Dispose();
                UserDBconn.Dispose();
            }

            return res;
        }
        #endregion

        #region 修改品牌
        public static DataResult SaveUpdateBrand(Brand brand, int CoID, string UserName)
        {
            string contents = string.Empty;
            var result = new DataResult(1,null);
            var res = GetBrandEdit(brand.ID.ToString());
            var brandOld = res.d as Brand;
            if (brandOld.Name != brand.Name)
            {
                contents = contents + "品牌:" + brandOld.Name + "=>" + brand.Name + ";";
            }
            if (brandOld.Intro != brand.Intro)
            {
                contents = contents + "简称:" + brandOld.Intro + "=>" + brand.Intro + ";";
            }
            if (brandOld.Link != brand.Link)
            {
                contents = contents + "链接:" + brandOld.Link + "=>" + brand.Link + ";";
            }
            var UserDBconn = new MySqlConnection(DbBase.UserConnectString);
            var CommDBconn = new MySqlConnection(DbBase.CommConnectString);
            UserDBconn.Open();
            CommDBconn.Open();
            var TransUser = UserDBconn.BeginTransaction();
            var TransComm = CommDBconn.BeginTransaction();
            try
            {
                string str = @"UPDATE brand
                SET Name = @Name,
                    Intro = @Intro,
                    Link = @Link              
                WHERE ID = @ID
                ";
                int count = CommDBconn.Execute(str, brand, TransComm);
                if (count <= 0)
                {
                    result.s = -3003;
                }
                else
                {
                    CoreUser.LogComm.InsertUserLogTran(TransUser, "修改品牌资料", "Brand", contents, UserName, CoID.ToString(), DateTime.Now);
                    TransUser.Commit();
                    TransComm.Commit();
                }
            }
            catch (Exception e)
            {
                TransComm.Rollback();
                TransUser.Rollback();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransComm.Dispose();
                TransUser.Dispose();
                CommDBconn.Dispose();
                UserDBconn.Dispose();
            }
            return result;
        }
        #endregion

        #region 删除品牌
        public static DataResult DelBrand(List<int> IDLst, int CoID, string UserName)
        {
            var res = new DataResult(1, null);
            var UserDBconn = new MySqlConnection(DbBase.UserConnectString);
            var CommDBconn = new MySqlConnection(DbBase.CommConnectString);
            UserDBconn.Open();
            CommDBconn.Open();
            var TransUser = UserDBconn.BeginTransaction();
            var TransComm = CommDBconn.BeginTransaction();
            try
            {
                var sql = string.Empty;
                var p = new DynamicParameters();
                sql = "update brand set IsDelete = @IsDelete,Deleter=@Deleter,DeleteDate=@DeleteDate where CoID = @CoID and ID in @IDLst";
                p.Add("@IsDelete", 1);
                p.Add("@Deleter", UserName);
                p.Add("@DeleteDate", DateTime.Now);
                p.Add("@CoID", CoID);
                p.Add("@IDLst", IDLst);
                int count = DbBase.CommDB.Execute(sql, p, TransComm);
                if (count > 0)
                {
                    string contents = "删除品牌=>" + string.Join(",", IDLst);
                    CoreUser.LogComm.InsertUserLogTran(TransUser, "删除品牌资料", "Brand", contents, UserName, CoID.ToString(), DateTime.Now);
                    TransComm.Commit();
                    TransUser.Commit();
                }
            }
            catch (Exception e)
            {
                TransComm.Rollback();
                TransUser.Rollback();
                res.s = -1;
                res.d = e.Message;
            }
            finally
            {
                TransComm.Dispose();
                TransUser.Dispose();
                CommDBconn.Dispose();
                UserDBconn.Dispose();
            }
            return res;
        }
        #endregion

        #region 获取品牌列表
        public static DataResult GetBrandALL(int CoID)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {
                    string querysql = @"SELECT
                                            brand.ID,
                                            brand.`Name`,
                                            brand.Intro,
                                            brand.Link,
                                            brand.`Enable`,
                                            brand.CoID,
                                            brand.Creator,
                                            brand.CreateDate
                                        FROM
                                            brand
                                        WHERE CoID = @CoID
                                        ORDER BY brand.`Name` ";
                    var p = new { CoID = CoID };
                    var info = DbBase.CommDB.Query<Brand>(querysql, p).AsList();
                    if (info == null)
                    {
                        res.s = -3001;
                    }
                    res.d = info;
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
    }
}