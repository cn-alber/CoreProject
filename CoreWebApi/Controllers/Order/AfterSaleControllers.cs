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
    public class AfterSaleController : ControllBase
    {
        [HttpGetAttribute("/Core/AfterSale/GetInitASData")]
        public ResponseResult GetInitASData()
        {
            var data = AfterSaleHaddle.GetInitASData(int.Parse(GetCoid()));
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
    }
}