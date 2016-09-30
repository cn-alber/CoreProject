using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using CoreModels.XyCore;
using CoreData.CoreCore;
using System;
using System.Collections.Generic;
namespace CoreWebApi
{
    public class PurchaseController : ControllBase
    {
        [AllowAnonymous]
        [HttpPostAttribute("/Core/Purchase/PurchaseList")]
        public ResponseResult PurchaseList([FromBodyAttribute]JObject co)
        {   
            var cp = new PurchaseParm();
            cp.CoID = int.Parse(GetCoid());
            cp.Purid = co["Purid"].ToString();
            cp.PurdateStart = DateTime.Parse(co["PurdateStart"].ToString());
            cp.PurdateEnd = DateTime.Parse(co["PurdateEnd"].ToString());
            cp.Status = int.Parse(co["Status"].ToString());
            cp.CoName = co["CoName"].ToString();
            cp.SortField = co["SortField"].ToString();
            cp.SortDirection = co["SortDirection"].ToString();
            cp.NumPerPage = int.Parse(co["NumPerPage"].ToString());
            cp.PageIndex = int.Parse(co["PageIndex"].ToString());
            var data = PurchaseHaddle.GetPurchaseList(cp);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [AllowAnonymous]
        [HttpPostAttribute("/Core/Purchase/PurchaseDetailList")]
        public ResponseResult PurchaseDetailList([FromBodyAttribute]JObject co)
        {   
            var cp = new PurchaseDetailParm();
            cp.CoID = int.Parse(GetCoid());
            cp.Purid = co["Purid"].ToString();
            cp.Skuid = co["Skuid"].ToString();
            cp.SkuName = co["SkuName"].ToString();
            cp.GoodsCode = co["GoodsCode"].ToString();
            cp.SortField = co["SortField"].ToString();
            cp.SortDirection = co["SortDirection"].ToString();
            cp.NumPerPage = int.Parse(co["NumPerPage"].ToString());
            cp.PageIndex = int.Parse(co["PageIndex"].ToString());
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
    }
}
