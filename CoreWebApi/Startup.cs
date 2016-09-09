
using CoreWebApi.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoreWebApi
{
    
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //Cors设置 AllowAny 允许全部CorsOptions
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyCors",
                    builder =>
                    builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
            });

            // Add memory cache services
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();

            // Add session related services.
            services.AddSession();

            // Add the system clock service
            services.AddSingleton<ISystemClock, SystemClock>();

            // services.AddAuthentication();
            services.AddAuthorization();

            services.AddMvcCore(config => 
            {
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            })
                     .AddJsonFormatters();

        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Debug);
            // Configure Session.
            app.UseSession();

            // Add static files to the request pipeline
            app.UseStaticFiles();

            app.UseCors("AllowAnyCors");

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationScheme = "CoreInstance",

                LoginPath = "/Account/Unauthorized/",
                AccessDeniedPath = new PathString("/sign/false/"),
                AutomaticAuthenticate = false,
                AutomaticChallenge = true
            });

            app.UseMvc();
        }
    }
}