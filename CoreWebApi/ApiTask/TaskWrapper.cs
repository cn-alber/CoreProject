using System;
using System.Runtime.Loader;
using System.Threading;


namespace CoreWebApi.ApiTask
{
    /// <summary>
    /// 封装了API任务调用
    /// </summary>
    public class TaskWrapper: IDisposable
    {
        /// <summary>
        /// AppDomain
        /// </summary>
        public AssemblyLoadContext AppDomain
        {
            get;
            private set;
        }

        /// <summary>
        /// CrossDomain
        /// </summary>
        public bool CrossDomain
        {
            get;
            private set;
        }

        /// <summary>
        /// BaseApi
        /// </summary>
        public BaseApi Api
        {
            get;
            private set;
        }


        internal TaskWrapper(BaseApi api)
        {
            this.AppDomain = null;
            this.Api = api;
            this.CrossDomain = false;
        }

        internal TaskWrapper(AssemblyLoadContext domain, BaseApi api)
        {            
            this.AppDomain = domain;
            this.Api = api;
            this.CrossDomain = (domain != null && domain != AssemblyLoadContext.Default);
        }


        private long _instances = 0; //实例数
        

        internal long GetInstances()
        {
            return Interlocked.Read(ref this._instances);
        }

        internal void InitInstance()
        {
            Interlocked.Exchange(ref this._instances, 0);
        }

        internal bool StartInstance()
        {
            if (this.closing)
            {
                return false;
            }

            Interlocked.Increment(ref this._instances);
            return true;
        }

        internal void EndInstance()
        {
            Interlocked.Decrement(ref this._instances);
        }

        internal void ReStart(){
            this.closing = false;
        }

        /// <summary>
        /// 重新进行初始化
        /// </summary>
        internal void Reload()
        {
            if (this.Api != null)
            {
                this.Api.Reload();
            }
        }

        /// <summary>
        /// 安全清除资源
        /// </summary>
        private void Clear()
        {
            try
            {
                while (this.GetInstances() > 0)
                {
                    Thread.Sleep(100);
                }

                if (this.CrossDomain && this.AppDomain != null)
                {
                    //AppDomain.();
                    
                    this.AppDomain = null;
                }
            }
            catch
            { }
        }


        /// <summary>
        /// 释放任务资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.closing = true;
            if (!disposed)
            {
                if (disposing)
                {
                    // Release managed resources
                    this.Clear();
                }

                // Release unmanaged resources
                disposed = true;
            }
        }

        ~TaskWrapper()
        {
            Dispose(false);
        }

        private bool closing = false;
        private bool disposed = false;
    }
}