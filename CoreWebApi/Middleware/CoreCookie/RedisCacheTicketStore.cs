using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
// using Microsoft.Extensions.Caching.Memory;
// using Microsoft.Extensions.Caching.Distributed;
// using Microsoft.Extensions.Caching.Redis;
using CoreWebApi.Cache.Redis;

namespace CoreWebApi.Middleware
{
    public class RedisCacheTicketStore : IRedisCacheTicketStore
    {
        private const string KeyPrefix = "AuthSessionStore-";
        private RedisCache _cache;

        public RedisCacheTicketStore()
        {
            _cache = new RedisCache(new RedisCacheOptions{
                
                Hosts = "114.55.11.89:6379",
                Password = "Core"
            });
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var guid = Guid.NewGuid();
            var key = KeyPrefix + guid.ToString();
            await RenewAsync(key, ticket);
            return key;
        }

        public Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            // var options = new DistributedCacheEntryOptions();
            // var expiresUtc = ticket.Properties.ExpiresUtc;
            // if (expiresUtc.HasValue)
            // {
            //     options.SetAbsoluteExpiration(expiresUtc.Value);
            // }
            // options.SetSlidingExpiration(TimeSpan.FromHours(1)); // TODO: configurable.

            _cache.Set(key, ticket, TimeSpan.FromHours(4));

            return Task.FromResult(0);
        }

        public Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            AuthenticationTicket ticket = null;
            _cache.TryGetValue(key, out ticket);
            return Task.FromResult(ticket);
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.FromResult(0);
        }
    }
}