using System;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CoreWebApi.Middleware
{
    public class CusCookieAuthenticationMiddleware : AuthenticationMiddleware<CusCookieAuthenticationOptions>
    {
        public CusCookieAuthenticationMiddleware(
            RequestDelegate next,
            IDataProtectionProvider dataProtectionProvider,
            ILoggerFactory loggerFactory,
            UrlEncoder urlEncoder,
            IOptions<CusCookieAuthenticationOptions> options)
            : base(next, options, loggerFactory, urlEncoder)
        {
            if (dataProtectionProvider == null)
            {
                throw new ArgumentNullException(nameof(dataProtectionProvider));
            }

            if (Options.Events == null)
            {
                Options.Events = new CusCookieAuthenticationEvents();
            }
            if (String.IsNullOrEmpty(Options.CookieName))
            {
                Options.CookieName = CusCookieAuthenticationDefaults.CookiePrefix + Options.AuthenticationScheme;
            }
            if (Options.TicketDataFormat == null)
            {
                var provider = Options.DataProtectionProvider ?? dataProtectionProvider;
                var dataProtector = provider.CreateProtector(typeof(CusCookieAuthenticationMiddleware).FullName, Options.AuthenticationScheme, "v2");
                Options.TicketDataFormat = new TicketDataFormat(dataProtector);
            }
            if (Options.CookieManager == null)
            {
                Options.CookieManager = new ChunkingCookieManager();
            }
            // if (!Options.LoginPath.HasValue)
            // {
            //     Options.LoginPath = CookieAuthenticationDefaults.LoginPath;
            // }
            // if (!Options.LogoutPath.HasValue)
            // {
            //     Options.LogoutPath = CookieAuthenticationDefaults.LogoutPath;
            // }
            // if (!Options.AccessDeniedPath.HasValue)
            // {
            //     Options.AccessDeniedPath = CookieAuthenticationDefaults.AccessDeniedPath;
            // }
        }

        protected override AuthenticationHandler<CusCookieAuthenticationOptions> CreateHandler()
        {
            return new CusCookieAuthenticationHandler();
        }
    }
}