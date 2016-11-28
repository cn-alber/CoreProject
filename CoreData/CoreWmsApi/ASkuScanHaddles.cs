using System.Collections.Generic;
using System.Linq;
using CoreModels;
using CoreModels.WmsApi;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Text;

namespace CoreData.CoreWmsApi
{

    public static class ASkuScanHaddles
    {
        /// <summary>
        /// 判断条码属于箱码or件码唯一码or普通Sku
        /// </summary>
        public static DataResult GetType(ASkuScanParam IParam)
        {
            var result = new DataResult(1, null);
            var CoreConn = new MySqlConnection(DbBase.CoreConnectString);
            var CommConn = new MySqlConnection(DbBase.CommConnectString);
            CoreConn.Open();
            CommConn.Open();
            var CoreTrans = CoreConn.BeginTransaction();
            var CommTrans = CommConn.BeginTransaction();
            try
            {
                int iCount = 0;
                string querysql = "SELECT ID,SkuID,SkuName,GoodsCode,Norm FROM coresku WHERE CoID=@CoID AND SkuID=@SkuID ORDER BY IsDelete";
                string skucountsql = "SELECT COUNT(ID) FROM coresku WHERE CoID=@CoID AND SkuID=@SkuID";
                string boxcountsql = "SELECT SkuID FROM wmsbox WHERE CoID=@CoID AND BoxCode = @BoxCode";
                string SkuID = IParam.BarCode.Substring(0, IParam.BarCode.Length > 6 ? IParam.BarCode.Length - 6 : IParam.BarCode.Length);
                iCount = CoreConn.QueryFirst<int>(skucountsql, new { CoID = IParam.CoID, SkuID = SkuID });
                var data = new ASkuScan();
                if (iCount > 0)//判断是否属于件码（0）
                {
                    data = CoreConn.QueryFirst<ASkuScan>(querysql, new { CoID = IParam.CoID, SkuID = SkuID });
                    data.SkuType = 0;
                }
                else
                {
                    iCount = CoreConn.QueryFirst<int>(skucountsql, new { CoID = IParam.CoID, SkuID = IParam.BarCode });
                    if (iCount > 0)//判断是否属于普通Sku（1）
                    {
                        data = CoreConn.QueryFirst<ASkuScan>(querysql, new { CoID = IParam.CoID, SkuID = IParam.BarCode });
                        data.SkuType = 1;
                    }
                    else
                    {
                        var Lst = CommConn.Query<string>(boxcountsql, new { CoID = IParam.CoID, BoxCode = IParam.BarCode }).AsList();
                        if (Lst.Count > 0)//判断是否属于箱码（2）
                        {
                            SkuID = Lst[0];
                            data = CoreConn.QueryFirst<ASkuScan>(querysql, new { CoID = IParam.CoID, SkuID = SkuID });
                            data.SkuType = 2;
                        }
                        else
                        {
                            result.s = -6000;//无效条码
                        }
                    }
                }
                result.d = data;
            }
            catch (Exception e)
            {
                CoreTrans.Rollback();
                CommTrans.Rollback();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                CoreTrans.Dispose();
                CoreTrans.Dispose();
                CoreConn.Close();
                CommConn.Close();
            }
            return result;
        }
    }
}
