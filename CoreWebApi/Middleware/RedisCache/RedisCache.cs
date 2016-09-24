using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace CoreWebApi.Cache.Redis
{
    public class RedisCache
    {

        // private ConnectionMultiplexer _connection;

        private readonly RedisCacheOptions _options;

        public RedisCache(IOptions<RedisCacheOptions> redisOptions)
        {
            if (redisOptions == null)
            {
                throw new ArgumentNullException(nameof(redisOptions));
            }

            _options = redisOptions.Value;
        }

        static ConcurrentDictionary<string, ConnectionMultiplexer> _multiplexers = new ConcurrentDictionary<string, ConnectionMultiplexer>();
        // <summary>
        /// 获取client
        /// </summary>
        /// <returns></returns>
        private static ConnectionMultiplexer GetMultiplexer(RedisCacheOptions config)
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
                return GetMultiplexer(_options); ;
            }
        }

        private IDatabase Cache
        {
            get
            {
                return conn.GetDatabase(_options.DataBase);
            }
        }
        private readonly IDataSerializer<AuthenticationTicket> _ticketSerializer = TicketSerializer.Default;
        public Task Set(string key, AuthenticationTicket value, TimeSpan expiresIn)
        {

            return Cache.StringSetAsync(key, _ticketSerializer.Serialize(value), expiresIn);
            // return Cache.KeyRestoreAsync(key, _ticketSerializer.Serialize(value), expiresIn);
        }

        public Task TryGetValue(string key, out AuthenticationTicket ticket)
        {
            var result = Cache.StringGet(key);

            if (result != RedisValue.Null)
            {
                ticket = _ticketSerializer.Deserialize(result);
            }
            else
            {
                ticket = null;
            }
            return Task.FromResult(0);
        }

        public Task Remove(string key)
        {
            return Cache.KeyDeleteAsync(key);
            // return Task.FromResult(0);
        }
    }
}