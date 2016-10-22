using System;
using System.Collections.Generic;
using CoreData;
using CoreData.CoreComm;
using CoreData.CoreUser;
using CoreModels.XyComm;
using CoreModels.XyUser;
using API.Data;
using Dapper;
using MySql.Data.MySqlClient;
using static CoreModels.Enum.OrderE;

namespace CoreWebApi.ApiTask
{
    /// <summary>
    /// API 任务工作项
    /// </summary>

    public class ApiJob
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public int JobId { get; protected set; }

        public int CoId { get; set; }

        public int ShopId { get; set; }

        public string ApiKey { get; set; }

        /// <summary>
        /// 自定义分任务ID
        /// </summary>
        public string SplitId { get; set; }

        public ApiTypes ApiType { get; set; }

        public string ApiGroup { get; set; }

        public string ApiName { get; set; }

        public string ApiAuthor { get; set; }

        public int ApiInterval { get; set; }

        public int ApiTimeout { get; set; }

        public bool ApiRunning { get; set; }

        public int ApiLazy { get; set; }

        public int Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public long RunTotal { get; set; }

        public string RunHost { get; set; }

        public long RunId { get; set; }

        public DateTime RunBof { get; set; }

        public DateTime RunEof { get; set; }

        public decimal RunTimes { get; set; }

        public int RunResult { get; set; }

        public string RunState { get; set; }

        public DateTime? RunTimestamp { get; set; }

        public long ErrTotal { get; set; }

        public int ErrRetry { get; set; }

        public int ErrCode { get; set; }

        public string ErrMessage { get; set; }

        public DateTime? ErrTimestamp { get; set; }

        public CompanyMulti Company
        {
            get
            {
                if (this.CoId > 0 && this.ShopId > 0 && this.company == null)
                {
                    //this.company = S3.User.CompanyService.GetCompany(this.CoId, true);
                    this.company = CompanyHaddle.GetCompanyEdit(this.CoId).d as CompanyMulti;

                }
                return this.company;
            }

            set
            {
                this.company = value;
            }
        }
        
        private CompanyMulti company;





        public Shop Shop
        {
            get
            {
                if (this.CoId > 0 && this.ShopId > 0 && this.shop == null)
                {
                    this.shop = ShopHaddle.ShopQuery(this.CoId.ToString(),this.ShopId.ToString()).d as Shop;
                }
                return this.shop;
            }

            set
            {
                this.shop = value;
            }
        }
        private Shop shop;

        /// <summary>
        /// 执行必要的初始化
        /// </summary>
        public void Load()
        {
            this.Company = null;
            this.Shop = null;
        }


        /// <summary>
        /// 实体映射的字段列
        /// </summary>
        public static readonly string TableFields = "job_id,co_id,shop_id,api_mode,api_key,split_id,api_type,api_group,api_name,api_author,api_interval,api_timeout,api_running,api_lazy,status,created,modified,run_total,run_host,run_id,run_bof,run_eof,run_times,run_result,run_state,run_timestamp,err_total,err_retry,err_code,err_message,err_timestamp";

        /// <summary>
        /// 从数据源创建一个 ApiJob 实例
        /// </summary>
        public static ApiJob Create(api_job reader, bool ready)
        {
            ApiJob item = null;
            if (ready)
            {
                item = new ApiJob();
                item.JobId = DbConvert.ToInt32(reader.job_id, 0);
                item.CoId = DbConvert.ToInt32(reader.co_id, 0);
                item.ShopId = DbConvert.ToInt32(reader.shop_id, 0);

                item.ApiKey = DbConvert.ToString(reader.api_key);
                item.SplitId = DbConvert.ToString(reader.split_id);
                ApiTypes apiType;
                if (Enum.TryParse<ApiTypes>(DbConvert.ToString(reader.api_type, "base"), true, out apiType))
                {
                    item.ApiType = apiType;
                }

                item.ApiGroup = DbConvert.ToString(reader.api_group);
                item.ApiName = DbConvert.ToString(reader.api_name);
                item.ApiAuthor = DbConvert.ToString(reader.api_author);
                item.ApiInterval = DbConvert.ToInt32(reader.api_interval, 86400);
                item.ApiTimeout = DbConvert.ToInt32(reader.api_timeout, 300);
                item.ApiRunning = DbConvert.ToBoolean(reader.api_running, false);
                item.ApiLazy = DbConvert.ToInt32(reader.api_lazy, 0);
                item.Status = DbConvert.ToInt32(reader.status, 0);
                item.Created = DbConvert.ToDateTime(reader.created, DateTime.Now);
                item.Modified = DbConvert.ToDateTime(reader.modified, DateTime.Now);
                item.RunTotal = DbConvert.ToInt64(reader.run_total, 0);
                item.RunHost = DbConvert.ToString(reader.run_host);
                item.RunId = DbConvert.ToInt64(reader.run_id, 0);
                item.RunBof = DbConvert.ToDateTime(reader.run_bof, DateTime.Now);
                item.RunEof = DbConvert.ToDateTime(reader.run_eof, DateTime.Now);
                item.RunTimes = DbConvert.ToDecimal(reader.run_times, 0);
                item.RunResult = Math.Max(DbConvert.ToInt32(reader.run_result, 0), 0);
                item.RunState = DbConvert.ToString(reader.run_state);
                item.RunTimestamp = DbConvert.ToDateTime(reader.run_timestamp);
                item.ErrTotal = DbConvert.ToInt64(reader.err_total, 0);
                item.ErrRetry = DbConvert.ToInt32(reader.err_retry, 0);
                item.ErrCode = DbConvert.ToInt32(reader.err_code, 0);
                item.ErrMessage = DbConvert.ToString(reader.err_message);
                item.ErrTimestamp = DbConvert.ToDateTime(reader.err_timestamp);
            }

            if (item != null)
            {
                item.Load();
            }


            return item;
        }

       

        public static void updateApiJob(Shop shop) {
            if (Convert.ToBoolean(shop.Enable)) { 
                string is_order_api_get = shop.ShopType.ToLower() + "\\\\order_api_get";
                string is_refund_api_get = shop.ShopType.ToLower() + "\\\\refund_api_get";
                string is_update_sku = shop.ShopType.ToLower() + "\\\\update_sku";
                string is_down_goods = shop.ShopType.ToLower() + "\\\\down_goods";
                string is_update_way_bill = shop.ShopType.ToLower() + "\\\\update_way_bill";

                var sqlList = new List<string>();
                if (Convert.ToBoolean(shop.UpdateSku))
                    sqlList.Add(String.Format(@"UPDATE api_job SET api_job.enabled = 1 WHERE  api_job.shop_id = {0} AND api_job.api_mode = 'S' AND api_job.api_key = '{1}' ;",shop.ID,is_update_sku));

                if (Convert.ToBoolean(shop.DownGoods))
                    sqlList.Add(String.Format(@"UPDATE api_job SET api_job.enabled = 1 WHERE  api_job.shop_id = {0} AND api_job.api_mode = 'S' AND api_job.api_key = '{1}' ;", shop.ID, is_down_goods));

                if (Convert.ToBoolean(shop.UpdateWayBill))
                    sqlList.Add(String.Format(@"UPDATE api_job SET api_job.enabled = 1 WHERE  api_job.shop_id = {0} AND api_job.api_mode = 'S' AND api_job.api_key = '{1}' ;", shop.ID, is_update_way_bill));

                sqlList.Add(String.Format(@"UPDATE api_job SET api_job.enabled = 1 WHERE  api_job.shop_id = {0} AND api_job.api_mode = 'S' AND api_job.api_key = '{1}' ;", shop.ID, is_order_api_get));
                sqlList.Add(String.Format(@"UPDATE api_job SET api_job.enabled = 1 WHERE  api_job.shop_id = {0} AND api_job.api_mode = 'S' AND api_job.api_key = '{1}' ;", shop.ID, is_refund_api_get));
                using (var conn = new MySqlConnection(DbBase.CommConnectString)){
                    try
                    {
                        conn.Execute(String.Join("\r\n", sqlList));
                    }
                    catch(Exception ex) {
                        ApiContext.WriteLog("error", String.Format("CoreWebApi.ApiTask.ApiContext.UpdateLazy()"), ex.ToString());
                    }
                }
                
            }


        }





    }
}
