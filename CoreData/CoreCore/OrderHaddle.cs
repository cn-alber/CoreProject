using CoreModels;
using MySql.Data.MySqlClient;
using System;
using CoreModels.XyCore;
using Dapper;
using System.Collections.Generic;
using CoreModels.XyComm;
using static CoreModels.Enum.OrderE;
namespace CoreData.CoreCore
{
    public static class OrderHaddle
    {
        ///<summary>
        ///查询订单List
        ///</summary>
        public static DataResult GetOrderList(OrderParm cp)
        {
            var result = new DataResult(1,null);    
            string sqlcount = "select count(id) from `order` where 1=1";
            string sqlcommand = @"select ID,Type,DealerType,IsMerge,IsSplit,OSource,SoID,ODate,PayDate,BuyerShopID,ShopName,Amount,PaidAmount,ExAmount,IsCOD,Status,AbnormalStatus,
                                  StatusDec,RecMessage,SendMessage,Express,RecLogistics,RecCity,RecDistrict,RecAddress,RecName,ExWeight,Distributor,SupDistributor,InvoiceTitle,
                                  PlanDate,SendWarehouse,SendDate,ExCode,Creator,RecTel,RecPhone,ExID from `order` where 1=1"; 
            string wheresql = string.Empty;
            if(cp.CoID != 1)//公司编号
            {
                wheresql = wheresql + " and coid = " + cp.CoID;
            }
            if(cp.ID > 0)//内部订单号
            {
                wheresql = wheresql + " and id = " + cp.ID;
            }
            if(cp.SoID > 0)//外部订单号
            {
                wheresql = wheresql + " and soid = " + cp.SoID;
            }
            if(!string.IsNullOrEmpty(cp.PayNbr))//付款单号
            {
               wheresql = wheresql + " and paynbr = '" + cp.PayNbr + "'";
            }
            if(!string.IsNullOrEmpty(cp.BuyerShopID))//买家账号
            {
               wheresql = wheresql + " and buyershopid like '%" + cp.BuyerShopID + "%'";
            }
            if(!string.IsNullOrEmpty(cp.ExCode))//快递单号
            {
               wheresql = wheresql + " and excode like '%" + cp.ExCode + "%'";
            }
            if(!string.IsNullOrEmpty(cp.RecName))//收货人
            {
               wheresql = wheresql + " and recname like '%" + cp.RecName + "%'";
            }
            if(!string.IsNullOrEmpty(cp.RecPhone))//手机
            {
               wheresql = wheresql + " and recphone like '%" + cp.RecPhone + "%'";
            }
            if(!string.IsNullOrEmpty(cp.RecTel))//固定电话
            {
               wheresql = wheresql + " and rectel like '%" + cp.RecTel + "%'";
            }
            if(!string.IsNullOrEmpty(cp.RecLogistics))//收货人省
            {
               wheresql = wheresql + " and reclogistics like '%" + cp.RecLogistics + "%'";
            }
            if(!string.IsNullOrEmpty(cp.RecCity))//收货人城市
            {
               wheresql = wheresql + " and reccity like '%" + cp.RecCity + "%'";
            }
            if(!string.IsNullOrEmpty(cp.RecDistrict))//收货人县区
            {
               wheresql = wheresql + " and recdistrict like '%" + cp.RecDistrict + "%'";
            }
            if(!string.IsNullOrEmpty(cp.RecAddress))//详细地址
            {
               wheresql = wheresql + " and recaddress like '%" + cp.RecAddress + "%'";
            }
            if (cp.StatusList != null)//状态List
            {
                wheresql = wheresql + " AND status in ("+ string.Join(",", cp.StatusList) + ")" ;
            }
            if (cp.AbnormalStatusList != null)//异常状态List
            {
                string status = " AND abnormalstatus in (0,"+ string.Join(",", cp.AbnormalStatusList) + ")" ;
                if(cp.StatusList != null)
                {
                    if(cp.StatusList.Count == 1 && cp.StatusList[0] == 7)
                    {
                        status = " AND abnormalstatus in ("+ string.Join(",", cp.AbnormalStatusList) + ")" ;
                    }
                }
                else
                {
                    status = " AND abnormalstatus in ("+ string.Join(",", cp.AbnormalStatusList) + ")" ;
                }
                wheresql = wheresql + status ;
            }
            if(cp.IsRecMsgYN.ToUpper() == "Y")
            {
                if(cp.RecMessage == null)
                {
                    wheresql = wheresql + " AND recmessage != '' and status in (0,1,2,7)" ;
                }
                else
                {
                    wheresql = wheresql + " AND recmessage like '%" + cp.RecMessage + "%' and status in (0,1,2,7)" ;
                }
            }
            if(cp.IsRecMsgYN.ToUpper() == "N")
            {
                wheresql = wheresql + " AND recmessage = '' and status in (0,1,2,7)" ;
            }
            if(cp.IsSendMsgYN.ToUpper() == "Y")
            {
                if(cp.SendMessage == null)
                {
                    wheresql = wheresql + " AND sendmessage != '' and status in (0,1,2,7)" ;
                }
                else 
                {
                    wheresql = wheresql + " AND sendmessage like '%" + cp.SendMessage + "%' and status in (0,1,2,7)" ;
                }
            }
            if(cp.IsSendMsgYN.ToUpper() == "N")
            {
                wheresql = wheresql + " AND sendmessage = '' and status in (0,1,2,7)" ;
            }
            if(cp.Datetype.ToUpper() == "ODATE")
            {
                wheresql = wheresql + " AND odate between '" + cp.DateStart + "' and '" + cp.DateEnd + "'" ;
            }
            if(cp.Datetype.ToUpper() == "SENDDATE")
            {
                wheresql = wheresql + " AND senddate between '" + cp.DateStart + "' and '" + cp.DateEnd + "'" ;
            }
            if(cp.Datetype.ToUpper() == "PAYDATE")
            {
                wheresql = wheresql + " AND paydate between '" + cp.DateStart + "' and '" + cp.DateEnd + "'" ;
            }
            if(cp.Datetype.ToUpper() == "PLANDATE")
            {
                wheresql = wheresql + " AND plandate between '" + cp.DateStart + "' and '" + cp.DateEnd + "'" ;
            }
            if(!string.IsNullOrEmpty(cp.Skuid))
            {
               wheresql = wheresql + " and exists(select id from orderitem where oid = order.id and skuid = '" + cp.Skuid + "')";
            }
            if(!string.IsNullOrEmpty(cp.GoodsCode))
            {
               wheresql = wheresql + " and exists(select id from orderitem where oid = order.id and GoodsCode = '" + cp.Skuid + "')";
            }
            if(cp.Ordqtystart > 0)
            {
                wheresql = wheresql + " AND ordqty >= " +  cp.Ordqtystart + " and status in (0,1,2,7)";
            }
            if(cp.Ordqtyend > 0)
            {
                wheresql = wheresql + " AND ordqty <= " +  cp.Ordqtyend + " and status in (0,1,2,7)";
            }
            if(cp.Ordamtstart > 0)
            {
                wheresql = wheresql + " AND amount >= " +  cp.Ordamtstart + " and status in (0,1,2,7)";
            }
            if(cp.Ordamtend > 0)
            {
                wheresql = wheresql + " AND amount <= " +  cp.Ordamtend + " and status in (0,1,2,7)";
            }
            if(!string.IsNullOrEmpty(cp.Skuname))
            {
               wheresql = wheresql + " and exists(select id from orderitem where oid = order.id and skuname like '%" + cp.Skuname + "%') and status in (0,1,2,7)";
            }
            if(!string.IsNullOrEmpty(cp.Norm))
            {
               wheresql = wheresql + " and exists(select id from orderitem where oid = order.id and norm like '%" + cp.Norm + "%') and status in (0,1,2,7)";
            }
            if(cp.ShopStatus != null)
            {
                string shopstatus = string.Empty;
                foreach(var x in cp.ShopStatus)
                {
                    shopstatus = shopstatus + "'" + x + "',";
                }
                shopstatus = shopstatus.Substring(0,shopstatus.Length - 1);
                wheresql = wheresql + " AND shopstatus in (" +  shopstatus + ")";
            }
            if(cp.OSource > -1)
            {
                wheresql = wheresql + " AND osource = " +  cp.OSource;
            }
            if (cp.Type != null)
            {
                wheresql = wheresql + " AND type in ("+ string.Join(",", cp.Type) + ")" ;
            }
            if(cp.IsCOD.ToUpper() == "Y")
            {
                wheresql = wheresql + " AND iscod = true" ;
            }
            if(cp.IsCOD.ToUpper() == "N")
            {
                wheresql = wheresql + " AND iscod = false" ;
            }
            if(cp.IsPaid.ToUpper() == "Y")
            {
                wheresql = wheresql + " AND IsPaid = true" ;
            }
            if(cp.IsPaid.ToUpper() == "N")
            {
                wheresql = wheresql + " AND IsPaid = false" ;
            }
            if (cp.IsShopSelectAll == false &&　cp.ShopID != null)
            {
                wheresql = wheresql + " AND shopid in ("+ string.Join(",", cp.ShopID) + ")" ;
            }
            if(cp.IsDisSelectAll == true)
            {
                wheresql = wheresql + " AND dealertype = 2" ;
            }
            else
            {
                if(cp.Distributor != null)
                {
                    string distributor = string.Empty;
                    foreach(var x in cp.Distributor)
                    {
                        distributor = distributor + "'" + x + "',";
                    }
                    distributor = distributor.Substring(0,distributor.Length - 1);
                    wheresql = wheresql + " AND dealertype = 2 AND distributor in (" +  distributor + ")";
                }
            }
            if(cp.ExID != null)
            {
                wheresql = wheresql + " AND exid in ("+ string.Join(",", cp.ExID) + ")" ;
            }
            if(cp.SendWarehouse != null)
            {
                string sendwarehouse = string.Empty;
                foreach(var x in cp.SendWarehouse)
                {
                    sendwarehouse = sendwarehouse + "'" + x + "',";
                }
                sendwarehouse = sendwarehouse.Substring(0,sendwarehouse.Length - 1);
                wheresql = wheresql + " AND sendwarehouse in (" +  sendwarehouse + ")";
            }
            if(cp.Others != null)
            {
                if(cp.Others.Contains(4))
                {
                    wheresql = wheresql + " and IsInvoice = true";
                }
                if(cp.Others.Contains(0) == true &&　cp.Others.Contains(0) == false)
                {
                    wheresql = wheresql + " and IsMerge = true";
                }
                if(cp.Others.Contains(0) == false &&　cp.Others.Contains(0) == true)
                {
                    wheresql = wheresql + " and IsMerge = false";
                }
                if(cp.Others.Contains(1) == true &&　cp.Others.Contains(3) == false)
                {
                    wheresql = wheresql + " and IsSplit = true";
                }
                if(cp.Others.Contains(1) == false &&　cp.Others.Contains(3) == true)
                {
                    wheresql = wheresql + " and IsSplit = false";
                }
            }
            if(!string.IsNullOrEmpty(cp.SortField) && !string.IsNullOrEmpty(cp.SortDirection))//排序
            {
                wheresql = wheresql + " ORDER BY "+cp.SortField +" "+ cp.SortDirection;
            }
            var res = new OrderData();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    int count = conn.QueryFirst<int>(sqlcount + wheresql);
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(cp.NumPerPage.ToString()));
                    int dataindex = (cp.PageIndex - 1)* cp.NumPerPage;
                    wheresql = wheresql + " limit " + dataindex.ToString() + " ," + cp.NumPerPage.ToString();
                    var u = conn.Query<OrderQuery>(sqlcommand + wheresql).AsList();
                    res.Datacnt = count;
                    res.Pagecnt = pagecnt;
                    res.Ord = u;
                    //订单资料
                    List<int> ItemID = new List<int>();
                    List<int> MID = new List<int>();
                    foreach(var a in res.Ord)
                    {
                        a.TypeString = GetTypeName(a.Type);
                        a.AbnormalStatusDec = a.StatusDec;
                        a.StatusDec = Enum.GetName(typeof(OrdStatus), a.Status);
                        if(a.OSource != 3)
                        {
                            a.Creator = "";
                        }
                        if(a.IsMerge == true)
                        {
                            MID.Add(a.ID);
                        }
                        ItemID.Add(a.ID);
                    }
                    //处理soid
                    var y = new List<Order>();
                    if(MID.Count > 0)
                    {
                        sqlcommand = "select MergeOID,soid from `order` where coid = @Coid and MergeOID in @ID";
                        y = conn.Query<Order>(sqlcommand,new{Coid = cp.CoID,ID = MID}).AsList();
                    }
                    sqlcommand = @"select id,oid,SkuAutoID,Img,Qty,GoodsCode,SkuID,SkuName,Norm,RealPrice,Amount,ShopSkuID,IsGift,Weight from orderitem 
                                        where oid in @ID and coid = @Coid";
                    var item = conn.Query<SkuList>(sqlcommand,new{ID = ItemID,Coid = cp.CoID}).AsList();
                    List<int> skuid = new List<int>();
                    foreach(var i in item)
                    {
                        skuid.Add(i.SkuAutoID);
                    }
                    sqlcommand = "select Skuautoid,(StockQty - LockQty + VirtualQty) as InvQty from inventory where coid = @Coid and WarehouseID = 0 and Skuautoid in @Skuid";
                    var inv = conn.Query<Inv>(sqlcommand,new{Coid=cp.CoID,Skuid = skuid}).AsList();
                    foreach(var i in item)
                    {
                        i.InvQty = 0;
                        foreach(var j in inv)
                        {
                            if(i.SkuAutoID == j.Skuautoid)
                            {
                                i.InvQty = j.InvQty;
                                break;
                            }
                        }
                    }
                    foreach(var a in res.Ord)
                    {
                        if(a.IsMerge == true)
                        {
                            var soid = new List<long>();
                            soid.Add(a.SoID);
                            foreach(var b in y)
                            {
                                if(a.ID == b.MergeOID)
                                {
                                    soid.Add(b.SoID);
                                }
                            }
                            a.SoIDList = soid;
                        }
                        var sd = new List<SkuList>();
                        foreach(var i in item)
                        {
                            if(a.ID == i.OID)
                            {
                                sd.Add(i);
                            }
                        }
                        a.SkuList = sd;
                    }
                    result.d = res;             
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }           
            return result;
        }
        ///<summary>
        ///刷新单笔订单明细显示
        ///</summary>
        public static DataResult GetSingleOrdItem(int OID,int CoID)
        {
            var result = new DataResult(1,null);   
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string wheresql = @"select id,SkuAutoID,Img,Qty,GoodsCode,SkuID,SkuName,Norm,RealPrice,Amount,ShopSkuID,IsGift,Weight from orderitem 
                                        where oid = " + OID + " and coid =" + CoID;
                    var u = conn.Query<SkuList>(wheresql).AsList();
                    foreach(var i in u)
                    {
                        i.InvQty = GetInvQty(CoID,i.SkuAutoID);
                    }
                    result.d = u;
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            return result;
        }
        ///<summary>
        ///根据Sku查询库存
        ///</summary>
        public static int GetInvQty(int CoID,int skuid)
        {
            int invqty = 0;
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string wheresql = "select ifnull(sum(StockQty - LockQty + VirtualQty),0) from inventory where coid = " + CoID + " and WarehouseID = 0 and Skuautoid = " + skuid;
                    invqty = conn.QueryFirst<int>(wheresql);
                }catch(Exception ex){
                    result.d  = ex.Message;
                    invqty = 0;
                    conn.Dispose();
                }
            } 
            return invqty;
        }
        ///<summary>
        ///新增订单
        ///</summary>
        public static DataResult InsertOrder(Order ord,string UserName,int CoID,bool isFaceToFace)
        {
            var result = new DataResult(1,null);   
            var rec = new RecInfo();
            var log = new Log();
            ord.CoID = CoID;
            ord.Creator = UserName;
            ord.Modifier = UserName;
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{
                    if ( ord.ShopID == 0)
                    {
                        result.s = -1;
                        result.d = "店铺必须有值!";
                        return result;
                    }
                    else
                    {
                        string wheresql = "select * from Shop where id =" + ord.ShopID + " and coid =" + CoID;
                        var u = conn.Query<Shop>(wheresql).AsList();
                        if(u.Count == 0)
                        {
                            result.s = -1;
                            result.d = "此店铺不存在!!";
                            return result;
                        }
                        else
                        {
                            if(u[0].Enable == false || u[0].Deleted == true)
                            {
                                result.s = -1;
                                result.d = "此店铺已停用或已删除!!";
                                return result;
                            }
                            else
                            {
                                ord.ShopName = u[0].ShopName;
                                ord.ShopSit = u[0].ShopSite;
                            }
                        }
                    }
                    if(isFaceToFace == true)
                    {
                        ord.Express = "现场取货";
                        var ee = GetExpID("现场取货",CoID);
                        if(ee.s == -1)
                        {
                            result.s = -1;
                            result.d = "请先设定现场取货的快递资料!!";
                            return result;
                        }
                        ord.ExID = ee.s;
                    }
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }
            //检查必输栏位是否都有值
            if ( ord.ODate == null)
            {
                result.s = -1;
                result.d = "订单日期必须有值!";
                return result;
            }
            if(isFaceToFace == false)
            {
                if ( ord.RecLogistics == null || ord.RecCity == null || ord.RecDistrict == null || ord.RecAddress == null)
                {
                    result.s = -1;
                    result.d = "收货地址必须有值!";
                    return result;
                }
                if ( ord.RecName == null)
                {
                    result.s = -1;
                    result.d = "收货人必须有值!";
                    return result;
                }
            }
            bool isRecExsit = true;
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    //检查平台编号是否重复
                    if(ord.SoID > 0)
                    {
                        string wheresql = "select count(id) from `order` where soid = " + ord.SoID + " and coid = " + CoID;
                        int u = conn.QueryFirst<int>(wheresql);
                        if(u > 0)
                        {
                            result.s = -1;
                            result.d = "线上订单号已经存在,请重新输入!";
                            return result;
                        }
                    }
                    else
                    {
                        string rand = DateTime.Now.Ticks.ToString();
                        ord.SoID = long.Parse(rand.Substring(0, 11));
                    }
                    //检查收货人是否存在
                    if(ord.BuyerShopID == null)
                    {
                        ord.BuyerShopID = ord.RecName;
                    }
                    if(ord.BuyerShopID != null && ord.RecName != null && ord.RecLogistics != null && ord.RecCity != null && ord.RecDistrict != null && ord.RecAddress != null)
                    {
                        string wheresql = "select count(id) from recinfo where coid = " + CoID + " and buyerid = '" + ord.BuyerShopID + "' and receiver = '" + ord.RecName + 
                                           "' and address = '" + ord.RecAddress + "' and logistics = '" + ord.RecLogistics + "' and city = '" + ord.RecCity + 
                                           "' and district = '" + ord.RecDistrict + "'";
                        int u = conn.QueryFirst<int>(wheresql);
                        if(u > 0)
                        {
                            wheresql = "select id from recinfo where coid = " + CoID + " and buyerid = '" + ord.BuyerShopID + "' and receiver = '" + ord.RecName + 
                                           "' and address = '" + ord.RecAddress + "' and logistics = '" + ord.RecLogistics + "' and city = '" + ord.RecCity + 
                                           "' and district = '" + ord.RecDistrict + "'";
                            u = conn.QueryFirst<int>(wheresql);
                            ord.BuyerID = u;
                        }
                        else
                        {
                            isRecExsit = false;
                            rec.BuyerId = ord.BuyerShopID;
                            rec.Receiver = ord.RecName;
                            rec.Tel = ord.RecTel;
                            rec.Phone = ord.RecPhone;
                            rec.Logistics = ord.RecLogistics;
                            rec.City = ord.RecCity;
                            rec.District = ord.RecDistrict;
                            rec.Address = ord.RecAddress;
                            rec.ZipCode = ord.RecZip;
                            rec.Express = ord.Express;
                            rec.ExID = ord.ExID;
                            rec.CoID = CoID;
                            rec.Creator = UserName;
                            rec.ShopSit = ord.ShopSit;
                        }
                    }
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            //检查订单是否符合合并的条件
            var res = isCheckMerge(ord);
            int reasonid = 0;
            if(res.s == 1)
            {
                reasonid = GetReasonID("等待订单合并",CoID).s;
                if(reasonid == -1)
                {
                    result.s = -1;
                    result.d = "请先设定【等待订单合并】的异常";
                    return result;
                }
            }
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string sqlCommandText = string.Empty;
                int count =0;
                if(isRecExsit == false)
                {
                    sqlCommandText = @"INSERT INTO recinfo(BuyerId,Receiver,Tel,Phone,Logistics,City,District,Address,ZipCode,Express,ExID,CoID,Creator,ShopSit) VALUES(
                            @BuyerId,@Receiver,@Tel,@Phone,@Logistics,@City,@District,@Address,@ZipCode,@Express,@ExID,@CoID,@Creator,@ShopSit)";
                    count =CoreDBconn.Execute(sqlCommandText,rec,TransCore);
                    if(count < 0)
                    {
                        result.s = -3002;
                        return result;
                    }
                    else
                    {
                        int rtn = CoreDBconn.QueryFirst<int>("select LAST_INSERT_ID()");
                        ord.BuyerID = rtn;
                        rec.ID = rtn;
                    }
                }
                if(res.s == 1)
                {
                    ord.Status = 7;
                    ord.AbnormalStatus = reasonid;
                    ord.StatusDec = "等待订单合并";
                }
                else
                {
                    ord.Status = 0;
                    ord.AbnormalStatus = 0;
                    ord.StatusDec = "";
                }
                sqlCommandText = @"INSERT INTO `order`(ODate,CoID,BuyerID,BuyerShopID,ShopID,ShopName,ShopSit,SoID,RecName,RecLogistics,RecCity,RecDistrict,RecAddress,RecZip,
                                                            RecTel,RecPhone,RecMessage,SendMessage,Express,ExID,Creator,Modifier,Status,AbnormalStatus,StatusDec) VALUES(
                                        @ODate,@CoID,@BuyerID,@BuyerShopID,@ShopID,@ShopName,@ShopSit,@SoID,@RecName,@RecLogistics,@RecCity,@RecDistrict,@RecAddress,@RecZip,
                                                            @RecTel,@RecPhone,@RecMessage,@SendMessage,@Express,@ExID,@Creator,@Modifier,@Status,@AbnormalStatus,@StatusDec)";
                count =CoreDBconn.Execute(sqlCommandText,ord,TransCore);
                if(count < 0)
                {
                    result.s = -3002;
                    return result;
                }
                else
                {
                    int rtn = CoreDBconn.QueryFirst<int>("select LAST_INSERT_ID()");
                    result.d = rtn;
                    rec.OID=rtn;
                    ord.ID = rtn;
                }
                log.OID = ord.ID;
                log.SoID = ord.SoID;
                log.Type = 0;
                log.LogDate = DateTime.Now;
                log.UserName = UserName;
                log.Title = "接单时间";
                log.Remark = "手工下单时间";
                log.CoID = CoID;
                string loginsert = @"INSERT INTO orderlog(OID,SoID,Type,LogDate,UserName,Title,Remark,CoID) 
                                                   VALUES(@OID,@SoID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                count =CoreDBconn.Execute(loginsert,log,TransCore);
                if(count < 0)
                {
                    result.s = -3002;
                    return result;
                }
                if(ord.Status == 7)
                {
                    log.LogDate = DateTime.Now;
                    log.Title = "标记异常";
                    log.Remark = "等待订单合并(自动)";
                    count =CoreDBconn.Execute(loginsert,log,TransCore);
                    if(count < 0)
                    {
                        result.s = -3002;
                        return result;
                    }
                }
                if(isRecExsit == false)
                {
                    sqlCommandText = @"update recinfo set OID = @OID where id = @ID and coid = @Coid";
                    count =CoreDBconn.Execute(sqlCommandText,rec,TransCore);
                    if(count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                }
                if(res.s == 1)
                {
                    List<int> idlist = res.d as List<int>;
                    string querySql = "select id,soid from `order` where id in @ID and coid = @Coid and status <> 7";
                    var v = CoreDBconn.Query<Order>(querySql,new{ID = idlist,Coid = CoID}).AsList();
                    if(v.Count > 0)
                    {
                        sqlCommandText = @"update `order` set status = 7,abnormalstatus = @Abnormalstatus,statusdec = '等待订单合并' where id in @ID and coid = @Coid and status <> 7";
                        count =CoreDBconn.Execute(sqlCommandText,new {Abnormalstatus = reasonid,ID = idlist,Coid = CoID},TransCore);
                        if(count < 0)
                        {
                            result.s = -3003;
                            return result;
                        }
                        foreach(var c in v)
                        {
                            log.OID = c.ID;
                            log.SoID = c.SoID;
                            log.LogDate = DateTime.Now;
                            log.Title = "标记异常";
                            log.Remark = "等待订单合并(自动)";
                            count =CoreDBconn.Execute(loginsert,log,TransCore);
                            if(count < 0)
                            {
                                result.s = -3002;
                                return result;
                            }
                        }
                    }
                }
                TransCore.Commit();
            }catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }
            return  result;
        }
        ///<summary>
        ///检查订单是否符合合并的条件
        ///</summary>
        public static DataResult isCheckMerge(Order ord)
        {
            var result = new DataResult(1,null);   
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string wheresql = "select id from `order` where coid = " + ord.CoID + " and buyershopid = '" + ord.BuyerShopID + "' and recname = '" + ord.RecName + 
                                      "' and reclogistics = '" + ord.RecLogistics + "' and reccity = '" + ord.RecCity + "' and recdistrict = '" + ord.RecDistrict + 
                                      "' and recaddress = '" + ord.RecAddress + "' and status in (0,1,7)";
                    var u = conn.Query<Order>(wheresql).AsList();
                    if(u.Count > 0)
                    {
                        List<int> id = new List<int>();
                        foreach(var a in u)
                        {
                            id.Add(a.ID);
                        }
                        result.s = 1;
                        result.d = id;
                    }
                    else
                    {
                        result.s = 0;
                    }
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            return result;
        }
        ///<summary>
        ///根据异常原因获取id
        ///</summary>
        public static DataResult GetReasonID(string ReasonName,int CoID)
        {
            var result = new DataResult(1,null);   
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string wheresql = "select id from orderabnormal where coid = " + CoID + " and name = '" + ReasonName + "' and iscustom = false";
                    int u = conn.QueryFirst<int>(wheresql);
                    if(u > 0)
                    {
                        result.s = u;
                    }
                    else
                    {
                        result.s = -1;
                    }
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            return result;
        }
        ///<summary>
        ///根据快递名称抓取ID
        ///</summary>
        public static DataResult GetExpID(string Express,int CoID)
        {
            var result = new DataResult(1,null);   
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{
                    string wheresql = "select id from express where coid = " + CoID + " and ExpName = '" + Express + "'";
                    int u = conn.QueryFirst<int>(wheresql);
                    if(u > 0)
                    {
                        result.s = u;
                    }
                    else
                    {
                        result.s = -1;
                    }
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            return result;
        }
        ///<summary>
        ///更新订单
        ///</summary>
        public static DataResult UpdateOrder(decimal ExAmount,string Remark,string InvTitle,string Logistics,string City,string District,string Address,string Name,
                                            string tel,string phone,int OID,string UserName,int CoID)
        {
            var result = new DataResult(1,null);   
            var logs = new List<Log>();
            var OrdOlder = new Order();
            var CancleOrdAb = new Order();
            int reasonid = 0;
            List<int> idlist = new List<int>();
            int x = 0;//未审核订单修改标记
            int y = 0;//审核订单修改标记
            int z = 0;//地址修改标记
            string RecLogistics="",RecCity="",RecDistrict="",RecAddress="",RecName="";
            string querySql = "select * from `order` where id = @ID and coid = @Coid";
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    var u = conn.Query<Order>(querySql,new{ID = OID,Coid = CoID}).AsList();
                    if(u.Count == 0)
                    {
                        result.s = -1;
                        result.d = "参数无效";
                        return result;
                    }
                    OrdOlder = u[0] as Order;
                    RecLogistics = OrdOlder.RecLogistics;
                    RecCity = OrdOlder.RecCity;
                    RecDistrict = OrdOlder.RecDistrict;
                    RecAddress = OrdOlder.RecAddress;
                    RecName = OrdOlder.RecName;
                    if(OrdOlder.Status == 0 || OrdOlder.Status == 1 || OrdOlder.Status == 7)
                    {
                        if(OrdOlder.ExAmount != ExAmount.ToString())
                        {
                            var log = new Log();
                            log.OID = OrdOlder.ID;
                            log.SoID = OrdOlder.SoID;
                            log.Type = 0;
                            log.LogDate = DateTime.Now;
                            log.UserName = UserName;
                            log.Title = "手动修改运费";
                            log.Remark = "运费" + OrdOlder.ExAmount + " => " + ExAmount;
                            log.CoID = CoID;
                            logs.Add(log);
                            OrdOlder.ExAmount = ExAmount.ToString();
                            x++;
                        }
                        string addressOlder = OrdOlder.RecLogistics + OrdOlder.RecCity + OrdOlder.RecDistrict + OrdOlder.RecAddress;
                        if(OrdOlder.RecLogistics != Logistics)
                        {
                            OrdOlder.RecLogistics = Logistics;
                            x++;
                            z++;
                        }
                        if(OrdOlder.RecCity != City)
                        {
                            OrdOlder.RecCity = City;
                            x++;
                            z++;
                        }
                        if(OrdOlder.RecDistrict != District)
                        {
                            OrdOlder.RecDistrict = District;
                            x++;
                            z++;
                        }
                        if(OrdOlder.RecAddress != Address)
                        {
                            OrdOlder.RecAddress = Address;
                            x++;
                            z++;
                        }
                        if(z > 0)
                        {
                            var log = new Log();
                            log.OID = OrdOlder.ID;
                            log.SoID = OrdOlder.SoID;
                            log.Type = 0;
                            log.LogDate = DateTime.Now;
                            log.UserName = UserName;
                            log.Title = "手动修改收货地址";
                            log.Remark = "收货地址" + addressOlder + " => " + OrdOlder.RecLogistics + OrdOlder.RecCity + OrdOlder.RecDistrict + OrdOlder.RecAddress;
                            log.CoID = CoID;
                            logs.Add(log);
                        }
                        if(OrdOlder.RecName != Name)
                        {
                            var log = new Log();
                            log.OID = OrdOlder.ID;
                            log.SoID = OrdOlder.SoID;
                            log.Type = 0;
                            log.LogDate = DateTime.Now;
                            log.UserName = UserName;
                            log.Title = "手动修改收货人";
                            log.Remark = "收货人" + OrdOlder.RecName + " => " + Name;
                            log.CoID = CoID;
                            logs.Add(log);
                            OrdOlder.RecName = Name;
                            x++;
                            z++;
                        }
                        if(OrdOlder.RecTel != tel)
                        {
                            var log = new Log();
                            log.OID = OrdOlder.ID;
                            log.SoID = OrdOlder.SoID;
                            log.Type = 0;
                            log.LogDate = DateTime.Now;
                            log.UserName = UserName;
                            log.Title = "手动修改电话";
                            log.Remark = "电话" + OrdOlder.RecTel + " => " + tel;
                            log.CoID = CoID;
                            logs.Add(log);
                            OrdOlder.RecTel = tel;
                            x++;
                        }
                        if(OrdOlder.RecPhone != phone)
                        {
                            var log = new Log();
                            log.OID = OrdOlder.ID;
                            log.SoID = OrdOlder.SoID;
                            log.Type = 0;
                            log.LogDate = DateTime.Now;
                            log.UserName = UserName;
                            log.Title = "手动修改手机";
                            log.Remark = "手机" + OrdOlder.RecPhone + " => " + phone;
                            log.CoID = CoID;
                            logs.Add(log);
                            OrdOlder.RecPhone = phone;
                            x++;
                        }
                    }
                    if(OrdOlder.SendMessage != Remark)
                    {
                        var log = new Log();
                        log.OID = OrdOlder.ID;
                        log.SoID = OrdOlder.SoID;
                        log.Type = 0;
                        log.LogDate = DateTime.Now;
                        log.UserName = UserName;
                        log.Title = "手动修改卖家备注";
                        log.Remark = "备注" + OrdOlder.SendMessage + " => " + Remark;
                        log.CoID = CoID;
                        logs.Add(log);
                        OrdOlder.SendMessage = Remark;
                        x++;
                        y++;
                    }
                    if(OrdOlder.InvoiceTitle != InvTitle)
                    {
                        var log = new Log();
                        log.OID = OrdOlder.ID;
                        log.SoID = OrdOlder.SoID;
                        log.Type = 0;
                        log.LogDate = DateTime.Now;
                        log.UserName = UserName;
                        log.Title = "手动修改发票抬头";
                        log.Remark = "发票抬头" + OrdOlder.InvoiceTitle + " => " + InvTitle;
                        log.CoID = CoID;
                        logs.Add(log);
                        OrdOlder.InvoiceTitle = InvTitle;
                        x++;
                        y++;
                    }
                    //检查订单是否符合合并的条件
                    if(z > 0)
                    {
                        //若订单本身是等待合并时，先判断是否需要还原资料
                        if(OrdOlder.Status == 7 && OrdOlder.StatusDec == "等待订单合并")
                        {
                            var log = new Log();
                            log.OID = OrdOlder.ID;
                            log.SoID = OrdOlder.SoID;
                            log.Type = 0;
                            log.LogDate = DateTime.Now;
                            log.UserName = UserName;
                            log.Title = "取消异常标记";
                            log.Remark = "等待订单合并(自动)";
                            log.CoID = CoID;
                            logs.Add(log);
                            if(OrdOlder.IsPaid == true)
                            {
                                OrdOlder.Status = 1;
                            }
                            else
                            {
                                OrdOlder.Status = 0;
                            }
                            OrdOlder.AbnormalStatus = 0;
                            OrdOlder.StatusDec="";
                            var ck = isCheckCancleMerge(OID,CoID,OrdOlder.BuyerShopID,RecName,RecLogistics,RecCity,RecDistrict,RecAddress);
                            if(ck.s == 1)
                            {
                                int oid = int.Parse(ck.d.ToString());
                                querySql = "select * from `order` where id = " + oid + " and coid = " + CoID;
                                var v = conn.Query<Order>(querySql).AsList();
                                CancleOrdAb = v[0] as Order;
                                log = new Log();
                                log.OID = CancleOrdAb.ID;
                                log.SoID = CancleOrdAb.SoID;
                                log.Type = 0;
                                log.LogDate = DateTime.Now;
                                log.UserName = UserName;
                                log.Title = "取消异常标记";
                                log.Remark = "等待订单合并(自动)";
                                log.CoID = CoID;
                                logs.Add(log);
                                if(CancleOrdAb.IsPaid == true)
                                {
                                    CancleOrdAb.Status = 1;
                                }
                                else
                                {
                                    CancleOrdAb.Status = 0;
                                }
                                CancleOrdAb.AbnormalStatus = 0;
                                CancleOrdAb.StatusDec="";
                                CancleOrdAb.Modifier = UserName;
                                CancleOrdAb.ModifyDate = DateTime.Now;
                            }
                        }
                        //检查订单是否符合合并的条件
                        var res = isCheckMerge(OrdOlder);
                        if(res.s == 1)
                        {
                            reasonid = GetReasonID("等待订单合并",CoID).s;
                            if(reasonid == -1)
                            {
                                result.s = -1;
                                result.d = "请先设定【等待订单合并】的异常";
                                return result;
                            }
                            OrdOlder.Status = 7;
                            OrdOlder.AbnormalStatus = reasonid;
                            OrdOlder.StatusDec="等待订单合并";
                            var log = new Log();
                            log.OID = OrdOlder.ID;
                            log.SoID = OrdOlder.SoID;
                            log.Type = 0;
                            log.LogDate = DateTime.Now;
                            log.UserName = UserName;
                            log.Title = "标记异常";
                            log.Remark = "等待订单合并(自动)";
                            log.CoID = CoID;
                            logs.Add(log);
                            idlist = res.d as List<int>;
                            querySql = "select id,soid from `order` where id in @ID and coid = @Coid and status <> 7";
                            var v = conn.Query<Order>(querySql,new{ID = idlist,Coid = CoID}).AsList();
                            if(v.Count > 0)
                            {
                                foreach(var c in v)
                                {
                                    log = new Log();
                                    log.OID = c.ID;
                                    log.SoID = c.SoID;
                                    log.Type = 0;
                                    log.LogDate = DateTime.Now;
                                    log.UserName = UserName;
                                    log.Title = "标记异常";
                                    log.Remark = "等待订单合并(自动)";
                                    log.CoID = CoID;
                                    logs.Add(log);
                                }
                            }
                            else
                            {
                                idlist = new List<int>();
                            }
                        }   
                    }
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string sqlCommandText = string.Empty;
                int count =0;
                OrdOlder.Modifier = UserName;
                OrdOlder.ModifyDate = DateTime.Now;
                if(x > 0)
                {
                    if(decimal.Parse(OrdOlder.SkuAmount) + ExAmount == decimal.Parse(OrdOlder.Amount))
                    {
                        OrdOlder.IsPaid = true;
                        if(OrdOlder.Status != 7)
                        {
                            OrdOlder.Status = 1;
                        }
                    }
                    else
                    {
                        OrdOlder.IsPaid = false;
                        if(OrdOlder.Status != 7)
                        {
                            OrdOlder.Status = 0;
                        }
                    }
                    sqlCommandText = @"update `order` set ExAmount = @ExAmount,Amount = ExAmount + SkuAmount,RecLogistics=@RecLogistics,RecCity=@RecCity,RecDistrict=@RecDistrict,
                                        RecAddress=@RecAddress,RecName=@RecName,RecTel=@RecTel,RecPhone=@RecPhone,SendMessage=@SendMessage,InvoiceTitle=@InvoiceTitle,Status=@Status,
                                        AbnormalStatus=@AbnormalStatus,StatusDec=@StatusDec,IsPaid=@IsPaid,Modifier=@Modifier,ModifyDate=@ModifyDate where ID = @ID and CoID = @CoID";
                    count =CoreDBconn.Execute(sqlCommandText,OrdOlder,TransCore);
                    if(count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                }
                else
                {
                    if(y > 0)
                    {
                        sqlCommandText = @"update `order` set SendMessage=@SendMessage,InvoiceTitle=@InvoiceTitle,Modifier=@Modifier,ModifyDate=@ModifyDate where ID = @ID and CoID = @CoID";
                        count =CoreDBconn.Execute(sqlCommandText,OrdOlder,TransCore);
                        if(count < 0)
                        {
                            result.s = -3003;
                            return result;
                        }
                    }
                }
                if(CancleOrdAb.ID > 0)
                {
                    sqlCommandText = @"update `order` set Status=@Status,AbnormalStatus=@AbnormalStatus,StatusDec=@StatusDec,Modifier=@Modifier,ModifyDate=@ModifyDate where ID = @ID and CoID = @CoID";
                    count =CoreDBconn.Execute(sqlCommandText,CancleOrdAb,TransCore);
                    if(count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                }
                if(idlist.Count > 0)
                {
                    sqlCommandText = @"update `order` set status = 7,abnormalstatus = @Abnormalstatus,statusdec = '等待订单合并' where id in @ID and coid = @Coid and status <> 7";
                    count =CoreDBconn.Execute(sqlCommandText,new {Abnormalstatus = reasonid,ID = idlist,Coid = CoID},TransCore);
                    if(count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                }
                string loginsert = @"INSERT INTO orderlog(OID,SoID,Type,LogDate,UserName,Title,Remark,CoID) 
                                            VALUES(@OID,@SoID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                count =CoreDBconn.Execute(loginsert,logs,TransCore);
                if(count < 0)
                {
                    result.s = -3002;
                    return result;
                }                
                TransCore.Commit();
            }catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }
            var ress = new UpdateOrd();
            var aa = GetOrderEdit(OID,CoID);
            if(aa.s == -1)
            {
                result.s = -1;
                result.d = aa.d;
                return result;
            }
            ress.Order = aa.d as OrderEdit;
            aa = GetOrderLog(OID,CoID);
            if(aa.s == -1)
            {
                result.s = -1;
                result.d = aa.d;
                return result;
            }
            ress.Log = aa.d as List<OrderLog>;
            result.d = ress;
            return  result;
        }
        ///<summary>
        ///检查订单是否需取消等待合并
        ///</summary>
        public static DataResult isCheckCancleMerge(int OID,int CoID,string BuyerShopID,string RecName,string RecLogistics,string RecCity,string RecDistrict,string RecAddress)
        {
            var result = new DataResult(1,null);   
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string wheresql = "select id from `order` where coid = " + CoID + " and id != "+ OID + " and buyershopid = '" + BuyerShopID + "' and recname = '" + RecName + 
                                      "' and reclogistics = '" + RecLogistics + "' and reccity = '" + RecCity + "' and recdistrict = '" + RecDistrict + 
                                      "' and recaddress = '" + RecAddress + "' and status = 7 and statusdec = '等待订单合并'";
                    var u = conn.Query<Order>(wheresql).AsList();
                    if(u.Count == 1)
                    {
                        result.s = 1;
                        result.d = u[0].ID;
                    }
                    else
                    {
                        result.s = 0;
                    }
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            return result;
        }
        ///<summary>
        ///查询买家账号
        ///</summary>
        public static DataResult GetRecInfoList(RecInfoParm cp)
        {
            var result = new DataResult(1,null);    
            string sqlcount = "select count(id) from recinfo where 1=1";
            string sqlcommand = "select ID,BuyerId,ShopSit,Receiver,Logistics,City,District,Address,Tel,Phone from recinfo where 1=1"; 
            string wheresql = string.Empty;
            if(cp.CoID != 1)//公司编号
            {
                wheresql = wheresql + " and coid = " + cp.CoID;
            }
            if(!string.IsNullOrEmpty(cp.BuyerId))
            {
                wheresql = wheresql + " and buyerid = '" + cp.BuyerId + "'";
            }
            if(!string.IsNullOrEmpty(cp.Receiver))
            {
                wheresql = wheresql + " and receiver = '" + cp.Receiver + "'";
            }
            if(!string.IsNullOrEmpty(cp.ShopSit))
            {
                wheresql = wheresql + " and shopsit = '" + cp.ShopSit + "'";
            }
            if(!string.IsNullOrEmpty(cp.SortField) && !string.IsNullOrEmpty(cp.SortDirection))//排序
            {
                wheresql = wheresql + " ORDER BY "+ cp.SortField +" "+ cp.SortDirection;
            }
            var res = new RecInfoData();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    int count = conn.QueryFirst<int>(sqlcount + wheresql);
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(cp.NumPerPage.ToString()));
                    int dataindex = (cp.PageIndex - 1)* cp.NumPerPage;
                    wheresql = wheresql + " limit " + dataindex.ToString() + " ," + cp.NumPerPage.ToString();
                    var u = conn.Query<RecInfo>(sqlcommand + wheresql).AsList();
                    res.Datacnt = count;
                    res.Pagecnt = pagecnt;
                    res.Recinfo = u;
                    result.d = res;             
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }           
            return result;
        }
        ///<summary>
        ///新增订单明细
        ///</summary>
        public static DataResult InsertOrderDetail(int id,List<int> skuid,int CoID,string Username,bool IsQuick)
        {
            var result = new DataResult(1,null);  
            var res = new OrderDetailInsert();
            // var sin = new SingleOrderItem();
            var logs = new List<Log>();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string wheresql = "select status,soid,amount,PaidAmount from `order` where id =" + id + " and coid =" + CoID;
                var u = CoreDBconn.Query<Order>(wheresql).AsList();
                if (u.Count == 0)
                {
                    result.s = -1;
                    result.d = "此订单不存在!";
                    return result;
                }
                else
                {
                    if (u[0].Status != 0 && u[0].Status != 1 && u[0].Status != 7)
                    {
                        result.s = -1;
                        result.d = "只有待付款/已付款待审核/异常的订单才可以新增明细!";
                        return result;
                    }
                }
                List<InsertFailReason> rt = new List<InsertFailReason>();
                List<int> rr = new List<int>();
                decimal amt = 0, weight = 0;
                foreach (int a in skuid)
                {
                    InsertFailReason rf = new InsertFailReason();
                    string skusql = "select skuid,skuname,norm,img,goodscode,enable,saleprice,weight from coresku where id =" + a + " and coid =" + CoID;
                    var s = CoreDBconn.Query<SkuInsert>(skusql).AsList();
                    if (s.Count == 0)
                    {
                        rf.id = a;
                        rf.reason = "此商品不存在!";
                        rt.Add(rf);
                        continue;
                    }
                    else
                    {
                        if (s[0].enable == false)
                        {
                            rf.id = a;
                            rf.reason = "此商品已停用!";
                            rt.Add(rf);
                            continue;
                        }
                    }
                    int x = CoreDBconn.QueryFirst<int>("select count(id) from orderitem where oid = " + id + " and coid =" + CoID + " and skuautoid = " + a + " AND IsGift = false");
                    if (x > 0)
                    {
                        rf.id = a;
                        rf.reason = null;
                        rt.Add(rf);
                        continue;
                    }
                    string sqlCommandText = @"INSERT INTO orderitem(oid,soid,coid,skuautoid,skuid,skuname,norm,GoodsCode,qty,saleprice,realprice,amount,img,weight,totalweight,DiscountRate,creator,modifier) 
                                            VALUES(@OID,@Soid,@Coid,@Skuautoid,@Skuid,@Skuname,@Norm,@GoodsCode,@Qty,@Saleprice,@Saleprice,@Saleprice,@Img,@Weight,@Weight,@Qty,@Creator,@Creator)";
                    var args = new
                    {
                        OID = id,
                        Soid = u[0].SoID,
                        Skuautoid = a,
                        Skuid = s[0].skuid,
                        Skuname = s[0].skuname,
                        Norm = s[0].norm,
                        GoodsCode = s[0].goodscode,
                        Qty = 1,
                        Saleprice = s[0].saleprice,
                        Img = s[0].img,
                        Weight = s[0].weight,
                        Coid = CoID,
                        Creator = Username
                    };
                    int count = CoreDBconn.Execute(sqlCommandText, args, TransCore);
                    if (count <= 0)
                    {
                        rf.id = a;
                        rf.reason = "新增明细失败!";
                        rt.Add(rf);
                    }
                    else
                    {
                        amt += decimal.Parse(s[0].saleprice);
                        weight += decimal.Parse(s[0].weight);
                        rr.Add(a);
                        var log = new Log();
                        log.OID = id;
                        log.SoID = u[0].SoID;
                        log.Type = 0;
                        log.LogDate = DateTime.Now;
                        log.UserName = Username;
                        log.Title = "添加商品";
                        log.Remark = s[0].skuid;
                        log.CoID = CoID;
                        logs.Add(log);
                    }
                }
                //更新订单的金额和重量
                if (rr.Count > 0)
                {
                    bool IsPaid;
                    int status = u[0].Status;
                    if(decimal.Parse(u[0].Amount) + amt == decimal.Parse(u[0].PaidAmount))
                    {
                        IsPaid = true;
                        if(status != 7)
                        {
                            status = 1;
                        }
                    }
                    else
                    {
                        IsPaid = false;
                        if(status != 7)
                        {
                            status = 0;
                        }
                    }
                    string sqlCommandText = @"update `order` set SkuAmount = SkuAmount + @SkuAmount,Amount = SkuAmount + ExAmount,ExWeight = ExWeight + @ExWeight,
                                            OrdQty = OrdQty + @OrdQty,Modifier=@Modifier,ModifyDate=@ModifyDate,IsPaid=@IsPaid,status=@Status where ID = @ID and CoID = @CoID";
                    int count = CoreDBconn.Execute(sqlCommandText, new { SkuAmount = amt, ExWeight = weight, OrdQty = rr.Count,Modifier = Username, ModifyDate = DateTime.Now,
                                                    IsPaid=IsPaid,Status=status, ID = id, CoID = CoID }, TransCore);
                    if (count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                    string loginsert = @"INSERT INTO orderlog(OID,SoID,Type,LogDate,UserName,Title,Remark,CoID) 
                                            VALUES(@OID,@SoID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                    int r = CoreDBconn.Execute(loginsert,logs, TransCore);
                    if (r < 0)
                    {
                        result.s = -3002;
                        return result;
                    }
                }
                res.successIDs = rr;
                res.failIDs = rt;
                
                if (result.s == 1)
                {
                    TransCore.Commit();
                }

                // wheresql = "select status,amount,ExWeight from `order` where id =" + id + " and coid =" + CoID;
                // u = CoreDBconn.Query<Order>(wheresql).AsList();
                // sin.Amount = u[0].Amount;
                // sin.Status = u[0].Status;
                // sin.StatusDec = Enum.GetName(typeof(OrdStatus), u[0].Status);
                // sin.Weight = u[0].ExWeight;
            }
            catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }
            if(IsQuick == true)
            {
                var ff = RefreshOrderItemQuick(id,CoID);
                if(ff.s == -1)
                {
                    result.s = -1;
                    result.d = ff.d;
                    return result;
                }
                res.Order = ff.d as SingleOrderItem;
                result.d = res;
            }
            else
            {
                var ress = new OrderDetailInsertI();
                ress.successIDs = res.successIDs;
                ress.failIDs = res.failIDs;
                var ff = RefreshOrderItem(id,CoID);
                if(ff.s == -1)
                {
                    result.s = -1;
                    result.d = ff.d;
                    return result;
                }
                ress.Order = ff.d as RefreshItem;
                result.d = ress;
            }
            
            return result;
        }
        ///<summary>
        ///删除订单明细
        ///</summary>
        public static DataResult DeleteOrderDetail(int id,int oid,int CoID,string Username,bool IsQuick)
        {
            var result = new DataResult(1,null);  
            var logs = new List<Log>();
            // var sin = new SingleOrderItem();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string wheresql = "select status,soid,amount,PaidAmount from `order` where id =" + oid + " and coid =" + CoID;
                var u = CoreDBconn.Query<Order>(wheresql).AsList();
                if (u.Count == 0)
                {
                    result.s = -1;
                    result.d = "此订单不存在!";
                    return result;
                }
                else
                {
                    if (u[0].Status != 0 && u[0].Status != 1 && u[0].Status != 7)
                    {
                        result.s = -1;
                        result.d = "只有待付款/已付款待审核/异常的订单才可以删除明细!";
                        return result;
                    }
                }
                decimal amt = 0, weight = 0,qty = 0;
                wheresql = @"select id,skuid,realprice,qty,amount,totalweight from orderitem where oid = " + oid + " and coid =" + CoID + " and id = " + id;
                var x = CoreDBconn.Query<OrderItem>(wheresql).AsList();
                if (x.Count > 0)
                {
                    qty += x[0].Qty;
                    amt += decimal.Parse(x[0].Amount);
                    weight += decimal.Parse(x[0].TotalWeight);
                    var log = new Log();
                    log.OID = oid;
                    log.SoID = u[0].SoID;
                    log.Type = 0;
                    log.LogDate = DateTime.Now;
                    log.UserName = Username;
                    log.Title = "删除商品";
                    log.Remark = x[0].SkuID + "(" + x[0].RealPrice + "*" + x[0].Qty + ")";
                    log.CoID = CoID;
                    logs.Add(log);
                }
                string sqlCommandText = @"delete from orderitem where oid = @OID and coid = @CoID and id = @ID";
                int count = CoreDBconn.Execute(sqlCommandText,new {OID=oid,CoID=CoID,ID = id}, TransCore);
                if (count < 0)
                {
                    result.s = -3004;
                    return result;
                }
                bool IsPaid;
                int status = u[0].Status;
                if(decimal.Parse(u[0].Amount) - amt == decimal.Parse(u[0].PaidAmount))
                {
                    IsPaid = true;
                    if(status != 7)
                    {
                        status = 1;
                    }
                }
                else
                {
                    IsPaid = false;
                    if(status != 7)
                    {
                        status = 0;
                    }
                }
                //更新订单的金额和重量
                sqlCommandText = @"update `order` set SkuAmount = SkuAmount - @SkuAmount,Amount = SkuAmount + ExAmount,ExWeight = ExWeight - @ExWeight,
                                  OrdQty = OrdQty - @OrdQty, Modifier=@Modifier,ModifyDate=@ModifyDate,IsPaid=@IsPaid,status=@Status  where ID = @ID and CoID = @CoID";
                count = CoreDBconn.Execute(sqlCommandText, new { SkuAmount = amt, ExWeight = weight, OrdQty = qty,Modifier = Username, ModifyDate = DateTime.Now, 
                                            IsPaid=IsPaid,Status=status,ID = oid, CoID = CoID }, TransCore);
                if (count < 0)
                {
                    result.s = -3003;
                    return result;
                }
                
                string loginsert = @"INSERT INTO orderlog(OID,SoID,Type,LogDate,UserName,Title,Remark,CoID) 
                                            VALUES(@OID,@SoID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                int r = CoreDBconn.Execute(loginsert,logs, TransCore);
                if (r < 0)
                {
                    result.s = -3002;
                    return result;
                }
                TransCore.Commit();
                // wheresql = "select status,amount,ExWeight from `order` where id =" + oid + " and coid =" + CoID;
                // u = CoreDBconn.Query<Order>(wheresql).AsList();
                // sin.Amount = u[0].Amount;
                // sin.Status = u[0].Status;
                // sin.StatusDec = Enum.GetName(typeof(OrdStatus), u[0].Status);
                // sin.Weight = u[0].ExWeight;
            }
            catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }
            if(IsQuick == true)
            {
                var ff = RefreshOrderItemQuick(oid,CoID);
                if(ff.s == -1)
                {
                    result.s = -1;
                    result.d = ff.d;
                    return result;
                }
                result.d = ff.d;
            }
            else
            {
                var ff = RefreshOrderItem(oid,CoID);
                if(ff.s == -1)
                {
                    result.s = -1;
                    result.d = ff.d;
                    return result;
                }
                result.d = ff.d;
            }
            return result;
        }
        ///<summary>
        ///更新订单明细
        ///</summary>
        public static DataResult UpdateOrderDetail(int id,int oid,int CoID,string Username,decimal price,int qty,string SkuName,bool IsQuick)
        {
            var result = new DataResult(1,null);  
            var logs = new List<Log>();
            // var sin = new SingleOrderItem();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string wheresql = "select status,soid,amount,PaidAmount from `order` where id =" + oid + " and coid =" + CoID;
                var u = CoreDBconn.Query<Order>(wheresql).AsList();
                if (u.Count == 0)
                {
                    result.s = -1;
                    result.d = "此订单不存在!";
                    return result;
                }
                else
                {
                    if (u[0].Status != 0 && u[0].Status != 1 && u[0].Status != 7)
                    {
                        result.s = -1;
                        result.d = "只有待付款/已付款待审核/异常的订单才可以修改明细!";
                        return result;
                    }
                }
                string sqlCommandText = "update orderitem set ";
                var p = new DynamicParameters();
                decimal amt = 0, weight = 0,pricenew = 0,qtynew = 0;
                wheresql = @"select id,skuid,realprice,qty,amount,totalweight,weight,saleprice,skuname from orderitem 
                             where oid = " + oid + " and coid =" + CoID + " and id = " + id;
                var x = CoreDBconn.Query<OrderItem>(wheresql).AsList();
                if (x.Count > 0)
                {
                    amt = decimal.Parse(x[0].Amount);
                    weight = decimal.Parse(x[0].TotalWeight);
                    pricenew = decimal.Parse(x[0].RealPrice);
                    qtynew = x[0].Qty;
                    if(price != -1)
                    {
                        if(price != decimal.Parse(x[0].RealPrice))
                        {
                            pricenew = price;
                            sqlCommandText = sqlCommandText + "realprice = @Realprice,";
                            p.Add("@Realprice", price);
                            var log = new Log();
                            log.OID = oid;
                            log.SoID = u[0].SoID;
                            log.Type = 0;
                            log.LogDate = DateTime.Now;
                            log.UserName = Username;
                            log.Title = "修改单价";
                            log.Remark = x[0].SkuID + " " + x[0].RealPrice + "=>" + price;
                            log.CoID = CoID;
                            logs.Add(log);
                        }
                    }
                    if(qty != -1)
                    {
                        if(qty != x[0].Qty)
                        {
                            qtynew = qty;
                            sqlCommandText = sqlCommandText + "qty = @Qty,";
                            p.Add("@Qty", qty);
                            var log = new Log();
                            log.OID = oid;
                            log.SoID = u[0].SoID;
                            log.Type = 0;
                            log.LogDate = DateTime.Now;
                            log.UserName = Username;
                            log.Title = "修改数量";
                            log.Remark = x[0].SkuID+ " " + x[0].Qty + "=>" + qtynew;
                            log.CoID = CoID;
                            logs.Add(log);
                        }
                    }
                    if(!string.IsNullOrEmpty(SkuName))
                    {
                        if(SkuName != x[0].SkuName)
                        {
                            sqlCommandText = sqlCommandText + "SkuName = @SkuName,";
                            p.Add("@SkuName", SkuName);
                            var log = new Log();
                            log.OID = oid;
                            log.SoID = u[0].SoID;
                            log.Type = 0;
                            log.LogDate = DateTime.Now;
                            log.UserName = Username;
                            log.Title = "修改商品名称";
                            log.Remark = x[0].SkuID + " " + x[0].SkuName + "=>" + SkuName;
                            log.CoID = CoID;
                            logs.Add(log);
                        }
                    }
                    sqlCommandText = sqlCommandText + "amount = @Amount,DiscountRate = @DiscountRate,TotalWeight=@TotalWeight,modifier=@Modifier,ModifyDate=@ModifyDate " + 
                                                       "where oid = @Oid and coid = @Coid and id = @ID";
                    p.Add("@Amount", pricenew * qtynew);
                    p.Add("@DiscountRate", pricenew/decimal.Parse(x[0].SalePrice));
                    p.Add("@TotalWeight", qtynew * decimal.Parse(x[0].Weight));
                    p.Add("@Modifier", Username);
                    p.Add("@ModifyDate", DateTime.Now);
                    p.Add("@Oid", oid);
                    p.Add("@Coid", CoID);
                    p.Add("@ID", id);
                    int count = CoreDBconn.Execute(sqlCommandText, p, TransCore);
                    if (count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                    bool IsPaid;
                    int status = u[0].Status;
                    if(decimal.Parse(u[0].Amount) + pricenew * qtynew - amt == decimal.Parse(u[0].PaidAmount))
                    {
                        IsPaid = true;
                        if(status != 7)
                        {
                            status = 1;
                        }
                    }
                    else
                    {
                        IsPaid = false;
                        if(status != 7)
                        {
                            status = 0;
                        }
                    }
                    sqlCommandText = @"update `order` set SkuAmount = SkuAmount + @SkuAmount,Amount = SkuAmount + ExAmount,ExWeight = ExWeight + @ExWeight,
                                        ordqty = ordqty + @Ordqty ,Modifier=@Modifier,ModifyDate=@ModifyDate,IsPaid=@IsPaid,status=@Status 
                                        where ID = @ID and CoID = @CoID";
                    count = CoreDBconn.Execute(sqlCommandText, new { SkuAmount = pricenew * qtynew - amt, ExWeight = qtynew * decimal.Parse(x[0].Weight) - weight,
                                                Ordqty = qtynew - x[0].Qty, Modifier = Username, ModifyDate = DateTime.Now,IsPaid=IsPaid,Status=status, ID = oid, CoID = CoID }, TransCore);
                    if (count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                    string loginsert = @"INSERT INTO orderlog(OID,SoID,Type,LogDate,UserName,Title,Remark,CoID) 
                                                VALUES(@OID,@SoID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                    int r = CoreDBconn.Execute(loginsert,logs, TransCore);
                    if (r < 0)
                    {
                        result.s = -3002;
                        return result;
                    }
                }
                TransCore.Commit();
                // wheresql = "select status,amount,ExWeight from `order` where id =" + oid + " and coid =" + CoID;
                // u = CoreDBconn.Query<Order>(wheresql).AsList();
                // sin.Amount = u[0].Amount;
                // sin.Status = u[0].Status;
                // sin.StatusDec = Enum.GetName(typeof(OrdStatus), u[0].Status);
                // sin.Weight = u[0].ExWeight;
            }
            catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }
            if(IsQuick == true)
            {
                var ff = RefreshOrderItemQuick(oid,CoID);
                if(ff.s == -1)
                {
                    result.s = -1;
                    result.d = ff.d;
                    return result;
                }
                result.d = ff.d;
            }
            else
            {
                var ff = RefreshOrderItem(oid,CoID);
                if(ff.s == -1)
                {
                    result.s = -1;
                    result.d = ff.d;
                    return result;
                }
                result.d = ff.d;
            }
            return result;
        }
        ///<summary>
        ///手工支付
        ///</summary>
        public static DataResult ManualPay(PayInfo pay,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            var logs = new List<Log>();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string wheresql = "select * from `order` where id =" + pay.OID + " and coid =" + CoID;
                var u = CoreDBconn.Query<Order>(wheresql).AsList();
                var ord = new Order();
                if (u.Count == 0)
                {
                    result.s = -1;
                    result.d = "此订单不存在!";
                    return result;
                }
                else
                {
                    ord = u[0] as Order;
                    if (ord.Status != 0 && ord.Status != 1 && ord.Status != 7)
                    {
                        result.s = -1;
                        result.d = "只有待付款/已付款待审核/异常的订单才可以新增付款!";
                        return result;
                    }
                }
                pay.SoID = ord.SoID;
                decimal PaidAmount=0,PayAmount=0,Amount=0;
                if(!string.IsNullOrEmpty(ord.PaidAmount))
                {
                    PaidAmount = decimal.Parse(ord.PaidAmount);
                }
                if(!string.IsNullOrEmpty(ord.PayAmount))
                {
                    PayAmount = decimal.Parse(ord.PayAmount);
                }
                if(!string.IsNullOrEmpty(ord.Amount))
                {
                    Amount = decimal.Parse(ord.Amount);
                }
                if(Amount - PaidAmount <= 0)
                {
                    result.s = -1;
                    result.d = "该笔订单已完成支付，不需再支付!";
                    return result;
                }
                pay.RecID = ord.BuyerID;
                pay.RecName = ord.RecName;
                pay.Title = ord.InvoiceTitle;
                pay.DataSource = 0;
                pay.Status = 1;
                pay.CoID = CoID;
                pay.Creator = UserName;
                pay.CreateDate = DateTime.Now;
                pay.Confirmer = UserName;
                pay.ConfirmDate = DateTime.Now;
                var log = new Log();
                log.OID = pay.OID;
                log.SoID = pay.SoID;
                log.Type = 0;
                log.LogDate = DateTime.Now;
                log.UserName = UserName;
                log.Title = "添加支付";
                log.Remark = pay.Payment + pay.PayAmount;
                log.CoID = CoID;
                logs.Add(log);
                if(PaidAmount + decimal.Parse(pay.PayAmount) != Amount &&　ord.Status != 7 )
                {
                    log = new Log();
                    log.OID = pay.OID;
                    log.SoID = pay.SoID;
                    log.Type = 0;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "判断金额不符";
                    log.Remark = "标记部分付款";
                    log.CoID = CoID;
                    logs.Add(log);
                    ord.Status = 7;
                    var rss = GetReasonID("部分付款",CoID);
                    if(rss.s == -1)
                    {
                        result.s = -1;
                        result.d = rss.d;
                        return result;
                    }
                    ord.AbnormalStatus = rss.s;
                    ord.StatusDec = "部分付款";
                }
                if(PaidAmount + decimal.Parse(pay.PayAmount) == Amount &&　ord.StatusDec == "部分付款" && ord.Status == 7)
                {
                    log = new Log();
                    log.OID = pay.OID;
                    log.SoID = pay.SoID;
                    log.Type = 0;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "取消异常标记";
                    log.Remark = ord.StatusDec;
                    log.CoID = CoID;
                    logs.Add(log);
                    ord.Status = 1;
                    ord.AbnormalStatus = 0;
                    ord.StatusDec = "";
                }
                log = new Log();
                log.OID = pay.OID;
                log.SoID = pay.SoID;
                log.Type = 0;
                log.LogDate = DateTime.Now;
                log.UserName = UserName;
                log.Title = "支付单确认";
                log.CoID = CoID;
                logs.Add(log);
                //新增支付单资料
                string sqlCommandText = @"INSERT INTO payinfo(PayNbr,RecID,RecName,OID,SOID,Payment,PayAccount,PayDate,Title,Amount,PayAmount,DataSource,Status,CoID,Creator,CreateDate,Confirmer,ConfirmDate) 
                                    VALUES(@PayNbr,@RecID,@RecName,@OID,@SOID,@Payment,@PayAccount,@PayDate,@Title,@Amount,@PayAmount,@DataSource,@Status,@CoID,@Creator,@CreateDate,@Confirmer,@ConfirmDate)";
                int count = CoreDBconn.Execute(sqlCommandText,pay,TransCore);
                if(count < 0)
                {
                    result.s = -3002;
                    return result;
                }
                //更新订单
                ord.PaidAmount = (PaidAmount + decimal.Parse(pay.PayAmount)).ToString();
                ord.PayAmount = (PayAmount + decimal.Parse(pay.PayAmount)).ToString();
                if(ord.PayDate == null || ord.PayDate <= DateTime.Parse("1900-01-01"))
                {
                    ord.PayDate = pay.PayDate;
                }
                if(string.IsNullOrEmpty(ord.PayNbr))
                {
                    ord.PayNbr = pay.PayNbr;
                }
                if(ord.PaidAmount == ord.Amount)
                {
                    ord.IsPaid = true;
                    if(ord.Status != 7)
                    {
                        ord.Status = 1;
                        ord.AbnormalStatus = 0;
                        ord.StatusDec = "";
                    }
                }
                else
                {
                    ord.IsPaid = false;
                    if(ord.Status != 7)
                    {
                        ord.Status = 0;
                        ord.AbnormalStatus = 0;
                        ord.StatusDec = "";
                    }
                }
                ord.Modifier = UserName;
                ord.ModifyDate = DateTime.Now;
                sqlCommandText = @"update `order` set PaidAmount = @PaidAmount,PayAmount = @PayAmount,PayDate =@PayDate,PayNbr = @PayNbr,IsPaid=@IsPaid,Status=@Status,AbnormalStatus=@AbnormalStatus,
                                    StatusDec=@StatusDec,Modifier=@Modifier,ModifyDate=@ModifyDate where ID = @ID and CoID = @CoID";
                count = CoreDBconn.Execute(sqlCommandText,ord, TransCore);
                if (count < 0)
                {
                    result.s = -3003;
                    return result;
                }
                string loginsert = @"INSERT INTO orderlog(OID,SoID,Type,LogDate,UserName,Title,Remark,CoID) 
                                                VALUES(@OID,@SoID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                count = CoreDBconn.Execute(loginsert,logs, TransCore);
                if (count < 0)
                {
                    result.s = -3002;
                    return result;
                }
                TransCore.Commit();
            }
            catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }
            var res = new UpdatePay();
            var aa = GetOrderEdit(pay.OID,CoID);
            if(aa.s == -1)
            {
                result.s = -1;
                result.d = aa.d;
                return result;
            }
            res.Order = aa.d as OrderEdit;

            aa = GetOrderPay(pay.OID,CoID);
            if(aa.s == -1)
            {
                result.s = -1;
                result.d = aa.d;
                return result;
            }
            res.Pay = aa.d as List<OrderPay>;

            aa = GetOrderLog(pay.OID,CoID);
            if(aa.s == -1)
            {
                result.s = -1;
                result.d = aa.d;
                return result;
            }
            res.Log = aa.d as List<OrderLog>;
            result.d = res;
            return result;
        }
        ///<summary>
        ///取消支付审核
        ///</summary>
        public static DataResult CancleConfirmPay(int oid,int payid,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            var logs = new List<Log>();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string wheresql = "select * from `order` where id =" + oid  + " and coid =" + CoID;
                var u = CoreDBconn.Query<Order>(wheresql).AsList();
                var ord = new Order();
                if (u.Count == 0)
                {
                    result.s = -1;
                    result.d = "此订单不存在!";
                    return result;
                }
                else
                {
                    ord = u[0] as Order;
                    if (ord.Status != 0 && ord.Status != 1 && ord.Status != 7)
                    {
                        result.s = -1;
                        result.d = "只有待付款/已付款待审核/异常的订单才可以取消核准付款";
                        return result;
                    }
                }
                decimal PaidAmount=0,PayAmount=0,Amount=0;
                if(!string.IsNullOrEmpty(ord.PaidAmount))
                {
                    PaidAmount = decimal.Parse(ord.PaidAmount);
                }
                if(!string.IsNullOrEmpty(ord.PayAmount))
                {
                    PayAmount = decimal.Parse(ord.PayAmount);
                }
                if(!string.IsNullOrEmpty(ord.Amount))
                {
                    Amount = decimal.Parse(ord.Amount);
                }
                var log = new Log();
                log.OID = oid;
                log.SoID = ord.SoID;
                log.Type = 0;
                log.LogDate = DateTime.Now;
                log.UserName = UserName;
                log.Title = "支付单取消确认";
                log.CoID = CoID;
                logs.Add(log);
                wheresql = "select PayAmount,status from payinfo where id =" + payid + " and coid =" + CoID;
                var payinfo = CoreDBconn.Query<PayInfo>(wheresql).AsList();
                if(payinfo.Count == 0)
                {
                    result.s = -1;
                    result.d = "该笔支付单参数异常!";
                    return result;
                }
                else
                {
                    if(payinfo[0].Status != 1)
                    {
                        result.s = -1;
                        result.d = "该笔支付单不可取消确认!";
                        return result;
                    }
                }
                decimal pay = decimal.Parse(payinfo[0].PayAmount);
                if(PaidAmount - pay != Amount && PaidAmount - pay > 0 &&　ord.Status != 7)
                {
                    log = new Log();
                    log.OID = oid;
                    log.SoID = ord.SoID;
                    log.Type = 0;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "判断金额不符";
                    log.Remark = "标记部分付款";
                    log.CoID = CoID;
                    logs.Add(log);
                    ord.Status = 7;
                    var rss = GetReasonID("部分付款",CoID);
                    if(rss.s == -1)
                    {
                        result.s = -1;
                        result.d = rss.d;
                        return result;
                    }
                    ord.AbnormalStatus = rss.s;
                    ord.StatusDec = "部分付款";
                }
                if(PaidAmount - pay == 0 &&　ord.StatusDec == "部分付款" && ord.Status == 7)
                {
                    log = new Log();
                    log.OID = oid;
                    log.SoID = ord.SoID;
                    log.Type = 0;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "取消异常标记";
                    log.Remark = ord.StatusDec;
                    log.CoID = CoID;
                    logs.Add(log);
                    ord.Status = 0;
                    ord.AbnormalStatus = 0;
                    ord.StatusDec = "";
                }
                if(PaidAmount - pay == Amount &&　ord.StatusDec == "部分付款" && ord.Status == 7)
                {
                    log = new Log();
                    log.OID = oid;
                    log.SoID = ord.SoID;
                    log.Type = 0;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "取消异常标记";
                    log.Remark = ord.StatusDec;
                    log.CoID = CoID;
                    logs.Add(log);
                    ord.Status = 1;
                    ord.AbnormalStatus = 0;
                    ord.StatusDec = "";
                }
                //更新支付单资料
                string sqlCommandText = @"update payinfo set Status = 0,Confirmer=@Confirmer,ConfirmDate = @ConfirmDate where id = @ID and coid = @Coid";
                int count = CoreDBconn.Execute(sqlCommandText,new {Confirmer = "",ConfirmDate=new DateTime(),ID = payid,Coid = CoID },TransCore);
                if(count < 0)
                {
                    result.s = -3003;
                    return result;
                }
                //更新订单
                ord.PaidAmount = (PaidAmount - pay).ToString();
                ord.PayAmount = (PayAmount - pay).ToString();
                ord.IsPaid = false;
                if(decimal.Parse(ord.PaidAmount) == 0)
                {
                    ord.PayDate = new DateTime();
                    ord.PayNbr = null;
                }
                if(ord.PaidAmount == ord.Amount)
                {
                    ord.IsPaid = true;
                    if(ord.Status != 7)
                    {
                        ord.Status = 1;
                        ord.AbnormalStatus = 0;
                        ord.StatusDec = "";
                    }
                }
                else
                {
                    ord.IsPaid = false;
                    if(ord.Status != 7)
                    {
                        ord.Status = 0;
                        ord.AbnormalStatus = 0;
                        ord.StatusDec = "";
                    }
                }
                ord.Modifier = UserName;
                ord.ModifyDate = DateTime.Now;
                sqlCommandText = @"update `order` set PaidAmount = @PaidAmount,PayAmount = @PayAmount,PayDate =@PayDate,PayNbr = @PayNbr,IsPaid=@IsPaid,Status=@Status,AbnormalStatus=@AbnormalStatus,
                                    StatusDec=@StatusDec,Modifier=@Modifier,ModifyDate=@ModifyDate where ID = @ID and CoID = @CoID";
                count = CoreDBconn.Execute(sqlCommandText,ord, TransCore);
                if (count < 0)
                {
                    result.s = -3003;
                    return result;
                }
                string loginsert = @"INSERT INTO orderlog(OID,SoID,Type,LogDate,UserName,Title,Remark,CoID) 
                                                VALUES(@OID,@SoID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                count = CoreDBconn.Execute(loginsert,logs, TransCore);
                if (count < 0)
                {
                    result.s = -3002;
                    return result;
                }
                TransCore.Commit();
            }
            catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }
            
            var res = new UpdatePay();
            var aa = GetOrderEdit(oid,CoID);
            if(aa.s == -1)
            {
                result.s = -1;
                result.d = aa.d;
                return result;
            }
            res.Order = aa.d as OrderEdit;

            aa = GetOrderPay(oid,CoID);
            if(aa.s == -1)
            {
                result.s = -1;
                result.d = aa.d;
                return result;
            }
            res.Pay = aa.d as List<OrderPay>;

            aa = GetOrderLog(oid,CoID);
            if(aa.s == -1)
            {
                result.s = -1;
                result.d = aa.d;
                return result;
            }
            res.Log = aa.d as List<OrderLog>;
            result.d = res;

            return result;
        }
        ///<summary>
        ///支付审核
        ///</summary>
        public static DataResult ConfirmPay(int oid,int payid,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            var logs = new List<Log>();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string wheresql = "select * from `order` where id =" + oid  + " and coid =" + CoID;
                var u = CoreDBconn.Query<Order>(wheresql).AsList();
                var ord = new Order();
                if (u.Count == 0)
                {
                    result.s = -1;
                    result.d = "此订单不存在!";
                    return result;
                }
                else
                {
                    ord = u[0] as Order;
                    if (ord.Status != 0 && ord.Status != 1 && ord.Status != 7)
                    {
                        result.s = -1;
                        result.d = "只有待付款/已付款待审核/异常的订单才可以核准付款!";
                        return result;
                    }
                }
                decimal PaidAmount=0,PayAmount=0,Amount=0;
                if(!string.IsNullOrEmpty(ord.PaidAmount))
                {
                    PaidAmount = decimal.Parse(ord.PaidAmount);
                }
                if(!string.IsNullOrEmpty(ord.PayAmount))
                {
                    PayAmount = decimal.Parse(ord.PayAmount);
                }
                if(!string.IsNullOrEmpty(ord.Amount))
                {
                    Amount = decimal.Parse(ord.Amount);
                }
                if(Amount - PaidAmount <= 0)
                {
                    result.s = -1;
                    result.d = "该笔订单已完成支付，不需再支付!";
                    return result;
                }
                var log = new Log();
                log.OID = oid;
                log.SoID = ord.SoID;
                log.Type = 0;
                log.LogDate = DateTime.Now;
                log.UserName = UserName;
                log.Title = "支付单确认";
                log.CoID = CoID;
                logs.Add(log);
                wheresql = "select PayAmount,status,paydate,paynbr from payinfo where id =" + payid + " and coid =" + CoID;
                var payinfo = CoreDBconn.Query<PayInfo>(wheresql).AsList();
                if(payinfo.Count == 0)
                {
                    result.s = -1;
                    result.d = "该笔支付单参数异常!";
                    return result;
                }
                else
                {
                    if(payinfo[0].Status != 0)
                    {
                        result.s = -1;
                        result.d = "该笔支付单不可确认!";
                        return result;
                    }
                }
                decimal pay = decimal.Parse(payinfo[0].PayAmount);
                if(PaidAmount +  pay != Amount &&　ord.Status != 7)
                {
                    log = new Log();
                    log.OID = oid;
                    log.SoID = ord.SoID;
                    log.Type = 0;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "判断金额不符";
                    log.Remark = "标记部分付款";
                    log.CoID = CoID;
                    logs.Add(log);
                    ord.Status = 7;
                    var rss = GetReasonID("部分付款",CoID);
                    if(rss.s == -1)
                    {
                        result.s = -1;
                        result.d = rss.d;
                        return result;
                    }
                    ord.AbnormalStatus = rss.s;
                    ord.StatusDec = "部分付款";
                }
                if(PaidAmount + pay == Amount &&　ord.StatusDec == "部分付款" && ord.Status == 7)
                {
                    log = new Log();
                    log.OID = oid;
                    log.SoID = ord.SoID;
                    log.Type = 0;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "取消异常标记";
                    log.Remark = ord.StatusDec;
                    log.CoID = CoID;
                    logs.Add(log);
                    ord.Status = 1;
                    ord.AbnormalStatus = 0;
                    ord.StatusDec = "";
                }
                //更新支付单资料
                string sqlCommandText = @"update payinfo set Status = 1,Confirmer=@Confirmer,ConfirmDate = @ConfirmDate where id = @ID and coid = @Coid";
                int count = CoreDBconn.Execute(sqlCommandText,new {Confirmer = UserName,ConfirmDate=DateTime.Now,ID = payid,Coid = CoID },TransCore);
                if(count < 0)
                {
                    result.s = -3003;
                    return result;
                }
                //更新订单
                ord.PaidAmount = (PaidAmount + pay).ToString();
                ord.PayAmount = (PayAmount + pay).ToString();
                if(ord.PayDate == null || ord.PayDate <= DateTime.Parse("1900-01-01"))
                {
                    ord.PayDate = payinfo[0].PayDate;
                }
                if(string.IsNullOrEmpty(ord.PayNbr))
                {
                    ord.PayNbr = payinfo[0].PayNbr;
                }
                if(ord.PaidAmount == ord.Amount)
                {
                    ord.IsPaid = true;
                    if(ord.Status != 7)
                    {
                        ord.Status = 1;
                        ord.AbnormalStatus = 0;
                        ord.StatusDec = "";
                    }
                }
                else
                {
                    ord.IsPaid = false;
                    if(ord.Status != 7)
                    {
                        ord.Status = 0;
                        ord.AbnormalStatus = 0;
                        ord.StatusDec = "";
                    }
                }
                ord.Modifier = UserName;
                ord.ModifyDate = DateTime.Now;
                sqlCommandText = @"update `order` set PaidAmount = @PaidAmount,PayAmount = @PayAmount,PayDate =@PayDate,PayNbr = @PayNbr,IsPaid=@IsPaid,Status=@Status,AbnormalStatus=@AbnormalStatus,
                                    StatusDec=@StatusDec,Modifier=@Modifier,ModifyDate=@ModifyDate where ID = @ID and CoID = @CoID";
                count = CoreDBconn.Execute(sqlCommandText,ord, TransCore);
                if (count < 0)
                {
                    result.s = -3003;
                    return result;
                }
                string loginsert = @"INSERT INTO orderlog(OID,SoID,Type,LogDate,UserName,Title,Remark,CoID) 
                                                VALUES(@OID,@SoID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                count = CoreDBconn.Execute(loginsert,logs, TransCore);
                if (count < 0)
                {
                    result.s = -3002;
                    return result;
                }
                TransCore.Commit();
            }
            catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }
            var res = new UpdatePay();
            var aa = GetOrderEdit(oid,CoID);
            if(aa.s == -1)
            {
                result.s = -1;
                result.d = aa.d;
                return result;
            }
            res.Order = aa.d as OrderEdit;

            aa = GetOrderPay(oid,CoID);
            if(aa.s == -1)
            {
                result.s = -1;
                result.d = aa.d;
                return result;
            }
            res.Pay = aa.d as List<OrderPay>;

            aa = GetOrderLog(oid,CoID);
            if(aa.s == -1)
            {
                result.s = -1;
                result.d = aa.d;
                return result;
            }
            res.Log = aa.d as List<OrderLog>;
            result.d = res;
            return result;
        }
        ///<summary>
        ///支付作废
        ///</summary>
        public static DataResult CanclePay(int oid,int payid,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            var logs = new List<Log>();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string wheresql = "select status,soid from `order` where id =" + oid + " and coid =" + CoID;
                var u = CoreDBconn.Query<Order>(wheresql).AsList();
                if (u.Count == 0)
                {
                    result.s = -1;
                    result.d = "此订单不存在!";
                    return result;
                }
                else
                {
                    if (u[0].Status != 0 && u[0].Status != 1 && u[0].Status != 7)
                    {
                        result.s = -1;
                        result.d = "只有待付款/已付款待审核/异常的订单才可以作废付款!";
                        return result;
                    }
                }
                var log = new Log();
                log.OID = oid;
                log.SoID = u[0].SoID;
                log.Type = 0;
                log.LogDate = DateTime.Now;
                log.UserName = UserName;
                log.Title = "支付单作废";
                log.CoID = CoID;
                logs.Add(log);
                wheresql = "select status from payinfo where id =" + payid + " and coid =" + CoID;
                var payinfo = CoreDBconn.Query<PayInfo>(wheresql).AsList();
                if(payinfo.Count == 0)
                {
                    result.s = -1;
                    result.d = "该笔支付单参数异常!";
                    return result;
                }
                else
                {
                    if(payinfo[0].Status != 0)
                    {
                        result.s = -1;
                        result.d = "该笔支付单不可作废!";
                        return result;
                    }
                }
                //更新支付单资料
                string sqlCommandText = @"update payinfo set Status = 2 where id = " + payid + " and coid = " + CoID;
                int count = CoreDBconn.Execute(sqlCommandText,TransCore);
                if(count < 0)
                {
                    result.s = -3003;
                    return result;
                }
                string loginsert = @"INSERT INTO orderlog(OID,SoID,Type,LogDate,UserName,Title,Remark,CoID) 
                                                VALUES(@OID,@SoID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                count = CoreDBconn.Execute(loginsert,logs, TransCore);
                if (count < 0)
                {
                    result.s = -3002;
                    return result;
                }
                TransCore.Commit();
            }
            catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }
            var res = new CanclePay();
            var aa = GetOrderPay(oid,CoID);
            if(aa.s == -1)
            {
                result.s = -1;
                result.d = aa.d;
                return result;
            }
            res.Pay = aa.d as List<OrderPay>;

            aa = GetOrderLog(oid,CoID);
            if(aa.s == -1)
            {
                result.s = -1;
                result.d = aa.d;
                return result;
            }
            res.Log = aa.d as List<OrderLog>;
            result.d = res;
            return result;
        }
        ///<summary>
        ///快速支付
        ///</summary>
        public static DataResult QuickPay(int id,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            var logs = new List<Log>();
            var pay = new PayInfo();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string wheresql = "select * from `order` where id =" + id + " and coid =" + CoID;
                var u = CoreDBconn.Query<Order>(wheresql).AsList();
                var ord = new Order();
                if (u.Count == 0)
                {
                    result.s = -1;
                    result.d = "此订单不存在!";
                    return result;
                }
                else
                {
                    ord = u[0] as Order;
                    if (ord.Status != 0 && ord.Status != 1 && ord.Status != 7)
                    {
                        result.s = -1;
                        result.d = "只有待付款/已付款待审核/异常的订单才可以新增付款!";
                        return result;
                    }
                }
                decimal PaidAmount=0,PayAmount=0,Amount=0;
                if(!string.IsNullOrEmpty(ord.PaidAmount))
                {
                    PaidAmount = decimal.Parse(ord.PaidAmount);
                }
                if(!string.IsNullOrEmpty(ord.PayAmount))
                {
                    PayAmount = decimal.Parse(ord.PayAmount);
                }
                if(!string.IsNullOrEmpty(ord.Amount))
                {
                    Amount = decimal.Parse(ord.Amount);
                }
                if(Amount - PaidAmount <= 0)
                {
                    result.s = -1;
                    result.d = "该笔订单已完成支付，不需再支付!";
                    return result;
                }
                pay.RecID = ord.BuyerID;
                pay.RecName = ord.RecName;
                pay.Title = ord.InvoiceTitle;
                pay.DataSource = 0;
                pay.Status = 1;
                pay.CoID = CoID;
                pay.OID = id;
                pay.SoID = u[0].SoID;
                pay.Payment = "快速支付";
                pay.PayNbr = "S" + DateTime.Now.Ticks.ToString().Substring(0,12);
                pay.PayDate = DateTime.Now;
                pay.PayAmount = (Amount - PaidAmount).ToString();
                pay.Amount = (Amount - PaidAmount).ToString();
                pay.Creator = UserName;
                pay.CreateDate = DateTime.Now;
                pay.Confirmer = UserName;
                pay.ConfirmDate = DateTime.Now;
                var log = new Log();
                log.OID = pay.OID;
                log.SoID = pay.SoID;
                log.Type = 0;
                log.LogDate = DateTime.Now;
                log.UserName = UserName;
                log.Title = "添加支付";
                log.Remark = "快速支付" + pay.PayAmount;
                log.CoID = CoID;
                logs.Add(log);
                if(ord.StatusDec == "部分付款" && ord.Status == 7)
                {
                    log = new Log();
                    log.OID = pay.OID;
                    log.SoID = pay.SoID;
                    log.Type = 0;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "取消异常标记";
                    log.Remark = ord.StatusDec;
                    log.CoID = CoID;
                    logs.Add(log);
                    ord.Status = 1;
                    ord.AbnormalStatus = 0;
                    ord.StatusDec = "";
                }
                log = new Log();
                log.OID = pay.OID;
                log.SoID = pay.SoID;
                log.Type = 0;
                log.LogDate = DateTime.Now;
                log.UserName = UserName;
                log.Title = "支付单确认";
                log.CoID = CoID;
                logs.Add(log);
                //新增支付单资料
                string sqlCommandText = @"INSERT INTO payinfo(PayNbr,RecID,RecName,OID,SOID,Payment,PayAccount,PayDate,Title,Amount,PayAmount,DataSource,Status,CoID,Creator,CreateDate,Confirmer,ConfirmDate) 
                                    VALUES(@PayNbr,@RecID,@RecName,@OID,@SOID,@Payment,@PayAccount,@PayDate,@Title,@Amount,@PayAmount,@DataSource,@Status,@CoID,@Creator,@CreateDate,@Confirmer,@ConfirmDate)";
                int count = CoreDBconn.Execute(sqlCommandText,pay,TransCore);
                if(count < 0)
                {
                    result.s = -3002;
                    return result;
                }
                //更新订单
                ord.PaidAmount = Amount.ToString();
                ord.PayAmount = (Amount - PaidAmount + decimal.Parse(ord.PayAmount)).ToString();
                if(ord.PayDate == null || ord.PayDate <= DateTime.Parse("1900-01-01"))
                {
                    ord.PayDate = pay.PayDate;
                }
                if(string.IsNullOrEmpty(ord.PayNbr))
                {
                    ord.PayNbr = pay.PayNbr;
                }
                ord.IsPaid = true;
                ord.Modifier = UserName;
                ord.ModifyDate = DateTime.Now;
                sqlCommandText = @"update `order` set PaidAmount = @PaidAmount,PayAmount = @PayAmount,PayDate =@PayDate,PayNbr = @PayNbr,IsPaid=@IsPaid,Status=@Status,AbnormalStatus=@AbnormalStatus,
                                    StatusDec=@StatusDec,Modifier=@Modifier,ModifyDate=@ModifyDate where ID = @ID and CoID = @CoID";
                count = CoreDBconn.Execute(sqlCommandText,ord, TransCore);
                if (count < 0)
                {
                    result.s = -3003;
                    return result;
                }
                string loginsert = @"INSERT INTO orderlog(OID,SoID,Type,LogDate,UserName,Title,Remark,CoID) 
                                        VALUES(@OID,@SoID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                 count = CoreDBconn.Execute(loginsert,logs, TransCore);
                if (count < 0)
                {
                    result.s = -3002;
                    return result;
                }
                TransCore.Commit();
            }
            catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }
            var res = new UpdatePay();
            var aa = GetOrderEdit(pay.OID,CoID);
            if(aa.s == -1)
            {
                result.s = -1;
                result.d = aa.d;
                return result;
            }
            res.Order = aa.d as OrderEdit;

            aa = GetOrderPay(pay.OID,CoID);
            if(aa.s == -1)
            {
                result.s = -1;
                result.d = aa.d;
                return result;
            }
            res.Pay = aa.d as List<OrderPay>;

            aa = GetOrderLog(pay.OID,CoID);
            if(aa.s == -1)
            {
                result.s = -1;
                result.d = aa.d;
                return result;
            }
            res.Log = aa.d as List<OrderLog>;
            result.d = res;
            return result;
        }
        ///<summary>
        ///抓取异常List
        ///</summary>
        public static DataResult GetAbnormalList(int CoID)
        {
            var result = new DataResult(1,null);
            string sqlcommand = "select ID,Name from orderabnormal where coid =" + CoID + " order by IsCustom asc,ID asc"; 
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    var u = conn.Query<AbnormalReason>(sqlcommand).AsList();
                    result.d = u;             
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }    
            return result;
        }
        ///<summary>
        ///异常单转正常
        ///</summary>
        public static DataResult TransferNormal(List<int> oid,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            var logs = new List<Log>();
            string sqlCommandText = string.Empty;
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string wheresql = "select count(id) from `order` where id in @ID and coid = @Coid and status <> 7";
                int count = CoreDBconn.QueryFirst<int>(wheresql,new {ID = oid,Coid = CoID});
                if (count > 0)
                {
                    result.s = -1;
                    result.d = "只有异常状态的订单才可转正常!";
                    return result;
                }
                wheresql = "select id,soid,StatusDec,IsPaid,coid from `order` where id in @ID and coid = @Coid";
                var u = CoreDBconn.Query<Order>(wheresql,new {ID = oid,Coid = CoID}).AsList();
                foreach(var a in u)
                {
                    var log = new Log();
                    log.OID = a.ID;
                    log.SoID = a.SoID;
                    log.Type = 0;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "取消异常标记";
                    log.Remark = a.StatusDec;
                    log.CoID = CoID;
                    logs.Add(log);
                    if(a.IsPaid == true)
                    {
                        a.Status = 1;
                    }
                    else
                    {
                        a.Status = 0;
                    }
                    a.AbnormalStatus = 0;
                    a.StatusDec = "";
                    a.Modifier = UserName;
                    a.ModifyDate = DateTime.Now;
                    sqlCommandText = @"update `order` set Status=@Status,AbnormalStatus=@AbnormalStatus,StatusDec=@StatusDec,Modifier=@Modifier,ModifyDate=@ModifyDate where ID = @ID and CoID = @CoID";
                    count = CoreDBconn.Execute(sqlCommandText,a,TransCore);
                    if (count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                }                
                string loginsert = @"INSERT INTO orderlog(OID,SoID,Type,LogDate,UserName,Title,Remark,CoID) 
                                        VALUES(@OID,@SoID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                count = CoreDBconn.Execute(loginsert,logs, TransCore);
                if (count < 0)
                {
                    result.s = -3002;
                    return result;
                }
                TransCore.Commit();
            }
            catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }
            return result;
        }
        ///<summary>
        ///订单合并资料筛选
        ///</summary>
        public static DataResult GetMergeOrd(int oid,int CoID)
        {
            var result = new DataResult(1,null);
            var res = new List<MergerOrd>();
            string sqlcommand = "select * from `order` where id = " + oid + " and coid = " + CoID; 
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    var u = conn.Query<Order>(sqlcommand).AsList();
                    if(u.Count == 0)
                    {
                        result.s = -1;
                        result.d = "内部订单号无效!";
                        return result;
                    }
                    else
                    {
                        if(u[0].Status != 1 &&　u[0].Status != 7)
                        {
                            result.s = -1;
                            result.d = "此订单状态不符合合并的条件!";
                            return result;
                        }
                        if(u[0].DealerType == 2)
                        {
                            result.s = -1;
                            result.d = "供销订单不允许合并,请联系分销商合并订单!";
                            return result;
                        }
                    }
                    var rr = new MergerOrd();
                    rr.type = "A";
                    rr.MOrd = u;
                    res.Add(rr);
                    //抓取推荐合并项
                    sqlcommand = "select * from `order` where coid = " + CoID + " and buyershopid = '" + u[0].BuyerShopID + "' and recname = '" + u[0].RecName + 
                                "' and reclogistics = '" + u[0].RecLogistics + "' and reccity = '" + u[0].RecCity + "' and recdistrict = '" + u[0].RecDistrict + 
                                "' and recaddress = '" + u[0].RecAddress + "' and status in (1,7) and IsPaid = true and id != " + oid;
                    var tt = conn.Query<Order>(sqlcommand).AsList();
                    rr = new MergerOrd();
                    rr.type = "L";
                    rr.MOrd = tt;
                    res.Add(rr);
                    //抓取中风险合并项
                    sqlcommand = "select * from `order` where coid = " + CoID + " and status in (1,7) and IsPaid = true" + " and ((" + 
                                "buyershopid = '" + u[0].BuyerShopID + "' and ( recname != '" + u[0].RecName + 
                                "' or reclogistics != '" + u[0].RecLogistics + "' or reccity != '" + u[0].RecCity + "' or recdistrict != '" + u[0].RecDistrict + 
                                "' or recaddress != '" + u[0].RecAddress + "')) or (" + 
                                "buyershopid != '" + u[0].BuyerShopID + "' and recname = '" + u[0].RecName + 
                                "' and reclogistics = '" + u[0].RecLogistics + "' and reccity = '" + u[0].RecCity + "' and recdistrict = '" + u[0].RecDistrict + 
                                "' and recaddress = '" + u[0].RecAddress + "'))" ;
                    tt = conn.Query<Order>(sqlcommand).AsList();
                    rr = new MergerOrd();
                    rr.type = "M";
                    rr.MOrd = tt;
                    res.Add(rr);
                    //抓取高风险合并项
                    sqlcommand = "select * from `order` where coid = " + CoID + " and buyershopid = '" + u[0].BuyerShopID + "' and recname = '" + u[0].RecName + 
                                "' and reclogistics = '" + u[0].RecLogistics + "' and reccity = '" + u[0].RecCity + "' and recdistrict = '" + u[0].RecDistrict + 
                                "' and recaddress = '" + u[0].RecAddress + "' and status in (0,7) and IsPaid = false";
                    tt = conn.Query<Order>(sqlcommand).AsList();
                    rr = new MergerOrd();
                    rr.type = "H";
                    rr.MOrd = tt;
                    res.Add(rr);
                    
                    result.d = res;             
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }    
            return result;
        }
        ///<summary>
        ///订单合并
        ///</summary>
        public static DataResult OrdMerger(int oid,List<int> MerID,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            var logs = new List<Log>();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string sqlcommand = "select * from `order` where id = " + oid + " and coid = " + CoID;
                var u = CoreDBconn.Query<Order>(sqlcommand).AsList();
                if(u.Count == 0)
                {
                    result.s = -1;
                    result.d = "主订单单号无效";
                    return result;
                }
                var MainOrd = u[0] as Order;
                sqlcommand = "select * from `order` where id  in @ID and coid = @Coid";
                u = CoreDBconn.Query<Order>(sqlcommand,new{ID = MerID,Coid = CoID}).AsList();
                if(u.Count == 0)
                {
                    result.s = -1;
                    result.d = "无需要合并的订单!";
                    return result;
                }
                var OrdList = u as List<Order>;
                string remark="";
                foreach(var o in OrdList)
                {
                    o.MergeOID = oid;
                    o.MergeSoID = MainOrd.SoID;
                    o.Status = 5;
                    o.AbnormalStatus = 0;
                    o.StatusDec = "";
                    o.Modifier = UserName;
                    o.ModifyDate = DateTime.Now;
                    MainOrd.OrdQty = MainOrd.OrdQty + o.OrdQty;
                    MainOrd.SkuAmount = (decimal.Parse(MainOrd.SkuAmount) + decimal.Parse(o.SkuAmount)).ToString();
                    MainOrd.PaidAmount = (decimal.Parse(MainOrd.PaidAmount) + decimal.Parse(o.PaidAmount)).ToString();
                    MainOrd.PayAmount = (decimal.Parse(MainOrd.PayAmount) + decimal.Parse(o.PayAmount)).ToString();
                    MainOrd.ExAmount = (decimal.Parse(MainOrd.ExAmount) + decimal.Parse(o.ExAmount)).ToString();
                    MainOrd.RecMessage = MainOrd.RecMessage + o.RecMessage;
                    MainOrd.SendMessage = MainOrd.SendMessage + o.SendMessage;
                    MainOrd.ExWeight = (decimal.Parse(MainOrd.ExWeight) + decimal.Parse(o.ExWeight)).ToString();
                    
                    var log = new Log();
                    log.OID = o.ID;
                    log.SoID = o.SoID;
                    log.Type = 0;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "被合并";
                    log.Remark = oid.ToString();
                    log.CoID = CoID;
                    logs.Add(log);
                    remark = remark + o.ID.ToString() + ",";

                    sqlcommand = @"update `order` set MergeOID = @MergeOID,MergeSoID = @MergeSoID,Status=@Status,AbnormalStatus=@AbnormalStatus,StatusDec=@StatusDec,
                                    Modifier=@Modifier,ModifyDate=@ModifyDate where ID = @ID and CoID = @CoID";
                    int r = CoreDBconn.Execute(sqlcommand,o,TransCore);
                    if (r < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                }
                MainOrd.Amount = (decimal.Parse(MainOrd.ExAmount) + decimal.Parse(MainOrd.SkuAmount)).ToString();
                MainOrd.IsMerge = true;
                if(decimal.Parse(MainOrd.Amount) == decimal.Parse(MainOrd.PaidAmount))
                {
                    MainOrd.IsPaid = true;
                    MainOrd.Status = 1;
                }
                else
                {
                    MainOrd.IsPaid = false;
                    MainOrd.Status = 0;
                }
                MainOrd.AbnormalStatus = 0;
                MainOrd.StatusDec = "";
                MainOrd.Modifier = UserName;
                MainOrd.ModifyDate = DateTime.Now;

                var logn = new Log();
                logn.OID = MainOrd.ID;
                logn.SoID = MainOrd.SoID;
                logn.Type = 0;
                logn.LogDate = DateTime.Now;
                logn.UserName = UserName;
                logn.Title = "合并";
                logn.Remark = remark.Substring(0,remark.Length - 1);
                logn.CoID = CoID;
                logs.Add(logn);

                sqlcommand = @"update `order` set OrdQty = @OrdQty,SkuAmount = @SkuAmount,PaidAmount=@PaidAmount,PayAmount=@PayAmount,ExAmount=@ExAmount,RecMessage=@RecMessage,
                                SendMessage=@SendMessage,ExWeight=@ExWeight,Amount=@Amount,IsMerge=@IsMerge,IsPaid=@IsPaid,Status=@Status,AbnormalStatus=@AbnormalStatus,StatusDec=@StatusDec,
                                Modifier=@Modifier,ModifyDate=@ModifyDate where ID = @ID and CoID = @CoID";
                int count = CoreDBconn.Execute(sqlcommand,MainOrd,TransCore);
                if (count < 0)
                {
                    result.s = -3003;
                    return result;
                }
                string loginsert = @"INSERT INTO orderlog(OID,SoID,Type,LogDate,UserName,Title,Remark,CoID) 
                                        VALUES(@OID,@SoID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                count = CoreDBconn.Execute(loginsert,logs, TransCore);
                if (count < 0)
                {
                    result.s = -3002;
                    return result;
                }
                //订单明细
                sqlcommand = "select * from orderitem where oid  in @ID and coid = @Coid";
                var item = CoreDBconn.Query<OrderItem>(sqlcommand,new{ID = MerID,Coid = CoID}).AsList();
                if(item.Count == 0)
                {
                    result.s = -1;
                    result.d = "订单明细异常!";
                    return result;
                }
                foreach(var i in item)
                {
                    sqlcommand = "select * from orderitem where oid  = @ID and coid = @Coid and skuautoid = @Sku";
                    var x = CoreDBconn.Query<OrderItem>(sqlcommand,new{ID = oid,Coid = CoID,Sku = i.SkuAutoID}).AsList();
                    if(x.Count == 0)
                    {
                        sqlcommand = @"INSERT INTO orderitem(oid,soid,coid,skuautoid,skuid,skuname,norm,qty,saleprice,realprice,amount,discountrate,img,weight,totalweight,isgift,remark,creator,modifier) 
                                    VALUES(@OID,@Soid,@Coid,@Skuautoid,@Skuid,@Skuname,@Norm,@Qty,@Saleprice,@Realprice,@Amount,@DiscountRate,@Img,@Weight,@Totalweight,@IsGift,@Remark,@Creator,@Modifier)";
                        i.OID = oid;
                        i.SoID = MainOrd.SoID;
                        i.Modifier = UserName;
                        count = CoreDBconn.Execute(sqlcommand,i, TransCore);
                        if (count < 0)
                        {
                            result.s = -3002;
                            return result;
                        }
                    }
                    else
                    {
                        sqlcommand = @"update orderitem set qty = qty + @Qty,amount = amount + @Amount,realprice = amount/qty,totalweight = weight * qty,
                                        modifier = @Modifier,modifydate = @ModifyDate where id = @ID and coid = @Coid";
                        count = CoreDBconn.Execute(sqlcommand,new{Qty = i.Qty,Amount = i.Amount,Modifier=UserName,ModifyDate=DateTime.Now,ID=x[0].ID,Coid = CoID}, TransCore);
                        if (count < 0)
                        {
                            result.s = -3003;
                            return result;
                        }
                    }
                }
                //付款明细
                sqlcommand = "select * from payinfo where oid in @ID and coid = @Coid";
                var pay = CoreDBconn.Query<PayInfo>(sqlcommand,new{ID = MerID,Coid = CoID}).AsList();
                foreach(var p in pay)
                {
                    sqlcommand = @"update payinfo set oid = @Oid,soid = @Soid where oid in @ID and coid = @Coid";
                    count = CoreDBconn.Execute(sqlcommand,new{Oid = MainOrd.ID,Soid = MainOrd.SoID,ID=MerID,Coid = CoID}, TransCore);
                    if (count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                }
                TransCore.Commit();
            }
            catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }

            return result;
        }
        ///<summary>
        ///订单合并还原
        ///</summary>
        public static DataResult CancleOrdMerge(List<int> oid,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            var logs = new List<Log>();
            var MainOrd = new List<Order>();
            if (oid.Count  == 0)
            {
                string sqlcommand = "select * from `order` where IsMerge = true and status in (0,1,7) and coid = " + CoID; 
                using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                    try{    
                        var u = conn.Query<Order>(sqlcommand).AsList();
                        if(u.Count == 0)
                        {
                            result.s = -1;
                            result.d = "无符合条件的资料!";
                            return result;
                        }
                        else
                        {
                            MainOrd = u;
                        }         
                    }catch(Exception ex){
                        result.s = -1;
                        result.d = ex.Message;
                        conn.Dispose();
                    }
                }    
            }
            else
            {
                string sqlcommand = "select * from `order` where id in @ID and coid = @Coid"; 
                using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                    try{    
                        var u = conn.Query<Order>(sqlcommand,new{id = oid,coid = CoID}).AsList();
                        if(u.Count == 0)
                        {
                            result.s = -1;
                            result.d = "参数异常!";
                            return result;
                        }
                        else
                        {
                            foreach(var a in u)
                            {
                                if(a.Status == 3 || a.Status == 4 ||a.Status == 5 ||a.Status == 6)
                                {
                                    result.s = -1;
                                    result.d = "只能还原未发货的订单!";
                                    return result;
                                }
                            }
                            MainOrd = u;
                        }         
                    }catch(Exception ex){
                        result.s = -1;
                        result.d = ex.Message;
                        conn.Dispose();
                    }
                }    
            }
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                foreach(var a in MainOrd)
                {
                    List<int> merid = new List<int>();
                    string sqlcommand = "select * from `order` where MergeOID = " + a.ID + " and coid = " + CoID; 
                    var u = CoreDBconn.Query<Order>(sqlcommand).AsList();
                    foreach(var b in u)
                    {
                        merid.Add(b.ID);
                        b.MergeOID = 0;
                        b.MergeSoID = 0;
                        if(b.IsPaid == true)
                        {
                            b.Status = 1;
                        }
                        else
                        {
                            b.Status = 0;
                        }
                        b.Modifier = UserName;
                        b.ModifyDate = DateTime.Now;
                        a.OrdQty = a.OrdQty - b.OrdQty;
                        a.SkuAmount = (decimal.Parse(a.SkuAmount) - decimal.Parse(b.SkuAmount)).ToString();
                        a.PaidAmount = (decimal.Parse(a.PaidAmount) - decimal.Parse(b.PaidAmount)).ToString();
                        a.PayAmount = (decimal.Parse(a.PayAmount) - decimal.Parse(b.PayAmount)).ToString();
                        a.ExAmount = (decimal.Parse(a.ExAmount) - decimal.Parse(b.ExAmount)).ToString();
                        a.ExWeight = (decimal.Parse(a.ExWeight) - decimal.Parse(b.ExWeight)).ToString();
                        
                        var log = new Log();
                        log.OID = b.ID;
                        log.SoID = b.SoID;
                        log.Type = 0;
                        log.LogDate = DateTime.Now;
                        log.UserName = UserName;
                        log.Title = "合并还原";
                        log.CoID = CoID;
                        logs.Add(log);

                        sqlcommand = @"update `order` set MergeOID = @MergeOID,MergeSoID = @MergeSoID,Status=@Status,
                                        Modifier=@Modifier,ModifyDate=@ModifyDate where ID = @ID and CoID = @CoID";
                        int r = CoreDBconn.Execute(sqlcommand,b,TransCore);
                        if (r < 0)
                        {
                            result.s = -3003;
                            return result;
                        }
                        if(decimal.Parse(b.PaidAmount) >0)
                        {
                            //付款明细
                            sqlcommand = @"update payinfo set oid = @Oid,soid = @Soid where oid = @ID and coid = @Coid and PayNbr =@PayNbr";
                            r = CoreDBconn.Execute(sqlcommand,new{Oid = b.ID,Soid = b.SoID,ID=a.ID,Coid = CoID,PayNbr = b.PayNbr}, TransCore);
                            if (r < 0)
                            {
                                result.s = -3003;
                                return result;
                            }
                        }
                    }
                    a.Amount = (decimal.Parse(a.ExAmount) + decimal.Parse(a.SkuAmount)).ToString();
                    a.IsMerge = false;
                    if(decimal.Parse(a.Amount) == decimal.Parse(a.PaidAmount))
                    {
                        a.IsPaid = true;
                        a.Status = 1;
                    }
                    else
                    {
                        a.IsPaid = false;
                        a.Status = 0;
                    }
                    a.Modifier = UserName;
                    a.ModifyDate = DateTime.Now;

                    var logn = new Log();
                    logn.OID = a.ID;
                    logn.SoID = a.SoID;
                    logn.Type = 0;
                    logn.LogDate = DateTime.Now;
                    logn.UserName = UserName;
                    logn.Title = "合并还原";
                    logn.CoID = CoID;
                    logs.Add(logn);

                    sqlcommand = @"update `order` set OrdQty = @OrdQty,SkuAmount = @SkuAmount,PaidAmount=@PaidAmount,PayAmount=@PayAmount,ExAmount=@ExAmount,
                                    ExWeight=@ExWeight,Amount=@Amount,IsMerge=@IsMerge,IsPaid=@IsPaid,Status=@Status,
                                    Modifier=@Modifier,ModifyDate=@ModifyDate where ID = @ID and CoID = @CoID";
                    int count = CoreDBconn.Execute(sqlcommand,a,TransCore);
                    if (count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                    //订单明细
                    sqlcommand = "select * from orderitem where oid  in @ID and coid = @Coid";
                    var item = CoreDBconn.Query<OrderItem>(sqlcommand,new{ID = merid,Coid = CoID}).AsList();
                    if(item.Count == 0)
                    {
                        result.s = -1;
                        result.d = "订单明细异常!";
                        return result;
                    }
                    foreach(var i in item)
                    {
                        sqlcommand = "select * from orderitem where oid  = @ID and coid = @Coid and skuautoid = @Sku";
                        var x = CoreDBconn.Query<OrderItem>(sqlcommand,new{ID = a.ID,Coid = CoID,Sku = i.SkuAutoID}).AsList();
                        if(x[0].Qty == i.Qty)
                        {
                            sqlcommand = @"delete from orderitem where oid  = @ID and coid = @Coid and skuautoid = @Sku";
                            count = CoreDBconn.Execute(sqlcommand,new{ID = a.ID,Coid = CoID,Sku = i.SkuAutoID}, TransCore);
                            if (count < 0)
                            {
                                result.s = -3004;
                                return result;
                            }
                        }
                        else
                        {
                            sqlcommand = @"update orderitem set qty = qty - @Qty,amount = amount - @Amount,realprice = amount/qty,totalweight = weight * qty,
                                            modifier = @Modifier,modifydate = @ModifyDate where id = @ID and coid = @Coid";
                            count = CoreDBconn.Execute(sqlcommand,new{Qty = i.Qty,Amount = i.Amount,Modifier=UserName,ModifyDate=DateTime.Now,ID=x[0].ID,Coid = CoID}, TransCore);
                            if (count < 0)
                            {
                                result.s = -3003;
                                return result;
                            }
                        }
                    }
                }
                string loginsert = @"INSERT INTO orderlog(OID,SoID,Type,LogDate,UserName,Title,Remark,CoID) 
                                            VALUES(@OID,@SoID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                int ii = CoreDBconn.Execute(loginsert,logs, TransCore);
                if (ii < 0)
                {
                    result.s = -3002;
                    return result;
                }
                TransCore.Commit();
            }
            catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }
            return result;
        }
        ///<summary>
        ///订单拆分
        ///</summary>
        public static DataResult OrdSplit(int oid,List<SplitOrd> SplitArray,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            var logs = new List<Log>();
            int i = 0,j = 0,qtyNew = 0;
            decimal amt = 0,amtNew = 0,weight = 0,weightNew = 0;
            foreach(var a in SplitArray)
            {
                if(a.Qty == a.QtyNew)
                {
                    i ++;
                }
                if(a.QtyNew == 0)
                {
                    j ++;
                }
                if(a.QtyNew > a.Qty)
                {
                    result.s = -1;
                    result.d = "拆分数量不能大于原订单数量!";
                    return result;
                }
                qtyNew = qtyNew + a.QtyNew;
                amt = amt + (a.Qty - a.QtyNew) * a.Price;
                amtNew = amtNew + a.QtyNew * a.Price;
                weight = weight + (a.Qty - a.QtyNew) * a.Weight;
                weightNew = weightNew + a.QtyNew * a.Weight;
            }
            if(i == SplitArray.Count)
            {
                result.s = -1;
                result.d = "订单数量不能全部拆分!";
                return result;
            }
            if(j == SplitArray.Count)
            {
                result.s = -1;
                result.d = "订单拆分数量不能为零!";
                return result;
            }
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string sqlcommand = "select * from `order` where id = " + oid + " and coid = " + CoID;
                var u = CoreDBconn.Query<Order>(sqlcommand).AsList();
                var ord = u[0] as Order;
                var ordNew = u[0] as Order;
                long soid = u[0].SoID;
                if(u.Count == 0)
                {
                    result.s = -1;
                    result.d = "订单单号参数无效!";
                    return result;
                }
                else
                {
                    if(u[0].Status != 1)
                    {
                        result.s = -1;
                        result.d = "只有已付款待审核的订单才可以拆分!";
                        return result;
                    }
                    if(u[0].DealerType == 2)
                    {
                        result.s = -1;
                        result.d = "供销订单不允许拆分,请联系分销商拆分订单!";
                        return result;
                    }
                }
                decimal PaidAmount = 0,PayAmount = 0,ExAmount = 0;
                PaidAmount = decimal.Parse(ord.PaidAmount);
                PayAmount = decimal.Parse(ord.PayAmount);
                ExAmount = decimal.Parse(ord.ExAmount);
                //更新原订单
                ord.IsSplit = true;
                ord.OrdQty = ord.OrdQty - qtyNew;
                ord.SkuAmount = amt.ToString();
                ord.PaidAmount =  Math.Round(amt/(amt + amtNew) * PaidAmount,2).ToString();
                ord.PayAmount =  Math.Round(amt/(amt + amtNew) * PayAmount,2).ToString();
                ord.ExAmount =  Math.Round(amt/(amt + amtNew) * ExAmount,2).ToString();
                ord.Amount = (amt + decimal.Parse(ord.ExAmount)).ToString();
                ord.ExWeight = weight.ToString();
                ord.Modifier = UserName;
                ord.ModifyDate = DateTime.Now;
                sqlcommand = @"update `order` set IsSplit = @IsSplit,OrdQty=@OrdQty,SkuAmount=@SkuAmount,PaidAmount=@PaidAmount,PayAmount=@PayAmount,ExAmount=@ExAmount,
                                Amount = @Amount,ExWeight = @ExWeight,Modifier=@Modifier,ModifyDate=@ModifyDate where ID = @ID and CoID = @CoID";
                int count = CoreDBconn.Execute(sqlcommand,ord,TransCore);
                if (count < 0)
                {
                    result.s = -3003;
                    return result;
                }
                //新增订单
                ordNew.MergeOID = ord.ID;
                ordNew.SoID = long.Parse(DateTime.Now.Ticks.ToString().Substring(0, 11));
                ordNew.MergeSoID = soid;
                ordNew.OrdQty = qtyNew;
                ordNew.SkuAmount = amtNew.ToString();
                ordNew.PaidAmount =  Math.Round(amtNew/(amt + amtNew) * PaidAmount,2).ToString();
                ordNew.PayAmount =  Math.Round(amtNew/(amt + amtNew) * PayAmount,2).ToString();
                ordNew.ExAmount =  Math.Round(amtNew/(amt + amtNew) * ExAmount,2).ToString();
                ordNew.Amount = (amtNew + decimal.Parse(ordNew.ExAmount)).ToString();
                ordNew.ExWeight = weightNew.ToString();
                ordNew.Creator = UserName;
                ordNew.Modifier = UserName;
                sqlcommand = @"INSERT INTO `order`(MergeOID,Type,DealerType,IsMerge,IsSplit,OSource,ODate,CoID,BuyerID,BuyerShopID,ShopID,ShopName,ShopSit,SoID,MergeSoID,
                                                    OrdQty,Amount,SkuAmount,PaidAmount,PayAmount,ExAmount,IsInvoice,InvoiceType,InvoiceTitle,InvoiceDate,IsPaid,PayDate,
                                                    PayNbr,IsCOD,Status,AbnormalStatus,StatusDec,ShopStatus,RecName,RecLogistics,RecCity,RecDistrict,RecAddress,RecZip,
                                                    RecTel,RecPhone,RecMessage,SendMessage,Express,ExID,ExWeight,Creator,Modifier) 
                                VALUES(@MergeOID,@Type,@DealerType,@IsMerge,@IsSplit,@OSource,@ODate,@CoID,@BuyerID,@BuyerShopID,@ShopID,@ShopName,@ShopSit,@SoID,@MergeSoID,
                                       @OrdQty,@Amount,@SkuAmount,@PaidAmount,@PayAmount,@ExAmount,@IsInvoice,@InvoiceType,@InvoiceTitle,@InvoiceDate,@IsPaid,@PayDate,
                                       @PayNbr,@IsCOD,@Status,@AbnormalStatus,@StatusDec,@ShopStatus,@RecName,@RecLogistics,@RecCity,@RecDistrict,@RecAddress,@RecZip,
                                       @RecTel,@RecPhone,@RecMessage,@SendMessage,@Express,@ExID,@ExWeight,@Creator,@Modifier)";
                count = CoreDBconn.Execute(sqlcommand,ordNew,TransCore);
                if (count < 0)
                {
                    result.s = -3002;
                    return result;
                }
                int rtn = CoreDBconn.QueryFirst<int>("select LAST_INSERT_ID()");
                //订单明细处理
                foreach(var a in SplitArray)
                {
                    if(a.QtyNew == 0)//未拆分明细，不需处理
                    {
                        continue;
                    }
                    if(a.QtyNew == a.Qty)//数量全部拆分到新订单
                    {
                        sqlcommand = @"update orderitem set OID = @OID,SoID=@SoID,Modifier=@Modifier,ModifyDate=@ModifyDate where oid = @ID and CoID = @CoID and skuautoid = @Sku";
                        count = CoreDBconn.Execute(sqlcommand,new{OID = rtn,SoID = ordNew.SoID,Modifier=UserName,ModifyDate = DateTime.Now,ID = oid,CoID = CoID,Sku = a.Skuid},TransCore);
                        if (count < 0)
                        {
                            result.s = -3003;
                            return result;
                        }
                    }
                    if(a.QtyNew > 0 && a.QtyNew != a.Qty)//拆分订单
                    {
                        //更新原订单数量
                        sqlcommand = @"update orderitem set Qty = @Qty,Amount=RealPrice*Qty,TotalWeight = Weight*Qty,Modifier=@Modifier,ModifyDate=@ModifyDate where oid = @ID and CoID = @CoID and skuautoid = @Sku";
                        count = CoreDBconn.Execute(sqlcommand,new{Qty = a.Qty - a.QtyNew,Modifier=UserName,ModifyDate = DateTime.Now,ID = oid,CoID = CoID,Sku = a.Skuid},TransCore);
                        if (count < 0)
                        {
                            result.s = -3003;
                            return result;
                        }
                        //新增订单明细
                        sqlcommand = "select * from  orderitem  where oid = " + oid + " and CoID =" + CoID + " and skuautoid =" + a.Skuid;
                        var item = CoreDBconn.Query<OrderItem>(sqlcommand).AsList();
                        item[0].OID = rtn;
                        item[0].SoID = ordNew.SoID;
                        item[0].Qty = a.QtyNew;
                        item[0].Amount = (a.QtyNew * decimal.Parse(item[0].RealPrice)).ToString();
                        item[0].TotalWeight = (a.QtyNew * decimal.Parse(item[0].Weight)).ToString();
                        item[0].Creator = UserName;
                        item[0].Modifier = UserName;
                        sqlcommand = @"INSERT INTO orderitem (OID,SoID,CoID,SkuAutoID,SkuID,SkuName,Norm,Qty,SalePrice,RealPrice,Amount,DiscountRate,img,Weight,TotalWeight,
                                                            IsGift,Remark,Creator,Modifier) 
                                        VALUES(@OID,@SoID,@CoID,@SkuAutoID,@SkuID,@SkuName,@Norm,@Qty,@SalePrice,@RealPrice,@Amount,@DiscountRate,@img,@Weight,@TotalWeight,
                                            @IsGift,@Remark,@Creator,@Modifier)";
                        count = CoreDBconn.Execute(sqlcommand,item[0],TransCore);
                        if (count < 0)
                        {
                            result.s = -3002;
                            return result;
                        }
                    }
                }
                //log写入
                var log = new Log();
                log.OID = oid;
                log.SoID = soid;
                log.Type = 0;
                log.LogDate = DateTime.Now;
                log.UserName = UserName;
                log.Title = "被拆分";
                log.Remark = rtn.ToString();
                log.CoID = CoID;
                logs.Add(log);
                log = new Log();
                log.OID = rtn;
                log.SoID = ordNew.SoID;
                log.Type = 0;
                log.LogDate = DateTime.Now;
                log.UserName = UserName;
                log.Title = "拆分";
                log.Remark = oid.ToString();
                log.CoID = CoID;
                logs.Add(log);
                string loginsert = @"INSERT INTO orderlog(OID,SoID,Type,LogDate,UserName,Title,Remark,CoID) 
                                            VALUES(@OID,@SoID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                count = CoreDBconn.Execute(loginsert,logs, TransCore);
                if (count < 0)
                {
                    result.s = -3002;
                    return result;
                }
                //payinfo写入
                sqlcommand = "select * from payinfo where oid = " + oid + " and coid = " + CoID;
                var pay = CoreDBconn.Query<PayInfo>(sqlcommand).AsList();
                foreach(var p in pay)
                {
                    decimal Amount = decimal.Parse(p.Amount);
                    PayAmount = decimal.Parse(p.PayAmount);
                    //原明细
                    p.Amount = Math.Round(amt/(amt + amtNew) * Amount,2).ToString();
                    p.PayAmount = Math.Round(amt/(amt + amtNew) * PayAmount,2).ToString();
                    sqlcommand = @"update payinfo set Amount = @Amount,PayAmount=@PayAmount  where ID = @ID and CoID = @CoID";
                    count = CoreDBconn.Execute(sqlcommand,p,TransCore);
                    if (count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                    //新明细
                    p.CoID = CoID;
                    p.OID = rtn;
                    p.SoID = ordNew.SoID;
                    p.Amount = Math.Round(amtNew/(amt + amtNew) * Amount,2).ToString();
                    p.PayAmount = Math.Round(amtNew/(amt + amtNew) * PayAmount,2).ToString();
                    sqlcommand = @"INSERT INTO payinfo(PayNbr,RecID,RecName,OID,SOID,Payment,PayAccount,SellerAccount,Platform,PayDate,Bank,BankName,Title,Name,Amount,
                                                    PayAmount,DiscountFree,DataSource,Status,Creator,Confirmer,ConfirmDate) 
                                VALUES(@PayNbr,@RecID,@RecName,@OID,@SOID,@Payment,@PayAccount,@SellerAccount,@Platform,@PayDate,@Bank,@BankName,@Title,@Name,@Amount,
                                       @PayAmount,@DiscountFree,@DataSource,@Status,@Creator,@Confirmer,@ConfirmDate)";
                    count = CoreDBconn.Execute(sqlcommand,p,TransCore);
                    if (count < 0)
                    {
                        result.s = -3002;
                        return result;
                    }
                }
                TransCore.Commit();
            }
            catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }

            return result;
        }
        ///<summary>
        ///修改运费
        ///</summary>
        public static DataResult ModifyFreight(List<int> oid,decimal freight,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            var logs = new List<Log>();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string sqlcommand = "select count(id) from `order` where id in @ID and coid = @Coid and status in (2,3,4,5,6)";
                int rtn = CoreDBconn.QueryFirst<int>(sqlcommand,new{ID = oid,Coid = CoID});
                if(rtn > 0)
                {
                    result.s = -1;
                    result.d = "待付款;已付款待审核;异常的订单才可以修改运费";
                    return result;
                }
                sqlcommand = "select id,soid,ExAmount,SkuAmount,PaidAmount,status from `order` where id in @ID and coid = @Coid "; 
                var ord = CoreDBconn.Query<Order>(sqlcommand,new{ID = oid,Coid = CoID}).AsList();
                foreach(var a in ord)
                {
                    decimal skuamt = decimal.Parse(a.SkuAmount);
                    if(skuamt + freight == decimal.Parse(a.PaidAmount))
                    {
                        a.IsPaid = true;
                        if(a.Status == 0)
                        {
                            a.Status = 1;
                        }
                    }
                    else
                    {
                        a.IsPaid = false;
                        if(a.Status == 1)
                        {
                            a.Status = 0;
                        }
                    }
                    var log = new Log();
                    log.OID = a.ID;
                    log.SoID = a.SoID;
                    log.Type = 0;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "手动修改运费";
                    log.Remark = "运费 " + a.ExAmount + "=>" + freight.ToString();                  
                    log.CoID = CoID;
                    logs.Add(log);
                    sqlcommand = @"update `order` set ExAmount = @ExAmount,Amount = SkuAmount + ExAmount,IsPaid = @IsPaid,Status=@Status,
                                  modifier = @Modifier,modifydate=@ModifyDate where id = @ID and coid = @Coid";  
                    int i = CoreDBconn.Execute(sqlcommand,new {ExAmount=freight,IsPaid=a.IsPaid,Status=a.Status,Modifier=UserName,ModifyDate=DateTime.Now,ID = a.ID,Coid=CoID}, TransCore);
                    if (i < 0)
                    {
                        result.s = -3003;
                        return result;
                    }            
                }                           
                string loginsert = @"INSERT INTO orderlog(OID,SoID,Type,LogDate,UserName,Title,Remark,CoID) 
                                            VALUES(@OID,@SoID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                int count = CoreDBconn.Execute(loginsert,logs, TransCore);
                if (count < 0)
                {
                    result.s = -3002;
                    return result;
                }                              
                TransCore.Commit();
            }
            catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }
            return result;
        }
        ///<summary>
        ///初始资料
        ///</summary>
        public static DataResult GetInitData(int CoID)                
        {
            var result = new DataResult(1,null);
            var res = new OrdInitData();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{  
                    //订单状态设定
                    var ss = new List<OStatus>();
                    foreach (int  myCode in Enum.GetValues(typeof(OrdStatus)))
                    {
                        var s = new OStatus();
                        s.value = myCode.ToString();
                        s.label = Enum.GetName(typeof(OrdStatus), myCode);//获取名称
                        if(myCode == 0 ||myCode == 1 ||myCode == 2 ||myCode == 7)
                        {
                            int i = conn.QueryFirst<int>("select count(id) from `order` where status = " + myCode + " and coid =" + CoID);
                            s.count = i;
                        }
                        ss.Add(s);
                    }
                    res.OrdStatus = ss;
                    //订单异常状态设定
                    var re = GetAbnormalList(CoID);
                    if(re.s == -1)
                    {
                        result.s = -1;
                        result.d = re.d;
                        return result;
                    }
                    var ab = re.d as List<AbnormalReason>;
                    ss = new List<OStatus>();
                    foreach(var a in ab)
                    {
                        var s = new OStatus();
                        s.value= a.ID.ToString();
                        s.label = a.Name;
                        int i = conn.QueryFirst<int>("select count(id) from `order` where status = 7 and coid =" + CoID + " and AbnormalStatus =" + a.ID);
                        s.count = i;
                        ss.Add(s);
                    }
                    res.OrdAbnormalStatus = ss;
                    //分销商
                    string sqlcommand = "select ID,DistributorName as Name from distributor where coid =" + CoID + " and enable = true";
                    var Distributor = conn.Query<AbnormalReason>(sqlcommand).AsList();
                    var aa = new List<Filter>();
                    foreach(var d in Distributor)
                    {
                        var a = new Filter();
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
            //仓库资料
            List<Filter> wh = new List<Filter>();
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{  
                    var h = new Filter();
                    string sqlcommand = "select WarehouseName from Warehouse where coid =" + CoID + " and enable = true and type = 0";
                    string name = conn.QueryFirst<string>(sqlcommand);
                    h.value = "0";
                    h.label = name;
                    wh.Add(h);
                    }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }  
            //分仓
            var w = CoreComm.WarehouseHaddle.getWarelist(CoID.ToString());
            foreach(var h in w)
            {
                var a = new Filter();
                a.value = h.id.ToString();
                a.label = h.warename;
                wh.Add(a);
            }
            res.Warehouse = wh ;
            //买家留言条件设定
            var ff = new List<Filter>();
            var f = new Filter();
            f.value = "A";
            f.label = "不过滤";
            ff.Add(f);
            f = new Filter();
            f.value = "N";
            f.label = "无留言";
            ff.Add(f);
            f = new Filter();
            f.value = "Y";
            f.label = "有留言";
            ff.Add(f);
            res.BuyerRemark = ff;
            //卖家备注
            ff = new List<Filter>();
            f = new Filter();
            f.value = "A";
            f.label = "不过滤";
            ff.Add(f);
            f = new Filter();
            f.value = "N";
            f.label = "无备注";
            ff.Add(f);
            f = new Filter();
            f.value = "Y";
            f.label = "有备注";
            ff.Add(f);
            res.SellerRemark = ff;
            //订单资料来源
            var oo = new List<Filter>();
            var o = new Filter();
            o.value = "-1";
            o.label = "--- 不限 ---";
            oo.Add(o);
            foreach (int  myCode in Enum.GetValues(typeof(OrdSource)))
            {
                o = new Filter();
                o.value = myCode.ToString();
                o.label = Enum.GetName(typeof(OrdSource), myCode);//获取名称
                oo.Add(o);
            }
            res.OSource = oo;
            //订单类型
            oo = new List<Filter>();
            o = new Filter();
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
            res.OType = oo;
            //贷款方式
            ff = new List<Filter>();
            f = new Filter();
            f.value = "A";
            f.label = "所有(不区分货款方式)";
            ff.Add(f);
            f = new Filter();
            f.value = "N";
            f.label = "在线支付(非货到付款)";
            ff.Add(f);
            f = new Filter();
            f.value = "Y";
            f.label = "货到付款";
            ff.Add(f);
            res.LoanType = ff;
            //是否付款
            ff = new List<Filter>();
            f = new Filter();
            f.value = "A";
            f.label = "所有(不区分是否付款)";
            ff.Add(f);
            f = new Filter();
            f.value = "N";
            f.label = "未付款";
            ff.Add(f);
            f = new Filter();
            f.value = "Y";
            f.label = "已付款";
            ff.Add(f);
            res.IsPaid = ff;
            //获取店铺List
            var shop = CoreComm.ShopHaddle.getShopEnum(CoID.ToString()) as List<shopEnum>;
            ff = new List<Filter>();
            foreach(var t in shop)
            {
                f = new Filter();
                f.value = t.value.ToString();
                f.label = t.label;
                ff.Add(f);
            }
            res.Shop = ff;  
            //快递Lsit
            var Express = CoreComm.ExpressHaddle.GetExpressSimple(CoID).d as List<ExpressSimple>;
            ff = new List<Filter>();
            foreach(var t in Express)
            {
                f = new Filter();
                f.value = t.ID;
                f.label = t.Name;
                ff.Add(f);
            }
            res.Express = ff;  
            //其他
            oo = new List<Filter>();
            o = new Filter();
            o.value = "0";
            o.label = "合并订单";
            oo.Add(o);
            o = new Filter();
            o.value = "1";
            o.label = "拆分订单";
            oo.Add(o);
            o = new Filter();
            o.value = "2";
            o.label = "非合并订单";
            oo.Add(o);
            o = new Filter();
            o.value = "3";
            o.label = "非拆分订单";
            oo.Add(o);
            o = new Filter();
            o.value = "4";
            o.label = "开发票";
            oo.Add(o);
            res.Others = oo;

            result.d = res;
            return result;
        }
        ///<summary>
        ///读取各状态订单笔数
        ///</summary>
        public static DataResult GetStatusCount(int CoID)
        {
            var result = new DataResult(1,null);
            var res = new StatusCount();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{  
                    //订单状态设定
                    var ss = new List<OStatusCnt>();
                    foreach (int  myCode in Enum.GetValues(typeof(OrdStatus)))
                    {
                        if(myCode == 0 ||myCode == 1 ||myCode == 2 ||myCode == 7)
                        {
                            var s = new OStatusCnt();
                            s.value = myCode;
                            int i = conn.QueryFirst<int>("select count(id) from `order` where status = " + myCode + " and coid =" + CoID);
                            s.count = i;
                            ss.Add(s);
                        }
                    }
                    res.OrdStatus = ss;
                    //订单异常状态设定
                    var re = GetAbnormalList(CoID);
                    if(re.s == -1)
                    {
                        result.s = -1;
                        result.d = re.d;
                        return result;
                    }
                    var ab = re.d as List<AbnormalReason>;
                    ss = new List<OStatusCnt>();
                    foreach(var a in ab)
                    {
                        var s = new OStatusCnt();
                        s.value= a.ID;
                        int i = conn.QueryFirst<int>("select count(id) from `order` where status = 7 and coid =" + CoID + " and AbnormalStatus =" + a.ID);
                        s.count = i;
                        ss.Add(s);
                    }
                    res.OrdAbnormalStatus = ss;
                    result.d = res;
                    }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }    
            return result;
        }
        ///<summary>
        ///获取type说明
        ///</summary>
        public static string GetTypeName(int type)                
        {
            string Name = string.Empty;
            if(type == 0)
            {
                Name = "普通订单";
            }
            if(type == 1)
            {
                Name = "补发订单";
            }
            if(type == 2)
            {
                Name = "换货订单";
            }
            if(type == 3)
            {
                Name = "天猫分销";
            }
            if(type == 4)
            {
                Name = "天猫供销";
            }
            if(type == 5)
            {
                Name = "协同订单";
            }
            if(type == 6)
            {
                Name = "普通订单,分销+";
            }
            if(type == 7)
            {
                Name = "补发订单,分销+";
            }
            if(type == 8)
            {
                Name = "换货订单,分销+";
            }
            if(type == 9)
            {
                Name = "天猫供销,分销+";
            }
            if(type == 10)
            {
                Name = "协同订单,分销+";
            }
            if(type == 11)
            {
                Name = "普通订单,供销+";
            }
            if(type == 12)
            {
                Name = "补发订单,供销+";
            }
            if(type == 13)
            {
                Name = "换货订单,供销+";
            }
            if(type == 14)
            {
                Name = "天猫供销,供销+";
            }
            if(type == 15)
            {
                Name = "协同订单,供销+";
            }
            if(type == 16)
            {
                Name = "普通订单,分销+,供销+";
            }
            if(type == 17)
            {
                Name = "补发订单,分销+,供销+";
            }
            if(type == 18)
            {
                Name = "换货订单,分销+,供销+";
            }
            if(type == 19)
            {
                Name = "天猫供销,分销+,供销+";
            }
            if(type == 20)
            {
                Name = "协同订单,分销+,供销+";
            }            
            return Name;
        }
        ///<summary>
        ///平台订单新增入口
        ///</summary>
        public static DataResult ImportOrderInsert(ImportOrderInsert Order,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            //检查平台单号是否已经存在，若是，则不能新增
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string wheresql = "select count(id) from `order` where soid =" + Order.SoID + " and coid =" + CoID;
                    int u = conn.QueryFirst<int>(wheresql);
                    if(u > 0)
                    {
                        result.s = -1;
                        result.d = "该订单已经导入!";
                        return result;
                    }
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }
            int shopid = 0;
            string shopsit = "";
            var logs = new List<Log>();
            List<int> idlist = new List<int>();
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{
                    string wheresql = "select * from Shop where ShopName ='" + Order.ShopName + "' and coid =" + CoID;
                    var u = conn.Query<Shop>(wheresql).AsList();
                    if(u.Count > 0)
                    {
                        shopid = u[0].ID;
                        shopsit = u[0].ShopSite;
                    }
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                var orderitem = new List<OrderItem>();
                decimal Amount = 0,TotalWeight = 0;
                int Qty = 0;
                string sqlcommand = "";
                bool IsSku = true,isRecExsit = true,isMerge = false;
                var rec = new RecInfo();
                //处理订单明细
                foreach(var a in Order.Item)
                {
                    var item = new OrderItem();
                    item.SoID = Order.SoID;
                    item.CoID = CoID;
                    item.SkuID = a.SkuID;
                    item.Qty = a.Qty;
                    item.RealPrice = a.Price.ToString();
                    item.Amount = a.Amount.ToString();
                    item.Remark = a.Remark;
                    item.Creator = UserName;
                    item.CreateDate = DateTime.Now;
                    item.Modifier = UserName;
                    item.ModifyDate= DateTime.Now;
                    item.ShopSkuID = a.ShopSkuID;
                    Amount = Amount + a.Amount;
                    Qty = Qty + a.Qty;
                    int i = CoreDBconn.QueryFirst<int>("select count(id) from coresku where skuid = '" + a.SkuID + "' and coid = " + CoID);
                    if(i == 0)
                    {
                        IsSku = false;
                    }
                    else
                    {
                        int j = CoreDBconn.QueryFirst<int>("select id from coresku where skuid = '" + a.SkuID + "' and coid = " + CoID);
                        sqlcommand = "select skuid,skuname,norm,img,goodscode,enable,saleprice,weight from coresku where id =" + j + " and coid =" + CoID;
                        var s = CoreDBconn.Query<SkuInsert>(sqlcommand).AsList();
                        item.SkuAutoID = j;
                        item.SkuName = s[0].skuname;
                        item.Norm = s[0].norm;
                        item.GoodsCode = s[0].goodscode;
                        item.SalePrice = s[0].saleprice;
                        item.DiscountRate = Math.Round(a.Price/decimal.Parse(item.SalePrice),2).ToString();
                        item.img = s[0].img;
                        item.Weight = s[0].weight;
                        item.TotalWeight = (a.Qty * decimal.Parse(s[0].weight)).ToString();
                        item.IsGift = false;
                        TotalWeight = TotalWeight + decimal.Parse(item.TotalWeight);
                    }
                    orderitem.Add(item);
                }
                //处理付款资料
                var PayList= new List<PayInfo>();
                foreach(var a in Order.Pay)
                {
                    var pay = new PayInfo();
                    pay.PayNbr = a.PayNbr;
                    pay.RecName = Order.RecName;
                    pay.SoID = Order.SoID;
                    pay.Payment = a.Payment;
                    pay.PayAccount = a.PayAccount;
                    pay.SellerAccount = a.SellerAccount;
                    pay.Platform = a.Platform;
                    pay.PayDate = a.PayDate;
                    pay.Bank = a.Bank;
                    pay.BankName= a.BankName;
                    pay.Title = a.Title;
                    pay.Name = a.Name;
                    pay.Amount = a.Amount;
                    pay.PayAmount = a.PayAmount;
                    pay.DataSource = 0;
                    pay.Status = 1;
                    pay.CoID = CoID;
                    pay.Creator = UserName;
                    pay.CreateDate = DateTime.Now;
                    pay.Confirmer = UserName;
                    pay.ConfirmDate= DateTime.Now;
                    PayList.Add(pay);
                }
                //产生订单资料
                var ord = new Order();
                ord.Type = Order.Type;
                ord.DealerType = 0;
                if(!string.IsNullOrEmpty(Order.Distributor))
                {
                    ord.DealerType = 1;
                }
                if(!string.IsNullOrEmpty(Order.SupDistributor))
                {
                    ord.DealerType = 2;
                }
                ord.OSource = Order.OSource;
                ord.ODate = Order.ODate;
                ord.CoID = CoID;
                ord.BuyerShopID = Order.BuyerShopID;
                ord.ShopID = shopid;
                ord.ShopName = Order.ShopName;
                ord.ShopSit = shopsit;
                ord.SoID = Order.SoID;
                ord.OrdQty = Qty;
                ord.Amount = Order.Amount;
                ord.SkuAmount = Amount.ToString();
                ord.PaidAmount = Order.PaidAmount.ToString();
                ord.PayAmount = Order.PayAmount.ToString();
                ord.ExAmount = Order.ExAmount.ToString();
                ord.IsInvoice = Order.IsInvoice;
                ord.InvoiceType = Order.InvoiceType;
                ord.InvoiceTitle = Order.InvoiceTitle;
                ord.InvoiceDate = Order.InvoiceDate;
                if(ord.Amount == ord.PaidAmount)
                {
                    ord.IsPaid = true;
                    ord.Status = 1;
                }
                else
                {
                    ord.IsPaid = false;
                    ord.Status = 0;
                }
                if(PayList.Count > 0)
                {
                    ord.PayDate = PayList[0].PayDate;
                    ord.PayNbr = PayList[0].PayNbr;
                }
                ord.IsCOD = Order.IsCOD;
                ord.AbnormalStatus = 0;
                ord.StatusDec = "";
                ord.ShopStatus = Order.ShopStatus;
                ord.RecName = Order.RecName;
                ord.RecLogistics = Order.RecLogistics;
                ord.RecCity = Order.RecCity;
                ord.RecDistrict = Order.RecDistrict;
                ord.RecAddress = Order.RecAddress;
                ord.RecZip = Order.RecZip;
                ord.RecTel = Order.RecTel;
                ord.RecPhone = Order.RecPhone;
                ord.RecMessage = Order.RecMessage;
                ord.ExWeight = TotalWeight.ToString();
                ord.Distributor = Order.Distributor;
                ord.SupDistributor = Order.SupDistributor;
                ord.Creator = UserName;
                ord.CreateDate = DateTime.Now;
                ord.Modifier = UserName;
                ord.ModifyDate = DateTime.Now;
                //标记异常
                if(IsSku == false)
                {
                    int reasonid = GetReasonID("商品编码缺失",CoID).s;
                    if(reasonid == -1)
                    {
                        result.s = -1;
                        result.d = "请先设定【商品编码缺失】的异常";
                        return result;
                    }
                    ord.Status = 7;
                    ord.AbnormalStatus = reasonid;
                    ord.StatusDec = "商品编码缺失";
                }
                else
                {
                    //检查订单是否符合合并的条件
                    var res = isCheckMerge(ord);
                    int reasonid = 0;
                    if(res.s == 1)
                    {
                        isMerge = true;
                        idlist = res.d as List<int>;
                        reasonid = GetReasonID("等待订单合并",CoID).s;
                        if(reasonid == -1)
                        {
                            result.s = -1;
                            result.d = "请先设定【等待订单合并】的异常";
                            return result;
                        }
                        ord.Status = 7;
                        ord.AbnormalStatus = reasonid;
                        ord.StatusDec = "等待订单合并";
                    }
                }
                //检查收货人是否存在
                sqlcommand = "select count(id) from recinfo where coid = " + CoID + " and buyerid = '" + Order.BuyerShopID + "' and receiver = '" + Order.RecName + 
                                "' and address = '" + Order.RecAddress + "' and logistics = '" + Order.RecLogistics + "' and city = '" + Order.RecCity + 
                                "' and district = '" + Order.RecDistrict + "'";
                int u = CoreDBconn.QueryFirst<int>(sqlcommand);
                if(u > 0)
                {
                    sqlcommand = "select id from recinfo where coid = " + CoID + " and buyerid = '" + Order.BuyerShopID + "' and receiver = '" + Order.RecName + 
                                "' and address = '" + Order.RecAddress + "' and logistics = '" + Order.RecLogistics + "' and city = '" + Order.RecCity + 
                                "' and district = '" + Order.RecDistrict + "'";
                    u = CoreDBconn.QueryFirst<int>(sqlcommand);
                    ord.BuyerID = u;
                    foreach(var p in PayList)
                    {
                        p.RecID = u;
                    }
                }
                else
                {
                    isRecExsit = false;
                    rec.BuyerId = ord.BuyerShopID;
                    rec.Receiver = ord.RecName;
                    rec.Tel = ord.RecTel;
                    rec.Phone = ord.RecPhone;
                    rec.Logistics = ord.RecLogistics;
                    rec.City = ord.RecCity;
                    rec.District = ord.RecDistrict;
                    rec.Address = ord.RecAddress;
                    rec.ZipCode = ord.RecZip;
                    rec.Express = ord.Express;
                    rec.ExID = ord.ExID;
                    rec.CoID = CoID;
                    rec.Creator = UserName;
                    rec.ShopSit = ord.ShopSit;
                }
                //更新收货人信息
                int count =0;
                if(isRecExsit == false)
                {
                    sqlcommand = @"INSERT INTO recinfo(BuyerId,Receiver,Tel,Phone,Logistics,City,District,Address,ZipCode,Express,ExID,CoID,Creator,ShopSit) VALUES(
                            @BuyerId,@Receiver,@Tel,@Phone,@Logistics,@City,@District,@Address,@ZipCode,@Express,@ExID,@CoID,@Creator,@ShopSit)";
                    count =CoreDBconn.Execute(sqlcommand,rec,TransCore);
                    if(count < 0)
                    {
                        result.s = -3002;
                        return result;
                    }
                    else
                    {
                        int rtn = CoreDBconn.QueryFirst<int>("select LAST_INSERT_ID()");
                        ord.BuyerID = rtn;
                        foreach(var p in PayList)
                        {
                            p.RecID = rtn;
                        }
                        rec.ID = rtn;
                    }
                }
                //新增订单资料
                sqlcommand = @"INSERT INTO `order`(Type,DealerType,OSource,ODate,CoID,BuyerID,BuyerShopID,ShopID,ShopName,ShopSit,SoID,OrdQty,Amount,SkuAmount,PaidAmount,PayAmount,
                                                   ExAmount,IsInvoice,InvoiceType,InvoiceTitle,InvoiceDate,IsPaid,Status,PayDate,PayNbr,IsCOD,AbnormalStatus,StatusDec,ShopStatus,
                                                   RecName,RecLogistics,RecCity,RecDistrict,RecAddress,RecZip,RecTel,RecPhone,RecMessage,ExWeight,Distributor,SupDistributor,Creator,Modifier) 
                                            VALUES(@Type,@DealerType,@OSource,@ODate,@CoID,@BuyerID,@BuyerShopID,@ShopID,@ShopName,@ShopSit,@SoID,@OrdQty,@Amount,@SkuAmount,@PaidAmount,@PayAmount,
                                                   @ExAmount,@IsInvoice,@InvoiceType,@InvoiceTitle,@InvoiceDate,@IsPaid,@Status,@PayDate,@PayNbr,@IsCOD,@AbnormalStatus,@StatusDec,@ShopStatus,
                                                   @RecName,@RecLogistics,@RecCity,@RecDistrict,@RecAddress,@RecZip,@RecTel,@RecPhone,@RecMessage,@ExWeight,@Distributor,@SupDistributor,@Creator,@Modifier)";
                count =CoreDBconn.Execute(sqlcommand,ord,TransCore);
                if(count < 0)
                {
                    result.s = -3002;
                    return result;
                }
                else
                {
                    int rtn = CoreDBconn.QueryFirst<int>("select LAST_INSERT_ID()");
                    rec.OID=rtn;
                    ord.ID = rtn;
                    foreach(var i in orderitem)
                    {
                        i.OID = rtn;
                    }
                    foreach(var p in PayList)
                    {
                        p.OID = rtn;
                    }
                }
                //新增日志
                var log = new Log();
                log.OID = ord.ID;
                log.SoID = ord.SoID;
                log.Type = 0;
                log.LogDate = DateTime.Now;
                log.UserName = UserName;
                log.Title = "接单时间";
                log.Remark = "接口下载订单时间";
                log.CoID = CoID;
                logs.Add(log);
                if(ord.Status == 7)
                {
                    log = new Log();
                    log.OID = ord.ID;
                    log.SoID = ord.SoID;
                    log.Type = 0;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "标记异常";
                    log.Remark = ord.StatusDec;
                    log.CoID = CoID;
                    logs.Add(log);
                }
                //新增订单明细
                sqlcommand = @"INSERT INTO orderitem(oid,soid,coid,skuautoid,skuid,skuname,norm,GoodsCode,qty,saleprice,realprice,amount,DiscountRate,img,
                                                     weight,totalweight,IsGift,Remark,creator,modifier,ShopSkuID) 
                                              VALUES(@OID,@Soid,@Coid,@Skuautoid,@Skuid,@Skuname,@Norm,@GoodsCode,@Qty,@Saleprice,@Realprice,@Amount,@DiscountRate,@Img,
                                                     @Weight,@Totalweight,@IsGift,@Remark,@Creator,@Creator,@ShopSkuID)";
                count = CoreDBconn.Execute(sqlcommand, orderitem, TransCore);
                if (count <= 0)
                {
                    result.s = -3002;
                    return result;
                }
                //新增付款资料
                sqlcommand = @"INSERT INTO payinfo(PayNbr,RecID,RecName,OID,SOID,Payment,PayAccount,SellerAccount,Platform,PayDate,Bank,BankName,Title,Name,Amount,
                                                   PayAmount,DataSource,Status,CoID,Creator,CreateDate,Confirmer,ConfirmDate) 
                                           VALUES(@PayNbr,@RecID,@RecName,@OID,@SOID,@Payment,@PayAccount,@SellerAccount,@Platform,@PayDate,@Bank,@BankName,@Title,@Name,@Amount,
                                                  @PayAmount,@DataSource,@Status,@CoID,@Creator,@CreateDate,@Confirmer,@ConfirmDate)";
                count = CoreDBconn.Execute(sqlcommand,PayList,TransCore);
                if(count < 0)
                {
                    result.s = -3002;
                    return result;
                }
                //待合并资料更新
                if(isMerge == true)
                {
                    string querySql = "select id,soid from `order` where id in @ID and coid = @Coid and status <> 7";
                    var v = CoreDBconn.Query<Order>(querySql,new{ID = idlist,Coid = CoID}).AsList();
                    if(v.Count > 0)
                    {
                        sqlcommand = @"update `order` set status = 7,abnormalstatus = @Abnormalstatus,statusdec = '等待订单合并' where id in @ID and coid = @Coid and status <> 7";
                        count =CoreDBconn.Execute(sqlcommand,new {Abnormalstatus = ord.AbnormalStatus,ID = idlist,Coid = CoID},TransCore);
                        if(count < 0)
                        {
                            result.s = -3003;
                            return result;
                        }
                        foreach(var c in v)
                        {
                            log = new Log();
                            log.OID = c.ID;
                            log.SoID = c.SoID;
                            log.Type = 0;
                            log.LogDate = DateTime.Now;
                            log.UserName = UserName;
                            log.Title = "标记异常";
                            log.Remark = "等待订单合并";
                            log.CoID = CoID;
                            logs.Add(log);
                        }
                    }
                }
                //更新日志
                string loginsert = @"INSERT INTO orderlog(OID,SoID,Type,LogDate,UserName,Title,Remark,CoID) 
                                                   VALUES(@OID,@SoID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                count =CoreDBconn.Execute(loginsert,logs,TransCore);
                if(count < 0)
                {
                    result.s = -3002;
                    return result;
                }
                //更新收货人资料
                if(isRecExsit == false)
                {
                    sqlcommand = @"update recinfo set OID = @OID where id = @ID and coid = @Coid";
                    count =CoreDBconn.Execute(sqlcommand,rec,TransCore);
                    if(count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                }
                result.d = ord.ID;
                TransCore.Commit();
            }
            catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }
            return result;
        }
        ///<summary>
        ///平台订单更新入口
        ///</summary>
        public static DataResult ImportOrderUpdate(ImportOrderUpdate Order,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            string sqlcommand = "select id,soid,coid,recname,amount,PaidAmount,PayAmount,status,IsPaid,PayDate,PayNbr,ShopStatus from `order` where soid = " + Order.SoID + " and coid = " + CoID;
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                var ord = CoreDBconn.Query<Order>(sqlcommand).AsList();
                //处理付款资料
                var PayList= new List<PayInfo>();
                decimal amount = 0;
                bool isUpdate = false;
                foreach(var a in Order.Pay)
                {
                    sqlcommand = "select count(id) from payinfo where oid = " + ord[0].ID + " and coid = " + CoID + " and PayNbr = '" + a.PayNbr + "'";
                    int i = CoreDBconn.QueryFirst<int>(sqlcommand);
                    if (i > 0) continue;
                    var pay = new PayInfo();
                    pay.PayNbr = a.PayNbr;
                    pay.RecName = ord[0].RecName;
                    pay.OID = ord[0].ID;
                    pay.SoID = ord[0].SoID;
                    pay.Payment = a.Payment;
                    pay.PayAccount = a.PayAccount;
                    pay.SellerAccount = a.SellerAccount;
                    pay.Platform = a.Platform;
                    pay.PayDate = a.PayDate;
                    pay.Bank = a.Bank;
                    pay.BankName= a.BankName;
                    pay.Title = a.Title;
                    pay.Name = a.Name;
                    pay.Amount = a.Amount;
                    pay.PayAmount = a.PayAmount;
                    pay.DataSource = 0;
                    pay.Status = 1;
                    pay.CoID = CoID;
                    pay.Creator = UserName;
                    pay.CreateDate = DateTime.Now;
                    pay.Confirmer = UserName;
                    pay.ConfirmDate= DateTime.Now;
                    PayList.Add(pay);
                    amount = amount + decimal.Parse(pay.PayAmount);
                }
                if(amount > 0 && (ord[0].Status ==1 || ord[0].Status==0 ||ord[0].Status == 7))
                {
                    ord[0].PaidAmount = (decimal.Parse(ord[0].PaidAmount) + amount).ToString();
                    ord[0].PayAmount = (decimal.Parse(ord[0].PayAmount) + amount).ToString();
                    if(ord[0].PaidAmount == ord[0].Amount)
                    {
                        ord[0].IsPaid = true;
                        if(ord[0].Status != 7)
                        {
                            ord[0].Status = 1;
                        }
                    }
                    else
                    {
                        ord[0].IsPaid = false;
                        if(ord[0].Status != 7)
                        {
                            ord[0].Status = 0;
                        }
                    }
                    if(string.IsNullOrEmpty(ord[0].PayNbr))
                    {
                        ord[0].PayNbr = PayList[0].PayNbr;
                        ord[0].PayDate = PayList[0].PayDate;
                    }
                    isUpdate = true;
                }
                if(Order.ShopStatus != ord[0].ShopStatus)
                {
                    ord[0].ShopStatus = Order.ShopStatus;
                    isUpdate = true;
                }
                if(isUpdate == true)
                {
                    ord[0].Modifier = UserName;
                    ord[0].ModifyDate = DateTime.Now;
                    sqlcommand = @"update `order` set PaidAmount = @PaidAmount,PayAmount =@PayAmount,IsPaid=@IsPaid,Status=@Status,ShopStatus=@ShopStatus,PayNbr=@PayNbr,
                                   PayDate=@PayDate,Modifier=@Modifier,ModifyDate=@ModifyDate where id = @ID and coid = @Coid";
                    int count = CoreDBconn.Execute(sqlcommand,ord[0],TransCore);
                    if(count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                }
                TransCore.Commit();
            }
            catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }
            return result;
        }
        ///<summary>
        ///快捷修改明细资料刷新
        ///</summary>
        public static DataResult RefreshOrderItemQuick(int id,int CoID)
        {
            var result = new DataResult(1,null);
            var sin = new SingleOrderItem();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{  
                        string wheresql = "select status,amount,ExWeight from `order` where id =" + id + " and coid =" + CoID;
                        var u = conn.Query<Order>(wheresql).AsList();
                        sin.Amount = u[0].Amount;
                        sin.Status = u[0].Status;
                        sin.StatusDec = Enum.GetName(typeof(OrdStatus), u[0].Status);
                        sin.Weight = u[0].ExWeight;
                    }
                    catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }
            var ff = GetSingleOrdItem(id,CoID);
            if(ff.s == -1)
            {
                result.s = -1;
                result.d = ff.d;
                return result;
            }
            sin.SkuList = ff.d as List<SkuList>;
            result.d = sin;
            return result;
        }
        ///<summary>
        ///修改明细资料刷新
        ///</summary>
        public static DataResult RefreshOrderItem(int id,int CoID)
        {
            var result = new DataResult(1,null);
            var res = new RefreshItem();
            var aa = GetOrderEdit(id,CoID);
            if(aa.s == -1)
            {
                result.s = -1;
                result.d = aa.d;
                return result;
            }
            res.Order = aa.d as OrderEdit;

            aa = GetOrderItem(id,CoID);
            if(aa.s == -1)
            {
                result.s = -1;
                result.d = aa.d;
                return result;
            }
            res.OrderItem = aa.d as List<OrderItemEdit>;

            aa = GetOrderLog(id,CoID);
            if(aa.s == -1)
            {
                result.s = -1;
                result.d = aa.d;
                return result;
            }
            res.Log = aa.d as List<OrderLog>;

            result.d = res;
            return result;
        }
        ///<summary>
        ///新增赠品
        ///</summary>
        public static DataResult InsertGift(int id,List<int> skuid,int CoID,string Username)
        {
            var result = new DataResult(1,null);  
            var res = new OrderDetailInsert();
            // var sin = new SingleOrderItem();
            var logs = new List<Log>();
            string sqlCommandText = string.Empty;
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string wheresql = "select status,soid,amount,PaidAmount from `order` where id =" + id + " and coid =" + CoID;
                var u = CoreDBconn.Query<Order>(wheresql).AsList();
                if (u.Count == 0)
                {
                    result.s = -1;
                    result.d = "此订单不存在!";
                    return result;
                }
                else
                {
                    if (u[0].Status != 0 && u[0].Status != 1 && u[0].Status != 7)
                    {
                        result.s = -1;
                        result.d = "只有待付款/已付款待审核/异常的订单才可以添加赠品!";
                        return result;
                    }
                }
                List<InsertFailReason> rt = new List<InsertFailReason>();
                List<int> rr = new List<int>();
                decimal weight = 0;
                foreach (int a in skuid)
                {
                    InsertFailReason rf = new InsertFailReason();
                    string skusql = "select skuid,skuname,norm,img,goodscode,enable,saleprice,weight from coresku where id =" + a + " and coid =" + CoID;
                    var s = CoreDBconn.Query<SkuInsert>(skusql).AsList();
                    if (s.Count == 0)
                    {
                        rf.id = a;
                        rf.reason = "此商品不存在!";
                        rt.Add(rf);
                        continue;
                    }
                    if (s[0].enable == false)
                    {
                        rf.id = a;
                        rf.reason = "此商品已停用!";
                        rt.Add(rf);
                        continue;
                    }
                    weight = weight + decimal.Parse(s[0].weight);
                    int x = CoreDBconn.QueryFirst<int>("select count(id) from orderitem where oid = " + id + " and coid =" + CoID + " and skuautoid = " + a + " AND IsGift = true");
                    if(x == 0)
                    {
                        sqlCommandText = @"INSERT INTO orderitem(oid,soid,coid,skuautoid,skuid,skuname,norm,GoodsCode,qty,saleprice,img,weight,totalweight,IsGift,creator,modifier) 
                                        VALUES(@OID,@Soid,@Coid,@Skuautoid,@Skuid,@Skuname,@Norm,@GoodsCode,@Qty,@Saleprice,@Img,@Weight,@Weight,@IsGift,@Creator,@Creator)";
                        var args = new
                        {
                            OID = id,
                            Soid = u[0].SoID,
                            Skuautoid = a,
                            Skuid = s[0].skuid,
                            Skuname = s[0].skuname,
                            Norm = s[0].norm,
                            GoodsCode = s[0].goodscode,
                            Qty = 1,
                            Saleprice = s[0].saleprice,
                            Img = s[0].img,
                            Weight = s[0].weight,
                            Coid = CoID,
                            Creator = Username,
                            IsGift = true
                        };
                        int count = CoreDBconn.Execute(sqlCommandText, args, TransCore);
                        if (count <= 0)
                        {
                            rf.id = a;
                            rf.reason = "新增明细失败!";
                            rt.Add(rf);
                            res.failIDs = rt;
                            return result;
                        }
                    }
                    else
                    {
                        sqlCommandText = @"update orderitem set qty = qty + 1,totalweight = weight * qty,modifier=@Modifier,modifydate = @ModifyDate 
                                        where oid = @ID and coid = @Coid and skuautoid = @Skuautoid and IsGift = true";
                        var args = new
                        {
                            ID = id,
                            Skuautoid = a,
                            Coid = CoID,
                            Modifier = Username,
                            ModifyDate = DateTime.Now
                        };
                        int count = CoreDBconn.Execute(sqlCommandText, args, TransCore);
                        if (count <= 0)
                        {
                            rf.id = a;
                            rf.reason = "更新明细失败!";
                            rt.Add(rf);
                            continue;
                        }
                    }
                    rr.Add(a);
                    var log = new Log();
                    log.OID = id;
                    log.SoID = u[0].SoID;
                    log.Type = 0;
                    log.LogDate = DateTime.Now;
                    log.UserName = Username;
                    log.Title = "添加赠品";
                    log.Remark = s[0].skuid;
                    log.CoID = CoID;
                    logs.Add(log);     
                }           
                //更新订单的数量和重量
                if (rr.Count > 0)
                {
                    sqlCommandText = @"update `order` set ExWeight = ExWeight + @ExWeight,OrdQty = OrdQty + @Qty,Modifier=@Modifier,ModifyDate=@ModifyDate where ID = @ID and CoID = @CoID";
                    int count = CoreDBconn.Execute(sqlCommandText, new { ExWeight = weight,Qty = rr.Count,Modifier = Username, ModifyDate = DateTime.Now, ID = id, CoID = CoID }, TransCore);
                    if (count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                    string loginsert = @"INSERT INTO orderlog(OID,SoID,Type,LogDate,UserName,Title,Remark,CoID) 
                                                VALUES(@OID,@SoID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                    int r = CoreDBconn.Execute(loginsert,logs, TransCore);
                    if (r < 0)
                    {
                        result.s = -3002;
                        return result;
                    }
                }
                res.successIDs = rr;
                res.failIDs = rt;
                if (result.s == 1)
                {
                    TransCore.Commit();
                }
                // wheresql = "select status,amount,ExWeight from `order` where id =" + id + " and coid =" + CoID;
                // u = CoreDBconn.Query<Order>(wheresql).AsList();
                // sin.Amount = u[0].Amount;
                // sin.Status = u[0].Status;
                // sin.StatusDec = Enum.GetName(typeof(OrdStatus), u[0].Status);
                // sin.Weight = u[0].ExWeight;
            }
            catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }
            var ff = RefreshOrderItemQuick(id,CoID);
            if(ff.s == -1)
            {
                result.s = -1;
                result.d = ff.d;
                return result;
            }
            res.Order = ff.d as SingleOrderItem;
            // res.Order = sin;
            result.d = res;
            return result;
        }
        ///<summary>
        ///订单明细换货
        ///</summary>
        public static DataResult ChangeOrderDetail(int id,int oid,int skuidNew,int CoID,string Username)
        {
            var result = new DataResult(1,null);  
            // var sin = new SingleOrderItem();
            var logs = new List<Log>();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string wheresql = "select status,soid,amount,PaidAmount from `order` where id =" + oid + " and coid =" + CoID;
                var u = CoreDBconn.Query<Order>(wheresql).AsList();
                if (u.Count == 0)
                {
                    result.s = -1;
                    result.d = "此订单不存在!";
                    return result;
                }
                else
                {
                    if (u[0].Status != 0 && u[0].Status != 1 && u[0].Status != 7)
                    {
                        result.s = -1;
                        result.d = "只有待付款/已付款待审核/异常的订单才可以换货!";
                        return result;
                    }
                }
                string skusql = "select skuid,skuname,norm,img,goodscode,enable,saleprice,weight from coresku where id =" + skuidNew + " and coid =" + CoID;
                var s = CoreDBconn.Query<SkuInsert>(skusql).AsList();
                if (s.Count == 0)
                {
                    result.s = -1;
                    result.d = "换货的商品不存在!";
                    return result;
                }
                if (s[0].enable == false)
                {
                    result.s = -1;
                    result.d = "换货的商品已停用!";
                    return result;
                }
                string sqlCommandText = "select * from orderitem where oid = @OID and coid = @Coid and id = @ID";
                var item = CoreDBconn.Query<OrderItem>(sqlCommandText,new{OID=oid,Coid=CoID,ID=id}).AsList();
                sqlCommandText = "select count(id) from orderitem where oid = @OID and coid = @Coid and SkuAutoID = @Sku and IsGift = @IsGift";
                int count = CoreDBconn.QueryFirst<int>(sqlCommandText,new{OID = oid,Coid = CoID,Sku=skuidNew,IsGift=item[0].IsGift});
                if(count > 0)
                {
                    result.s = -1;
                    result.d = "换货的商品已经存在于订单明细!";
                    return result;
                }
                decimal weight = decimal.Parse(item[0].TotalWeight);
                int qty = item[0].Qty;
                sqlCommandText = @"update orderitem set SkuAutoID=@SkuAutoID,SkuID=@SkuID,SkuName=@SkuName,Norm=@Norm,GoodsCode=@GoodsCode,img=@img,
                                   Weight=@Weight,TotalWeight = Weight * Qty,Modifier =@Modifier,ModifyDate = @ModifyDate where oid = @OID and 
                                   coid = @Coid and id = @ID";
                var args = new{SkuAutoID =skuidNew,SkuID=s[0].skuid, SkuName=s[0].skuname, Norm=s[0].norm, GoodsCode=s[0].goodscode, img =s[0].img,Weight=s[0].weight,
                               Modifier = Username, ModifyDate=DateTime.Now, ID=id, Coid=CoID, OID = oid};
                count = CoreDBconn.Execute(sqlCommandText,args,TransCore);
                if (count < 0)
                {
                    result.s = -3003;
                    return result;
                }
                var log = new Log();
                log.OID = oid;
                log.SoID = u[0].SoID;
                log.Type = 0;
                log.LogDate = DateTime.Now;
                log.UserName = Username;
                log.Title = "更改商品";
                log.Remark = item[0].SkuID + "=>" + s[0].skuid;
                log.CoID = CoID;

                sqlCommandText = @"update `order` set ExWeight = ExWeight - @ExWeight + @ExWeightNew,Modifier=@Modifier,ModifyDate=@ModifyDate 
                                    where ID = @ID and CoID = @CoID";
                count = CoreDBconn.Execute(sqlCommandText, new { ExWeight = weight, ExWeightNew = qty * decimal.Parse(s[0].weight) ,Modifier = Username, 
                                            ModifyDate = DateTime.Now, ID = oid, CoID = CoID }, TransCore);
                if (count < 0)
                {
                    result.s = -3003;
                    return result;
                }
                string loginsert = @"INSERT INTO orderlog(OID,SoID,Type,LogDate,UserName,Title,Remark,CoID) 
                                            VALUES(@OID,@SoID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                int r = CoreDBconn.Execute(loginsert,log, TransCore);
                if (r < 0)
                {
                    result.s = -3002;
                    return result;
                }

                TransCore.Commit();
                // wheresql = "select status,amount,ExWeight from `order` where id =" + oid + " and coid =" + CoID;
                // u = CoreDBconn.Query<Order>(wheresql).AsList();
                // sin.Amount = u[0].Amount;
                // sin.Status = u[0].Status;
                // sin.StatusDec = Enum.GetName(typeof(OrdStatus), u[0].Status);
                // sin.Weight = u[0].ExWeight;
            }
            catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }
            var ff = RefreshOrderItemQuick(oid,CoID);
            if(ff.s == -1)
            {
                result.s = -1;
                result.d = ff.d;
                return result;
            }
            // sin.SkuList = 
            result.d = ff.d as SingleOrderItem;
            return result;
        }
        ///<summary>
        ///抓取单笔订单资料
        ///</summary>
        public static DataResult GetOrderEdit(int id,int CoID)
        {
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{  
                        string sqlcommand = @"select ID,ShopName,ODate,OSource,SoID,PayDate,BuyerShopID,ExAmount,Express,ExCode,RecMessage,SendMessage,InvoiceTitle,RecLogistics,
                                            RecCity,RecDistrict,RecAddress,RecName,RecTel,RecPhone,Status,AbnormalStatus,StatusDec as AbnormalStatusDec From `order` where
                                            id = @ID and coid = @Coid";
                        var order = conn.Query<OrderEdit>(sqlcommand,new{ID=id,Coid=CoID}).AsList();
                        if(order.Count == 0)
                        {
                            result.s = -1;
                            result.d = "订单单号无效!";
                            return result;
                        }
                        order[0].OSource = Enum.GetName(typeof(OrdSource), int.Parse(order[0].OSource));
                        order[0].StatusDec = Enum.GetName(typeof(OrdStatus), order[0].Status);
                        result.d = order[0];
                    }
                    catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }
            return result;
        }
        ///<summary>
        ///抓取单笔订单的付款资料
        ///</summary>
        public static DataResult GetOrderPay(int id,int CoID)
        {
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{  
                        string sqlcommand = @"select ID,PayNbr,Payment,PayDate,PayAmount,Status From payinfo where
                                            oid = @ID and coid = @Coid and Status != 2 order by PayDate Asc";
                        var pay = conn.Query<OrderPay>(sqlcommand,new{ID=id,Coid=CoID}).AsList();
                        result.d = pay;
                    }
                    catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }
            return result;
        }
        ///<summary>
        ///抓取单笔订单的订单明细
        ///</summary>
        public static DataResult GetOrderItem(int id,int CoID)
        {
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{  
                        string sqlcommand = @"select ID,SkuAutoID,SkuID,SkuName,Norm,GoodsCode,Qty,SalePrice,RealPrice,Amount,img,IsGift,ShopSkuID From orderitem where
                                            oid = @ID and coid = @Coid";
                        var item = conn.Query<OrderItemEdit>(sqlcommand,new{ID=id,Coid=CoID}).AsList();
                        foreach(var i in item)
                        {
                            i.InvQty = GetInvQty(CoID,i.SkuAutoID);
                        }
                        result.d = item;
                    }
                    catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }
            return result;
        }
        ///<summary>
        ///抓取单笔订单的日志
        ///</summary>
        public static DataResult GetOrderLog(int id,int CoID)
        {
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{  
                        string sqlcommand = @"select LogDate,UserName,Title,Remark From orderlog where
                                            oid = @ID and coid = @Coid and type = 0 order by LogDate Asc";
                        var Log = conn.Query<OrderLog>(sqlcommand,new{ID=id,Coid=CoID}).AsList();
                        result.d = Log;
                    }
                    catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }
            return result;
        }
        ///<summary>
        ///返回订单详情
        ///</summary>
        public static DataResult GetOrderSingle(int id,int CoID)
        {
            var result = new DataResult(1,null);
            var res = new OrderSingle();
            var aa = GetOrderEdit(id,CoID);
            if(aa.s == -1)
            {
                result.s = -1;
                result.d = aa.d;
                return result;
            }
            res.Order = aa.d as OrderEdit;

            aa = GetOrderPay(id,CoID);
            if(aa.s == -1)
            {
                result.s = -1;
                result.d = aa.d;
                return result;
            }
            res.Pay = aa.d as List<OrderPay>;

            aa = GetOrderItem(id,CoID);
            if(aa.s == -1)
            {
                result.s = -1;
                result.d = aa.d;
                return result;
            }
            res.OrderItem = aa.d as List<OrderItemEdit>;

            aa = GetOrderLog(id,CoID);
            if(aa.s == -1)
            {
                result.s = -1;
                result.d = aa.d;
                return result;
            }
            res.Log = aa.d as List<OrderLog>;
            result.d = res;
            return result;
        }
        ///<summary>
        ///修改备注
        ///</summary>
        public static DataResult ModifyRemark(int id,int CoID,string Username,string Remark)
        {
            var result = new DataResult(1,null);
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string sqlcommand = @"select soid,SendMessage from `order` where id = " + id + " and coid = " + CoID;
                var order = CoreDBconn.Query<Order>(sqlcommand).AsList();
                if(order.Count == 0)
                {
                    result.s = -1;
                    result.d = "订单无效!";
                    return result;
                }
                var log = new Log();
                log.OID = id;
                log.SoID = order[0].SoID;
                log.Type = 0;
                log.LogDate = DateTime.Now;
                log.UserName = Username;
                log.Title = "修改备注";
                log.Remark = order[0].SendMessage + "=>" + Remark;
                log.CoID = CoID;

                sqlcommand = @"update `order` set SendMessage = @SendMessage,Modifier=@Modifier,ModifyDate=@ModifyDate 
                               where ID = @ID and CoID = @CoID";
                int count = CoreDBconn.Execute(sqlcommand, new {SendMessage = Remark,Modifier = Username,ModifyDate = DateTime.Now,ID = id,CoID = CoID},TransCore);
                if (count < 0)
                {
                    result.s = -3003;
                    return result;
                }
                string loginsert = @"INSERT INTO orderlog(OID,SoID,Type,LogDate,UserName,Title,Remark,CoID) 
                                            VALUES(@OID,@SoID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                count = CoreDBconn.Execute(loginsert,log, TransCore);
                if (count < 0)
                {
                    result.s = -3002;
                    return result;
                }
                TransCore.Commit();
            }
            catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }
            return result;
        }
        ///<summary>
        ///修改收货地址
        ///</summary>
        public static DataResult ModifyAddress(string Logistics,string City,string District,string Address,string Name,string tel,string phone,int OID,string UserName,int CoID)
        {
            var result = new DataResult(1,null);   
            var logs = new List<Log>();
            var OrdOlder = new Order();
            var CancleOrdAb = new Order();
            int reasonid = 0;
            List<int> idlist = new List<int>();
            int x = 0;//资料修改标记
            int z = 0;//地址修改标记
            string RecLogistics="",RecCity="",RecDistrict="",RecAddress="",RecName="";
            string querySql = "select * from `order` where id = @ID and coid = @Coid";
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    var u = conn.Query<Order>(querySql,new{ID = OID,Coid = CoID}).AsList();
                    if(u.Count == 0)
                    {
                        result.s = -1;
                        result.d = "参数无效";
                        return result;
                    }
                    else
                    {
                        if(u[0].Status != 0 && u[0].Status != 1 && u[0].Status != 7)
                        {
                            result.s = -1;
                            result.d = "只有待审核/已付款待审核/异常的订单才可以修改收货地址!";
                            return result;
                        }
                    }
                    OrdOlder = u[0] as Order;
                    RecLogistics = OrdOlder.RecLogistics;
                    RecCity = OrdOlder.RecCity;
                    RecDistrict = OrdOlder.RecDistrict;
                    RecAddress = OrdOlder.RecAddress;
                    RecName = OrdOlder.RecName;
                    string addressOlder = RecLogistics + RecCity + RecDistrict + RecAddress;
                    if(OrdOlder.RecLogistics != Logistics)
                    {
                        OrdOlder.RecLogistics = Logistics;
                        x++;
                        z++;
                    }
                    if(OrdOlder.RecCity != City)
                    {
                        OrdOlder.RecCity = City;
                        x++;
                        z++;
                    }
                    if(OrdOlder.RecDistrict != District)
                    {
                        OrdOlder.RecDistrict = District;
                        x++;
                        z++;
                    }
                    if(OrdOlder.RecAddress != Address)
                    {
                        OrdOlder.RecAddress = Address;
                        x++;
                        z++;
                    }
                    if(z > 0)
                    {
                        var log = new Log();
                        log.OID = OrdOlder.ID;
                        log.SoID = OrdOlder.SoID;
                        log.Type = 0;
                        log.LogDate = DateTime.Now;
                        log.UserName = UserName;
                        log.Title = "手动修改收货地址";
                        log.Remark = "收货地址" + addressOlder + " => " + OrdOlder.RecLogistics + OrdOlder.RecCity + OrdOlder.RecDistrict + OrdOlder.RecAddress;
                        log.CoID = CoID;
                        logs.Add(log);
                    }
                    if(OrdOlder.RecName != Name)
                    {
                        var log = new Log();
                        log.OID = OrdOlder.ID;
                        log.SoID = OrdOlder.SoID;
                        log.Type = 0;
                        log.LogDate = DateTime.Now;
                        log.UserName = UserName;
                        log.Title = "手动修改收货人";
                        log.Remark = "收货人" + OrdOlder.RecName + " => " + Name;
                        log.CoID = CoID;
                        logs.Add(log);
                        OrdOlder.RecName = Name;
                        x++;
                        z++;
                    }
                    if(OrdOlder.RecTel != tel)
                    {
                        var log = new Log();
                        log.OID = OrdOlder.ID;
                        log.SoID = OrdOlder.SoID;
                        log.Type = 0;
                        log.LogDate = DateTime.Now;
                        log.UserName = UserName;
                        log.Title = "手动修改电话";
                        log.Remark = "电话" + OrdOlder.RecTel + " => " + tel;
                        log.CoID = CoID;
                        logs.Add(log);
                        OrdOlder.RecTel = tel;
                        x++;
                    }
                    if(OrdOlder.RecPhone != phone)
                    {
                        var log = new Log();
                        log.OID = OrdOlder.ID;
                        log.SoID = OrdOlder.SoID;
                        log.Type = 0;
                        log.LogDate = DateTime.Now;
                        log.UserName = UserName;
                        log.Title = "手动修改手机";
                        log.Remark = "手机" + OrdOlder.RecPhone + " => " + phone;
                        log.CoID = CoID;
                        logs.Add(log);
                        OrdOlder.RecPhone = phone;
                        x++;
                    }
                    //检查订单是否符合合并的条件
                    if(z > 0)
                    {
                        //若订单本身是等待合并时，先判断是否需要还原资料
                        if(OrdOlder.Status == 7 && OrdOlder.StatusDec == "等待订单合并")
                        {
                            var log = new Log();
                            log.OID = OrdOlder.ID;
                            log.SoID = OrdOlder.SoID;
                            log.Type = 0;
                            log.LogDate = DateTime.Now;
                            log.UserName = UserName;
                            log.Title = "取消异常标记";
                            log.Remark = "等待订单合并(自动)";
                            log.CoID = CoID;
                            logs.Add(log);
                            if(OrdOlder.IsPaid == true)
                            {
                                OrdOlder.Status = 1;
                            }
                            else
                            {
                                OrdOlder.Status = 0;
                            }
                            OrdOlder.AbnormalStatus = 0;
                            OrdOlder.StatusDec="";
                            var ck = isCheckCancleMerge(OID,CoID,OrdOlder.BuyerShopID,RecName,RecLogistics,RecCity,RecDistrict,RecAddress);
                            if(ck.s == 1)
                            {
                                int oid = int.Parse(ck.d.ToString());
                                querySql = "select * from `order` where id = " + oid + " and coid = " + CoID;
                                var v = conn.Query<Order>(querySql).AsList();
                                CancleOrdAb = v[0] as Order;
                                log = new Log();
                                log.OID = CancleOrdAb.ID;
                                log.SoID = CancleOrdAb.SoID;
                                log.Type = 0;
                                log.LogDate = DateTime.Now;
                                log.UserName = UserName;
                                log.Title = "取消异常标记";
                                log.Remark = "等待订单合并(自动)";
                                log.CoID = CoID;
                                logs.Add(log);
                                if(CancleOrdAb.IsPaid == true)
                                {
                                    CancleOrdAb.Status = 1;
                                }
                                else
                                {
                                    CancleOrdAb.Status = 0;
                                }
                                CancleOrdAb.AbnormalStatus = 0;
                                CancleOrdAb.StatusDec="";
                                CancleOrdAb.Modifier = UserName;
                                CancleOrdAb.ModifyDate = DateTime.Now;
                            }
                        }
                        //检查订单是否符合合并的条件
                        var res = isCheckMerge(OrdOlder);
                        if(res.s == 1)
                        {
                            reasonid = GetReasonID("等待订单合并",CoID).s;
                            if(reasonid == -1)
                            {
                                result.s = -1;
                                result.d = "请先设定【等待订单合并】的异常";
                                return result;
                            }
                            OrdOlder.Status = 7;
                            OrdOlder.AbnormalStatus = reasonid;
                            OrdOlder.StatusDec="等待订单合并";
                            var log = new Log();
                            log.OID = OrdOlder.ID;
                            log.SoID = OrdOlder.SoID;
                            log.Type = 0;
                            log.LogDate = DateTime.Now;
                            log.UserName = UserName;
                            log.Title = "标记异常";
                            log.Remark = "等待订单合并(自动)";
                            log.CoID = CoID;
                            logs.Add(log);
                            idlist = res.d as List<int>;
                            querySql = "select id,soid from `order` where id in @ID and coid = @Coid and status <> 7";
                            var v = conn.Query<Order>(querySql,new{ID = idlist,Coid = CoID}).AsList();
                            if(v.Count > 0)
                            {
                                foreach(var c in v)
                                {
                                    log = new Log();
                                    log.OID = c.ID;
                                    log.SoID = c.SoID;
                                    log.Type = 0;
                                    log.LogDate = DateTime.Now;
                                    log.UserName = UserName;
                                    log.Title = "标记异常";
                                    log.Remark = "等待订单合并(自动)";
                                    log.CoID = CoID;
                                    logs.Add(log);
                                }
                            }
                            else
                            {
                                idlist = new List<int>();
                            }
                        }   
                    }
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string sqlCommandText = string.Empty;
                int count =0;
                OrdOlder.Modifier = UserName;
                OrdOlder.ModifyDate = DateTime.Now;
                if(x > 0)
                {
                    sqlCommandText = @"update `order` set RecLogistics=@RecLogistics,RecCity=@RecCity,RecDistrict=@RecDistrict,RecAddress=@RecAddress,RecName=@RecName,
                                       RecTel=@RecTel,RecPhone=@RecPhone,Status=@Status,AbnormalStatus=@AbnormalStatus,StatusDec=@StatusDec,Modifier=@Modifier,
                                       ModifyDate=@ModifyDate where ID = @ID and CoID = @CoID";
                    count =CoreDBconn.Execute(sqlCommandText,OrdOlder,TransCore);
                    if(count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                    if(CancleOrdAb.ID > 0)
                    {
                        sqlCommandText = @"update `order` set Status=@Status,AbnormalStatus=@AbnormalStatus,StatusDec=@StatusDec,Modifier=@Modifier,ModifyDate=@ModifyDate where ID = @ID and CoID = @CoID";
                        count =CoreDBconn.Execute(sqlCommandText,CancleOrdAb,TransCore);
                        if(count < 0)
                        {
                            result.s = -3003;
                            return result;
                        }
                    }
                    if(idlist.Count > 0)
                    {
                        sqlCommandText = @"update `order` set status = 7,abnormalstatus = @Abnormalstatus,statusdec = '等待订单合并' where id in @ID and coid = @Coid and status <> 7";
                        count =CoreDBconn.Execute(sqlCommandText,new {Abnormalstatus = reasonid,ID = idlist,Coid = CoID},TransCore);
                        if(count < 0)
                        {
                            result.s = -3003;
                            return result;
                        }
                    }
                    string loginsert = @"INSERT INTO orderlog(OID,SoID,Type,LogDate,UserName,Title,Remark,CoID) 
                                                VALUES(@OID,@SoID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                    count =CoreDBconn.Execute(loginsert,logs,TransCore);
                    if(count < 0)
                    {
                        result.s = -3002;
                        return result;
                    }         
                }       
                TransCore.Commit();
            }catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }
            return  result;
        }
        ///<summary>
        ///设定快递显示
        ///</summary>
        public static DataResult GetExp(int CoID,bool IsQuick,string Logistics,string City,string District)
        {
            var result = new DataResult(1,null);
            var res = new SetExp();
            if(IsQuick == true)
            {
                using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                    try{  
                            string sqlcommand = "select id from area where Name = '" + Logistics + "' and LevelType = 1";
                            int id = conn.QueryFirst<int>(sqlcommand);
                            sqlcommand = "select id from area where Name = '" + City + "' and LevelType = 2 and ParentId = " + id;
                            id = conn.QueryFirst<int>(sqlcommand);
                            sqlcommand = "select id from area where Name = '" + District + "' and LevelType = 3 and ParentId = " + id;
                            id = conn.QueryFirst<int>(sqlcommand);
                            sqlcommand = @"select kd_name,cp_name_raw,cp_location,delivery_contact,delivery_area_1,delivery_area_0 From kd_cj_02 where
                                           city_id_raw = " + id + " and kd_id_sys > 0 order by kd_name";
                            var u = conn.Query<LogisticsNetwork>(sqlcommand).AsList();
                            res.LogisticsNetwork = u;
                        }
                        catch(Exception ex){
                        result.s = -1;
                        result.d = ex.Message;
                        conn.Dispose();
                    }
                }
            }
            var Express = CoreComm.ExpressHaddle.GetExpressSimple(CoID).d as List<ExpressSimple>;
            // var a = new ExpressSimple();
            // a.ID = "A";
            // a.Name= "{清空已设快递}";
            // Express.Add(a);
            // a = new ExpressSimple();
            // a.ID = "B";
            // a.Name= "{让系统自动计算}";
            // Express.Add(a);
            // a = new ExpressSimple();
            // a.ID = "C";
            // a.Name= "{菜鸟智选物流}";
            // Express.Add(a);
            res.Express = Express;
            result.d = res;
            return result;
        }
        ///<summary>
        ///设定快递更新
        ///</summary>
        public static DataResult SetExp(List<int> oid,int CoID,string ExpID,string ExpName,string UserName)
        {            
            var result = new DataResult(1,null);
            string title = "手工指定快递";
            if(ExpID == "A")
            {
                title = "手工清空快递";
            }
            if(ExpID == "B")
            {
                title = "自动计算快递";
                var res = new DataResult(1,"圆通速递");//待新增方法
                if(res.s == -1)
                {
                    result.s = -1;
                    result.d = res.d;
                    return result;
                }
                ExpID = res.s.ToString();
                ExpName = res.d.ToString();
            }
            if(ExpID == "C")
            {
                title = "菜鸟智选快递";
                var res = new DataResult(1,"圆通速递");//待新增方法
                if(res.s == -1)
                {
                    result.s = -1;
                    result.d = res.d;
                    return result;
                }
                ExpID = res.s.ToString();
                ExpName = res.d.ToString();
            }
            var logs = new List<Log>();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string sqlcommand = "select id,soid,Express from `order` where id in @ID and coid = @Coid and status in (0,1,7)";
                var u = CoreDBconn.Query<Order>(sqlcommand,new{id = oid,Coid = CoID}).AsList();
                if(u.Count == 0)
                {
                    result.s = -1;
                    result.d = "无符合条件的订单!";
                    return result;
                }
                foreach(var a in u)
                {
                    var log = new Log();
                    log.OID = a.ID;
                    log.SoID = a.SoID;
                    log.Type = 0;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = title;
                    if(ExpID == "A")
                    {
                        log.Remark = a.Express;
                    }
                    else
                    {
                        log.Remark = ExpName;
                    }
                    log.CoID = CoID;
                    logs.Add(log);
                }
                string loginsert = @"INSERT INTO orderlog(OID,SoID,Type,LogDate,UserName,Title,Remark,CoID) 
                                                VALUES(@OID,@SoID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                int count =CoreDBconn.Execute(loginsert,logs,TransCore);
                if(count < 0)
                {
                    result.s = -3002;
                    return result;
                }     
                if(ExpID == "A")
                {
                    ExpID = null;
                    ExpName = null;
                }
                sqlcommand = @"update `order` set ExID=@ExID,Express =@Express,Modifier=@Modifier,ModifyDate=@ModifyDate where id in @ID and coid = @Coid and status in (0,1,7)";
                count =CoreDBconn.Execute(sqlcommand,new{ExID=ExpID,Express=ExpName,Modifier=UserName,ModifyDate=DateTime.Now,id = oid,Coid = CoID},TransCore);
                if(count < 0)
                {
                    result.s = -3003;
                    return result;
                }     
                TransCore.Commit();
            }
            catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }
            return result;
        }
        ///<summary>
        ///新增订单仓库分配策略
        ///</summary>
        public static DataResult InsertOrdWhStrategy(OrdWhStrategy s)
        {
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{  
                        string sqlcommand = @"INSERT INTO ord_wh_strategy(StrategyName,Priority,WarehouseID,WarehouseName,LimitLogistics,LimitShop,Distributor,ContainSkuID,ExcludeSkuID,
                                                                          ContainGoodsCode,ExcludeGoodsCode,MinOrdQty,MaxOrdQty,LoanType,CoID,Creator,Modifier) 
                                                            VALUES(@StrategyName,@Priority,@WarehouseID,@WarehouseName,@LimitLogistics,@LimitShop,@Distributor,@ContainSkuID,@ExcludeSkuID,
                                                                   @ContainGoodsCode,@ExcludeGoodsCode,@MinOrdQty,@MaxOrdQty,@LoanType,@CoID,@Creator,@Modifier)";
                        int count = conn.Execute(sqlcommand,s);
                        if(count <= 0)
                        {
                            result.s = -3002;
                            return result;
                        }
                        sqlcommand = "select ID,StrategyName from ord_wh_strategy where coid =" + s.CoID;
                        var u = conn.Query<OrdWhStrategySimple>(sqlcommand).AsList();
                        result.d = u;
                    }
                    catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }
            return result;
        }
        ///<summary>
        ///查询单笔策略资料
        ///</summary>
        public static DataResult OrdWhStrategyEdit(int ID,int CoID)
        {
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{  
                        string sqlcommand = @"select ID,StrategyName,Priority,WarehouseID,WarehouseName,LimitLogistics,LimitShop,Distributor,ContainSkuID,ExcludeSkuID,
                                              ContainGoodsCode,ExcludeGoodsCode,MinOrdQty,MaxOrdQty,LoanType,CoID,Creator,Modifier from ord_wh_strategy where id = " + 
                                              ID + " and Coid = " + CoID;
                        var u = conn.Query<OrdWhStrategyEdit>(sqlcommand).AsList();
                        if(u.Count <= 0)
                        {
                            result.s = -1;
                            result.d = "参数异常";
                            return result;
                        }
                        result.d = u[0];
                    }
                    catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }
            return result;
        }
        ///<summary>
        ///保存修改后策略资料
        ///</summary>
        public static DataResult UpdateOrdWhStrategy(OrdWhStrategy s,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{  
                        string sqlcommand = @"select * from ord_wh_strategy where id = " + s.ID + " and coid = " + CoID;
                        var u = conn.Query<OrdWhStrategy>(sqlcommand).AsList();
                        if(u.Count == 0)
                        {
                            result.s = -1;
                            result.d = "参数异常";
                            return result;
                        }
                        u[0].StrategyName = s.StrategyName;
                        u[0].Priority = s.Priority;
                        u[0].WarehouseID = s.WarehouseID;
                        u[0].WarehouseName = s.WarehouseName;
                        u[0].LimitLogistics = s.LimitLogistics;
                        u[0].LimitShop = s.LimitShop;
                        u[0].Distributor = s.Distributor;
                        u[0].ExcludeSkuID = s.ExcludeSkuID;
                        u[0].ContainSkuID = s.ContainSkuID;
                        u[0].ContainGoodsCode = s.ContainGoodsCode;
                        u[0].ExcludeGoodsCode = s.ExcludeGoodsCode;
                        u[0].MinOrdQty = s.MinOrdQty;
                        u[0].MaxOrdQty = s.MaxOrdQty;
                        u[0].LoanType = s.LoanType;
                        u[0].Modifier = UserName;
                        u[0].ModifyDate = DateTime.Now;
                        sqlcommand = @"update ord_wh_strategy set StrategyName=@StrategyName,Priority=@Priority,WarehouseID=@WarehouseID,WarehouseName=@WarehouseName,
                                       LimitLogistics=@LimitLogistics,LimitShop=@LimitShop,Distributor=@Distributor,ContainSkuID=@ContainSkuID,ExcludeSkuID=@ExcludeSkuID,
                                       ContainGoodsCode=@ContainGoodsCode,ExcludeGoodsCode=@ExcludeGoodsCode,MinOrdQty=@MinOrdQty,MaxOrdQty=@MaxOrdQty,LoanType=@LoanType,
                                       Modifier=@Modifier,ModifyDate=@ModifyDate WHERE id = @ID and Coid = @Coid";
                        int count = conn.Execute(sqlcommand,u[0]);
                        if(count <= 0)
                        {
                            result.s = -3003;
                            return result;
                        }
                        sqlcommand = "select ID,StrategyName from ord_wh_strategy where coid =" + CoID;
                        var t = conn.Query<OrdWhStrategySimple>(sqlcommand).AsList();
                        result.d = t;
                    }
                    catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }
            return result;
        }
        ///<summary>
        ///删除策略资料
        ///</summary>
        public static DataResult DeleteOrdWhStrategy(int ID,int CoID)
        {
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{  
                        string sqlcommand = @"delete from ord_wh_strategy WHERE id = " + ID + " and Coid = " + CoID;
                        int count = conn.Execute(sqlcommand);
                        if(count <= 0)
                        {
                            result.s = -3004;
                            return result;
                        }
                        sqlcommand = "select ID,StrategyName from ord_wh_strategy where coid =" + CoID;
                        var u = conn.Query<OrdWhStrategySimple>(sqlcommand).AsList();
                        result.d = u;
                    }
                    catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }
            return result;
        }
        ///<summary>
        ///策略初始资料
        ///</summary>
        public static DataResult OrdWhStrategyInit(int CoID)
        {
            var result = new DataResult(1,null);
            var res = new OrdWhStrategyInit();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{  
                        //策略List
                        string sqlcommand = "select ID,StrategyName from ord_wh_strategy where coid =" + CoID;
                        var u = conn.Query<OrdWhStrategySimple>(sqlcommand).AsList();
                        res.Strategy = u;
                        //分销商
                        sqlcommand = "select ID as value,DistributorName as label from distributor where coid =" + CoID + " and enable = true";
                        var Distributor = conn.Query<Filter>(sqlcommand).AsList();
                        var d = new Filter();
                        d.value = "0";
                        d.label = "(自营)";
                        Distributor.Add(d);
                        res.Distributor = Distributor;
                    }
                    catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{  
                        //省份List
                        string sqlcommand = "select ID as value,Name as label from area where LevelType = 1";
                        var u = conn.Query<Filter>(sqlcommand).AsList();
                        res.Logistics = u;
                    }
                    catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }
            //仓库资料
            var wh = new List<Filter>();
            var a = new Filter();
            a.value = "0";
            a.label = "(本仓)";
            wh.Add(a);
            var w = CoreComm.WarehouseHaddle.getWarelist(CoID.ToString());
            foreach(var h in w)
            {
                a = new Filter();
                a.value = h.id.ToString();
                a.label = h.warename;
                wh.Add(a);
            }
            res.Warehouse = wh;
            //贷款资料
            var ff = new List<Filter>();
            var f = new Filter();
            f.value = "0";
            f.label = "--不限定--";
            ff.Add(f);
            f = new Filter();
            f.value = "1";
            f.label = "货到付款";
            ff.Add(f);
            f = new Filter();
            f.value = "2";
            f.label = "排除货到付款(即限定在线支付或线下打款)";
            ff.Add(f);
            res.Loan = ff;
            //获取店铺List
            var shop = CoreComm.ShopHaddle.getShopEnum(CoID.ToString()) as List<shopEnum>;
            ff = new List<Filter>();
            foreach(var t in shop)
            {
                f = new Filter();
                f.value = t.value.ToString();
                f.label = t.label;
                ff.Add(f);
            }
            res.Shop = ff;  

            result.d = res;
            return result;
        }
        ///<summary>
        ///设定仓库基本显示
        ///</summary>
        public static DataResult GetWarehouse(int CoID)
        {
            var result = new DataResult(1,null);
            var wh = new List<Filter>();
            var w = CoreComm.WarehouseHaddle.getWarelist(CoID.ToString());
            foreach(var h in w)
            {
                var a = new Filter();
                a.value = h.id.ToString();
                a.label = h.warename;
                wh.Add(a);
            }
            var b = new Filter();
            b.value = "0";
            b.label = "(本仓)";
            wh.Add(b);
            b = new Filter();
            b.value = "A";
            b.label = "{依照订单分配策略自动计算}";
            wh.Add(b);
            result.d = wh;
            return result;
        }
        ///<summary>
        ///订单策略挑选仓库
        ///</summary>
        public static DataResult StrategyWarehouse(Order ord,List<OrderItem> itemm,int CoID)
        {
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{  
                        string sqlcommand = @"select * from ord_wh_strategy where coid = " + CoID;
                        var u = conn.Query<OrdWhStrategy>(sqlcommand).AsList();
                        if(u.Count == 0)
                        {
                            result.s = 0;
                            return result;
                        }
                        //将符合订单条件的资料筛选出来
                        var lst = new List<OrdWhStrategy>();
                        foreach(var a in u)
                        {
                            //限定省份
                            if(!string.IsNullOrEmpty(a.LimitLogistics))
                            {
                                if(!a.LimitLogistics.Contains(ord.RecLogistics))
                                {
                                    continue;
                                }
                            }
                            //限定店铺
                            if(!string.IsNullOrEmpty(a.LimitShop))
                            {
                                if(!a.LimitShop.Contains(ord.ShopName))
                                {
                                    continue;
                                }
                            }
                            //分销商
                            if(!string.IsNullOrEmpty(a.Distributor))
                            {
                                if(!string.IsNullOrEmpty(ord.Distributor))
                                {
                                    if(!a.Distributor.Contains(ord.Distributor))
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    if(!a.Distributor.Contains("(自营)"))
                                    {
                                        continue;
                                    }
                                }
                            }
                            //包含商品
                            if(!string.IsNullOrEmpty(a.Distributor))
                            {
                                
                            }


                            //贷款方式
                            if(a.LoanType == 1 && ord.IsCOD == false)
                            {
                                continue;
                            }
                            if(a.LoanType == 2 && ord.IsCOD == true)
                            {
                                continue;
                            }
                            lst.Add(a);
                        }
                        if(lst.Count == 0)
                        {
                            result.s = 0;
                            return result;
                        }
                        //取得优先级最小的资料
                        int Priority = 10000;
                        foreach(var a in lst)
                        {
                            if(Priority > int.Parse(a.Priority))
                            {
                                Priority = int.Parse(a.Priority);
                                result.s = int.Parse(a.WarehouseID);
                                result.d = a.WarehouseName;
                            }
                        }
                    }
                    catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }
            return result;
        }










        ///<summary>
        ///修改商品
        ///</summary>
        public static DataResult ModifySku(List<int> oid,int ModifySku,decimal ModifyPrice,int DeleteSku,int AddSku,decimal AddPrice,
                                            decimal AddQty,string AddType,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                foreach(var i in oid)
                {
                   string sqlcommand = "select id,soid from `order` where id = "+ i + " and coid = " + CoID;
                   var u = CoreDBconn.Query<Order>(sqlcommand).AsList();
                //    if()
                   sqlcommand = "select count(id) from orderitem where oid = " + i + " and coid = " + CoID + " and skuautoid = " + ModifySku;
                   int count = CoreDBconn.QueryFirst<int>(sqlcommand);
                   if (count > 0)
                   {
                       sqlcommand = "update orderitem set RealPrice=@RealPrice,Amount=RealPrice*Qty,";
                   } 

                }

                TransCore.Commit();
            }
            catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }
            return result;
        }
        
        public static DataResult SetExpress()
        {
            var result = new DataResult(1,null);




            return result;
        }
    }
}