using CoreModels;
using MySql.Data.MySqlClient;
using System;
using CoreModels.XyCore;
using Dapper;
using System.Collections.Generic;
using CoreModels.XyComm;
using static CoreModels.Enum.OrderE;
using CoreData.CoreUser;
using CoreModels.XyUser;
namespace CoreData.CoreCore
{
    public static class PayinfoHaddle
    {
        ///<summary>
        ///查询支付单List
        ///</summary>
        // public static DataResult GetPayinfoList(PayInfoParm cp)
        // {
        //     var result = new DataResult(1,null);    
        //     string sqlcount = "select count(id) from payinfo where 1=1";
        //     string sqlcommand = @"select ID,PayDate,OID,SoID,PayNbr,PayAmount,Status,Payment,PayAccount,BuyerShopID from payinfo where 1=1";         
        //     string wheresql = string.Empty;
        //     if(cp.CoID != 1)//公司编号
        //     {
        //         wheresql = wheresql + " and coid = " + cp.CoID;
        //     }
        //     if(cp.ID > 0)//内部订单号
        //     {
        //         wheresql = wheresql + " and id = " + cp.ID;
        //     }
        //     if(cp.SoID > 0)//外部订单号
        //     {
        //         wheresql = wheresql + " and soid = " + cp.SoID;
        //     }
        //     if(!string.IsNullOrEmpty(cp.PayNbr))//付款单号
        //     {
        //        wheresql = wheresql + " and paynbr = '" + cp.PayNbr + "'";
        //     }
        //     if(!string.IsNullOrEmpty(cp.BuyerShopID))//买家账号
        //     {
        //        wheresql = wheresql + " and buyershopid like '%" + cp.BuyerShopID + "%'";
        //     }
        //     if(!string.IsNullOrEmpty(cp.ExCode))//快递单号
        //     {
        //        wheresql = wheresql + " and excode like '%" + cp.ExCode + "%'";
        //     }
        //     if(!string.IsNullOrEmpty(cp.RecName))//收货人
        //     {
        //        wheresql = wheresql + " and recname like '%" + cp.RecName + "%'";
        //     }
        //     if(!string.IsNullOrEmpty(cp.RecPhone))//手机
        //     {
        //        wheresql = wheresql + " and recphone like '%" + cp.RecPhone + "%'";
        //     }
        //     if(!string.IsNullOrEmpty(cp.RecTel))//固定电话
        //     {
        //        wheresql = wheresql + " and rectel like '%" + cp.RecTel + "%'";
        //     }
        //     if(!string.IsNullOrEmpty(cp.RecLogistics))//收货人省
        //     {
        //        wheresql = wheresql + " and reclogistics like '%" + cp.RecLogistics + "%'";
        //     }
        //     if(!string.IsNullOrEmpty(cp.RecCity))//收货人城市
        //     {
        //        wheresql = wheresql + " and reccity like '%" + cp.RecCity + "%'";
        //     }
        //     if(!string.IsNullOrEmpty(cp.RecDistrict))//收货人县区
        //     {
        //        wheresql = wheresql + " and recdistrict like '%" + cp.RecDistrict + "%'";
        //     }
        //     if(!string.IsNullOrEmpty(cp.RecAddress))//详细地址
        //     {
        //        wheresql = wheresql + " and recaddress like '%" + cp.RecAddress + "%'";
        //     }
        //     if (cp.StatusList != null)//状态List
        //     {
        //         wheresql = wheresql + " AND status in ("+ string.Join(",", cp.StatusList) + ")" ;
        //     }
        //     if (cp.AbnormalStatusList != null)//异常状态List
        //     {
        //         string status = " AND abnormalstatus in (0,"+ string.Join(",", cp.AbnormalStatusList) + ")" ;
        //         if(cp.StatusList != null)
        //         {
        //             if(cp.StatusList.Count == 1 && cp.StatusList[0] == 7)
        //             {
        //                 status = " AND abnormalstatus in ("+ string.Join(",", cp.AbnormalStatusList) + ")" ;
        //             }
        //         }
        //         else
        //         {
        //             status = " AND abnormalstatus in ("+ string.Join(",", cp.AbnormalStatusList) + ")" ;
        //         }
        //         wheresql = wheresql + status ;
        //     }
        //     if(cp.IsRecMsgYN.ToUpper() == "Y")
        //     {
        //         if(cp.RecMessage == null)
        //         {
        //             wheresql = wheresql + " AND recmessage != '' and status in (0,1,2,7)" ;
        //         }
        //         else
        //         {
        //             wheresql = wheresql + " AND recmessage like '%" + cp.RecMessage + "%' and status in (0,1,2,7)" ;
        //         }
        //     }
        //     if(cp.IsRecMsgYN.ToUpper() == "N")
        //     {
        //         wheresql = wheresql + " AND recmessage = '' and status in (0,1,2,7)" ;
        //     }
        //     if(cp.IsSendMsgYN.ToUpper() == "Y")
        //     {
        //         if(cp.SendMessage == null)
        //         {
        //             wheresql = wheresql + " AND sendmessage != '' and status in (0,1,2,7)" ;
        //         }
        //         else 
        //         {
        //             wheresql = wheresql + " AND sendmessage like '%" + cp.SendMessage + "%' and status in (0,1,2,7)" ;
        //         }
        //     }
        //     if(cp.IsSendMsgYN.ToUpper() == "N")
        //     {
        //         wheresql = wheresql + " AND sendmessage = '' and status in (0,1,2,7)" ;
        //     }
        //     if(cp.Datetype.ToUpper() == "ODATE")
        //     {
        //         wheresql = wheresql + " AND odate between '" + cp.DateStart + "' and '" + cp.DateEnd + "'" ;
        //     }
        //     if(cp.Datetype.ToUpper() == "SENDDATE")
        //     {
        //         wheresql = wheresql + " AND senddate between '" + cp.DateStart + "' and '" + cp.DateEnd + "'" ;
        //     }
        //     if(cp.Datetype.ToUpper() == "PAYDATE")
        //     {
        //         wheresql = wheresql + " AND paydate between '" + cp.DateStart + "' and '" + cp.DateEnd + "'" ;
        //     }
        //     if(cp.Datetype.ToUpper() == "PLANDATE")
        //     {
        //         wheresql = wheresql + " AND plandate between '" + cp.DateStart + "' and '" + cp.DateEnd + "'" ;
        //     }
        //     if(!string.IsNullOrEmpty(cp.Skuid))
        //     {
        //        wheresql = wheresql + " and exists(select id from orderitem where oid = order.id and skuid = '" + cp.Skuid + "')";
        //     }
        //     if(!string.IsNullOrEmpty(cp.GoodsCode))
        //     {
        //        wheresql = wheresql + " and exists(select id from orderitem where oid = order.id and GoodsCode = '" + cp.Skuid + "')";
        //     }
        //     if(cp.Ordqtystart > 0)
        //     {
        //         wheresql = wheresql + " AND ordqty >= " +  cp.Ordqtystart + " and status in (0,1,2,7)";
        //     }
        //     if(cp.Ordqtyend > 0)
        //     {
        //         wheresql = wheresql + " AND ordqty <= " +  cp.Ordqtyend + " and status in (0,1,2,7)";
        //     }
        //     if(cp.Ordamtstart > 0)
        //     {
        //         wheresql = wheresql + " AND amount >= " +  cp.Ordamtstart + " and status in (0,1,2,7)";
        //     }
        //     if(cp.Ordamtend > 0)
        //     {
        //         wheresql = wheresql + " AND amount <= " +  cp.Ordamtend + " and status in (0,1,2,7)";
        //     }
        //     if(!string.IsNullOrEmpty(cp.Skuname))
        //     {
        //        wheresql = wheresql + " and exists(select id from orderitem where oid = order.id and skuname like '%" + cp.Skuname + "%') and status in (0,1,2,7)";
        //     }
        //     if(!string.IsNullOrEmpty(cp.Norm))
        //     {
        //        wheresql = wheresql + " and exists(select id from orderitem where oid = order.id and norm like '%" + cp.Norm + "%') and status in (0,1,2,7)";
        //     }
        //     if(cp.ShopStatus != null)
        //     {
        //         string shopstatus = string.Empty;
        //         foreach(var x in cp.ShopStatus)
        //         {
        //             shopstatus = shopstatus + "'" + x + "',";
        //         }
        //         shopstatus = shopstatus.Substring(0,shopstatus.Length - 1);
        //         wheresql = wheresql + " AND shopstatus in (" +  shopstatus + ")";
        //     }
        //     if(cp.OSource > -1)
        //     {
        //         wheresql = wheresql + " AND osource = " +  cp.OSource;
        //     }
        //     if (cp.Type != null)
        //     {
        //         wheresql = wheresql + " AND type in ("+ string.Join(",", cp.Type) + ")" ;
        //     }
        //     if(cp.IsCOD.ToUpper() == "Y")
        //     {
        //         wheresql = wheresql + " AND iscod = true" ;
        //     }
        //     if(cp.IsCOD.ToUpper() == "N")
        //     {
        //         wheresql = wheresql + " AND iscod = false" ;
        //     }
        //     if(cp.IsPaid.ToUpper() == "Y")
        //     {
        //         wheresql = wheresql + " AND IsPaid = true" ;
        //     }
        //     if(cp.IsPaid.ToUpper() == "N")
        //     {
        //         wheresql = wheresql + " AND IsPaid = false" ;
        //     }
        //     if (cp.IsShopSelectAll == false &&　cp.ShopID != null)
        //     {
        //         wheresql = wheresql + " AND shopid in ("+ string.Join(",", cp.ShopID) + ")" ;
        //     }
        //     if(cp.IsDisSelectAll == true)
        //     {
        //         wheresql = wheresql + " AND dealertype = 2" ;
        //     }
        //     else
        //     {
        //         if(cp.Distributor != null)
        //         {
        //             string distributor = string.Empty;
        //             foreach(var x in cp.Distributor)
        //             {
        //                 distributor = distributor + "'" + x + "',";
        //             }
        //             distributor = distributor.Substring(0,distributor.Length - 1);
        //             wheresql = wheresql + " AND dealertype = 2 AND distributor in (" +  distributor + ")";
        //         }
        //     }
        //     if(cp.ExID != null)
        //     {
        //         wheresql = wheresql + " AND exid in ("+ string.Join(",", cp.ExID) + ")" ;
        //     }
        //     if(cp.SendWarehouse != null)
        //     {
        //         wheresql = wheresql + " AND WarehouseID in ("+ string.Join(",", cp.SendWarehouse) + ")" ;
        //     }
        //     if(cp.Others != null)
        //     {
        //         if(cp.Others.Contains(4))
        //         {
        //             wheresql = wheresql + " and IsInvoice = true";
        //         }
        //         if(cp.Others.Contains(0) == true &&　cp.Others.Contains(0) == false)
        //         {
        //             wheresql = wheresql + " and IsMerge = true";
        //         }
        //         if(cp.Others.Contains(0) == false &&　cp.Others.Contains(0) == true)
        //         {
        //             wheresql = wheresql + " and IsMerge = false";
        //         }
        //         if(cp.Others.Contains(1) == true &&　cp.Others.Contains(3) == false)
        //         {
        //             wheresql = wheresql + " and IsSplit = true";
        //         }
        //         if(cp.Others.Contains(1) == false &&　cp.Others.Contains(3) == true)
        //         {
        //             wheresql = wheresql + " and IsSplit = false";
        //         }
        //     }
        //     if(!string.IsNullOrEmpty(cp.SortField) && !string.IsNullOrEmpty(cp.SortDirection))//排序
        //     {
        //         wheresql = wheresql + " ORDER BY "+cp.SortField +" "+ cp.SortDirection;
        //     }
        //     var res = new OrderData();
        //     using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
        //         try{    
        //             int count = conn.QueryFirst<int>(sqlcount + wheresql);
        //             decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(cp.NumPerPage.ToString()));
        //             int dataindex = (cp.PageIndex - 1)* cp.NumPerPage;
        //             wheresql = wheresql + " limit " + dataindex.ToString() + " ," + cp.NumPerPage.ToString();
        //             var u = conn.Query<OrderQuery>(sqlcommand + wheresql).AsList();
        //             res.Datacnt = count;
        //             res.Pagecnt = pagecnt;
        //             res.Ord = u;
        //             //订单资料
        //             List<int> ItemID = new List<int>();
        //             List<int> MID = new List<int>();
        //             foreach(var a in res.Ord)
        //             {
        //                 a.TypeString = GetTypeName(a.Type);
        //                 a.AbnormalStatusDec = a.StatusDec;
        //                 a.StatusDec = Enum.GetName(typeof(OrdStatus), a.Status);
        //                 if(!string.IsNullOrEmpty(a.ExID.ToString()))
        //                 {
        //                     a.ExpNamePinyin = GetExpNamePinyin(cp.CoID,a.ExID);
        //                 }
        //                 if(!string.IsNullOrEmpty(a.PaidAmount))
        //                 {
        //                     if(decimal.Parse(a.PaidAmount) == 0)
        //                     {
        //                         a.PayDate = null;
        //                     }
        //                 }
        //                 if(a.OSource != 3)
        //                 {
        //                     a.Creator = "";
        //                 }
        //                 if(a.IsMerge == true)
        //                 {
        //                     MID.Add(a.ID);
        //                 }
        //                 ItemID.Add(a.ID);
        //             }
        //             //处理soid
        //             var y = new List<Order>();
        //             if(MID.Count > 0)
        //             {
        //                 sqlcommand = "select MergeOID,soid from `order` where coid = @Coid and MergeOID in @ID";
        //                 y = conn.Query<Order>(sqlcommand,new{Coid = cp.CoID,ID = MID}).AsList();
        //             }
        //             sqlcommand = @"select id,oid,SkuAutoID,Img,Qty,GoodsCode,SkuID,SkuName,Norm,RealPrice,Amount,ShopSkuID,IsGift,Weight from orderitem 
        //                                 where oid in @ID and coid = @Coid";
        //             var item = conn.Query<SkuList>(sqlcommand,new{ID = ItemID,Coid = cp.CoID}).AsList();
        //             List<int> skuid = new List<int>();
        //             foreach(var i in item)
        //             {
        //                 skuid.Add(i.SkuAutoID);
        //             }
        //             sqlcommand = "select Skuautoid,(StockQty - LockQty + VirtualQty) as InvQty from inventory_sale where coid = @Coid and Skuautoid in @Skuid";
        //             var inv = conn.Query<Inv>(sqlcommand,new{Coid=cp.CoID,Skuid = skuid}).AsList();
        //             foreach(var i in item)
        //             {
        //                 i.InvQty = 0;
        //                 foreach(var j in inv)
        //                 {
        //                     if(i.SkuAutoID == j.Skuautoid)
        //                     {
        //                         i.InvQty = j.InvQty;
        //                         break;
        //                     }
        //                 }
        //             }
        //             foreach(var a in res.Ord)
        //             {
        //                 if(a.IsMerge == true)
        //                 {
        //                     var soid = new List<long>();
        //                     soid.Add(a.SoID);
        //                     foreach(var b in y)
        //                     {
        //                         if(a.ID == b.MergeOID)
        //                         {
        //                             soid.Add(b.SoID);
        //                         }
        //                     }
        //                     a.SoIDList = soid;
        //                 }
        //                 var sd = new List<SkuList>();
        //                 foreach(var i in item)
        //                 {
        //                     if(a.ID == i.OID)
        //                     {
        //                         sd.Add(i);
        //                     }
        //                 }
        //                 a.SkuList = sd;
        //             }
        //             result.d = res;             
        //         }catch(Exception ex){
        //             result.s = -1;
        //             result.d = ex.Message;
        //             conn.Dispose();
        //         }
        //     }           
        //     return result;
        // }
    }
}