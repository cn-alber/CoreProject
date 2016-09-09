
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;

namespace CoreWebApi.Middleware
{
    // DON'T DO THIS. IT MAKES ME CRY.
    public class SimpleBearerHandler : AuthenticationHandler<SimpleBearerOptions>
    {
        public SimpleBearerHandler()
        {
        }

        protected override Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
        {
            Response.StatusCode = 200;
            Response.Headers["WWW-Authenticate"] = "Bearer";
            var msg = Newtonsoft.Json.JsonConvert.SerializeObject(new ResponseResult(100,null,"未授权"));
            var b = System.Text.Encoding.UTF8.GetBytes(msg);
            Response.Body.Write(b,0, b.Length);
            return Task.FromResult(true);
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Request.Headers.ContainsKey("Authorization"))
            {
                var header = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(header) || !header.StartsWith("Bearer "))
                {
                    var msg = Newtonsoft.Json.JsonConvert.SerializeObject(new ResponseResult(100, null, "接口请求异常"));
                    return Task.FromResult(AuthenticateResult.Fail(msg));
                }

                var user = header.Substring(7);
                var principal = new ClaimsPrincipal();

                if (principal == null)
                {
                    return Task.FromResult(AuthenticateResult.Fail("No such user"));
                }

                var ticket = new AuthenticationTicket(principal, new AuthenticationProperties(), Options.AuthenticationScheme);
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            else
            {
                var msg = Newtonsoft.Json.JsonConvert.SerializeObject(new ResponseResult(100, null, "接口请求异常"));
                return Task.FromResult(AuthenticateResult.Fail(msg));
            }

        }
    }
}