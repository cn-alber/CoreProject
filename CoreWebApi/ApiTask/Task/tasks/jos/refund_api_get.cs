using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreData;
using CoreData.CoreComm;
using CoreData.CoreApi;
using CoreModels.XyApi.JingDong;
using CoreWebApi.ApiTask;
using Dapper;
using MySql.Data.MySqlClient;

namespace tasks.jos
{

    public class refund_api_get : CoreWebApi.ApiTask.BaseApi
    {

        public override string Name
        {
            get { return "从JOS售后退款资料"; }
        }

        public override string Author
        {
            get { return "827869959@qq.com"; }
        }

        public override int Timeout
        {
            get { return 300; }
        }

        public override int Interval
        {
            get { return 300; }
        }
        
        public override int Type{
            get {return 3; }
        }

        protected override void Execute(CoreWebApi.ApiTask.ApiRunData apiData)
        {

            var modified = apiData.Job.RunTimestamp.HasValue ? apiData.Job.RunTimestamp.Value.AddSeconds(1).ToString() : DateTime.Today.AddDays(-3).ToString();           
            var now = DateTime.Now.AddMinutes(-3).ToString();

            string orderIDs = "";

            if (!apiData.Args.IsNullOrEmpty())
            {
                modified = "";
                now = "";
                foreach (var arg in apiData.Args)
                {
                    orderIDs = arg.ToString();
                }
            }else{
                var monthlater = Convert.ToDateTime(modified).AddDays(3);
                if (DateTime.Compare(monthlater, Convert.ToDateTime(now)) < 0)
                { //启动时间戳与当前时间相差不能查过3天
                    now = monthlater.ToString();
                }
            }
            apiData.Job.RunResult = 0;
            GetPage(apiData, modified, now, 1); 
            
        }

         protected void GetPage(CoreWebApi.ApiTask.ApiRunData apiData, string bof, string eof, int pageNo)
        {

            try { 
                var response = JingDHaddle.jdRefundList("", "", "", "", "", bof, eof, "", "", pageNo, 50, apiData.Job.Shop.Token);

                if (response.s !=1)
                {
                    if (response.d.ToString().IndexOf("重新授权") > -1)
                    {
                        ShopHaddle.TokenExpired(apiData.Job.Shop.ID.ToString(),apiData.Job.Shop.CoID.ToString());
                        using (var conn = new MySqlConnection(DbBase.CommConnectString)){  
                            try{
                                conn.Execute(@"UPDATE api_job SET api_job.enabled = 0 WHERE api_job.job_id = @job_id",new {job_id = apiData.Job.JobId });
                            }catch{
                                conn.Dispose();
                            }
                        }
                    }
                    throw new ApiTaskException(601, string.Format("{0}", response.d.ToString()));
                }

                var shop = apiData.Job.Shop;
                Task[] tasks = new Task[10];
                jdRefundListQueryresult searchRes = response.d as jdRefundListQueryresult; 
                List<jdRefundListresult> refundinfos = searchRes.result;

                if (refundinfos.Count > 0)
                {
                    for (int i = 0; i < refundinfos.Count; i++)
                    {
                        if (i == 0)
                        {
                            tasks[i] = Task.Factory.StartNew(() =>
                            {
                                jdAFtolocal(refundinfos[i], int.Parse(shop.CoID.ToString()), shop.ShopName, shop.ShopSite, shop.Token, shop.ID);
                            });
                            System.Threading.Thread.Sleep(1000);
                        }
                        else
                        {
                            try
                            {
                                tasks[i % 10] = Task.Factory.StartNew(() =>
                                {
                                    if (i != 100)
                                    {
                                        jdAFtolocal(refundinfos[i], int.Parse(shop.CoID.ToString()), shop.ShopName, shop.ShopSite, shop.Token, shop.ID);
                                    }
                                });
                                System.Threading.Thread.Sleep(1000);
                            }
                            catch
                            {

                                //XyComm.threadlog.InsertThreadLog("店铺：" + shop.ShopName + "下，单号：" + refundinfos[i].orderId + "退款导入报错", ex.Message, DateTime.Now.ToString(), "线程运行", "");
                                continue;
                            }
                        }
                        if (i % 10 == 9)
                        {
                            Task.WaitAll(tasks);
                            tasks = new Task[10];
                        }
                    } 
                    apiData.Job.RunTimestamp = Convert.ToDateTime(refundinfos.LastOrDefault().applyTime);

                }
                long pageTotal = Convert.ToInt64(searchRes.totalCount);
                if (pageNo < pageTotal)
                {
                    pageNo++;
                    this.GetPage(apiData, bof, eof, pageNo);
                }

            }
            catch (ApiTaskException ex)
            {
                throw new ApiTaskException(602, string.Format("{0}", ex.Message));
            }


        }
        public static void jdAFtolocal(jdRefundListresult refundinfo, int CoID, string ShopName, string ShopSite, string token, int shopid)
        {
            // try
            // {
            //     ImpASModule AsMod = new ImpASModule();
            //     AsMod.SoID = long.Parse(refundinfo.orderId);
            //     AsMod.RegisterDate = Convert.ToDateTime(refundinfo.applyTime);
            //     switch (refundinfo.status)
            //     {
            //         case "0":
            //             AsMod.RefundStatus = "未审核";
            //             break;
            //         case "1":
            //             AsMod.RefundStatus = "审核通过";
            //             break;
            //         case "2":
            //             AsMod.RefundStatus = "审核不通过";
            //             break;
            //         case "3":
            //             AsMod.RefundStatus = "京东财务审核通过";
            //             break;
            //         case "4":
            //             AsMod.RefundStatus = "京东财务审核不通过";
            //             break;
            //         case "5":
            //             AsMod.RefundStatus = "人工审核通过";
            //             break;
            //         case "6":
            //             AsMod.RefundStatus = "拦截并退款";
            //             break;
            //         case "7":
            //             AsMod.RefundStatus = "青龙拦截成功";
            //             break;
            //         case "8":
            //             AsMod.RefundStatus = "青龙拦截失败";
            //             break;
            //         case "9":
            //             AsMod.RefundStatus = "强制关单并退款";
            //             break;
            //     }
            //     AsMod.Remark = "";

            //     BaseResult baseRe = XyOrder.DataUpdate.ImpAS(AsMod, CoID, "线程运行");
            //     if (!baseRe.IsSuccess)
            //     {
            //         XyComm.threadlog.InsertThreadLog("店铺：" + ShopName + "下，单号：" + refundinfo.orderId + "存入本地报错", baseRe.Message, DateTime.Now.ToString(), "", "");
            //     }

            // }
            // catch (Exception ex)
            // {

            //     XyComm.threadlog.InsertThreadLog("店铺：" + ShopName + "下，单号：" + refundinfo.orderId + "导入退款申请报错", ex.Message, DateTime.Now.ToString(), "", "");
            // }

        }

   

    }



}