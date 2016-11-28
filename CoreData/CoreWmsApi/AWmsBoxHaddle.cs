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
            var result = new DataResult(1, null);
            var CommConn = new MySqlConnection(DbBase.CommConnectString);
            CommConn.Open();
            try
            {
                var res = AUserHaddle.GetUniqCode(IParam.CoID.ToString());
                if (res.s == 1)
                {
                    var uniq = Convert.ToInt32(res.d);
                    if (uniq == 1)
                    {
                        int count = CommConn.QueryFirst<int>("select count(ID) from wmsbox where CoID=@CoID and BarCode=@BarCode", new { CoID = IParam.CoID, BarCode = IParam.BarCode });
                        if (count > 0)
                        {
                            result.s = -6001;//此条码已装箱
                        }
                    }
                    if (result.s == 1)
                    {
                        var cp = new ASkuScanParam();
                        cp.CoID = IParam.CoID;
                        cp.BarCode = IParam.BarCode;
                        result = ASkuScanHaddles.GetType(cp);//获取条码资料
                    }
                }
            }
            catch (Exception e)
            {
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                CommConn.Close();
            }

            return result;
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