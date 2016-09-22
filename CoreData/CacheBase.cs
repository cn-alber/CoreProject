using System;

namespace CoreData
{
    /// <summary>
    /// 可以根据情况使用不同的缓存机制。这里也可以做成反射+缓存机制来动态配置使用哪种缓存机制。
    /// </summary>
    public static class CacheBase
    {
        public static Redis.RedisSession noSql = new Redis.RedisSession("2fabab704b8e46c1.redis.rds.aliyuncs.com","6379","S13814987627s");
        public static bool Remove(string key)
        {
            return noSql.Remove(key);
        }
        public static bool Set<T>(string key, T value)
        {
            return noSql.Set(key, value);
        }
        public static bool Set<T>(string key, T value, TimeSpan expiresIn)
        {
            return noSql.Set(key, value, expiresIn);
        }
        public static T Get<T>(string key)
        {
            return noSql.Get<T>(key);
        }
    }
}