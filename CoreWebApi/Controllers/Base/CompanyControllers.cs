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
    public class CompanyController : ControllBase
    {
        [HttpGetAttribute("/Core/Company/CompanyList")]
        public ResponseResult CompanyList(string Enable,string Filter,string SortField,string SortDirection,string PageIndex,string NumPerPage)
        {   
            var cp = new CompanyParm();
            cp.CoID = int.Parse(GetCoid());
            if(Enable.ToUpper() == "TRUE" || Enable.ToUpper() == "FALSE")
            {
                cp.Enable = Enable;
            }
            cp.Filter = Filter;
            if(CommHaddle.SysColumnExists(DbBase.CoreConnectString,"company",SortField).s == 1)
            {
                cp.SortField = SortField;
            }
            if(SortDirection.ToUpper() == "ASC")
            {
                cp.SortDirection = SortDirection;
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
            Dictionary<int,string> IDsDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int,string>>(co["IDsDic"].ToString());
            string Company = co["Company"].ToString();
            string UserName = GetUname(); 
            bool Enable = co["Enable"].ToString().ToUpper()=="TRUE"?true:false;
            
            var data = CompanyHaddle.UpdateComEnable(IDsDic,Company,UserName,Enable);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Company/UpdateCompany")]
        public ResponseResult UpdateCompamy([FromBodyAttribute]JObject co)
        {   
            string modifyFlag = co["ModifyFlag"].ToString();
            var com = Newtonsoft.Json.JsonConvert.DeserializeObject<CompanySingle>(co["Com"].ToString());
            string UserName = GetUname(); 
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