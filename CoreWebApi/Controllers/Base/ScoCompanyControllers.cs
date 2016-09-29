using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using CoreModels.XyCore;
using CoreData.CoreCore;
using System.Collections.Generic;
namespace CoreWebApi
{
    public class ScoCompanyController : ControllBase
    {
        [AllowAnonymous]
        [HttpPostAttribute("/Core/ScoCompany/CallScoCompanyList")]
        public ResponseResult ScoCompanyList([FromBodyAttribute]JObject co)
        {   
            var cp = new ScoCompanyParm();
            cp.CoID = int.Parse(GetCoid());
            cp.Enable = co["Enable"].ToString();
            cp.Filter = co["Filter"].ToString();
            cp.SortField = co["SortField"].ToString();
            cp.SortDirection = co["SortDirection"].ToString();
            cp.NumPerPage = int.Parse(co["NumPerPage"].ToString());
            cp.PageIndex = int.Parse(co["PageIndex"].ToString());
            var data = ScoCompanyHaddle.GetScoCompanyList(cp);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [AllowAnonymous]
        [HttpPostAttribute("/Core/ScoCompany/ScoCompanyEnable")]
        public ResponseResult CompanyEnable([FromBodyAttribute]JObject co)
        {   
            Dictionary<int,string> IDsDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int,string>>(co["IDsDic"].ToString());
            string Company = co["Company"].ToString();
            string UserName = GetUname(); 
            bool Enable = co["Enable"].ToString().ToUpper()=="TRUE"?true:false;
            
            var data = ScoCompanyHaddle.UpdateScoComEnable(IDsDic,Company,UserName,Enable);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [AllowAnonymous]
        [HttpPostAttribute("/Core/ScoCompany/ScoCompanySingle")]
        public ResponseResult ScoCompanySingle([FromBodyAttribute]JObject co)
        {   
            int id = int.Parse(co["ID"].ToString());
            var data = ScoCompanyHaddle.GetScoCompanyEdit(id);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [AllowAnonymous]
        [HttpPostAttribute("/Core/ScoCompany/UpdateScoCompany")]
        public ResponseResult UpdateScoCompany([FromBodyAttribute]JObject co)
        {   
            string modifyFlag = co["ModifyFlag"].ToString();
            string CoID = GetCoid();
            var com = Newtonsoft.Json.JsonConvert.DeserializeObject<ScoCompanySingle>(co["Com"].ToString());
            string UserName = GetUname(); 
            string Company = co["Company"].ToString();
            if(modifyFlag == "new")
            {
                var res = ScoCompanyHaddle.IsScoComExist(com.sconame,int.Parse(CoID));
                if (bool.Parse(res.d.ToString()) == true)
                {
                    return CoreResult.NewResponse(-1, "客户已存在,不允许新增", "General"); 
                }
            }
            var data = ScoCompanyHaddle.SaveScoCompany(modifyFlag,com,UserName,Company,int.Parse(CoID));
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [AllowAnonymous]
        [HttpPostAttribute("/Core/ScoCompany/GetScoCompanyAll")]
        public ResponseResult GetScoCompanyAll()
        {   
            int CoID = int.Parse(GetCoid());
            var data = ScoCompanyHaddle.GetScoCompanyAll(CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

    }
}