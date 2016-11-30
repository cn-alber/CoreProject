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
        /// 判断条码属于=>0.件码(唯一码)||1.普通Sku||2.箱码
        /// </summary>
        public static DataResult GetType(ASkuScanParam IParam)
        {
            var result = new DataResult(1, null);
            using (var CoreConn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    // int iCount = 0;
                    string querysql = "SELECT ID AS Skuautoid,SkuID,SkuName,GoodsCode,Norm,@BarCode AS BarCode FROM coresku WHERE CoID=@CoID AND SkuID=@SkuID ORDER BY IsDelete";
                    // string skucountsql = "SELECT COUNT(ID) FROM coresku WHERE CoID=@CoID AND SkuID=@SkuID";
                    string boxcountsql = "SELECT BarCode,Skuautoid,SkuID,BoxCode,SUM(Qty) AS Qty FROM wmsbox WHERE CoID=@CoID AND BoxCode = @BoxCode";
                    string SkuID = IParam.BarCode.Substring(0, IParam.BarCode.Length > 6 ? IParam.BarCode.Length - 6 : IParam.BarCode.Length);
                    var data = new ASkuScan();
                    var Lst = CoreConn.Query<ASkuScan>(querysql, new { CoID = IParam.CoID, SkuID = SkuID, BarCode = IParam.BarCode }).AsList();
                    if (Lst.Count > 0)//判断是否属于0.件码(唯一码)
                    {
                        Lst[0].SkuType = 0;
                        data = Lst[0];
                    }
                    else
                    {
                        Lst = CoreConn.Query<ASkuScan>(querysql, new { CoID = IParam.CoID, SkuID = IParam.BarCode, BarCode = IParam.BarCode }).AsList();
                        if (Lst.Count > 0)//判断是否属于1.普通Sku
                        {
                            Lst[0].SkuType = 1;
                            data = Lst[0];
                        }
                        else
                        {
                            Lst = CoreConn.Query<ASkuScan>(boxcountsql, new { CoID = IParam.CoID, BoxCode = IParam.BarCode }).AsList();
                            if (Lst.Count > 0)//判断是否属于箱码（2）
                            {
                                SkuID = Lst[0].SkuID;
                                var sku = CoreConn.Query<ASkuScan>(querysql, new { CoID = IParam.CoID, SkuID = SkuID,BarCode = IParam.BarCode}).AsList();                                
                                Lst = Lst.Select(a=>new ASkuScan{
                                    BarCode = IParam.BarCode,
                                    Skuautoid = a.Skuautoid,
                                    SkuID = a.SkuID,
                                    SkuName = sku.Count>0?sku[0].SkuName:"",
                                    GoodsCode = sku.Count>0?sku[0].GoodsCode:"",
                                    Norm = sku.Count>0?sku[0].Norm:"",
                                    Qty = a.Qty,
                                    // BoxCode = a.BoxCode
                                }).AsList();
                                Lst[0].SkuType = 2;
                                data = Lst[0];
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
                    result.s = -1;
                    result.d = e.Message;
                }
            }
            return result;
        }
    }
}
