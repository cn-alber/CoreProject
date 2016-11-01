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

            // string sql = @"SELECT id,sizeid,name FROM coresize WHERE CoID=@CoID AND kindid = @KindID AND IsDelete=0";
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
                                            AND pid IN @PidLst";
                    var SkuProps = conn.Query<skuprops>(SkuPropSql, new { CoID = CoID }).AsList();
                    var SkuPropLst = (from p in SkuProps
                                      group p by new { p.pid, p.name } into g
                                      select new skuprops
                                      {
                                          pid = g.Key.pid,
                                          name = g.Key.name
                                      }).AsList();
                    //SkuProps.GroupBy(s=>).Select(a=>new skuprops{pid=a.pid,name=a.name}).AsList();
                    if (SkuPropLst.Count > 0)
                    {
                        var SkuPropValues = conn.Query<skuprops_value>(PropValueSql, new { CoID = CoID, PidLst = SkuPropLst.Select(a => a.pid).AsList() }).AsList();
                        foreach(var prop in SkuPropLst)
                        {
                            prop.skuprops_values = SkuPropValues.Where(a=>a.pid==prop.pid).AsList();
                            prop.KindNames = string.Join(",",SkuProps.Where(a=>a.pid==prop.pid).Select(a=>a.KindNames).AsList().ToArray());
                        }
                    }
                    res.d = SkuPropLst;
                    //         var SizeLst = conn.Query<SizeData>(sql, new { KindID = KindID, CoID = CoID }).AsList();
                    //         res.d = SizeLst;
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
        // public static DataResult DelSkuProps
        #endregion
    }
}