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
                    sqlCommandText = @"update `order` set status = 7,abnormalstatus = @Abnormalstatus,statusdec = '等待订单合并' where id in @ID and coid = @Coid";
                    count =CoreDBconn.Execute(sqlCommandText,new {Abnormalstatus = reasonid,ID = idlist,Coid = CoID},TransCore);
                    if(count < 0)
                    {
                        result.s = -3003;
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
    }
}