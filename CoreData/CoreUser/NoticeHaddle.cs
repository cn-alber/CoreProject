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
        public static DataResult GetNoticeLst()
        {
            var res = new DataResult(1, null);
            var not = new NoticeData();
            using (var conn = new MySqlConnection(DbBase.UserConnectString))
            {
                try
                {
                    string sql = @"SELECT * FROM notice ORDER BY ID desc LIMIT 0,1";
                    var lst = conn.Query<Notice2>(sql).AsList();
                    if(lst.Count >0 ){
                        res.d = lst[0];
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

      
        #region 新增系统通知
        public static DataResult SaveInsertNot(Notice not, int CoID, String UserName)
        {
            var result = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.UserConnectString))
            {
                try
                {
                    string sql = @"INSERT INTO notice(Coid,Title,Content,UserID, Date) VALUES(@Coid,@Title,@Content,@UserID,Now())";

                    var rnt = conn.Execute(sql,new {
                        Coid = CoID,
                        Title = not.Title,
                        Content = not.Content,
                        UserID = not.UserID
                    });
                    if(rnt >0){
                        result.s = 1;
                    }else{
                        result.s = -1;
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

        
    }
}