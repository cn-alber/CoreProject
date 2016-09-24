
using Microsoft.Extensions.Options;

namespace CoreWebApi.Cache.Redis
{
    /// <summary>
    /// Configuration options for <see cref="RedisCache"/>.
    /// </summary>
    public class RedisCacheOptions:IOptions<RedisCacheOptions>
    {
       /// <summary>
        /// 别名
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// 默认数据库
        /// </summary>
        public int DataBase
        {
            get;
            set;
        }
        /// <summary>
        /// 主机列表
        /// </summary>
        public string Hosts
        {
            get;
            set;
        }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get;
            set;
        }
        /// <summary>
        /// 缓存默认过期时间
        /// </summary>
        public int CacheExpireMinutes
        {
            get;
            set;
        }

        RedisCacheOptions IOptions<RedisCacheOptions>.Value
        {
            get { return this; }
        }
    }
}