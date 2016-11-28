
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
// using CoreData.CoreUser;
// using CoreModels.XyUser;
// using System.Collections.Generic;
// using CoreData.CoreComm;
using CoreData.CoreWmsApi;
// using CoreData;
// using CoreModels;
using CoreModels.WmsApi;

namespace CoreWebApi
{
    [AllowAnonymous]
    public class AUserController : ControllBase
    {
        // #region Wms登陆 Core/AUser/ALogin?Account=admin&Password=admin
        // // [HttpGetAttribute("Core/Api/AUser/ALogin")]
        // // public ResponseResult ALogin(string Account,string Password)
        // [HttpPostAttribute("Core/AUser/ALogin")]
        // public ResponseResult ALogin([FromBodyAttribute]JObject obj)
        // {
        //     var cp = Newtonsoft.Json.JsonConvert.DeserializeObject<AUserParam>(obj.ToString());
        //     // var cp = new AUserParam();
        //     // cp.Account = Account;
        //     cp.Password = GetMD5(cp.Password, "Xy@.");
        //     var res = AUserHaddle.GetAUser(cp);
        //     return CoreResult.NewResponse(res.s, res.d, "Indentity");
        // }
        // #endregion


        #region Wms登陆 Core/AUser/ALogin?Account=admin&Password=admin
        [HttpGetAttribute("Core/AUser/ALogin")]
        public async Task<ResponseResult> ALogin(string Account, string Password)
        {
            var cp = new AUserParam();
            cp.Account = Account;
            cp.Password = GetMD5(Password, "Xy@.");
            var res = AUserHaddle.GetAUser(cp);
            if (res.s == 1) //登陆认证
            {
                try
                {
                    var user = res.d as AUser;
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
            }
            return CoreResult.NewResponse(res.s, res.d, "Indentity");
        }
        #endregion

        #region Wms检查 - 是否启用 唯一码管理
        [HttpGetAttribute("Core/AUser/AUniCode")]
        public ResponseResult AUniCode()
        {
            string CoID = GetCoid();
            var res = AUserHaddle.GetUniqCode(CoID);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion
    }
}