using System.Collections.Generic;
using System.Linq;
using CoreModels;
using CoreModels.XyUser;
using CoreModels.XyComm;
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
        #region 重装箱作业-装箱条码检查
        public static DataResult CheckBarCode(WmsBoxParams IParam)
        {
            var result = new DataResult(1, null);
            var Conn = new MySqlConnection(DbBase.CoreConnectString);
            Conn.Open();
            try
            {
                var res = AUserHaddle.GetUniqCode(IParam.CoID.ToString());//是否启用唯一码
                if (res.s == 1)
                {
                    var uniq = Convert.ToInt32(res.d);
                    if (uniq == 1)//启用唯一码，条码重复装箱卡关
                    {
                        int count = Conn.QueryFirst<int>("select count(ID) from wmsbox where CoID=@CoID and BarCode=@BarCode", new { CoID = IParam.CoID, BarCode = IParam.BarCode });
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
                Conn.Close();
            }

            return result;
        }
        #endregion
        #region 重装箱作业-产生装箱资料WmsBox
        public static DataResult AddWmsBox(ApiBoxParam IParam)
        {
            var res = new DataResult(1, null);
            var CoreConn = new MySqlConnection(DbBase.CoreConnectString);
            CoreConn.Open();
            var CoreTrans = CoreConn.BeginTransaction();
            try
            {
                if (IParam.BoxSkuLst.Count > 0)
                {
                    string BoxCode = IParam.Code + CommHaddle.GetRecordID(IParam.CoID);
                    var BoxLst = IParam.BoxSkuLst.Select(a => new AWmsBox
                    {
                        BarCode = a.BarCode,
                        Skuautoid = a.Skuautoid,
                        SkuID = a.SkuID,
                        BoxCode = BoxCode,
                        Qty = a.Qty,
                        CoID = IParam.CoID,
                        Creator = IParam.Creator,
                        CreateDate = IParam.CreateDate
                    }).AsList();//装箱明细
                    var logLst = IParam.BoxSkuLst.Select(a => new AWmslog
                    {
                        BarCode = a.BarCode,
                        Skuautoid = a.Skuautoid,
                        SkuID = a.SkuID,
                        BoxCode = BoxCode,
                        Qty = a.Qty,
                        Contents = IParam.Contents,
                        CoID = IParam.CoID,
                        Creator = IParam.Creator,
                        CreateDate = IParam.CreateDate
                    }).AsList();//操作明细
                                //生成装箱单&操作记录
                    int count1 = CoreConn.Execute(AddBoxSql(), BoxLst, CoreTrans);
                    int count2 = CoreConn.Execute(AddWmsLogSql(), logLst, CoreTrans);
                    if (count1 < 1 || count2 < 1)
                    {
                        res.s = -3002;
                    }
                    else
                    {
                        CoreTrans.Commit();
                        res.d = BoxCode;
                    }
                }
            }
            catch (Exception e)
            {
                CoreTrans.Rollback();
                res.s = -1;
                res.d = e.Message;
            }
            finally
            {
                CoreTrans.Dispose();
                CoreConn.Close();
            }
            return res;
        }
        #endregion

        #region 大单装箱-装箱条码检查
        public static DataResult CheckBarCodeBig(WmsBoxParams IParam)
        {
            var result = new DataResult(1, null);
            var Conn = new MySqlConnection(DbBase.CoreConnectString);
            Conn.Open();
            try
            {
                var res = AUserHaddle.GetUniqCode(IParam.CoID.ToString());//是否启用唯一码
                if (res.s == 1)
                {
                    var uniq = Convert.ToInt32(res.d);
                    if (uniq == 1)//启用唯一码，条码重复装箱卡关
                    {
                        int count = Conn.QueryFirst<int>("select count(ID) from wmsbox where CoID=@CoID and BarCode=@BarCode and SUBSTRING(BoxCode,1,2)='BG'", new { CoID = IParam.CoID, BarCode = IParam.BarCode });
                        if (count > 0)
                        {
                            result.s = -6001;//此条码已装箱
                        }
                    }

                    if (result.s == 1)
                    {
                        string sql = @"SELECT ID,
                                    BatchID,
                                    OID,
                                    SoID,
                                    BarCode,
                                    Skuautoid,
                                    Qty,
                                    `Status`,
                                    OutID
                                    BatchtaskID,
                                    SortCode 
                                FROM batchpicked
                                WHERE CoID=@CoID 
                                AND BarCode=@BarCode";
                        var PickLst = Conn.Query<ABatchPicked>(sql, new { CoID = IParam.CoID, BarCode = IParam.BarCode }).AsList();
                        if (PickLst.Count <= 0)
                        {
                            result.s = -6020;//请先执行拣货任务
                        }
                        else
                        {
                            var PLst = PickLst.Where(a => a.OutID == 0).AsList();
                            if (PLst.Count <= 0)
                            {
                                result.s = -6016;//重复发货
                            }
                            else
                            {
                                var cp = new ASkuScanParam();
                                cp.CoID = IParam.CoID;
                                cp.BarCode = IParam.BarCode;
                                result = ASkuScanHaddles.GetType(cp);
                                if (result.s > 0)
                                {
                                }
                            }
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
                Conn.Close();
            }

            return result;
        }
        #endregion


        #region 新增装箱资料
        public static string AddBoxSql()
        {
            string sql = @"
                    INSERT INTO wmsbox(
                        BarCode,
                        Skuautoid,
                        SkuID,
                        BoxCode,
                        Qty,
                        CoID,
                        Creator,
                        CreateDate)
                    VALUES (
                        @BarCode,
                        @Skuautoid,
                        @SkuID,
                        @BoxCode,
                        @Qty,
                        @CoID,
                        @Creator,
                        @CreateDate)";
            return sql;
        }
        #endregion

        #region 新增装箱操作记录

        public static string AddWmsLogSql()
        {
            string sql = @"INSERT INTO wmslog(BarCode,
                                            Skuautoid,
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
                                            @Skuautoid,
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