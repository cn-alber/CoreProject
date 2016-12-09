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
    public class PayinfoController : ControllBase
    {
        [HttpGetAttribute("/Core/Pay/GetPayinfoList")]
        public ResponseResult GetPayinfoList(string ID,string OID,string SoID,string PayNbr,string DateStart,string Dateend,string Status,string BuyerShopID,string Payment,
                                             string SortField,string SortDirection,string PageIndex,string NumPerPage)
        {   
            int x;
            long l;
            var cp = new PayInfoParm();
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
            cp.PayNbr = PayNbr;
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
            cp.BuyerShopID = BuyerShopID;
            cp.Payment = Payment;
            if(!string.IsNullOrEmpty(SortField))
            {
                if(CommHaddle.SysColumnExists(DbBase.CoreConnectString,"payinfo",SortField).s == 1)
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
            var data = PayinfoHaddle.GetPayinfoList(cp);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Pay/GetPayStatusInit")]
        public ResponseResult GetPayStatusInit()
        {   
            var data = PayinfoHaddle.GetPayStatusInit();
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Pay/UpdatePay")]
        public ResponseResult UpdatePay([FromBodyAttribute]JObject co)
        {   
            DateTime Paydate = DateTime.Parse("1900-01-01"),x;
            if(co["Paydate"] != null)
            {
                string Text = co["Paydate"].ToString();
                if (DateTime.TryParse(Text, out x))
                {
                    Paydate = DateTime.Parse(Text);
                }
            }
            decimal PayAmount = -1,y;
            if(co["PayAmount"] != null)
            {
                string Text = co["PayAmount"].ToString();
                if (decimal.TryParse(Text, out y))
                {
                    PayAmount = decimal.Parse(Text);
                    if(PayAmount <= 0)
                    {
                        return CoreResult.NewResponse(-1, "金额必须大于零", "General");
                    }
                }
            }
            string PayNbr = null,Payment=null,PayAccount=null;
            if(co["PayNbr"] != null)
            {
                PayNbr = co["PayNbr"].ToString();
            }
            if(co["Payment"] != null)
            {
                Payment = co["Payment"].ToString();
            }
            if(co["PayAccount"] != null)
            {
                PayAccount = co["PayAccount"].ToString();
            }
            int ID = 0,j;
            if(co["ID"] != null)
            {
                string Text = co["ID"].ToString();
                if (int.TryParse(Text, out j))
                {
                    ID = int.Parse(Text);
                }
                else
                {
                    return CoreResult.NewResponse(-1, "内部付款单号参数无效", "General");
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "内部付款单号必填", "General");
            }
            int CoID = int.Parse(GetCoid());
            var data = PayinfoHaddle.UpdatePay(Paydate,PayNbr,PayAmount,Payment,PayAccount,ID,CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Pay/CanclePay")]
        public ResponseResult CanclePay([FromBodyAttribute]JObject co)
        {   
            int ID = 0,j;
            if(co["ID"] != null)
            {
                string Text = co["ID"].ToString();
                if (int.TryParse(Text, out j))
                {
                    ID = int.Parse(Text);
                }
                else
                {
                    return CoreResult.NewResponse(-1, "内部付款单号参数无效", "General");
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "内部付款单号必填", "General");
            }
            int CoID = int.Parse(GetCoid());
            string UserName = GetUname();
            var data = PayinfoHaddle.CanclePay(ID,CoID,UserName);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
    }
}