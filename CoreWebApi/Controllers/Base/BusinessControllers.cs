using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using CoreData.CoreUser;
// using Microsoft.AspNetCore.Authorization;
using CoreModels.XyUser;
namespace CoreWebApi
{
    public class BusinessController : ControllBase
    {
        [HttpGetAttribute("/Core/Business/GetBusiness")]
        public ResponseResult GetBusiness()
        {   
            int CoID = int.Parse(GetCoid());
            var data = BusinessHaddle.GetBusiness(CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Business/InsertBusiness")]
        public ResponseResult InsertBusiness([FromBodyAttribute]JObject co)
        {   
            var business = Newtonsoft.Json.JsonConvert.DeserializeObject<Business>(co["Business"].ToString());
            string UserName = GetUname(); 
            string Company = co["Company"].ToString();
            int CoID = int.Parse(GetCoid());
            var data = BusinessHaddle.InsertBusiness(business,UserName,Company,CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Business/UpdateBusiness")]
        public ResponseResult UpdateBusiness([FromBodyAttribute]JObject co)
        {   
            var business = Newtonsoft.Json.JsonConvert.DeserializeObject<Business>(co["Business"].ToString());
            string UserName = GetUname(); 
            string Company = co["Company"].ToString();
            var data = BusinessHaddle.UpdateBusiness(business,UserName,Company);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
    }
}