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
                    foreach(var a in u)
                    {
                        a.TypeString = OrderHaddle.GetTypeName(a.Type);
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
        ///根据OID查询订单明细
        ///</summary>
        public static DataResult GetASOrderItemS(int OID,int CoID)
        {
            var result = new DataResult(1,null);    
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    string sqlcommand = @"select id,oid,SkuAutoID,Img,Qty,GoodsCode,SkuID,SkuName,Norm,RealPrice,Amount,ShopSkuID,IsGift,Weight from orderitem 
                                    where oid = @ID and coid = @Coid";
                    var item = conn.Query<SkuList>(sqlcommand,new{ID = OID,Coid = CoID}).AsList();
                    result.d = item;             
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
        public static DataResult InsertASInit(int CoID)
        {
            var result = new DataResult(1,null);
            var res = new ASOrderInit();   
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
        ///<summary>
        ///根据订单带出售后可退货明细
        ///</summary>
        public static DataResult GetASOrdItem(int CoID,int RID)
        {
            var result = new DataResult(1,null);
            string sqlcommand = string.Empty;   
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    sqlcommand = "select * from aftersale where id = " + RID + " and coid = " + CoID;
                    var u = conn.Query<AfterSale>(sqlcommand).AsList();
                    if(u.Count == 0)
                    {
                        result.s = -1;
                        result.d = "售后ID无效!";
                        return result;
                    }
                    if(u[0].OID == -1)
                    {
                        result.s = -1;
                        result.d = "无信息件不能从订单新增退货明细!";
                        return result;
                    }
                    if(u[0].Type != 0 && u[0].Type != 2)
                    {
                        result.s = -1;
                        result.d = "普通退货/换货的售后单才可以新增退回商品!";
                        return result;
                    }
                    if(u[0].Status != 0)
                    {
                        result.s = -1;
                        result.d = "待确认的售后单才可以新增明细!";
                        return result;
                    }
                    sqlcommand = "select ID,SkuID,SkuName,Norm,Qty,SalePrice,RealPrice,Amount,DiscountRate,img,IsGift from orderitem where oid =" + u[0].OID + " and coid = " + CoID;
                    var item = conn.Query<ASOrderItem>(sqlcommand).AsList();
                    result.d = item;
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }         
            return result;
        }
        ///<summary>
        ///从订单新增退货/补发明细
        ///</summary>
        public static DataResult InsertASItemOrder(int CoID,string UserName,int RID,List<int> ODetailID)
        {
            var result = new DataResult(1,null);
            int count = 0;
            var logs = new List<LogInsert>();
            var log = new LogInsert();
            int ReturnType = 0;
            string sqlcommand = string.Empty;
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
                if(u[0].OID == -1)
                {
                    result.s = -1;
                    result.d = "无信息件不能从订单新增退货明细!";
                    return result;
                }
                if(u[0].Type != 0 && u[0].Type != 2 && u[0].Type != 3)
                {
                    result.s = -1;
                    result.d = "普通退货/换货/补发的售后单才可以新增退回商品!";
                    return result;
                }
                if(u[0].Status != 0)
                {
                    result.s = -1;
                    result.d = "待确认的售后单才可以新增明细!";
                    return result;
                }
                if(u[0].Type == 0 || u[0].Type == 2)
                {   
                    ReturnType = 0;
                }
                if(u[0].Type == 3)
                {
                    ReturnType = 2;
                }
                sqlcommand = "select count(id) from orderitem where oid = @OID and coid = @Coid and id in @ID";
                int c = CoreDBconn.QueryFirst<int>(sqlcommand,new{OID=u[0].OID,Coid = CoID,ID = ODetailID});
                if(c != ODetailID.Count || c == 0)
                {
                    result.s = -1;
                    result.d = "订单明细ID和订单ID不匹配,请选择正确的订单明细";
                    return result;
                }
                sqlcommand = "select * from orderitem where oid = @OID and coid = @Coid and id in @ID";
                var item = CoreDBconn.Query<OrderItem>(sqlcommand,new{OID=u[0].OID,Coid = CoID,ID = ODetailID});
                var ASitem = new List<AfterSaleItem>();
                foreach(var it in item)
                {
                    var i = new AfterSaleItem();
                    i.RID = RID;
                    i.ReturnType = ReturnType;
                    i.SkuAutoID = it.SkuAutoID;
                    i.SkuID = it.SkuID;
                    i.SkuName = it.SkuName;
                    i.Norm = it.Norm;
                    i.GoodsCode = it.GoodsCode;
                    i.RegisterQty = it.Qty;
                    if(ReturnType == 0)
                    {
                        i.Price = decimal.Parse(it.RealPrice);
                        i.Amount = decimal.Parse(it.Amount);
                    }
                    else
                    {
                        i.Price = 0;
                        i.Amount = 0;
                    }
                    i.img = it.img;
                    i.CoID = CoID;
                    i.Creator = UserName;
                    i.Modifier = UserName;
                    ASitem.Add(i);
                    log = new LogInsert();
                    log.OID = RID;
                    log.Type = 1;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "新增" + Enum.GetName(typeof(ReturnType), ReturnType) + "商品";
                    log.Remark = i.SkuID;
                    log.CoID = CoID;
                    logs.Add(log);
                }
                sqlcommand = @"INSERT INTO aftersaleitem(RID,ReturnType,SkuAutoID,SkuID,SkuName,Norm,GoodsCode,RegisterQty,Price,Amount,Img,CoID,Creator,Modifier) 
                                           VALUES(@RID,@ReturnType,@SkuAutoID,@SkuID,@SkuName,@Norm,@GoodsCode,@RegisterQty,@Price,@Amount,@Img,@CoID,@Creator,@Creator)";
                count = CoreDBconn.Execute(sqlcommand,ASitem,TransCore);
                if(count < 0)
                {
                    result.s = -3002;
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
        ///根据售后ID返回售后明细
        ///</summary>
        public static DataResult GetAfterSaleItem(int CoID,int RID)
        {
            var result = new DataResult(1,null);
            var res = new GetAfterSaleItemReturn();
            string sqlcommand = string.Empty;   
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    sqlcommand = "select status,type,oid from aftersale where id = " + RID + " and coid = " + CoID;
                    var sa = conn.Query<AfterSale>(sqlcommand).AsList();
                    if(sa.Count > 0)
                    {
                        res.Status = sa[0].Status;
                        res.Type = sa[0].Type;
                        res.OID = sa[0].OID;
                    }
                    sqlcommand = "select ID,ReturnType,SkuAutoID,SkuID,SkuName,Norm,GoodsCode,RegisterQty,ReturnQty,Price,Amount,img,Creator from aftersaleitem where rid = " + RID + 
                                 " and coid = " + CoID;
                    var u = conn.Query<AfterSaleItemQuery>(sqlcommand).AsList();
                    foreach(var a in u)
                    {
                        a.ReturnTypeString = Enum.GetName(typeof(ReturnType), a.ReturnType);//获取名称
                    }
                    res.AfterSaleItem = u;
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
        ///从商品库新增明细
        ///</summary>
        public static DataResult InsertASItemSku(int CoID,string UserName,int RID,List<int> SkuID)
        {
            var result = new DataResult(1,null);
            int count = 0;
            var logs = new List<LogInsert>();
            var log = new LogInsert();
            var rt = new List<InsertFailReason>();
            int ReturnType = 0;
            string sqlcommand = string.Empty;
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
                if(u[0].Type != 0 && u[0].Type != 2 && u[0].Type != 3)
                {
                    result.s = -1;
                    result.d = "普通退货/换货/补发的售后单才可以新增商品库商品!";
                    return result;
                }
                if(u[0].Status != 0)
                {
                    result.s = -1;
                    result.d = "待确认的售后单才可以新增明细!";
                    return result;
                }
                if(u[0].Type == 0)
                {   
                    ReturnType = 0;
                }
                if(u[0].Type == 2)
                {   
                    ReturnType = 1;
                }
                if(u[0].Type == 3)
                {
                    ReturnType = 2;
                }
                foreach (int a in SkuID)
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
                    sqlcommand = @"INSERT INTO aftersaleitem(RID,ReturnType,SkuAutoID,SkuID,SkuName,Norm,GoodsCode,RegisterQty,Price,Amount,Img,CoID,Creator,Modifier) 
                                           VALUES(@RID,@ReturnType,@SkuAutoID,@SkuID,@SkuName,@Norm,@GoodsCode,@RegisterQty,@Price,@Price,@Img,@CoID,@Creator,@Creator)";
                    decimal Price = decimal.Parse(s[0].saleprice);
                    if(ReturnType == 2)
                    {
                        Price = 0;
                    }
                    var args = new
                    {
                        RID = RID,
                        ReturnType = ReturnType,
                        SkuAutoID = a,
                        SkuID = s[0].skuid,
                        SkuName = s[0].skuname,
                        Norm = s[0].norm,
                        GoodsCode = s[0].goodscode,
                        RegisterQty = 1,
                        Price = Price,
                        Img = s[0].img,
                        Coid = CoID,
                        Creator = UserName
                    };
                    count = CoreDBconn.Execute(sqlcommand, args, TransCore);
                    if (count <= 0)
                    {
                        rf.id = a;
                        rf.reason = "新增明细失败!";
                        rt.Add(rf);
                    }
                    else
                    {
                        log = new LogInsert();
                        log.OID = RID;
                        log.Type = 1;
                        log.LogDate = DateTime.Now;
                        log.UserName = UserName;
                        log.Title = "新增退货商品";
                        log.Remark = s[0].skuid;
                        log.CoID = CoID;
                        logs.Add(log);
                    }
                }
                string loginsert = @"INSERT INTO orderlog(OID,Type,LogDate,UserName,Title,Remark,CoID) 
                                                    VALUES(@OID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                count =CoreDBconn.Execute(loginsert,logs,TransCore);
                if(count < 0)
                {
                    result.s = -3002;
                    return result;
                }
                result.d = rt;
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
        ///更新售后明细
        ///</summary>
        public static DataResult UpdateASItem(int CoID,string UserName,int RID,int RDetailID,int Qty,decimal Amount)
        {
            var result = new DataResult(1,null);
            int count = 0;
            var logs = new List<LogInsert>();
            var log = new LogInsert();
            string sqlcommand = string.Empty;
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
                if(u[0].Type != 0 && u[0].Type != 1 && u[0].Type != 2 && u[0].Type != 3)
                {
                    result.s = -1;
                    result.d = "普通退货/拒收退货/换货/补发的售后单才可以修改明细!";
                    return result;
                }
                if(u[0].Status != 0)
                {
                    result.s = -1;
                    result.d = "待确认的售后单才可以修改明细!";
                    return result;
                }
                sqlcommand = "select * from aftersaleitem where id =" + RDetailID + " and coid = " + CoID + " and rid = " + RID; 
                var item = CoreDBconn.Query<AfterSaleItem>(sqlcommand).AsList();
                if(item.Count == 0)
                {
                    result.s = -1;
                    result.d = "售后明细ID无效!";
                    return result;
                }
                int i = 0;
                if(Qty != -1 &&　item[0].RegisterQty != Qty)
                {
                    i ++;
                    log = new LogInsert();
                    log.OID = RID;
                    log.Type = 1;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "修改退货数量";
                    log.Remark = item[0].RegisterQty + "=>" + Qty;
                    log.CoID = CoID;
                    logs.Add(log);
                    item[0].RegisterQty = Qty;
                    item[0].Amount = item[0].Price * Qty;
                }
                if(Amount != -1 &&　item[0].Amount != Amount)
                {
                    i ++;
                    log = new LogInsert();
                    log.OID = RID;
                    log.Type = 1;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "修改金额";
                    log.Remark = item[0].Amount + "=>" + Amount;
                    log.CoID = CoID;
                    logs.Add(log);
                    item[0].Amount = Amount;
                    item[0].Price = Amount/item[0].RegisterQty;
                }
                if(i > 0)
                {
                    item[0].Modifier = UserName;
                    item[0].ModifyDate = DateTime.Now;
                    
                    sqlcommand = @"update aftersaleitem set RegisterQty=@RegisterQty,Amount=@Amount,Price=@Price,Modifier=@Modifier,ModifyDate=@ModifyDate where ID = @ID and coid = @Coid";
                    count = CoreDBconn.Execute(sqlcommand, item[0], TransCore);
                    if(count < 0)
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
        ///<summary>
        ///删除售后明细
        ///</summary>
        public static DataResult DeleteASItem(int CoID,string UserName,int RID,List<int> RDetailID)
        {
            var result = new DataResult(1,null);
            int count = 0;
            var logs = new List<LogInsert>();
            var log = new LogInsert();
            string sqlcommand = string.Empty;
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
                if(u[0].Type != 0 && u[0].Type != 2 && u[0].Type != 3)
                {
                    result.s = -1;
                    result.d = "普通退货/换货/补发的售后单才可以删除明细!";
                    return result;
                }
                if(u[0].Status != 0)
                {
                    result.s = -1;
                    result.d = "待确认的售后单才可以删除明细!";
                    return result;
                }
                sqlcommand = "select * from aftersaleitem where id in @ID and coid = @Coid and rid = @RID"; 
                var item = CoreDBconn.Query<AfterSaleItem>(sqlcommand,new{ID = RDetailID,Coid = CoID,RID = RID}).AsList();
                if(item.Count == 0)
                {
                    result.s = -1;
                    result.d = "售后明细ID无效!";
                    return result;
                }
                foreach(var a in item)
                {
                    log = new LogInsert();
                    log.OID = RID;
                    log.Type = 1;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "删除售后明细";
                    log.Remark = a.SkuID;
                    log.CoID = CoID;
                    logs.Add(log);
                }
                                
                sqlcommand = @"delete from aftersaleitem where id in @ID and coid = @Coid and rid = @RID";
                count = CoreDBconn.Execute(sqlcommand, new{ID = RDetailID,Coid = CoID,RID = RID}, TransCore);
                if(count < 0)
                {
                    result.s = -3004;
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
        ///抓取售后单的日志
        ///</summary>
        public static DataResult GetOrderLog(int rid,int CoID)
        {
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{  
                        string sqlcommand = @"select ID,LogDate,UserName,Title,Remark From orderlog where
                                            oid = @ID and coid = @Coid and type = 1 order by LogDate Desc";
                        var Log = conn.Query<OrderLog>(sqlcommand,new{ID=rid,Coid=CoID}).AsList();
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
        ///抓取单笔售后资料
        ///</summary>
        public static DataResult GetAfterSaleSingle(int rid,int CoID)
        {
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{  
                        string sqlcommand = @"select ID,OID,ShopName,Status,GoodsStatus,SoID,BuyerShopID,RecName,RecTel,RecPhone,Type,IssueType,RegisterDate,SalerReturnAmt,BuyerUpAmt,
                                              RealReturnAmt,ReturnAccount,WarehouseID,RecWarehouse,Express,ExCode,Remark From aftersale where id = " + rid + " and coid = " + CoID;
                        var u = conn.Query<AfterSaleEdit>(sqlcommand).AsList();
                        u[0].TypeString = Enum.GetName(typeof(ASType), u[0].Type);
                        u[0].IssueTypeString = Enum.GetName(typeof(IssueType), u[0].IssueType);
                        u[0].StatusString = Enum.GetName(typeof(ASStatus), u[0].Status);
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
        ///抓取售后详情的资料
        ///</summary>
        public static DataResult GetAfterSaleEdit(int rid,int CoID)
        {
            var result = new DataResult(1,null);
            var res = new AfterSaleEditReturn();
            res.AfterSale = GetAfterSaleSingle(rid,CoID).d as AfterSaleEdit;
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
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            res.AfterSaleItem = GetAfterSaleItem(CoID,rid).d as List<AfterSaleItemQuery>;
            res.Log = GetOrderLog(rid,CoID).d as List<OrderLog>;
            result.d = res;
            return result;
        }
        ///<summary>
        ///绑定订单
        ///</summary>
        public static DataResult BindOrd(int RID,int OID,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            int count = 0;
            var logs = new List<LogInsert>();
            var log = new LogInsert();
            // var res = new BindOrdReturn();
            // var fa = new List<InsertFailReason>();
            // var sID = new List<int>();
            string sqlcommand = string.Empty;
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                sqlcommand = "select * from `order` where id = " + OID + " and coid = " + CoID;
                var order = CoreDBconn.Query<Order>(sqlcommand).AsList();
                if(order.Count == 0)
                {
                    result.s = -1;
                    result.d = "订单单号无效";
                    return result;
                }
                sqlcommand = "select * from aftersale where id in @ID and coid = @CoID";
                var u = CoreDBconn.Query<AfterSale>(sqlcommand,new{ID = RID,CoID = CoID}).AsList();
                if(u.Count == 0)
                {
                    result.s = -1;
                    result.d = "售后单号无效";
                    return result;
                }
                var a = u[0] as AfterSale;
                if(a.Status != 0)
                {
                    result.s = -1;
                    result.d = "待确认的单据才可以绑定订单";
                    return result;
                }
                if(a.OID != -1)
                {
                    result.s = -1;
                    result.d = "无信息件才可以绑定订单";
                    return result;
                }
                a.OID = order[0].ID;
                a.SoID = order[0].SoID;
                a.BuyerShopID = order[0].BuyerShopID;
                a.RecName = order[0].RecName;
                a.RecTel = order[0].RecTel;
                a.RecPhone = order[0].RecPhone;
                a.ShopID = order[0].ShopID;
                a.ShopName = order[0].ShopName;
                a.OrdType = order[0].Type;
                a.Distributor = order[0].Distributor;
                a.Modifier = UserName;
                a.ModifyDate = DateTime.Now;
                sqlcommand = @"update aftersale set OID=@OID,SoID=@SoID,BuyerShopID=@BuyerShopID,RecName=@RecName,RecTel=@RecTel,RecPhone=@RecPhone,ShopID=@ShopID,
                                ShopName=@ShopName,OrdType=@OrdType,Distributor=@Distributor,Modifier=@Modifier,ModifyDate=@ModifyDate where id = @ID and coid = @CoID";
                count = CoreDBconn.Execute(sqlcommand,a,TransCore);
                if(count < 0)
                {
                    result.s = -3003;
                    return result;
                }
                log = new LogInsert();
                log.OID = a.ID;
                log.Type = 1;
                log.LogDate = DateTime.Now;
                log.UserName = UserName;
                log.Title = "绑定订单";
                log.Remark = a.OID.ToString();
                log.CoID = CoID;
                logs.Add(log);
                if(a.Type == 1)
                {
                    sqlcommand = "select * from orderitem where oid = " + a.OID + " and coid = " + CoID;
                    var item = CoreDBconn.Query<OrderItem>(sqlcommand).AsList();
                    foreach(var it in item)
                    {
                        sqlcommand = @"INSERT INTO aftersaleitem(RID,ReturnType,SkuAutoID,SkuID,SkuName,Norm,GoodsCode,RegisterQty,Price,Amount,Img,CoID,Creator,Modifier) 
                                        VALUES(@RID,@ReturnType,@SkuAutoID,@SkuID,@SkuName,@Norm,@GoodsCode,@RegisterQty,@Price,@Amount,@Img,@CoID,@Creator,@Creator)";
                        var args = new{RID = a.ID,ReturnType = 0,SkuAutoID=it.SkuAutoID,SkuID = it.SkuID,SkuName=it.SkuName,Norm=it.Norm,GoodsCode=it.GoodsCode,RegisterQty=it.Qty,
                                        Price=it.RealPrice,Amount=it.Amount,Img=it.img,CoID=CoID,Creator=UserName};
                        count = CoreDBconn.Execute(sqlcommand,args,TransCore);
                        if(count < 0)
                        {
                            result.s = -3002;
                            return result;
                        }
                    }
                }
                string loginsert = @"INSERT INTO orderlog(OID,Type,LogDate,UserName,Title,Remark,CoID) 
                                                   VALUES(@OID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                count =CoreDBconn.Execute(loginsert,logs,TransCore);
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
            if(result.s > 0)
            {
                sqlcommand = @"select ID,OID,RegisterDate,BuyerShopID,RecName,Type,RecPhone,SalerReturnAmt,BuyerUpAmt,RealReturnAmt,ReturnAccount,ShopName,WarehouseID,RecWarehouse,
                                SoID,IssueType,OrdType,Remark,Status,ShopStatus,Result,GoodsStatus,ModifyDate,Modifier,Creator,RefundStatus,Express,ExCode,IsSubmit,ConfirmDate
                                from aftersale where id = @ID and coid = @Coid";  
                using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                    try{    
                        var u = conn.Query<AfterSaleQuery>(sqlcommand,new{ID=RID,Coid = CoID}).AsList();
                        foreach(var a in u)
                        {
                            a.TypeString = Enum.GetName(typeof(ASType), a.Type);
                            a.IssueTypeString = Enum.GetName(typeof(IssueType), a.IssueType);
                            a.OrdTypeString = OrderHaddle.GetTypeName(a.OrdType);
                            a.StatusString = Enum.GetName(typeof(ASStatus), a.Status);
                            a.ResultString = Enum.GetName(typeof(Result), a.Result);
                        }
                        result.d = u[0];
                    }catch(Exception ex){
                        result.s = -1;
                        result.d = ex.Message;
                        conn.Dispose();
                    }
                }       
            }
            return result;
        }
        ///<summary>
        ///订单详情关闭后,刷新主页面
        ///</summary>
        public static DataResult RefreshAS(int RID,int CoID)
        {
            var result = new DataResult(1,null);
            var res = new RefreshASReturn();
            string sqlcommand = string.Empty;
            sqlcommand = @"select ID,OID,RegisterDate,BuyerShopID,RecName,Type,RecPhone,SalerReturnAmt,BuyerUpAmt,RealReturnAmt,ReturnAccount,ShopName,WarehouseID,RecWarehouse,
                            SoID,IssueType,OrdType,Remark,Status,ShopStatus,Result,GoodsStatus,ModifyDate,Modifier,Creator,RefundStatus,Express,ExCode,IsSubmit,ConfirmDate
                            from aftersale where id = " + RID + " and coid = " + CoID;  
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    var u = conn.Query<AfterSaleQuery>(sqlcommand).AsList();
                    u[0].TypeString = Enum.GetName(typeof(ASType), u[0].Type);
                    u[0].IssueTypeString = Enum.GetName(typeof(IssueType), u[0].IssueType);
                    u[0].OrdTypeString = OrderHaddle.GetTypeName(u[0].OrdType);
                    u[0].StatusString = Enum.GetName(typeof(ASStatus), u[0].Status);
                    u[0].ResultString = Enum.GetName(typeof(Result), u[0].Result);
                    res.AfterSale = u[0];
                    sqlcommand = "select ID,ReturnType,SkuAutoID,SkuID,SkuName,Norm,GoodsCode,RegisterQty,ReturnQty,Price,Amount,img,Creator from aftersaleitem where rid = " + RID + 
                                 " and coid = " + CoID;
                    var g = conn.Query<AfterSaleItemQuery>(sqlcommand).AsList();
                    foreach(var a in g)
                    {
                        a.ReturnTypeString = Enum.GetName(typeof(ReturnType), a.ReturnType);//获取名称
                    }
                    res.AfterSaleItem = g;

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
        ///作废售后单
        ///</summary>
        public static DataResult CancleAfterSale(List<int> RID,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            int count = 0;
            var logs = new List<LogInsert>();
            var log = new LogInsert();
            var res = new CancleAfterSaleReturn();
            var fa = new List<InsertFailReason>();
            var su = new List<CancleAfterSaleSuccess>();
            string sqlcommand = string.Empty;
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                sqlcommand = "select * from aftersale where id in @ID and coid = @CoID";
                var u = CoreDBconn.Query<AfterSale>(sqlcommand,new{ID = RID,CoID = CoID}).AsList();
                if(u.Count == 0)
                {
                    result.s = -1;
                    result.d = "售后单号无效";
                    return result;
                }
                foreach(var a in u)
                {
                    if(a.Status != 0)
                    {
                        var ff = new InsertFailReason();
                        ff.id = a.ID;
                        ff.reason = "只有待确认的单据才可以作废!";
                        fa.Add(ff);
                        continue;
                    }
                    
                    a.Status = 2;
                    a.Modifier = UserName;
                    a.ModifyDate = DateTime.Now;
                    sqlcommand = @"update aftersale set Status=@Status,Modifier=@Modifier,ModifyDate=@ModifyDate where id = @ID and coid = @CoID";
                    count = CoreDBconn.Execute(sqlcommand,a,TransCore);
                    if(count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
       
                    log = new LogInsert();
                    log.OID = a.ID;
                    log.Type = 1;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "作废";
                    log.Remark = "Cancle";
                    log.CoID = CoID;
                    logs.Add(log);
                    
                    var ss = new CancleAfterSaleSuccess();
                    ss.ID = a.ID;
                    ss.Status = a.Status;
                    ss.StatusString = Enum.GetName(typeof(ASStatus), a.Status);
                    su.Add(ss);
                }
                string loginsert = @"INSERT INTO orderlog(OID,Type,LogDate,UserName,Title,Remark,CoID) 
                                                   VALUES(@OID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                count =CoreDBconn.Execute(loginsert,logs,TransCore);
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
            res.SuccessIDs = su;
            res.FailIDs = fa;
            result.d = res; 
            return result;
        }
        ///<summary>
        ///同意退货
        ///</summary>
        public static DataResult AgressReturn(List<int> RID,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            int count = 0;
            var logs = new List<LogInsert>();
            var log = new LogInsert();
            var res = new AgressReturn();
            var fa = new List<InsertFailReason>();
            var su = new List<AgressReturnSuccess>();
            string sqlcommand = string.Empty;
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                sqlcommand = "select * from aftersale where id in @ID and coid = @CoID";
                var u = CoreDBconn.Query<AfterSale>(sqlcommand,new{ID = RID,CoID = CoID}).AsList();
                if(u.Count == 0)
                {
                    result.s = -1;
                    result.d = "售后单号无效";
                    return result;
                }
                foreach(var a in u)
                {
                    if(a.Status != 0)
                    {
                        var ff = new InsertFailReason();
                        ff.id = a.ID;
                        ff.reason = "只有待确认的单据才可以操作!";
                        fa.Add(ff);
                        continue;
                    }
                    if(a.ShopStatus != "买家已经申请退款,等待卖家同意")
                    {
                        var ff = new InsertFailReason();
                        ff.id = a.ID;
                        ff.reason = "只有[买家已经申请退款,等待卖家同意]的单据才可以操作!";
                        fa.Add(ff);
                        continue;
                    }
                    a.ShopStatus = "卖家已经同意退款,等待买家退货";
                    a.Modifier = UserName;
                    a.ModifyDate = DateTime.Now;
                    sqlcommand = @"update aftersale set ShopStatus=@ShopStatus,Modifier=@Modifier,ModifyDate=@ModifyDate where id = @ID and coid = @CoID";
                    count = CoreDBconn.Execute(sqlcommand,a,TransCore);
                    if(count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
       
                    log = new LogInsert();
                    log.OID = a.ID;
                    log.Type = 1;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "同意退货";
                    log.CoID = CoID;
                    logs.Add(log);
                    
                    var ss = new AgressReturnSuccess();
                    ss.ID = a.ID;
                    ss.ShopStatus = a.ShopStatus;
                    su.Add(ss);
                }
                string loginsert = @"INSERT INTO orderlog(OID,Type,LogDate,UserName,Title,Remark,CoID) 
                                                   VALUES(@OID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                count =CoreDBconn.Execute(loginsert,logs,TransCore);
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
            res.SuccessIDs = su;
            res.FailIDs = fa;
            result.d = res; 
            return result;
        }
        ///<summary>
        ///拒绝退货
        ///</summary>
        public static DataResult DisagressReturn(List<int> RID,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            int count = 0;
            var logs = new List<LogInsert>();
            var log = new LogInsert();
            var res = new AgressReturn();
            var fa = new List<InsertFailReason>();
            var su = new List<AgressReturnSuccess>();
            string sqlcommand = string.Empty;
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                sqlcommand = "select * from aftersale where id in @ID and coid = @CoID";
                var u = CoreDBconn.Query<AfterSale>(sqlcommand,new{ID = RID,CoID = CoID}).AsList();
                if(u.Count == 0)
                {
                    result.s = -1;
                    result.d = "售后单号无效";
                    return result;
                }
                foreach(var a in u)
                {
                    if(a.Status != 0)
                    {
                        var ff = new InsertFailReason();
                        ff.id = a.ID;
                        ff.reason = "只有待确认的单据才可以操作!";
                        fa.Add(ff);
                        continue;
                    }
                    if(a.ShopStatus != "买家已经申请退款,等待卖家同意")
                    {
                        var ff = new InsertFailReason();
                        ff.id = a.ID;
                        ff.reason = "只有[买家已经申请退款,等待卖家同意]的单据才可以操作!";
                        fa.Add(ff);
                        continue;
                    }
                    a.ShopStatus = "卖家拒绝退款";
                    a.Modifier = UserName;
                    a.ModifyDate = DateTime.Now;
                    sqlcommand = @"update aftersale set ShopStatus=@ShopStatus,Modifier=@Modifier,ModifyDate=@ModifyDate where id = @ID and coid = @CoID";
                    count = CoreDBconn.Execute(sqlcommand,a,TransCore);
                    if(count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
       
                    log = new LogInsert();
                    log.OID = a.ID;
                    log.Type = 1;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "拒绝退货";
                    log.CoID = CoID;
                    logs.Add(log);
                    
                    var ss = new AgressReturnSuccess();
                    ss.ID = a.ID;
                    ss.ShopStatus = a.ShopStatus;
                    su.Add(ss);
                }
                string loginsert = @"INSERT INTO orderlog(OID,Type,LogDate,UserName,Title,Remark,CoID) 
                                                   VALUES(@OID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                count =CoreDBconn.Execute(loginsert,logs,TransCore);
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
            res.SuccessIDs = su;
            res.FailIDs = fa;
            result.d = res; 
            return result;
        }
        ///<summary>
        ///确认
        ///</summary>
        public static DataResult ConfirmAfterSale(List<int> RID,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            var bu = OrderHaddle.GetConfig(CoID);
            var business = bu.d as Business;
            int count = 0;
            var logs = new List<LogInsert>();
            var log = new LogInsert();
            var res = new ConfirmAfterSaleReturn();
            var fa = new List<InsertFailReason>();
            var su = new List<ConfirmAfterSaleSuccess>();
            string sqlcommand = string.Empty;
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                sqlcommand = "select * from aftersale where id in @ID and coid = @CoID";
                var u = CoreDBconn.Query<AfterSale>(sqlcommand,new{ID = RID,CoID = CoID}).AsList();
                if(u.Count == 0)
                {
                    result.s = -1;
                    result.d = "售后单号无效";
                    return result;
                }
                foreach(var a in u)
                {
                    if(a.Status != 0)
                    {
                        var ff = new InsertFailReason();
                        ff.id = a.ID;
                        ff.reason = "只有待确认的单据才可以操作!";
                        fa.Add(ff);
                        continue;
                    }
                    if(a.Type == 2 && a.OID > -1)//检查换货的售后单，明细中换货&退货的资料是否匹配,产生换货订单
                    {
                        sqlcommand = "select * from aftersale where rid = " + a.ID + " and coid = " + CoID;
                        var item = CoreDBconn.Query<AfterSaleItem>(sqlcommand).AsList();
                        int x = 0,y = 0;
                        foreach(var i in item)
                        {
                            if(i.ReturnType == 0)
                            {
                                x ++;
                            }
                            if(i.ReturnType == 1)
                            {
                                y ++;
                            }
                        }
                        if(x != y)
                        {
                            var ff = new InsertFailReason();
                            ff.id = a.ID;
                            ff.reason = "退货的商品数量不等于换货的商品数量!";
                            fa.Add(ff);
                            continue;
                        }
                        //订单明细
                        var orditems = new List<OrderItem>();
                        decimal amt = 0,weight = 0;
                        int qty = 0;
                        foreach(var i in item)
                        {
                            if(i.ReturnType == 0) continue;
                            string skusql = "select saleprice,weight from coresku where id =" + i.SkuAutoID + " and coid =" + CoID;
                            var s = CoreDBconn.Query<SkuInsert>(skusql).AsList();
                            var orditem = new OrderItem();
                            orditem.SoID = a.SoID;
                            orditem.CoID = CoID;
                            orditem.SkuAutoID = i.SkuAutoID;
                            orditem.SkuID = i.SkuID;
                            orditem.SkuName = i.SkuName;
                            orditem.Norm = i.Norm;
                            orditem.GoodsCode = i.GoodsCode;
                            orditem.Qty = i.RegisterQty;
                            orditem.SalePrice = s[0].saleprice;
                            orditem.RealPrice = i.Price.ToString();
                            orditem.Amount = i.Amount.ToString();
                            orditem.DiscountRate = (i.Price/decimal.Parse(s[0].saleprice)).ToString();
                            orditem.img = i.img;
                            orditem.Weight = s[0].weight;
                            orditem.TotalWeight = (decimal.Parse(s[0].weight) * i.RegisterQty).ToString();
                            orditem.Creator = UserName;
                            orditem.Modifier = UserName;
                            orditems.Add(orditem);
                            qty = qty + i.RegisterQty;
                            amt = amt + i.Amount;
                            weight = weight + decimal.Parse(orditem.TotalWeight);
                        }
                        sqlcommand = "select * from `order` where id = " + a.OID + " and coid = " + CoID;
                        var ord = CoreDBconn.Query<Order>(sqlcommand).AsList();
                        //订单资料
                        var ordNew = new Order();
                        if(ord[0].Type == 0 || ord[0].Type == 1 || ord[0].Type == 2 ||ord[0].Type == 3 ||ord[0].Type == 4 ||ord[0].Type == 5)
                        {
                            ordNew.Type = 2;
                        }
                        if(ord[0].Type == 6 || ord[0].Type == 7 || ord[0].Type == 8 ||ord[0].Type == 9 ||ord[0].Type == 10)
                        {
                            ordNew.Type = 8;
                        }
                        if(ord[0].Type == 11 || ord[0].Type == 12 ||ord[0].Type == 13 ||ord[0].Type == 14 ||ord[0].Type == 15)
                        {
                            ordNew.Type = 13;
                        }
                        if(ord[0].Type == 16 || ord[0].Type == 17 || ord[0].Type == 18 ||ord[0].Type == 19 ||ord[0].Type == 20)
                        {
                            ordNew.Type = 18;
                        }
                        ordNew.DealerType = ord[0].DealerType;
                        ordNew.MergeOID = ord[0].MergeOID;
                        ordNew.IsMerge = ord[0].IsMerge;
                        ordNew.IsSplit = ord[0].IsSplit;
                        ordNew.OSource = 6;
                        ordNew.ODate = DateTime.Now;
                        ordNew.CoID = CoID;
                        ordNew.BuyerID = ord[0].BuyerID;
                        ordNew.BuyerShopID = ord[0].BuyerShopID;
                        ordNew.ShopID = ord[0].ShopID;
                        ordNew.ShopName = ord[0].ShopName;
                        ordNew.ShopSit = ord[0].ShopSit;
                        ordNew.SoID = ord[0].SoID;
                        ordNew.MergeSoID = ord[0].MergeSoID;
                        ordNew.OrdQty = qty;
                        ordNew.Amount = amt.ToString();
                        ordNew.SkuAmount = amt.ToString();
                        ordNew.PaidAmount = amt.ToString();
                        ordNew.PayAmount = amt.ToString();
                        ordNew.ExAmount = "0";
                        ordNew.IsInvoice = ord[0].IsInvoice;
                        ordNew.InvoiceType = ord[0].InvoiceType;
                        ordNew.InvoiceTitle = ord[0].InvoiceTitle;
                        ordNew.InvoiceDate = ord[0].InvoiceDate;
                        ordNew.IsPaid = true;
                        ordNew.PayDate = DateTime.Now;
                        ordNew.PayNbr = ord[0].PayNbr + "_In";
                        ordNew.IsCOD = false;
                        if(business.isexceptions == true)
                        {
                            int reasonid = OrderHaddle.GetReasonID("等待售后收货",CoID,7).s;
                            if(reasonid == -1)
                            {
                                result.s = -1;
                                result.d = "请先设定【等待售后收货】的异常";
                                return result;
                            }
                            ordNew.Status = 7;
                            ordNew.AbnormalStatus = reasonid;
                            ordNew.StatusDec = "等待售后收货";
                        }
                        else
                        {
                            ordNew.Status = 1;
                            ordNew.AbnormalStatus = 0;
                            ordNew.StatusDec = "";
                        }
                        ordNew.ShopStatus = ord[0].ShopStatus;
                        ordNew.RecName = ord[0].RecName;
                        ordNew.RecLogistics = ord[0].RecLogistics;
                        ordNew.RecCity = ord[0].RecCity;
                        ordNew.RecDistrict = ord[0].RecDistrict;
                        ordNew.RecAddress = ord[0].RecAddress;
                        ordNew.RecZip = ord[0].RecZip;
                        ordNew.RecTel = ord[0].RecTel;
                        ordNew.RecPhone = ord[0].RecPhone;
                        ordNew.RecMessage = ord[0].RecMessage;
                        ordNew.SendMessage = ord[0].SendMessage;
                        ordNew.ExWeight = weight.ToString();
                        ordNew.Distributor = ord[0].Distributor;
                        ordNew.SupDistributor = ord[0].SupDistributor;
                        ordNew.Creator = UserName;
                        ordNew.Modifier = UserName;
                        int rtn = 0;
                        sqlcommand = @"INSERT INTO `order`(Type,DealerType,MergeOID,IsMerge,IsSplit,OSource,ODate,CoID,BuyerID,BuyerShopID,ShopID,ShopName,ShopSit,SoID,
                                                           MergeSoID,OrdQty,Amount,SkuAmount,PaidAmount,PayAmount,ExAmount,IsInvoice,InvoiceType,InvoiceTitle,InvoiceDate,
                                                           IsPaid,PayDate,PayNbr,IsCOD,Status,AbnormalStatus,StatusDec,ShopStatus,RecName,RecLogistics,RecCity,RecDistrict,
                                                           RecAddress,RecZip,RecTel,RecPhone,RecMessage,SendMessage,ExWeight,Distributor,SupDistributor,Creator,Modifier) 
                                       VALUES(@Type,@DealerType,@MergeOID,@IsMerge,@IsSplit,@OSource,@ODate,@CoID,@BuyerID,@BuyerShopID,@ShopID,@ShopName,@ShopSit,@SoID,
                                              @MergeSoID,@OrdQty,@Amount,@SkuAmount,@PaidAmount,@PayAmount,@ExAmount,@IsInvoice,@InvoiceType,@InvoiceTitle,@InvoiceDate,
                                              @IsPaid,@PayDate,@PayNbr,@IsCOD,@Status,@AbnormalStatus,@StatusDec,@ShopStatus,@RecName,@RecLogistics,@RecCity,@RecDistrict,
                                              @RecAddress,@RecZip,@RecTel,@RecPhone,@RecMessage,@SendMessage,@ExWeight,@Distributor,@SupDistributor,@Creator,@Modifier)";
                        count = CoreDBconn.Execute(sqlcommand,ordNew,TransCore);
                        if(count < 0)
                        {
                            result.s = -3002;
                            return result;
                        }
                        else
                        {
                            rtn = CoreDBconn.QueryFirst<int>("select LAST_INSERT_ID()");
                        }
                        foreach(var i in orditems)
                        {
                            i.OID = rtn;
                        }
                        sqlcommand = @"INSERT INTO orderitem(OID,SoID,CoID,SkuAutoID,SkuID,SkuName,Norm,GoodsCode,Qty,SalePrice,RealPrice,Amount,DiscountRate,img,
                                                             Weight,TotalWeight,Creator,Modifier) 
                                       VALUES(@OID,@SoID,@CoID,@SkuAutoID,@SkuID,@SkuName,@Norm,@GoodsCode,@Qty,@SalePrice,@RealPrice,@Amount,@DiscountRate,@img,
                                              @Weight,@TotalWeight,@Creator,@Modifier)";
                        count = CoreDBconn.Execute(sqlcommand,orditems,TransCore);
                        if(count < 0)
                        {
                            result.s = -3002;
                            return result;
                        }
                        //付款资料
                        var pay = new PayInfo();
                        pay.PayNbr = ordNew.PayNbr;
                        pay.RecID = ordNew.BuyerID;
                        pay.RecName = ordNew.RecName;
                        pay.OID = rtn;
                        pay.SoID = ordNew.SoID;
                        pay.Payment = "内部流转";
                        pay.PayDate = ordNew.PayDate;
                        pay.Amount = ordNew.PaidAmount;
                        pay.PayAmount = ordNew.PaidAmount;
                        pay.DataSource = 0;
                        pay.Status = 1;
                        pay.CoID = CoID;
                        pay.Creator = UserName;
                        pay.Confirmer = UserName;
                        sqlcommand = @"INSERT INTO payinfo(PayNbr,RecID,RecName,OID,SoID,Payment,PayDate,Amount,PayAmount,DataSource,Status,CoID,Creator,Confirmer) 
                                       VALUES(@PayNbr,@RecID,@RecName,@OID,@SoID,@Payment,@PayDate,@Amount,@PayAmount,@DataSource,@Status,@CoID,@Creator,@Confirmer)";
                        count = CoreDBconn.Execute(sqlcommand,pay,TransCore);
                        if(count < 0)
                        {
                            result.s = -3002;
                            return result;
                        }
                        //Log
                        log = new LogInsert();
                        log.OID = rtn;
                        log.SoID = a.SoID;
                        log.Type = 0;
                        log.LogDate = DateTime.Now;
                        log.UserName = UserName;
                        log.Title = "接单时间";
                        log.Remark = "售后时间";
                        log.CoID = CoID;
                        logs.Add(log);
                    }
                    if(a.Type == 3 && a.OID > -1)//产生补发订单
                    {
                        sqlcommand = "select * from aftersale where rid = " + a.ID + " and coid = " + CoID;
                        var item = CoreDBconn.Query<AfterSaleItem>(sqlcommand).AsList();
                        //订单明细
                        var orditems = new List<OrderItem>();
                        decimal amt = 0,weight = 0;
                        int qty = 0;
                        foreach(var i in item)
                        {
                            string skusql = "select saleprice,weight from coresku where id =" + i.SkuAutoID + " and coid =" + CoID;
                            var s = CoreDBconn.Query<SkuInsert>(skusql).AsList();
                            var orditem = new OrderItem();
                            orditem.SoID = a.SoID;
                            orditem.CoID = CoID;
                            orditem.SkuAutoID = i.SkuAutoID;
                            orditem.SkuID = i.SkuID;
                            orditem.SkuName = i.SkuName;
                            orditem.Norm = i.Norm;
                            orditem.GoodsCode = i.GoodsCode;
                            orditem.Qty = i.RegisterQty;
                            orditem.SalePrice = s[0].saleprice;
                            orditem.RealPrice = i.Price.ToString();
                            orditem.Amount = i.Amount.ToString();
                            orditem.DiscountRate = (i.Price/decimal.Parse(s[0].saleprice)).ToString();
                            orditem.img = i.img;
                            orditem.Weight = s[0].weight;
                            orditem.TotalWeight = (decimal.Parse(s[0].weight) * i.RegisterQty).ToString();
                            orditem.Creator = UserName;
                            orditem.Modifier = UserName;
                            orditems.Add(orditem);
                            qty = qty + i.RegisterQty;
                            amt = amt + i.Amount;
                            weight = weight + decimal.Parse(orditem.TotalWeight);
                        }
                        sqlcommand = "select * from `order` where id = " + a.OID + " and coid = " + CoID;
                        var ord = CoreDBconn.Query<Order>(sqlcommand).AsList();
                        //订单资料
                        var ordNew = new Order();
                        if(ord[0].Type == 0 || ord[0].Type == 1 || ord[0].Type == 2 ||ord[0].Type == 3 ||ord[0].Type == 4 ||ord[0].Type == 5)
                        {
                            ordNew.Type = 1;
                        }
                        if(ord[0].Type == 6 || ord[0].Type == 7 || ord[0].Type == 8 ||ord[0].Type == 9 ||ord[0].Type == 10)
                        {
                            ordNew.Type = 7;
                        }
                        if(ord[0].Type == 11 || ord[0].Type == 12 ||ord[0].Type == 13 ||ord[0].Type == 14 ||ord[0].Type == 15)
                        {
                            ordNew.Type = 12;
                        }
                        if(ord[0].Type == 16 || ord[0].Type == 17 || ord[0].Type == 18 ||ord[0].Type == 19 ||ord[0].Type == 20)
                        {
                            ordNew.Type = 17;
                        }
                        ordNew.DealerType = ord[0].DealerType;
                        ordNew.MergeOID = ord[0].MergeOID;
                        ordNew.IsMerge = ord[0].IsMerge;
                        ordNew.IsSplit = ord[0].IsSplit;
                        ordNew.OSource = 6;
                        ordNew.ODate = DateTime.Now;
                        ordNew.CoID = CoID;
                        ordNew.BuyerID = ord[0].BuyerID;
                        ordNew.BuyerShopID = ord[0].BuyerShopID;
                        ordNew.ShopID = ord[0].ShopID;
                        ordNew.ShopName = ord[0].ShopName;
                        ordNew.ShopSit = ord[0].ShopSit;
                        ordNew.SoID = ord[0].SoID;
                        ordNew.MergeSoID = ord[0].MergeSoID;
                        ordNew.OrdQty = qty;
                        ordNew.Amount = amt.ToString();
                        ordNew.SkuAmount = amt.ToString();
                        ordNew.PaidAmount = amt.ToString();
                        ordNew.PayAmount = amt.ToString();
                        ordNew.ExAmount = "0";
                        ordNew.IsInvoice = ord[0].IsInvoice;
                        ordNew.InvoiceType = ord[0].InvoiceType;
                        ordNew.InvoiceTitle = ord[0].InvoiceTitle;
                        ordNew.InvoiceDate = ord[0].InvoiceDate;
                        ordNew.IsPaid = true;
                        if(amt > 0)
                        {
                            ordNew.PayDate = DateTime.Now;
                            ordNew.PayNbr = ord[0].PayNbr + "_In";
                        }
                        ordNew.IsCOD = false;
                        ordNew.Status = 1;
                        ordNew.AbnormalStatus = 0;
                        ordNew.StatusDec = "";
                        ordNew.ShopStatus = ord[0].ShopStatus;
                        ordNew.RecName = ord[0].RecName;
                        ordNew.RecLogistics = ord[0].RecLogistics;
                        ordNew.RecCity = ord[0].RecCity;
                        ordNew.RecDistrict = ord[0].RecDistrict;
                        ordNew.RecAddress = ord[0].RecAddress;
                        ordNew.RecZip = ord[0].RecZip;
                        ordNew.RecTel = ord[0].RecTel;
                        ordNew.RecPhone = ord[0].RecPhone;
                        ordNew.RecMessage = ord[0].RecMessage;
                        ordNew.SendMessage = ord[0].SendMessage;
                        ordNew.ExWeight = weight.ToString();
                        ordNew.Distributor = ord[0].Distributor;
                        ordNew.SupDistributor = ord[0].SupDistributor;
                        ordNew.Creator = UserName;
                        ordNew.Modifier = UserName;
                        int rtn = 0;
                        sqlcommand = @"INSERT INTO `order`(Type,DealerType,MergeOID,IsMerge,IsSplit,OSource,ODate,CoID,BuyerID,BuyerShopID,ShopID,ShopName,ShopSit,SoID,
                                                           MergeSoID,OrdQty,Amount,SkuAmount,PaidAmount,PayAmount,ExAmount,IsInvoice,InvoiceType,InvoiceTitle,InvoiceDate,
                                                           IsPaid,PayDate,PayNbr,IsCOD,Status,AbnormalStatus,StatusDec,ShopStatus,RecName,RecLogistics,RecCity,RecDistrict,
                                                           RecAddress,RecZip,RecTel,RecPhone,RecMessage,SendMessage,ExWeight,Distributor,SupDistributor,Creator,Modifier) 
                                       VALUES(@Type,@DealerType,@MergeOID,@IsMerge,@IsSplit,@OSource,@ODate,@CoID,@BuyerID,@BuyerShopID,@ShopID,@ShopName,@ShopSit,@SoID,
                                              @MergeSoID,@OrdQty,@Amount,@SkuAmount,@PaidAmount,@PayAmount,@ExAmount,@IsInvoice,@InvoiceType,@InvoiceTitle,@InvoiceDate,
                                              @IsPaid,@PayDate,@PayNbr,@IsCOD,@Status,@AbnormalStatus,@StatusDec,@ShopStatus,@RecName,@RecLogistics,@RecCity,@RecDistrict,
                                              @RecAddress,@RecZip,@RecTel,@RecPhone,@RecMessage,@SendMessage,@ExWeight,@Distributor,@SupDistributor,@Creator,@Modifier)";
                        count = CoreDBconn.Execute(sqlcommand,ordNew,TransCore);
                        if(count < 0)
                        {
                            result.s = -3002;
                            return result;
                        }
                        else
                        {
                            rtn = CoreDBconn.QueryFirst<int>("select LAST_INSERT_ID()");
                        }
                        foreach(var i in orditems)
                        {
                            i.OID = rtn;
                        }
                        sqlcommand = @"INSERT INTO orderitem(OID,SoID,CoID,SkuAutoID,SkuID,SkuName,Norm,GoodsCode,Qty,SalePrice,RealPrice,Amount,DiscountRate,img,
                                                             Weight,TotalWeight,Creator,Modifier) 
                                       VALUES(@OID,@SoID,@CoID,@SkuAutoID,@SkuID,@SkuName,@Norm,@GoodsCode,@Qty,@SalePrice,@RealPrice,@Amount,@DiscountRate,@img,
                                              @Weight,@TotalWeight,@Creator,@Modifier)";
                        count = CoreDBconn.Execute(sqlcommand,orditems,TransCore);
                        if(count < 0)
                        {
                            result.s = -3002;
                            return result;
                        }
                        //付款资料
                        if(amt > 0)
                        {
                            var pay = new PayInfo();
                            pay.PayNbr = ordNew.PayNbr;
                            pay.RecID = ordNew.BuyerID;
                            pay.RecName = ordNew.RecName;
                            pay.OID = rtn;
                            pay.SoID = ordNew.SoID;
                            pay.Payment = "内部流转";
                            pay.PayDate = ordNew.PayDate;
                            pay.Amount = ordNew.PaidAmount;
                            pay.PayAmount = ordNew.PaidAmount;
                            pay.DataSource = 0;
                            pay.Status = 1;
                            pay.CoID = CoID;
                            pay.Creator = UserName;
                            pay.Confirmer = UserName;
                            sqlcommand = @"INSERT INTO payinfo(PayNbr,RecID,RecName,OID,SoID,Payment,PayDate,Amount,PayAmount,DataSource,Status,CoID,Creator,Confirmer) 
                                        VALUES(@PayNbr,@RecID,@RecName,@OID,@SoID,@Payment,@PayDate,@Amount,@PayAmount,@DataSource,@Status,@CoID,@Creator,@Confirmer)";
                            count = CoreDBconn.Execute(sqlcommand,pay,TransCore);
                            if(count < 0)
                            {
                                result.s = -3002;
                                return result;
                            }
                        }
                        //Log
                        log = new LogInsert();
                        log.OID = rtn;
                        log.SoID = a.SoID;
                        log.Type = 0;
                        log.LogDate = DateTime.Now;
                        log.UserName = UserName;
                        log.Title = "接单时间";
                        log.Remark = "售后时间";
                        log.CoID = CoID;
                        logs.Add(log);
                    }
                    a.Status = 1;
                    a.Modifier = UserName;
                    a.ModifyDate = DateTime.Now;
                    a.ConfirmDate = DateTime.Now;
                    sqlcommand = @"update aftersale set Status=@Status,Modifier=@Modifier,ModifyDate=@ModifyDate,ConfirmDate=@ConfirmDate where id = @ID and coid = @CoID";
                    count = CoreDBconn.Execute(sqlcommand,a,TransCore);
                    if(count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
       
                    log = new LogInsert();
                    log.OID = a.ID;
                    log.Type = 1;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "确认";
                    log.Remark = "Confirm";
                    log.CoID = CoID;
                    logs.Add(log);
                    
                    var ss = new ConfirmAfterSaleSuccess();
                    ss.ID = a.ID;
                    ss.Status = a.Status;
                    ss.StatusString = Enum.GetName(typeof(ASStatus), a.Status);
                    ss.Modifier = a.Modifier;
                    ss.ModifyDate = a.ModifyDate.ToString();
                    ss.ConfirmDate = a.ConfirmDate.ToString();
                    su.Add(ss);
                }
                string loginsert = @"INSERT INTO orderlog(OID,Type,LogDate,UserName,Title,Remark,CoID) 
                                                   VALUES(@OID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                count =CoreDBconn.Execute(loginsert,logs,TransCore);
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
            res.SuccessIDs = su;
            res.FailIDs = fa;
            result.d = res; 
            return result;
        }
    }
}