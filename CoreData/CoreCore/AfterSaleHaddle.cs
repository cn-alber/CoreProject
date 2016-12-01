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
    public static class AfterSaleHaddle
    {
        ///<summary>
        ///初始资料
        ///</summary>
        public static DataResult GetInitASData(int CoID)                
        {
            var result = new DataResult(1,null);
            var res = new ASInitData();
            //获取店铺List
            var shop = CoreComm.ShopHaddle.getShopEnum(CoID.ToString()) as List<shopEnum>;
            var ff = new List<Filter>();
            var f = new Filter();
            f.value = "-1";
            f.label = "---不限---";
            ff.Add(f);
            foreach(var t in shop)
            {
                f = new Filter();
                f.value = t.value.ToString();
                f.label = t.label;
                ff.Add(f);
            }
            f = new Filter();
            f.value = "0";
            f.label = "{线下}";
            ff.Add(f);
            res.Shop = ff;  
            //售后状态
            ff = new List<Filter>();
            f = new Filter();
            f.value = "-1";
            f.label = "---不限---";
            ff.Add(f);
            foreach (int  myCode in Enum.GetValues(typeof(ASStatus)))
            {
                f = new Filter();
                f.value = myCode.ToString();
                f.label = Enum.GetName(typeof(ASStatus), myCode);//获取名称
                ff.Add(f);
            }
            res.ASStatus = ff;
            //售后类型
            ff = new List<Filter>();
            f = new Filter();
            f.value = "-1";
            f.label = "---不限---";
            ff.Add(f);
            foreach (int  myCode in Enum.GetValues(typeof(ASType)))
            {
                f = new Filter();
                f.value = myCode.ToString();
                f.label = Enum.GetName(typeof(ASType), myCode);//获取名称
                ff.Add(f);
            }
            res.ASType = ff;
            //订单类型
            var oo = new List<Filter>();
            var o = new Filter();
            o.value = "0";
            o.label = "普通订单";
            oo.Add(o);
            o = new Filter();
            o.value = "1";
            o.label = "补发订单";
            oo.Add(o);
            o = new Filter();
            o.value = "2";
            o.label = "换货订单";
            oo.Add(o);
            o = new Filter();
            o.value = "3";
            o.label = "天猫分销";
            oo.Add(o);
            o = new Filter();
            o.value = "4";
            o.label = "天猫供销";
            oo.Add(o);
            o = new Filter();
            o.value = "5";
            o.label = "协同订单";
            oo.Add(o);
            o = new Filter();
            o.value = "6";
            o.label = "普通订单,分销+";
            oo.Add(o);
            o = new Filter();
            o.value = "7";
            o.label = "补发订单,分销+";
            oo.Add(o);
            o = new Filter();
            o.value = "8";
            o.label = "换货订单,分销+";
            oo.Add(o);
            o = new Filter();
            o.value = "9";
            o.label = "天猫供销,分销+";
            oo.Add(o);
            o = new Filter();
            o.value = "10";
            o.label = "协同订单,分销+";
            oo.Add(o);
            o = new Filter();
            o.value = "11";
            o.label = "普通订单,供销+";
            oo.Add(o);
            o = new Filter();
            o.value = "12";
            o.label = "补发订单,供销+";
            oo.Add(o);
            o = new Filter();
            o.value = "13";
            o.label = "换货订单,供销+";
            oo.Add(o);
            o = new Filter();
            o.value = "14";
            o.label = "天猫供销,供销+";
            oo.Add(o);
            o = new Filter();
            o.value = "15";
            o.label = "协同订单,供销+";
            oo.Add(o);
            o = new Filter();
            o.value = "16";
            o.label = "普通订单,分销+,供销+";
            oo.Add(o);
            o = new Filter();
            o.value = "17";
            o.label = "补发订单,分销+,供销+";
            oo.Add(o);
            o = new Filter();
            o.value = "18";
            o.label = "换货订单,分销+,供销+";
            oo.Add(o);
            o = new Filter();
            o.value = "19";
            o.label = "天猫供销,分销+,供销+";
            oo.Add(o);
            o = new Filter();
            o.value = "20";
            o.label = "协同订单,分销+,供销+";
            oo.Add(o);
            res.OrdType = oo;
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{  
                    //分销商
                    string sqlcommand = "select ID ,DistributorName as Name from distributor where coid =" + CoID + " and enable = true and type = 0";
                    var Distributor = conn.Query<AbnormalReason>(sqlcommand).AsList();
                    var aa = new List<Filter>();
                    var a = new Filter();
                    a.value = "-1";
                    a.label = "---不限---";
                    aa.Add(a);
                    foreach(var d in Distributor)
                    {
                        a = new Filter();
                        a.value = d.ID.ToString();
                        a.label = d.Name;
                        aa.Add(a);
                    }
                    res.Distributor = aa;
                    }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }   
            //问题类型
            ff = new List<Filter>();
            f = new Filter();
            f.value = "-1";
            f.label = "---不限---";
            ff.Add(f);
            foreach (int  myCode in Enum.GetValues(typeof(IssueType)))
            {
                f = new Filter();
                f.value = myCode.ToString();
                f.label = Enum.GetName(typeof(IssueType), myCode);//获取名称
                ff.Add(f);
            }
            res.IssueType = ff;
            //处理结果
            ff = new List<Filter>();
            f = new Filter();
            f.value = "-1";
            f.label = "---不限---";
            ff.Add(f);
            f = new Filter();
            f.value = "-2";
            f.label = "---空---";
            ff.Add(f);
            foreach (int  myCode in Enum.GetValues(typeof(Result)))
            {
                f = new Filter();
                f.value = myCode.ToString();
                f.label = Enum.GetName(typeof(Result), myCode);//获取名称
                ff.Add(f);
            }
            res.Result = ff;
            result.d = res;
            return result;
        }
        ///<summary>
        ///查询售后单List
        ///</summary>
        // public static DataResult GetAsList(AfterSaleParm cp)
        // {
        //     var result = new DataResult(1,null);    
        //     string sqlcount = "select count(id) from aftersale where 1=1";
        //     string sqlcommand = @"select ID,Type,DealerType,IsMerge,IsSplit,OSource,SoID,ODate,PayDate,BuyerShopID,ShopName,Amount,PaidAmount,ExAmount,IsCOD,Status,AbnormalStatus,
        //                           StatusDec,RecMessage,SendMessage,Express,RecLogistics,RecCity,RecDistrict,RecAddress,RecName,ExWeight,Distributor,SupDistributor,InvoiceTitle,
        //                           PlanDate,SendWarehouse,SendDate,ExCode,Creator,RecTel,RecPhone,ExID from aftersale where 1=1"; 
        //     string wheresql = string.Empty;
        //     if(cp.CoID != 1)//公司编号
        //     {
        //         wheresql = wheresql + " and coid = " + cp.CoID;
        //     }
        //     if(!string.IsNullOrEmpty(cp.ExCode))//快递单号
        //     {
        //        wheresql = wheresql + " and excode = '" + cp.ExCode + "'";
        //     }
        //     if(cp.SoID > 0)//外部订单号
        //     {
        //         wheresql = wheresql + " and soid = " + cp.SoID;
        //     }
        //     if(cp.OID > 0)//内部订单号
        //     {
        //         wheresql = wheresql + " and oid = " + cp.OID;
        //     }
        //     if(cp.ID > 0)//售后单号
        //     {
        //         wheresql = wheresql + " and id = " + cp.ID;
        //     }
        //     if(!string.IsNullOrEmpty(cp.BuyerShopID))//买家账号
        //     {
        //        wheresql = wheresql + " and buyershopid like '%" + cp.BuyerShopID + "%'";
        //     }
        //     if(!string.IsNullOrEmpty(cp.RecName))//收货人
        //     {
        //        wheresql = wheresql + " and recname like '%" + cp.RecName + "%'";
        //     }
        //     if(!string.IsNullOrEmpty(cp.Modifier))//修改人
        //     {
        //        wheresql = wheresql + " and Modifier like '%" + cp.Modifier + "%'";
        //     }
        //     if(!string.IsNullOrEmpty(cp.RecPhone))//手机
        //     {
        //        wheresql = wheresql + " and recphone like '%" + cp.RecPhone + "%'";
        //     }
        //     if(!string.IsNullOrEmpty(cp.RecTel))//固定电话
        //     {
        //        wheresql = wheresql + " and rectel like '%" + cp.RecTel + "%'";
        //     }
        //     if(!string.IsNullOrEmpty(cp.Creator))//制单人
        //     {
        //        wheresql = wheresql + " and Creator like '%" + cp.Creator + "%'";
        //     }
        //     if(!string.IsNullOrEmpty(cp.Remark))//备注
        //     {
        //        wheresql = wheresql + " and Remark like '%" + cp.Remark + "%'";
        //     }
        //     if(cp.DateType.ToUpper() == "REGISTERDATE")
        //     {
        //         if(cp.DateStart > DateTime.Parse("1900-01-01"))
        //         {
        //             wheresql = wheresql + " AND RegisterDate >= '" + cp.DateStart + "'" ;
        //         }
        //         if(cp.DateEnd > DateTime.Parse("1900-01-01"))
        //         {
        //             wheresql = wheresql + " AND RegisterDate <= '" + cp.DateEnd + "'" ;
        //         }
        //     }
        //     if(cp.DateType.ToUpper() == "MODIFYDATE")
        //     {
        //         if(cp.DateStart > DateTime.Parse("1900-01-01"))
        //         {
        //             wheresql = wheresql + " AND ModifyDate >= '" + cp.DateStart + "'" ;
        //         }
        //         if(cp.DateEnd > DateTime.Parse("1900-01-01"))
        //         {
        //             wheresql = wheresql + " AND ModifyDate <= '" + cp.DateEnd + "'" ;
        //         }
        //     }
        //     if(cp.DateType.ToUpper() == "CONFIRMDATE")
        //     {
        //         if(cp.DateStart > DateTime.Parse("1900-01-01"))
        //         {
        //             wheresql = wheresql + " AND ConfirmDate >= '" + cp.DateStart + "'" ;
        //         }
        //         if(cp.DateEnd > DateTime.Parse("1900-01-01"))
        //         {
        //             wheresql = wheresql + " AND ConfirmDate <= '" + cp.DateEnd + "'" ;
        //         }
        //     }
       
        // // public string _SkuID = null;//商品编码
        // // public string _IsNoOID = "A";//是否无信息件
        // // public string _IsInterfaceLoad = "A";//接口下载
        // // public string _IsSubmitDis = "A";//分销提交
        // // public int _ShopID = -1;//店铺
        // // public int _Status = -1;//状态
        // // public string _GoodsStatus = null;//货物状态
        // // public int _Type = -1;//售后分类
        // // public int _OrdType = -1;//订单类型
        // // public List<string> _ShopStatus = null;//淘宝状态
        // // public List<string> _RefundStatus = null;//退款状态
        // // public int _Distributor = -1;//分销商
        // // public string _IsSubmit = "A";//供销提交
        // // public int _IssueType = -1;//问题类型
        // // public int _Result = -1;//处理结果
            
            
            
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