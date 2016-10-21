using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using CoreData.CoreUser;
using Microsoft.AspNetCore.Authorization;
using CoreModels.XyUser;
using System.Collections.Generic;
using CoreData.CoreComm;
using CoreData;
using CoreModels;
namespace CoreWebApi
{
    [AllowAnonymous]
    public class CompanyController : ControllBase
    {
        [HttpGetAttribute("/Core/Company/CompanyList")]
        public ResponseResult CompanyList(string Enable,string Filter,string SortField,string SortDirection,string PageIndex,string NumPerPage)
        {   
            var cp = new CompanyParm();
            cp.CoID = int.Parse(GetCoid());
            if(!string.IsNullOrEmpty(Enable))
            {
               if(Enable.ToUpper() == "TRUE" || Enable.ToUpper() == "FALSE")
                {
                    cp.Enable = Enable;
                }
            }
            cp.Filter = Filter;
            if(!string.IsNullOrEmpty(SortField))
            {
                if(CommHaddle.SysColumnExists(DbBase.CoreConnectString,"company",SortField).s == 1)
                {
                    cp.SortField = SortField;
                }
            }
            if(!string.IsNullOrEmpty(SortDirection))
            {
                 if(SortDirection.ToUpper() == "ASC" || SortDirection.ToUpper() == "DESC")
                {
                    cp.SortDirection = SortDirection;
                }
            }
            int x;
            if (int.TryParse(NumPerPage, out x))
            {
                cp.NumPerPage = int.Parse(NumPerPage);
            }
            if (int.TryParse(PageIndex, out x))
            {
                cp.PageIndex = int.Parse(PageIndex);
            }
            var data = CompanyHaddle.GetCompanyList(cp);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Company/GetCompanySingle")]
        public ResponseResult CompanySingle(string ID)
        {   
            int x,id;
            var data = new DataResult(1,null);  
            if (int.TryParse(ID, out x))
            {
                id = int.Parse(ID);
                data = CompanyHaddle.GetCompanyEdit(id);
            }
            else
            {
                data.s = -1;
                data.d = "参数无效!";
            }
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Company/CompanyEnable")]
        public ResponseResult CompanyEnable([FromBodyAttribute]JObject co)
        {   
            List<int> IDsDic = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["IDList"].ToString());
            string Company = "";//co["Company"].ToString();
            string UserName = GetUname(); 
            bool Enable = co["Enable"].ToString().ToUpper()=="TRUE"?true:false;
            
            var data = CompanyHaddle.UpdateComEnable(IDsDic,Company,UserName,Enable);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Company/UpdateCompany")]
        public ResponseResult UpdateCompamy([FromBodyAttribute]JObject co)
        {   
            var com = Newtonsoft.Json.JsonConvert.DeserializeObject<Company>(co["Com"].ToString());
            string UserName = GetUname(); 
            string Company = "";//co["Company"].ToString();
            var data = CompanyHaddle.UpdateCompany(com,UserName,Company);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Company/InsertCompany")]
        public ResponseResult InsertCompany([FromBodyAttribute]JObject co)
        {   
            var data = new DataResult(1,null);  
            var com = Newtonsoft.Json.JsonConvert.DeserializeObject<Company>(co["Com"].ToString());
            string UserName = GetUname(); 
            string Company = "";//co["Company"].ToString();
            string account = co["Account"].ToString();
            if(string.IsNullOrEmpty(account))
            {
                data.s = -1;
                data.d = "Account不能为空!";
                return CoreResult.NewResponse(data.s, data.d, "General"); 
            }
            string Name = co["Name"].ToString();
            if(string.IsNullOrEmpty(Name))
            {
                data.s = -1;
                data.d = "Name不能为空!";
                return CoreResult.NewResponse(data.s, data.d, "General"); 
            }
            string Password = co["Password"].ToString();
            if(string.IsNullOrEmpty(Password))
            {
                data.s = -1;
                data.d = "Password不能为空!";
                return CoreResult.NewResponse(data.s, data.d, "General"); 
            }
            string Email = co["Email"].ToString();
            string Gender = co["Gender"].ToString();
            string Mobile = co["Mobile"].ToString();
            string QQ = co["QQ"].ToString();
            var res = CompanyHaddle.IsComExist(com.name);
            if (bool.Parse(res.d.ToString()) == true)
            {
                return CoreResult.NewResponse(-1, "公司已存在,不允许新增", "General"); 
            }
            data = CompanyHaddle.InsertCompany(com,UserName,Company,account,Name,Password,Email,Gender,Mobile,QQ);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
    }
}