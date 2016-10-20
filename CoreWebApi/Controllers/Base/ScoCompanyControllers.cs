using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
// using Microsoft.AspNetCore.Authorization;
using CoreModels.XyCore;
using CoreData.CoreCore;
using System.Collections.Generic;
using CoreData.CoreComm;
using CoreData;
using CoreModels;

namespace CoreWebApi
{
    // [AllowAnonymous]
    public class ScoCompanyController : ControllBase
    {
        [HttpGetAttribute("/Core/ScoCompany/ScoCompanyList")]
        public ResponseResult ScoCompanyList(string Enable,string Filter,string SortField,string SortDirection,string PageIndex,string NumPerPage)
        {   
            var cp = new ScoCompanyParm();
            cp.CoID = int.Parse(GetCoid());
            if(Enable.ToUpper() == "TRUE" || Enable.ToUpper() == "FALSE")
            {
                cp.Enable = Enable;
            }
            cp.Filter = Filter;
            if(CommHaddle.SysColumnExists(DbBase.CoreConnectString,"supplycompany",SortField).s == 1)
            {
                cp.SortField = SortField;
            }
            if(!string.IsNullOrEmpty(SortDirection))
            {
                 if(SortDirection.ToUpper() == "ASC")
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
            var data = ScoCompanyHaddle.GetScoCompanyList(cp);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/ScoCompany/ScoCompanyEnable")]
        public ResponseResult CompanyEnable([FromBodyAttribute]JObject co)
        {   
            List<int> id = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["IDList"].ToString());
            string Company = co["Company"].ToString();
            string UserName = GetUname(); 
            bool Enable = co["Enable"].ToString().ToUpper()=="TRUE"?true:false;
            var data = ScoCompanyHaddle.UpdateScoComEnable(id,Company,UserName,Enable);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/ScoCompany/ScoCompanySingle")]
        public ResponseResult ScoCompanySingle(string ID)
        {   
            int x,id;
            var data = new DataResult(1,null);  
            if (int.TryParse(ID, out x))
            {
                id = int.Parse(ID);
                data = ScoCompanyHaddle.GetScoCompanyEdit(id);
            }
            else
            {
                data.s = -1;
                data.d = "参数无效!";
            }
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/ScoCompany/InsetScoCompany")]
        public ResponseResult InsetScoCompany([FromBodyAttribute]JObject co)
        {   
            string CoID = GetCoid();
            var com = Newtonsoft.Json.JsonConvert.DeserializeObject<ScoCompanySingle>(co["Com"].ToString());
            string UserName = GetUname(); 
            string Company = co["Company"].ToString();
            var res = ScoCompanyHaddle.IsScoComExist(com.sconame,int.Parse(CoID));
            if (bool.Parse(res.d.ToString()) == true)
            {
                return CoreResult.NewResponse(-1, "客户已存在,不允许新增", "General"); 
            }
            var data = ScoCompanyHaddle.InsertScoCompany(int.Parse(CoID),com,UserName,Company);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/ScoCompany/UpdateScoCompany")]
        public ResponseResult UpdateScoCompany([FromBodyAttribute]JObject co)
        {   
            string CoID = GetCoid();
            var com = Newtonsoft.Json.JsonConvert.DeserializeObject<ScoCompanySingle>(co["Com"].ToString());
            string UserName = GetUname(); 
            string Company = co["Company"].ToString();
            var data = ScoCompanyHaddle.UpdateScoCompany(int.Parse(CoID),com,UserName,Company);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
    }
}