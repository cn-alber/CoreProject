using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using CoreData.CoreUser;
// using Microsoft.AspNetCore.Authorization;
using CoreModels.XyUser;
using System.Collections.Generic;
using CoreData.CoreComm;
using CoreData;
using CoreModels;
namespace CoreWebApi
{
    // [AllowAnonymous]
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
            int CoID = int.Parse(GetCoid());
            string UserName = GetUname(); 
            bool Enable = co["Enable"].ToString().ToUpper()=="TRUE"?true:false;
            
            var data = CompanyHaddle.UpdateComEnable(IDsDic,CoID,UserName,Enable);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Company/UpdateCompany")]
        public ResponseResult UpdateCompamy([FromBodyAttribute]JObject co)
        {   
            var com = Newtonsoft.Json.JsonConvert.DeserializeObject<Company>(co["Com"].ToString());
            string UserName = GetUname(); 
            int CoID = int.Parse(GetCoid());
            var data = CompanyHaddle.UpdateCompany(com,UserName,CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Company/InsertCompany")]
        public ResponseResult InsertCompany([FromBodyAttribute]JObject co)
        {   
            var data = new DataResult(1,null);  
            var com = Newtonsoft.Json.JsonConvert.DeserializeObject<Company>(co["Com"].ToString());
            string UserName = GetUname(); 
            int CoID = int.Parse(GetCoid());
            var User = Newtonsoft.Json.JsonConvert.DeserializeObject<UserEdit>(co["User"].ToString());
            if(com.name == null)
            {
                data.s = -1;
                data.d = "公司名称必须有值!";
                return CoreResult.NewResponse(data.s, data.d, "General");
            }
            if(User.Account == null)
            {
                data.s = -1;
                data.d = "Account不能为空!";
                return CoreResult.NewResponse(data.s, data.d, "General"); 
            }
            if(User.Name == null)
            {
                data.s = -1;
                data.d = "Name不能为空!";
                return CoreResult.NewResponse(data.s, data.d, "General"); 
            }
            if(User.PassWord == null)
            {
                data.s = -1;
                data.d = "Password不能为空!";
                return CoreResult.NewResponse(data.s, data.d, "General"); 
            }
            var res = CompanyHaddle.IsComExist(com.name);
            if (bool.Parse(res.d.ToString()) == true)
            {
                return CoreResult.NewResponse(-1, "公司已存在,不允许新增", "General"); 
            }
            data = CompanyHaddle.InsertCompany(com,UserName,CoID,User);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
    }
}