using Microsoft.AspNetCore.Mvc;
using CoreData.CoreCore;
using Microsoft.AspNetCore.Authorization;

namespace CoreWebApi
{
    public class BasicController : ControllBase
    {
        [AllowAnonymous]
        [HttpPostAttribute("/Core/Basic/GetScoCompany")]
        public ResponseResult GetScoCompany()
        {   
            int CoId = int.Parse(GetCoid());
            var data = ScoCompanyHaddle.GetScoCompanyAll(CoId);
            return CoreResult.NewResponse(data.s, data.d, "General");
        }
    }
}
