using CoreModels;
using Dapper;
using System.Collections.Generic;
// using System.Linq;
using System;
using System.Text;
using System.Data;
using CoreModels.XyCore;
using CoreModels.XyComm;
using CoreData.CoreComm;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CoreData.CoreCore
{
    public static class CoreSkuHaddle
    {
        #region 商品资料管理-查询货品资料-商品维护查询
        public static DataResult GetGoodsLst(CoreSkuParam IParam)
        {
            var cs = new CoreSkuQuery();
            var res = new DataResult(1, null);
            StringBuilder querysql = new StringBuilder();
            StringBuilder querycount = new StringBuilder();
            var p = new DynamicParameters();
            querycount.Append("SELECT count(ID) FROM coresku_main where Type = @Type");
            querysql.Append("select GoodsCode,GoodsName,KindID,KindName,Enable,SalePrice,ScoGoodsCode from coresku_main where Type = @Type");
            p.Add("@Type", IParam.Type);
            if (IParam.CoID != 1)
            {
                querycount.Append(" AND CoID = @CoID");
                querysql.Append(" AND CoID = @CoID");
                p.Add("@CoID", IParam.CoID);
            }
            if (!string.IsNullOrEmpty(IParam.Enable))//是否启用
            {
                querycount.Append(" AND Enable = @Enable");
                querysql.Append(" AND Enable = @Enable");
                p.Add("@Enable", IParam.Enable);
            }
            if (!string.IsNullOrEmpty(IParam.KindID))
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
            var CommConn = new MySqlConnection(DbBase.CommConnectString);
            var CoreConn = new MySqlConnection(DbBase.CoreConnectString);
            try
            {
                var DataCount = CoreConn.QueryFirst<int>(querycount.ToString(), p);
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
                    var GoodsLst = CoreConn.Query<GoodsQuery>(querysql.ToString(), p).AsList();
                    var KindIDLst = GoodsLst.Select(a => a.KindID).AsList();
                    string kindsql = "SELECT ID AS KindID,KindName FROM customkind WHERE ID in @KindIDLst AND CoID=@CoID";
                    var KindLst = CommConn.Query<CoreKind>(kindsql, new { KindIDLst = KindIDLst, CoID = IParam.CoID }).AsList();
                    foreach (var goods in GoodsLst)
                    {
                        goods.KindName = KindLst.Where(a => a.KindID == goods.KindID).Select(a => a.KindName).First();
                    }
                    cs.GoodsLst = GoodsLst;
                    res.d = cs;
                }
            }
            catch (Exception e)
            {
                res.s = -1;
                res.d = e.Message;
            }
            finally
            {
                CommConn.Dispose();
                CoreConn.Dispose();
                CommConn.Close();
                CoreConn.Close();
            }
            return res;
        }
        #endregion

        #region 商品资料管理-查询SKU明细-普通商品查询
        public static DataResult GetSkuLst(CoreSkuParam IParam)
        {
            var cs = new CoreSkuQuery();
            var res = new DataResult(1, null);
            StringBuilder querysql = new StringBuilder();
            StringBuilder querycount = new StringBuilder();
            var p = new DynamicParameters();
            string sql = @"SELECT  ID,
                            GoodsCode,
                            SkuID,
                            SkuName,
                            SkuSimple,
                            GBCode,
                            Norm,
                            IFNULL(Brand,0) AS Brand,
                            PurPrice,
                            MarketPrice,
                            SalePrice,
                            Weight,
                            `Enable`,
                            ScoGoodsCode,
                            ScoSku,
                            IFNULL(SCoID,0) AS SCoID,
                            Creator,
                            CreateDate,
                            Modifier,
                            ModifyDate
                        FROM
                            coresku
                        WHERE 1=1";
            querycount.Append("SELECT count(ID) FROM coresku where 1 = 1");
            querysql.Append(sql);
            if (IParam.CoID != 1)
            {
                querycount.Append(" AND CoID = @CoID");
                querysql.Append(" AND CoID = @CoID");
                p.Add("@CoID", IParam.CoID);
            }
            if (!string.IsNullOrEmpty(IParam.Enable))//是否启用
            {
                querycount.Append(" AND Enable = @Enable");
                querysql.Append(" AND Enable = @Enable");
                p.Add("@Enable", IParam.Enable);
            }
            if (!string.IsNullOrEmpty(IParam.Filter))
            {

                querycount.Append(" AND (SkuID like @Filter or GoodsCode like @Filter)");
                querysql.Append(" AND (SkuID like @Filter or GoodsCode like @Filter)");
                p.Add("@Filter", "%" + IParam.Filter + "%");
            }
            if (!string.IsNullOrEmpty(IParam.SkuName))
            {
                querycount.Append(" AND SkuName LIKE @SkuName");
                querysql.Append(" AND SkuName LIKE @SkuName");
                p.Add("@SkuName", "%" + IParam.SkuName + "%");
            }
            if (!string.IsNullOrEmpty(IParam.Brand))
            {
                querycount.Append(" AND Brand LIKE @Brand");
                querysql.Append(" AND Brand LIKE @Brand");
                p.Add("@Brand", "%" + IParam.Brand + "%");
            }
            if (!string.IsNullOrEmpty(IParam.SCoID))
            {
                querycount.Append(" AND SCoID = @SCoID");
                querysql.Append(" AND SCoID = @SCoID");
                p.Add("@SCoID", IParam.SCoID);
            }
            if (!string.IsNullOrEmpty(IParam.ScoGoodsCode))
            {
                querycount.Append(" AND ScoGoodsCode LIKE @ScoGoodsCode");
                querysql.Append(" AND ScoGoodsCode LIKE @ScoGoodsCode");
                p.Add("@ScoGoodsCode", "%" + IParam.ScoGoodsCode + "%");
            }
            if (!string.IsNullOrEmpty(IParam.ScoSku))
            {
                querycount.Append(" AND ScoSku LIKE @ScoSku");
                querysql.Append(" AND ScoSku LIKE @ScoSku");
                p.Add("@ScoSku", "%" + IParam.ScoSku + "%");
            }

            if (!string.IsNullOrEmpty(IParam.SkuSimple))
            {
                querycount.Append(" AND SkuSimple LIKE @SkuSimple");
                querysql.Append(" AND SkuSimple LIKE @SkuSimple");
                p.Add("@SkuSimple", "%" + IParam.SkuSimple + "%");
            }
            if (!string.IsNullOrEmpty(IParam.Norm))
            {
                querycount.Append(" AND Norm LIKE @Norm");
                querysql.Append(" AND Norm LIKE @Norm");
                p.Add("@Norm", "%" + IParam.Norm + "%");
            }
            if (!string.IsNullOrEmpty(IParam.PriceS))
            {
                querycount.Append(" AND IFNULL(PurPrice,0) >= @PriceS");
                querysql.Append(" AND IFNULL(PurPrice,0) >= @PriceS");
                p.Add("@PriceS", IParam.PriceS);
            }
            if (!string.IsNullOrEmpty(IParam.PriceT))
            {
                querycount.Append(" AND IFNULL(PurPrice,0) <= @PriceT");
                querysql.Append(" AND IFNULL(PurPrice,0) <= @PriceT");
                p.Add("@PriceT", IParam.PriceT);
            }
            if (!string.IsNullOrEmpty(IParam.SortField) && !string.IsNullOrEmpty(IParam.SortDirection))//排序
            {
                querysql.Append(" ORDER BY " + IParam.SortField + " " + IParam.SortDirection);
            }
            var CoreConn = new MySqlConnection(DbBase.CoreConnectString);
            var CommConn = new MySqlConnection(DbBase.CommConnectString);
            try
            {
                var DataCount = CoreConn.QueryFirst<int>(querycount.ToString(), p);
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
                    var SkuLst = CoreConn.Query<SkuQuery>(querysql.ToString(), p).AsList();
                    cs.SkuLst = SkuLst;
                    cs.BrandLst = new List<BrandDDLB>();
                    cs.ScoLst = new List<ScoCompDDLB>();
                    var BrandIDLst = SkuLst.Where(a => a.Brand != null).Select(a => a.Brand).Distinct().AsList();
                    var SCoIDLst = SkuLst.Where(a => a.SCoID != null).Select(a => a.SCoID).Distinct().AsList();
                    if (BrandIDLst.Count > 0)
                    {
                        var BrandLst = CommConn.Query<BrandDDLB>("SELECT ID,Name,Intro FROM Brand WHERE CoID=@CoID AND ID IN @IDLST", new { CoID = IParam.CoID, IDLST = BrandIDLst }).AsList();
                        cs.BrandLst = BrandLst;
                    }
                    if (SCoIDLst.Count > 0)
                    {
                        var SCoLst = CoreConn.Query<ScoCompDDLB>("SELECT id,scocode,scosimple FROM supplycompany WHERE CoID=@CoID AND id IN @IDLST", new { CoID = IParam.CoID, IDLST = SCoIDLst }).AsList();
                        cs.ScoLst = SCoLst;
                    }
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
        #region 商品管理 - 获取单笔Sku详情 - 编辑商品维护
        public static DataResult GetCoreSkuEdit(string ID, string CoID)
        {
            var cs = new CoreSkuAuto();
            var result = new DataResult(1, null);
            string msql = @"SELECT * FROM coresku_main WHERE ID=@ID AND CoID = @CoID";
            string itempropsql = @"SELECT * FROM coresku_item_props WHERE ParentID=@ID AND CoID = @CoID AND ISDelete = 0";
            string skupropsql = @"SELECT * FROM coresku_sku_props WHERE ParentID=@ID AND CoID = @CoID AND ISDelete = 0";
            string itemsql = @"SELECT * FROM coresku ParentID=@ID AND CoID = @CoID AND IsDelete=0";
            var p = new DynamicParameters();
            p.Add("@ID", ID);
            p.Add("@CoID", CoID);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    var main = conn.QueryFirst<Coresku_main>(msql, p);
                    if (main == null)
                    {
                        result.s = -3001;
                    }
                    else
                    {
                        var KindID = main.KindID;
                        var res = CustomKindPropsHaddle.GetItemPropsByKind(KindID, CoID);
                        if (res.s == 1)
                        {
                            cs.itemprops_base = res.d as List<itemprops>;
                        }
                        res = SkuPropsHaddle.GetSkuPropsByKind(KindID, CoID);
                        if (res.s == 1)
                        {
                            cs.skuprops_base = res.d as List<skuprops>;
                        }
                        cs.items = conn.Query<CoreSkuItem>(itemsql, p).AsList();
                        cs.itemprops = conn.Query<goods_item_props>(itempropsql, p).AsList();
                        cs.skuprops = conn.Query<goods_sku_props>(skupropsql, p).AsList();
                        result.d = cs;
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

        #region 商品管理 - 删除商品 - 修改Delete标记
        public static DataResult DelGoods(List<int> IDLst, string UserName, int CoID)
        {
            var res = new DataResult(1, null);
            var conn = new MySqlConnection(DbBase.CoreConnectString);
            conn.Open();
            var Trans = conn.BeginTransaction();
            try
            {
                string UptMainSql = @"UPDATE coresku_main SET IsDelete=1,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE CoID = @CoID AND ID IN @IDLst AND IsDelete=0";
                string UptSql = @"UPDATE coresku SET IsDelete=1,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE CoID = @CoID AND ParentID IN @IDLst AND IsDelete=0";
                string UptSkuSql = @"UPDATE coresku_sku_props SET Enable=0,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE ParentID IN @IDLst AND IsDelete=0 AND Enable=1";
                var p = new DynamicParameters();
                p.Add("@CoID", CoID);
                p.Add("@IDLst", IDLst);
                p.Add("@Modifier", UserName);
                p.Add("@ModifyDate", DateTime.Now.ToString());
                int count1 = conn.Execute(UptMainSql, p, Trans);
                int count2 = conn.Execute(UptSql, p, Trans);
                int count3 = conn.Execute(UptSkuSql, p, Trans);
                if (count1 < 0 || count2 < 0 || count3 < 0)
                {
                    res.s = -3004;
                }
                else
                {
                    Task.Factory.StartNew(() =>
                    {
                        // wareployContain("", GoodsLst, CoID, 1);
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
        public static DataResult DelSku(List<int> IDLst, string UserName, int CoID)
        {
            var res = new DataResult(1, null);
            var conn = new MySqlConnection(DbBase.CoreConnectString);
            conn.Open();
            var Trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
            try
            {
                string UptSql = @"UPDATE coresku SET IsDelete=1,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE CoID = @CoID AND ID IN @IDLst AND IsDelete=0";
                var p = new DynamicParameters();
                p.Add("@CoID", CoID);
                p.Add("@IDLst", IDLst);
                p.Add("@Modifier", UserName);
                p.Add("@ModifyDate", DateTime.Now.ToString());
                int count = conn.Execute(UptSql, p, Trans);
                if (count < 0)
                {
                    res.s = -3004;
                }
                else
                {
                    #region 获取现有Sku明细属性，更新商品Sku属性表
                    string goodSql = @"SELECT Distinct ParentID FROM coresku WHERE ID IN @IDLst AND CoID = @CoID";
                    string PropSql = @"SELECT ParentID,pid1,val_id1,pid2,val_id2,pid3,val_id3 FROM coresku WHERE CoID=@CoID AND IsDelete=0 AND ParentID IN @ParentIDLst";
                    var ParentIDLst = conn.Query<string>(goodSql, new { CoID = CoID, IDLst = IDLst }, Trans).AsList();
                    var DSkuProps = conn.Query<DetailSkuProps>(PropSql, new { CoID = CoID, ParentIDLst = ParentIDLst }, Trans).AsList();
                    var SkuPropsValLst = new List<string>();
                    if (DSkuProps.Count > 0)
                    {
                        foreach (var d in DSkuProps)
                        {
                            if (!SkuPropsValLst.Contains(d.val_id1))
                                SkuPropsValLst.Add(d.val_id1);
                            if (!SkuPropsValLst.Contains(d.val_id2))
                                SkuPropsValLst.Add(d.val_id2);
                            if (!SkuPropsValLst.Contains(d.val_id3))
                                SkuPropsValLst.Add(d.val_id3);
                        }
                    }
                    string UptSkuPropSql = "UPDATE coresku_sku_props SET Enable=0,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE CoID=@CoID AND ParentID IN @ParentIDLst AND Enable = 1";                    
                    var p1 = new DynamicParameters();
                    p1.Add("@CoID", CoID);
                    p1.Add("@ParentIDLst", ParentIDLst);
                    p1.Add("@Modifier", UserName);
                    p1.Add("@ModifyDate", DateTime.Now.ToString());
                    if (SkuPropsValLst.Count > 0)
                    {
                        UptSkuPropSql = UptSkuPropSql + " AND ID NOT IN @SkuPropsValLst";
                        p1.Add("@SkuPropsValLst",SkuPropsValLst);
                    }
                    conn.Execute(UptSkuPropSql,p1,Trans);   

                    #endregion

                    Trans.Commit();

                    Task.Factory.StartNew(() =>
                    {
                        // wareployContain(Sku, null, CoID, 1);
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

        #region 清除商品回收站--停用
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
        // public static DataResult NewCore(CoreSkuAuto ckm, CoreSkuItem cki)
        // {

        //     var res = new DataResult(1, null);
        //     DbBase.CoreDB.Open();
        //     var Trans = DbBase.CoreDB.BeginTransaction();
        //     try
        //     {
        //         string sql = @"INSERT INTO coresku(SkuID,SkuName,Type,Unit,Weight,
        //                                CoID, Brand,  KID, KName,SCoList,
        //                                GoodsCode,GoodsName,ColorID,ColorName,SizeID,
        //                                ParentID,SizeName,CostPrice,SalePrice,Norm,
        //                                Creator,CreateDate
        //                                ) 
        //                         VALUES(@SkuID,@SkuName,@Type,@Unit,@Weight,
        //                                @CoID,@Brand,  @KID, @KName,@SCoList,
        //                                @GoodsCode,@GoodsName,@ColorID,@ColorName,@SizeID,
        //                                @ParentID,@SizeName,@CostPrice,@SalePrice,@Norm,
        //                                @Creator,@CreateDate
        //                         ) ";

        //         var sku = new Coresku();
        //         sku.SkuID = cki.SkuID;
        //         sku.SkuName = ckm.SkuName;
        //         sku.Type = 0;
        //         sku.Unit = ckm.Unit;
        //         sku.Weight = ckm.Weight;
        //         sku.CoID = ckm.CoID;
        //         sku.Brand = ckm.Brand;
        //         sku.KindID = ckm.KindID;
        //         sku.KindName = ckm.KindName;
        //         sku.SCoList = ckm.SCoList;
        //         sku.GoodsCode = ckm.GoodsCode;
        //         sku.GoodsName = ckm.GoodsName;
        //         // sku.ColorID = cki.ColorID;
        //         // sku.ColorName = cki.ColorName;
        //         // sku.SizeID = cki.SizeID;
        //         sku.ParentID = cki.ParentID;
        //         // sku.SizeName = cki.SizeName;
        //         sku.CostPrice = cki.CostPrice;
        //         sku.SalePrice = cki.SalePrice;
        //         sku.Norm = cki.ColorName + ";" + cki.SizeName;
        //         sku.Creator = ckm.Creator;
        //         sku.CreateDate = DateTime.Now.ToString();
        //         int count = DbBase.UserDB.Execute(sql, sku, Trans);
        //         if (count == 0)
        //         {
        //             res.s = -3002;
        //         }
        //         Trans.Commit();
        //     }
        //     catch (Exception e)
        //     {
        //         res.s = -1;
        //         res.d = e.Message;
        //     }
        //     finally
        //     {
        //         Trans.Dispose();
        //         DbBase.CoreDB.Close();
        //     }

        //     return res;
        // }
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

        public static DataResult createSku(TmallSku sku)
        {
            var result = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    string sql = "SELECT ID FROM coresku WHERE SkuID = '" + sku.SkuID + "'";
                    var res = conn.Query<long>(sql).AsList();
                    if (res.Count > 0)
                    {
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
                    }
                    else
                    {
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


                    var rnt = conn.Execute(sql, sku);
                    if (rnt > 0)
                    {
                        result.s = 1;
                    }
                    else
                    {
                        result.s = -1;
                        Console.WriteLine("---------" + sku.SkuID);
                    }


                }
                catch (Exception ex)
                {
                    result.s = -1;
                    result.d = ex.Message;
                    Console.WriteLine(ex.Message);
                    conn.Dispose();
                }

            }
            return result;
        }


        public static DataResult getWareSku(CoreSkuParam IParam, string coid)
        {
            var result = new DataResult(1, null);
            var goods = getWareGoodsInner(coid);
            string goodCodes = "'0'";
            foreach (var good in goods)
            {
                goodCodes += ",'" + good.GoodsCode + "'";
            }

            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    StringBuilder querysql = new StringBuilder();
                    StringBuilder totalSql = new StringBuilder();
                    var p = new DynamicParameters();
                    querysql.Append("SELECT distinct  SkuID,ID, SkuName,Norm FROM coresku WHERE Type=0 AND IsParent = FALSE AND SkuName !='' AND IsDelete = FALSE AND CoID = " + coid + " AND GoodsCode in (" + goodCodes + ") ");
                    totalSql.Append("SELECT COUNT(ID) FROM coresku WHERE Type=0 AND IsParent = FALSE AND SkuName !='' AND IsDelete = FALSE  AND CoID = " + coid + "  AND GoodsCode in (" + goodCodes + ") ");
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
                    decimal total = conn.Query<decimal>(totalSql.ToString(), p).AsList()[0];
                    if (total > 0)
                    {
                        decimal pagecnt = Math.Ceiling(total / decimal.Parse(IParam.PageSize.ToString()));
                        int dataindex = (IParam.PageIndex - 1) * IParam.PageSize;
                        querysql.Append(" LIMIT @ls , @le");
                        p.Add("@ls", dataindex);
                        p.Add("@le", IParam.PageSize);
                        var list = conn.Query<wareSku>(querysql.ToString(), p).AsList();
                        if (IParam.PageIndex == 1)
                        {
                            result.d = new
                            {
                                list = list,
                                page = IParam.PageIndex,
                                pageSize = IParam.PageSize,
                                pageTotal = pagecnt,
                                total = total
                            };
                        }
                        else
                        {
                            result.d = new
                            {
                                list = list,
                                page = IParam.PageIndex
                            };
                        }
                    }
                }
                catch
                {
                    conn.Dispose();
                }
            }
            return result;
        }

        public static DataResult getWareGoods(CoreSkuParam IParam, string coid)
        {
            var result = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    StringBuilder querysql = new StringBuilder();
                    StringBuilder totalSql = new StringBuilder();
                    var p = new DynamicParameters();
                    querysql.Append("SELECT distinct  SkuID,ID, SkuName,Norm,GoodsCode FROM coresku WHERE Type=0 AND IsParent = True  AND IsDelete = FALSE AND CoID = " + coid + " ");
                    totalSql.Append("SELECT COUNT(ID) FROM coresku WHERE Type=0 AND IsParent = TRUE  AND IsDelete = FALSE AND CoID = " + coid + " ");
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
                    decimal total = conn.Query<decimal>(totalSql.ToString(), p).AsList()[0];
                    if (total > 0)
                    {
                        decimal pagecnt = Math.Ceiling(total / decimal.Parse(IParam.PageSize.ToString()));
                        int dataindex = (IParam.PageIndex - 1) * IParam.PageSize;
                        querysql.Append(" LIMIT @ls , @le");
                        p.Add("@ls", dataindex);
                        p.Add("@le", IParam.PageSize);
                        var list = conn.Query<wareGoods>(querysql.ToString(), p).AsList();
                        if (IParam.PageIndex == 1)
                        {
                            result.d = new
                            {
                                list = list,
                                page = IParam.PageIndex,
                                pageSize = IParam.PageSize,
                                pageTotal = pagecnt,
                                total = total
                            };
                        }
                        else
                        {
                            result.d = new
                            {
                                list = list,
                                page = IParam.PageIndex
                            };
                        }
                    }
                }
                catch
                {
                    conn.Dispose();
                }
            }
            return result;
        }

        public static List<wareGoods> getWareGoodsInner(string coid)
        {
            var result = new List<wareGoods>();
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    StringBuilder querysql = new StringBuilder();
                    StringBuilder totalSql = new StringBuilder();
                    var p = new DynamicParameters();
                    querysql.Append("SELECT distinct  SkuID,SkuName,Norm,GoodsCode FROM coresku WHERE Type=0 AND IsParent = True  AND IsDelete = FALSE AND CoID = " + coid + " ORDER BY SkuID ASC");
                    result = conn.Query<wareGoods>(querysql.ToString()).AsList();


                }
                catch
                {
                    conn.Dispose();
                }
            }
            return result;
        }

        public static void wareployContain(string skuid, List<string> goodscode, int coid, int type)
        {    //type: 1 sku  2 good     
            string sql = "";
            var ids = new List<string>();
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    if (type == 1)
                    {
                        sql = "SELECT id FROM coresku WHERE SkuID='" + skuid + "' AND CoID = '" + coid + "';";
                    }
                    else
                    {

                        sql = "SELECT id FROM coresku WHERE GoodsCode in (" + string.Join(",", goodscode.ToArray()) + ") AND CoID = '" + coid + "';";
                    }

                }
                catch
                {
                    conn.Dispose();
                }
            }
            if (ids.Count > 0)
            {
                int i = 0;
                var tasks = new Task[10];
                foreach (var id in ids)
                {
                    tasks[i] = Task.Factory.StartNew(() =>
                    {
                        removeContain(id);
                    });
                    i++;
                    if (i == 10)
                    {
                        i = 0;
                        Task.WaitAll(tasks);
                    }
                }
            }
        }

        public static void removeContain(string id)
        {
            string sql = "";
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {

                    sql = "SELECT ID FROM wareploy WHERE ContainGoods LIKE '," + id + ",' OR ContainSkus LIKE '," + id + ",' OR RemoveGoods LIKE '," + id + ",' OR RemoveSkus LIKE '," + id + ",' ;";
                    var ids = conn.Query<int>(sql).AsList();
                    if (ids.Count > 0)
                    {
                        string a = string.Join(",", ids.ToArray());
                        sql = @"UPDATE wareploy SET	
                                    ContainGoods = REPLACE(ContainGoods, '," + id + @",', ','),
                                    ContainSkus  = REPLACE(ContainSkus, '," + id + @",', ','),
                                    RemoveGoods  = REPLACE(RemoveGoods, '," + id + @",', ','),
                                    RemoveSkus = REPLACE(RemoveSkus, '," + id + @",', ',')
                                WHERE ID in(" + a + ")";
                    }


                }
                catch
                {
                    conn.Dispose();
                }
            }
        }







    }

}