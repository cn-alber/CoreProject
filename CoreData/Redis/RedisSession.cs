using System;

namespace CoreData.Redis
{
    public class RedisSession
    {
        private ICache Cache;
        public RedisSession()
        {

            Cache = new Redis();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public RedisSession(string host = "", string port = "", string pwd = "")
        {
            Cache = new Redis(host, port, pwd);
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            return Cache.Remove(key);
        }
        /// <summary>
        /// 新增/覆盖 缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Set<T>(string key, T value)
        {
            return Cache.Set(key, value);
        }
        /// <summary>
        /// 新增/覆盖 缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresIn"></param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, TimeSpan expiresIn)
        {
            return Cache.Set(key, value, expiresIn);
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return Cache.Get<T>(key);
        }

        public long Increment(string key)
        {
            return Cache.Increment(key);
        }
    }
}