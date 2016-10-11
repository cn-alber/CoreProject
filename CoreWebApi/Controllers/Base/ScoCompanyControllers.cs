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
            Dictionary<int,string> IDsDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int,string>>(co["IDsDic"].ToString());
            string Company = co["Company"].ToString();
            string UserName = GetUname(); 
            bool Enable = co["Enable"].ToString().ToUpper()=="TRUE"?true:false;
            
            var data = ScoCompanyHaddle.UpdateScoComEnable(IDsDic,Company,UserName,Enable);
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
    }
}