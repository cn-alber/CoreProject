using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using CoreModels.XyCore;
using CoreData.CoreCore;
using System;
using CoreData.CoreComm;
using CoreData;
namespace CoreWebApi
{
    [AllowAnonymous]
    public class RefundinfoController : ControllBase
    {
        [HttpGetAttribute("/Core/Refund/GetRefundinfoList")]
        public ResponseResult GetRefundinfoList(string ID,string OID,string SoID,string RefundNbr,string DateType,string DateStart,string Dateend,string Status,string ShopID,
                                                string BuyerShopID,string Refundment,string Distributor,string SortField,string SortDirection,string PageIndex,string NumPerPage)
        {   
            int x;
            long l;
            var cp = new RefundInfoParm();
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
            cp.RefundNbr = RefundNbr;
            cp.DateType = DateType;
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
            if(!string.IsNullOrEmpty(ShopID))
            {
                if (int.TryParse(ShopID, out x))
                {
                    cp.ShopID = int.Parse(ShopID);
                }
            }
            cp.BuyerShopID = BuyerShopID;
            cp.Refundment = Refundment;
            cp.Distributor = Distributor;
            if(!string.IsNullOrEmpty(SortField))
            {
                if(CommHaddle.SysColumnExists(DbBase.CoreConnectString,"refundinfo",SortField).s == 1)
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
            var data = RefundinfoHaddle.GetRefundinfoList(cp);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Refund/GetRefundStatusInit")]
        public ResponseResult GetRefundStatusInit()
        {   
            var data = RefundinfoHaddle.GetRefundStatusInit();
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
    }
}