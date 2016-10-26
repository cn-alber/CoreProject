using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using CoreData.CoreUser;
// using Microsoft.AspNetCore.Authorization;
using CoreModels.XyUser;
namespace CoreWebApi
{
    // [AllowAnonymous]
    public class BusinessController : ControllBase
    {
        [HttpGetAttribute("/Core/Business/GetBusiness")]
        public ResponseResult GetBusiness()
        {   
            int CoID = int.Parse(GetCoid());
            var data = BusinessHaddle.GetBusiness(CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
        [HttpPostAttribute("/Core/Business/UpdateBusiness")]
        public ResponseResult UpdateBusiness([FromBodyAttribute]JObject co)
        {   
            var business = Newtonsoft.Json.JsonConvert.DeserializeObject<Business>(co["Business"].ToString());
            string UserName = GetUname(); 
            int CoID = int.Parse(GetCoid());
            var data = BusinessHaddle.UpdateBusiness(business,UserName,CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
    }
}