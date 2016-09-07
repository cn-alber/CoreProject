using CoreModels.XyUser;
using MySql.Data.MySqlClient;
using Dapper;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CoreData
{
    public static class MySqlData
    {
        public static List<User> GetData()
        {    
            MySqlConnection con = new MySqlConnection("server=xieyuntestout.mysql.rds.aliyuncs.com;database=xyuser;uid=xieyun;pwd=xieyun123;Port=3306;SslMode=None");
            return con.Query<User>("select * from user limit 10").AsList();
            
        }

        public static string GetRedisData()
        {
            User user = DbBase.UserDB.Query<User>("select * from user limit 1").AsList()[0];
            if (!CacheBase.Set<User>("user1", user))
            {
                return null;
            }

            CacheBase.Set<string>("user2", "test");

            var cuser = CacheBase.Get<string>("user2");

            return JsonConvert.SerializeObject(cuser);
        }
    }
}
