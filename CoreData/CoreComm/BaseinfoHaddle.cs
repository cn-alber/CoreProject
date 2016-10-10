using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using CoreModels;
using CoreModels.XyComm;
using Dapper;
using MySql.Data.MySqlClient;
namespace CoreData.CoreComm
{
    public static class BaseinfoHaddle
    {
        #region 数据字典 - 资料查询
        public static DataResult GetBaseinfoLst(BaseinfoParam IParam)
        {
            var res = new DataResult(1, null);
            var info = new BaseinfoData();
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {
                    StringBuilder querysql = new StringBuilder();
                    var p = new DynamicParameters();
                    string sql = @"SELECT
                                        baseinfo.ID,
                                        baseinfo.Kind,
                                        baseinfo.`Order`,
                                        baseinfo.`Name`,
                                        baseinfo.Content,
                                        baseinfo.`Value`,
                                        baseinfo.`Enable`,
                                        baseinfo.Remark,
                                        baseinfo.CoID,
                                        baseinfo.Creator,
                                        baseinfo.CreateDate
                                    FROM
                                        baseinfo
                                    WHERE CoID = @CoID AND IsDelete = 0";
                    querysql.Append(sql);
                    p.Add("@CoID", IParam.CoID);
                    if (!string.IsNullOrEmpty(IParam.Kind) && IParam.Kind != "数据类型")
                    {
                        querysql.Append(" AND baseinfo.kind = @Kind");
                        p.Add("@Kind", IParam.Kind);
                    }
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
                    var BaseinfoLst = DbBase.CommDB.Query<Baseinfo>(querysql.ToString(), p).AsList();
                    if (BaseinfoLst.Count == 0)
                    {
                        res.s = -3001;
                    }
                    else
                    {
                        info.DataCount = BaseinfoLst.Count;
                        decimal pagecnt = Math.Ceiling(decimal.Parse(info.DataCount.ToString()) / decimal.Parse(IParam.PageSize.ToString()));
                        info.PageCount = Convert.ToInt32(pagecnt);
                        int dataindex = (IParam.PageIndex - 1) * IParam.PageSize;
                        querysql.Append(" LIMIT @ls , @le");
                        p.Add("@ls", dataindex);
                        p.Add("@le", IParam.PageSize);
                        BaseinfoLst = CoreData.DbBase.CommDB.Query<Baseinfo>(querysql.ToString(), p).AsList();
                        info.BaseinfoLst = BaseinfoLst;
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
        #region 数据字典 - 编辑|查询
        public static DataResult GetBaseinfoEdit(string infoID)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {
                    string querysql = @"SELECT
                                            baseinfo.ID,
                                            baseinfo.Kind,
                                            baseinfo.`Order`,
                                            baseinfo.`Name`,
                                            baseinfo.Content,
                                            baseinfo.`Value`,
                                            baseinfo.`Enable`,
                                            baseinfo.Remark,
                                            baseinfo.CoID,
                                            baseinfo.Creator,
                                            baseinfo.CreateDate
                                        FROM
                                            baseinfo
                                        WHERE ID = @ID ";
                    var p = new { ID = infoID };
                    var info = DbBase.CommDB.QueryFirst<Baseinfo>(querysql, p);
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
        #region 新增数据字典
        public static DataResult SaveInsertinfo(Baseinfo info, int CoID, string UserName)
        {
            var result = new DataResult(1, null);
            string sqlCommandText = @"INSERT INTO baseinfo
                                        (Kind,
                                        `Order`,
                                        `Name`,
                                        Content,
                                        `Value`,
                                        `Enable`,
                                        Remark,
                                        CoID,
                                        Creator,
                                        CreateDate) VALUES(
                                        @Kind,
                                        @Order,
                                        @Name,
                                        @Content,
                                        @Value,
                                        @Enable,
                                        @Remark,
                                        @CoID,
                                        @Creator,
                                        @CreateDate
                                        )";
            var n = new Baseinfo();
            n.CoID = CoID;
            n.Content = info.Content;
            n.Enable = info.Enable;
            n.Kind = info.Kind;
            n.Name = info.Name;
            n.Value = info.Value;
            n.Order = info.Order;
            n.Remark = info.Remark;
            n.Creator = UserName;
            n.CreateDate = DateTime.Now.ToString();
            var UserDBconn = new MySqlConnection(DbBase.UserConnectString);
            var CommDBconn = new MySqlConnection(DbBase.CommConnectString);
            UserDBconn.Open();
            CommDBconn.Open();
            var TransComm = CommDBconn.BeginTransaction();
            var TransUser = UserDBconn.BeginTransaction();
            try
            {
                int count = CommDBconn.Execute(sqlCommandText, n, TransComm);
                if (count < 0)
                {
                    result.s = -3002;
                }
                else
                {
                    CoreUser.LogComm.InsertUserLogTran(TransUser, "新增数据字典", "Baseinfo", n.Kind + ' ' + n.Name, UserName, CoID.ToString(), DateTime.Now);
                }
                if (result.s == 1)
                {
                    TransComm.Commit();
                    TransUser.Commit();
                }
            }
            catch (Exception e)
            {
                TransComm.Rollback();
                TransUser.Rollback();
                TransComm.Dispose();
                TransUser.Dispose();
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

        #region 修改数据字典
        public static DataResult SaveUpdateinfo(Baseinfo info, int CoID, string UserName)
        {
            string contents = string.Empty;
            var result = new DataResult(1, null);
            var res = GetBaseinfoEdit(info.ID.ToString());
            var infOld = res.d as Baseinfo;
            if (infOld.Kind != info.Kind)
            {
                contents = contents + "类别：" + infOld.Kind + "=>" + info.Kind;
            }
            if (infOld.Order != info.Order)
            {
                contents = contents + "顺序：" + infOld.Order + "=>" + info.Order;
            }
            if (infOld.Name != info.Name)
            {
                contents = contents + "名称：" + infOld.Name + "=>" + info.Name;
            }
            if (infOld.Content != info.Content)
            {
                contents = contents + "内容：" + infOld.Content + "=>" + info.Content;
            }
            if (infOld.Value != info.Value)
            {
                contents = contents + "文本值：" + infOld.Value + "=>" + info.Value;
            }
            if (infOld.Remark != info.Remark)
            {
                contents = contents + "备注：" + infOld.Remark + "=>" + info.Remark;
            }
            var UserDBconn = new MySqlConnection(DbBase.UserConnectString);
            var CommDBconn = new MySqlConnection(DbBase.CommConnectString);
            UserDBconn.Open();
            CommDBconn.Open();
            var TransComm = CommDBconn.BeginTransaction();
            var TransUser = UserDBconn.BeginTransaction();
            try
            {
                string str = @"UPDATE baseinfo
                    SET Kind = @Kind,
                    `Order` = @Order,
                    `Name` = @Name,
                    Content = @Content,
                    `Value` = @Value,
                    Remark = @Remark
                    WHERE ID = @ID";
                int count = CommDBconn.Execute(str, info, TransComm);
                if (count <= 0)
                {
                    result.s = -3003;
                }
                else
                {
                    CoreUser.LogComm.InsertUserLogTran(TransUser, "修改数据字典", "Baseinfo", contents, UserName, CoID.ToString(), DateTime.Now);
                    TransComm.Commit();
                    TransUser.Commit();
                }
            }
            catch (Exception e)
            {
                TransComm.Rollback();
                TransUser.Rollback();
                TransComm.Dispose();
                TransUser.Dispose();
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

        #region 删除数据字典
        public static DataResult Deleteinfo(List<int> IDLst, int CoID, string UserName)
        {
            var result = new DataResult(1,null);
            var UserDBconn = new MySqlConnection(DbBase.UserConnectString);
            var CommDBconn = new MySqlConnection(DbBase.CommConnectString);
            UserDBconn.Open();
            CommDBconn.Open();
            var TransComm = CommDBconn.BeginTransaction();
            var TransUser = UserDBconn.BeginTransaction();
            try
            {
                var p = new DynamicParameters();
                 var sql = "update baseinfo set IsDelete = @IsDelete,Deleter=@Deleter,DeleteDate=@DeleteDate where Coid = @CoID and ID in @ID";
                    p.Add("@IsDelete", 1);
                    p.Add("@Deleter", UserName);
                    p.Add("@DeleteDate",DateTime.Now);                   
                    p.Add("@CoID", CoID);
                    p.Add("@ID", IDLst);
                    int count = DbBase.CommDB.Execute(sql, p, TransComm);
                    if (count > 0)
                    {
                        string contents = "删除数据字典=>" + string.Join(",", IDLst);
                        CoreUser.LogComm.InsertUserLogTran(TransUser, "删除通知", "Baseinfo", contents, UserName, CoID.ToString(), DateTime.Now);
                    }
                    TransUser.Commit();
                    TransComm.Commit();
            }
            catch (Exception e)
            {
                TransComm.Rollback();
                TransUser.Rollback();
                TransComm.Dispose();
                TransUser.Dispose();
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


    }
}