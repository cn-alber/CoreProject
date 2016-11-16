using CoreModels;
using Dapper;
using System;
using System.Collections.Generic;
using CoreModels.XyComm;
using CoreModels.XyCore;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
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

        #region 获取所有省市区
        public static DataResult GetAllArea()
        {
            var result = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {
                    string sql = @"SELECT  area.ID as value,  area.ParentId ,  area.`Name` as label FROM area WHERE LevelType = 1 AND ParentId = 100000 ";
                    string cSql = "";
                    var province = conn.Query<AreaAll>(sql).AsList();
                    foreach (AreaAll p in province)
                    {
                        cSql = @"SELECT  area.ID as value,  area.ParentId ,  area.`Name` as label FROM area WHERE LevelType = 2 AND ParentId =  " + p.value;
                        p.children = new List<AreaAll>();
                        var citys = conn.Query<AreaAll>(cSql).AsList();
                        p.children = citys;
                        cSql = "";
                        string dSql = "";
                        foreach (AreaAll c in citys)
                        {
                            dSql = @"SELECT  area.ID as value,  area.ParentId ,  area.`Name` as label FROM area WHERE LevelType = 3 AND ParentId =  " + c.value;
                            c.children = new List<AreaAll>();
                            var district = conn.Query<AreaAll>(dSql).AsList();
                            c.children = district;
                            dSql = "";
                        }
                    }

                    result.d = JsonConvert.DeserializeObject<List<AreaCascader>>(JsonConvert.SerializeObject(province));
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
            comp = comp + DateTime.Now.ToString("yy-MM-dd").Substring(0, 2);
            comp = comp + DateTime.Now.ToString("yy-MM-dd").Substring(3, 2);
            comp = comp + DateTime.Now.ToString("yy-MM-dd").Substring(6, 2);
            Random Rd = new Random((int)DateTime.Now.Ticks);
            string rds = Rd.Next().ToString();
            comp = comp + rds.Remove(0, rds.Length - 5);
            return comp;
        }
        #endregion


        #region 公用方法仓库基础资料查询
        public static DataResult GetWhViewAll(string CoID)
        {
            var result = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {
                    Dictionary<string, object> DicWh = new Dictionary<string, object>();
                    string Sql = @"SELECT ID,WarehouseName AS WhName FROM warehouse WHERE CoID=@CoID";
                    var WhLst = conn.Query<Warehouse_view>(Sql, new { CoID = CoID }).AsList();
                    foreach (var wh in WhLst)
                    {
                        DicWh.Add(wh.ID, wh);
                    }
                    result.d = DicWh;
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d = e.Message;
                }
            }
            return result;
        }
        public static DataResult GetWhViewByID(string CoID, List<String> IDLst)
        {
            var result = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {
                    Dictionary<string, object> DicWh = new Dictionary<string, object>();
                    string Sql = @"SELECT ID,WarehouseName AS WhName FROM warehouse WHERE CoID=@CoID AND ID in @IDLst";
                    var WhLst = conn.Query<Warehouse_view>(Sql, new { CoID = CoID, IDLst = IDLst }).AsList();
                    foreach (var wh in WhLst)
                    {
                        DicWh.Add(wh.ID, wh);
                    }
                    result.d = DicWh;
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d = e.Message;
                }
            }
            return result;
        }
        #endregion


        #region 公用方法——获取商品编号&名称&规格&图片
        public static DataResult GetSkuViewByID(string CoID, List<String> IDLst)
        {
            var result = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    Dictionary<string, object> DicSku = new Dictionary<string, object>();
                    string Sql = @"SELECT ID,SkuID,SkuName,Norm,Img FROM coresku WHERE CoID=@CoID AND ID in @IDLst";
                    var SkuLst = conn.Query<CoreSkuView>(Sql, new { CoID = CoID, IDLst = IDLst }).AsList();
                    foreach (var sku in SkuLst)
                    {
                        DicSku.Add(sku.ID, sku);
                    }
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d = e.Message;
                }
            }
            return result;
        }
        #endregion



        #region 公用方法 - 判断仓库是否启用 精细化管理
        public static DataResult IsPositionAccurate(string CoID)
        {
            var result = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                { 
                    string sql = @"SELECT IsPositionAccurate FROM ware_setting WHERE CoID=@CoID";
                    var ispost = conn.Query(sql,new{CoID=CoID}).AsList();
                    // if


                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d = e.Message;
                }
            }
            return result;
        }

        #endregion
    }
}