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
                        res.s=-1;
                        res.d="无效条码";
                    }
                    else
                    res.d=Lst;
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
            
            try
            {
                IParam.BoxCode="BX"+CommHaddle.GetRecordID(IParam.CoID);

                var BoxLst = NewWmsboxLst(IParam);

            }
            catch (Exception e)
            {
                res.s = -1;
                res.d = e.Message;
            }
            finally
            {
                DbBase.GoodsDB.Close();               
            }
            return res;
        }
        #endregion

        public static List<AWmsBox> NewWmsboxLst(WmsBoxParams IParam)
        {
            var boxList = new List<AWmsBox>();
            foreach (var IParm in IParam.ABarCodeLst)
            {
                var box = new AWmsBox();
                box.BarCode = IParm;
                box.BoxCode = IParam.BoxCode;
                box.SkuID = IParm.Substring(0,IParm.Length - 6);
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

    }
}