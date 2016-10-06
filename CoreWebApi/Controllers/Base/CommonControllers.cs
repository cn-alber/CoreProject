using Microsoft.AspNetCore.Mvc;
using CoreData.CoreCore;

namespace CoreWebApi
{
    public class CommonController : ControllBase
    {
        [HttpGetAttribute("/Core/Common/ScoCompany")]
        public ResponseResult GetScoCompany()
        {   
            int CoId = int.Parse(GetCoid());
            var data = ScoCompanyHaddle.GetScoCompanyAll(CoId);
            return CoreResult.NewResponse(data.s, data.d, "General");
        }
    }
}
