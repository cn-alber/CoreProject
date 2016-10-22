using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace CoreWebApi.ApiTask
{
    /// <summary>
    /// 辅助泛型集合
    /// </summary>

    public class HashData<T> : Dictionary<string, T>
    {
        /// <summary>
        /// 实例化
        /// </summary>
        public HashData() : base() { }

        /// <summary>
        /// 实例化
        /// </summary>
        public HashData(IEqualityComparer<string> comparer) : base(comparer) { }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        //protected HashData(SerializationInfo info, StreamingContext context) : base(info, context) { }


        /// <summary>
        /// 获取或设置对应的值
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns></returns>
        public new T this[string key]
        {
            get
            {
                if (key == null)
                    return default(T);

                T value;
                if (this.TryGetValue(key, out value))
                    return value;
                else
                    return default(T);
            }

            set
            {
                if (value == null)
                    base.Remove(key);
                else
                    base[key] = value;
            }
        }


        /// <summary>
        /// 序列化 HashData
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Serialize(HashData<T> data)
        {
            if (data == null || data.Count == 0)
                return null; //(new byte[0]);

            return  Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
        }


        /// <summary>
        /// 反序列化 HashData
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static HashData<T> Deserialize(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
                return new HashData<T>();
            var data = JsonConvert.DeserializeObject<dynamic>(Encoding.UTF8.GetString(buffer));        
            if (data == null)
                return new HashData<T>();
            else
                return data;
        }
    }
}