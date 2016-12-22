using System;
using System.Collections.Generic;
using CoreModels.XyCore;
using Dapper;
using MySql.Data.MySqlClient;

namespace CoreData.CoreCore
{
    public static class DistributorHaddle
    {
        public static List<distributorEnum> getDisEnum(string CoID)
        {
            var res = new List<distributorEnum>();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try
                {
                    string sql = @"SELECT ID as value ,DistributorName as label FROM distributor WHERE CoID="+CoID+" AND Type = 0 AND `Enable`=TRUE;";
                    Console.WriteLine(sql);
                    res = conn.Query<distributorEnum>(sql).AsList();                                  
                }
                catch
                {
                    conn.Dispose();
                }
            }
            return res;
        }
        public static string getDisName(string CoID,string id)
        {
            string res = string.Empty;
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try
                {
                    string sql = @"SELECT DistributorName FROM distributor WHERE CoID="+CoID+" AND id = " + id;
                    res  = conn.QueryFirst<string>(sql);                                  
                }
                catch
                {
                    conn.Dispose();
                }
            }
            return res;
        }
    }
}