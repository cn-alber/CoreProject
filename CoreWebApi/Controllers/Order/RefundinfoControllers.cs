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

        [HttpPostAttribute("/Core/Refund/UpdateRefund")]
        public ResponseResult UpdateRefund([FromBodyAttribute]JObject co)
        {   
            DateTime RefundDate = DateTime.Parse("1900-01-01"),x;
            if(co["RefundDate"] != null)
            {
                string Text = co["RefundDate"].ToString();
                if (DateTime.TryParse(Text, out x))
                {
                    RefundDate = DateTime.Parse(Text);
                }
            }
            decimal Amount = -1,y;
            if(co["Amount"] != null)
            {
                string Text = co["Amount"].ToString();
                if (decimal.TryParse(Text, out y))
                {
                    Amount = decimal.Parse(Text);
                    if(Amount <= 0)
                    {
                        return CoreResult.NewResponse(-1, "金额必须大于零", "General");
                    }
                }
            }
            string RefundNbr = null,Refundment=null,PayAccount=null;
            if(co["RefundNbr"] != null)
            {
                RefundNbr = co["RefundNbr"].ToString();
            }
            if(co["Refundment"] != null)
            {
                Refundment = co["Refundment"].ToString();
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
            string UserName = GetUname();
            var data = RefundinfoHaddle.UpdateRefund(RefundDate,RefundNbr,Amount,Refundment,PayAccount,ID,CoID,UserName);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Refund/CancleRefund")]
        public ResponseResult CancleRefund([FromBodyAttribute]JObject co)
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
            var data = RefundinfoHaddle.CancleRefund(ID,CoID,UserName);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Refund/ComfirmRefund")]
        public ResponseResult ComfirmRefund([FromBodyAttribute]JObject co)
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
            var data = RefundinfoHaddle.ComfirmRefund(ID,CoID,UserName);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Refund/CancleComfirmRefund")]
        public ResponseResult CancleComfirmRefund([FromBodyAttribute]JObject co)
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
            var data = RefundinfoHaddle.CancleComfirmRefund(ID,CoID,UserName);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Refund/CompleteRefund")]
        public ResponseResult CompleteRefund([FromBodyAttribute]JObject co)
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
            var data = RefundinfoHaddle.CompleteRefund(ID,CoID,UserName);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
    }
}