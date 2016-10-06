using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using CoreModels.XyCore;
using CoreData.CoreCore;
// using System;
// using System.Collections.Generic;
namespace CoreWebApi
{
    public class PurchaseReceiveController : ControllBase
    {
        [AllowAnonymous]
        [HttpPostAttribute("/Core/PurchaseReceive/PurchaseReceiveList")]
        public ResponseResult PurchaseReceiveList([FromBodyAttribute]JObject co)
        {   
            var cp = new PurchaseReceiveParm();
            cp.CoID = int.Parse(GetCoid());
            cp.Recid = co["Recid"].ToString();
            cp.Purid = co["Purid"].ToString();
            cp.Skuname = co["Skuname"].ToString();
            cp.Warehousename = co["Warehousename"].ToString();
            cp.Status = int.Parse(co["Status"].ToString());
            cp.SortField = co["SortField"].ToString();
            cp.SortDirection = co["SortDirection"].ToString();
            cp.NumPerPage = int.Parse(co["NumPerPage"].ToString());
            cp.PageIndex = int.Parse(co["PageIndex"].ToString());
            var data = PurchaseReceiveHaddle.GetPurchaseRecList(cp);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
    }
}