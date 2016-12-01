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
            res.ASType = ff; 
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
    }
}