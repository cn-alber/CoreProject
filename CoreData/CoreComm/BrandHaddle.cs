using System;
using System.Text;
using System.Collections.Generic;
// using System.Linq;
using CoreModels;
using CoreModels.XyComm;
using Dapper;
using MySql.Data.MySqlClient;
namespace CoreData.CoreComm
{
    public static class BrandHaddle
    {
        #region 品牌 - 资料查询
        public static DataResult GetBrandLst(BrandParam IParam)
        {
            var res = new DataResult(1, null);
            var info = new BrandData();
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {
                    StringBuilder querysql = new StringBuilder();
                    var p = new DynamicParameters();
                    string sql = @"SELECT
                                        brand.ID,
                                        brand.`Name`,
                                        brand.Intro,
                                        brand.Link,
                                        brand.`Enable`,
                                        brand.CoID,
                                        brand.Creator,
                                        brand.CreateDate
                                    FROM
                                        brand
                                    WHERE CoID = @CoID";
                    querysql.Append(sql);
                    p.Add("@CoID", IParam.CoID);
                    if (!string.IsNullOrEmpty(IParam.Enable) && IParam.Enable.ToUpper() != "ALL")//是否启用
                    {
                        querysql.Append(" AND Enable = @Enable");
                        p.Add("@Enable", IParam.Enable.ToUpper() == "TRUE" ? true : false);
                    }
                    if (!string.IsNullOrEmpty(IParam.Filter))
                    {
                        querysql.Append(" AND (Name like @Filter)");
                        p.Add("@Filter", "%" + IParam.Filter + "%");
                    }
                    if (!string.IsNullOrEmpty(IParam.SortField) && !string.IsNullOrEmpty(IParam.SortDirection))//排序
                    {
                        querysql.Append(" ORDER BY " + IParam.SortField + " " + IParam.SortDirection);
                    }
                    var BrandLst = DbBase.CommDB.Query<Brand>(querysql.ToString(), p).AsList();
                    if (BrandLst.Count == 0)
                    {
                        res.s = -3001;
                    }
                    else
                    {
                        info.DataCount = BrandLst.Count;
                        decimal pagecnt = Math.Ceiling(decimal.Parse(info.DataCount.ToString()) / decimal.Parse(IParam.PageSize.ToString()));
                        info.PageCount = Convert.ToInt32(pagecnt);
                        int dataindex = (IParam.PageIndex - 1) * IParam.PageSize;
                        querysql.Append(" LIMIT @ls , @le");
                        p.Add("@ls", dataindex);
                        p.Add("@le", IParam.PageSize);
                        BrandLst = CoreData.DbBase.CommDB.Query<Brand>(querysql.ToString(), p).AsList();
                        info.BrandLst = BrandLst;
                        res.d = info;
                    }
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
            return res;
        }
        #endregion

        #region 品牌 - 编辑|查询
        public static DataResult GetBrandEdit(string ID)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {
                    string querysql = @"SELECT
                                            brand.ID,
                                            brand.`Name`,
                                            brand.Intro,
                                            brand.Link,
                                            brand.`Enable`,
                                            brand.CoID,
                                            brand.Creator,
                                            brand.CreateDate
                                        FROM
                                            brand
                                        WHERE ID = @ID ";
                    var p = new { ID = ID };
                    var info = DbBase.CommDB.QueryFirst<Brand>(querysql, p);
                    if (info == null)
                    {
                        res.s = -3001;
                    }
                    res.d = info;
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
            return res;
        }
        #endregion

        #region 新增品牌
        #endregion

        #region 修改品牌
        #endregion

        #region 删除品牌
        #endregion

        #region 获取品牌列表
        public static DataResult GetBrandALL(int CoID)
        {
             var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {
                    string querysql = @"SELECT
                                            brand.ID,
                                            brand.`Name`,
                                            brand.Intro,
                                            brand.Link,
                                            brand.`Enable`,
                                            brand.CoID,
                                            brand.Creator,
                                            brand.CreateDate
                                        FROM
                                            brand
                                        WHERE CoID = @CoID
                                        ORDER BY brand.`Name` ";
                    var p = new { CoID = CoID };
                    var info = DbBase.CommDB.Query<Brand>(querysql, p).AsList();
                    if (info == null)
                    {
                        res.s = -3001;
                    }
                    res.d = info;
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
            return res;
        }
        #endregion
    }
}