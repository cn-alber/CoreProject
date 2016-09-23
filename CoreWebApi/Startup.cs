using CoreWebApi.Middleware;
using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
// using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace CoreWebApi
{
    
    public class Startup
    {

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            builder.AddEnvironmentVariables();
            // if(env.IsDevelopment())
            // {
            //     builder.()
            // }
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

            // services.AddAuthentication();
            services.AddAuthorization();

            // Configuration.GetSection("AppSettings");
            // services.AddOptions();
            // services.Configure<BasicCode>(Configuration);
            // services.Configure<BasicCode>(Configuration.GetSection("BasicCode"));

            services.AddMvc(config => 
            {
                var policy = new AuthorizationPolicyBuilder("CoreInstance")
                                 .RequireAuthenticatedUser()
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            }).AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver()); //json返回默认使用model字段，避免小写：Result -> result
            // services.AddMvc();
            // services.add();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Debug);

            // app.Use(async (context, next) =>
            // {
            //     try
            //     {
            //         await next();
            //     }
            //     catch
            //     {
            //          if (context.Response.HasStarted)
            //          {
            //              throw;
            //          }
            //          context.Response.StatusCode = 200;
            //          await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(new ResponseResult(100,null,"参数不符")));
            //     }
            // }

            // );

            // Configure Session.
            app.UseSession();

            // Add static files to the request pipeline
            app.UseStaticFiles();

            app.UseCors("AllowAnyCors");

            // var ss = new CookieAuthenticationOptions();
            // ss.

            app.UseCusCookieAuthentication(new CusCookieAuthenticationOptions()
            {
                AuthenticationScheme = "CoreInstance",
                AutomaticAuthenticate = false,
                AutomaticChallenge = false,
                SessionStore = new MemoryCacheTicketStore()
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