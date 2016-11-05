using CoreModels;
using Dapper;
using System;
using System.Text;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using CoreModels.XyComm;
using CoreModels.XyApi.Tmall;
using MySql.Data.MySqlClient;
using CoreData.CoreApi;

namespace CoreData.CoreComm
{
    public static class SkuPropsHaddle
    {
        #region 根据商品类目获取尺码列表
        public static DataResult GetSkuProps(string CoID)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {
                    string SkuPropSql = @"SELECT
                                            customkind_skuprops.pid,
                                            customkind_skuprops.`name`,
                                            customkind.KindName AS KindNames
                                        FROM
                                            customkind_skuprops,
                                            customkind
                                        WHERE
                                            customkind_skuprops.kindid = customkind.ID
                                        AND customkind_skuprops.IsDelete = 0
                                        AND customkind_skuprops.CoID = customkind.CoID
                                        AND customkind_skuprops.CoID = @CoID";
                    string PropValueSql = @"SELECT
                                                pid,
                                                id,
                                                mapping,
                                                NAME
                                            FROM
                                                customkind_skuprops_value
                                            WHERE
                                                CoID =@CoID
                                            AND pid IN @PidLst
                                            AND IsDelete = 0";
                    var SkuProps = conn.Query<skuprops>(SkuPropSql, new { CoID = CoID }).AsList();
                    var SkuPropLst = (from p in SkuProps
                                      group p by new { p.pid, p.name } into g
                                      select new skuprops
                                      {
                                          pid = g.Key.pid,
                                          name = g.Key.name
                                      }).AsList();
                    if (SkuPropLst.Count > 0)
                    {
                        var SkuPropValues = conn.Query<skuprops_value>(PropValueSql, new { CoID = CoID, PidLst = SkuPropLst.Select(a => a.pid).AsList() }).AsList();
                        foreach (var prop in SkuPropLst)
                        {
                            prop.skuprops_values = SkuPropValues.Where(a => a.pid == prop.pid).AsList();
                            prop.KindNames = string.Join(",", SkuProps.Where(a => a.pid == prop.pid).Select(a => a.KindNames).AsList().ToArray());
                        }
                    }
                    res.d = SkuPropLst;
                }
                catch (Exception e)
                {
                    res.s = -1;
                    res.d = e.Message;
                }
            }
            return res;
        }
        #endregion
        #region 删除商品类目尺码列表
        public static DataResult DelSkuProps(string ID, string CoID, string UserName)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                conn.Open();
                var Trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    string sql = @" UPDATE
                                        customkind_skuprops_value
                                    SET IsDelete=1,Modifier=@Modifier,ModifyDate=@ModifyDate
                                    WHERE
                                        CoID =@CoID
                                    AND id = @ID";
                    conn.Execute(sql, new { CoID = CoID, ID = ID, Modifier = UserName, ModifyDate = DateTime.Now.ToString() }, Trans);
                    Trans.Commit();
                }
                catch (Exception e)
                {
                    Trans.Rollback();
                    res.s = -1;
                    res.d = e.Message;
                }
            }
            return res;
        }
        #endregion

        #region 修改商品类目
        public static DataResult UptSkuProps(List<skuprops> SkuPropLst, string CoID, string UserName)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                conn.Open();
                var Trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    var PropValLst = new List<skuprops_value>();
                    // var NewValLst = new List<skuprops_value>();
                    // var UptValLst = new List<skuprops_value>();
                    List<string> PidLst = SkuPropLst.Select(a => a.pid).AsList();
                    string PropValueSql = @"SELECT
                                                pid,
                                                id,
                                                mapping,
                                                NAME
                                            FROM
                                                customkind_skuprops_value
                                            WHERE
                                                CoID =@CoID
                                            AND pid IN @PidLst
                                            AND IsDelete = 0";
                    var OldValLst = conn.Query<skuprops_value>(PropValueSql, new { CoID = CoID, PidLst = PidLst });
                    foreach (var prop in SkuPropLst)
                    {
                        var valLst = prop.skuprops_values.Select(a => new skuprops_value { pid = prop.pid, id = a.id, mapping = a.mapping, name = a.name }).AsList();
                        PropValLst.AddRange(valLst);
                    }
                    //新增sku属性值Lst
                    var NewValLst = PropValLst.Where(a => a.id <= 0)
                    .Select(b => new Customkind_skuprops_value
                    {
                        mapping = b.mapping,
                        name = b.name,
                        pid = b.pid,
                        Creator = UserName,
                        CreateDate = DateTime.Now.ToString(),
                        CoID = int.Parse(CoID)
                    }).AsList();
                    //修改Sku属性值Lst
                    var UptValLst = PropValLst
                    .Where(a => OldValLst.Any(b => b.id == a.id && !(b.mapping == a.mapping && b.name == a.name)))
                    .Select(c => new Customkind_skuprops_value
                    {
                        id = c.id,
                        mapping = c.mapping,
                        name = c.name,
                        pid = c.pid,
                        Modifier = UserName,
                        ModifyDate = DateTime.Now.ToString(),
                        CoID = int.Parse(CoID)
                    }).AsList();
                    if (NewValLst.Count > 0)
                    {
                        conn.Execute(CustomKindHaddle.AddSkuPropValSql(), NewValLst, Trans);
                    }
                    if (UptValLst.Count > 0)
                    {
                        string sql = @" UPDATE
                                        customkind_skuprops_value
                                    SET mapping=@mapping,name=@name,Modifier=@Modifier,ModifyDate=@ModifyDate
                                    WHERE
                                        CoID =@CoID
                                    AND id = @id";
                        conn.Execute(sql, UptValLst, Trans);
                    }
                    Trans.Commit();
                }
                catch (Exception e)
                {
                    Trans.Rollback();
                    res.s = -1;
                    res.d = e.Message;
                }
            }
            return res;
        }
        #endregion

        #region 根据商品类目获取商品规格属性
        public static DataResult GetSkuPropsByKind(string KindID, string CoID)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {
                    string SkuPropSql = @"SELECT
                                            customkind_skuprops.pid,
                                            customkind_skuprops.`name`,
                                            customkind.KindName AS KindNames
                                        FROM
                                            customkind_skuprops,
                                            customkind
                                        WHERE
                                            customkind_skuprops.kindid = customkind.ID
                                        AND customkind_skuprops.IsDelete = 0
                                        AND customkind_skuprops.CoID = customkind.CoID
                                        AND customkind_skuprops.CoID = @CoID
                                        AND customkind.ID=@KindID";
                    string PropValueSql = @"SELECT
                                                pid,
                                                id,
                                                mapping,
                                                NAME
                                            FROM
                                                customkind_skuprops_value
                                            WHERE
                                                CoID =@CoID
                                            AND pid IN @PidLst
                                            AND IsDelete = 0";
                    var SkuProps = conn.Query<skuprops>(SkuPropSql, new { CoID = CoID, KindID = KindID }).AsList();
                    var SkuPropLst = (from p in SkuProps
                                      group p by new { p.pid, p.name } into g
                                      select new skuprops
                                      {
                                          pid = g.Key.pid,
                                          name = g.Key.name
                                      }).AsList();
                    if (SkuPropLst.Count > 0)
                    {
                        var SkuPropValues = conn.Query<skuprops_value>(PropValueSql, new { CoID = CoID, PidLst = SkuPropLst.Select(a => a.pid).AsList() }).AsList();
                        foreach (var prop in SkuPropLst)
                        {
                            prop.skuprops_values = SkuPropValues.Where(a => a.pid == prop.pid).AsList();
                            prop.KindNames = string.Join(",", SkuProps.Where(a => a.pid == prop.pid).Select(a => a.KindNames).AsList().ToArray());
                        }
                    }
                    res.d = SkuPropLst;
                }
                catch (Exception e)
                {
                    res.s = -1;
                    res.d = e.Message;
                }
                return res;
            }
        }
        #endregion
    }
}