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
        ///<summary>
        ///更新退款单资料
        ///</summary>
        public static DataResult UpdateRefund(DateTime RefundDate,string RefundNbr,decimal Amount,string Refundment,string PayAccount,int ID,int CoID,string UserName)
        {
            var result = new DataResult(1,null);   
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string sqlcommand = "select * from refundinfo where id = " + ID + " and coid = " + CoID;
                var u = CoreDBconn.Query<RefundInfo>(sqlcommand).AsList();
                if(u.Count == 0)
                {
                    result.s = -1;
                    result.d = "付款ID无效!";
                    return result;
                }
                if(u[0].Status != 0)
                {
                    result.s = -1;
                    result.d = "待审核的退款单才可以修改!";
                    return result;
                }
                int i = 0;
                if(RefundDate > DateTime.Parse("1900-01-01") && RefundDate != u[0].RefundDate)
                {
                    i ++;
                    u[0].RefundDate = RefundDate;
                }
                if(Amount > 0 && Amount != u[0].Amount)
                {
                    i ++;
                    u[0].Amount = Amount;
                }
                if(!string.IsNullOrEmpty(RefundNbr) && RefundNbr != u[0].RefundNbr)
                {
                    i ++;
                    u[0].RefundNbr = RefundNbr;
                }
                if(!string.IsNullOrEmpty(Refundment) && Refundment != u[0].Refundment)
                {
                    i ++;
                    u[0].Refundment = Refundment;
                }
                if(!string.IsNullOrEmpty(PayAccount) && PayAccount != u[0].PayAccount)
                {
                    i ++;
                    u[0].PayAccount = PayAccount;
                }
                if(i > 0)
                {
                    u[0].Modifier = UserName;
                    u[0].ModifyDate = DateTime.Now;
                    sqlcommand = @"update refundinfo set RefundDate=@RefundDate,Amount=@Amount,RefundNbr=@RefundNbr,Refundment=@Refundment,PayAccount=@PayAccount,Modifier=@Modifier,
                                   ModifyDate=@ModifyDate where id = @ID and coid = @Coid";
                    int count = CoreDBconn.Execute(sqlcommand,u[0],TransCore);
                    if(count <= 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                }
                sqlcommand = @"select ShopName,ID,RefundDate,ConfirmDate,ModifyDate,OID,SoID,RefundNbr,Amount,Status,Refundment,PayAccount,BuyerShopID,RID,RType,IssueType,RRmark 
                               from refundinfo where id = " + ID + " and coid = " + CoID;
                var uu = CoreDBconn.Query<RefundInfoQuery>(sqlcommand).AsList();
                foreach(var a in uu)
                {
                    a.StatusString = Enum.GetName(typeof(RefundStatus), a.Status);
                    a.RTypeString = Enum.GetName(typeof(ASType), a.RType);
                    a.IssueTypeString = Enum.GetName(typeof(IssueType), a.IssueType);
                }
                result.d = uu[0];
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
            return result;
        }
        ///<summary>
        ///退款作废
        ///</summary>
        public static DataResult CancleRefund(int Refundid,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string  wheresql = "select * from refundinfo where id =" + Refundid + " and coid =" + CoID;
                var payinfo = CoreDBconn.Query<RefundInfo>(wheresql).AsList();
                if(payinfo.Count == 0)
                {
                    result.s = -1;
                    result.d = "付款ID无效!";
                    return result;
                }
                if(payinfo[0].Status != 0)
                {
                    result.s = -1;
                    result.d = "待审核的退款单才可以作废!";
                    return result;
                }            
                //更新支付单资料
                string sqlCommandText = @"update refundinfo set Status = 2,Modifier=@Modifier,ModifyDate=@ModifyDate where id = " + Refundid + " and coid = " + CoID;
                int count = CoreDBconn.Execute(sqlCommandText,new{Modifier=UserName,ModifyDate=DateTime.Now},TransCore);
                if(count < 0)
                {
                    result.s = -3003;
                    return result;
                }
                string sqlcommand = @"select ShopName,ID,RefundDate,ConfirmDate,ModifyDate,OID,SoID,RefundNbr,Amount,Status,Refundment,PayAccount,BuyerShopID,RID,RType,IssueType,RRmark 
                               from refundinfo where id = " + Refundid + " and coid = " + CoID;
                var uu = CoreDBconn.Query<RefundInfoQuery>(sqlcommand).AsList();
                foreach(var a in uu)
                {
                    a.StatusString = Enum.GetName(typeof(RefundStatus), a.Status);
                    a.RTypeString = Enum.GetName(typeof(ASType), a.RType);
                    a.IssueTypeString = Enum.GetName(typeof(IssueType), a.IssueType);
                }
                result.d = uu[0];
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
        ///退款审核
        ///</summary>
        public static DataResult ComfirmRefund(int payid,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string  wheresql = "select * from refundinfo where id =" + payid + " and coid =" + CoID;
                var payinfo = CoreDBconn.Query<RefundInfo>(wheresql).AsList();
                if(payinfo.Count == 0)
                {
                    result.s = -1;
                    result.d = "内部支付单ID参数异常!";
                    return result;
                }
                if(payinfo[0].Status != 0)
                {
                    result.s = -1;
                    result.d = "该笔退款单不可审核!";
                    return result;
                }
                //更新支付单资料
                string sqlCommandText = @"update refundinfo set Status = 1,Modifier=@Modifier,ModifyDate=@ModifyDate,Confirmer=@Modifier,ConfirmDate=@ModifyDate
                                           where id = " + payid + " and coid = " + CoID;
                int count = CoreDBconn.Execute(sqlCommandText,new{Modifier=UserName,ModifyDate=DateTime.Now},TransCore);
                if(count < 0)
                {
                    result.s = -3003;
                    return result;
                }    
                string sqlcommand = @"select ShopName,ID,RefundDate,ConfirmDate,ModifyDate,OID,SoID,RefundNbr,Amount,Status,Refundment,PayAccount,BuyerShopID,RID,RType,IssueType,RRmark 
                               from refundinfo where id = " + payid + " and coid = " + CoID;
                var uu = CoreDBconn.Query<RefundInfoQuery>(sqlcommand).AsList();
                foreach(var a in uu)
                {
                    a.StatusString = Enum.GetName(typeof(RefundStatus), a.Status);
                    a.RTypeString = Enum.GetName(typeof(ASType), a.RType);
                    a.IssueTypeString = Enum.GetName(typeof(IssueType), a.IssueType);
                }
                result.d = uu[0];
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
        ///退款审核取消
        ///</summary>
        public static DataResult CancleComfirmRefund(int payid,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string  wheresql = "select * from refundinfo where id =" + payid + " and coid =" + CoID;
                var payinfo = CoreDBconn.Query<RefundInfo>(wheresql).AsList();
                if(payinfo.Count == 0)
                {
                    result.s = -1;
                    result.d = "内部支付单ID参数异常!";
                    return result;
                }
                if(payinfo[0].Status != 1)
                {
                    result.s = -1;
                    result.d = "该笔退款单不可取消!";
                    return result;
                }
                //更新支付单资料
                string sqlCommandText = @"update refundinfo set Status = 0,Modifier=@Modifier,ModifyDate=@ModifyDate,Confirmer=@Confirmer,ConfirmDate=@ConfirmDate
                                           where id = " + payid + " and coid = " + CoID;
                int count = CoreDBconn.Execute(sqlCommandText,new{Modifier=UserName,ModifyDate=DateTime.Now,Confirmer="",ConfirmDate=new DateTime()},TransCore);
                if(count < 0)
                {
                    result.s = -3003;
                    return result;
                }    
                string sqlcommand = @"select ShopName,ID,RefundDate,ConfirmDate,ModifyDate,OID,SoID,RefundNbr,Amount,Status,Refundment,PayAccount,BuyerShopID,RID,RType,IssueType,RRmark 
                               from refundinfo where id = " + payid + " and coid = " + CoID;
                var uu = CoreDBconn.Query<RefundInfoQuery>(sqlcommand).AsList();
                foreach(var a in uu)
                {
                    a.StatusString = Enum.GetName(typeof(RefundStatus), a.Status);
                    a.RTypeString = Enum.GetName(typeof(ASType), a.RType);
                    a.IssueTypeString = Enum.GetName(typeof(IssueType), a.IssueType);
                }
                result.d = uu[0];
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
        ///退款审核取消
        ///</summary>
        public static DataResult CompleteRefund(int payid,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string  wheresql = "select * from refundinfo where id =" + payid + " and coid =" + CoID;
                var payinfo = CoreDBconn.Query<RefundInfo>(wheresql).AsList();
                if(payinfo.Count == 0)
                {
                    result.s = -1;
                    result.d = "内部支付单ID参数异常!";
                    return result;
                }
                if(payinfo[0].Status != 1)
                {
                    result.s = -1;
                    result.d = "该笔退款单不可确认完成!";
                    return result;
                }
                //更新支付单资料
                string sqlCommandText = @"update refundinfo set Status = 3,Modifier=@Modifier,ModifyDate=@ModifyDate where id = " + payid + " and coid = " + CoID;
                int count = CoreDBconn.Execute(sqlCommandText,new{Modifier=UserName,ModifyDate=DateTime.Now},TransCore);
                if(count < 0)
                {
                    result.s = -3003;
                    return result;
                }    
                string sqlcommand = @"select ShopName,ID,RefundDate,ConfirmDate,ModifyDate,OID,SoID,RefundNbr,Amount,Status,Refundment,PayAccount,BuyerShopID,RID,RType,IssueType,RRmark 
                               from refundinfo where id = " + payid + " and coid = " + CoID;
                var uu = CoreDBconn.Query<RefundInfoQuery>(sqlcommand).AsList();
                foreach(var a in uu)
                {
                    a.StatusString = Enum.GetName(typeof(RefundStatus), a.Status);
                    a.RTypeString = Enum.GetName(typeof(ASType), a.RType);
                    a.IssueTypeString = Enum.GetName(typeof(IssueType), a.IssueType);
                }
                result.d = uu[0];
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