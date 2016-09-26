using CoreModels;
using Dapper;
// using System.Collections.Generic;
// using System.Linq;
using System;
using System.Text;
using CoreModels.XyCore;

namespace CoreData.CoreCore
{
    public static class CoreSkuHaddle
    {
        #region 商品资料管理-查询货品资料
        public static DataResult GetGoodsLst(CoreSkuParam IParam)
        {
            var cs = new CoreSkuQuery();
            var res = new DataResult(1, null);
            StringBuilder querysql = new StringBuilder();
            var p = new DynamicParameters();
            querysql.Append("select distinct GoodsCode,GoodsName,KName,Brand,Type from coresku where Type = @Type");

            p.Add("@Type", IParam.Type);
            if (IParam.CoID != 1)
            {
                querysql.Append(" AND CoID = @CoID");
                p.Add("@CoID", IParam.CoID);
            }
            if (!string.IsNullOrEmpty(IParam.Enable) && IParam.Enable.ToUpper() != "ALL")//是否启用
            {
                querysql.Append(" AND Enable = @Enable");
                p.Add("@Enable", IParam.Enable == "true" ? true : false);
            }
            if (!string.IsNullOrEmpty(IParam.GoodsCode))
            {
                querysql.Append(" AND GoodsCode = @GoodsCode");
                p.Add("@GoodsCode", IParam.GoodsCode);
            }
            if (!string.IsNullOrEmpty(IParam.GoodsName))
            {
                querysql.Append(" AND GoodsName = @GoodsName");
                p.Add("@GoodsName", IParam.GoodsName);
            }
            if (!string.IsNullOrEmpty(IParam.SortField) && !string.IsNullOrEmpty(IParam.SortDirection))//排序
            {
                querysql.Append(" ORDER BY " + IParam.SortField + " " + IParam.SortDirection);
                // p.Add("@SortField", IParam.SortField);
                // p.Add("@SortDirection", IParam.SortDirection);
            }
            try
            {
                var GoodsLst = CoreData.DbBase.CoreDB.Query<GoodsQuery>(querysql.ToString(), p).AsList();
                if (GoodsLst.Count == 0)
                {
                    res.s = -3001;
                }
                else
                {
                    cs.DataCount = GoodsLst.Count;
                    decimal pagecnt = Math.Ceiling(decimal.Parse(cs.DataCount.ToString()) / decimal.Parse(IParam.PageSize.ToString()));
                    cs.PageCount = Convert.ToInt32(pagecnt);
                    int dataindex = (IParam.PageIndex - 1) * IParam.PageSize;
                    querysql.Append(" LIMIT @ls , @le");
                    p.Add("@ls", dataindex);
                    p.Add("@le", IParam.PageSize);
                    GoodsLst = CoreData.DbBase.CoreDB.Query<GoodsQuery>(querysql.ToString(), p).AsList();
                    cs.GoodsLst = GoodsLst;
                    res.d = cs;
                }
            }
            catch (Exception e)
            {
                res.s = -1;
                res.d = e.Message;
            }


            return res;
        }
        #endregion

        #region 商品资料管理-查询SKU明细
        public static DataResult GetSkuLst(CoreSkuParam IParam)
        {
            var cs = new CoreSkuQuery();
            var res = new DataResult(1, null);
            StringBuilder querysql = new StringBuilder();
            var p = new DynamicParameters();
            querysql.Append("select GoodsCode,GoodsName,SkuID,SkuName,Norm,CostPrice,SalePrice,Enable,Creator,CreateDate from coresku where Type = @Type");
            p.Add("@Type", IParam.Type);
            if (IParam.CoID != 1)
            {
                querysql.Append(" AND CoID = @CoID");
                p.Add("@CoID", IParam.CoID);
            }
            if (!string.IsNullOrEmpty(IParam.Enable) && IParam.Enable.ToUpper() != "ALL")//是否启用
            {
                querysql.Append(" AND Enable = @Enable");
                p.Add("@Enable", IParam.Enable == "true" ? true : false);
            }
            if (!string.IsNullOrEmpty(IParam.GoodsCode))
            {
                querysql.Append(" AND GoodsCode = @GoodsCode");
                p.Add("@GoodsCode", IParam.GoodsCode);
            }
            if (!string.IsNullOrEmpty(IParam.GoodsName))
            {
                querysql.Append(" AND GoodsName = @GoodsName");
                p.Add("@GoodsName", IParam.GoodsName);
            }
            if (!string.IsNullOrEmpty(IParam.Filter))
            {
                querysql.Append(" AND (SkuID like @Filter or SkuName like @Filter or Norm like @Filter)");
                p.Add("@Filter", "'%" + IParam.Filter + "'");
            }
            if (!string.IsNullOrEmpty(IParam.SortField) && !string.IsNullOrEmpty(IParam.SortDirection))//排序
            {
                querysql.Append(" ORDER BY " + IParam.SortField + " " + IParam.SortDirection);
                // p.Add("@SortField", IParam.SortField);
                // p.Add("@SortDirection", IParam.SortDirection);
            }
            try
            {
                var SkuLst = CoreData.DbBase.CoreDB.Query<SkuQuery>(querysql.ToString(), p).AsList();
                if (SkuLst.Count == 0)
                {
                    res.s = -3001;
                }
                else
                {
                    cs.DataCount = SkuLst.Count;
                    decimal pagecnt = Math.Ceiling(decimal.Parse(cs.DataCount.ToString()) / decimal.Parse(IParam.PageSize.ToString()));
                    cs.PageCount = Convert.ToInt32(pagecnt);
                    int dataindex = (IParam.PageIndex - 1) * IParam.PageSize;
                    querysql.Append(" LIMIT @ls , @le");
                    p.Add("@ls", dataindex);
                    p.Add("@le", IParam.PageSize);
                    SkuLst = CoreData.DbBase.CoreDB.Query<SkuQuery>(querysql.ToString(), p).AsList();
                    cs.SkuLst = SkuLst;
                    res.d = cs;
                }
            }
            catch (Exception e)
            {
                res.s = -1;
                res.d = e.Message;
            }


            return res;
        }
        #endregion
        #region 商品管理 - 获取单笔Sku详情
        public static DataResult GetCoreSkuEdit(string GoodsCode, int CoID)
        {
            var cs = new CoreSkuAuto();
            var res = new DataResult(1, null);
            var p = new DynamicParameters();
            string msql = @"select distinct GoodsCode,GoodsName,Brand,KID,KName,CoID,
                            Unit,Weight,Creator,SCoList,SkuName      
                         from coresku where GoodsCode=@GoodsCode and CoID = @CoID
                         LIMIT 1";
            p.Add("@GoodsCode", GoodsCode);
            p.Add("@CoID", CoID);
            try
            {
                var main = DbBase.CoreDB.QueryFirst<CoreSkuAuto>(msql, p);
                if (main == null)
                {
                    res.s = -3001;
                }
                else
                {
                    string dsql = @"select GoodsCode,SkuID,SkuName,CostPrice,SalePrice,
                                    ColorID,ColorName,SizeID,SizeName,ParentID,Remark
                         from coresku where GoodsCode=@GoodsCode and CoID = @CoID";
                    var item = DbBase.CoreDB.Query<CoreSkuItem>(dsql, p).AsList();
                    main.items = item;
                    res.d = main;
                }

            }
            catch (Exception e)
            {
                res.s = -1;
                res.d = e.Message;
            }

            return res;

        }
        #endregion
    }

}