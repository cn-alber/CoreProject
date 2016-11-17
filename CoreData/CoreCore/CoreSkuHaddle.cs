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
            querysql.Append("select ID,GoodsCode,GoodsName,KindID,KindName,Enable,SalePrice,ScoGoodsCode from coresku_main where Type = @Type");
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
                    cs.BrandLst = new Dictionary<string, object>();
                    cs.ScoLst = new Dictionary<string, object>();
                    // cs.ScoLst = new List<ScoCompDDLB>();
                    var BrandIDLst = SkuLst.Where(a => a.Brand != null).Select(a => a.Brand).Distinct().AsList();
                    var SCoIDLst = SkuLst.Where(a => a.SCoID != null).Select(a => a.SCoID).Distinct().AsList();
                    if (BrandIDLst.Count > 0)
                    {
                        var BrandLst = CommConn.Query<BrandDDLB>("SELECT ID,Name,Intro FROM Brand WHERE CoID=@CoID AND ID IN @IDLST", new { CoID = IParam.CoID, IDLST = BrandIDLst }).AsList();
                        foreach (var b in BrandLst)
                        {
                            cs.BrandLst.Add(b.ID, b);
                        }
                    }
                    if (SCoIDLst.Count > 0)
                    {
                        var SCoLst = CoreConn.Query<ScoCompDDLB>("SELECT id,scocode,scosimple FROM supplycompany WHERE CoID=@CoID AND id IN @IDLST", new { CoID = IParam.CoID, IDLST = SCoIDLst }).AsList();
                        foreach (var s in SCoLst)
                        {
                            cs.ScoLst.Add(s.id, s);
                        }
                        // cs.ScoLst = SCoLst;
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
            string itemsql = @"SELECT * FROM coresku WHERE ParentID=@ID AND CoID = @CoID AND IsDelete=0";
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
                        cs.main = main;
                        cs.items = conn.Query<CoreSkuItem>(itemsql, p).AsList();
                        cs.itemprops = conn.Query<goods_item_props>(itempropsql, p).AsList();
                        cs.skuprops = conn.Query<goods_sku_props>(skupropsql, p).AsList();
                        var res1 = CommHaddle.GetBrandByID(CoID, main.Brand);//品牌
                        var res2 = CommHaddle.GetScoNameByID(CoID, main.ScoID);//供应商
                        var res3 = CommHaddle.GetBrandByID(CoID, main.KindID);//类目名称
                        var res4 = CommHaddle.GetShopNameByID(CoID, main.TempShopID);
                        main.BrandName = res1.d as string;
                        main.ScoName = res2.d as string;
                        main.KindName = res3.d as string;
                        main.TempShopName = res4.d as string;
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
                        p1.Add("@SkuPropsValLst", SkuPropsValLst);
                    }
                    conn.Execute(UptSkuPropSql, p1, Trans);

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
                // string strsql = "delete from coresku where CoID = @CoID and GoodsCode in @GoodsLst and IsDelete=1";
                // var p = new DynamicParameters();
                // p.Add("@CoID", CoID);
                // p.Add("@GoodsLst", GoodsLst);
                // int count = DbBase.CoreDB.Execute(strsql, p);
                // if (count <= 0)
                // {
                //     res.s = -3004;
                // }
            }
            catch (Exception e)
            {
                res.s = -1;
                res.d = e.Message;
            }
            return res;
        }
        #endregion

        #region 商品维护 - 判断商品sku是否存在
        public static DataResult ExistSku(int ParentID, List<string> SkuIDLst, string CoID)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    StringBuilder querystr = new StringBuilder();
                    querystr.Append("select SkuID from coresku where CoID = @CoID AND IsDelete=0 AND SkuID in @SkuIDLst ");
                    var p = new DynamicParameters();
                    p.Add("@CoID", CoID);
                    p.Add("@SkuIDLst", SkuIDLst);
                    if (ParentID > 0)
                    {
                        querystr.Append(" and ParentID !=@ParentID");
                        p.Add("@ParentID", ParentID);
                    }
                    var Lst = conn.Query<string>(querystr.ToString(), p).AsList();
                    if (Lst.Count > 0)
                    {
                        res.s = -1;
                        res.d = "商品编码已存在:" + string.Join(",", Lst.ToArray()); ;
                    }
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

        #region 商品维护 - 新增
        public static DataResult NewCore(Coresku_main main, List<goods_item_props> itemprops, List<goods_sku_props> skuprops, List<CoreSkuItem> items)
        {
            //编码是否已存在查询
            var SkuIDLst = items.Select(a => a.SkuID).AsList();
            var res = ExistSku(main.ID, SkuIDLst, main.CoID);
            if (res.s == 1)
            {
                var conn = new MySqlConnection(DbBase.CoreConnectString);
                conn.Open();
                var Trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    conn.Execute(AddCoresku_Main_Sql(), main, Trans);//新增商品主资料
                    long MainID = conn.QueryFirst<long>("select LAST_INSERT_ID()", Trans);//获取新增id
                    var itemPropValLst = itemprops.Select(a => new coresku_item_props
                    {
                        pid = a.pid,
                        val_id = a.val_id,
                        val_name = a.val_name,
                        ParentID = MainID.ToString(),
                        CoID = main.CoID,
                        Creator = main.Creator,
                        CreateDate = main.CreateDate
                    }).AsList();

                    var skuPropValLst = skuprops.Select(a => new coresku_sku_props
                    {
                        pid = a.pid,
                        val_id = a.val_id,
                        val_name = a.val_name,
                        mapping = a.mapping,
                        IsOther = a.IsOther,
                        ParentID = MainID.ToString(),
                        CoID = main.CoID,
                        Creator = main.Creator,
                        CreateDate = main.CreateDate
                    }).AsList();

                    // var itemPropValLst = new List<coresku_item_props>();
                    //                 foreach (var p in itemprops)
                    //                 {
                    //                     var val = new coresku_item_props();
                    //                     val.GoodsCode = main.GoodsCode;
                    //                     val.ParentID = MainID.ToString();
                    //                     val.pid = p.pid;
                    //                     val.val_id = p.val_id;
                    //                     val.val_name = p.val_name;
                    //                     val.CoID = main.CoID;
                    //                     val.Creator = main.Creator;
                    //                     val.CreateDate = main.CreateDate;
                    //                     itemPropValLst.Add(val);
                    //                 }
                    // skuPropValLst= new List<coresku_sku_props>();
                    // foreach (var p in skuprops)
                    // {
                    //     var val = new coresku_sku_props();
                    //     // val.GoodsCode = main.GoodsCode;
                    //     val.ParentID = MainID.ToString();
                    //     val.pid = p.pid;
                    //     val.val_id = p.val_id;
                    //     val.val_name = p.val_name;
                    //     val.mapping = p.mapping;
                    //     val.IsOther = p.IsOther;
                    //     val.CoID = main.CoID;
                    //     val.Creator = main.Creator;
                    //     val.CreateDate = main.CreateDate;
                    //     skuPropValLst.Add(val);
                    // }
                    var SkuLst = new List<Coresku>();
                    foreach (var item in items)
                    {
                        var sku = new Coresku();
                        sku.GoodsCode = main.GoodsCode;
                        sku.GoodsName = main.GoodsName;
                        sku.Brand = main.Brand;
                        sku.KindID = main.KindID;
                        sku.KindName = main.KindName;
                        sku.Type = main.Type;
                        sku.ScoID = main.ScoID;
                        sku.ScoGoodsCode = main.ScoGoodsCode;
                        sku.ScoSku = main.ScoSku;
                        sku.MarketPrice = main.MarketPrice;
                        sku.SkuID = item.SkuID;
                        sku.SkuName = item.SkuName;
                        sku.SkuSimple = item.SkuSimple;
                        sku.PurPrice = item.PurPrice;
                        sku.SalePrice = item.SalePrice;
                        sku.Weight = item.Weight;
                        sku.pid1 = item.pid1;
                        sku.val_id1 = item.val_id1;
                        sku.pid2 = item.pid2;
                        sku.val_id2 = item.val_id2;
                        sku.pid3 = item.pid3;
                        sku.val_id3 = item.val_id3;
                        sku.Norm = item.Norm;
                        sku.CoID = main.CoID;
                        sku.Creator = main.Creator;
                        sku.CreateDate = main.CreateDate;
                        sku.ParentID = MainID.ToString();
                        SkuLst.Add(sku);
                    }
                    conn.Execute(AddCoresku_item_props_sql(), itemPropValLst, Trans);
                    conn.Execute(AddCoresku_sku_props_sql(), skuPropValLst, Trans);
                    conn.Execute(AddCoresku_sql(), SkuLst, Trans);
                    if (SkuLst.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(SkuLst[0].pid1))
                        {
                            string UptValSql = @"UPDATE coresku,coresku_sku_props
                                    SET val_id1 =  coresku_sku_props.ID
                                    WHERE coresku.ParentID = coresku_sku_props.ParentID
                                    AND coresku.CoID = coresku_sku_props.CoID
                                    AND coresku.pid1 = coresku_sku_props.pid
                                    AND coresku.val_id1 = coresku_sku_props.val_id
                                    AND coresku.CoID = @CoID
                                    AND coresku.ParentID = @ParentID";
                            conn.Execute(UptValSql, new { CoID = main.CoID, ParentID = MainID }, Trans);
                        }
                        if (!string.IsNullOrEmpty(SkuLst[0].pid2))
                        {
                            string UptValSql = @"UPDATE coresku,coresku_sku_props
                                    SET val_id2 =  coresku_sku_props.ID
                                    WHERE coresku.ParentID = coresku_sku_props.ParentID
                                    AND coresku.CoID = coresku_sku_props.CoID
                                    AND coresku.pid2 = coresku_sku_props.pid
                                    AND coresku.val_id2 = coresku_sku_props.val_id
                                    AND coresku.CoID = @CoID
                                    AND coresku.ParentID = @ParentID";
                            conn.Execute(UptValSql, new { CoID = main.CoID, ParentID = MainID }, Trans);
                        }
                        if (!string.IsNullOrEmpty(SkuLst[0].pid3))
                        {
                            string UptValSql = @"UPDATE coresku,coresku_sku_props
                                    SET val_id3 =  coresku_sku_props.ID
                                    WHERE coresku.ParentID = coresku_sku_props.ParentID
                                    AND coresku.CoID = coresku_sku_props.CoID
                                    AND coresku.pid3 = coresku_sku_props.pid
                                    AND coresku.val_id3 = coresku_sku_props.val_id
                                    AND coresku.CoID = @CoID
                                    AND coresku.ParentID = @ParentID";
                            conn.Execute(UptValSql, new { CoID = main.CoID, ParentID = MainID }, Trans);
                        }
                    }
                    Trans.Commit();
                }
                catch (Exception e)
                {
                    Trans.Rollback();
                    res.s = -1;
                    res.d = e.Message;
                }
                finally
                {
                    Trans.Dispose();
                    conn.Close();
                }

            }

            return res;
        }
        #endregion

        #region 商品维护 - 修改
        public static DataResult EditCore(Coresku_main main, List<goods_item_props> itemprops, List<goods_sku_props> skuprops, List<CoreSkuItem> items, string CoID, string UserName)
        {
            var SkuIDLst = items.Select(a => a.SkuID).AsList();
            var res = ExistSku(main.ID, SkuIDLst, main.CoID);
            if (res.s == 1)
            {
                var conn = new MySqlConnection(DbBase.CoreConnectString);
                bool IsCommit = false;
                conn.Open();
                var Trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                string msql = @"SELECT * FROM coresku_main WHERE ID=@ID AND CoID = @CoID";
                string itempropsql = @"SELECT * FROM coresku_item_props WHERE CoID = @CoID AND ParentID=@ID AND ISDelete = 0";
                string skupropsql = @"SELECT * FROM coresku_sku_props WHERE CoID = @CoID AND ParentID=@ID AND ISDelete = 0";
                string itemsql = @"SELECT * FROM coresku WHERE CoID = @CoID AND ParentID=@ID";
                string contents = string.Empty;
                var p = new DynamicParameters();
                p.Add("@ID", main.ID);
                p.Add("@CoID", main.ScoID);
                try
                {
                    var main_Old = conn.QueryFirst<Coresku_main>(msql, p, Trans);
                    var itemprops_Old = conn.Query<coresku_item_props>(itempropsql, p, Trans).AsList();
                    var skuprops_Old = conn.Query<coresku_sku_props>(skupropsql, p, Trans).AsList();
                    var items_Old = conn.Query<Coresku>(itemsql, p, Trans).AsList();
                    #region 更新主表信息
                    string UptMainSql = "UPDATE coresku_main SET Modifier = @Modifier,ModifyDate=@ModifyDate";
                    if (main.GoodsCode != main_Old.GoodsCode)
                    {
                        contents = contents + "货号:" + main_Old.GoodsCode + "=>" + main.GoodsCode + ";";
                        UptMainSql = UptMainSql + " ,GoodsCode=@GoodsCode";
                    }
                    if (main.GoodsName != main_Old.GoodsName)
                    {
                        contents = contents + "货品名称:" + main_Old.GoodsName + "=>" + main.GoodsName + ";";
                        UptMainSql = UptMainSql + " ,GoodsName=@GoodsName";
                    }
                    if (main.Brand != main_Old.Brand)
                    {
                        contents = contents + "品牌:" + main_Old.Brand + "=>" + main.Brand + ";";
                        UptMainSql = UptMainSql + " ,Brand=@Brand";
                    }
                    if (main.KindID != main_Old.KindID)
                    {
                        contents = contents + "分类:" + main_Old.KindID + "=>" + main.KindID + ";";
                        UptMainSql = UptMainSql + " ,KindID=@KindID";
                    }
                    if (main.ScoID != main_Old.ScoID)
                    {
                        contents = contents + "供应商:" + main_Old.ScoID + "=>" + main.ScoID + ";";
                        UptMainSql = UptMainSql + " ,ScoID=@ScoID";
                    }
                    if (main.ScoGoodsCode != main_Old.ScoGoodsCode)
                    {
                        contents = contents + "供应商货号:" + main_Old.ScoGoodsCode + "=>" + main.ScoGoodsCode + ";";
                        UptMainSql = UptMainSql + " ,ScoGoodsCode=@ScoGoodsCode";
                    }
                    if (main.Weight != main_Old.Weight)
                    {
                        contents = contents + "货品重量:" + main_Old.Weight + "=>" + main.Weight + ";";
                        UptMainSql = UptMainSql + " ,Weight=@Weight";
                    }
                    if (main.PurPrice != main_Old.PurPrice)
                    {
                        contents = contents + "采购成本:" + main_Old.PurPrice + "=>" + main.PurPrice + ";";
                        UptMainSql = UptMainSql + " ,PurPrice=@PurPrice";
                    }
                    if (main.SalePrice != main_Old.SalePrice)
                    {
                        contents = contents + "基本零售价:" + main_Old.SalePrice + "=>" + main.SalePrice + ";";
                        UptMainSql = UptMainSql + " ,SalePrice=@SalePrice";
                    }
                    if (main.MarketPrice != main_Old.MarketPrice)
                    {
                        contents = contents + "市场吊牌价:" + main_Old.MarketPrice + "=>" + main.MarketPrice + ";";
                        UptMainSql = UptMainSql + " ,MarketPrice=@MarketPrice";
                    }
                    if (main.TempShopID != main_Old.TempShopID)
                    {
                        contents = contents + "淘宝模板店铺:" + main_Old.TempShopID + "=>" + main.TempShopID + ";";
                        UptMainSql = UptMainSql + " ,TempShopID=@TempShopID";
                    }
                    if (main.TempID != main_Old.TempID)
                    {
                        contents = contents + "淘宝宝贝编号:" + main_Old.TempID + "=>" + main.TempID + ";";
                        UptMainSql = UptMainSql + " ,TempID=@TempID";
                    }
                    if (main.Img != main_Old.Img)
                    {
                        contents = contents + "图片地址:" + main_Old.Img + "=>" + main.Img + ";";
                        UptMainSql = UptMainSql + " ,Img=@Img";
                    }
                    if (main.Remark != main_Old.Remark)
                    {
                        contents = contents + "备注说明:" + main_Old.Remark + "=>" + main.Remark + ";";
                        UptMainSql = UptMainSql + " ,Remark=@Remark";
                    }
                    if (!string.IsNullOrEmpty(contents))
                    {
                        IsCommit = true;
                        UptMainSql = UptMainSql + " WHERE CoID=@CoID AND ID=@ID";
                        conn.Execute(UptMainSql, main, Trans);
                    }
                    #endregion
                    #region 更新商品item属性/Sku属性/商品明细资料
                    var NewItemPorpLst = itemprops.Where(a => a.ID <= 0)
                                                .Select(a => new coresku_item_props
                                                {
                                                    pid = a.pid,
                                                    val_id = a.val_id,
                                                    val_name = a.val_name,
                                                    ParentID = main.ID.ToString(),
                                                    CoID = CoID,
                                                    Creator = UserName,
                                                    CreateDate = DateTime.Now.ToString()
                                                }).AsList();
                    var UptItemPropLst = itemprops.Where(a => a.ID > 0 &&
                                                         !itemprops_Old.Any(b => a.pid == b.pid &&
                                                                                a.val_id == b.val_id &&
                                                                                a.val_name == b.val_name &&
                                                                                a.Enable == b.Enable))
                                                .Select(a => new coresku_item_props
                                                {
                                                    ID = a.ID,
                                                    pid = a.pid,
                                                    val_id = a.val_id,
                                                    val_name = a.val_name,
                                                    Enable = a.Enable,
                                                    ParentID = main.ID.ToString(),
                                                    CoID = CoID,
                                                    Modifier = UserName,
                                                    ModifyDate = DateTime.Now.ToString()
                                                }).AsList();
                    var NewSkuPropLst = skuprops.Where(a => a.ID <= 0)
                                            .Select(a => new coresku_sku_props
                                            {
                                                pid = a.pid,
                                                val_id = a.val_id,
                                                val_name = a.val_name,
                                                mapping = a.mapping,
                                                IsOther = a.IsOther,
                                                ParentID = main.ID.ToString(),
                                                CoID = CoID,
                                                Creator = UserName,
                                                CreateDate = DateTime.Now.ToString()
                                            }).AsList();
                    var UptSkuPropLst = skuprops.Where(a => a.ID > 0 &&
                                                         !skuprops_Old.Any(b => a.pid == b.pid &&
                                                                                a.val_id == b.val_id &&
                                                                                a.val_name == b.val_name &&
                                                                                a.mapping == b.mapping &&
                                                                                a.Enable == b.Enable))
                                            .Select(a => new coresku_sku_props
                                            {
                                                ID = a.ID,
                                                pid = a.pid,
                                                val_id = a.val_id,
                                                val_name = a.val_name,
                                                mapping = a.mapping,
                                                Enable = a.Enable,
                                                ParentID = main.ID.ToString(),
                                                CoID = CoID,
                                                Modifier = UserName,
                                                ModifyDate = DateTime.Now.ToString()
                                            }).AsList();

                    var NewCoreSkuLst = items.Where(a => !items_Old.Any(b => a.pid1 == b.pid1 &&
                                                                    a.val_id1 == b.val_id1 &&
                                                                    a.pid2 == b.pid2 &&
                                                                    a.val_id2 == b.val_id2 &&
                                                                    a.pid3 == b.pid3 &&
                                                                    a.val_id3 == b.val_id3))

                                            .Select((a) => new Coresku
                                            {
                                                GoodsCode = main.GoodsCode,
                                                GoodsName = main.GoodsName,
                                                Brand = main.Brand,
                                                KindID = main.KindID,
                                                KindName = main.KindName,
                                                Type = main.Type,
                                                ScoID = main.ScoID,
                                                ScoGoodsCode = main.ScoGoodsCode,
                                                ScoSku = main.ScoSku,
                                                MarketPrice = main.MarketPrice,
                                                SkuID = a.SkuID,
                                                SkuName = a.SkuName,
                                                SkuSimple = a.SkuSimple,
                                                PurPrice = a.PurPrice,
                                                SalePrice = a.SalePrice,
                                                Weight = a.Weight,
                                                pid1 = a.pid1,
                                                val_id1 = a.val_id1,
                                                pid2 = a.pid2,
                                                val_id2 = a.val_id2,
                                                pid3 = a.pid3,
                                                val_id3 = a.val_id3,
                                                Norm = a.Norm,
                                                Remark = main.Remark,
                                                CoID = CoID,
                                                Creator = UserName,
                                                CreateDate = DateTime.Now.ToString(),
                                                ParentID = main.ID.ToString()
                                            }).AsList();

                    var UptCoreSkuLst = (from a in items
                                         join b in items_Old
                                         on new { pid1 = a.pid1, val_id1 = a.val_id1, pid2 = a.pid2, val_id2 = a.val_id2, pid3 = a.pid3, val_id3 = a.val_id3 }
                                         equals new { pid1 = b.pid1, val_id1 = b.val_id1, pid2 = b.pid2, val_id2 = b.val_id2, pid3 = b.pid3, val_id3 = b.val_id3 }
                                         group new { a, b } by new
                                         {
                                             b.ID,
                                             a.SkuID,
                                             a.SkuName,
                                             a.SkuSimple,
                                             a.Norm,
                                             a.PurPrice,
                                             a.SalePrice,
                                             a.Weight,
                                             a.pid1,
                                             a.val_id1,
                                             a.pid2,
                                             a.val_id2,
                                             a.pid3,
                                             a.val_id3
                                         } into n
                                         select new Coresku
                                         {
                                             ID = n.Key.ID,
                                             Type = main.Type,
                                             GoodsCode = main.GoodsCode,
                                             GoodsName = main.GoodsName,
                                             Brand = main.Brand,
                                             KindID = main.KindID,
                                             KindName = main.KindName,
                                             ScoID = main.ScoID,
                                             ScoGoodsCode = main.ScoGoodsCode,
                                             ScoSku = main.ScoSku,
                                             Remark = main.Remark,
                                             SkuID = n.Key.SkuID,
                                             SkuName = n.Key.SkuName,
                                             SkuSimple = n.Key.SkuSimple,
                                             Norm = n.Key.Norm,
                                             MarketPrice = main.MarketPrice,
                                             PurPrice = n.Key.PurPrice,
                                             SalePrice = n.Key.SalePrice,
                                             Weight = n.Key.Weight,
                                             pid1 = n.Key.pid1,
                                             val_id1 = n.Key.val_id1,
                                             pid2 = n.Key.pid2,
                                             val_id2 = n.Key.val_id2,
                                             pid3 = n.Key.pid3,
                                             val_id3 = n.Key.val_id3,
                                             CoID = CoID,
                                             Modifier = UserName,
                                             ModifyDate = DateTime.Now.ToString(),
                                             ParentID = main.ID.ToString()
                                         })
                                        .AsList();
                    var RmvLst = new List<Coresku>();
                    RmvLst.AddRange(UptCoreSkuLst);
                    if (String.IsNullOrEmpty(contents))
                    {
                        foreach (var Upt in RmvLst)//移除未修改项
                        {
                            var EqCount = items_Old.Where(a => a.ID == Upt.ID &&
                            a.SkuID == Upt.SkuID &&
                            a.SkuName == Upt.SkuName &&
                            a.SkuSimple == Upt.SkuName &&
                            a.Norm == Upt.Norm &&
                            a.Weight == Upt.Weight &&
                            a.PurPrice == Upt.PurPrice &&
                            a.SalePrice == Upt.SalePrice &&
                            a.IsDelete == false).Count();
                            if (EqCount > 0)
                            {
                                UptCoreSkuLst.Remove(Upt);
                            }
                        }
                    }

                    var DelIDLst = items_Old.Select(a => a.ID).Where(a => items.Select(b => b.ID).Contains(a)).AsList();
                    if (!string.IsNullOrEmpty(contents))
                    {
                        string UptMain = @"UPDATE coresku_main
                                        SET GoodsCode =@GoodsCode,
                                            GoodsName =@GoodsName,
                                            Brand =@Brand,
                                            KindID =@KindID,
                                            KindName =@KindName,
                                            ScoID =@ScoID,
                                            ScoGoodsCode =@ScoGoodsCode,
                                            Weight =@ScoGoodsCode,
                                            PurPrice =@PurPrice,
                                            SalePrice=@SalePrice,
                                            MarketPrice=@MarketPrice,
                                            Modifier = @Modifier,
                                            ModifyDate = @ModifyDate
                                        WHERE ID=@ID AND CoID=@CoID;
                                            ";
                        conn.Execute(UptMain, main, Trans);//更新商品主表
                        IsCommit = true;
                    }
                    if (NewItemPorpLst.Count > 0)//新增商品屬性
                    {
                        conn.Execute(AddCoresku_item_props_sql(), NewItemPorpLst, Trans);
                        IsCommit = true;
                    }
                    if (UptItemPropLst.Count > 0)
                    {
                        string UptSql = @"UPDATE coresku_item_props
                                        SET pid =@pid,
                                        val_id =@val_id,
                                        val_name =@val_name,
                                        Enable = @Enable,
                                        Modifier =@Modifier,
                                        ModifyDate =@ModifyDate
                                        WHERE CoID=@CoID
                                        AND ParentID=@ParentID
                                        AND ID = @ID";
                        conn.Execute(UptSql, UptItemPropLst, Trans);
                        IsCommit = true;
                    }
                    if (NewSkuPropLst.Count > 0)
                    {
                        conn.Execute(AddCoresku_sku_props_sql(), NewSkuPropLst, Trans);
                        IsCommit = true;
                    }
                    if (UptSkuPropLst.Count > 0)
                    {
                        string UptSql = @"UPDATE coresku_sku_props
                                         SET pid =@pid,
                                        val_id =@val_id,
                                        val_name =@val_name,
                                        mapping =@mapping,                                        
                                        Enable = @Enable,
                                        Modifier =@Modifier,
                                        ModifyDate =@ModifyDate
                                        WHERE CoID=@CoID
                                        AND ParentID=@ParentID
                                        AND ID = @ID";
                        conn.Execute(UptSql, UptSkuPropLst, Trans);
                        IsCommit = true;
                    }
                    if (UptCoreSkuLst.Count > 0)
                    {
                        conn.Execute(UptCoreSkuSql(), UptCoreSkuLst, Trans);
                        IsCommit = true;
                    }
                    if (DelIDLst.Count > 0)
                    {
                        conn.Execute(DelCoreSkuSql(), new { CoID = CoID, IDLst = DelIDLst, Modifier = UserName, ModifyDate = DateTime.Now.ToString() }, Trans);
                        IsCommit = true;
                    }
                    if (NewCoreSkuLst.Count > 0)
                    {
                        conn.Execute(AddCoresku_sql(), NewCoreSkuLst, Trans);
                        long ID = conn.QueryFirst<long>("select LAST_INSERT_ID()", Trans);//获取新增id
                        if (!string.IsNullOrEmpty(NewCoreSkuLst[0].pid1))
                        {
                            string UptValSql = @"UPDATE coresku,coresku_sku_props
                                    SET val_id1 =  coresku_sku_props.ID
                                    WHERE coresku.ParentID = coresku_sku_props.ParentID
                                    AND coresku.CoID = coresku_sku_props.CoID
                                    AND coresku.pid1 = coresku_sku_props.pid
                                    AND coresku.val_id1 = coresku_sku_props.val_id
                                    AND coresku.CoID = @CoID
                                    AND coresku.ParentID = @ParentID
                                    AND coresku.ID>=@ID";
                            conn.Execute(UptValSql, new { CoID = main.CoID, ParentID = main.ID, ID = ID }, Trans);
                        }
                        if (!string.IsNullOrEmpty(NewCoreSkuLst[0].pid2))
                        {
                            string UptValSql = @"UPDATE coresku,coresku_sku_props
                                    SET val_id2 =  coresku_sku_props.ID
                                    WHERE coresku.ParentID = coresku_sku_props.ParentID
                                    AND coresku.CoID = coresku_sku_props.CoID
                                    AND coresku.pid2 = coresku_sku_props.pid
                                    AND coresku.val_id2 = coresku_sku_props.val_id
                                    AND coresku.CoID = @CoID
                                    AND coresku.ParentID = @ParentID
                                    AND coresku.ID>=@ID";
                            conn.Execute(UptValSql, new { CoID = main.CoID, ParentID = main.ID, ID = ID }, Trans);
                        }
                        if (!string.IsNullOrEmpty(NewCoreSkuLst[0].pid3))
                        {
                            string UptValSql = @"UPDATE coresku,coresku_sku_props
                                    SET val_id3 =  coresku_sku_props.ID
                                    WHERE coresku.ParentID = coresku_sku_props.ParentID
                                    AND coresku.CoID = coresku_sku_props.CoID
                                    AND coresku.pid3 = coresku_sku_props.pid
                                    AND coresku.val_id3 = coresku_sku_props.val_id
                                    AND coresku.CoID = @CoID
                                    AND coresku.ParentID = @ParentID
                                    AND coresku.ID>=@ID";
                            conn.Execute(UptValSql, new { CoID = main.CoID, ParentID = main.ID, ID = ID }, Trans);
                        }
                    }
                    #endregion
                    if (IsCommit)
                        Trans.Commit();
                }
                catch (Exception e)
                {
                    Trans.Rollback();
                    res.s = -1;
                    res.d = e.Message;
                }
                finally
                {
                    Trans.Dispose();
                    conn.Close();
                }
            }
            return res;
        }
        #endregion

        #region 商品维护——启用|停用|备用
        public static DataResult UptGoodsEnable(List<int> IDLst, string CoID, string UserName, int Enable)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                conn.Open();
                var Trans = conn.BeginTransaction();
                try
                {
                    string contents = string.Empty;
                    string UptSql = @"UPDATE coresku_main SET Enable=@Enable,Modifier=@ModifyDate,ModifyDate=@ModifyDate where CoID=@CoID AND ID in @IDLst";
                    string UptSkuSql = @"UPDATE coresku SET Enable=@Enable,Modifier=@ModifyDate,ModifyDate=@ModifyDate where CoID=@CoID AND ParentID in @IDLst";
                    var p = new DynamicParameters();
                    p.Add("@CoID", CoID);
                    p.Add("@Enable", Enable);
                    p.Add("@Modifier", UserName);
                    p.Add("@ModifyDate", DateTime.Now);
                    p.Add("@IDLst", IDLst);
                    int count1 = conn.Execute(UptSql, p, Trans);
                    int count2 = conn.Execute(UptSkuSql, p, Trans);

                    if (count1 < 0 || count2 < 0)
                    {
                        res.s = -3003;
                    }
                    else
                    {
                        if (Enable == 1)
                        {
                            contents = "商品启用：";
                            // res.s = 3001;
                        }
                        else if (Enable == 0)
                        {
                            contents = "商品停用：";
                            // res.s = 3002;
                        }
                        else if (Enable == 2)
                        {
                            contents = "商品备用：";
                        }
                        contents += string.Join(",", IDLst.ToArray());
                        CoreUser.LogComm.InsertUserLog("修改商品状态", "Coresku_main", contents, UserName, int.Parse(CoID), DateTime.Now);
                        if (res.s > 0)
                        {
                            Trans.Commit();
                        }
                    }
                }
                catch (Exception e)
                {
                    Trans.Rollback();
                    res.s = -1;
                    res.d = e.Message;
                }
                finally
                {
                    Trans.Dispose();
                    conn.Close();
                }
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
            querysql.Append("select ID,GoodsCode,GoodsName,SkuID,SkuName,Norm,GBCode,Brand,PurPrice,SalePrice,Enable,Img,Creator,CreateDate from coresku where 1=1");
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
                    //商品明细
                    var SkuLst = CoreData.DbBase.CoreDB.Query<SkuQuery>(querysql.ToString(), p).AsList();
                    //品牌列表
                    if (SkuLst.Count > 0)
                    {
                        var BrandIDLst = SkuLst.Select(a => a.Brand).Distinct().AsList();
                        res = CommHaddle.GetBrandsByID(IParam.CoID.ToString(), BrandIDLst);
                        if (res.s == 1)
                        {
                            var BrandLst = res.d as List<BrandDDLB>;
                            SkuLst = (from a in SkuLst
                                      join b in BrandLst
                                      on new { Brand = a.Brand } equals new { Brand = b.ID }
                                      group new { a, b } by new
                                      {
                                          a.ID,
                                          a.GoodsCode,
                                          a.GoodsName,
                                          a.SkuID,
                                          a.SkuName,
                                          a.Norm,
                                          a.GBCode,
                                          a.Brand,
                                          b.Name,
                                          a.PurPrice,
                                          a.SalePrice,
                                          a.Enable,
                                          a.Img,
                                          a.Creator,
                                          a.CreateDate
                                      } into c
                                      select new SkuQuery
                                      {
                                          ID = c.Key.ID,
                                          GoodsCode = c.Key.GoodsCode,
                                          GoodsName = c.Key.GoodsName,
                                          SkuID = c.Key.SkuID,
                                          SkuName = c.Key.SkuName,
                                          Norm = c.Key.Norm,
                                          GBCode = c.Key.GBCode,
                                          Brand = c.Key.Brand,
                                          BrandName = c.Key.Name,
                                          PurPrice = c.Key.PurPrice,
                                          SalePrice = c.Key.SalePrice,
                                          Enable = c.Key.Enable,
                                          Img = c.Key.Img,
                                          Creator = c.Key.Creator,
                                          CreateDate = c.Key.CreateDate
                                      }).AsList();
                        }
                        cs.SkuLst = SkuLst;
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

        #region 新增 - 商品主表SQL - 商品维护
        public static string AddCoresku_Main_Sql()
        {
            string sql = @"INSERT INTO coresku_main(GoodsCode,
                                            GoodsName,
                                            Brand,
                                            KindID,
                                            KindName,
                                            ScoID,
                                            ScoGoodsCode,
                                            CostPrice,
                                            PurPrice,
                                            MarketPrice,
                                            SalePrice,
                                            Weight,
                                            TempShopID,
                                            TempID,
                                            Remark,
                                            Img,
                                            Type,
                                            CoID,
                                            `Enable`,
                                            Creator,
                                            CreateDate) 
                                    VALUES (@GoodsCode,
                                            @GoodsName,
                                            @Brand,
                                            @KindID,
                                            @KindName,
                                            @ScoID,
                                            @ScoGoodsCode,
                                            @CostPrice,
                                            @PurPrice,
                                            @MarketPrice,
                                            @SalePrice,
                                            @Weight,
                                            @TempShopID,
                                            @TempID,
                                            @Remark,
                                            @Img,
                                            @Type,
                                            @CoID,
                                            @Enable,
                                            @Creator,
                                            @CreateDate)";
            return sql;
        }
        #endregion
        #region 新增 - 商品item属性SQL
        public static string AddCoresku_item_props_sql()
        {
            string sql = @"INSERT INTO coresku_item_props(
                                            pid,
                                            val_id,
                                            val_name,
                                            ParentID,
                                            CoID,
                                            `Enable`,
                                            Creator,
                                            CreateDate)
                                        VALUES (
                                            @pid,
                                            @val_id,
                                            @val_name,
                                            @ParentID,
                                            @CoID,
                                            @Enable,
                                            @Creator,
                                            @CreateDate
                                            )";
            return sql;
        }
        #endregion
        #region 新增 - 商品sku属性SQL
        public static string AddCoresku_sku_props_sql()
        {
            string sql = @"INSERT INTO coresku_sku_props(
                                            pid,
                                            val_id,
                                            val_name,
                                            mapping,
                                            IsOther,
                                            ParentID,
                                            CoID,
                                            `Enable`,
                                            Creator,
                                            CreateDate)
                                        VALUES (
                                            @pid,
                                            @val_id,
                                            @val_name,
                                            @mapping,
                                            @IsOther,
                                            @ParentID,
                                            @CoID,
                                            @Enable,
                                            @Creator,
                                            @CreateDate
                                            )";
            return sql;
        }
        #endregion
        #region 新增 - 商品明细SQL
        public static string AddCoresku_sql()
        {
            string sql = @"INSERT INTO coresku
                        (
                            SkuID,
                            SkuName,
                            SkuSimple,
                            pid1,
                            val_id1,
                            pid2,
                            val_id2,
                            pid3,
                            val_id3,
                            Norm,
                            PurPrice,
                            MarketPrice,
                            SalePrice,
                            Weight,
                            GBCode,
                            Brand,
                            KindID,
                            KindName,
                            GoodsCode,
                            GoodsName,
                            ScoID,
                            ScoGoodsCode,
                            ScoSku,
                            Img,
                            BigImg,
                            Remark,
                            ParentID,
                            Type,
                            CoID,
                            `Enable`)
                        VALUES (
                            @SkuID,
                            @SkuName,
                            @SkuSimple,
                            @pid1,
                            @val_id1,
                            @pid2,
                            @val_id2,
                            @pid3,
                            @val_id3,
                            @Norm,
                            @PurPrice,
                            @MarketPrice,
                            @SalePrice,
                            @Weight,
                            @GBCode,
                            @Brand,
                            @KindID,
                            @KindName,
                            @GoodsCode,
                            @GoodsName,
                            @ScoID,
                            @ScoGoodsCode,
                            @ScoSku,
                            @Img,
                            @BigImg,
                            @Remark,
                            @ParentID,
                            @Type,
                            @CoID,
                            @Enable) ";
            return sql;
        }
        #endregion

        #region 修改 - 商品明细SQL
        public static string UptCoreSkuSql()
        {
            string UptSql = @"UPDATE coresku
                                    SET SkuID =@SkuID,
                                        SkuName =@SkuName,
                                        SkuSimple =@SkuSimple,
                                        Brand =@Brand,
                                        KindID =@KindID,
                                        Type =@Type,
                                        GoodsCode =@GoodsCode,
                                        GoodsName =@GoodsName,
                                        GBCode =@GBCode,
                                        Weight =@Weight,
                                        PurPrice =@PurPrice,
                                        SalePrice =@SalePrice,
                                        MarketPrice =@MarketPrice,
                                        pid1 =@pid1,
                                        val_id1 =@val_id1,
                                        pid2 =@pid2,
                                        val_id2 =@val_id2,
                                        pid3 =@pid3,
                                        val_id3 =@val_id3,
                                        Norm =@Norm,
                                        Img =@Img,
                                        BigImg =@BigImg,
                                        ScoID =@ScoID,
                                        ScoGoodsCode =@ScoGoodsCode,
                                        ScoSku =@ScoSku,
                                        Remark =@Remark,
                                        ISDelete=0,
                                        Modifier = @Modifier,
                                        ModifyDate = @ModifyDate
                                    WHERE
                                        ID =@ID
                                        AND CoID =@CoID
                                        AND ParentID = @ParentID
                                    ";
            return UptSql;
        }
        #endregion

        #region 删除 - 商品明细SQL
        public static string DelCoreSkuSql()
        {
            string UptSql = @"UPDATE coresku
                                    SET IsDelete = 1,
                                        Modifier =@Modifier,
                                        ModifyDate =@ModifyDate
                                    WHERE
                                        ID IN @IDLst
                                    AND CoID =@CoID";
            return UptSql;
        }

        #endregion


    }

}