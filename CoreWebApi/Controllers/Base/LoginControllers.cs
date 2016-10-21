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
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CoreWebApi
{
    public class LoginController : ControllBase
    {
        [AllowAnonymous]
        [HttpGet("/ws")]
        public void Ws(int id)
        {
            var oo = new {action="action", controller="controller", param=new {qq="sdf"}};
            CoreHelper.Debuger.log(oo);
            CoreHelper.Debuger.warning(oo);
            CoreHelper.Debuger.success(oo);
            CoreHelper.Debuger.error(oo);
            //CoreHelper.Debuger.success(oo);
        }

        [AllowAnonymous]
        [HttpGet("/apirun")]
        public void apiRun(){
            //CoreWebApi.ApiTask.ApiContext.Run();

        }


        [AllowAnonymous]
        [HttpGet("/api")]
        public async Task<ResponseResult> Get()
        {
            var test = new Models.Login();
            test.account = "admin";
            test.password = "admin";
            test.vcode = "123123";
            var user = new User();
            try
            {
                var data = UserHaddle.GetUserInfo("admin", GetMD5("admin", "Xy@."));
                user = data.d as User;
            }
            catch
            {
                return CoreResult.NewResponse(1, "数据库", "Basic");
            }

            try
            {
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
            }
            catch (Exception ex)
            {
                return CoreResult.NewResponse(1, ex.ToString(), "Basic");
            }
            // HttpContext.re
            var ss = JsonConvert.SerializeObject(test);
            return CoreResult.NewResponse(1, user, "Basic");
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
            var data = UserHaddle.GetUserInfo(lo["account"].ToString(), password);
            var user = data.d as User;
           
           if(user != null){
                //记录登录日志
                UserHaddle.loginLog(user.ID, Request.Headers["User-Agent"], Request.HttpContext.Connection.RemoteIpAddress.ToString());
           }
           

            if (data.s < 0)
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
            return CoreResult.NewResponse(1, null, "Basic");
        }

        // [HttpGetAttribute("/core/sign/mune")]
        // public ResponseResult MenuList()
        // {
        //     var roleid = GetRoleid();
        //     var coid = GetCoid();

        //     var m = UserHaddle.GetMenuList(roleid, coid);
        //     return CoreResult.NewResponse(m.s, m.d, "Basic");
        // }

        // [HttpPostAttribute("/Core/sign/out")]    
        // public async Task loginout()
        // {
        //     await HttpContext.Authentication.SignOutAsync("CoreInstance");
        // }

        [HttpPostAttribute("/Core/sign/out")]
        public ResponseResult loginout()
        {
            var a = HttpContext.Authentication.SignOutAsync("CoreInstance");
            return CoreResult.NewResponse(1, null, "Basic");
        }

        [HttpPostAttribute("/Core/account/password")]
        public ResponseResult editpassword([FromBodyAttribute]JObject lo)
        {

            string oldPwd = lo["oldPwd"].ToString();
            string newPwd = lo["newPwd"].ToString();
            string reNewPwd = lo["reNewPwd"].ToString();

            string regexstr = @".{6,18}";
            if (string.IsNullOrEmpty(oldPwd)) { return CoreResult.NewResponse(-2006, null, "Indentity"); ; }
            if (string.IsNullOrEmpty(newPwd)) { return CoreResult.NewResponse(-2012, null, "Indentity"); ; }
            if (!Regex.IsMatch(newPwd, regexstr)) { return CoreResult.NewResponse(-2007, null, "Indentity"); ; }
            if (newPwd != reNewPwd) { return CoreResult.NewResponse(-2010, null, "Indentity"); ; }

            var m = UserHaddle.editPwd(GetUid(), GetMD5(oldPwd, "Xy@."), GetMD5(newPwd, "Xy@."));
            return CoreResult.NewResponse(m.s, m.d, "Indentity"); ;
            //return CoreResult.NewResponse(m.s,m.d, "Indentity");

        }



        // public static async Task ValidateAsync(CookieValidatePrincipalContext context)
        // {
        //     // Pull database from registered DI services.
        //     // var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
        //     // var userPrincipal = context.Principal;

        //     // // Look for the last changed claim.
        //     // string lastChanged;
        //     // lastChanged = (from c in userPrincipal.Claims
        //     //                where c.Type == "LastUpdated"
        //     //                select c.Value).FirstOrDefault();

        //     // if (string.IsNullOrEmpty(lastChanged) ||
        //     //     !userRepository.ValidateLastChanged(userPrincipal, lastChanged))
        //     // {
        //     //     context.RejectPrincipal();
        //     //     await context.HttpContext.Authentication.SignOutAsync("MyCookieMiddlewareInstance");
        //     // }
        // }
    }
}