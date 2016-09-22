using Microsoft.AspNetCore.Mvc;
//using System.Threading.Tasks;
//using System.Security.Claims;
//using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using Microsoft.AspNetCore.Http.Authentication;
//using System;
//using Microsoft.AspNetCore.Http;
using CoreData.CoreUser;
using Microsoft.AspNetCore.Authorization;
//using CoreModels.XyUser;
//using System.Linq;

namespace CoreWebApi
{
    public class CompanyController : ControllBase
    {
        // [HttpGet("api/Company")]
        // public ResponseResult Get()
        // {
        //     var data = CompanyHaddle.GetCompanyAll(1,"","all",33,3);
        //     return CoreResult.NewResponse(data.s, data.d, "Basic"); 
        // }
        [AllowAnonymous]
        [HttpPostAttribute("/Core/Company/GetCompany")]
        public ResponseResult CompanyList([FromBodyAttribute]JObject co)
        {   
            int id = int.Parse(co["Id"].ToString());
            string nameFilter = co["Filter"].ToString();
            string enable = co["Enable"].ToString();
            int pageIndex = int.Parse(co["PageIndex"].ToString());
            int numPerPage = int.Parse(co["NumPerPage"].ToString());
            var data = CompanyHaddle.GetCompanyAll(id,nameFilter,enable,pageIndex,numPerPage);
            return CoreResult.NewResponse(data.s, data.d, "Basic"); 
        }

        [AllowAnonymous]
        [HttpPostAttribute("/Core/Company/GetCompanySingle")]
        public ResponseResult CompanySingle([FromBodyAttribute]JObject co)
        {   
            int id = int.Parse(co["Id"].ToString());
            var data = CompanyHaddle.GetCompanyEdit(id);
            return CoreResult.NewResponse(data.s, data.d, "Basic"); 
        }

        [AllowAnonymous]
        [HttpPostAttribute("/Core/Company/UpdateEnable")]
        public ResponseResult UpdateEnable([FromBodyAttribute]JObject co)
        {   
            string idList = co["Id"].ToString();
            bool enable = bool.Parse(co["Enable"].ToString());
            string nameList = co["Name"].ToString();
            var data = CompanyHaddle.UpdateEnable(idList,nameList,enable);
            return CoreResult.NewResponse(data.s, data.d, "Basic"); 
        }



        // [AllowAnonymous]
        // [HttpPostAttribute("/Core/Company/UpdateCompany")]
        // public ResponseResult UpdateCompamy([FromBodyAttribute]JObject co)
        // {   
        //     string idList = co["Id"].ToString();
        //     bool enable = bool.Parse(co["Enable"].ToString());
        //     string nameList = co["Name"].ToString();
        //     var data = CompanyHaddle.UpdateEnable(idList,nameList,enable);
        //     return CoreResult.NewResponse(data.s, data.d, "Basic"); 
        // }
    }
}