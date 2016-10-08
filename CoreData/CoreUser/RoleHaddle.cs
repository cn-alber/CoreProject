using System;
using CoreModels;
using CoreModels.XyUser;
using Dapper;
using MySql.Data.MySqlClient;

namespace CoreData.CoreUser
{
    public static class RoleHaddle{
        public static DataResult getrolelist(string coid){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.UserConnectString) ){
                try
                {
                    string sql = "SELECT a.ID, a.`Name` FROM role as a WHERE a.CompanyID ="+coid;
                    var rnt = conn.Query<RoleList>(sql).AsList();
                    result.d = rnt;
                }
                catch(Exception ex)
                {
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }


            return result;
        }









    }
}