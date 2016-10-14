using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
// using Microsoft.AspNetCore.Authorization;
using CoreModels.XyCore;
using CoreData.CoreCore;
using System;
using CoreData.CoreComm;
using CoreData;
using System.Collections.Generic;
using CoreModels;
namespace CoreWebApi
{
    // [AllowAnonymous]
    public class PurchaseController : ControllBase
    {
        [HttpGetAttribute("/Core/Purchase/PurchaseList")]
        public ResponseResult PurchaseList(string Purid,string PurdateStart,string PurdateEnd,string Status,string Scoid,string Skuid,string Warehouseid,string Buyyer,string SortField,string SortDirection,string PageIndex,string NumPerPage)
        {   
            int x;
            var cp = new PurchaseParm();
            cp.CoID = int.Parse(GetCoid());
            if(!string.IsNullOrEmpty(Purid))
            {
                if (int.TryParse(Purid, out x))
                {
                    cp.Purid = int.Parse(Purid);
                }
                else
                {
                    cp.Purid = 0;
                }
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
            if (int.TryParse(Scoid, out x))
            {
                cp.Scoid = int.Parse(Scoid);
            }
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
            if(!string.IsNullOrEmpty(SortDirection))
            {
                 if(SortDirection.ToUpper() == "ASC")
                {
                    cp.SortDirection = SortDirection;
                }
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
            if(!string.IsNullOrEmpty(SortDirection))
            {
                 if(SortDirection.ToUpper() == "ASC")
                {
                    cp.SortDirection = SortDirection;
                }
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

        [HttpPostAttribute("/Core/Purchase/CanclePur")]
        public ResponseResult CanclePur([FromBodyAttribute]JObject co)
        {   
            List<int> puridList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["PurIdList"].ToString());
            int CoId = int.Parse(GetCoid());
            var data = PurchaseHaddle.CanclePurchase(puridList,CoId);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Purchase/InsertPur")]
        public ResponseResult InsertPur([FromBodyAttribute]JObject co)
        {   
            var pur = Newtonsoft.Json.JsonConvert.DeserializeObject<Purchase>(co["Pur"].ToString());
            string UserName = GetUname(); 
            int CoID = int.Parse(GetCoid());
            var data = PurchaseHaddle.InsertPurchase(pur,UserName,CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Purchase/UpdatePur")]
        public ResponseResult UpdatePur([FromBodyAttribute]JObject co)
        {   
            var pur = Newtonsoft.Json.JsonConvert.DeserializeObject<Purchase>(co["Pur"].ToString());
            int CoID = int.Parse(GetCoid());
            var data = PurchaseHaddle.UpdatePurchase(pur,CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Purchase/ConfirmPur")]
        public ResponseResult ConfirmPur([FromBodyAttribute]JObject co)
        {   
            List<int> puridList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["PurIdList"].ToString());
            int CoId = int.Parse(GetCoid());
            var data = PurchaseHaddle.ConfirmPurchase(puridList,CoId);
            return CoreResult.NewResponse(data.s, data.d, "General");
        }

        [HttpGetAttribute("/Core/Purchase/PurchaseSingle")]
        public ResponseResult PurchaseSingle(string ID)
        {   
            int x,id = 0;
            var data = new DataResult(1,null);  
            int CoID = int.Parse(GetCoid());
            if (int.TryParse(ID, out x))
            {
                id = int.Parse(ID);
                data = PurchaseHaddle.GetPurchaseEdit(id,CoID);
            }
            else
            {
                data.s = -1;
                data.d = "参数无效!";
            }
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Purchase/CompletePur")]
        public ResponseResult CompletePur([FromBodyAttribute]JObject co)
        {   
            List<int> puridList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["PurIdList"].ToString());
            int CoId = int.Parse(GetCoid());
            var data = PurchaseHaddle.CompletePurchase(puridList,CoId);
            return CoreResult.NewResponse(data.s, data.d, "General");
        }

        [HttpPostAttribute("/Core/Purchase/InsertPurDetail")]
        public ResponseResult InsertPurDetail([FromBodyAttribute]JObject co)
        { 
            int id = int.Parse(co["purchaseid"].ToString());
            List<int> detailid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["ids"].ToString());
            int CoId = int.Parse(GetCoid());
            var data = PurchaseHaddle.InsertPurDetail(id,detailid,CoId);
            return CoreResult.NewResponse(data.s, data.d, "General");
        }

        [HttpPostAttribute("/Core/Purchase/UpdatePurDetail")]
        public ResponseResult UpdatePurDetail([FromBodyAttribute]JObject co)
        {   
            var detail = Newtonsoft.Json.JsonConvert.DeserializeObject<PurchaseDetail>(co["PurDetail"].ToString());
            int CoId = int.Parse(GetCoid());
            var data = PurchaseHaddle.UpdatePurDetail(detail,CoId);
            return CoreResult.NewResponse(data.s, data.d, "General");
        }

        [HttpPostAttribute("/Core/Purchase/DelPurDetail")]
        public ResponseResult DelPurDetail([FromBodyAttribute]JObject co)
        {   
            int id = int.Parse(co["ID"].ToString());
            List<int> detailid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["DetailID"].ToString());
            int CoId = int.Parse(GetCoid());
            var data = PurchaseHaddle.DeletePurDetail(id,detailid,CoId);
            return CoreResult.NewResponse(data.s, data.d, "General");
        }

        [HttpGetAttribute("/Core/Purchase/QualityRevList")]
        public ResponseResult QualityRevList(string Purid,string PageIndex,string NumPerPage)
        {   
            var data = new DataResult(1,null);
            int CoID = int.Parse(GetCoid());
            int x,pageindex,numperpage;
            if (int.TryParse(Purid, out x))
            {
                int id = int.Parse(Purid);
                if (int.TryParse(PageIndex, out x))
                {
                    pageindex = int.Parse(PageIndex);
                }
                else
                {
                    pageindex = 1;
                }
                if (int.TryParse(NumPerPage, out x))
                {
                    numperpage = int.Parse(NumPerPage);
                }
                else
                {
                    numperpage = 20;
                }
                data = PurchaseHaddle.QualityRevList(id,CoID,pageindex,numperpage);
            }
            else
            {
                data.s = -1;
                data.d = "参数无效!";
            }
            
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Purchase/InsertQualityRev")]
        public ResponseResult InsertQualityRev([FromBodyAttribute]JObject co)
        {   
            var qua = Newtonsoft.Json.JsonConvert.DeserializeObject<QualityRev>(co["Quality"].ToString());
            int CoId = int.Parse(GetCoid());
            string Username = GetUname();
            var data = PurchaseHaddle.InsertQualityRev(qua,CoId,Username);
            return CoreResult.NewResponse(data.s, data.d, "General");
        }

        [HttpPostAttribute("/Core/Purchase/UpdateQualityRev")]
        public ResponseResult UpdateQualityRev([FromBodyAttribute]JObject co)
        {   
            var qua = Newtonsoft.Json.JsonConvert.DeserializeObject<QualityRev>(co["Quality"].ToString());
            var data = PurchaseHaddle.UpdateQualityRev(qua);
            return CoreResult.NewResponse(data.s, data.d, "General");
        }

        [HttpPostAttribute("/Core/Purchase/DeleteQualityRev")]
        public ResponseResult DeleteQualityRev([FromBodyAttribute]JObject co)
        {   
            List<int> id = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["ID"].ToString());
            var data = PurchaseHaddle.DeleteQualityRev(id);
            return CoreResult.NewResponse(data.s, data.d, "General");
        }

        [HttpPostAttribute("/Core/Purchase/ConfirmQualityRev")]
        public ResponseResult ConfirmQualityRev([FromBodyAttribute]JObject co)
        {   
            int id = Newtonsoft.Json.JsonConvert.DeserializeObject<int>(co["ID"].ToString());
            var data = PurchaseHaddle.ConfirmQualityRev(id);
            return CoreResult.NewResponse(data.s, data.d, "General");
        }

        [HttpGetAttribute("/Core/Purchase/GetPurchaseInit")]
        public ResponseResult GetPurchaseInit()
        {   
            int CoID = int.Parse(GetCoid());
            var data = PurchaseHaddle.GetPurchaseInit(CoID);
            return CoreResult.NewResponse(data.s, data.d, "General");
        }

        [HttpPostAttribute("/Core/Purchase/UpdatePurRemark")]
        public ResponseResult UpdatePurRemark([FromBodyAttribute]JObject co)
        {   
            int id = int.Parse(co["ID"].ToString());
            string remark = co["Remark"].ToString();
            var data = PurchaseHaddle.UpdatePurRemark(id,remark);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Purchase/RestorePur")]
        public ResponseResult RestorePur([FromBodyAttribute]JObject co)
        {   
            List<int> puridList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["PurIdList"].ToString());
            int CoId = int.Parse(GetCoid());
            var data = PurchaseHaddle.RestorePur(puridList,CoId);
            return CoreResult.NewResponse(data.s, data.d, "General");
        }
    }
}
