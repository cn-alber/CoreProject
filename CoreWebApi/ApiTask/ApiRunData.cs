using System;
using Newtonsoft.Json;

namespace CoreWebApi.ApiTask
{
    /// <summary>
    /// API 运行状态数据
    /// </summary>

    public class ApiRunData //: MarshalByRefObject
    {
        /// <summary>
        /// 应用配置项
        /// </summary>
        public HashData<object> AppSettings { get; set; }

        /// <summary>
        /// 是否首次运行
        /// </summary>
        public bool IsNew
        {
            get
            {
                return (this.Job != null && this.Job.RunTotal == 0);
            }
        }

        /// <summary>
        /// API任务项
        /// </summary>
        public ApiJob Job { get; set; }

        /// <summary>
        /// 自定义参数
        /// </summary>
        public object[] Args { get; set; }


        /// <summary>
        /// 获取或更新状态数据
        /// </summary>
        public dynamic StateData
        {
            get
            {
                if (_stateData == null)
                {
                    if (this.Job != null && !this.Job.RunState.IsNullOrEmpty())
                    {
                        _stateData = JsonConvert.DeserializeObject<dynamic>(this.Job.RunState);
                    }

                    if (_stateData == null)
                    {
                        _stateData = new object();
                    }
                }
                return _stateData;
            }
        }
        private object _stateData;

        /// <summary>
        /// 获取指定类型的状态数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetStateData<T>(string key)
        {
            return GetStateData(key, default(T));
        }

        /// <summary>
        /// 获取指定类型的状态数据; 如果未找到,则使用默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetStateData<T>(string key, T defaultValue)
        {
            object value = this.StateData[key];

            if (value != null)
            {
                if (value is decimal)
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                return (T)value;
            }
            return defaultValue;
        }

        /// <summary>
        /// 设置指定键的状态数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetStateData(string key, object value)
        {
            this.StateData[key] = value;

            if (value != null)
                return this.StateData.ContainsKey(key);
            else
                return !this.StateData.ContainsKey(key);
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