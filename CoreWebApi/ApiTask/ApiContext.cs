using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using static CoreModels.Enum.OrderE;
using CoreModels.XyComm;
using MySql.Data.MySqlClient;
using Dapper;
using CoreData;
using API.Core.Threading;
using CoreData.CoreComm;


namespace CoreWebApi.ApiTask
{
   /// <summary>
    /// 封装了API请求的特定信息
    /// </summary>
    public static class ApiContext
    {
        static object locker = new object();
        static long TaskTimestamp = long.MinValue;
        static string ClusterLockerCacheKey = "Cluster.Locker";

        static ApiContext()
        {
            
            //Hxj.Data.Common.SplitTableInfo.ConvertSplitTable = S3.Common.Data.SplitTableConverter.ConvertSplitTable;
            ApiContext.HostName = Environment.MachineName.SubLeft(50);
        }



        /// <summary>
        /// 主机名称
        /// </summary>
        internal static String HostName { get; private set; }

        /// <summary>
        /// 主机是否启用
        /// </summary>
        internal static bool HostEnabled { get; private set; }

        /// <summary>
        /// 是否为管理主机
        /// </summary>
        public static bool IsMaster { get; private set; }

        /// <summary>
        /// 是否已开始运行
        /// </summary>
        public static bool IsRunning { get; private set; }

        /// <summary>
        /// 全局配置项
        /// </summary>
        public static HashData<string> Settings { get; private set; }

        /// <summary>
        /// 双11活跃的商家ID列表
        /// </summary>
        public static IList<int> HotCompanies { get; private set; }

        /// <summary>
        /// 通知中心管理器
        /// </summary>
        //public static NotifyCenter NotifyMgr { get; private set; }



        /// <summary>
        /// 开始自动运行
        /// </summary>
        public static void Run()
        {
            if (ApiContext.IsRunning)
            {
                return;
            }

            lock (locker)
            {
                if (ApiContext.IsRunning)
                {
                    return;
                }
                ApiContext.IsRunning = true;
            }


            // HttpContext httpContext = HttpContext.Current;
            // if (httpContext == null)
            // {
            //     //ApiContext.WriteLog("error", "CoreWebApi.ApiTask.ApiContext.Run", "ApplicationException: HttpContext is NULL.");               
            // }            

            ApiContext.LoadSettings();
            ApiContext.RegisterHost();

            TaskManager.MaxThreads = 40;
            TaskManager.MinThreads = 40;
            TaskManager.TaskTimeout += ApiContext.DoTimeout;

            var thread = new Thread(ApiContext.Execute);
            thread.IsBackground = true;
            thread.Start();
        }


        /// <summary>
        /// 运行指定任务[异步]
        /// </summary>
        /// <param name="apiKey">任务名</param>
        /// <param name="coId">指定商家</param>
        /// <param name="shopId">指定商店</param>
        /// <param name="args">交互的数据</param>
        public static void RunAsync(string apiKey, int coId, int shopId, params object[] args)
        {
            ApiJob job = new ApiJob();
            job.ApiKey = apiKey;
            job.CoId = coId;
            job.ShopId = shopId;
            job.Load();
            ApiContext.RunAsync(job, args);
        }

        /// <summary>
        /// 运行工作任务[异步]
        /// </summary>
        /// <param name="job">ApiJob</param>
        /// <param name="args">交互的数据</param>
        public static void RunAsync(ApiJob job, params object[] args)
        {
            ApiContext.CheckTask(job);
            TaskManager.Add((state) => ApiContext.RunTask(state, true, args), job, 0, Timeout.Infinite, job.ApiTimeout);
        }

        /// <summary>
        /// 运行指定任务[同步](以：Task/tasks/jos/order_api_get.cs 为例)
        /// </summary>
        /// <param name="apiKey">任务名</param>
        /// <param name="coId">指定商家</param>
        /// <param name="shopId">指定商店</param>
        /// <param name="args">交互的数据</param>
        public static void RunOnce(string apiKey, int coId, int shopId,params object[] args)
        {
            ApiJob job = new ApiJob();
            job.ApiKey = apiKey;           
            job.CoId = coId;
            job.ShopId = shopId;
            job.Load();
            foreach (string temp in Enum.GetNames(typeof(ApiTypes)))
            {
                if (apiKey.IndexOf(temp.ToLower()) > -1) {
                    job.ApiType = (ApiTypes)Enum.Parse(typeof(ApiTypes), temp);
                    break;
                }
            }


            ApiContext.RunTask(job, false, args);
        }

        /// <summary>
        /// 运行工作任务[同步]
        /// </summary>
        /// <param name="job">ApiJob</param>
        /// <param name="args">交互的数据</param>
        public static void RunOnce(ApiJob job, params object[] args)
        {
            ApiContext.RunTask(job, false, args);
        }

        /// <summary>
        /// 运行工作任务[同步]
        /// </summary>
        /// <param name="job">ApiJob</param>
        /// <param name="async">是否异步</param>
        /// <param name="args">交互的数据</param>
        static void RunTask(ApiJob job, bool async, params object[] args)
        {
            ApiContext.CheckTask(job);

            var startTime = DateTime.Now;
            while (true)
            {
                TaskWrapper wrapper = null;
                try
                {
                    if (!apiTasks.TryGetValue(job.ApiKey, out wrapper))
                    {
                        lock (apiLocker)
                        {
                            if (!apiTasks.TryGetValue(job.ApiKey, out wrapper))
                            {
                                wrapper = Utility.CreateApi(job);
                                if (wrapper == null)
                                {
                                    throw new Exception(String.Format("[{0}] is not BaseApi.", job.ApiKey));
                                }

                                wrapper = apiTasks.AddOrUpdate(job.ApiKey, wrapper, (_key, _value) =>
                                {
                                    if (_value.CrossDomain)
                                    {
                                        _value.Dispose();
                                    }
                                    return wrapper;
                                });
                            }
                        }
                    }else{
                        wrapper.ReStart();
                    }
                }
                catch (Exception ex)
                {
                    ApiContext.WriteLog("error", "Create: " + job.ApiKey, ex.ToString());
                    if (!async)
                    {
                        throw;
                    }
                    break;
                }

                try
                {
                    if (!wrapper.Api.Loaded)
                    {
                        try
                        {
                            wrapper.InitInstance();
                            var initData = new ApiInitData();
                            initData.SysSettings = new HashData<string>();

                            var taskType = job.ApiType;
                            //if (taskType == ApiTypes.Tfx)
                            //{
                            //    taskType = ApiTypes.Taobao;
                            //}
                                           
                            foreach (var item in ApiContext.Settings)
                            {
                                var _baseType = "Base.";
                                var _taskType = taskType + ".";
                                if (_baseType.Equals(_taskType, StringComparison.OrdinalIgnoreCase) || item.Key.StartsWith(_baseType, StringComparison.OrdinalIgnoreCase) || item.Key.StartsWith(_taskType, StringComparison.OrdinalIgnoreCase))
                                {
                                    initData.SysSettings[item.Key] = item.Value;
                                }
                            }
                            wrapper.Api.Init(initData);
                        }
                        catch (Exception ex)
                        {
                            ApiContext.WriteLog("error", "Init: " + job.ApiKey, ex.ToString());
                            if (!async)
                            {
                                throw;
                            }
                            break;
                        }
                    }
                    if (wrapper.StartInstance())
                    {
                        try
                        {
                            var runData = new ApiRunData();
                            runData.AppSettings = new HashData<object>();
                            runData.Job = job;
                            runData.Args = args;
                        
                            var taskData = wrapper.Api.Run(runData);
                            var apiData = taskData.Data;
                            var apiJob = apiData.Job;

                            apiJob.RunTimes = Convert.ToDecimal(Math.Round((DateTime.Now - startTime).TotalSeconds, 3));
                            if (taskData.Data.StateData != null)
                            {
                                apiJob.RunState = JsonConvert.SerializeObject(taskData.Data.StateData);
                            }

                            if (apiJob.JobId > 0 && apiData.Args.IsNullOrEmpty())
                            {
                                if (taskData.Succeed)
                                {
                                    using (var conn = new MySqlConnection(DbBase.CommConnectString))
                                    {
                                        try
                                        {
                                             string sql = @"UPDATE api_job SET 
                                                                   api_job.api_name=@api_name,
                                                                   api_job.api_author=@api_author,
                                                                   api_job.api_interval=@api_interval,
                                                                   api_job.api_timeout=@api_timeout,
                                                                   api_job.api_running=0,
                                                                   api_job.run_eof=NOW(),
                                                                   api_job.run_next = (CASE api_job.api_interval WHEN 0 THEN NULL ELSE DATE_ADD(NOW(), INTERVAL api_job.api_interval SECOND_MICROSECOND) END),
                                                                   api_job.run_times =@run_times,
                                                                   api_job.run_result =@run_result,
                                                                   api_job.run_state =@run_state,
                                                                   api_job.run_timestamp =@run_timestamp,
                                                                   api_job.run_total = api_job.run_total + @runtotal,
                                                                   api_job.err_retry = 0,
                                                                   api_job.err_code = 0 
                                                            WHERE 
                                                                   api_job.job_id =@job_id AND api_job.run_host =@run_host AND api_job.run_id =@run_id";                                                                  
                                            
                                            var rnt = conn.Execute(sql,new {
                                                job_id = apiJob.JobId,
                                                run_host = apiJob.RunHost,
                                                run_id = apiJob.RunId,
                                                api_name = apiJob.ApiName,
                                                api_author = apiJob.ApiAuthor,
                                                api_interval = apiJob.ApiInterval,
                                                api_timeout = apiJob.ApiTimeout,
                                                run_times = apiJob.RunTimes,
                                                run_result = apiJob.RunResult,
                                                run_state = apiJob.RunState,
                                                run_timestamp = apiJob.RunTimestamp,
                                                runtotal = apiJob.RunResult > 0 ? 1 : 0
                                            });                                            
                                        }
                                        catch
                                        {                                            
                                            conn.Dispose();
                                        }
                                    }

                                }
                                else{
                                    apiJob.ErrRetry++;
                                    int interval = apiJob.ApiInterval;
                                    if (apiJob.ErrRetry > 1 && apiJob.ApiInterval < 3600)
                                    {
                                        interval += apiJob.ErrRetry * apiJob.ApiInterval;
                                        if (interval > 3600)
                                        {
                                            interval = 3600;
                                        }
                                    }
                                    using (var conn = new MySqlConnection(DbBase.CommConnectString))
                                    {
                                        try
                                        {
                                            string sql = @"UPDATE api_job SET 
                                                        api_job.api_name=@api_name,
                                                        api_job.api_author=@api_author,
                                                        api_job.api_interval=@api_interval,
                                                        api_job.api_timeout=@api_timeout,
                                                        api_job.api_running=0,
                                                        api_job.run_eof=NOW(),
                                                        api_job.run_next = (CASE api_job.api_interval WHEN 0 THEN NULL ELSE DATE_ADD(NOW(), INTERVAL api_job.api_interval SECOND_MICROSECOND) END),
                                                        api_job.run_times =@run_times,
                                                        api_job.run_result =@run_result,
                                                        api_job.run_state =@run_state,
                                                        api_job.run_timestamp =@run_timestamp,
                                                        api_job.err_total = api_job.err_total + 1,
                                                        api_job.err_retry = api_job.err_retry + 1,
                                                        api_job.err_code =@err_code,
                                                        api_job.err_message =@err_message,
                                                        api_job.err_timestamp = NOW() 
                                                    WHERE 
                                                        api_job.job_id =@job_id AND api_job.run_host =@run_host AND api_job.run_id =@run_id";
                                            var rnt = conn.Execute(sql,new {
                                                    job_id = apiJob.JobId,
                                                    run_host = apiJob.RunHost,
                                                    run_id = apiJob.RunId,
                                                    api_name = apiJob.ApiName,
                                                    api_author = apiJob.ApiAuthor,
                                                    api_interval = apiJob.ApiInterval,
                                                    api_timeout = apiJob.ApiTimeout,
                                                    run_times = apiJob.RunTimes,
                                                    run_result = apiJob.RunResult,
                                                    run_state = apiJob.RunState,
                                                    run_timestamp = apiJob.RunTimestamp,
                                                    err_code = taskData.ErrCode,
                                                    err_message = taskData.ErrMessage.SubLeft(2000)
                                                });

                                        }catch{
                                            conn.Dispose();
                                        }
                                    }                                                                                                                                
                                }
                            }
                            else
                            {
                                if (!taskData.Succeed)
                                {
                                    ApiContext.WriteLog("error", "Run: " + job.ApiKey, String.Format("[{0}] {1}", taskData.ErrCode, taskData.ErrMessage).SubLeft(2000));
                                }
                            }
                            return;
                        }
                        finally
                        {
                            wrapper.EndInstance();
                            wrapper.Dispose();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // if (!(ex is ThreadAbortException))
                    // {
                    ApiContext.WriteLog("error", "Run: " + job.ApiKey, ex.ToString());
                    if (!async)
                    {
                        throw;
                    }
                    //}
                }
                break;
            }

            if (job.JobId > 0)
            {            
                using (var conn = new MySqlConnection(DbBase.CommConnectString)){
                    try{
                        var sql = @"UPDATE api_job SET 
                                        api_job.api_running=0,api_job.run_eof=NOW(),
                                        api_job.run_next=(CASE api_job.api_interval WHEN 0 THEN NULL ELSE DATE_ADD(NOW(), INTERVAL api_job.api_interval SECOND_MICROSECOND) END)  
                                    WHERE 
                                        api_job.job_id=@JobId AND api_job.run_host=@RunHost AND api_job.run_id=@RunId"; 

                        var rnt = conn.Execute(sql,job);
                    }catch(Exception ex){
                        ApiContext.WriteLog("error", "Run: " + job.ApiKey, ex.ToString());
                    }
                }           
            }
        }

        /// <summary>
        /// Task 队列
        /// </summary>
        static ConcurrentDictionary<string, TaskWrapper> apiTasks = new ConcurrentDictionary<string, TaskWrapper>(StringComparer.OrdinalIgnoreCase);
        static object apiLocker = new object();
        

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="type">日志类型,如:error</param>
        /// <param name="title">日志标题</param>
        /// <param name="body">日志内容</param>
        internal static void WriteLog(string type, string title, string body)
        {
            using (var conn = new MySqlConnection(DbBase.CommConnectString)){
                try
                {
                    var sql = @"INSERT INTO api_log(api_log.type,api_log.title,api_log.body,api_log.host,api_log.created) VALUES(@type,@title,@body,@host,@created);";
                    conn.Execute(sql,new { 
                                    type = type.SubLeft(20),
                                    title = title.SubLeft(100),
                                    body = body.SubLeft(2000),
                                    host = ApiContext.HostName,
                                    created = DateTime.Now.ToString()
                    });
                }
                catch
                { }
            }
        }


        /// <summary>
        /// 检测任务
        /// </summary>
        static void CheckTask(ApiJob job)
        {
            if (job == null)
            {
                ApiContext.WriteLog("error", "CoreWebApi.ApiTask.ApiContext.Check", "ArgumentException: ApiJob is invalid.");
                throw new ArgumentException("ApiJob is invalid.", "job");
            }

            if (job.ApiKey.IsNullOrEmpty())
            {
                ApiContext.WriteLog("error", "CoreWebApi.ApiTask.ApiContext.Check", "ArgumentException: Key is invalid.");
                throw new ArgumentException("Api: Key is invalid.", "apiKey");
            }

            if (job.CoId < 1 && job.ShopId > 0)
            {
                ApiContext.WriteLog("error", "CoreWebApi.ApiTask.ApiContext.Check", "ArgumentException:  CoId is invalid.");
                throw new ArgumentException("Api: CoId is invalid.", "coId");
            }

            if (job.ShopId > 0 && job.Shop == null)
            {
                ApiContext.WriteLog("error", "CoreWebApi.ApiTask.ApiContext.Check", "ArgumentException: Shop is invalid.");
                throw new ArgumentException("Api: Shop is invalid.", "shopSite");
            }
        }

        /// <summary>
        /// 实际运行
        /// </summary>
        static void Execute()
        {
            Thread.Sleep(15000); //延迟15秒启动定时任务处理

            var random = new Random();
            int x1 = 0, x2 = 0, x3 = 0;

            while (!TaskManager.Stoped)
            {
                Thread.Sleep(random.Next(1000, 6000));

                x2++; //注册或更新主机
                if (x2 > 4)
                {
                    ApiContext.RegisterHost();
                    x2 = 0;
                }

                x1++; //重新加载配置项
                if (x1 > 40)
                {
                    ApiContext.LoadSettings();
                    x1 = 0;
                }

                x3++; //清理运行过久的任务
                if (x3 > 400)
                {
                    ApiContext.CleanTasks();
                    x3 = 0;
                }

                try
                {
                    //var sql = String.Empty;
                    var sql2 = "";
                    var hasTasks = false;

                    if (ApiContext.HostEnabled && TaskManager.WaitingTasks < 200)
                    {
                        hasTasks = true;//线程池是否有空闲
                        //sql += Resources.SQL_JOB_SELECT;   
                        

                        if (ApiContext.TaskTimestamp == long.MinValue)
                            sql2 = @"SELECT MAX(UNIX_TIMESTAMP(api_task.ts)),@tts FROM api_task ;";
                        else
                            sql2 = @"SELECT api_task.api_key,UNIX_TIMESTAMP(api_task.ts) FROM api_task WHERE api_task.ts>FROM_UNIXTIME(@tts);";
                    }

                    if (hasTasks)
                    {
                        var keys = new List<string>();
                        var list = new List<ApiJob>(800);

                        int retry = 3;
                        while (!CacheBase.Set(ApiContext.ClusterLockerCacheKey, ApiContext.HostName, new TimeSpan(0, 0, 0, 30)))
                        {
                            retry--;
                            if (retry > 0)
                                Thread.Sleep(1000);
                            else
                                break;
                        }
                        using (var conn = new MySqlConnection(DbBase.CommConnectString)){
                            try
                            {
                                List<int> jobIds = conn.Query<int>(@"SELECT api_job.job_id FROM api_job WHERE api_job.api_running = 0 AND api_job.status = 2 AND api_job.enabled = 1 AND api_job.api_lazy = 0 ORDER BY api_job.run_next ASC limit 0, 800").AsList();
                                if (jobIds.Count>0) {                            
                                    string jobid = string.Join(",", jobIds.ToArray());                        
                                    //线程池开始空闲时获取需要执行的任务
                                    var reader = conn.Execute(string.Format(@"UPDATE api_job SET api_job.api_running=1,api_job.run_bof=NOW(),
                                        api_job.run_stop=(CASE api_job.api_timeout WHEN 0 THEN NULL ELSE DATE_ADD(NOW(),INTERVAL api_job.api_timeout SECOND_MICROSECOND) END),
                                        api_job.run_host = '{0}',api_job.run_id = '{1}' WHERE api_job.job_id IN({2});", ApiContext.HostName, DateTime.Now.Ticks, jobid.Substring(0, jobid.Length - 1)));

                                    var outreader = conn.Query<api_job>(string.Format(@"SELECT api_job.* from api_job WHERE api_job.job_id in ({0});", jobid)).AsList();
                            
                                    foreach (api_job aj in outreader) {
                                        var item = ApiJob.Create(aj, true);
                                        list.Add(item);
                                    }
                                    var nextReader = conn.ExecuteReader(sql2,new {tts = ApiContext.TaskTimestamp});

                                    if (ApiContext.TaskTimestamp == long.MinValue)
                                    {
                                        if (nextReader.Read())
                                        {
                                            ApiContext.TaskTimestamp = Convert.ToInt64(nextReader[0]);
                                        }
                                    }
                                    else
                                    {
                                        while (nextReader.Read())
                                        {
                                            var _timestamp = Convert.ToInt64(nextReader[1]);
                                            if (_timestamp > ApiContext.TaskTimestamp)
                                            {
                                                ApiContext.TaskTimestamp = _timestamp;
                                            }
                                            keys.Add(nextReader[0].ToString());
                                        }
                                    }
                                }


                            }catch(Exception ex){
                                Console.WriteLine(ex.Message);
                            }
                            finally
                            {
                                if (retry > 0)
                                {
                                    CacheBase.Remove(ApiContext.ClusterLockerCacheKey);
                                }
                            }
                        }

                        ApiContext.CleanTasks(keys);

                        if (list.Count > 0)
                        {
                            foreach (var item in list)
                            {
                                ApiContext.RunAsync(item);
                            }
                        }
                    }
                }
                // catch (ThreadAbortException ex)
                // {
                //     return;
                // }
                catch (Exception ex)
                {
                    ApiContext.WriteLog("error", "CoreWebApi.ApiTask.ApiContext.Execute", ex.ToString());
                }
            }
        }

        static void DoTimeout(ThreadItem<TaskItem> item)
        {
            if (item != null)
            {
                var job = item.Context.State as ApiJob;
                if (job != null)
                {
                    ApiContext.WriteLog("error", "ApiContext.TaskTimeout", string.Format("{0}[{1}] is timeout({2}s).", job.ApiKey, job.JobId, item.RunTimes));
                }
            }
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        static void LoadSettings()
        {
            try
            {
                if (ApiContext.Settings == null)
                {
                    ApiContext.Settings = new HashData<string>(StringComparer.OrdinalIgnoreCase);
                }

                var _settings = new HashData<string>(StringComparer.OrdinalIgnoreCase);
                
                using (var conn = new MySqlConnection(DbBase.CommConnectString)) {
                    //获取API全局配置信息
                    string SQL_SETTING_SELECT = "SELECT api_setting.id,api_setting.`value` FROM api_setting ;";
                    var reader = conn.ExecuteReader(String.Format(SQL_SETTING_SELECT));
                    while (reader.Read()) {
                        _settings[reader[0].ToString()] = reader[1].ToString();
                    }
                }
                //ApiContext.HotCompanies = SqlCommandExecutor.ExecuteListValue<int>("SELECT [co_id] FROM [Api_11_Company] WITH(NOLOCK)", ci);
                if (ApiContext.IsRunning)
                {
                    bool updated = false; //检测是否有更新
                    if (ApiContext.Settings.Count == _settings.Count)
                    {
                        string _value = null;
                        foreach (var _setting in _settings)
                        {
                            if (!ApiContext.Settings.TryGetValue(_setting.Key, out _value) || _setting.Value != _value)
                            {
                                updated = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (!_settings.IsNullOrEmpty())
                        {
                            updated = true;
                        }
                    }

                    if (updated)
                    {
                        ApiContext.Settings = _settings;
                        ApiContext.CleanTasks();
                    }
                }
                else
                {
                    ApiContext.Settings = _settings;
                }
            }
            catch (Exception ex)
            {
                ApiContext.WriteLog("error", "CoreWebApi.ApiTask.ApiContext.Load", ex.ToString());
            }
        }

        /// <summary>
        /// 注册或更新到主机列表
        /// </summary>
        static void RegisterHost()
        {
            using (var conn = new MySqlConnection(DbBase.CommConnectString)){
                try
                {   
                    //注册或更新
                    string sql =@"UPDATE api_host SET 
                                        api_host.register_total = api_host.register_total+1,
                                        api_host.register_time = @registerTime ,
                                        api_host.task_threads = @taskThreads  ,
                                        api_host.task_waiting = @taskWaiting ,
                                        api_host.task_working = @taskWorking ,
                                        api_host.task_worked = @taskWorked 
                                    WHERE 
                                        api_host.host_name =@HostName";    
                    int rtn = conn.Execute(sql,new {
                                    registerTime = DateTime.Now.ToString(),
                                    taskThreads = TaskManager.CurrentThreads,
                                    taskWaiting = TaskManager.WaitingTasks,
                                    taskWorking = TaskManager.WorkingTasks,
                                    taskWorked = TaskManager.WorkedTasks,
                                    HostName = ApiContext.HostName
                    });
                                                          
                    if (rtn == 0) {
                        sql =@"Insert api_host SET 
                                        api_host.host_name = @HostName,
                                        api_host.register_total = 1,
                                        api_host.register_time = @registerTime ,
                                        api_host.task_threads = @taskThreads  ,
                                        api_host.task_waiting = @taskWaiting ,
                                        api_host.task_working = @taskWorking ,
                                        api_host.task_worked = @taskWorked ";
                        rtn = conn.Execute(sql,new {
                                    registerTime = DateTime.Now.ToString(),
                                    taskThreads = TaskManager.CurrentThreads,
                                    taskWaiting = TaskManager.WaitingTasks,
                                    taskWorking = TaskManager.WorkingTasks,
                                    taskWorked = TaskManager.WorkedTasks,
                                    HostName = ApiContext.HostName
                        });                        
                    }
                    if (rtn > 0) {                        
                        ApiContext.HostEnabled = conn.Query<bool>("SELECT api_host.enabled FROM api_host WHERE api_host.host_name = '"+ApiContext.HostName+"'").AsList()[0];                       
                    }
                    //检测主机是否为管理员主机
                    sql = "UPDATE api_cluster SET  api_cluster.master_time = '"+DateTime.Now.ToString()+"' WHERE api_cluster.master_host  ='"+ApiContext.HostName+"';";
                    rtn = conn.Execute(sql);
                    if (rtn == 0) {
                        sql = "UPDATE api_cluster SET api_cluster.master_host = '"+ApiContext.HostName+"',  api_cluster.master_time = '"+DateTime.Now.ToString()+"' WHERE api_cluster.master_time < '"+DateTime.Now.AddSeconds(-300).ToString()+"' ;";
                        rtn = conn.Execute(sql);
                    }
                    ApiContext.IsMaster = rtn > 0;
                }
                catch (Exception ex)
                {
                    conn.Dispose();
                    ApiContext.WriteLog("error", "CoreWebApi.ApiTask.ApiContext.RegisterHost", ex.ToString());
                }
            }

            bool isCloud = false;//ConfigurationManager.AppSettings["CacheProvider"] == "aliyun";

            //只有管理主机执行任务维护工作，防止并发异常
            if (ApiContext.IsMaster)
            {
                ApiContext.RegisterTasks();

                // if (ApiContext.NotifyMgr == null)
                // {
                //     ApiContext.NotifyMgr = new NotifyCenter();
                // }
                if (isCloud)
                {
                   // TaobaoApi.StartNotify();
                }
            }
            else
            {
                if (isCloud)
                {
                   // TaobaoApi.StopNotify();
                }
            }
        }

        /// <summary>
        /// 注册或更新到任务列表
        /// </summary>
        static void RegisterTasks()
        {
            try
            {
                string SQL_TASK_REGISTER_GLOBAL = @"#--添加未创建的任务(商店)
                        INSERT INTO api_job (api_job.api_type,
                                             api_job.run_next,
                                             api_job.api_group,
                                             api_job.api_key,
                                             api_job.api_mode,
                                             api_job.api_lazy,
                                             api_job.status,
                                             api_job.enabled,
                                             api_job.co_id,
                                             api_job.shop_id,
                        api_job.run_timestamp)SELECT 
                                            T.api_type,NOW(),
                                            T.api_group,
                                            T.api_key,
                                            T.api_mode,
                                            T.api_lazy,
                                            2,
                                            S.Enable,
                                            S.CoID,
                                            S.ID,
                                            s.ShopBegin 
                        FROM api_task AS T 
                        CROSS  JOIN  shop AS S 
                        WHERE 
                                            (T.api_type=S.ShopType AND T.api_mode='S' AND T.enabled = 1 AND T.limited=0) AND S.Enable=1  
                            AND 
                                            S.Istoken = 1
                            AND 
                                            NOT EXISTS(SELECT 0 FROM api_job AS J WHERE J.api_key=T.api_key 
                            AND 
                                            J.co_id=S.CoID 
                            AND 
                                            J.shop_id=S.ID);";
                var sqlList = System.Text.RegularExpressions.Regex.Split(SQL_TASK_REGISTER_GLOBAL, "GO--");
                var sqlIndex = 0;
               
                int retry = 10;
                while (!CacheBase.Set(ApiContext.ClusterLockerCacheKey, ApiContext.HostName, new TimeSpan(0, 0, 0, 30)))
                {
                    retry--;
                    if (retry > 0)
                        Thread.Sleep(1000);
                    else
                        break;
                }
                using (var conn = new MySqlConnection(DbBase.CommConnectString)){
                    try
                    {
                        foreach (var sql in sqlList)
                        {
                            conn.Execute(sql);
                            Thread.Sleep(10);
                            sqlIndex++;
                        }                        
                    }
                    catch (Exception ex)
                    {
                        conn.Dispose();
                        ApiContext.WriteLog("error", string.Format("CoreWebApi.ApiTask.ApiContext.RegisterTasks({0})", sqlIndex), ex.ToString());
                    }
                    finally
                    {
                        if (retry > 0)
                        {                        
                            CacheBase.Remove(ApiContext.ClusterLockerCacheKey);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ApiContext.WriteLog("error", "CoreWebApi.ApiTask.ApiContext.RegisterTasks", ex.ToString());
            }
        }

        /// <summary>
        /// 店铺注册任务
        /// </summary>
        /// <param name="co_id"></param>
        /// <param name="shop_id"></param>
        /// <param name="adapter"></param>
        public static void RegisterTasks(int co_id, int shop_id)
        {
            var shopInfo = ShopHaddle.ShopQuery(co_id.ToString(),shop_id.ToString()).d as Shop;

            if (shopInfo == null || shopInfo.SitType.ToString() == ApiTypes.Base.ToString() || shopInfo.SitType.ToString() == ApiTypes.Unknow.ToString())
            {
                ApiContext.WriteLog("error", String.Format("CoreWebApi.ApiTask.ApiContext.RegisterTasks({0},{1})",co_id, shop_id), "Arguments invalid.");
            }
            else
            {
                if (Convert.ToBoolean(shopInfo.Enable))
                {
                    using (var conn = new MySqlConnection(DbBase.CommConnectString)){
                        try
                        {
                            string sql = @"SELECT api_job.job_id FROM api_job WHERE api_job.co_id=@Coid AND api_job.shop_id=@Shopid AND api_job.api_type=@apiType AND api_job.api_mode='S'";   
                            int sqlCount = conn.Query<int>(sql,new {Coid = co_id , Shopid = shop_id , apiType = shopInfo.ShopType.ToString() }).AsList()[0];                

                            if (sqlCount == 0) {
                                
                                 sql = @"INSERT INTO api_job (api_job.api_key,api_job.api_mode,api_job.api_group,api_job.api_lazy,api_job.co_id,api_job.shop_id,api_job.api_type)SELECT " +
                                            "T.api_key,T.api_mode,T.api_group,T.api_lazy,@Coid,@Shopid,@apiType FROM api_task AS T  WHERE T.api_type =@apiType AND T.api_mode = 'S' AND T.enabled = TRUE;  ";
                                            //"UPDATE api_job SET api_job.enabled = 1 WHERE api_job.co_id =@0 AND api_job.shop_id =@1 AND api_job.api_type =@2 AND api_job.api_mode = 'S' AND api_job.api_group = 'Item'; ";          
                                conn.Execute(sql,new {Coid = co_id , Shopid = shop_id , apiType = shopInfo.ShopType.ToString() });
                            }
                        }
                        catch (Exception ex)
                        {
                            conn.Dispose();
                            ApiContext.WriteLog("error", String.Format("CoreWebApi.ApiTask.ApiContext.RegisterTasks({0},{1})",co_id, shop_id), ex.ToString());
                        }                       
                    }
                }
            }
        }

        /// <summary>
        /// 店铺注销任务
        /// </summary>
        /// <param name="co_id"></param>
        /// <param name="shop_id"></param>
        /// <param name="adapter"></param>
        public static void UnregisterTasks(int co_id, int shop_id)
        {
            if (co_id > 0 && shop_id > 0)
            {
                using (var conn = new MySqlConnection(DbBase.CommConnectString)){
                    try
                    {       
                        conn.Execute("DELETE from api_job  WHERE api_job.co_id='"+co_id+"' AND api_job.shop_id="+shop_id);             
                    }
                    catch (Exception ex)
                    {
                        conn.Dispose();
                        ApiContext.WriteLog("error", String.Format("CoreWebApi.ApiTask.ApiContext.UnregisterTasks({0},{1})", co_id, shop_id), ex.ToString());
                    }                
                }
            }
        }

        /// <summary>
        /// 清理定时任务程序
        /// </summary>
        static void CleanTasks()
        {
            try
            {
                IList<string> keys = null;
                lock (apiLocker)
                {
                    keys = apiTasks.Keys.ToArray();
                }

                ApiContext.CleanTasks(keys);
            }
            catch
            { }
            finally
            {
                GC.Collect();
            }
        }

        /// <summary>
        /// 清理指定的任务程序
        /// </summary>
        /// <param name="keys"></param>
        static void CleanTasks(IList<string> keys)
        {
            if (!keys.IsNullOrEmpty())
            {
                foreach (var key in keys)
                {
                    TaskWrapper wrapper;
                    if (ApiContext.apiTasks.TryGetValue(key, out wrapper) && wrapper != null)
                    {
                        if (wrapper.CrossDomain)
                        {
                            if (ApiContext.apiTasks.TryRemove(key, out wrapper))
                            {
                                wrapper.Dispose();
                            }
                        }
                        else
                        {
                            wrapper.Reload();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取空闲可用的数据库
        /// </summary>
        /// <param name="type"></param>
        /// <param name="db_id">排除的数据库</param>
        /// <returns></returns>
        public static string GetFreeDatabase(ApiTypes type, string db_id = null)
        {
            using (var conn = new MySqlConnection(DbBase.CommConnectString)){
                try{                
                    if (db_id.IsNullOrEmpty()) { 
                            db_id = conn.Query<string>("SELECT  api_db.db_id FROM api_db WHERE api_db.db_type='"+type.ToString()+"'  ORDER BY api_db.shop_count ASC LIMIT 0,1").AsList()[0];
                    }
                    else {                     
                            db_id = conn.Query<string>("SELECT api_db.db_id FROM api_db  WHERE api_db.db_type='"+type.ToString()+"' AND api_db.db_id<> "+db_id+" ORDER BY api_db.shop_count ASC LIMIT 0,1")                            
                                .AsList()[0];
                    }
                }catch{
                    conn.Dispose();
                }
            }

            if (db_id.IsNullOrEmpty())
            {
                throw new Exception("No database available."); 
            }
            return db_id;
        }

        /// <summary>
        /// 更新任务延迟值
        /// </summary>
        /// <param name="job"></param>
        public static void UpdateLazy(ApiJob job)
        {
            if (job != null && job.CoId > 0 && job.ShopId > 0 && job.RunTotal == 0 && job.RunResult > 0)
            {
                using (var conn = new MySqlConnection(DbBase.CommConnectString)){
                    try
                    {
                        var sql = @"UPDATE api_job SET api_job.api_lazy=(api_job.api_lazy-1) WHERE api_job.co_id=@coid AND api_job.shop_id=@shopid AND api_job.api_type=@apiType AND api_job.api_lazy>@3";
                        conn.Execute(sql,new {
                                    coid = job.CoId,
                                    shopid = job.ApiType.ToString(),
                                    apiType = job.ApiLazy
                        });                    
                    }
                    catch (Exception ex)
                    {
                        conn.Dispose();
                        ApiContext.WriteLog("error", String.Format("CoreWebApi.ApiTask.ApiContext.UpdateLazy()"), ex.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 启用或禁用指定商店的相关接口
        /// </summary>
        /// <param name="co_id">商家编号</param>
        /// <param name="shop_id">商店编号</param>
        /// <param name="tfx_enabled">启用分销</param>
        /// <param name="is_order_get">下载订单</param>
        /// <param name="is_refund_get">下载售后单</param>
        /// <param name="is_inventory_set">上传库存</param>
        /// <param name="is_shipping_set">上传快递单</param>
        public static void UpdateEnabled(int co_id, int shop_id, bool? tfx_enabled, bool? is_order_get = null, bool? is_refund_get = null, bool? is_inventory_set = null, bool? is_shipping_set = null)
        {
            CoreWebApi.ApiTask.ApiContext.RegisterTasks(co_id, shop_id);

            var sqlList = new List<string>();
            if (tfx_enabled.HasValue)
                sqlList.Add(String.Format(@"UPDATE api_job SET api_job.enabled={0} WHERE api_job.shop_id=@shopId AND api_job.api_mode='S' AND api_job.enabled={1} AND api_job.api_type='Tfx';", tfx_enabled.Value ? 1 : 0, tfx_enabled.Value ? 0 : 1));

            if (is_order_get.HasValue)
                sqlList.Add(String.Format(@"UPDATE api_job SET api_job.enabled={0} WHERE api_job.shop_id=@shopId AND api_job.api_mode='S' AND api_job.enabled={1} AND api_job.api_group='Order';", is_order_get.Value ? 1 : 0, is_order_get.Value ? 0 : 1));

            if (is_refund_get.HasValue)
                sqlList.Add(String.Format(@"UPDATE api_job SET api_job.enabled={0} WHERE api_job.shop_id=@shopId AND api_job.api_mode='S' AND api_job.enabled={1} AND api_job.api_group='Refund';", is_refund_get.Value ? 1 : 0, is_refund_get.Value ? 0 : 1));

            if (is_inventory_set.HasValue)
                sqlList.Add(String.Format(@"UPDATE api_job SET api_job.enabled={0} WHERE api_job.shop_id=@shopId AND api_job.api_mode='S' AND api_job.enabled={1} AND api_job.api_group='Inventory';", is_inventory_set.Value ? 1 : 0, is_inventory_set.Value ? 0 : 1));

            if (is_shipping_set.HasValue)
                sqlList.Add(String.Format(@"UPDATE api_job SET api_job.enabled={0} WHERE api_job.shop_id=@shopId AND api_job.api_mode='S' AND api_job.enabled={1} AND api_job.api_group='Shipping';", is_shipping_set.Value ? 1 : 0, is_shipping_set.Value ? 0 : 1));

            if (sqlList.Count > 0)
            {
                using (var conn = new MySqlConnection(DbBase.CommConnectString)){
                    try{
                        conn.Execute(String.Join("\r\n", sqlList),new { shopId= shop_id });
                    }catch{
                        conn.Dispose();
                    }
                }                
            }
        }

        /// <summary>
        /// 永久停止并释放任务管理器，适用于应用程序退出时执行。
        /// <para>注意：执行此操作后，将不能再继续执行其它操作。</para>
        /// </summary>
        public static void Stop()
        {
            TaskManager.Stop();
        }
    } 
}
