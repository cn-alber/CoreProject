using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using CoreData.CoreUser;
using Microsoft.AspNetCore.Authorization;
using CoreModels.XyUser;
using System.Collections.Generic;

namespace CoreWebApi
{
    public class CompanyController : ControllBase
    {
        [AllowAnonymous]
        [HttpPostAttribute("/Core/Company/CallCompanyList")]
        public ResponseResult CompanyList([FromBodyAttribute]JObject co)
        {   
            var cp = new CompanyParm();
            cp.CoID = int.Parse(GetCoid());
            cp.Enable = co["Enable"].ToString();
            cp.Filter = co["Filter"].ToString();
            cp.SortField = co["SortField"].ToString();
            cp.SortDirection = co["SortDirection"].ToString();
            cp.NumPerPage = int.Parse(co["NumPerPage"].ToString());
            cp.PageIndex = int.Parse(co["PageIndex"].ToString());
            var data = CompanyHaddle.GetCompanyList(cp);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        // [AllowAnonymous]
        // [HttpPostAttribute("/Core/Company/GetCompanySingle")]
        // public ResponseResult CompanySingle([FromBodyAttribute]JObject co)
        // {   
        //     int id = int.Parse(co["Id"].ToString());
        //     var data = CompanyHaddle.GetCompanyEdit(id);
        //     return CoreResult.NewResponse(data.s, data.d, "Basic"); 
        // }

        [AllowAnonymous]
        [HttpPostAttribute("/Core/Company/CompanyEnable")]
        public ResponseResult CompanyEnable([FromBodyAttribute]JObject co)
        {   
            Dictionary<int,string> IDsDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int,string>>(co["IDsDic"].ToString());
            string Company = "携云科技";//obj["Company"].ToString(); 
            string UserName = "系统管理员";//obj["UserName"].ToString(); 
            bool Enable = co["Enable"].ToString().ToUpper()=="TRUE"?true:false;
            
            var data = CompanyHaddle.UpdateComEnable(IDsDic,Company,UserName,Enable);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
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