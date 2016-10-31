using CoreModels;
using MySql.Data.MySqlClient;
using System;
using CoreModels.XyCore;
using Dapper;
using System.Collections.Generic;
using CoreModels.XyComm;
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
                        ord.ExID = 1;
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
                        var u = conn.Query<RecInfo>(wheresql).AsList();
                        if(u.Count > 0)
                        {
                            ord.BuyerID = u[0].ID;
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
                    string sqlCommandText = @"update `order` set SkuAmount = SkuAmount + @SkuAmount,Amount = SkuAmount + ExAmount,ExWeight = ExWeight + @ExWeight,Modifier=@Modifier,ModifyDate=@ModifyDate 
                                              where ID = @ID and CoID = @CoID";
                    int count = CoreDBconn.Execute(sqlCommandText, new { SkuAmount = amt, ExWeight = weight, Modifier = Username, ModifyDate = DateTime.Now, ID = id, CoID = CoID }, TransCore);
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
                decimal amt = 0, weight = 0;
                foreach (int a in skuid)
                {
                    var x = CoreDBconn.Query<OrderItem>("select id,skuid,realprice,qty,amount,totalweight from orderitem where oid = " + id + " and soid =" + soid + " and coid =" + CoID + " and skuautoid = " + a).AsList();
                    if (x.Count > 0)
                    {
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
                sqlCommandText = @"update `order` set SkuAmount = SkuAmount - @SkuAmount,Amount = SkuAmount + ExAmount,ExWeight = ExWeight - @ExWeight,Modifier=@Modifier,ModifyDate=@ModifyDate 
                                            where ID = @ID and CoID = @CoID";
                count = CoreDBconn.Execute(sqlCommandText, new { SkuAmount = amt, ExWeight = weight, Modifier = Username, ModifyDate = DateTime.Now, ID = id, CoID = CoID }, TransCore);
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
                    sqlCommandText = @"update `order` set SkuAmount = SkuAmount + @SkuAmount,Amount = SkuAmount + ExAmount,ExWeight = ExWeight + @ExWeight,Modifier=@Modifier,ModifyDate=@ModifyDate 
                                              where ID = @ID and CoID = @CoID";
                    count = CoreDBconn.Execute(sqlCommandText, new { SkuAmount = pricenew * qtynew - amt, ExWeight = qtynew * decimal.Parse(x[0].Weight) - weight, Modifier = Username, ModifyDate = DateTime.Now, ID = id, CoID = CoID }, TransCore);
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
    }
}