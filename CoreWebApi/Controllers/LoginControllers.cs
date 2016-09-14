using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

            var data = UserHaddle.GetUserInfo("admin",GetMD5("admin","Xy@."));
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
            return CoreResult.NewResponse(0, user, "Basic");
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
        [HttpPostAttribute("/Core/sign/in")]
        public async Task<ResponseResult> login([FromBodyAttribute]JObject lo)
        {
            var password = GetMD5(lo["password"].ToString(), "Xy@.");
            var data = UserHaddle.GetUserInfo(lo["account"].ToString(),password);
            var user = data.d as User;

            if(data.s>0)
            {
                return CoreResult.NewResponse(data.s, lo, "Indentity");
            }

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
            return CoreResult.NewResponse(0, null, "Basic");
        }

        [HttpGetAttribute("/core/sign/mune")]
        public ResponseResult MenuList()
        {
            var roleid = GetRoleid();
            var coid = GetCoid();
            // var roleid = "1";
            // var coid = "1";
            var m = UserHaddle.GetMenuList(roleid, coid);
            return CoreResult.NewResponse(m.s, m.d, "Basic");
        }

        [HttpGetAttribute("/Core/sign/out")]    
        public async Task loginout()
        {
            await HttpContext.Authentication.SignOutAsync("CoreInstance");
        }
    }
}