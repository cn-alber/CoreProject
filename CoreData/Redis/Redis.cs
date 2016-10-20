using System;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace CoreData.Redis
{
    public class Redis : ICache
    {
        private static RedisConfig _conf;
        /// <summary>
        /// 
        /// </summary>
        public static RedisConfig Conf
        {
            get
            {
                return _conf;
            }
            set { _conf = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Redis()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public Redis(string host, string port, string pwd)
        {
            Conf = new RedisConfig()
            {
                Hosts = host
                + ":" +
                port,
                Password = pwd
            };
        }

        static ConcurrentDictionary<string, ConnectionMultiplexer> _multiplexers = new ConcurrentDictionary<string, ConnectionMultiplexer>();
        /// <summary>
        /// 获取client
        /// </summary>
        /// <returns></returns>
        private static ConnectionMultiplexer GetMultiplexer(RedisConfig config)
        {
            ConnectionMultiplexer multiplexer = null;
            if (!_multiplexers.TryGetValue(config.Hosts, out multiplexer))
            {
                var redisconf = ConfigurationOptions.Parse(config.Hosts);
                redisconf.AllowAdmin = true;
                redisconf.ConnectRetry = 2;
                redisconf.Password = config.Password;
                multiplexer = ConnectionMultiplexer.Connect(redisconf);
                _multiplexers.TryAdd(config.Hosts, multiplexer);
            }
            return multiplexer;
        }
        /// <summary>
        /// redis连接端
        /// </summary>
        public ConnectionMultiplexer conn
        {
            get
            {
                return GetMultiplexer(Conf); ;
            }
        }
        public IDatabase Cache
        {
            get
            {
                return conn.GetDatabase(Conf.DataBase);
            }
        }
        public bool Remove(string key)
        {
            return Cache.KeyDelete(key.ToLower());
        }
        public bool Set<T>(string key, T value)
        {
            return Cache.StringSet(key.ToLower(), JsonConvert.SerializeObject(value));
        }
        public bool Set<T>(string key, T value, TimeSpan expiresIn)
        {
            return Cache.StringSet(key.ToLower(), JsonConvert.SerializeObject(value), expiresIn);
        }
        public T Get<T>(string key)
        {
            var result = Cache.StringGet(key.ToLower());

            if (result != RedisValue.Null)
            {
                return JsonConvert.DeserializeObject<T>(result);
            }
            return default(T);
        }

        public long Increment(string key)
        {
            return Cache.StringIncrement(key);
        }
    }
}