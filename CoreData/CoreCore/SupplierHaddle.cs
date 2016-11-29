using System;
using System.Collections.Generic;
using CoreModels.XyCore;
using Dapper;
using MySql.Data.MySqlClient;

namespace CoreData.CoreCore
{
    public static class SupplierHaddle{
        public static List<supplierEnum> getSupEnum(string CoID){
            var res = new List<supplierEnum>();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try
                {
                    string sql = @"SELECT ID as value ,DistributorName as label FROM distributor WHERE CoID="+CoID+" AND Type = 1 AND `Enable`=TRUE;";
                    Console.WriteLine(sql);
                    res = conn.Query<supplierEnum>(sql).AsList();                                  
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