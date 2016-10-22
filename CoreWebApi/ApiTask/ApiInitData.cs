using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace CoreWebApi.ApiTask
{
    /// <summary>
    /// 初始化数据
    /// </summary>
   
    public class ApiInitData //: MarshalByRefObject
    {
        /// <summary>
        /// 系统配置项
        /// </summary>
        public HashData<string> SysSettings { get; set; }


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