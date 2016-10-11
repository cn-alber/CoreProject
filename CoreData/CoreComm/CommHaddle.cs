using CoreModels;
using Dapper;
using System;
using System.Collections.Generic;
using CoreModels.XyComm;
using MySql.Data.MySqlClient;
// using Newtonsoft.Json;



namespace CoreData.CoreComm
{
    public static class CommHaddle
    {
        #region 判断系统表是否存在指定字段
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
                    res.d = "无效参数" + colname;
                    // res.d = "表(" + tablename + ")不包含名(" + colname + ")";
                }
            }
            return res;
        }
        #endregion

        #region 获取省市区列表
        public static DataResult GetAreaLst(int LevelType, int ParentId)
        {
            var res = new DataResult(1, null);
            var areaname = "area" + LevelType.ToString();
            // string strCache = CacheBase.Get<string>(areaname);
            var AreaLst = CacheBase.Get<List<Area>>(areaname);
            // if (string.IsNullOrEmpty(strCache))
            if (AreaLst == null || AreaLst.Count == 0)
            {
                string sql = @"SELECT
                                area.ID,
                                area.ParentId,
                                area.`Name`,
                                area.MergerName,
                                area.ShortName,
                                area.MergerShortName,
                                area.LevelType,
                                area.CityCode,
                                area.ZipCode,
                                area.Pinyin,
                                area.Jianpin,
                                area.FirstChar
                            FROM
                                area
                            WHERE
                                LevelType = @LevelType
                            AND ParentId = @ParentId
                            ";
                var args = new { LevelType = LevelType, ParentId = ParentId };
                using (var conn = new MySqlConnection(DbBase.CommConnectString))
                {
                    try
                    {
                        AreaLst = conn.Query<Area>(sql, args).AsList();
                        if (AreaLst.Count <= 0)
                        {
                            res.s = -3001;
                        }
                        res.d = AreaLst;
                        //strCache = JsonConvert.SerializeObject(AreaLst);
                        CacheBase.Set(areaname, AreaLst);
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
            }
            else
            {
                //var AreaLst = JsonConvert.DeserializeObject<List<Area>>(strCache);
                res.d = AreaLst;
            }
            return res;
        }
        #endregion

        #region 获取单据编号
        public static string GetRecordID(int CoID)
        {
            string comp = CoID.ToString();
            int a = comp.Length;
            if (a == 1)
            { comp = comp + "000"; }
            if (a == 2)
            { comp = comp + "00"; }
            if (a == 3)
            { comp = comp + "0"; }
            if (a > 3)
            { comp = comp.Substring(0, 4); }
            comp = comp + DateTime.Now.ToString("yyyy-MM-dd").Substring(0, 4);
            comp = comp + DateTime.Now.ToString("yyyy-MM-dd").Substring(5, 2);
            comp = comp + DateTime.Now.ToString("yyyy-MM-dd").Substring(8, 2);
            Random Rd = new Random((int)DateTime.Now.Ticks);
            string rds = Rd.Next().ToString();
            comp = comp + rds.Remove(0, rds.Length - 5);
            return comp;
        }
        #endregion
    }
}