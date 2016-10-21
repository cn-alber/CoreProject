using System.Collections.Generic;
using System.Linq;
using CoreModels;
using CoreModels.XyUser;
using CoreModels.XyComm;
using CoreModels.XyMessage;
using CoreModels.WmsApi;
using CoreData.CoreComm;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Text;

namespace CoreData.CoreWmsApi
{
    public static class AWmsBoxHaddle
    {
        #region Api Check BarCode 重装箱作业-装箱条码检查
        public static DataResult CheckBarCode(WmsBoxParams IParam)
        {
            var res = new DataResult(1, null);
            try
            {
                int count = DbBase.GoodsDB.QueryFirst<int>("select count(ID) from wmsbox where CoID=@CoID and BarCode=@BarCode", new { CoID = IParam.CoID, BarCode = IParam.BarCode });
                if (count > 0)
                {
                    res.s = -1;
                    res.d = "此条码已装箱";
                }
                else
                {
                    var Lst = DbBase.CoreDB.Query<ACoreSku>("select * from coresku where CoID=@CoID and SkuID=@SkuID", new { CoID = IParam.CoID, SkuID = IParam.SkuID }).AsList();
                    if (Lst.Count == 0)
                    {
                        res.s = -1;
                        res.d = "无效条码";
                    }
                    else
                        res.d = Lst;
                }

            }
            catch (Exception e)
            {
                res.s = -1;
                res.d = e.Message;
            }
            finally
            {
                DbBase.GoodsDB.Close();
                DbBase.CoreDB.Close();
            }
            return res;
        }
        #endregion
        #region 重装箱作业-产生装箱资料WmsBox
        public static DataResult AddWmsBox(WmsBoxParams IParam)
        {
            var res = new DataResult(1, null);
            var GoodsConn = new MySqlConnection(DbBase.GoodsConnectString);
            var MsgConn = new MySqlConnection(DbBase.MsgConnectString);
            GoodsConn.Open();
            MsgConn.Open();
            var GoodsTrans = GoodsConn.BeginTransaction();
            var MsgTrans = MsgConn.BeginTransaction();
            try
            {
                IParam.BoxCode = "BX" + CommHaddle.GetRecordID(IParam.CoID);
                var BoxLst = NewWmsboxLst(IParam);
                int count = GoodsConn.Execute(AddBoxSql(), BoxLst, GoodsTrans);
                if (count < 1)
                {
                    res.s = -3002;
                }
                else
                {
                    IParam.WmsboxLst = BoxLst;
                    IParam.Contents = "扫描装箱";
                    MsgConn.Execute(AddWorkLogSql(), InitBoxWorklog(IParam, 1), MsgTrans);
                    GoodsTrans.Commit();
                    MsgTrans.Commit();
                }
            }
            catch (Exception e)
            {
                GoodsTrans.Rollback();
                MsgTrans.Rollback();
                res.s = -1;
                res.d = e.Message;
            }
            finally
            {
                GoodsTrans.Dispose();
                MsgTrans.Dispose();
                GoodsConn.Close();
                MsgConn.Close();
            }
            return res;
        }
        #endregion

        #region 普通收货作业-获取Sku详情
        public static DataResult GetBoxSku(WmsBoxParams IParam)
        {
            var res = new DataResult(1, null);
            try
            {
                IParam.IsBox = IsBoxTF(IParam.SkuID, IParam.CoID);//检查参数是箱码or件码
                if (IsExist(IParam.IsBox, IParam.Type, IParam.BoxCode, IParam.CoID))
                {
                    res.s = -1;
                    res.d = "重复扫描";
                }
                else
                {
                    res = GetBoxSkuDetail(IParam);//获取Sku信息                   
                }
            }
            catch (Exception e)
            {
                res.s = -1;
                res.d = e.Message;
            }
            finally
            {
                DbBase.GoodsDB.Close();
                DbBase.CoreDB.Close();
            }
            return res;
        }
        #endregion














        #region 检查参数是箱码or件码
        public static Boolean IsBoxTF(string SkuID, int CoID)
        {
            Boolean IsBox = true;
            int count = DbBase.CoreDB.QueryFirst<int>("SELECT count(ID) FROM coresku WHERE SkuID=@SkuID AND CoID=@CoID", new { SkuID = SkuID, CoID = CoID });
            if (count > 0)
                IsBox = false;
            return IsBox;
        }
        #endregion
        #region 判断箱体是否已收入or已上架
        public static Boolean IsExist(Boolean IsBox, int Type, string BoxCode, int CoID)
        {
            Boolean IsExist = true;
            string sql = "SELECT IFNULL(SUM(qty),0) FROM worklog WHERE CoID=@CoID AND Type=@Type";
            if (IsBox)
            {
                sql = sql + " AND BoxCode=@BoxCode";
            }
            else
            {
                sql = sql + " AND BarCode=@BoxCode";
            }
            var p = new DynamicParameters();
            p.Add("@CoID", CoID);
            p.Add("@Type", Type);
            p.Add("@BoxCode", BoxCode);
            var Qty = DbBase.MsgDB.QueryFirst<int>(sql, p);
            if (Qty <= 0)
                IsExist = false;
            return IsExist;
        }
        #endregion
        #region 根据箱号取Sku
        public static DataResult GetBoxSkuDetail(WmsBoxParams IParam)
        {
            var res = new DataResult(1, null);
            int Qty = 1;
            try
            {
                if (IParam.IsBox)
                {
                    Qty = DbBase.GoodsDB.QueryFirst<int>("SELECT count(ID) FROM wmsbox WHERE CoID=@CoID AND BoxCode=@BoxCode", new { CoID = IParam.CoID, BoxCode = IParam.BoxCode });
                    IParam.SkuID = DbBase.GoodsDB.QueryFirst<string>("SELECT SkuID FROM wmsbox WHERE CoID=@CoID AND BoxCode=@BoxCode LIMIT 1", new { CoID = IParam.CoID, BoxCode = IParam.BoxCode });
                }
                string sql = @"SELECT ID,
                                    SkuID,
                                    GoodsCode,
                                    ColorName,
                                    SizeName
                               FROM coresku 
                               WHERE CoID=@CoID AND SkuID=@SkuID";
                var ACore = DbBase.CoreDB.QueryFirst<ACoreSku>(sql, new { CoID = IParam.CoID, SkuID = IParam.SkuID });
                if (ACore != null)
                {
                    ACore.Qty = Qty;
                    ACore.IsBox = IParam.IsBox;
                    res.d = ACore;
                }
                else
                {
                    res.s=-1;
                    res.d="无效Sku";
                }
            }
            catch (Exception e)
            {
                res.s = -1;
                res.d = e.Message;
            }
            finally
            {
                DbBase.GoodsDB.Close();
                DbBase.CoreDB.Close();
            }
            return res;
        }
        #endregion

        #region 新增装箱资料
        public static List<AWmsBox> NewWmsboxLst(WmsBoxParams IParam)
        {
            var boxList = new List<AWmsBox>();
            foreach (var IParm in IParam.ABarCodeLst)
            {
                var box = new AWmsBox();
                box.BarCode = IParm;
                box.BoxCode = IParam.BoxCode;
                box.SkuID = IParm.Substring(0, IParm.Length - 6);
                box.Creator = IParam.Creator;
                box.CreateDate = DateTime.Now;
                box.CoID = IParam.CoID;
                boxList.Add(box);
            }
            return boxList;
        }

        public static string AddBoxSql()
        {
            string sql = @"
                    INSERT INTO wmsbox(
                        BarCode,
                        SkuID,
                        BoxCode,
                        CoID,
                        Creator,
                        CreateDate)
                    VALUES (
                        @BarCode,
                        @SkuID,
                        @BoxCode,
                        @CoID,
                        @Creator,
                        @CreateDate)";
            return sql;
        }
        #endregion

        #region 新增装箱操作记录
        public static List<Worklog> InitBoxWorklog(WmsBoxParams param, int i)
        {
            var logLst = new List<Worklog>();
            foreach (var IParm in param.WmsboxLst)
            {
                var log = new Worklog();
                log.CoID = IParm.CoID;
                log.BarCode = IParm.BarCode;
                log.BoxCode = param.BoxCode;
                log.SkuID = IParm.SkuID;
                log.WarehouseID = param.WarehouseID;
                log.qty = 1 * i;
                log.Type = param.Type;
                log.RecordID = param.RecordID;
                log.Contents = param.Contents;
                log.Creator = param.Creator;
                log.CreateDate = DateTime.Now;
                logLst.Add(log);
            }
            return logLst;
        }

        public static string AddWorkLogSql()
        {
            string sql = @"INSERT INTO worklog(BarCode,
                                            SkuID,
                                            BoxCode,
                                            WarehouseID,
                                            qty,
                                            PCode,
                                            Contents,
                                            Type,
                                            CoID,
                                            Creator,
                                            CreateDate)
                                      VALUES (@BarCode,
                                            @SkuID,
                                            @BoxCode,
                                            @WarehouseID,
                                            @qty,
                                            @PCode,
                                            @Contents,
                                            @Type,
                                            @CoID,
                                            @Creator,
                                            @CreateDate )";
            return sql;
        }
        #endregion
    }
}