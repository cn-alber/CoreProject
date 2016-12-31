using System;
using System.Collections.Generic;
using System.Linq;
using CoreModels;
using CoreModels.XyCore;
using Dapper;
using MySql.Data.MySqlClient;
using static CoreModels.Enum.OrderE;

namespace CoreData.CoreCore
{
    public static class StatisticsHaddle{

        #region 获取订单数据
        public static DataResult getOrderData(string CoID){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try
                {
                    var start = DateTime.Now.ToString("yyyy-MM-dd ")+"  00:00:00";
                    var end = DateTime.Now.ToString("yyyy-MM-dd ")+"  23:59:59 ";
                    // var start = "2016-09-03 00:00:00";
                    // var end = "2016-09-20 23:59:59 ";
                    
                    string sql = @"select o.Status, SUM(o.Status) AS Num,SUM(o.PayAmount) AS Amount from `order` as o   
                                    WHERE  o.CoID = @CoID AND o.ODate > @start AND o.ODate < @end
                                    group by o.Status DESC";
                    var states = conn.Query<orderStatic>(sql, new {
                        CoID = CoID,
                        start = start,
                        end = end
                    }).AsList();

                    var res = new List<orderStatic>();
                    for(var k=0;k<9;k++) {
                        res.Add(new orderStatic{
                            Name = ((OrdStatus)k).ToString(),
                            Status = k,
                            Num = 0,
                            Amount = 0
                        });   
                    }
                    foreach(var item in res) {
                        foreach(var i in states){
                            if(item.Status == i.Status) {
                                item.Amount = i.Amount;
                                item.Num = i.Num;
                            }
                            // i.Name = ((OrdStatus)i.Status).ToString();
                            // res.Add(i);
                        }
                    }
                    
                    result.d = res;

                }
                catch
                {
                    conn.Dispose();
                }
            }

            return result;
        }


        #endregion
        public static DataResult lastSales_7(string CoID){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try
                {
                    var start = DateTime.Now.ToString("yyyy-MM-dd ")+"  00:00:00";
                    var end = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd ")+"  23:59:59 ";
                    //var start = "2016-09-03 00:00:00";
                    //var end = "2016-09-20 23:59:59 ";
                    string sql = @"SELECT SoID FROM `order` as o WHERE o.CoID = @CoID  AND o.ODate > @start AND o.ODate < @end";
                    var ids = conn.Query<decimal>(sql, new {
                        CoID = CoID,
                        start = start,
                        end = end
                    }).AsList();
                    if(ids.Count == 0){
                        result.d = null;
                    }else {
                        sql = @"select i.SkuID ,SUM(i.Qty) AS Qty from orderitem as i  WHERE i.CoID = @CoID AND i.SoID in ("+string.Join(",",ids.ToArray())+") group by i.SkuID ORDER BY Qty DESC";
                        var res = conn.Query<lastday_7>(sql, new {
                            CoID = CoID
                        }).AsList();
                        res.Add(new lastday_7{SkuID="其他", Qty = 0});
                        var last = res[res.Count-1];
                        for(var i=0; i<res.Count;i++){
                            if(i >5 && res[i].SkuID != "其他"){
                                last.Qty += res[i].Qty;
                            }
                        }
                        result.d = res.OrderByDescending(a => a.Qty).Take(5).ToList();
                    }                                                                         
                }
                catch (Exception ex)
                {
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }

            return result;
        }


        public static DataResult lastAreaSale_7(string CoID){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try
                {
                    var start = DateTime.Now.ToString("yyyy-MM-dd ")+"  00:00:00";
                    var end = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd ")+"  23:59:59 ";
                    string sql = @"SELECT o.SoID,o.RecLogistics FROM `order` as o WHERE o.CoID = @CoID  AND o.ODate > @start AND o.ODate < @end";
                    var list = conn.Query<AreaSale>(sql, new {
                        CoID = CoID,
                        start = start,
                        end = end
                    }).AsList();
                    if(list.Count == 0){
                        result.d = null;
                    }else {
                        var ids = new List<decimal>();
                        foreach(var item in list){
                            ids.Add(item.SoID);
                        }
                        sql = @"select i.SoID ,i.Amount from orderitem as i  WHERE i.CoID = @CoID AND i.SoID in ("+string.Join(",",ids.ToArray())+")";
                        var res = conn.Query<areaItem>(sql, new {
                            CoID = CoID
                        }).AsList();

                        foreach(var item in list){
                            foreach(var i in res){
                                if(item.SoID == i.SoID) {
                                    item.Amount = i.Amount;
                                    res.Remove(i);
                                    break;
                                }
                            }
                        }
                        var data =  list.GroupBy(a => a.RecLogistics).Select(g => (new {RecLogistics = g.Key, TotalAmount = g.Sum(item => item.Amount) }));  
                        result.d = data;
                    }                                         
                }
                catch (Exception ex)
                {
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }

            return result;
        }

        public static DataResult lastSales_15(string CoID){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try
                {
                    // var start = DateTime.Now.ToString("yyyy-MM-dd ")+"  00:00:00";
                    // var end = DateTime.Now.AddDays(-15).ToString("yyyy-MM-dd ")+"  23:59:59 ";
                    var start = "2016-09-03 00:00:00";
                    var end = "2016-09-20 23:59:59 ";
                    string sql = @"SELECT date_format(o.ODate,'%y%m%d')  AS D, o.PayAmount AS Pay , SoID FROM `order` as o WHERE o.CoID = @CoID  AND o.ODate > @start AND o.ODate < @end";
                    var list = conn.Query<lastday_15>(sql, new {
                        CoID = CoID,
                        start = start,
                        end = end
                    }).AsList();
                    if(list.Count == 0){
                        result.d = null;
                    }else {
                        var ids = new List<decimal>();
                        foreach(var item in list){
                            ids.Add(item.SoID);
                        }
                        sql = @"select i.SoID ,i.Qty AS Amount from orderitem as i  WHERE i.CoID = @CoID AND i.SoID in ("+string.Join(",",ids.ToArray())+")";
                        var res = conn.Query<areaItem>(sql, new {
                            CoID = CoID
                        }).AsList();
                        foreach(var item in list){
                            foreach(var i in res){
                                if(item.SoID == i.SoID) {
                                    item.Qty = int.Parse(i.Amount.ToString());
                                    res.Remove(i);
                                    break;
                                }
                            }
                        }
                        var data =  list.GroupBy(a => a.D).Select(g => (new {D = g.Key,Qty =g.Sum(item => item.Qty) , Pay = g.Sum(item => item.Pay) })); 
                        result.d = data;
                    }                                                                         
                }
                catch (Exception ex)
                {
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }

            return result;
        }






    }
}