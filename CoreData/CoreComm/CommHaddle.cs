using CoreModels;
using Dapper;
using MySql.Data.MySqlClient;
namespace CoreData.CoreComm
{
    public static class CommHaddle
    {
        public static DataResult SysColumnExists(string CommConnectString, string tablename, string colname)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(CommConnectString))
            {
                string sql = @"SELECT
                                    count(*)
                                FROM
                                    information_schema. COLUMNS
                                WHERE
                                    table_name = @tablename
                                AND column_name = @colname";
                var args = new { tablename = tablename, colname = colname };
                int count = conn.QueryFirst<int>(sql, args);
                if (count == 0)
                {
                    res.s = -1;
                    res.d = "无效参数"+colname;
                    // res.d = "表(" + tablename + ")不包含名(" + colname + ")";
                }
            }
            return res;
        }
    }
}