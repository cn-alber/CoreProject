using System;
using System.Collections.Generic;

namespace CoreWebApi.ApiTask
{
    /// <summary>
    /// API 任务运行结果
    /// </summary>

    public class ApiTaskData //: MarshalByRefObject
    {
        /// <summary>
        /// 成功标识
        /// </summary>
        public bool Succeed { get; set; }

        /// <summary>
        /// 错误代码
        /// </summary>
        public int ErrCode { get; set; }

        /// <summary>
        /// 错误描述
        /// </summary>
        public string ErrMessage { get; set; }


        /// <summary>
        /// 查询结果数据
        /// </summary>
        public ApiRunData Data { get; internal set; }


        /// <summary>
        /// 实例化
        /// </summary>
        public ApiTaskData()
        {
            this.Succeed = false;
            this.ErrCode = 0;
        }


        // /// <summary>
        // /// 重写Remoting对象生存期为无限制
        // /// </summary>
        // /// <returns></returns>
        // public override object InitializeLifetimeService()
        // {
        //     return null;
        // }
    }
}