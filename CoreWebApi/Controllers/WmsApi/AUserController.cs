using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
// using CoreData.CoreUser;
using Microsoft.AspNetCore.Authorization;
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
        #region Wms登陆 Core/Api/AUser/ALogin?Account=admin&Password=admin
        // [HttpGetAttribute("Core/Api/AUser/ALogin")]
        // public ResponseResult ALogin(string Account,string Password)
        [HttpPostAttribute("Core/AUser/ALogin")]
        public ResponseResult ALogin([FromBodyAttribute]JObject obj)
        {
            var cp = Newtonsoft.Json.JsonConvert.DeserializeObject<AUserParam>(obj.ToString());
            // var cp = new AUserParam();
            // cp.Account = Account;
            cp.Password = GetMD5(cp.Password, "Xy@.");
            var res = AUserHaddle.GetAUser(cp);
            return CoreResult.NewResponse(res.s, res.d, "Indentity");
        }
        #endregion


        #region Wms登陆 Core/Api/AUser/ALogin1?Account=admin&Password=admin
        [HttpGetAttribute("Core/AUser/ALogin1")]
        public ResponseResult ALogin1(string Account,string Password)
        {
            var cp = new AUserParam();
            cp.Account = Account;
            cp.Password = GetMD5(Password, "Xy@.");
            var res = AUserHaddle.GetAUser(cp);
            return CoreResult.NewResponse(res.s, res.d, "Indentity");
        }
        #endregion
    }
}