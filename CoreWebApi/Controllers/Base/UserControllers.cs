using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using CoreData.CoreUser;
using Microsoft.AspNetCore.Authorization;
using CoreModels.XyUser;
using System.Collections.Generic;

namespace CoreWebApi
{
    [AllowAnonymous]
     public class UserController : ControllBase
     {
         public ResponseResult UserLst([FromBodyAttribute]JObject obj)
         {
             var cp = Newtonsoft.Json.JsonConvert.DeserializeObject<UserParam>(obj["UserParam"].ToString());
             var res = UserHaddle.GetUserLst(cp);
            return CoreResult.NewResponse(res.s,res.d,"General");
         }

     }

}