using System.Collections.Generic;
using CoreModels;
using CoreModels.XyUser;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Text;
namespace CoreData.CoreUser
{
    public static class NoticeHaddle
    {
        #region 系统通知 - 资料查询
        public static DataResult GetNoticeLst(NoticeParam IParam)
        {
            var res = new DataResult(1, null);
            var not = new NoticeData();
            using (var conn = new MySqlConnection(DbBase.UserConnectString))
            {
                try
                {
                    StringBuilder querysql = new StringBuilder();
                    var p = new DynamicParameters();
                    string sql = @"SELECT
                                    notice.ID,
                                    notice.Coid,
                                    notice.Title,
                                    notice.Content,
                                    notice.Creator,
                                    notice.CreateDate,
                                    notice.Type
                                FROM notice
                                WHERE notice.Coid = @CoID AND IsDelete = 0";
                    querysql.Append(sql);
                    p.Add("@CoID", IParam.CoID);

                    if (!string.IsNullOrEmpty(IParam.Enable) && IParam.Enable.ToUpper() != "ALL")//是否启用
                    {
                        querysql.Append(" AND `user`.Enable = @Enable");
                        p.Add("@Enable", IParam.Enable.ToUpper() == "TRUE" ? true : false);
                    }
                    if (!string.IsNullOrEmpty(IParam.Filter))
                    {
                        querysql.Append(" AND (Title like @Filter or Content like @Filter)");
                        p.Add("@Filter", "%" + IParam.Filter + "%");
                    }
                    if (!string.IsNullOrEmpty(IParam.SortField) && !string.IsNullOrEmpty(IParam.SortDirection))//排序
                    {
                        querysql.Append(" ORDER BY " + IParam.SortField + " " + IParam.SortDirection);
                    }
                    var NotLst = DbBase.UserDB.Query<Notice>(querysql.ToString(), p).AsList();
                    if (NotLst.Count == 0)
                    {
                        res.s = -3001;
                    }
                    else
                    {
                        not.DataCount = NotLst.Count;
                        decimal pagecnt = Math.Ceiling(decimal.Parse(not.DataCount.ToString()) / decimal.Parse(IParam.PageSize.ToString()));
                        not.PageCount = Convert.ToInt32(pagecnt);
                        int dataindex = (IParam.PageIndex - 1) * IParam.PageSize;
                        querysql.Append(" LIMIT @ls , @le");
                        p.Add("@ls", dataindex);
                        p.Add("@le", IParam.PageSize);
                        NotLst = CoreData.DbBase.UserDB.Query<Notice>(querysql.ToString(), p).AsList();
                        not.NoticeLst = NotLst;
                        res.d = not;
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

        #region 单笔系统通知 - 编辑|查询
        public static DataResult GetNoticeEdit(String NotID)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.UserConnectString))
            {
                try
                {
                    string querysql = @"SELECT
                                    notice.ID,
                                    notice.Coid,
                                    notice.Title,
                                    notice.Content,
                                    notice.Creator,
                                    notice.CreateDate,
                                    notice.Type
                                FROM notice
                                WHERE ID = @NotID AND IsDelete = 0";
                    var p = new { NotID = NotID };
                    var us = DbBase.UserDB.QueryFirst<Notice>(querysql, p);
                    if (us == null)
                    {
                        res.s = -3001;
                    }
                    res.d = us;
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

        #region 新增系统通知
        public static DataResult SaveInsertNot(Notice not, int CoID, String UserName)
        {
            var result = new DataResult(1, null);
            string sqlCommandText = @"INSERT INTO notice
                                        (Coid,
                                        Title,
                                        Content,
                                        Creator,
                                        CreateDate,
                                        Type) VALUES(
                                        @Coid,
                                        @Title,
                                        @Content,
                                        @Creator,
                                        @CreateDate,
                                        @Type)";

            var n = new Notice();
            n.Coid = CoID;
            n.Title = not.Title;
            n.Content = not.Content;
            n.Creator = UserName;
            n.CreateDate = DateTime.Now.ToString();
            n.Type = not.Type;
            var UserDBconn = new MySqlConnection(DbBase.UserConnectString);
            UserDBconn.Open();
            var TransUser = UserDBconn.BeginTransaction();
            try
            {
                int count = UserDBconn.Execute(sqlCommandText, n, TransUser);
                if (count < 0)
                {
                    result.s = -3002;
                }
                else
                {
                    CoreUser.LogComm.InsertUserLogTran(TransUser, "新增通知", "Notice", n.Title, UserName, CoID.ToString(), DateTime.Now);
                }
                if (result.s == 1)
                {
                    TransUser.Commit();
                }
            }
            catch (Exception e)
            {
                TransUser.Rollback();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransUser.Dispose();
                UserDBconn.Close();
            }

            return result;
        }
        #endregion

        #region 修改系统通知
        public static DataResult SaveUpdateNot(Notice not, int CoID, string UserName)
        {

            string contents = string.Empty;
            var result = new DataResult(1, null);
            var res = GetNoticeEdit(not.ID.ToString());
            var notOld = res.d as Notice;
            if (notOld.Title != not.Title)
            {
                contents = contents + "通知名称：" + notOld.Title + "=>" + not.Title;
            }
            if (notOld.Type != not.Type)
            {
                contents = contents + "通知类型：" + notOld.Type + "=>" + not.Type;
            }
            if (notOld.Content != not.Content)
            {
                contents = contents + "通知内容：" + notOld.Content + "=>" + not.Content;
            }
            using (var UserDBconn = new MySqlConnection(DbBase.UserConnectString))
            {
                UserDBconn.Open();
                var TransUser = UserDBconn.BeginTransaction();
                try
                {
                    string str = @"UPDATE notice
                    SET Title = @Title,
                    Type = @Type,
                    Content = @Content
                    WHERE ID = @ID";
                    int count = UserDBconn.Execute(str, not, TransUser);
                    if (count <= 0)
                    {
                        result.s = -3003;
                    }
                    else
                    {
                        CoreUser.LogComm.InsertUserLogTran(TransUser, "修改系统通知", "Notice", contents, UserName, CoID.ToString(), DateTime.Now);
                        TransUser.Commit();
                    }

                }
                catch (Exception e)
                {
                    TransUser.Rollback();
                    result.s = -1;
                    result.d = e.Message;
                }
                finally
                {
                    TransUser.Dispose();
                    UserDBconn.Dispose();
                }
                return result;
            }
        }
        #endregion

        #region 删除系统通知
        public static DataResult DeleteNot(List<int> IDLst, int CoID, string UserName)
        {
            var result = new DataResult(1, null);
            using (var UserDBconn = new MySqlConnection(DbBase.UserConnectString))
            {
                UserDBconn.Open();
                var TransUser = UserDBconn.BeginTransaction();
                try
                {
                    var p = new DynamicParameters();
                    //var sql = "delete from notice where Coid = @CoID and ID in @ID";
                    var sql = "update notice set IsDelete = @IsDelete,Deleter=@Deleter,DeleteDate=@DeleteDate where Coid = @CoID and ID in @ID";
                    p.Add("@IsDelete", 1);
                    p.Add("@Deleter", UserName);
                    p.Add("@DeleteDate",DateTime.Now);                   
                    p.Add("@CoID", CoID);
                    p.Add("@ID", IDLst);
                    int count = DbBase.UserDB.Execute(sql, p, TransUser);
                    if (count > 0)
                    {
                        string contents = "删除通知=>" + string.Join(",", IDLst);
                        LogComm.InsertUserLogTran(TransUser, "删除通知", "Notice", contents, UserName, CoID.ToString(), DateTime.Now);
                    }
                    TransUser.Commit();
                }
                catch (Exception e)
                {
                    TransUser.Rollback();
                    result.s = -1;
                    result.d = e.Message;
                }
                finally
                {
                    TransUser.Dispose();
                    UserDBconn.Dispose();
                }

            }
            return result;
        }
        #endregion
    }
}