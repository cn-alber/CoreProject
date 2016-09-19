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
    [Route("api/[controller]")]
    public class CompanyController : ControllBase
    {
        [HttpGet("id={id}&filter={nameFilter}&enable={enable}")]
        public ResponseResult Get(int id,string nameFilter,string enable)
        {
            var data = CompanyHaddle.GetCompanyAll(id,nameFilter,enable);
            return CoreResult.NewResponse(data.s, data.d, "Basic");
        }
    }
}