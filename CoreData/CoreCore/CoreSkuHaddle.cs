using CoreModels;
using Dapper;
using System.Collections.Generic;
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
                p.Add("@Enable", IParam.Enable.ToUpper() == "TRUE" ? true : false);
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
                if (GoodsLst.Count < 0)
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
            StringBuilder querycount = new StringBuilder();
            var p = new DynamicParameters();
            querycount.Append("SELECT count(GoodsCode) FROM coresku where Type = @Type");
            querysql.Append("select ID,GoodsCode,GoodsName,SkuID,SkuName,Norm,CostPrice,SalePrice,Enable,Creator,CreateDate from coresku where Type = @Type");
            p.Add("@Type", IParam.Type);
            if (IParam.CoID != 1)
            {
                querycount.Append(" AND CoID = @CoID");
                querysql.Append(" AND CoID = @CoID");
                p.Add("@CoID", IParam.CoID);
            }
            if (!string.IsNullOrEmpty(IParam.Enable) && IParam.Enable.ToUpper() != "ALL")//是否启用
            {
                querycount.Append(" AND Enable = @Enable");
                querysql.Append(" AND Enable = @Enable");
                p.Add("@Enable", IParam.Enable.ToUpper() == "TRUE" ? true : false);
            }
            if (!string.IsNullOrEmpty(IParam.GoodsCode))
            {
                querycount.Append(" AND GoodsCode = @GoodsCode");
                querysql.Append(" AND GoodsCode = @GoodsCode");
                p.Add("@GoodsCode", IParam.GoodsCode);
            }
            if (!string.IsNullOrEmpty(IParam.GoodsName))
            {
                querycount.Append(" AND GoodsName = @GoodsName");
                querysql.Append(" AND GoodsName = @GoodsName");
                p.Add("@GoodsName", IParam.GoodsName);
            }
            if (!string.IsNullOrEmpty(IParam.Filter))
            {
                querycount.Append(" AND (SkuID like @Filter or SkuName like @Filter or Norm like @Filter)");
                querysql.Append(" AND (SkuID like @Filter or SkuName like @Filter or Norm like @Filter)");
                p.Add("@Filter", "'%" + IParam.Filter + "'");
            }
            if (!string.IsNullOrEmpty(IParam.SortField) && !string.IsNullOrEmpty(IParam.SortDirection))//排序
            {
                querycount.Append(" ORDER BY " + IParam.SortField + " " + IParam.SortDirection);
                querysql.Append(" ORDER BY " + IParam.SortField + " " + IParam.SortDirection);
            }
            try
            {
                var DataCount = CoreData.DbBase.CoreDB.QueryFirst<int>(querycount.ToString(), p);
                if (DataCount < 0)
                {
                    res.s = -3001;
                }
                else
                {
                    cs.DataCount = DataCount;
                    decimal pagecnt = Math.Ceiling(decimal.Parse(cs.DataCount.ToString()) / decimal.Parse(IParam.PageSize.ToString()));
                    cs.PageCount = Convert.ToInt32(pagecnt);
                    int dataindex = (IParam.PageIndex - 1) * IParam.PageSize;
                    querysql.Append(" LIMIT @ls , @le");
                    p.Add("@ls", dataindex);
                    p.Add("@le", IParam.PageSize);
                    var SkuLst = CoreData.DbBase.CoreDB.Query<SkuQuery>(querysql.ToString(), p).AsList();
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

        #region 商品管理 - 删除商品 - 修改Delete标记
        public static DataResult DelGoods(List<string> GoodsLst, bool ISDelete, string UserName, int CoID)
        {
            var res = new DataResult(1, null);
            string Deleter = UserName;
            DateTime? DeleteDate = DateTime.Now;
            DateTime? DateNull = null;

            try
            {
                string UptSql = @"UPDATE coresku
                                    SET IsDelete = @ISDelete,
                                    Deleter = @Deleter,
                                    DeleteDate = @DeleteDate
                                    WHERE CoID = @CoID
                                    AND GoodsCode IN @GoodsLst  ";
                var p = new DynamicParameters();
                p.Add("@CoID", CoID);
                p.Add("@IsDelete", ISDelete);
                p.Add("@GoodsLst", GoodsLst);
                if (!ISDelete)
                {
                    Deleter = string.Empty;
                    DeleteDate = DateNull;
                }
                p.Add("@Deleter", Deleter);
                p.Add("@DeleteDate", DeleteDate);

                int count = DbBase.CoreDB.Execute(UptSql, p);
                if (count <= 0)
                {
                    res.s = -3004;
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

        #region 商品管理 - 删除Sku商品 - 修改Delete标记
        public static DataResult DelSku(string Sku, bool ISDelete, string UserName, int CoID)
        {
            var res = new DataResult(1, null);
            string Deleter = UserName;
            DateTime? DeleteDate = DateTime.Now;
            DateTime? DateNull = null;
            try
            {
                string UptSql = @"UPDATE coresku
                                    SET IsDelete = @ISDelete,
                                    Deleter = @Deleter,
                                    DeleteDate = @DeleteDate
                                    WHERE CoID = @CoID
                                    AND SkuID = @Sku  ";
                var p = new DynamicParameters();
                p.Add("@CoID", CoID);
                p.Add("@IsDelete", ISDelete);
                p.Add("@Sku", Sku);
                if (!ISDelete)
                {
                    Deleter = string.Empty;
                    DeleteDate = DateNull;
                }
                p.Add("@Deleter", Deleter);
                p.Add("@DeleteDate", DeleteDate);

                int count = DbBase.CoreDB.Execute(UptSql, p);
                if (count <= 0)
                {
                    res.s = -3004;
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

        #region 清除商品回收站
        public static DataResult DelGoodsRec(List<string> GoodsLst, int CoID)
        {
            var res = new DataResult(1, null);
            try
            {
                string strsql = "delete from coresku where CoID = @CoID and GoodsCode in @GoodsLst and IsDelete=1";
                var p = new DynamicParameters();
                p.Add("@CoID", CoID);
                p.Add("@GoodsLst", GoodsLst);
                int count = DbBase.CoreDB.Execute(strsql, p);
                if (count <= 0)
                {
                    res.s = -3004;
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

        #region 
        public static DataResult NewCore(CoreSkuAuto ckm, CoreSkuItem cki)
        {

            var res = new DataResult(1, null);
            DbBase.CoreDB.Open();
            var Trans = DbBase.CoreDB.BeginTransaction();
            try
            {
                string sql = @"INSERT INTO coresku(SkuID,SkuName,Type,Unit,Weight,
                                       CoID, Brand,  KID, KName,SCoList,
                                       GoodsCode,GoodsName,ColorID,ColorName,SizeID,
                                       ParentID,SizeName,CostPrice,SalePrice,Norm,
                                       Creator,CreateDate
                                       ) 
                                VALUES(@SkuID,@SkuName,@Type,@Unit,@Weight,
                                       @CoID,@Brand,  @KID, @KName,@SCoList,
                                       @GoodsCode,@GoodsName,@ColorID,@ColorName,@SizeID,
                                       @ParentID,@SizeName,@CostPrice,@SalePrice,@Norm,
                                       @Creator,@CreateDate
                                ) ";

                var sku = new Coresku();
                sku.SkuID = cki.SkuID;
                sku.SkuName = ckm.SkuName;
                sku.Type = 0;
                sku.Unit = ckm.Unit;
                sku.Weight = ckm.Weight;
                sku.CoID = ckm.CoID;
                sku.Brand = ckm.Brand;
                sku.KID = ckm.KID;
                sku.KName = ckm.KName;
                sku.SCoList = ckm.SCoList;
                sku.GoodsCode = ckm.GoodsCode;
                sku.GoodsName = ckm.GoodsName;
                sku.ColorID = cki.ColorID;
                sku.ColorName = cki.ColorName;
                sku.SizeID = cki.SizeID;
                sku.ParentID = cki.ParentID;
                sku.SizeName = cki.SizeName;
                sku.CostPrice = cki.CostPrice;
                sku.SalePrice = cki.SalePrice;
                sku.Norm = cki.ColorName + ";" + cki.SizeName;
                sku.Creator = ckm.Creator;
                sku.CreateDate = DateTime.Now;
                int count = DbBase.UserDB.Execute(sql, sku, Trans);
                if (count == 0)
                {
                    res.s = -3002;
                }
                Trans.Commit();
            }
            catch (Exception e)
            {
                res.s = -1;
                res.d = e.Message;
            }
            finally
            {
                Trans.Dispose();
                DbBase.CoreDB.Close();
            }

            return res;
        }
        #endregion


        #region 根据条件抓取商品list(采购用)
        public static DataResult GetSkuAll(SkuParam cp, int CoID, int Type)
        {
            var result = new DataResult(1, null);
            StringBuilder querysql = new StringBuilder();
            var sql = @"select  SkuID,SkuName,
                                GoodsCode,SalePrice,
                                ColorName,SizeName 
                        from coresku
                        where CoID = @CoID AND Type = @Type AND Enable = 1 ";
            querysql.Append(sql);
            var p = new DynamicParameters();
            p.Add("@CoID", CoID);
            p.Add("@Type", Type);

            if (!string.IsNullOrEmpty(cp.GoodsCode))
            {
                querysql.Append(" AND GoodsCode like @GoodsCode");
                p.Add("@GoodsCode", "%" + cp.GoodsCode + "%");
            }
            if (!string.IsNullOrEmpty(cp.SkuID))
            {
                querysql.Append(" AND SkuID like @SkuID");
                p.Add("@SkuID", "%" + cp.SkuID + "%");
            }
            if (!string.IsNullOrEmpty(cp.SkuName))
            {
                querysql.Append(" AND SkuName like @SkuName");
                p.Add("@SkuName", "%" + cp.SkuName + "%");
            }
            try
            {
                var Lst = DbBase.CoreDB.Query<SkuQuery>(querysql.ToString(), p).AsList();
                if (Lst.Count < 0)
                {
                    result.s = -3001;
                }
                else
                {
                    result.d = Lst;
                }
            }
            catch (Exception e)
            {
                result.s = -1;
                result.d = e.Message;
            }

            return result;
        }
        #endregion


        #region 商品资料管理-查询SKU明细-通用查询
        public static DataResult GetCommSkuLst(CommSkuParam IParam)
        {
            var cs = new CoreSkuQuery();
            var res = new DataResult(1, null);
            StringBuilder querysql = new StringBuilder();
            StringBuilder querycount = new StringBuilder();
            var p = new DynamicParameters();
            querycount.Append("SELECT count(GoodsCode) FROM coresku where 1=1");
            querysql.Append("select ID,GoodsCode,GoodsName,SkuID,SkuName,Norm,GBCode,Brand,CostPrice,SalePrice,Enable,Img,Creator,CreateDate from coresku where 1=1");
            if (!string.IsNullOrEmpty(IParam.Type))
            {
                querycount.Append(" AND Type = @Type");
                querysql.Append(" AND Type = @Type");
                p.Add("@Type", IParam.Type);
            }
            if (IParam.CoID != 1)
            {
                querycount.Append(" AND CoID = @CoID");
                querysql.Append(" AND CoID = @CoID");
                p.Add("@CoID", IParam.CoID);
            }
            if (!string.IsNullOrEmpty(IParam.Enable) && IParam.Enable.ToUpper() != "ALL")//是否启用
            {
                querycount.Append(" AND Enable = @Enable");
                querysql.Append(" AND Enable = @Enable");
                p.Add("@Enable", IParam.Enable.ToUpper() == "TRUE" ? true : false);
            }
            if (!string.IsNullOrEmpty(IParam.GoodsCode))
            {
                querycount.Append(" AND GoodsCode = @GoodsCode");
                querysql.Append(" AND GoodsCode = @GoodsCode");
                p.Add("@GoodsCode", IParam.GoodsCode);
            }
            if (!string.IsNullOrEmpty(IParam.SkuID))
            {
                querycount.Append(" AND SkuID = @SkuID");
                querysql.Append(" AND SkuID = @SkuID");
                p.Add("@SkuID", IParam.SkuID);
            }
            if (!string.IsNullOrEmpty(IParam.Brand))
            {
                querycount.Append(" AND Brand = @Brand");
                querysql.Append(" AND Brand = @Brand");
                p.Add("@Brand", IParam.Brand);
            }
            if (!string.IsNullOrEmpty(IParam.SCoID))
            {
                querycount.Append(" AND CONCAT(',',IFNULL(SCoList,''),',') LIKE @SCoID");
                querysql.Append(" AND CONCAT(',',IFNULL(SCoList,''),',') LIKE @SCoID");
                p.Add("@SCoID", "%," + IParam.SCoID + ",%");
            }
            if (!string.IsNullOrEmpty(IParam.Filter))
            {
                querycount.Append(" AND (GoodsName like @Filter or SkuName like @Filter or Norm like @Filter)");
                querysql.Append(" AND (GoodsName like @Filter or SkuName like @Filter or Norm like @Filter)");
                p.Add("@Filter", "%" + IParam.Filter + "%");
            }
            if (!string.IsNullOrEmpty(IParam.SortField) && !string.IsNullOrEmpty(IParam.SortDirection))//排序
            {
                querycount.Append(" ORDER BY " + IParam.SortField + " " + IParam.SortDirection);
                querysql.Append(" ORDER BY " + IParam.SortField + " " + IParam.SortDirection);
            }
            try
            {
                var DataCount = CoreData.DbBase.CoreDB.QueryFirst<int>(querycount.ToString(), p);
                if (DataCount < 0)
                {
                    res.s = -3001;
                }
                else
                {
                    cs.DataCount = DataCount;
                    decimal pagecnt = Math.Ceiling(decimal.Parse(cs.DataCount.ToString()) / decimal.Parse(IParam.PageSize.ToString()));
                    cs.PageCount = Convert.ToInt32(pagecnt);
                    int dataindex = (IParam.PageIndex - 1) * IParam.PageSize;
                    querysql.Append(" LIMIT @ls , @le");
                    p.Add("@ls", dataindex);
                    p.Add("@le", IParam.PageSize);
                    var SkuLst = CoreData.DbBase.CoreDB.Query<SkuQuery>(querysql.ToString(), p).AsList();
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
    }

}