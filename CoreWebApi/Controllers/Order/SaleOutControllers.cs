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
    public class SaleOutController : ControllBase
    {
        [HttpGetAttribute("/Core/SaleOut/GetSaleOutList")]
        public ResponseResult GetSaleOutList(string ID,string OID,string SoID,string ExCode,string DateStart,string Dateend,string Status,string IsWeightYN,string SkuID,
                                             string GoodsCode,string ExID,string IsExpPrint,string ShopID,string RecName,string BatchID,string SortField,
                                             string SortDirection,string PageIndex,string NumPerPage)
        {   
            int x;
            long l;
            var cp = new SaleOutParm();
            cp.CoID = int.Parse(GetCoid());
            if(!string.IsNullOrEmpty(ID))
            {
                if (int.TryParse(ID, out x))
                {
                    cp.ID = int.Parse(ID);
                }
            }
            if(!string.IsNullOrEmpty(OID))
            {
                if (int.TryParse(OID, out x))
                {
                    cp.OID = int.Parse(OID);
                }
            }
            if(!string.IsNullOrEmpty(SoID))
            {
                if (long.TryParse(SoID, out l))
                {
                    cp.SoID = long.Parse(SoID);
                }
            }
            cp.ExCode = ExCode;
            DateTime date;
            if (DateTime.TryParse(DateStart, out date))
            {
                cp.DateStart = DateTime.Parse(DateStart);
            }
            if (DateTime.TryParse(Dateend, out date))
            {
                cp.DateEnd = DateTime.Parse(Dateend);
            }
            if(!string.IsNullOrEmpty(Status))
            {
                if (int.TryParse(Status, out x))
                {
                    cp.Status = int.Parse(Status);
                }
            }
            cp.IsWeightYN = IsWeightYN;
            cp.SkuID = SkuID;
            cp.GoodsCode = GoodsCode;
            if(!string.IsNullOrEmpty(ExID))
            {
                if (int.TryParse(ExID, out x))
                {
                    cp.ExID = int.Parse(ExID);
                }
            }
            cp.IsExpPrint = IsExpPrint;
            if(!string.IsNullOrEmpty(ShopID))
            {
                if (int.TryParse(ShopID, out x))
                {
                    cp.ShopID = int.Parse(ShopID);
                }
            }
            cp.RecName = RecName;
            if(!string.IsNullOrEmpty(BatchID))
            {
                if (int.TryParse(BatchID, out x))
                {
                    cp.BatchID = int.Parse(BatchID);
                }
            }
            if(!string.IsNullOrEmpty(SortField))
            {
                if(CommHaddle.SysColumnExists(DbBase.CoreConnectString,"saleout",SortField).s == 1)
                {
                    cp.SortField = SortField;
                }
            }
            if(!string.IsNullOrEmpty(SortDirection))
            {
                 if(SortDirection.ToUpper() == "ASC" || SortDirection.ToUpper() == "DESC")
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
            var data = SaleOutHaddle.GetSaleOutList(cp);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/SaleOut/GetStatusInitData")]
        public ResponseResult GetStatusInitData()
        {   
            var data = SaleOutHaddle.GetStatusInitData();
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
    }
}