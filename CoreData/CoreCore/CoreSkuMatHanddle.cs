using CoreModels;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using CoreModels.XyCore;

namespace CoreData.CoreCore
{
    public static class CoreSkuMatHaddle
    {
        #region 物料资料管理 - 查询物料明细
        public static DataResult GetMatLst(CoreSkuParam IParam)
        {
            var cs = new MatQuery();
            var res = new DataResult(1, null);
            StringBuilder querysql = new StringBuilder();
            var p = new DynamicParameters();
            string sql = @"select GoodsCode,GoodsName,KName,Norm,
            ColorName,SizeName,PurPrice,Enable,
            Remark,Creator,CreateDate
            from coresku 
            where Type = @Type AND IsParent=0";
            querysql.Append(sql);
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
            if (!string.IsNullOrEmpty(IParam.Filter))
            {
                querysql.Append(@" 
                 AND (GoodsCode like @Filter or
                 GoodsName like @Filter or
                 Brand like @Filter or
                 ColorName like @Filter or
                 SizeName like @Filter or
                 SkuID like @Filter or 
                 SkuName like @Filter or 
                 Norm like @Filter)");
                p.Add("@Filter", "'%" + IParam.Filter + "'");
            }
            if (!string.IsNullOrEmpty(IParam.SortField) && !string.IsNullOrEmpty(IParam.SortDirection))//排序
            {
                querysql.Append(" ORDER BY " + IParam.SortField + " " + IParam.SortDirection);
            }
            try
            {
                var SkuLst = CoreData.DbBase.CoreDB.Query<CoreSkuMatQuery>(querysql.ToString(), p).AsList();
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
                    SkuLst = CoreData.DbBase.CoreDB.Query<CoreSkuMatQuery>(querysql.ToString(), p).AsList();
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

        #region 物料管理 - 获取单笔Sku详情
        public static DataResult GetCoreMatEdit(string GoodsCode, int CoID)
        {
            var cs = new CoreSkuMatAuto();
            var res = new DataResult(1, null);
            var p = new DynamicParameters();
            p.Add("@GoodsCode", GoodsCode);
            p.Add("@CoID", CoID);
            string msql = @"SELECT CoID,GoodsCode,GoodsName,Norm,
                            KID,KName,SCoList,Unit,ValUnit,
                            CnvRate,PurPrice,ParentID,Enable
                            FROM coresku
                            WHERE CoID = @CoID AND GoodsCode = @GoodsCode AND IsParent = 1";
            try
            {
                var main = DbBase.CoreDB.QueryFirstOrDefault<CoreSkuMatAuto>(msql, p);
                if (main == null)
                {
                    res.s = -3001;
                }
                else
                {
                    string dsql = @"select GoodsCode,SkuID,ColorID,ColorName,
                                        SizeID,SizeName,Remark
                                    from coresku where GoodsCode=@GoodsCode and CoID = @CoID
                                     AND IsParent = 0 AND Enable=1";
                    var item = DbBase.CoreDB.Query<CoreSkuMatItem>(dsql, p).AsList();
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

        #region 保存物料
        public static DataResult SaveSkuMat(CoreSkuMatAuto sku)
        {
            var res = new DataResult(1, null);
            int count = 0;
            DbBase.CoreDB.Open();
            var Trans = DbBase.CoreDB.BeginTransaction();
            try
            {
                #region 新增主表
                if (sku.mainew == 1)
                {
                    // string sql = @"INSERT INTO(CoID,GoodsCode,GoodsName,SkuID,Norm,Unit,ValUnit,CnvRate,PurPrice,KID,KName,";
                    // sql = sql + "SCoList,ParentID,Type,Creator,CreateDate,IsParent)";
                    // sql = sql + " VALUES(@CoID,@GoodsCode,@GoodsName,@SkuID,@Norm,@Unit,@ValUnit,@CnvRate,@PurPrice,@KID,@KName,";
                    // sql = sql + "@SCoList,@ParentID,@Type,@Creator,@CreateDate,@IsParent)";

                    string sql = @"INSERT INTO coresku(CoID,GoodsCode,GoodsName,SkuID,Norm,Unit,ValUnit,CnvRate,PurPrice,KID,KName,SCoList,ParentID,Type,Creator,CreateDate,IsParent)
                                   VALUES(@CoID,@GoodsCode,@GoodsName,@SkuID,@Norm,@Unit,@ValUnit,@CnvRate,@PurPrice,@KID,@KName,@SCoList,@ParentID,@Type,@Creator,@CreateDate,@IsParent)                                       
                                       ";
                    var core = NewMat(sku);
                    count = DbBase.CoreDB.Execute(sql, core, Trans);
                    if (count == 0)
                    {
                        res.s = -3002;
                    }
                }
                #endregion
                #region 新增明细
                var newitems = sku.items.Where(a => a.status == 1).AsList();
                if (newitems.Count > 0)
                {
                    var itemLst = NewMatDetail(sku, newitems);
                    string itemsql = @"INSERT INTO coresku(CoID,GoodsCode,GoodsName,Norm,Unit,ValUnit,
                                            CnvRate,PurPrice,KID,KName,SCoList,SkuID,ColorID,ColorName,
                                            ParentID,SizeID,SizeName,Type,Creator,CreateDate,IsParent )
                                       VALUES(@CoID,@GoodsCode,@GoodsName,@Norm,@Unit,@ValUnit,
                                            @CnvRate,@PurPrice,@KID,@KName,@SCoList,@SkuID,@ColorID,@ColorName,
                                            @ParentID,@SizeID,@SizeName,@Type,@Creator,@CreateDate,@IsParent)";
                    count = DbBase.CoreDB.Execute(itemsql.ToString(), itemLst, Trans);
                    if (count == 0)
                    {
                        res.s = -3002;
                    }
                }
                #endregion
                #region 单独修改明细                
                var edititems = sku.items.Where(a => a.status == 0).AsList();
                if (edititems.Count > 0)
                {
                    string uptsql = @"UPDATE coresku SET SkuID = @SkuID,Remark = @Remark 
                                      WHERE CoID = @CoID AND GoodsCode = @GoodsCode";
                    count = DbBase.CoreDB.Execute(uptsql.ToString(), edititems, Trans);
                    if (count == 0)
                    {
                        res.s = -3002;
                    }
                }
                #endregion
                #region 删除明细
                var delSkuLst = (sku.items.Where(a=>a.status==2).AsList()).Select(b=>b.SkuID).ToList();
                if (delSkuLst.Count>0)
                {
                    string delsql = @"delete from coresku where SkuID in @DelSkuLst";
                    count = DbBase.CoreDB.Execute(delsql.ToString(), new { DelSkuLst = delSkuLst }, Trans);
                    if (count == 0)
                    {
                        res.s = -3004;
                    }
                }

                #endregion
                #region 主表标记新增
                if (sku.status == 0)
                {
                    string uptsql = @"UPDATE coresku SET GoodsName = @GoodsName,Norm = @Norm,Unit = @Unit,
                                    ValUnit = @ValUnit,CnvRate = @CnvRate,PurPrice = @PurPrice,KID = @KID,
                                    KName = @KName,SCoList = @SCoList,ParentID = @ParentID,Enable = @Enable
                                    WHERE CoID = @CoID AND GoodsCode = @GoodsCode";
                    count = DbBase.CoreDB.Execute(uptsql.ToString(), sku, Trans);
                    if (count == 0)
                    {
                        res.s = -3002;
                    }

                }
                #endregion

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

        public static Coresku NewMat(CoreSkuMatAuto ckm)
        {
            var sku = new Coresku();
            sku.CoID = ckm.CoID;
            sku.GoodsCode = ckm.GoodsCode;
            sku.SkuID = ckm.GoodsCode;
            sku.GoodsName = ckm.GoodsName;
            sku.Norm = ckm.Norm;
            sku.Unit = ckm.Unit;
            sku.ValUnit = ckm.ValUnit;
            sku.CnvRate = ckm.CnvRate;
            sku.PurPrice = ckm.PurPrice;
            sku.KID = ckm.KID;
            sku.KName = ckm.KName;
            sku.SCoList = ckm.SCoList;
            sku.ParentID = ckm.ParentID;
            sku.Type = 2;
            sku.Creator = ckm.Creator;
            sku.CreateDate = DateTime.Now;
            sku.IsParent = true;
            return sku;
        }

        public static List<Coresku> NewMatDetail(CoreSkuMatAuto ckm, List<CoreSkuMatItem> ckiLst)
        {
            var skuLst = new List<Coresku>();
            foreach (var cki in ckiLst)
            {
                var sku = new Coresku();
                sku.CoID = ckm.CoID;
                sku.GoodsCode = ckm.GoodsCode;
                sku.GoodsName = ckm.GoodsName;
                sku.Norm = ckm.Norm;
                sku.Unit = ckm.Unit;
                sku.ValUnit = ckm.ValUnit;
                sku.CnvRate = ckm.CnvRate;
                sku.PurPrice = ckm.PurPrice;
                sku.KID = ckm.KID;
                sku.KName = ckm.KName;
                sku.SCoList = ckm.SCoList;
                sku.SkuID = cki.SkuID;
                sku.ColorID = cki.ColorID;
                sku.ColorName = cki.ColorName;
                sku.ParentID = ckm.ParentID;
                sku.SizeID = cki.SizeID;
                sku.SizeName = cki.SizeName;
                sku.Type = 2;
                sku.Creator = ckm.Creator;
                sku.CreateDate = DateTime.Now;
                sku.IsParent = false;
                skuLst.Add(sku);
            }
            return skuLst;
        }
    }


}