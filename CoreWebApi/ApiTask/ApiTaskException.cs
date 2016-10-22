using System;

namespace CoreWebApi.ApiTask
{
    /// <summary>
    /// 任务异常类
    /// </summary>
   
    public class ApiTaskException : Exception
    {
        /// <summary>
        /// 错误编码
        /// </summary>
        protected int ErrorCode { get; private set; }

        /// <summary>
        /// 错误编码
        /// </summary>
        public int Code
        {
            get
            {
                return this.ErrorCode;
            }
        }

        /// <summary>
        /// 错误描述
        /// </summary>
        protected string ErrorMessage { get; private set; }

        /// <summary>
        /// 错误描述
        /// </summary>
        public override string Message
        {
            get
            {
                if (base.Message == null || base.Message == this.ErrorMessage)
                    return this.ErrorMessage;
                else
                    return this.ErrorMessage + Environment.NewLine + base.Message;
            }
        }


        /// <summary>
        /// 实例化异常
        /// </summary>
        /// <param name="errorCode">错误编码(600+)</param>
        /// <param name="errorMessage">错误描述</param>
        public ApiTaskException(int errorCode, string errorMessage)
            : base(errorMessage)
        {
            this.ErrorCode = errorCode;
            this.ErrorMessage = errorMessage;
        }

        /// <summary>
        /// 实例化异常
        /// </summary>
        /// <param name="errorCode">错误编码(600+)</param>
        /// <param name="errorMessage">错误描述</param>
        /// <param name="innerException">内部异常</param>
        public ApiTaskException(int errorCode, string errorMessage, Exception innerException)
            : base(innerException.Message, innerException)
        {
            this.ErrorCode = errorCode;
            this.ErrorMessage = errorMessage;
        }

        // /// <summary>
        // /// GetObjectData
        // /// </summary>
        // /// <param name="info"></param>
        // /// <param name="context"></param>
        // public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        // {
        //     base.GetObjectData(info, context);
        // }
    }
}