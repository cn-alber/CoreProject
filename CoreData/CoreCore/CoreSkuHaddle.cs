using CoreModels;
using Dapper;
using System.Collections.Generic;
// using System.Linq;
using System;
using System.Text;
using CoreModels.XyCore;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Threading.Tasks;

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
            StringBuilder querycount = new StringBuilder();
            var p = new DynamicParameters();            
            querycount.Append("SELECT count(ID) FROM coresku_main where Type = @Type");
            querysql.Append("select GoodsCode,GoodsName,KindName,Enable,Price,ScoGoodsCode from coresku_main where Type = @Type");
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
            if(!string.IsNullOrEmpty(IParam.KindID))
            {
                querycount.Append(" AND KindID = @KindID");
                querysql.Append(" AND KindID = @KindID");
                p.Add("@KindID", IParam.GoodsCode);
            }
            if (!string.IsNullOrEmpty(IParam.GoodsCode))
            {
                querycount.Append(" AND GoodsCode like @GoodsCode");
                querysql.Append(" AND GoodsCode like @GoodsCode");
                p.Add("@GoodsCode", "%" + IParam.GoodsCode + "%");
            }
             if (!string.IsNullOrEmpty(IParam.ScoGoodsCode))
            {                
                querycount.Append(" AND ScoGoodsCode like @ScoGoodsCode");
                querysql.Append(" AND ScoGoodsCode like @ScoGoodsCode");
                p.Add("@ScoGoodsCode", "%" + IParam.ScoGoodsCode + "%");
            }
            if (!string.IsNullOrEmpty(IParam.SkuID))
            {
                querycount.Append(" AND GoodsCode in (SELECT distinct GoodsCode FROM coresku WHERE coresku.CoID= coresku_main.CoID AND coresku.SkuID LIKE @SkuID ))");
                querysql.Append(" AND GoodsCode in (SELECT distinct GoodsCode FROM coresku WHERE coresku.CoID= coresku_main.CoID AND coresku.SkuID LIKE @SkuID ))");
                p.Add("@SkuID", "%" + IParam.SkuID + "%");
            }
            if (!string.IsNullOrEmpty(IParam.SortField) && !string.IsNullOrEmpty(IParam.SortDirection))//排序
            {
                querysql.Append(" ORDER BY " + IParam.SortField + " " + IParam.SortDirection);
                // p.Add("@SortField", IParam.SortField);
                // p.Add("@SortDirection", IParam.SortDirection);
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
                    cs.DataCount =DataCount;
                    decimal pagecnt = Math.Ceiling(decimal.Parse(cs.DataCount.ToString()) / decimal.Parse(IParam.PageSize.ToString()));
                    cs.PageCount = Convert.ToInt32(pagecnt);
                    int dataindex = (IParam.PageIndex - 1) * IParam.PageSize;
                    querysql.Append(" LIMIT @ls , @le");
                    p.Add("@ls", dataindex);
                    p.Add("@le", IParam.PageSize);
                    var GoodsLst = CoreData.DbBase.CoreDB.Query<GoodsQuery>(querysql.ToString(), p).AsList();
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
                }else{
                    Task.Factory.StartNew(()=>{
                        wareployContain("",GoodsLst,CoID,1);
                    });
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
                }else{
                    Task.Factory.StartNew(()=>{
                        wareployContain(Sku,null,CoID,1);
                    });
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
                sku.KindID = ckm.KindID;
                sku.KindName = ckm.KindName;
                sku.SCoList = ckm.SCoList;
                sku.GoodsCode = ckm.GoodsCode;
                sku.GoodsName = ckm.GoodsName;
                // sku.ColorID = cki.ColorID;
                // sku.ColorName = cki.ColorName;
                // sku.SizeID = cki.SizeID;
                sku.ParentID = cki.ParentID;
                // sku.SizeName = cki.SizeName;
                sku.CostPrice = cki.CostPrice;
                sku.SalePrice = cki.SalePrice;
                sku.Norm = cki.ColorName + ";" + cki.SizeName;
                sku.Creator = ckm.Creator;
                sku.CreateDate = DateTime.Now.ToString();
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

        public static DataResult createSku(TmallSku sku){
            var result = new DataResult(1,null); 
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try
                {
                    string sql = "SELECT ID FROM coresku WHERE SkuID = '"+sku.SkuID+"'";
                    var res = conn.Query<long>(sql).AsList();
                    if(res.Count >0){
                        Console.WriteLine("--------UPDATE");
                        sql = @"UPDATE coresku SET                                     
                                    coresku.SkuName =@SkuName,
                                    coresku.GoodsCode = @GoodsCode,
                                    coresku.GoodsName = @GoodsName,
                                    coresku.SalePrice = @SalePrice,
                                    coresku.Norm = @Norm,
                                    coresku.Img = @Img,
                                    coresku.CoID = @CoID,
                                    coresku.`Enable` = @Enable,
                                    coresku.Creator = @Creator,
                                    coresku.CreateDate = NOW(),
                                    coresku.SafeQty = @SafeQty,                                   
                                    coresku.IsParent = @IsParent
                                WHERE 
                                    coresku.SkuID = @SkuID;";
                    }else{
                        sql = @"INSERT coresku SET 
                                    coresku.SkuID = @SkuID,
                                    coresku.SkuName =@SkuName,
                                    coresku.GoodsCode = @GoodsCode,
                                    coresku.GoodsName = @GoodsName,
                                    coresku.SalePrice = @SalePrice,
                                    coresku.Norm = @Norm,
                                    coresku.Img = @Img,
                                    coresku.CoID = @CoID,
                                    coresku.`Enable` = @Enable,
                                    coresku.Creator = @Creator,
                                    coresku.CreateDate = NOW(),
                                    coresku.SafeQty = @SafeQty,
                                    coresku.IsParent = @IsParent;";
                    }

                    
                    var rnt = conn.Execute(sql,sku);
                    if(rnt>0){
                        result.s = 1;
                    }else{
                        result.s = -1;
                        Console.WriteLine("---------"+sku.SkuID);
                    }


                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message; 
                    Console.WriteLine(ex.Message);
                    conn.Dispose();
                }

            }
            return result;
        }


        public static DataResult getWareSku(CoreSkuParam IParam,string coid){
            var result = new DataResult(1,null); 
            var goods =getWareGoodsInner(coid);
            string goodCodes = "'0'";
            foreach(var good in goods){
                goodCodes +=",'"+good.GoodsCode+"'";
            }
            
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try
                {
                    StringBuilder querysql=new StringBuilder();
                    StringBuilder totalSql = new StringBuilder();
                    var p = new DynamicParameters();
                    querysql.Append("SELECT distinct  SkuID,ID, SkuName,Norm FROM coresku WHERE Type=0 AND IsParent = FALSE AND SkuName !='' AND IsDelete = FALSE AND CoID = "+coid+" AND GoodsCode in ("+goodCodes+") ");
                    totalSql.Append("SELECT COUNT(ID) FROM coresku WHERE Type=0 AND IsParent = FALSE AND SkuName !='' AND IsDelete = FALSE  AND CoID = "+coid+"  AND GoodsCode in ("+goodCodes+") ");
                    if (!string.IsNullOrEmpty(IParam.GoodsCode))
                    {
                        querysql.Append(" AND GoodsCode = @GoodsCode");
                        totalSql.Append(" AND GoodsCode = @GoodsCode");
                        p.Add("@GoodsCode", IParam.GoodsCode);
                    }
                    if (!string.IsNullOrEmpty(IParam.GoodsName))
                    {
                        querysql.Append(" AND GoodsName = @GoodsName");
                        totalSql.Append(" AND GoodsName = @GoodsName");
                        p.Add("@GoodsName", IParam.GoodsName);
                    }
                    if (!string.IsNullOrEmpty(IParam.SortField) && !string.IsNullOrEmpty(IParam.SortDirection))//排序
                    {
                        querysql.Append(" ORDER BY " + IParam.SortField + " " + IParam.SortDirection);
                        totalSql.Append(" ORDER BY " + IParam.SortField + " " + IParam.SortDirection);
                    }
                    decimal total = conn.Query<decimal>(totalSql.ToString(),p).AsList()[0];
                    if(total>0){
                        decimal pagecnt = Math.Ceiling(total / decimal.Parse(IParam.PageSize.ToString()));
                        int dataindex = (IParam.PageIndex - 1) * IParam.PageSize;
                        querysql.Append(" LIMIT @ls , @le");
                        p.Add("@ls", dataindex);
                        p.Add("@le", IParam.PageSize);
                        var list = conn.Query<wareSku>(querysql.ToString(),p).AsList();
                        if(IParam.PageIndex == 1){
                            result.d = new {
                                list = list,                                          
                                page = IParam.PageIndex,
                                pageSize = IParam.PageSize,
                                pageTotal = pagecnt,
                                total = total
                            };
                        }else{
                            result.d = new {
                                list = list,                                          
                                page = IParam.PageIndex
                            };
                        }                        
                    }                                                
                }catch{
                    conn.Dispose();
                }
            }
            return result;
        }

        public static DataResult getWareGoods(CoreSkuParam IParam,string coid){
            var result = new DataResult(1,null); 
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try
                {
                    StringBuilder querysql=new StringBuilder();
                    StringBuilder totalSql = new StringBuilder();
                    var p = new DynamicParameters();
                    querysql.Append("SELECT distinct  SkuID,ID, SkuName,Norm,GoodsCode FROM coresku WHERE Type=0 AND IsParent = True  AND IsDelete = FALSE AND CoID = "+coid+" ");
                    totalSql.Append("SELECT COUNT(ID) FROM coresku WHERE Type=0 AND IsParent = TRUE  AND IsDelete = FALSE AND CoID = "+coid+" ");
                    if (!string.IsNullOrEmpty(IParam.GoodsCode))
                    {
                        querysql.Append(" AND GoodsCode = @GoodsCode");
                        totalSql.Append(" AND GoodsCode = @GoodsCode");
                        p.Add("@GoodsCode", IParam.GoodsCode);
                    }
                    if (!string.IsNullOrEmpty(IParam.GoodsName))
                    {
                        querysql.Append(" AND GoodsName = @GoodsName");
                        totalSql.Append(" AND GoodsName = @GoodsName");
                        p.Add("@GoodsName", IParam.GoodsName);
                    }
                    if (!string.IsNullOrEmpty(IParam.SortField) && !string.IsNullOrEmpty(IParam.SortDirection))//排序
                    {
                        querysql.Append(" ORDER BY " + IParam.SortField + " " + IParam.SortDirection);
                        totalSql.Append(" ORDER BY " + IParam.SortField + " " + IParam.SortDirection);
                    }
                    decimal total = conn.Query<decimal>(totalSql.ToString(),p).AsList()[0];
                    if(total>0){
                        decimal pagecnt = Math.Ceiling(total / decimal.Parse(IParam.PageSize.ToString()));
                        int dataindex = (IParam.PageIndex - 1) * IParam.PageSize;
                        querysql.Append(" LIMIT @ls , @le");
                        p.Add("@ls", dataindex);
                        p.Add("@le", IParam.PageSize);
                        var list = conn.Query<wareGoods>(querysql.ToString(),p).AsList();
                        if(IParam.PageIndex == 1){
                            result.d = new {
                                list = list,                                          
                                page = IParam.PageIndex,
                                pageSize = IParam.PageSize,
                                pageTotal = pagecnt,
                                total = total
                            };
                        }else{
                            result.d = new {
                                list = list,                                          
                                page = IParam.PageIndex
                            };
                        }                        
                    }                                                
                }catch{
                    conn.Dispose();
                }
            }
            return result;
        }

        public static List<wareGoods> getWareGoodsInner(string coid){
            var result = new List<wareGoods>(); 
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try
                {
                    StringBuilder querysql=new StringBuilder();
                    StringBuilder totalSql = new StringBuilder();
                    var p = new DynamicParameters();
                    querysql.Append("SELECT distinct  SkuID,SkuName,Norm,GoodsCode FROM coresku WHERE Type=0 AND IsParent = True  AND IsDelete = FALSE AND CoID = "+coid+" ORDER BY SkuID ASC");                                
                    result = conn.Query<wareGoods>(querysql.ToString()).AsList();
                                      
                                                                   
                }catch{
                    conn.Dispose();
                }
            }
            return result;
        }

        public static void wareployContain(string skuid , List<string> goodscode,int coid,int type){    //type: 1 sku  2 good     
            string sql ="";
            var  ids = new List<string>();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try
                {
                    if(type == 1){
                        sql = "SELECT id FROM coresku WHERE SkuID='"+skuid+"' AND CoID = '"+coid+"';";
                    }else{
                        
                        sql = "SELECT id FROM coresku WHERE GoodsCode in ("+string.Join(",",goodscode.ToArray())+") AND CoID = '"+coid+"';";
                    }
                                                                   
                }catch{
                    conn.Dispose();
                }
            }
            if(ids.Count>0){
                int  i= 0;
                var tasks = new Task[10];
                foreach(var id in ids){
                    tasks[i]= Task.Factory.StartNew(()=>{
                        removeContain(id);
                    });
                    i++;
                    if(i == 10){
                        i=0;
                        Task.WaitAll(tasks);
                    }
                }
            }
        }

        public static void removeContain(string id){
            string sql = "";
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    
                    sql = "SELECT ID FROM wareploy WHERE ContainGoods LIKE ',"+id+",' OR ContainSkus LIKE ',"+id+",' OR RemoveGoods LIKE ',"+id+",' OR RemoveSkus LIKE ',"+id+",' ;";                                
                    var ids = conn.Query<int>(sql).AsList();
                    if(ids.Count>0){
                        string a = string.Join(",", ids.ToArray());
                        sql=@"UPDATE wareploy SET	
                                    ContainGoods = REPLACE(ContainGoods, ',"+id+@",', ','),
                                    ContainSkus  = REPLACE(ContainSkus, ',"+id+@",', ','),
                                    RemoveGoods  = REPLACE(RemoveGoods, ',"+id+@",', ','),
                                    RemoveSkus = REPLACE(RemoveSkus, ',"+id+@",', ',')
                                WHERE ID in("+a+")";                         
                    }
                                        
                                                                    
                }catch{
                    conn.Dispose();
                }
            }
        }

        





    }

}