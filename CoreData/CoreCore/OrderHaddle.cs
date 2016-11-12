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
            string sqlcommand = "select * from `order` where 1=1"; 
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
                wheresql = wheresql + " AND abnormalstatus in (0,"+ string.Join(",", cp.StatusList) + ")" ;
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
               wheresql = wheresql + " and exists(select id from orditem where oid = order.id and skuid = '" + cp.Skuid + "')";
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
               wheresql = wheresql + " and exists(select id from orditem where oid = order.id and skuname like '%" + cp.Skuname + "%') and status in (0,1,2,7)";
            }
            if(!string.IsNullOrEmpty(cp.Norm))
            {
               wheresql = wheresql + " and exists(select id from orditem where oid = order.id and norm like '%" + cp.Norm + "%') and status in (0,1,2,7)";
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
            if (cp.ShopID != null)
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
                    var u = conn.Query<Order>(sqlcommand + wheresql).AsList();
                    res.Datacnt = count;
                    res.Pagecnt = pagecnt;
                    res.Ord = u;
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
                        string wheresql = "select id from recinfo where coid = " + CoID + " and buyerid = '" + ord.BuyerShopID + "' and receiver = '" + ord.RecName + 
                                           "' and address = '" + ord.RecAddress + "' and logistics = '" + ord.RecLogistics + "' and city = '" + ord.RecCity + 
                                           "' and district = '" + ord.RecDistrict + "'";
                        int u = conn.QueryFirst<int>(wheresql);
                        if(u > 0)
                        {
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
                log.Title = "新增订单";
                log.Remark = "新增订单";
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
        public static DataResult UpdateOrder(Order ord,string UserName,int CoID)
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
                    var u = conn.Query<Order>(querySql,new{ID = ord.ID,Coid = CoID}).AsList();
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
                        if(ord.ExAmount != null)
                        {
                            if(OrdOlder.ExAmount != ord.ExAmount)
                            {
                                var log = new Log();
                                log.OID = OrdOlder.ID;
                                log.SoID = OrdOlder.SoID;
                                log.Type = 0;
                                log.LogDate = DateTime.Now;
                                log.UserName = UserName;
                                log.Title = "手动修改运费";
                                log.Remark = "运费" + OrdOlder.ExAmount + " => " + ord.ExAmount;
                                log.CoID = CoID;
                                logs.Add(log);
                                OrdOlder.ExAmount = ord.ExAmount;
                                x++;
                            }
                        }
                        string addressOlder = OrdOlder.RecLogistics + OrdOlder.RecCity + OrdOlder.RecDistrict + OrdOlder.RecAddress;
                        if(ord.RecLogistics != null)
                        {
                            if(OrdOlder.RecLogistics != ord.RecLogistics)
                            {
                                OrdOlder.RecLogistics = ord.RecLogistics;
                                x++;
                                z++;
                            }
                        }
                        if(ord.RecCity != null)
                        {
                            if(OrdOlder.RecCity != ord.RecCity)
                            {
                                OrdOlder.RecCity = ord.RecCity;
                                x++;
                                z++;
                            }
                        }
                        if(ord.RecDistrict != null)
                        {
                            if(OrdOlder.RecDistrict != ord.RecDistrict)
                            {
                                OrdOlder.RecDistrict = ord.RecDistrict;
                                x++;
                                z++;
                            }
                        }
                        if(ord.RecAddress != null)
                        {
                            if(OrdOlder.RecAddress != ord.RecAddress)
                            {
                                OrdOlder.RecAddress = ord.RecAddress;
                                x++;
                                z++;
                            }
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
                        if(ord.RecName != null)
                        {
                            if(OrdOlder.RecName != ord.RecName)
                            {
                                var log = new Log();
                                log.OID = OrdOlder.ID;
                                log.SoID = OrdOlder.SoID;
                                log.Type = 0;
                                log.LogDate = DateTime.Now;
                                log.UserName = UserName;
                                log.Title = "手动修改收货人";
                                log.Remark = "收货人" + OrdOlder.RecName + " => " + ord.RecName;
                                log.CoID = CoID;
                                logs.Add(log);
                                OrdOlder.RecName = ord.RecName;
                                x++;
                                z++;
                            }
                        }
                        if(ord.RecTel != null)
                        {
                            if(OrdOlder.RecTel != ord.RecTel)
                            {
                                var log = new Log();
                                log.OID = OrdOlder.ID;
                                log.SoID = OrdOlder.SoID;
                                log.Type = 0;
                                log.LogDate = DateTime.Now;
                                log.UserName = UserName;
                                log.Title = "手动修改电话";
                                log.Remark = "电话" + OrdOlder.RecTel + " => " + ord.RecTel;
                                log.CoID = CoID;
                                logs.Add(log);
                                OrdOlder.RecTel = ord.RecTel;
                                x++;
                            }
                        }
                        if(ord.RecPhone != null)
                        {
                            if(OrdOlder.RecPhone != ord.RecPhone)
                            {
                                var log = new Log();
                                log.OID = OrdOlder.ID;
                                log.SoID = OrdOlder.SoID;
                                log.Type = 0;
                                log.LogDate = DateTime.Now;
                                log.UserName = UserName;
                                log.Title = "手动修改手机";
                                log.Remark = "手机" + OrdOlder.RecPhone + " => " + ord.RecPhone;
                                log.CoID = CoID;
                                logs.Add(log);
                                OrdOlder.RecPhone = ord.RecPhone;
                                x++;
                            }
                        }
                    }
                    if(ord.SendMessage != null)
                    {
                        if(OrdOlder.SendMessage != ord.SendMessage)
                        {
                            var log = new Log();
                            log.OID = OrdOlder.ID;
                            log.SoID = OrdOlder.SoID;
                            log.Type = 0;
                            log.LogDate = DateTime.Now;
                            log.UserName = UserName;
                            log.Title = "手动修改卖家备注";
                            log.Remark = "备注" + OrdOlder.SendMessage + " => " + ord.SendMessage;
                            log.CoID = CoID;
                            logs.Add(log);
                            OrdOlder.SendMessage = ord.SendMessage;
                            x++;
                            y++;
                        }
                    }
                    if(ord.InvoiceTitle != null)
                    {
                        if(OrdOlder.InvoiceTitle != ord.InvoiceTitle)
                        {
                            var log = new Log();
                            log.OID = OrdOlder.ID;
                            log.SoID = OrdOlder.SoID;
                            log.Type = 0;
                            log.LogDate = DateTime.Now;
                            log.UserName = UserName;
                            log.Title = "手动修改发票抬头";
                            log.Remark = "发票抬头" + OrdOlder.InvoiceTitle + " => " + ord.InvoiceTitle;
                            log.CoID = CoID;
                            logs.Add(log);
                            OrdOlder.InvoiceTitle = ord.InvoiceTitle;
                            x++;
                            y++;
                        }
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
                            var ck = isCheckCancleMerge(OrdOlder,OrdOlder.BuyerShopID,RecName,RecLogistics,RecCity,RecDistrict,RecAddress);
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
            ord.CoID = CoID;
            ord.ModifyDate = DateTime.Now;
            ord.Modifier = UserName;
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string sqlCommandText = string.Empty;
                int count =0;
                if(x > 0)
                {
                    sqlCommandText = @"update `order` set ExAmount = @ExAmount,Amount = ExAmount + SkuAmount,RecLogistics=@RecLogistics,RecCity=@RecCity,RecDistrict=@RecDistrict,
                                        RecAddress=@RecAddress,RecName=@RecName,RecTel=@RecTel,RecPhone=@RecPhone,SendMessage=@SendMessage,InvoiceTitle=@InvoiceTitle,Status=@Status,
                                        AbnormalStatus=@AbnormalStatus,StatusDec=@StatusDec,Modifier=@Modifier,ModifyDate=@ModifyDate where ID = @ID and CoID = @CoID";
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
            return  result;
        }
        ///<summary>
        ///检查订单是否需取消等待合并
        ///</summary>
        public static DataResult isCheckCancleMerge(Order ord,string BuyerShopID,string RecName,string RecLogistics,string RecCity,string RecDistrict,string RecAddress)
        {
            var result = new DataResult(1,null);   
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string wheresql = "select id from `order` where coid = " + ord.CoID + " and id != "+ ord.ID + " and buyershopid = '" + BuyerShopID + "' and recname = '" + RecName + 
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
        public static DataResult InsertOrderDetail(int id,long soid,List<int> skuid,int CoID,string Username)
        {
            var result = new DataResult(1,null);  
            var res = new OrderDetailInsert();
            var logs = new List<Log>();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string wheresql = "select status from `order` where id =" + id + " and soid = " + soid + " and coid =" + CoID;
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
                    int x = CoreDBconn.QueryFirst<int>("select count(id) from orderitem where oid = " + id + " and soid =" + soid + " and coid =" + CoID + " and skuautoid = " + a);
                    if (x > 0)
                    {
                        rf.id = a;
                        rf.reason = null;
                        rt.Add(rf);
                        continue;
                    }
                    string sqlCommandText = @"INSERT INTO orderitem(oid,soid,coid,skuautoid,skuid,skuname,norm,qty,saleprice,realprice,amount,img,weight,totalweight,creator,modifier) 
                                            VALUES(@OID,@Soid,@Coid,@Skuautoid,@Skuid,@Skuname,@Norm,@Qty,@Saleprice,@Saleprice,@Saleprice,@Img,@Weight,@Weight,@Creator,@Creator)";
                    var args = new
                    {
                        OID = id,
                        Soid = soid,
                        Skuautoid = a,
                        Skuid = s[0].skuid,
                        Skuname = s[0].skuname,
                        Norm = s[0].norm,
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
                        log.SoID = soid;
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
                    string sqlCommandText = @"update `order` set SkuAmount = SkuAmount + @SkuAmount,Amount = SkuAmount + ExAmount,ExWeight = ExWeight + @ExWeight,
                                            OrdQty = @OrdQty,Modifier=@Modifier,ModifyDate=@ModifyDate where ID = @ID and CoID = @CoID";
                    int count = CoreDBconn.Execute(sqlCommandText, new { SkuAmount = amt, ExWeight = weight, OrdQty = rr.Count,Modifier = Username, ModifyDate = DateTime.Now, ID = id, CoID = CoID }, TransCore);
                    if (count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                }
                string loginsert = @"INSERT INTO orderlog(OID,SoID,Type,LogDate,UserName,Title,Remark,CoID) 
                                            VALUES(@OID,@SoID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                int r = CoreDBconn.Execute(loginsert,logs, TransCore);
                if (r < 0)
                {
                    result.s = -3002;
                    return result;
                }

                res.successIDs = rr;
                res.failIDs = rt;
                result.d = res;

                if (result.s == 1)
                {
                    TransCore.Commit();
                }
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
        ///删除订单明细
        ///</summary>
        public static DataResult DeleteOrderDetail(int id,long soid,List<int> skuid,int CoID,string Username)
        {
            var result = new DataResult(1,null);  
            var logs = new List<Log>();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string wheresql = "select status from `order` where id =" + id + " and soid = " + soid + " and coid =" + CoID;
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
                foreach (int a in skuid)
                {
                    var x = CoreDBconn.Query<OrderItem>("select id,skuid,realprice,qty,amount,totalweight from orderitem where oid = " + id + " and soid =" + soid + " and coid =" + CoID + " and skuautoid = " + a).AsList();
                    if (x.Count > 0)
                    {
                        qty += x[0].Qty;
                        amt += decimal.Parse(x[0].Amount);
                        weight += decimal.Parse(x[0].TotalWeight);
                        var log = new Log();
                        log.OID = id;
                        log.SoID = soid;
                        log.Type = 0;
                        log.LogDate = DateTime.Now;
                        log.UserName = Username;
                        log.Title = "删除商品";
                        log.Remark = x[0].SkuID + "(" + x[0].RealPrice + "*" + x[0].Qty + ")";
                        log.CoID = CoID;
                        logs.Add(log);
                    }
                }
                string sqlCommandText = @"delete from orderitem where oid = @OID and soid = @SoID and coid = @CoID and skuautoid in @Sku";
                int count = CoreDBconn.Execute(sqlCommandText,new {OID=id,SoID=soid,CoID=CoID,Sku = skuid}, TransCore);
                if (count < 0)
                {
                    result.s = -3004;
                    return result;
                }
                //更新订单的金额和重量
                sqlCommandText = @"update `order` set SkuAmount = SkuAmount - @SkuAmount,Amount = SkuAmount + ExAmount,ExWeight = ExWeight - @ExWeight,
                                  OrdQty = OrdQty - @OrdQty, Modifier=@Modifier,ModifyDate=@ModifyDate  where ID = @ID and CoID = @CoID";
                count = CoreDBconn.Execute(sqlCommandText, new { SkuAmount = amt, ExWeight = weight, OrdQty = qty,Modifier = Username, ModifyDate = DateTime.Now, ID = id, CoID = CoID }, TransCore);
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
        ///更新订单明细
        ///</summary>
        public static DataResult UpdateOrderDetail(int id,long soid,int skuid,int CoID,string Username,decimal price,int qty)
        {
            var result = new DataResult(1,null);  
            var logs = new List<Log>();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string wheresql = "select status from `order` where id =" + id + " and soid = " + soid + " and coid =" + CoID;
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
                var x = CoreDBconn.Query<OrderItem>("select id,skuid,realprice,qty,amount,totalweight,weight,saleprice from orderitem where oid = " + id + " and soid =" + soid + " and coid =" + CoID + " and skuautoid = " + skuid).AsList();
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
                            log.OID = id;
                            log.SoID = soid;
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
                            log.OID = id;
                            log.SoID = soid;
                            log.Type = 0;
                            log.LogDate = DateTime.Now;
                            log.UserName = Username;
                            log.Title = "修改数量";
                            log.Remark = x[0].SkuID+ " " + x[0].Qty + "=>" + qtynew;
                            log.CoID = CoID;
                            logs.Add(log);
                        }
                    }
                    sqlCommandText = sqlCommandText + "amount = @Amount,DiscountRate = @DiscountRate,TotalWeight=@TotalWeight,modifier=@Modifier,ModifyDate=@ModifyDate " + 
                                                       "where oid = @Oid and soid = @Soid and coid = @Coid and skuautoid = @Sku";
                    p.Add("@Amount", pricenew * qtynew);
                    p.Add("@DiscountRate", pricenew/decimal.Parse(x[0].SalePrice));
                    p.Add("@TotalWeight", qtynew * decimal.Parse(x[0].Weight));
                    p.Add("@Modifier", Username);
                    p.Add("@ModifyDate", DateTime.Now);
                    p.Add("@Oid", id);
                    p.Add("@Soid", soid);
                    p.Add("@Coid", CoID);
                    p.Add("@Sku", skuid);
                    int count = CoreDBconn.Execute(sqlCommandText, p, TransCore);
                    if (count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                    sqlCommandText = @"update `order` set SkuAmount = SkuAmount + @SkuAmount,Amount = SkuAmount + ExAmount,ExWeight = ExWeight + @ExWeight,ordqty = ordqty + @Ordqty ,Modifier=@Modifier,ModifyDate=@ModifyDate 
                                              where ID = @ID and CoID = @CoID";
                    count = CoreDBconn.Execute(sqlCommandText, new { SkuAmount = pricenew * qtynew - amt, ExWeight = qtynew * decimal.Parse(x[0].Weight) - weight,Ordqty = qtynew - x[0].Qty, Modifier = Username, ModifyDate = DateTime.Now, ID = id, CoID = CoID }, TransCore);
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
                string wheresql = "select * from `order` where id =" + pay.OID + " and soid = " + pay.SoID + " and coid =" + CoID;
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
                if(PaidAmount + decimal.Parse(pay.PayAmount) != Amount &&　ord.StatusDec != "部分付款")
                {
                    if(ord.Status == 7 )
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
                        ord.Status = 0;
                        ord.AbnormalStatus = 0;
                        ord.StatusDec = "";
                    }
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
                    ord.Status = 1;
                    ord.AbnormalStatus = 0;
                    ord.StatusDec = "";
                }
                else
                {
                    if(ord.Status != 7 && ord.StatusDec != "部分付款")
                    {
                        ord.IsPaid = false;
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
            
            return result;
        }
        ///<summary>
        ///取消支付审核
        ///</summary>
        public static DataResult CancleConfirmPay(int oid,long soid,int payid,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            var logs = new List<Log>();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string wheresql = "select * from `order` where id =" + oid + " and soid = " + soid + " and coid =" + CoID;
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
                log.SoID = soid;
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
                if(PaidAmount - pay > 0 &&　ord.StatusDec != "部分付款")
                {
                    if(ord.Status == 7 )
                    {
                        log = new Log();
                        log.OID = oid;
                        log.SoID = soid;
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
                    log = new Log();
                    log.OID = oid;
                    log.SoID = soid;
                    log.Type = 0;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "判断金额不符";
                    log.Remark = "标记部分付款";
                    log.CoID = CoID;
                    logs.Add(log);
                }
                if(PaidAmount - pay == 0 &&　ord.StatusDec == "部分付款" && ord.Status == 7)
                {
                    log = new Log();
                    log.OID = oid;
                    log.SoID = soid;
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
                if(decimal.Parse(ord.PaidAmount) == 0 && ord.Status != 7)
                {
                    ord.Status = 0;
                    ord.AbnormalStatus = 0;
                    ord.StatusDec = "";
                }
                if(decimal.Parse(ord.PaidAmount) > 0)
                {
                    if(ord.Status != 7 && ord.StatusDec != "部分付款")
                    {
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
            
            return result;
        }
        ///<summary>
        ///支付审核
        ///</summary>
        public static DataResult ConfirmPay(int oid,long soid,int payid,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            var logs = new List<Log>();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string wheresql = "select * from `order` where id =" + oid + " and soid = " + soid + " and coid =" + CoID;
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
                log.SoID = soid;
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
                if(PaidAmount +  pay != Amount &&　ord.StatusDec != "部分付款")
                {
                    if(ord.Status == 7 )
                    {
                        log = new Log();
                        log.OID = oid;
                        log.SoID = soid;
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
                    log = new Log();
                    log.OID = oid;
                    log.SoID = soid;
                    log.Type = 0;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "判断金额不符";
                    log.Remark = "标记部分付款";
                    log.CoID = CoID;
                    logs.Add(log);
                }
                if(PaidAmount + pay == Amount &&　ord.StatusDec == "部分付款" && ord.Status == 7)
                {
                    log = new Log();
                    log.OID = oid;
                    log.SoID = soid;
                    log.Type = 0;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "取消异常标记";
                    log.Remark = ord.StatusDec;
                    log.CoID = CoID;
                    logs.Add(log);
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
                    ord.Status = 1;
                    ord.AbnormalStatus = 0;
                    ord.StatusDec = "";
                }
                else
                {
                    if(ord.Status != 7 && ord.StatusDec != "部分付款")
                    {
                        ord.IsPaid = false;
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
            
            return result;
        }
        ///<summary>
        ///支付作废
        ///</summary>
        public static DataResult CanclePay(int oid,long soid,int payid,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            var logs = new List<Log>();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string wheresql = "select status from `order` where id =" + oid + " and soid = " + soid + " and coid =" + CoID;
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
                log.SoID = soid;
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
            
            return result;
        }
        ///<summary>
        ///快速支付
        ///</summary>
        public static DataResult QuickPay(int id,long soid,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            var logs = new List<Log>();
            var pay = new PayInfo();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string wheresql = "select * from `order` where id =" + id + " and soid = " + soid + " and coid =" + CoID;
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
                pay.SoID = soid;
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
                ord.Status = 1;
                ord.AbnormalStatus = 0;
                ord.StatusDec = "";
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
        public static DataResult GetInitData(int CoID,int NumPerPage)
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
                        s.Value = myCode;
                        s.Label = Enum.GetName(typeof(OrdStatus), myCode);//获取名称
                        if(myCode == 0 ||myCode == 1 ||myCode == 2 ||myCode == 7)
                        {
                            int i = conn.QueryFirst<int>("select count(id) from `order` where status = " + myCode + " and coid =" + CoID);
                            s.Count = i;
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
                        s.Value= a.ID;
                        s.Label = a.Name;
                        int i = conn.QueryFirst<int>("select count(id) from `order` where status = 7 and coid =" + CoID + " and AbnormalStatus =" + a.ID);
                        s.Count = i;
                        ss.Add(s);
                    }
                    res.OrdAbnormalStatus = ss;
                    //分销商
                    string sqlcommand = "select ID,DistributorName as Name from distributor where coid =" + CoID + " and enable = true";
                    var Distributor = conn.Query<AbnormalReason>(sqlcommand).AsList();
                    res.Distributor = Distributor;
                    //订单资料
                    sqlcommand = @"select count(id)  from `order` where coid =" + CoID + " and status in (0,1,2,3,7)";
                    int count = conn.QueryFirst<int>(sqlcommand);
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(NumPerPage.ToString()));
                    sqlcommand = @"select ID,Type,DealerType,IsMerge,IsSplit,SoID,ODate,PayDate,BuyerShopID,ShopName,Amount,PaidAmount,ExAmount,IsCOD,Status,AbnormalStatus,
                                   StatusDec,RecMessage,SendMessage,Express,RecLogistics,RecCity,RecDistrict,RecAddress,RecName,ExWeight,Distributor,SupDistributor,InvoiceTitle,
                                   PlanDate,SendWarehouse,SendDate,ExCode  from `order` where coid =" + CoID + " and status in (0,1,2,3,7) order by ID Desc limit 0," + NumPerPage.ToString();
                    var u = conn.Query<OrderQuery>(sqlcommand).AsList();
                    res.Datacnt = count;
                    res.Pagecnt = pagecnt;
                    res.Ord = u;
                    foreach(var a in res.Ord)
                    {
                        if(a.IsMerge == true)
                        {
                            var soid = new List<long>();
                            soid.Add(a.SoID);
                            sqlcommand = "select soid from `ord` where coid = " + CoID + " and MergeOID = " + a.ID;
                            var y = conn.Query<Order>(sqlcommand).AsList();
                            foreach(var b in y)
                            {
                                soid.Add(b.SoID);
                            }
                            a.SoIDList = soid;
                        }
                        sqlcommand = "select SkuAutoID,Img,Qty,GoodsCode,SkuID,SkuName,Norm,RealPrice from orderitem where oid = " + a.ID + " and coid =" + CoID;
                        var item = conn.Query<SkuList>(sqlcommand).AsList();
                        a.SkuList = item;
                    }
                    }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }    
            //买家留言条件设定
            var ff = new List<Filter>();
            var f = new Filter();
            f.Value = "A";
            f.Label = "不过滤";
            ff.Add(f);
            f = new Filter();
            f.Value = "N";
            f.Label = "无留言";
            ff.Add(f);
            f = new Filter();
            f.Value = "Y";
            f.Label = "有留言";
            ff.Add(f);
            res.BuyerRemark = ff;
            //卖家备注
            ff = new List<Filter>();
            f = new Filter();
            f.Value = "A";
            f.Label = "不过滤";
            ff.Add(f);
            f = new Filter();
            f.Value = "N";
            f.Label = "无备注";
            ff.Add(f);
            f = new Filter();
            f.Value = "Y";
            f.Label = "有备注";
            ff.Add(f);
            res.SellerRemark = ff;
            //订单资料来源
            var oo = new List<AbnormalReason>();
            var o = new AbnormalReason();
            o.ID = -1;
            o.Name = "--- 不限 ---";
            oo.Add(o);
            foreach (int  myCode in Enum.GetValues(typeof(OrdSource)))
            {
                o = new AbnormalReason();
                o.ID = myCode;
                o.Name = Enum.GetName(typeof(OrdSource), myCode);//获取名称
                oo.Add(o);
            }
            res.OSource = oo;
            //订单类型
            oo = new List<AbnormalReason>();
            o = new AbnormalReason();
            o.ID = 0;
            o.Name = "普通订单";
            oo.Add(o);
            o = new AbnormalReason();
            o.ID = 1;
            o.Name = "补发订单";
            oo.Add(o);
            o = new AbnormalReason();
            o.ID = 2;
            o.Name = "换货订单";
            oo.Add(o);
            o = new AbnormalReason();
            o.ID = 3;
            o.Name = "天猫分销";
            oo.Add(o);
            o = new AbnormalReason();
            o.ID = 4;
            o.Name = "天猫供销";
            oo.Add(o);
            o = new AbnormalReason();
            o.ID = 5;
            o.Name = "协同订单";
            oo.Add(o);
            o = new AbnormalReason();
            o.ID = 6;
            o.Name = "普通订单,分销+";
            oo.Add(o);
            o = new AbnormalReason();
            o.ID = 7;
            o.Name = "补发订单,分销+";
            oo.Add(o);
            o = new AbnormalReason();
            o.ID = 8;
            o.Name = "换货订单,分销+";
            oo.Add(o);
            o = new AbnormalReason();
            o.ID = 9;
            o.Name = "天猫供销,分销+";
            oo.Add(o);
            o = new AbnormalReason();
            o.ID = 10;
            o.Name = "协同订单,分销+";
            oo.Add(o);
            o = new AbnormalReason();
            o.ID = 11;
            o.Name = "普通订单,供销+";
            oo.Add(o);
            o = new AbnormalReason();
            o.ID = 12;
            o.Name = "补发订单,供销+";
            oo.Add(o);
            o = new AbnormalReason();
            o.ID = 13;
            o.Name = "换货订单,供销+";
            oo.Add(o);
            o = new AbnormalReason();
            o.ID = 14;
            o.Name = "天猫供销,供销+";
            oo.Add(o);
            o = new AbnormalReason();
            o.ID = 15;
            o.Name = "协同订单,供销+";
            oo.Add(o);
            o = new AbnormalReason();
            o.ID = 16;
            o.Name = "普通订单,分销+,供销+";
            oo.Add(o);
            o = new AbnormalReason();
            o.ID = 17;
            o.Name = "补发订单,分销+,供销+";
            oo.Add(o);
            o = new AbnormalReason();
            o.ID = 18;
            o.Name = "换货订单,分销+,供销+";
            oo.Add(o);
            o = new AbnormalReason();
            o.ID = 19;
            o.Name = "天猫供销,分销+,供销+";
            oo.Add(o);
            o = new AbnormalReason();
            o.ID = 20;
            o.Name = "协同订单,分销+,供销+";
            oo.Add(o);
            res.OType = oo;
            //贷款方式
            ff = new List<Filter>();
            f = new Filter();
            f.Value = "A";
            f.Label = "所有(不区分货款方式)";
            ff.Add(f);
            f = new Filter();
            f.Value = "N";
            f.Label = "在线支付(非货到付款)";
            ff.Add(f);
            f = new Filter();
            f.Value = "Y";
            f.Label = "货到付款";
            ff.Add(f);
            res.LoanType = ff;
            //是否付款
            ff = new List<Filter>();
            f = new Filter();
            f.Value = "A";
            f.Label = "所有(不区分是否付款)";
            ff.Add(f);
            f = new Filter();
            f.Value = "N";
            f.Label = "未付款";
            ff.Add(f);
            f = new Filter();
            f.Value = "Y";
            f.Label = "已付款";
            ff.Add(f);
            res.IsPaid = ff;
            //获取店铺List
            res.Shop = CoreComm.ShopHaddle.getShopEnum(CoID.ToString()) as List<shopEnum>;  
            //快递Lsit
            res.Express = GetExpress(CoID).d as List<AbnormalReason>;
            //其他
            oo = new List<AbnormalReason>();
            o = new AbnormalReason();
            o.ID = 0;
            o.Name = "合并订单";
            oo.Add(o);
            o = new AbnormalReason();
            o.ID = 1;
            o.Name = "拆分订单";
            oo.Add(o);
            o = new AbnormalReason();
            o.ID = 2;
            o.Name = "非合并订单";
            oo.Add(o);
            o = new AbnormalReason();
            o.ID = 3;
            o.Name = "非拆分订单";
            oo.Add(o);
            o = new AbnormalReason();
            o.ID = 4;
            o.Name = "开发票";
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






            return result;
        }
        ///<summary>
        ///修改商品
        ///</summary>
        public static DataResult ModifySku()
        {
            var result = new DataResult(1,null);






            return result;
        }


        ///<summary>
        ///抓取快递List
        ///</summary>
        public static DataResult GetExpress(int CoID)
        {
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{  
                    //快递公司
                    string sqlcommand = "select ID,ExpName as Name from express where coid =" + CoID + " and enable = true";
                    var Express = conn.Query<AbnormalReason>(sqlcommand).AsList();
                    result.d = Express;   
                    }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }    
            return result;
        }
        ///<summary>
        ///设定快递
        ///</summary>
        public static DataResult SetExpress()
        {
            var result = new DataResult(1,null);




            return result;
        }
    }
}