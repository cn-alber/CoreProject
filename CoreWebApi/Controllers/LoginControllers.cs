using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.Authentication;
using System;
using Microsoft.AspNetCore.Http;
using CoreData.CoreUser;
using CoreModels.XyUser;
using System.Linq;

namespace CoreWebApi
{
    public class LoginController : ControllBase
    {
        [AllowAnonymous]
        [HttpGet("/api")]
        public async Task<ResponseResult> Get()
        {
            var test = new Models.Login();
            test.account = "admin";
            test.password = "admin";
            test.vcode = "123123";

            var data = UserHaddle.GetUserInfo("admin","admin");
            var user = data.d as User;

             var userc = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] {
                        new Claim("uid", user.ID.ToString()),
                        new Claim("uname",user.Name.ToString()),
                        new Claim("coid",user.CompanyID.ToString()),
                        new Claim("roleid", user.RoleID.ToString())
                         },
                         "CoreInstance"));   
                    

            await HttpContext.Authentication.SignInAsync("CoreInstance", userc,
                new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                    IsPersistent = false
                });
            var ss = JsonConvert.SerializeObject(test);
            return new ResponseResult(100, data, "");
        }

        [HttpGet("/api/{id}")]
        public IActionResult Get(int id)
        {
            var user = HttpContext.User;
            var cc = user.Claims;
             var test = new Models.Login();
            test.account = "admin";
            test.password = "admin";
            test.vcode = cc.FirstOrDefault(q => q.Type.Equals("uid")).Value;

            
            // cc.FirstOrDefault(q => q.Type.Equals("uid"));

            return new OkObjectResult(test);
        }

        [AllowAnonymous]
        [HttpPostAttribute("/Core/loginin")]
        public async Task<ResponseResult> login(dynamic lo)
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] {
                        new Claim(ClaimTypes.Name, "xishuai")
                         },
                         "CoreInstance"));

            await HttpContext.Authentication.SignInAsync("CoreInstance", user,
                new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                    IsPersistent = false
                });
            return new ResponseResult(100, lo, "");
        }

        [HttpGetAttribute("/Core/loginout")]    
        public async Task loginout()
        {
            await HttpContext.Authentication.SignOutAsync("CoreInstance");
        }
    }
}