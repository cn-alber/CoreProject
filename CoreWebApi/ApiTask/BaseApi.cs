using System;

namespace CoreWebApi.ApiTask
{
    /// <summary>
    /// API 基础类
    /// </summary>
    public abstract class BaseApi //: MarshalByRefObject
    {

        /// <summary>
        /// 逗号分隔符
        /// </summary>
        public readonly static char[] COMMA_SPLITER = new char[] { ',', '，' };

        /// <summary>
        /// 冒号分隔符
        /// </summary>
        public readonly static char[] COLON_SPLITER = new char[] { ':', '：' };

        /// <summary>
        /// 分号分割符
        /// </summary>
        public readonly static char[] SEMICOLON_SPLITER = new char[] { ';', '；' };


        /// <summary>
        /// 配置键: 根应用程序目录
        /// </summary>
        public const string BASE_APP_DIRECTORY = "@Base.AppDirectory";

        /// <summary>
        /// 是否已初始化
        /// </summary>
        public bool Loaded
        {
            get;
            protected set;
        }

        private object locker = new object();


        /// <summary>
        /// 系统配置项
        /// </summary>
        protected HashData<string> SysSettings { get; set; }

        /// <summary>
        /// 应用配置文件
        /// </summary>
        public XmlConfig AppSettings { get; set; }

        /// <summary>
        /// 当前应用目录
        /// </summary>
        public string AppDirectory { get; private set; }


        /// <summary>
        /// API 类型 (必须重写)
        /// </summary>
        public abstract int Type
        {
            get;
        }

        /// <summary>
        /// API 友好名称 (必须重写)
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// API 开发人员 (必须重写)
        /// </summary>
        public abstract string Author
        {
            get;
        }

        /// <summary>
        /// API 超时时间[秒] (必须重写)
        /// </summary>
        public abstract int Timeout
        {
            get;
        }

        /// <summary>
        /// API 间隔时间[秒] (必须重写)
        /// </summary>
        public abstract int Interval
        {
            get;
        }




        /// <summary>
        /// 执行具体的业务逻辑 (必须重写)
        /// </summary>
        /// <param name="apiData"></param>
        protected abstract void Execute(ApiRunData apiData);

        /// <summary>
        /// 重新进行初始化
        /// </summary>
        internal void Reload()
        {
            this.Loaded = false;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data">初始化数据</param>
        public virtual void Init(ApiInitData data)
        {
            if (!this.Loaded)
            {
                lock (this.locker)
                {
                    if (!this.Loaded)
                    {                        
                        this.SysSettings = data.SysSettings;
                        this.Loaded = true;
                    }
                }
            }
        }

        /// <summary>
        /// 运行任务
        /// </summary>
        /// <param name="data">运行数据</param>
        /// <returns></returns>
        public ApiTaskData Run(ApiRunData data)
        {
            var result = new ApiTaskData();
            try
            {
                if ((int)data.Job.ApiType != this.Type)
                {
                    throw new TypeAccessException("Api type is not match.");
                }

                result.Data = data;
                data.Job.ApiName = this.Name;
                data.Job.ApiAuthor = this.Author;
                data.Job.ApiInterval = this.Interval;
                data.Job.ApiTimeout = this.Timeout;
                this.Reload();
                this.Execute(data);

                result.Succeed = true;
            }
            catch (ApiTaskException ex)
            {
                result.ErrCode = ex.Code;
                result.ErrMessage = ex.ToString();
                if (ex.ToString().IndexOf("重新授权") == -1) {//非授权错误接口报错,Mail给指定开发
                    //CoreWebApi.ApiTask.MailService.SendingMail("Api报错：" + ex.Code, ex.ToString(),this.Author);
                }
                
            }
            catch (Exception ex)
            {
                result.ErrCode = 500;
                result.ErrMessage = String.Format("[{0}]\r\n{1}", ex.Source, ex.ToString());
            }
            return result;
        }


         
    }
}
