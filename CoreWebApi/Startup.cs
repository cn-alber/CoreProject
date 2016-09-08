
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoreWebApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                    .AddJsonFormatters();
            //Cors设置 AllowAny 允许全部CorsOptions
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyCors",
                    builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            services.AddAuthentication();
        }
       
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Debug);
            app.UseCors("AllowAnyCors");
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationScheme = "CoreInstance",
                // LoginPath = new PathString("/Account/Unauthorized/"),
                // AccessDeniedPath = new PathString("/Account/Forbidden/"),
                AutomaticAuthenticate = true
                // AutomaticChallenge = true
            });
            app.UseMvc();
        }
    }
}