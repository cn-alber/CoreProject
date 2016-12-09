using CoreModels;
using MySql.Data.MySqlClient;
using System;
using CoreModels.XyCore;
using Dapper;
using System.Collections.Generic;
using static CoreModels.Enum.OrderE;
namespace CoreData.CoreCore
{
    public static class RefundinfoHaddle
    {
        ///<summary>
        ///查询支付单List
        ///</summary>
        public static DataResult GetRefundinfoList(RefundInfoParm cp)
        {
            var result = new DataResult(1,null);    
            string sqlcount = "select count(id) from refundinfo where 1=1";
            string sqlcommand = @"select ShopName,ID,RefundDate,ConfirmDate,ModifyDate,OID,SoID,RefundNbr,Amount,Status,Refundment,PayAccount,BuyerShopID,RID,RType,IssueType,RRmark 
                                  from refundinfo where 1=1";         
            string wheresql = string.Empty;
            if(cp.CoID != 1)//公司编号
            {
                wheresql = wheresql + " and coid = " + cp.CoID;
            }
            if(cp.ID > 0)//内部支付单号
            {
                wheresql = wheresql + " and id = " + cp.ID;
            }
            if(cp.OID > 0)//内部订单号
            {
                wheresql = wheresql + " and oid = " + cp.ID;
            }
            if(cp.SoID > 0)//外部订单号
            {
                wheresql = wheresql + " and soid = " + cp.SoID;
            }
            if(!string.IsNullOrEmpty(cp.RefundNbr))//付款单号
            {
               wheresql = wheresql + " and RefundNbr = '" + cp.RefundNbr + "'";
            }
            if(cp.DateStart > DateTime.Parse("1900-01-01"))
            {
                if(!string.IsNullOrEmpty(cp.DateType) &&　cp.DateType.ToUpper() == "REFUNDDATE")
                {
                    wheresql = wheresql + " AND RefundDate >= '" + cp.DateStart + "'" ;
                }
                if(!string.IsNullOrEmpty(cp.DateType) &&　cp.DateType.ToUpper() == "CONFIRMDATE")
                {
                    wheresql = wheresql + " AND ConfirmDate >= '" + cp.DateStart + "'" ;
                }
                if(!string.IsNullOrEmpty(cp.DateType) &&　cp.DateType.ToUpper() == "MODIFYDATE")
                {
                    wheresql = wheresql + " AND ModifyDate >= '" + cp.DateStart + "'" ;
                }
            }
            if(cp.DateEnd > DateTime.Parse("1900-01-01"))
            {
                if(!string.IsNullOrEmpty(cp.DateType) &&　cp.DateType.ToUpper() == "REFUNDDATE")
                {
                    wheresql = wheresql + " AND RefundDate <= '" + cp.DateEnd + "'" ;
                }
                if(!string.IsNullOrEmpty(cp.DateType) &&　cp.DateType.ToUpper() == "CONFIRMDATE")
                {
                    wheresql = wheresql + " AND ConfirmDate <= '" + cp.DateEnd + "'" ;
                }
                if(!string.IsNullOrEmpty(cp.DateType) &&　cp.DateType.ToUpper() == "MODIFYDATE")
                {
                    wheresql = wheresql + " AND ModifyDate <= '" + cp.DateEnd + "'" ;
                }
            }
            if(cp.Status >= 0)//状态
            {
                wheresql = wheresql + " and Status = " + cp.Status;
            }
            if(cp.ShopID >= 0)
            {
                wheresql = wheresql + " and ShopID = " + cp.ShopID;
            }
            if(!string.IsNullOrEmpty(cp.BuyerShopID))//买家账号
            {
               wheresql = wheresql + " and buyershopid like '%" + cp.BuyerShopID + "%'";
            }
            if(!string.IsNullOrEmpty(cp.Refundment))//支付方式
            {
               wheresql = wheresql + " and Refundment like '%" + cp.Refundment + "%'";
            }
            if(!string.IsNullOrEmpty(cp.Distributor))//分销商
            {
               wheresql = wheresql + " and Distributor = '" + cp.Distributor + "'";
            }
            if(!string.IsNullOrEmpty(cp.SortField) && !string.IsNullOrEmpty(cp.SortDirection))//排序
            {
                wheresql = wheresql + " ORDER BY "+cp.SortField +" "+ cp.SortDirection;
            }
            var res = new RefundInfoData();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    int count = conn.QueryFirst<int>(sqlcount + wheresql);
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(cp.NumPerPage.ToString()));
                    int dataindex = (cp.PageIndex - 1)* cp.NumPerPage;
                    wheresql = wheresql + " limit " + dataindex.ToString() + " ," + cp.NumPerPage.ToString();
                    var u = conn.Query<RefundInfoQuery>(sqlcommand + wheresql).AsList();
                    foreach(var a in u)
                    {
                        a.StatusString = Enum.GetName(typeof(RefundStatus), a.Status);
                        a.RTypeString = Enum.GetName(typeof(ASType), a.RType);
                        a.IssueTypeString = Enum.GetName(typeof(IssueType), a.IssueType);
                    }
                    res.Datacnt = count;
                    res.Pagecnt = pagecnt;
                    res.Refund = u;
                    var data = GetRefundStatusInit().d as GetPayStatusInitReturn;
                    res.Payment = data.Payment;
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
        ///初始状态资料
        ///</summary>
        public static DataResult GetRefundStatusInit()                
        {
            var result = new DataResult(1,null);
            var res = new GetPayStatusInitReturn();
            var filter = new List<Filter>();
            foreach (int  myCode in Enum.GetValues(typeof(RefundStatus)))
            {
                var s = new Filter();
                s.value = myCode.ToString();
                s.label = Enum.GetName(typeof(RefundStatus), myCode);//获取名称
                filter.Add(s);
                res.Status = filter;
            }
            filter = new List<Filter>();
            var ff = new Filter();
            ff.value = "支付宝";
            ff.label = "支付宝";
            filter.Add(ff);
            ff = new Filter();
            ff.value = "快钱";
            ff.label = "快钱";
            filter.Add(ff);
            ff = new Filter();
            ff.value = "招行直连";
            ff.label = "招行直连";
            filter.Add(ff);
            ff = new Filter();
            ff.value = "财付通";
            ff.label = "财付通";
            filter.Add(ff);
            ff = new Filter();
            ff.value = "现金支付";
            ff.label = "现金支付";
            filter.Add(ff);
            ff = new Filter();
            ff.value = "银行转账";
            ff.label = "银行转账";
            filter.Add(ff);
            ff = new Filter();
            ff.value = "其他";
            ff.label = "其他";
            filter.Add(ff);
            ff = new Filter();
            ff.value = "供销支付";
            ff.label = "供销支付";
            filter.Add(ff);
            ff = new Filter();
            ff.value = "快速支付";
            ff.label = "快速支付";
            filter.Add(ff);
            ff = new Filter();
            ff.value = "微信支付";
            ff.label = "微信支付";
            filter.Add(ff);
            ff = new Filter();
            ff.value = "授信";
            ff.label = "授信";
            filter.Add(ff);
            ff = new Filter();
            ff.value = "预支付";
            ff.label = "预支付";
            filter.Add(ff);
            ff = new Filter();
            ff.value = "内部流转";
            ff.label = "内部流转";
            filter.Add(ff);
            ff = new Filter();
            ff.value = "门店会员余额";
            ff.label = "门店会员余额";
            filter.Add(ff);
            res.Payment = filter;

            result.d = res;
            return result;
        }
    }
}