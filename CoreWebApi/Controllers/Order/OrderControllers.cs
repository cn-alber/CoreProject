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
    public class OrderController : ControllBase
    {
        [HttpGetAttribute("/Core/Order/OrderList")]
        public ResponseResult OrderList(string ID,string SoID,string PayNbr,string BuyerShopID,string ExCode,string RecName,string RecPhone,string RecTel,
                                        string RecLogistics,string RecCity,string RecDistrict,string RecAddress,string StatusList,string AbnormalStatusList,
                                        string IsRecMsgYN,string RecMessage,string IsSendMsgYN,string SendMessage,string Datetype,string DateStart,string Dateend,
                                        string Skuid,string Ordqtystart,string Ordqtyend,string Ordamtstart,string Ordamtend,string Skuname,string Norm,
                                        string ShopStatus,string Osource,string Type,string IsCOD,string ShopID,string IsDisSelectAll,string Distributor,
                                        string ExID,string SendWarehouse,string SortField,string SortDirection,string PageIndex,string NumPerPage)
        {   
            int x;
            var cp = new OrderParm();
            cp.CoID = int.Parse(GetCoid());
            if(!string.IsNullOrEmpty(ID))
            {
                if (int.TryParse(ID, out x))
                {
                    cp.ID = int.Parse(ID);
                }
            }
            if(!string.IsNullOrEmpty(SoID))
            {
                if (int.TryParse(SoID, out x))
                {
                    cp.SoID = int.Parse(SoID);
                }
            }
            cp.PayNbr = PayNbr;
            cp.BuyerShopID = BuyerShopID;
            cp.ExCode = ExCode;
            cp.RecName = RecName;
            cp.RecPhone = RecPhone;
            cp.RecTel = RecTel;
            cp.RecLogistics = RecLogistics;
            cp.RecCity = RecCity;
            cp.RecDistrict = RecDistrict;
            cp.RecAddress = RecAddress;
            if(!string.IsNullOrEmpty(StatusList))
            {
                string[] a = StatusList.Split(',');
                foreach(var i in a)
                {
                    cp.StatusList.Add(int.Parse(i));
                }
            }
            if(!string.IsNullOrEmpty(AbnormalStatusList))
            {
                string[] a = AbnormalStatusList.Split(',');
                foreach(var i in a)
                {
                    cp.AbnormalStatusList.Add(int.Parse(i));
                }
            }
            if(!string.IsNullOrEmpty(IsRecMsgYN))
            {
                if(IsRecMsgYN.ToUpper() == "Y" || IsRecMsgYN.ToUpper() == "N")
                {
                    cp.IsRecMsgYN = IsRecMsgYN;
                }
            }
            cp.RecMessage = RecMessage;
            if(!string.IsNullOrEmpty(IsSendMsgYN))
            {
                if(IsSendMsgYN.ToUpper() == "Y" || IsSendMsgYN.ToUpper() == "N")
                {
                    cp.IsSendMsgYN = IsSendMsgYN;
                }
            }
            cp.SendMessage = SendMessage;
            if(!string.IsNullOrEmpty(Datetype))
            {
                if(Datetype.ToUpper() == "SENDDATE" || Datetype.ToUpper() == "PAYDATE" || Datetype.ToUpper() == "PLANDATE")
                {
                    cp.Datetype = Datetype;
                }
            }
            DateTime date;
            if (DateTime.TryParse(DateStart, out date))
            {
                cp.DateStart = DateTime.Parse(DateStart);
            }
            if (DateTime.TryParse(Dateend, out date))
            {
                cp.DateEnd = DateTime.Parse(Dateend);
            }
            cp.Skuid = Skuid;
            if(!string.IsNullOrEmpty(Ordqtystart))
            {
                if (int.TryParse(Ordqtystart, out x))
                {
                    cp.Ordqtystart = int.Parse(Ordqtystart);
                }
            }
            if(!string.IsNullOrEmpty(Ordqtyend))
            {
                if (int.TryParse(Ordqtyend, out x))
                {
                    cp.Ordqtyend = int.Parse(Ordqtyend);
                }
            }
            decimal y;
            if(!string.IsNullOrEmpty(Ordamtstart))
            {
                if (decimal.TryParse(Ordamtstart, out y))
                {
                    cp.Ordamtstart = decimal.Parse(Ordamtstart);
                }
            }
            if(!string.IsNullOrEmpty(Ordamtend))
            {
                if (decimal.TryParse(Ordamtend, out y))
                {
                    cp.Ordamtend = decimal.Parse(Ordamtend);
                }
            }
            cp.Skuname = Skuname;
            cp.Norm = Norm;
            if(!string.IsNullOrEmpty(ShopStatus))
            {
                string[] a = ShopStatus.Split(',');
                foreach(var i in a)
                {
                    cp.ShopStatus.Add(i);
                }
            }
            if(!string.IsNullOrEmpty(Osource))
            {
                if (int.TryParse(Osource, out x))
                {
                    cp.OSource = int.Parse(Osource);
                }
            }
            if(!string.IsNullOrEmpty(Type))
            {
                string[] a = Type.Split(',');
                foreach(var i in a)
                {
                    cp.Type.Add(int.Parse(i));
                }
            }
            if(!string.IsNullOrEmpty(IsCOD))
            {
                if(IsCOD.ToUpper() == "Y" || IsCOD.ToUpper() == "N")
                {
                    cp.IsCOD = IsCOD;
                }
            }
            if(!string.IsNullOrEmpty(ShopID))
            {
                string[] a = ShopID.Split(',');
                foreach(var i in a)
                {
                    cp.ShopID.Add(int.Parse(i));
                }
            }
            if(!string.IsNullOrEmpty(IsDisSelectAll))
            {
                if(IsDisSelectAll.ToUpper() == "TRUE")
                {
                    cp.IsDisSelectAll = true;
                }
            }
            if(!string.IsNullOrEmpty(Distributor))
            {
                string[] a = Distributor.Split(',');
                foreach(var i in a)
                {
                    cp.Distributor.Add(i);
                }
            }
            if(!string.IsNullOrEmpty(ExID))
            {
                string[] a = ExID.Split(',');
                foreach(var i in a)
                {
                    cp.ExID.Add(int.Parse(i));
                }
            }
            if(!string.IsNullOrEmpty(SendWarehouse))
            {
                string[] a = SendWarehouse.Split(',');
                foreach(var i in a)
                {
                    cp.SendWarehouse.Add(i);
                }
            }
            if(!string.IsNullOrEmpty(SortField))
            {
                if(CommHaddle.SysColumnExists(DbBase.CoreConnectString,"order",SortField).s == 1)
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
            var data = OrderHaddle.GetOrderList(cp);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/InsertOrder")]
        public ResponseResult InsertOrd([FromBodyAttribute]JObject co)
        {   
            var ord = Newtonsoft.Json.JsonConvert.DeserializeObject<Order>(co["Ord"].ToString());
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            string FaceToFace = co["IsFaceToFace"].ToString();
            bool IsFaceToFace = false;
            if(FaceToFace.ToUpper() == "TRUE")
            {
                IsFaceToFace = true;
            }
            var data = OrderHaddle.InsertOrder(ord,username,CoID,IsFaceToFace);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/UpdateOrder")]
        public ResponseResult UpdateOrder([FromBodyAttribute]JObject co)
        {   
            var ord = Newtonsoft.Json.JsonConvert.DeserializeObject<Order>(co["Ord"].ToString());
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.UpdateOrder(ord,username,CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Order/RecInfoList")]
        public ResponseResult RecInfoList(string BuyerId,string Receiver,string ShopSit,string SortField,string SortDirection,string PageIndex,string NumPerPage)
        {   
            int x;
            var cp = new RecInfoParm();
            cp.CoID = int.Parse(GetCoid());
            cp.BuyerId = BuyerId;
            cp.Receiver = Receiver;
            cp.ShopSit = ShopSit;
            if(!string.IsNullOrEmpty(SortField))
            {
                if(CommHaddle.SysColumnExists(DbBase.CoreConnectString,"recinfo",SortField).s == 1)
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
            var data = OrderHaddle.GetRecInfoList(cp);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/InsertOrderDetail")]
        public ResponseResult InsertOrderDetail([FromBodyAttribute]JObject co)
        {   
            int id = int.Parse(co["OID"].ToString());
            long soid = long.Parse(co["SoID"].ToString());
            List<int> skuid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["SkuIDList"].ToString());
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.InsertOrderDetail(id,soid,skuid,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/DeleteOrderDetail")]
        public ResponseResult DeleteOrderDetail([FromBodyAttribute]JObject co)
        {   
            int id = int.Parse(co["OID"].ToString());
            long soid = long.Parse(co["SoID"].ToString());
            List<int> skuid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["SkuIDList"].ToString());
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.DeleteOrderDetail(id,soid,skuid,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/UpdateOrderDetail")]
        public ResponseResult UpdateOrderDetail([FromBodyAttribute]JObject co)
        {   
            int id = int.Parse(co["OID"].ToString());
            long soid = long.Parse(co["SoID"].ToString());
            int skuid = int.Parse(co["SkuAutoID"].ToString());
            decimal price = -1;
            int qty = -1;
            if(!string.IsNullOrEmpty(co["Price"].ToString()))
            {
                price = decimal.Parse(co["Price"].ToString());
            }
            if(!string.IsNullOrEmpty(co["Qty"].ToString()))
            {
                qty = int.Parse(co["Qty"].ToString());
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.UpdateOrderDetail(id,soid,skuid,CoID,username,price,qty);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/InsertManualPay")]
        public ResponseResult InsertManualPay([FromBodyAttribute]JObject co)
        {   
            var data = new DataResult(1,null);
            var pay = new PayInfo();
            if(string.IsNullOrEmpty(co["Payment"].ToString()))
            {
                data.s = -1;
                data.d = "支付方式必须有值!";
                return CoreResult.NewResponse(data.s, data.d, "General"); 
            }
            else
            {
                pay.Payment = co["Payment"].ToString();
            }
            if(string.IsNullOrEmpty(co["PayNbr"].ToString()))
            {
                if(pay.Payment != "现金支付")
                {
                    data.s = -1;
                    data.d = "支付单号必须有值!";
                    return CoreResult.NewResponse(data.s, data.d, "General"); 
                }
            }
            else
            {
                pay.PayNbr = co["PayNbr"].ToString();
            }
            pay.OID = int.Parse(co["OID"].ToString());
            pay.SoID = long.Parse(co["SoID"].ToString());
            if(!string.IsNullOrEmpty(co["PayAccount"].ToString()))
            {
                pay.PayAccount = co["PayAccount"].ToString();
            }
            if(string.IsNullOrEmpty(co["PayDate"].ToString()))
            {
                data.s = -1;
                data.d = "支付日期必须有值!";
                return CoreResult.NewResponse(data.s, data.d, "General"); 
            }
            else
            {
                pay.PayDate = DateTime.Parse(co["PayDate"].ToString());
            }
            if(string.IsNullOrEmpty(co["PayAmount"].ToString()))
            {
                data.s = -1;
                data.d = "支付金额必须有值!";
                return CoreResult.NewResponse(data.s, data.d, "General"); 
            }
            else
            {
                pay.PayAmount = co["PayAmount"].ToString();
                pay.Amount = co["PayAmount"].ToString();
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            data = OrderHaddle.ManualPay(pay,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/CancleConfirmPay")]
        public ResponseResult CancleConfirmPay([FromBodyAttribute]JObject co)
        {   
            int OID = int.Parse(co["OID"].ToString());
            long SoID = long.Parse(co["SoID"].ToString());
            int payid = int.Parse(co["PayID"].ToString());
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.CancleConfirmPay(OID,SoID,payid,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/ConfirmPay")]
        public ResponseResult ConfirmPay([FromBodyAttribute]JObject co)
        {   
            int OID = int.Parse(co["OID"].ToString());
            long SoID = long.Parse(co["SoID"].ToString());
            int payid = int.Parse(co["PayID"].ToString());
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.ConfirmPay(OID,SoID,payid,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/CanclePay")]
        public ResponseResult CanclePay([FromBodyAttribute]JObject co)
        {   
            int OID = int.Parse(co["OID"].ToString());
            long SoID = long.Parse(co["SoID"].ToString());
            int payid = int.Parse(co["PayID"].ToString());
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.CanclePay(OID,SoID,payid,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/QuickPay")]
        public ResponseResult QuickPay([FromBodyAttribute]JObject co)
        {   
            int OID = int.Parse(co["OID"].ToString());
            long SoID = long.Parse(co["SoID"].ToString());
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.QuickPay(OID,SoID,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Order/GetAbnormalList")]
        public ResponseResult GetAbnormalList()
        {   
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.GetAbnormalList(CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
    }
}