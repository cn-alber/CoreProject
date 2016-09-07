using CoreModels.XyUser;
using MySql.Data.MySqlClient;
using Dapper;
using System.Collections.Generic;

namespace CoreData
{
    public static class MySqlData
    {
        public static List<User> GetData()
        {    
            MySqlConnection con = new MySqlConnection("server=xieyuntestout.mysql.rds.aliyuncs.com;database=xyuser;uid=xieyun;pwd=xieyun123;Port=3306;SslMode=None");
            return con.Query<User>("select * from user limit 10").AsList();
            
        }
    }
}
