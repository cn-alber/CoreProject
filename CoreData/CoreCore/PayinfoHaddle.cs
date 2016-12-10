using CoreModels;
using MySql.Data.MySqlClient;
using System;
using CoreModels.XyCore;
using Dapper;
using System.Collections.Generic;
using static CoreModels.Enum.OrderE;
namespace CoreData.CoreCore
{
    public static class PayinfoHaddle
    {
        ///<summary>
        ///查询支付单List
        ///</summary>
        public static DataResult GetPayinfoList(PayInfoParm cp)
        {
            var result = new DataResult(1,null);    
            string sqlcount = "select count(id) from payinfo where 1=1";
            string sqlcommand = @"select ID,PayDate,OID,SoID,PayNbr,PayAmount,Status,Payment,PayAccount,BuyerShopID from payinfo where 1=1";         
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
            if(!string.IsNullOrEmpty(cp.PayNbr))//付款单号
            {
               wheresql = wheresql + " and paynbr = '" + cp.PayNbr + "'";
            }
            if(cp.DateStart > DateTime.Parse("1900-01-01"))
            {
                wheresql = wheresql + " AND paydate >= '" + cp.DateStart + "'" ;
            }
            if(cp.DateEnd > DateTime.Parse("1900-01-01"))
            {
                wheresql = wheresql + " AND paydate <= '" + cp.DateEnd + "'" ;
            }
            if(cp.Status >= 0)//状态
            {
                wheresql = wheresql + " and Status = " + cp.Status;
            }
            if(!string.IsNullOrEmpty(cp.BuyerShopID))//买家账号
            {
               wheresql = wheresql + " and buyershopid like '%" + cp.BuyerShopID + "%'";
            }
            if(!string.IsNullOrEmpty(cp.Payment))//支付方式
            {
               wheresql = wheresql + " and Payment like '%" + cp.Payment + "%'";
            }
            if(!string.IsNullOrEmpty(cp.SortField) && !string.IsNullOrEmpty(cp.SortDirection))//排序
            {
                wheresql = wheresql + " ORDER BY "+cp.SortField +" "+ cp.SortDirection;
            }
            var res = new PayInfoData();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    int count = conn.QueryFirst<int>(sqlcount + wheresql);
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(cp.NumPerPage.ToString()));
                    int dataindex = (cp.PageIndex - 1)* cp.NumPerPage;
                    wheresql = wheresql + " limit " + dataindex.ToString() + " ," + cp.NumPerPage.ToString();
                    var u = conn.Query<PayInfoQuery>(sqlcommand + wheresql).AsList();
                    foreach(var a in u)
                    {
                        a.StatusString = Enum.GetName(typeof(PayStatus), a.Status);
                    }
                    res.Datacnt = count;
                    res.Pagecnt = pagecnt;
                    res.Pay = u;
                    var data = GetPayStatusInit().d as GetPayStatusInitReturn;
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
        public static DataResult GetPayStatusInit()                
        {
            var result = new DataResult(1,null);
            var res = new GetPayStatusInitReturn();
            var filter = new List<Filter>();
            foreach (int  myCode in Enum.GetValues(typeof(PayStatus)))
            {
                var s = new Filter();
                s.value = myCode.ToString();
                s.label = Enum.GetName(typeof(PayStatus), myCode);//获取名称
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
        ///更新付款资料
        ///</summary>
        public static DataResult UpdatePay(DateTime Paydate,string PayNbr,decimal PayAmount,string Payment,string PayAccount,int ID,int CoID)
        {
            var result = new DataResult(1,null);   
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string sqlcommand = "select * from payinfo where id = " + ID + " and coid = " + CoID;
                var u = CoreDBconn.Query<PayInfo>(sqlcommand).AsList();
                if(u.Count == 0)
                {
                    result.s = -1;
                    result.d = "付款ID无效!";
                    return result;
                }
                if(u[0].Status != 0)
                {
                    result.s = -1;
                    result.d = "待审核的付款单才可以修改!";
                    return result;
                }
                int i = 0;
                if(Paydate > DateTime.Parse("1900-01-01") && Paydate != u[0].PayDate)
                {
                    i ++;
                    u[0].PayDate = Paydate;
                }
                if(PayAmount > 0 && PayAmount != decimal.Parse(u[0].PayAmount))
                {
                    i ++;
                    u[0].PayAmount = PayAmount.ToString();
                }
                if(!string.IsNullOrEmpty(PayNbr) && PayNbr != u[0].PayNbr)
                {
                    i ++;
                    u[0].PayNbr = PayNbr;
                }
                if(!string.IsNullOrEmpty(Payment) && Payment != u[0].Payment)
                {
                    i ++;
                    u[0].Payment = Payment;
                }
                if(!string.IsNullOrEmpty(PayAccount) && PayAccount != u[0].PayAccount)
                {
                    i ++;
                    u[0].PayAccount = PayAccount;
                }
                if(i > 0)
                {
                    sqlcommand = "update payinfo set Paydate=@Paydate,PayAmount=@PayAmount,PayNbr=@PayNbr,Payment=@Payment,PayAccount=@PayAccount where id = @ID and coid = @Coid";
                    int count = CoreDBconn.Execute(sqlcommand,u[0],TransCore);
                    if(count <= 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                }
                sqlcommand = @"select ID,PayDate,OID,SoID,PayNbr,PayAmount,Status,Payment,PayAccount,BuyerShopID from payinfo where id = " + ID + " and coid = " + CoID;
                var uu = CoreDBconn.Query<PayInfoQuery>(sqlcommand).AsList();
                foreach(var a in uu)
                {
                    a.StatusString = Enum.GetName(typeof(PayStatus), a.Status);
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
        ///支付作废
        ///</summary>
        public static DataResult CanclePay(int payid,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            var logs = new List<LogInsert>();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string  wheresql = "select * from payinfo where id =" + payid + " and coid =" + CoID;
                var payinfo = CoreDBconn.Query<PayInfo>(wheresql).AsList();
                if(payinfo.Count == 0)
                {
                    result.s = -1;
                    result.d = "内部支付单ID参数异常!";
                    return result;
                }
                if(payinfo[0].Status != 0)
                {
                    result.s = -1;
                    result.d = "该笔支付单不可作废!";
                    return result;
                }
                if(payinfo[0].OID > 0)
                {
                    wheresql = "select status,soid from `order` where id =" + payinfo[0].OID + " and coid =" + CoID;
                    var u = CoreDBconn.Query<Order>(wheresql).AsList();
                    if(u.Count > 0)
                    {
                        var log = new LogInsert();
                        log.OID = payinfo[0].OID;
                        log.SoID = u[0].SoID;
                        log.Type = 0;
                        log.LogDate = DateTime.Now;
                        log.UserName = UserName;
                        log.Title = "支付单作废";
                        log.CoID = CoID;
                        logs.Add(log);
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
        ///支付审核
        ///</summary>
        public static DataResult ComfirmPay(int payid,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            var logs = new List<LogInsert>();
            bool isflag = false;
            int OID = 0;
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string  wheresql = "select * from payinfo where id =" + payid + " and coid =" + CoID;
                var payinfo = CoreDBconn.Query<PayInfo>(wheresql).AsList();
                if(payinfo.Count == 0)
                {
                    result.s = -1;
                    result.d = "内部支付单ID参数异常!";
                    return result;
                }
                if(payinfo[0].Status != 0)
                {
                    result.s = -1;
                    result.d = "该笔支付单不可审核!";
                    return result;
                }
                if(payinfo[0].OID > 0)
                {
                    wheresql = "select status,soid from `order` where id =" + payinfo[0].OID + " and coid =" + CoID;
                    var u = CoreDBconn.Query<Order>(wheresql).AsList();
                    if(u.Count == 0)
                    {
                        result.s = -1;
                        result.d = "订单ID无效!";
                        return result;
                    }
                    if(u[0].Status != 0 && u[0].Status != 1 && u[0].Status != 7)
                    {
                        result.s = -1;
                        result.d = "待付款/已付款待审核/异常的订单才可以审核支付单!";
                        return result;
                    }
                    isflag = true;
                    OID = payinfo[0].OID;
                }    
                else
                {
                    //更新支付单资料
                    string sqlCommandText = @"update payinfo set Status = 1 where id = " + payid + " and coid = " + CoID;
                    int count = CoreDBconn.Execute(sqlCommandText,TransCore);
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
            if(isflag == true)
            {
                result = OrderHaddle.ConfirmPay(OID,payid,CoID,UserName);
            }
            result.d = null;
            return result;
        }
        ///<summary>
        ///支付取消审核
        ///</summary>
        public static DataResult CancleComfirmPay(int payid,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            var logs = new List<LogInsert>();
            bool isflag = false;
            int OID = 0;
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string  wheresql = "select * from payinfo where id =" + payid + " and coid =" + CoID;
                var payinfo = CoreDBconn.Query<PayInfo>(wheresql).AsList();
                if(payinfo.Count == 0)
                {
                    result.s = -1;
                    result.d = "内部支付单ID参数异常!";
                    return result;
                }
                if(payinfo[0].Status != 1)
                {
                    result.s = -1;
                    result.d = "该笔支付单不可取消审核!";
                    return result;
                }
                if(payinfo[0].OID > 0)
                {
                    wheresql = "select status,soid from `order` where id =" + payinfo[0].OID + " and coid =" + CoID;
                    var u = CoreDBconn.Query<Order>(wheresql).AsList();
                    if(u.Count == 0)
                    {
                        result.s = -1;
                        result.d = "订单ID无效!";
                        return result;
                    }
                    if(u[0].Status != 0 && u[0].Status != 1 && u[0].Status != 7)
                    {
                        result.s = -1;
                        result.d = "待付款/已付款待审核/异常的订单才可以取消审核支付单!";
                        return result;
                    }
                    isflag = true;
                    OID = payinfo[0].OID;
                }    
                else
                {
                    //更新支付单资料
                    string sqlCommandText = @"update payinfo set Status = 0 where id = " + payid + " and coid = " + CoID;
                    int count = CoreDBconn.Execute(sqlCommandText,TransCore);
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
            if(isflag == true)
            {
                result = OrderHaddle.CancleConfirmPay(OID,payid,CoID,UserName);
            }
            result.d = null;
            return result;
        }
    }
}