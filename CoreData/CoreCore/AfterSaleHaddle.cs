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
            res.Status = ff;
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
            res.Type = ff;
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
            //货物状态
            ff = new List<Filter>();
            f = new Filter();
            f.value = "不限";
            f.label = "---不限---";
            ff.Add(f);
            f = new Filter();
            f.value = "卖家已收到退货";
            f.label = "卖家已收到退货";
            ff.Add(f);
            f = new Filter();
            f.value = "买家未收到货";
            f.label = "买家未收到货";
            ff.Add(f);
            f = new Filter();
            f.value = "买家已收到货";
            f.label = "买家已收到货";
            ff.Add(f);
            f = new Filter();
            f.value = "买家已退货";
            f.label = "买家已退货";
            ff.Add(f);
            f = new Filter();
            f.value = "实收与登记不符";
            f.label = "实收与登记不符";
            ff.Add(f);
            res.GoodsStatus = ff;
            //线上状态
            ff = new List<Filter>();
            f = new Filter();
            f.value = "等候卖家同意";
            f.label = "等候卖家同意";
            ff.Add(f);
            f = new Filter();
            f.value = "等候买家退货";
            f.label = "等候买家退货";
            ff.Add(f);
            f = new Filter();
            f.value = "等候卖家确认发货";
            f.label = "等候卖家确认发货";
            ff.Add(f);
            f = new Filter();
            f.value = "卖家拒绝退款";
            f.label = "卖家拒绝退款";
            ff.Add(f);
            f = new Filter();
            f.value = "退款关闭";
            f.label = "退款关闭";
            ff.Add(f);
            f = new Filter();
            f.value = "退款成功";
            f.label = "退款成功";
            ff.Add(f);
            res.ShopStatus = ff;
            //退款状态
            ff = new List<Filter>();
            f = new Filter();
            f.value = "未申请状态";
            f.label = "未申请状态";
            ff.Add(f);
            f = new Filter();
            f.value = "退款先行垫付申请中";
            f.label = "退款先行垫付申请中";
            ff.Add(f);
            f = new Filter();
            f.value = "退款先行垫付垫付完成";
            f.label = "退款先行垫付垫付完成";
            ff.Add(f);
            f = new Filter();
            f.value = "退款先行垫付卖家拒绝收货";
            f.label = "退款先行垫付卖家拒绝收货";
            ff.Add(f);
            f = new Filter();
            f.value = "退款先行垫付垫付关闭";
            f.label = "退款先行垫付垫付关闭";
            ff.Add(f);
            f = new Filter();
            f.value = "退款先行垫付垫付分账成功";
            f.label = "退款先行垫付垫付分账成功";
            ff.Add(f);
            res.RefundStatus = ff;
            result.d = res;
            return result;
        }
        ///<summary>
        ///查询售后单List
        ///</summary>
        public static DataResult GetAsList(AfterSaleParm cp)
        {
            var result = new DataResult(1,null);    
            string sqlcount = "select count(id) from aftersale where 1=1";
            string sqlcommand = @"select ID,OID,RegisterDate,BuyerShopID,RecName,Type,RecPhone,SalerReturnAmt,BuyerUpAmt,RealReturnAmt,ReturnAccount,ShopName,WarehouseID,RecWarehouse,
                                  SoID,IssueType,OrdType,Remark,Status,ShopStatus,Result,GoodsStatus,ModifyDate,Modifier,Creator,RefundStatus,Express,ExCode,IsSubmit,ConfirmDate
                                  from aftersale where 1=1";         
            string wheresql = string.Empty;
            if(cp.CoID != 1)//公司编号
            {
                wheresql = wheresql + " and coid = " + cp.CoID;
            }
            if(!string.IsNullOrEmpty(cp.ExCode))//快递单号
            {
               wheresql = wheresql + " and excode = '" + cp.ExCode + "'";
            }
            if(cp.SoID > 0)//外部订单号
            {
                wheresql = wheresql + " and soid = " + cp.SoID;
            }
            if(cp.OID > 0)//内部订单号
            {
                wheresql = wheresql + " and oid = " + cp.OID;
            }
            if(cp.ID > 0)//售后单号
            {
                wheresql = wheresql + " and id = " + cp.ID;
            }
            if(!string.IsNullOrEmpty(cp.BuyerShopID))//买家账号
            {
               wheresql = wheresql + " and buyershopid like '%" + cp.BuyerShopID + "%'";
            }
            if(!string.IsNullOrEmpty(cp.RecName))//收货人
            {
               wheresql = wheresql + " and recname like '%" + cp.RecName + "%'";
            }
            if(!string.IsNullOrEmpty(cp.Modifier))//修改人
            {
               wheresql = wheresql + " and Modifier like '%" + cp.Modifier + "%'";
            }
            if(!string.IsNullOrEmpty(cp.RecPhone))//手机
            {
               wheresql = wheresql + " and recphone like '%" + cp.RecPhone + "%'";
            }
            if(!string.IsNullOrEmpty(cp.RecTel))//固定电话
            {
               wheresql = wheresql + " and rectel like '%" + cp.RecTel + "%'";
            }
            if(!string.IsNullOrEmpty(cp.Creator))//制单人
            {
               wheresql = wheresql + " and Creator like '%" + cp.Creator + "%'";
            }
            if(!string.IsNullOrEmpty(cp.Remark))//备注
            {
               wheresql = wheresql + " and Remark like '%" + cp.Remark + "%'";
            }
            if(!string.IsNullOrEmpty(cp.DateType) && cp.DateType.ToUpper() == "REGISTERDATE")
            {
                if(cp.DateStart > DateTime.Parse("1900-01-01"))
                {
                    wheresql = wheresql + " AND RegisterDate >= '" + cp.DateStart + "'" ;
                }
                if(cp.DateEnd > DateTime.Parse("1900-01-01"))
                {
                    wheresql = wheresql + " AND RegisterDate <= '" + cp.DateEnd + "'" ;
                }
            }
            if(!string.IsNullOrEmpty(cp.DateType) && cp.DateType.ToUpper() == "MODIFYDATE")
            {
                if(cp.DateStart > DateTime.Parse("1900-01-01"))
                {
                    wheresql = wheresql + " AND ModifyDate >= '" + cp.DateStart + "'" ;
                }
                if(cp.DateEnd > DateTime.Parse("1900-01-01"))
                {
                    wheresql = wheresql + " AND ModifyDate <= '" + cp.DateEnd + "'" ;
                }
            }
            if(!string.IsNullOrEmpty(cp.DateType) && cp.DateType.ToUpper() == "CONFIRMDATE")
            {
                if(cp.DateStart > DateTime.Parse("1900-01-01"))
                {
                    wheresql = wheresql + " AND ConfirmDate >= '" + cp.DateStart + "'" ;
                }
                if(cp.DateEnd > DateTime.Parse("1900-01-01"))
                {
                    wheresql = wheresql + " AND ConfirmDate <= '" + cp.DateEnd + "'" ;
                }
            }
            if(!string.IsNullOrEmpty(cp.SkuID))
            {
               wheresql = wheresql + " and exists(select id from aftersaleitem where RID = aftersale.id and skuid = '" + cp.SkuID + "')";
            }
            if(!string.IsNullOrEmpty(cp.GoodsCode))
            {
               wheresql = wheresql + " and exists(select id from aftersaleitem where RID = aftersale.id and GoodsCode = '" + cp.GoodsCode + "')";
            }
            if(!string.IsNullOrEmpty(cp.IsNoOID) && cp.IsNoOID.ToUpper() == "Y")
            {
                wheresql = wheresql + " AND OID = -1" ;
            }
            if(!string.IsNullOrEmpty(cp.IsNoOID) && cp.IsNoOID.ToUpper() == "N")
            {
                wheresql = wheresql + " AND OID > 0" ;
            }
            if(!string.IsNullOrEmpty(cp.IsInterfaceLoad) && (cp.IsInterfaceLoad.ToUpper() == "Y" || cp.IsInterfaceLoad.ToUpper() == "N"))
            {
                wheresql = wheresql + " AND IsInterfaceLoad = '" + cp.IsInterfaceLoad.ToUpper() + "'" ;
            }
            if(!string.IsNullOrEmpty(cp.IsSubmitDis) && (cp.IsSubmitDis.ToUpper() == "Y" || cp.IsSubmitDis.ToUpper() == "N"))
            {
                wheresql = wheresql + " AND IsSubmitDis = '" + cp.IsSubmitDis.ToUpper() + "'" ;
            }
            if(cp.ShopID > 0)
            {
                wheresql = wheresql + " AND ShopID = " + cp.ShopID ;
            }
            if(cp.Status > 0)
            {
                wheresql = wheresql + " AND Status = " + cp.Status ;
            }
            if(!string.IsNullOrEmpty(cp.GoodsStatus) && !cp.GoodsStatus.Contains("不限"))
            {
                wheresql = wheresql + " AND GoodsStatus = '" + cp.GoodsStatus + "'" ;
            }
            if(cp.Type > 0)
            {
                wheresql = wheresql + " AND Type = " + cp.Type ;
            }
            if(cp.OrdType > 0)
            {
                wheresql = wheresql + " AND OrdType = " + cp.OrdType ;
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
            if(cp.RefundStatus != null)
            {
                string shopstatus = string.Empty;
                foreach(var x in cp.RefundStatus)
                {
                    shopstatus = shopstatus + "'" + x + "',";
                }
                shopstatus = shopstatus.Substring(0,shopstatus.Length - 1);
                wheresql = wheresql + " AND RefundStatus in (" +  shopstatus + ")";
            }
            if(cp.Distributor > 0)
            {
                wheresql = wheresql + " AND Distributor = " + cp.Distributor ;
            }
            if(!string.IsNullOrEmpty(cp.IsSubmit) && (cp.IsSubmit.ToUpper() == "Y" || cp.IsSubmit.ToUpper() == "N"))
            {
                wheresql = wheresql + " AND IsSubmit = '" + cp.IsSubmit.ToUpper() + "'" ;
            }
            if(cp.IssueType > 0)
            {
                wheresql = wheresql + " AND IssueType = " + cp.IssueType ;
            }
            if(cp.Result > 0)
            {
                wheresql = wheresql + " AND Result = " + cp.Result ;
            }
            if(cp.Result == -2)
            {
                wheresql = wheresql + " AND Result = null";
            }
            if(!string.IsNullOrEmpty(cp.SortField) && !string.IsNullOrEmpty(cp.SortDirection))//排序
            {
                wheresql = wheresql + " ORDER BY "+cp.SortField +" "+ cp.SortDirection;
            }
            var res = new AfterSaleData();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    int count = conn.QueryFirst<int>(sqlcount + wheresql);
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(cp.NumPerPage.ToString()));
                    int dataindex = (cp.PageIndex - 1)* cp.NumPerPage;
                    wheresql = wheresql + " limit " + dataindex.ToString() + " ," + cp.NumPerPage.ToString();
                    var u = conn.Query<AfterSaleQuery>(sqlcommand + wheresql).AsList();
                    foreach(var a in u)
                    {
                        a.TypeString = Enum.GetName(typeof(ASType), a.Type);
                        a.IssueTypeString = Enum.GetName(typeof(IssueType), a.IssueType);
                        a.OrdTypeString = OrderHaddle.GetTypeName(a.OrdType);
                        a.StatusString = Enum.GetName(typeof(ASStatus), a.Status);
                        a.ResultString = Enum.GetName(typeof(Result), a.Result);
                    }
                    res.Datacnt = count;
                    res.Pagecnt = pagecnt;
                    res.AfterSale = u;
                    
                         
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }        
            //售后类型
            var ff = new List<Filter>();
            foreach (int  myCode in Enum.GetValues(typeof(ASType)))
            {
                var f = new Filter();
                f.value = myCode.ToString();
                f.label = Enum.GetName(typeof(ASType), myCode);//获取名称
                ff.Add(f);
            }
            res.Type = ff; 
            //处理结果
            ff = new List<Filter>();
            foreach (int  myCode in Enum.GetValues(typeof(Result)))
            {
                var f = new Filter();
                f.value = myCode.ToString();
                f.label = Enum.GetName(typeof(Result), myCode);//获取名称
                ff.Add(f);
            }
            res.Result = ff;
            //仓库资料
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{    
                    wheresql = "select ID as value,WarehouseName as label from warehouse where ParentID > 0 and Enable = true and coid = " + cp.CoID;
                    var u = conn.Query<Filter>(wheresql).AsList();
                    res.Warehouse = u;                                    
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            result.d = res;          
            return result;
        }
        ///<summary>
        ///售后订单List查询
        ///</summary>
        public static DataResult GetASOrderList(ASOrderParm cp)
        {
            var result = new DataResult(1,null);    
            string sqlcount = "select count(id) from `order` where 1=1";
            string sqlcommand = @"select ID,ODate,BuyerShopID,ShopName,SoID,Amount,PaidAmount,ExAmount,Type,RecName,RecMessage,SendMessage,RecTel,RecPhone,RecLogistics,
                                  RecCity,RecDistrict,RecAddress,Express,ExCode from `order` where 1=1"; 
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
            if (cp.Status > 0)//状态List
            {
                wheresql = wheresql + " AND status = " + cp.Status ;
            }
            if(cp.DateStart > DateTime.Parse("1900-01-01"))
            {
                wheresql = wheresql + " AND odate >= '" + cp.DateStart + "'" ;
            }
            if(cp.DateEnd > DateTime.Parse("1900-01-01"))
            {
                wheresql = wheresql + " AND odate <= '" + cp.DateEnd + "'" ;
            }
            if(!string.IsNullOrEmpty(cp.Distributor))
            {
                wheresql = wheresql + " AND distributor = '" +  cp.Distributor + "'";
            }
            if(cp.ExID > 0)
            {
                wheresql = wheresql + " AND exid = "+ cp.ExID  ;
            }
            if(cp.SendWarehouse > 0)
            {
                wheresql = wheresql + " AND WarehouseID = "+ cp.SendWarehouse ;
            }
            if(cp.ShopID >= 0)
            {
                wheresql = wheresql + " AND ShopID = "+ cp.ShopID  ;
            }
            if(!string.IsNullOrEmpty(cp.SortField) && !string.IsNullOrEmpty(cp.SortDirection))//排序
            {
                wheresql = wheresql + " ORDER BY "+cp.SortField +" "+ cp.SortDirection;
            }
            var res = new ASOrderData();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    int count = conn.QueryFirst<int>(sqlcount + wheresql);
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(cp.NumPerPage.ToString()));
                    int dataindex = (cp.PageIndex - 1)* cp.NumPerPage;
                    wheresql = wheresql + " limit " + dataindex.ToString() + " ," + cp.NumPerPage.ToString();
                    var u = conn.Query<ASOrderQuery>(sqlcommand + wheresql).AsList();
                    var ItemID = new List<int>();
                    foreach(var a in u)
                    {
                        a.TypeString = OrderHaddle.GetTypeName(a.Type);
                        ItemID.Add(a.ID);
                    }
                    sqlcommand = @"select id,oid,SkuAutoID,Img,Qty,GoodsCode,SkuID,SkuName,Norm,RealPrice,Amount,ShopSkuID,IsGift,Weight from orderitem 
                                    where oid in @ID and coid = @Coid";
                    var item = conn.Query<SkuList>(sqlcommand,new{ID = ItemID,Coid = cp.CoID}).AsList();
                    foreach(var a in u)
                    {
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
        ///售后订单初始资料
        ///</summary>
        public static DataResult InsertASInit(string Type,int CoID)
        {
            var result = new DataResult(1,null);
            var res = new ASOrderInit();   
            if(Type == "A")
            {
                var cp = new ASOrderParm();
                cp.CoID = CoID;
                var re = GetASOrderList(cp);
                if(re.s == 1)
                {
                    res.Order = re.d as ASOrderData;
                }
                //获取店铺List
                var shop = CoreComm.ShopHaddle.getShopEnum(CoID.ToString()) as List<shopEnum>;
                var ff = new List<Filter>();
                foreach(var t in shop)
                {
                    var f = new Filter();
                    f.value = t.value.ToString();
                    f.label = t.label;
                    ff.Add(f);
                }
                var fd = new Filter();
                fd.value = "0";
                fd.label = "{线下}";
                ff.Add(fd);
                res.Shop = ff;  
                //快递Lsit
                var Express = CoreComm.ExpressHaddle.GetExpressSimple(CoID).d as List<ExpressSimple>;
                ff = new List<Filter>();
                foreach(var t in Express)
                {
                    var f = new Filter();
                    f.value = t.ID;
                    f.label = t.Name;
                    ff.Add(f);
                }
                res.Express = ff;  
                //分销商
                using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{  
                        string sqlcommand = "select ID  as value,DistributorName as label from distributor where coid =" + CoID + " and enable = true and type = 0";
                        var Distributor = conn.Query<Filter>(sqlcommand).AsList();
                        res.Distributor = Distributor;
                        
                        }catch(Exception ex){
                        result.s = -1;
                        result.d = ex.Message;
                        conn.Dispose();
                    }
                }  
                //仓库资料
                List<Filter> wh = new List<Filter>();
                var w = CoreComm.WarehouseHaddle.getWarelist(CoID.ToString()) as List<wareLst>;
                foreach(var h in w)
                {
                    var a = new Filter();
                    a.value = h.id.ToString();
                    a.label = h.warename;
                    wh.Add(a);
                }
                res.SendWarehouse = wh ;
            }
            //售后类型
            var filter = new List<Filter>();
            foreach (int  myCode in Enum.GetValues(typeof(ASType)))
            {
                var f = new Filter();
                f.value = myCode.ToString();
                f.label = Enum.GetName(typeof(ASType), myCode);//获取名称
                filter.Add(f);
            }
            res.Type = filter;
            //问题类型
            filter = new List<Filter>();
            foreach (int  myCode in Enum.GetValues(typeof(IssueType)))
            {
                var f = new Filter();
                f.value = myCode.ToString();
                f.label = Enum.GetName(typeof(IssueType), myCode);//获取名称
                filter.Add(f);
            }
            res.IssueType = filter;
            //仓库资料
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{    
                    string wheresql = "select ID as value,WarehouseName as label from warehouse where ParentID > 0 and Enable = true and coid = " + CoID;
                    var u = conn.Query<Filter>(wheresql).AsList();
                    res.Warehouse = u;    
                    wheresql = "select ID from warehouse where ParentID > 0 and Enable = true and type = 3 and coid = " + CoID;           
                    int a = conn.QueryFirst<int>(wheresql);        
                    res.DefaultWare = a;             
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            result.d = res;
            return result;  
        }
        ///<summary>
        ///新增售后单
        ///</summary>
        public static DataResult InsertAfterSale(string DuType,int CoID,int Type,decimal SalerReturnAmt,decimal BuyerUpAmt,int WarehouseID,int IssueType,string Remark,
                                                 string UserName,string Express,string ExCode,int OID)
        {
            var result = new DataResult(1,null);
            int count = 0;
            var log = new LogInsert();
            var afterAsale = new AfterSale();
            string WarehouseName = "";
            string sqlcommand = string.Empty;
            if(WarehouseID > 0)
            {   
                using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                    try{    
                        string wheresql = "select WarehouseName from warehouse where ID =" + WarehouseID + " and enable = true and coid = " + CoID;
                        var u = conn.Query<string>(wheresql).AsList();
                        if(u.Count == 0)
                        {
                            result.s = -1;
                            result.d = "仓库ID无效";
                            return result;
                        }
                        else
                        {
                            WarehouseName = u[0];
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
                afterAsale.RegisterDate = DateTime.Now;
                afterAsale.Type = Type;
                afterAsale.SalerReturnAmt = SalerReturnAmt;
                afterAsale.BuyerUpAmt = BuyerUpAmt;
                afterAsale.RealReturnAmt = SalerReturnAmt - BuyerUpAmt;
                if(WarehouseID > 0)
                {
                    afterAsale.WarehouseID = WarehouseID;
                    afterAsale.RecWarehouse = WarehouseName;
                }
                afterAsale.IssueType = IssueType;
                afterAsale.Remark = Remark;
                afterAsale.Status = 0;
                afterAsale.RefundStatus = "未申请状态";
                afterAsale.IsSubmit = false;
                afterAsale.IsSubmitDis = false;
                afterAsale.IsInterfaceLoad = false;
                afterAsale.CoID = CoID;
                afterAsale.Creator = UserName;
                afterAsale.Modifier = UserName;
                afterAsale.Express = Express;
                afterAsale.ExCode = ExCode;
                afterAsale.OID = OID;
                if(DuType == "A")
                {
                    sqlcommand = "select SoID,BuyerShopID,RecName,RecTel,RecPhone,ShopID,ShopName,Type,Distributor from `order` where id = " + OID + " and coid = " + CoID;
                    var u = CoreDBconn.Query<Order>(sqlcommand).AsList();
                    if(u.Count == 0)
                    {
                        result.s = -1;
                        result.d = "订单ID无效";
                        return result;
                    }
                    afterAsale.SoID = u[0].SoID;
                    afterAsale.BuyerShopID = u[0].BuyerShopID;
                    afterAsale.RecName = u[0].RecName;
                    afterAsale.RecTel = u[0].RecTel;
                    afterAsale.RecPhone = u[0].RecPhone;
                    afterAsale.ShopID = u[0].ShopID;
                    afterAsale.ShopName = u[0].ShopName;
                    afterAsale.OrdType = u[0].Type;
                    afterAsale.Distributor = u[0].Distributor;
                }                
                if(DuType == "A")
                {
                    sqlcommand = @"INSERT INTO aftersale(OID,SoID,RegisterDate,BuyerShopID,RecName,Type,RecTel,RecPhone,SalerReturnAmt,BuyerUpAmt,RealReturnAmt,ShopID,ShopName,
                                                         WarehouseID,RecWarehouse,IssueType,OrdType,Remark,Status,RefundStatus,Express,ExCode,IsSubmit,IsSubmitDis,IsInterfaceLoad,
                                                         Distributor,CoID,Creator,Modifier) 
                                                  VALUES(@OID,@SoID,@RegisterDate,@BuyerShopID,@RecName,@Type,@RecTel,@RecPhone,@SalerReturnAmt,@BuyerUpAmt,@RealReturnAmt,@ShopID,@ShopName,
                                                         @WarehouseID,@RecWarehouse,@IssueType,@OrdType,@Remark,@Status,@RefundStatus,@Express,@ExCode,@IsSubmit,@IsSubmitDis,@IsInterfaceLoad,
                                                         @Distributor,@CoID,@Creator,@Modifier)";
                }
                if(DuType == "B")
                {
                    sqlcommand = @"INSERT INTO aftersale(OID,RegisterDate,Type,SalerReturnAmt,BuyerUpAmt,RealReturnAmt,WarehouseID,RecWarehouse,IssueType,Remark,Status,RefundStatus,
                                                         Express,ExCode,IsSubmit,IsSubmitDis,IsInterfaceLoad,CoID,Creator,Modifier) 
                                                  VALUES(@OID,@RegisterDate,@Type,@SalerReturnAmt,@BuyerUpAmt,@RealReturnAmt,@WarehouseID,@RecWarehouse,@IssueType,@Remark,@Status,@RefundStatus,
                                                         @Express,@ExCode,@IsSubmit,@IsSubmitDis,@IsInterfaceLoad,@CoID,@Creator,@Modifier)";
                }
                count = CoreDBconn.Execute(sqlcommand,afterAsale,TransCore);
                int rtn = 0;
                if(count <= 0)
                {
                    result.s = -3002;
                    return result;
                }
                else
                {
                    rtn = CoreDBconn.QueryFirst<int>("select LAST_INSERT_ID()");
                    result.d = rtn;
                }
                log.OID = rtn;
                log.Type = 1;
                log.LogDate = DateTime.Now;
                log.UserName = UserName;
                log.Title = "售后时间";
                log.Remark = "手工新增售后时间";
                log.CoID = CoID;
                string loginsert = @"INSERT INTO orderlog(OID,Type,LogDate,UserName,Title,Remark,CoID) 
                                                   VALUES(@OID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                count =CoreDBconn.Execute(loginsert,log,TransCore);
                if(count < 0)
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
        ///修改售后单
        ///</summary>
        public static DataResult UpdateAfterSale(int CoID,int Type,decimal SalerReturnAmt,decimal BuyerUpAmt,string ReturnAccount,int WarehouseID,string Remark,
                                                 string UserName,string Express,string ExCode,int Result,int RID)
        {
            var result = new DataResult(1,null);
            int count = 0;
            var logs = new List<LogInsert>();
            var log = new LogInsert();
            string WarehouseName = "";
            string sqlcommand = string.Empty;
            if(WarehouseID > 0)
            {   
                using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                    try{    
                        string wheresql = "select WarehouseName from warehouse where ID =" + WarehouseID + " and enable = true and coid = " + CoID;
                        var u = conn.Query<string>(wheresql).AsList();
                        if(u.Count == 0)
                        {
                            result.s = -1;
                            result.d = "仓库ID无效";
                            return result;
                        }
                        else
                        {
                            WarehouseName = u[0];
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
                sqlcommand = "select * from aftersale where id =" + RID + " and coid = " + CoID;
                var u = CoreDBconn.Query<AfterSale>(sqlcommand).AsList();
                if(u.Count == 0)
                {
                    result.s = -1;
                    result.d = "售后ID无效!";
                    return result;
                }
                else
                {
                    if(u[0].Status != 0)
                    {
                        result.s = -1;
                        result.d = "只有待确认的售后单才可以修改!";
                        return result;
                    }
                }
                int i = 0;
                if(SalerReturnAmt >= 0 && u[0].SalerReturnAmt != SalerReturnAmt)
                {
                    log = new LogInsert();
                    log.OID = RID;
                    log.Type = 1;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "修改卖家应退金额";
                    log.Remark = u[0].SalerReturnAmt + "=>" + SalerReturnAmt;
                    log.CoID = CoID;
                    logs.Add(log);
                    u[0].SalerReturnAmt = SalerReturnAmt;
                    i ++;
                }
                if(BuyerUpAmt >= 0  && u[0].BuyerUpAmt != BuyerUpAmt)
                {
                    log = new LogInsert();
                    log.OID = RID;
                    log.Type = 1;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "修改买家应补金额";
                    log.Remark = u[0].BuyerUpAmt + "=>" + BuyerUpAmt;
                    log.CoID = CoID;
                    logs.Add(log);
                    u[0].BuyerUpAmt = BuyerUpAmt;
                    i ++;
                }
                if(i > 0)
                {
                    u[0].RealReturnAmt = u[0].SalerReturnAmt - u[0].BuyerUpAmt;
                }
                if(Type >= 0  && u[0].Type != Type)
                {
                    log = new LogInsert();
                    log.OID = RID;
                    log.Type = 1;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "修改售后分类";
                    log.Remark = Enum.GetName(typeof(ASType), u[0].Type) + "=>" + Enum.GetName(typeof(ASType), Type);
                    log.CoID = CoID;
                    logs.Add(log);
                    u[0].Type = Type;
                    i ++;
                    sqlcommand = "delete from aftersaleitem where RID = " + RID + " and coid = " + CoID;
                    count = CoreDBconn.Execute(sqlcommand,TransCore);
                    if(count < 0)
                    {
                        result.s = -3004;
                        return result;
                    }
                    if(Type == 1 && u[0].OID > 0)
                    {
                        sqlcommand = "select * from orderitem where oid = " + u[0].OID + " and coid = " + CoID;
                        var item = CoreDBconn.Query<OrderItem>(sqlcommand).AsList();
                        foreach(var it in item)
                        {
                            sqlcommand = @"INSERT INTO aftersaleitem(RID,ReturnType,SkuAutoID,SkuID,SkuName,Norm,GoodsCode,RegisterQty,Price,Amount,Img,CoID,Creator,Modifier) 
                                           VALUES(@RID,@ReturnType,@SkuAutoID,@SkuID,@SkuName,@Norm,@GoodsCode,@RegisterQty,@Price,@Amount,@Img,@CoID,@Creator,@Creator)";
                            var args = new{RID = RID,ReturnType = 0,SkuAutoID=it.SkuAutoID,SkuID = it.SkuID,SkuName=it.SkuName,Norm=it.Norm,GoodsCode=it.GoodsCode,RegisterQty=it.Qty,
                                           Price=it.RealPrice,Amount=it.Amount,Img=it.img,CoID=CoID,Creator=UserName};
                            count = CoreDBconn.Execute(sqlcommand,args,TransCore);
                            if(count < 0)
                            {
                                result.s = -3002;
                                return result;
                            }
                        }
                    }
                }
                if(ReturnAccount != null  && u[0].ReturnAccount != ReturnAccount)
                {
                    log = new LogInsert();
                    log.OID = RID;
                    log.Type = 1;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "修改退款账号";
                    log.Remark = u[0].ReturnAccount + "=>" + ReturnAccount;
                    log.CoID = CoID;
                    logs.Add(log);
                    u[0].ReturnAccount = ReturnAccount;
                    i ++;
                }
                if(WarehouseID > 0  && u[0].WarehouseID != WarehouseID)
                {
                    log = new LogInsert();
                    log.OID = RID;
                    log.Type = 1;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "修改退货仓库";
                    log.Remark = u[0].RecWarehouse + "=>" + WarehouseName;
                    log.CoID = CoID;
                    logs.Add(log);
                    u[0].WarehouseID = WarehouseID;
                    u[0].RecWarehouse = WarehouseName;
                    i ++;
                }
                if(Remark != null  && u[0].Remark != Remark)
                {
                    log = new LogInsert();
                    log.OID = RID;
                    log.Type = 1;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "修改备注";
                    log.Remark = u[0].Remark + "=>" + Remark;
                    log.CoID = CoID;
                    logs.Add(log);
                    u[0].Remark = Remark;
                    i ++;
                }
                if(Express != null  && u[0].Express != Express)
                {
                    log = new LogInsert();
                    log.OID = RID;
                    log.Type = 1;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "修改快递公司";
                    log.Remark = u[0].Express + "=>" + Express;
                    log.CoID = CoID;
                    logs.Add(log);
                    u[0].Express = Express;
                    i ++;
                }
                if(ExCode != null  && u[0].ExCode != ExCode)
                {
                    log = new LogInsert();
                    log.OID = RID;
                    log.Type = 1;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "修改运单号";
                    log.Remark = u[0].ExCode + "=>" + ExCode;
                    log.CoID = CoID;
                    logs.Add(log);
                    u[0].ExCode = ExCode;
                    i ++;
                }
                if(Result >= 0  && u[0].Result != Result)
                {
                    log = new LogInsert();
                    log.OID = RID;
                    log.Type = 1;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "修改处理结果";
                    log.Remark = Enum.GetName(typeof(Result), u[0].Result) + "=>" + Enum.GetName(typeof(Result), Result);
                    log.CoID = CoID;
                    logs.Add(log);
                    u[0].Result = Result;
                    i ++;
                }
                if(i > 0)
                {
                    u[0].Modifier = UserName;
                    u[0].ModifyDate = DateTime.Now;                
                    sqlcommand = @"update aftersale set SalerReturnAmt=@SalerReturnAmt,BuyerUpAmt=@BuyerUpAmt,RealReturnAmt=@RealReturnAmt,Type=@Type,ReturnAccount=@ReturnAccount,
                                   WarehouseID=@WarehouseID,RecWarehouse=@RecWarehouse,Remark=@Remark,Express=@Express,ExCode=@ExCode,Result=@Result,Modifier=@Modifier,
                                   ModifyDate=@ModifyDate where id = @ID and coid = @Coid";
                    count = CoreDBconn.Execute(sqlcommand,u[0],TransCore);
                    if(count <= 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                    string loginsert = @"INSERT INTO orderlog(OID,Type,LogDate,UserName,Title,Remark,CoID) 
                                                    VALUES(@OID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                    count =CoreDBconn.Execute(loginsert,logs,TransCore);
                    if(count < 0)
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