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

        [AllowAnonymous]
        [HttpPostAttribute("/Core/Company/GetCompanySingle")]
        public ResponseResult CompanySingle([FromBodyAttribute]JObject co)
        {   
            int id = int.Parse(co["ID"].ToString());
            var data = CompanyHaddle.GetCompanyEdit(id);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [AllowAnonymous]
        [HttpPostAttribute("/Core/Company/CompanyEnable")]
        public ResponseResult CompanyEnable([FromBodyAttribute]JObject co)
        {   
            Dictionary<int,string> IDsDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int,string>>(co["IDsDic"].ToString());
            string Company = co["Company"].ToString();
            string UserName = GetUname(); 
            bool Enable = co["Enable"].ToString().ToUpper()=="TRUE"?true:false;
            
            var data = CompanyHaddle.UpdateComEnable(IDsDic,Company,UserName,Enable);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [AllowAnonymous]
        [HttpPostAttribute("/Core/Company/CompanyCheckExist")]
        public ResponseResult CompanyCheckExist([FromBodyAttribute]JObject co)
        {   
            string name = co["ComName"].ToString();
            var data = CompanyHaddle.IsComExist(name);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [AllowAnonymous]
        [HttpPostAttribute("/Core/Company/UpdateCompany")]
        public ResponseResult UpdateCompamy([FromBodyAttribute]JObject co)
        {   
            string modifyFlag = co["ModifyFlag"].ToString();
            var com = new CompanySingle();
            if(!string.IsNullOrEmpty(co["ID"].ToString()))
            {
                com.id =  int.Parse(co["ID"].ToString());
            }
            else
            {
                com.id =  0;
            }            
            com.name = co["Name"].ToString();
            com.enable = co["Enable"].ToString().ToUpper()=="TRUE"?true:false;
            com.address = co["Address"].ToString();
            com.email = co["Email"].ToString();
            com.typelist = co["Typelist"].ToString();
            com.contacts = co["Contacts"].ToString();
            com.telphone = co["Telphone"].ToString();
            com.mobile = co["Mobile"].ToString();
            com.remark = co["Remark"].ToString();
            string UserName = "系统管理员";//GetUname(); 
            string Company = co["Company"].ToString();
            if(modifyFlag == "new")
            {
                var res = CompanyHaddle.IsComExist(com.name);
                if (bool.Parse(res.d.ToString()) == true)
                {
                    return CoreResult.NewResponse(-1, "公司已存在,不允许新增", "General"); 
                }
            }
            var data = CompanyHaddle.SaveCompany(modifyFlag,com,UserName,Company);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
    }
}