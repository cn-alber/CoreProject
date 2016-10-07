using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using CoreModels.XyCore;
using CoreData.CoreCore;
using System;
using CoreData.CoreComm;
using CoreData;
using System.Collections.Generic;
namespace CoreWebApi
{
    public class PurchaseController : ControllBase
    {
        [HttpGetAttribute("/Core/Purchase/PurchaseList")]
        public ResponseResult PurchaseList(string Purid,string PurdateStart,string PurdateEnd,string Status,string Coname,string Skuid,string Warehouseid,string Buyyer,string SortField,string SortDirection,string PageIndex,string NumPerPage)
        {   
            int x;
            var cp = new PurchaseParm();
            cp.CoID = int.Parse(GetCoid());
            if (int.TryParse(Purid, out x))
            {
                cp.Purid = int.Parse(Purid);
            }
            DateTime date;
            if (DateTime.TryParse(PurdateStart, out date))
            {
                cp.PurdateStart = DateTime.Parse(PurdateStart);
            }
            if (DateTime.TryParse(PurdateEnd, out date))
            {
                cp.PurdateEnd = DateTime.Parse(PurdateEnd);
            }
            if (int.TryParse(Status, out x))
            {
                cp.Status = int.Parse(Status);
            }
            cp.CoName = Coname;
            cp.Skuid = Skuid;
            if (int.TryParse(Warehouseid, out x))
            {
                cp.Warehouseid = int.Parse(Warehouseid);
            }
            cp.Buyyer = Buyyer;
            if(CommHaddle.SysColumnExists(DbBase.CoreConnectString,"purchase",SortField).s == 1)
            {
                cp.SortField = SortField;
            }
            if(SortDirection.ToUpper() == "ASC")
            {
                cp.SortDirection = SortDirection;
            }
            if (int.TryParse(NumPerPage, out x))
            {
                cp.NumPerPage = int.Parse(NumPerPage);
            }
            if (int.TryParse(PageIndex, out x))
            {
                cp.PageIndex = int.Parse(PageIndex);
            }
            var data = PurchaseHaddle.GetPurchaseList(cp);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Purchase/PurchaseDetailList")]
        public ResponseResult PurchaseDetailList(string Purid,string Skuid,string SkuName,string GoodsCode,string SortField,string SortDirection,string PageIndex,string NumPerPage)
        {   
            var cp = new PurchaseDetailParm();
            cp.CoID = int.Parse(GetCoid());
            int x;
            if (int.TryParse(Purid, out x))
            {
                cp.Purid = int.Parse(Purid);
            }
            cp.Skuid = Skuid;
            cp.SkuName = SkuName;
            cp.GoodsCode = GoodsCode;
            if(CommHaddle.SysColumnExists(DbBase.CoreConnectString,"purchasedetail",SortField).s == 1)
            {
                cp.SortField = SortField;
            }
            if(SortDirection.ToUpper() == "ASC")
            {
                cp.SortDirection = SortDirection;
            }
            if (int.TryParse(NumPerPage, out x))
            {
                cp.NumPerPage = int.Parse(NumPerPage);
            }
            if (int.TryParse(PageIndex, out x))
            {
                cp.PageIndex = int.Parse(PageIndex);
            }
            var data = PurchaseHaddle.GetPurchaseDetailList(cp);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [AllowAnonymous]
        [HttpPostAttribute("/Core/Purchase/DeletePur")]
        public ResponseResult DeletePur([FromBodyAttribute]JObject co)
        {   
            List<string> puridList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(co["PurIdList"].ToString());
            int CoId = int.Parse(GetCoid());
            var data = PurchaseHaddle.DeletePur(puridList,CoId);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [AllowAnonymous]
        [HttpPostAttribute("/Core/Purchase/SavePur")]
        public ResponseResult SavePur([FromBodyAttribute]JObject co)
        {   
            string modifyFlag = co["ModifyFlag"].ToString();
            var pur = Newtonsoft.Json.JsonConvert.DeserializeObject<Purchase>(co["Pur"].ToString());
            string UserName = GetUname(); 
            int CoID = int.Parse(GetCoid());
            var data = PurchaseHaddle.SavePurchase(modifyFlag,pur,UserName,CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [AllowAnonymous]
        [HttpPostAttribute("/Core/Purchase/ConfirmPur")]
        public ResponseResult ConfirmPur([FromBodyAttribute]JObject co)
        {   
            List<string> puridList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(co["PurIdList"].ToString());
            int CoId = int.Parse(GetCoid());
            var data = PurchaseHaddle.ConfirmPurchase(puridList,CoId);
            return CoreResult.NewResponse(data.s, data.d, "General");
        }

        [AllowAnonymous]
        [HttpPostAttribute("/Core/Purchase/ForcePur")]
        public ResponseResult ForcePur([FromBodyAttribute]JObject co)
        {   
            List<string> puridList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(co["PurIdList"].ToString());
            int CoId = int.Parse(GetCoid());
            var data = PurchaseHaddle.ForcePurchase(puridList,CoId);
            return CoreResult.NewResponse(data.s, data.d, "General");
        }

        [AllowAnonymous]
        [HttpPostAttribute("/Core/Purchase/InsertPurDetail")]
        public ResponseResult InsertPurDetail([FromBodyAttribute]JObject co)
        {   
            var detail = Newtonsoft.Json.JsonConvert.DeserializeObject<PurchaseDetail>(co["PurDetail"].ToString());
            int CoId = int.Parse(GetCoid());
            var data = PurchaseHaddle.InsertPurDetail(detail,CoId);
            return CoreResult.NewResponse(data.s, data.d, "General");
        }

        [AllowAnonymous]
        [HttpPostAttribute("/Core/Purchase/UpdatePurDetail")]
        public ResponseResult UpdatePurDetail([FromBodyAttribute]JObject co)
        {   
            var detail = Newtonsoft.Json.JsonConvert.DeserializeObject<PurchaseDetail>(co["PurDetail"].ToString());
            int CoId = int.Parse(GetCoid());
            var data = PurchaseHaddle.UpdatePurDetail(detail,CoId);
            return CoreResult.NewResponse(data.s, data.d, "General");
        }

        [AllowAnonymous]
        [HttpPostAttribute("/Core/Purchase/DelPurDetail")]
        public ResponseResult DelPurDetail([FromBodyAttribute]JObject co)
        {   
            int id = int.Parse(co["ID"].ToString());
            List<int> detailid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["DetailID"].ToString());
            int CoId = int.Parse(GetCoid());
            var data = PurchaseHaddle.DeletePurDetail(id,detailid,CoId);
            return CoreResult.NewResponse(data.s, data.d, "General");
        }

        [AllowAnonymous]
        [HttpPostAttribute("/Core/Purchase/GetScoCompany")]
        public ResponseResult GetScoCompany()
        {   
            int CoId = int.Parse(GetCoid());
            var data = ScoCompanyHaddle.GetScoCompanyAll(CoId);
            return CoreResult.NewResponse(data.s, data.d, "General");
        }

        [AllowAnonymous]
        [HttpPostAttribute("/Core/Purchase/GetSkuAll")]
        public ResponseResult GetSkuAll([FromBodyAttribute]JObject co)
        {   
            SkuParam cp = Newtonsoft.Json.JsonConvert.DeserializeObject<SkuParam>(co["SkuParam"].ToString());
            int CoId = int.Parse(GetCoid());
            var data = CoreSkuHaddle.GetSkuAll(cp,CoId,2);
            return CoreResult.NewResponse(data.s, data.d, "General");
        }
    }
}
