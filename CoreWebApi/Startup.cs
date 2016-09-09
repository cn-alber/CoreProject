
using CoreWebApi.Components;
using CoreWebApi.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoreWebApi
{
    
    public class Startup
    {

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder();
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        public IConfiguration Configuration { get; set; }
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

            services.AddMvc(config => 
            {
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            })
                     ;
            // services.add();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Debug);

            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch
                {
                     if (context.Response.HasStarted)
                     {
                         throw;
                     }
                     context.Response.StatusCode = 200;
                     await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(new ResponseResult(100,null,"接口请求异常")));
                }
            }

            );

            // Configure Session.
            app.UseSession();

            // Add static files to the request pipeline
            app.UseStaticFiles();

            app.UseCors("AllowAnyCors");

            // var ss = new CookieAuthenticationOptions();
            // ss.

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationScheme = "CoreInstance",

                // LoginPath = new PathString("/sign/false/"),
                // AccessDeniedPath = new PathString("/sign/false"),
                AutomaticAuthenticate = true,
                AutomaticChallenge = false
            });
            
            app.UseSimpleBearerAuthentication(new SimpleBearerOptions
            {
                AuthenticationScheme = "Bearer",
                AutomaticAuthenticate = false
            });

            app.UseMvc();
        }
    }
}