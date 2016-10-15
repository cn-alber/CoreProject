using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using CoreModels.XyCore;
using CoreData.CoreCore;
using System;
using CoreData.CoreComm;
using CoreData;
using System.Collections.Generic;
using CoreModels;
namespace CoreWebApi
{
    [AllowAnonymous]
    public class PurchaseReceiveController : ControllBase
    {
        [HttpGetAttribute("/Core/PurchaseReceive/PurchaseReceiveList")]//抓取收料单List
        public ResponseResult PurchaseReceiveList(string Purid,string IsNotPur,string RecdateStart,string RecdateEnd,string Status,string FinStatus,string Scoid,string Skuid,string Remark,string SortField,string SortDirection,string PageIndex,string NumPerPage)
        {   
            int x;
            var cp = new PurchaseReceiveParm();
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
            if(!string.IsNullOrEmpty(IsNotPur))
            {
                if(IsNotPur.ToUpper()!="true")
                {
                    cp.IsNotPur = true;
                }
            }
            DateTime date;
            if (DateTime.TryParse(RecdateStart, out date))
            {
                cp.RecdateStart = DateTime.Parse(RecdateStart);
            }
            if (DateTime.TryParse(RecdateEnd, out date))
            {
                cp.RecdateEnd = DateTime.Parse(RecdateEnd);
            }
            if (int.TryParse(Status, out x))
            {
                cp.Status = int.Parse(Status);
            }
            if (int.TryParse(FinStatus, out x))
            {
                cp.FinStatus = int.Parse(FinStatus);
            }
            if (int.TryParse(Scoid, out x))
            {
                cp.Scoid = int.Parse(Scoid);
            }
            cp.Skuid = Skuid;
            cp.Remark = Remark;
            if(CommHaddle.SysColumnExists(DbBase.CoreConnectString,"purchasereceive",SortField).s == 1)
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
            var data = PurchaseReceiveHaddle.GetPurchaseRecList(cp);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/PurchaseReceive/PurchaseRecDetailList")]//抓取收料单明细List
        public ResponseResult PurchaseRecDetailList(string Recid,string SortField,string SortDirection,string PageIndex,string NumPerPage)
        {
            var cp = new PurchaseRecDetailParm();
            cp.CoID = int.Parse(GetCoid());
            int x;
            if (int.TryParse(Recid, out x))
            {
                cp.Recid = int.Parse(Recid);
            }
            if(CommHaddle.SysColumnExists(DbBase.CoreConnectString,"purchaserecdetail",SortField).s == 1)
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
            var data = PurchaseReceiveHaddle.GetPurchaseRecDetailList(cp);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/PurchaseReceive/CancleReceive")]
        public ResponseResult CancleReceive([FromBodyAttribute]JObject co)
        {   
            List<int> recidList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["RecIdList"].ToString());
            int CoId = int.Parse(GetCoid());
            var data = PurchaseReceiveHaddle.CancleReceive(recidList,CoId);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/PurchaseReceive/InsertRec")]
        public ResponseResult InsertRec([FromBodyAttribute]JObject co)
        {   
            int id = int.Parse(co["ID"].ToString());
            string type = co["Type"].ToString();
            string UserName = GetUname(); 
            int CoID = int.Parse(GetCoid());
            var data = PurchaseReceiveHaddle.InsertReceive(id,type,UserName,CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/PurchaseReceive/UpdateRec")]
        public ResponseResult UpdateRec([FromBodyAttribute]JObject co)
        {   
            var pur = Newtonsoft.Json.JsonConvert.DeserializeObject<PurchaseReceive>(co["Rec"].ToString());
            int CoID = int.Parse(GetCoid());
            var data = PurchaseReceiveHaddle.UpdateReceive(pur,CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        // [HttpPostAttribute("/Core/PurchaseReceive/ConfirmRec")]
        // public ResponseResult ConfirmRec([FromBodyAttribute]JObject co)
        // {   
        //     List<int> puridList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["PurIdList"].ToString());
        //     int CoId = int.Parse(GetCoid());
        //     var data = PurchaseHaddle.ConfirmPurchase(puridList,CoId);
        //     return CoreResult.NewResponse(data.s, data.d, "General");
        // }

        // [HttpPostAttribute("/Core/PurchaseReceive/FinConfirmRec")]
        // public ResponseResult FinConfirmRec([FromBodyAttribute]JObject co)
        // {   
        //     List<int> puridList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["PurIdList"].ToString());
        //     int CoId = int.Parse(GetCoid());
        //     var data = PurchaseHaddle.CompletePurchase(puridList,CoId);
        //     return CoreResult.NewResponse(data.s, data.d, "General");
        // }

        [HttpPostAttribute("/Core/PurchaseReceive/InsertRecDetail")]
        public ResponseResult InsertRecDetail([FromBodyAttribute]JObject co)
        {   
            int id = int.Parse(co["recid"].ToString());
            List<int> detailid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["ids"].ToString());
            int CoId = int.Parse(GetCoid());
            var data = PurchaseReceiveHaddle.InsertRecDetail(id,detailid,CoId);
            return CoreResult.NewResponse(data.s, data.d, "General");
        }

        [HttpPostAttribute("/Core/PurchaseReceive/UpdateRecDetail")]
        public ResponseResult UpdateRecDetail([FromBodyAttribute]JObject co)
        {   
            var detail = Newtonsoft.Json.JsonConvert.DeserializeObject<PurchaseRecDetail>(co["RecDetail"].ToString());
            int CoId = int.Parse(GetCoid());
            var data = PurchaseReceiveHaddle.UpdateRecDetail(detail,CoId);
            return CoreResult.NewResponse(data.s, data.d, "General");
        }

        [HttpPostAttribute("/Core/PurchaseReceive/DelRecDetail")]
        public ResponseResult DelRecDetail([FromBodyAttribute]JObject co)
        {   
            int recid = int.Parse(co["Recid"].ToString());
            List<int> detailid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["DetailID"].ToString());
            int CoID = int.Parse(GetCoid());
            var data = PurchaseReceiveHaddle.DelRecDetail(detailid,recid,CoID);
            return CoreResult.NewResponse(data.s, data.d, "General");
        }

        [HttpGetAttribute("/Core/PurchaseReceive/PurReceiveSingle")]
        public ResponseResult PurReceiveSingle(string ID)
        {   
            int x,id = 0;
            var data = new DataResult(1,null);  
            int CoID = int.Parse(GetCoid());
            if (int.TryParse(ID, out x))
            {
                id = int.Parse(ID);
                data = PurchaseReceiveHaddle.PurReceiveSingle(id,CoID);
            }
            else
            {
                data.s = -1;
                data.d = "参数无效!";
            }
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/PurchaseReceive/UpdateRecRemark")]
        public ResponseResult UpdateRecRemark([FromBodyAttribute]JObject co)
        {   
            List<int> id = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["IDList"].ToString());
            string remark = co["Remark"].ToString();
            var data = PurchaseReceiveHaddle.UpdateRecRemark(id,remark);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/PurchaseReceive/GetPurchaseRecInit")]
        public ResponseResult GetPurchaseRecInit()
        {   
            int CoID = int.Parse(GetCoid());
            var data = PurchaseReceiveHaddle.GetPurchaseRecInit(CoID);
            return CoreResult.NewResponse(data.s, data.d, "General");
        }

        [HttpPostAttribute("/Core/PurchaseReceive/ConfirmPurRec")]
        public ResponseResult ConfirmPurRec([FromBodyAttribute]JObject co)
        {   
            List<int> recidList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["RecIdList"].ToString());
            int CoId = int.Parse(GetCoid());
            var data = PurchaseReceiveHaddle.ConfirmPurRec(recidList,CoId);
            return CoreResult.NewResponse(data.s, data.d, "General");
        }
    }
}