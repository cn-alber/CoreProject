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
                                        string Skuid,string GoodsCode,string Ordqtystart,string Ordqtyend,string Ordamtstart,string Ordamtend,string Skuname,string Norm,
                                        string ShopStatus,string Osource,string Type,string IsCOD,string IsPaid,string IsShopSelectAll,string ShopID,string IsDisSelectAll,string Distributor,
                                        string ExID,string SendWarehouse,string Others,string SortField,string SortDirection,string PageIndex,string NumPerPage)
        {   
            int x;
            long l;
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
                if (long.TryParse(SoID, out l))
                {
                    cp.SoID = long.Parse(SoID);
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
                List<int> s = new List<int>();
                foreach(var i in a)
                {
                    s.Add(int.Parse(i));
                }
                cp.StatusList = s;
            }
            if(!string.IsNullOrEmpty(AbnormalStatusList))
            {
                string[] a = AbnormalStatusList.Split(',');
                List<int> s = new List<int>();
                foreach(var i in a)
                {
                    s.Add(int.Parse(i));
                }
                cp.AbnormalStatusList = s;
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
            cp.GoodsCode = GoodsCode;
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
                List<string> s = new List<string>();
                foreach(var i in a)
                {
                    s.Add(i);
                }
                cp.ShopStatus = s;
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
                List<int> s = new List<int>();
                foreach(var i in a)
                {
                    s.Add(int.Parse(i));
                }
                cp.Type = s;
            }
            if(!string.IsNullOrEmpty(IsCOD))
            {
                if(IsCOD.ToUpper() == "Y" || IsCOD.ToUpper() == "N")
                {
                    cp.IsCOD = IsCOD;
                }
            }
            if(!string.IsNullOrEmpty(IsPaid))
            {
                if(IsPaid.ToUpper() == "Y" || IsPaid.ToUpper() == "N")
                {
                    cp.IsPaid = IsPaid;
                }
            }
            if(!string.IsNullOrEmpty(IsShopSelectAll))
            {
                if(IsShopSelectAll.ToUpper() == "TRUE")
                {
                    cp.IsShopSelectAll = true;
                }
            }
            if(!string.IsNullOrEmpty(ShopID))
            {
                string[] a = ShopID.Split(',');
                List<int> s = new List<int>();
                foreach(var i in a)
                {
                    s.Add(int.Parse(i));
                }
                cp.ShopID = s;
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
                List<string> s = new List<string>();
                foreach(var i in a)
                {
                    s.Add(i);
                }
                cp.Distributor = s;
            }
            if(!string.IsNullOrEmpty(ExID))
            {
                string[] a = ExID.Split(',');
                List<int> s = new List<int>();
                foreach(var i in a)
                {
                    s.Add(int.Parse(i));
                }
                cp.ExID = s;
            }
            if(!string.IsNullOrEmpty(SendWarehouse))
            {
                string[] a = SendWarehouse.Split(',');
                List<int> s = new List<int>();
                foreach(var i in a)
                {
                    s.Add(int.Parse(i));
                }
                cp.SendWarehouse = s;
            }
            if(!string.IsNullOrEmpty(Others))
            {
                string[] a = Others.Split(',');
                List<int> s = new List<int>();
                foreach(var i in a)
                {
                    s.Add(int.Parse(i));
                }
                cp.Others = s;
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
            var ord = new Order();
            if(co["BuyerShopID"] != null)
            {
                ord.BuyerShopID = co["BuyerShopID"].ToString();
            }
            if(co["RecName"] != null)
            {
                ord.RecName = co["RecName"].ToString();
            }
            if(co["RecLogistics"] != null)
            {
                ord.RecLogistics = co["RecLogistics"].ToString();
            }
            if(co["RecCity"] != null)
            {
                ord.RecCity = co["RecCity"].ToString();
            }
            if(co["RecDistrict"] != null)
            {
                ord.RecDistrict = co["RecDistrict"].ToString();
            }
            if(co["RecAddress"] != null)
            {
                ord.RecAddress = co["RecAddress"].ToString();
            }
            if(co["RecPhone"] != null)
            {
                ord.RecPhone = co["RecPhone"].ToString();
            }
            if(co["RecTel"] != null)
            {
                ord.RecTel = co["RecTel"].ToString();
            }
            if(co["RecMessage"] != null)
            {
                ord.RecMessage = co["RecMessage"].ToString();
            }
            if(co["SendMessage"] != null)
            {
                ord.SendMessage = co["SendMessage"].ToString();
            }
            if(co["ODate"] != null)
            {
                string text = co["ODate"].ToString();
                DateTime x;
                if (DateTime.TryParse(text, out x))
                {
                    ord.ODate = DateTime.Parse(text);
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单日期必填", "General"); 
            }
            if(co["SoID"] != null)
            {
                string text = co["SoID"].ToString();
                long y;
                if (long.TryParse(text, out y))
                {
                    ord.SoID = long.Parse(text);
                }
            }   
            if(co["ExAmount"] != null)
            {
                string text = co["ExAmount"].ToString();
                decimal z;
                if (decimal.TryParse(text, out z))
                {
                    ord.ExAmount = text;
                }
            }
            if(co["ShopID"] != null)
            {
                string text = co["ShopID"].ToString();
                int i;
                if (int.TryParse(text, out i))
                {
                    ord.ShopID = int.Parse(text);
                }     
            }
            else
            {
                return CoreResult.NewResponse(-1, "店铺必填", "General"); 
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            bool IsFaceToFace = false;
            if(co["IsFaceToFace"] != null)
            {
                string FaceToFace = co["IsFaceToFace"].ToString();
                if(FaceToFace.ToUpper() == "TRUE")
                {
                    IsFaceToFace = true;
                }
            }
            var data = OrderHaddle.InsertOrder(ord,username,CoID,IsFaceToFace);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/UpdateOrder")]
        public ResponseResult UpdateOrder([FromBodyAttribute]JObject co)
        {   
            string ExAmount = null,Remark=null,InvTitle=null,Logistics=null,City=null,District=null,Address=null,Name=null,tel=null,phone=null;
            int OID = 0;
            if(co["ExAmount"] != null)
            {
                ExAmount = co["ExAmount"].ToString();
            }
            if(co["SendMessage"] != null)
            {
                Remark = co["SendMessage"].ToString();
            }
            if(co["InvoiceTitle"] != null)
            {
                InvTitle = co["InvoiceTitle"].ToString();
            }
            if(co["RecLogistics"] != null)
            {
                Logistics = co["RecLogistics"].ToString();
            }
            if(co["RecCity"] != null)
            {
                City = co["RecCity"].ToString();
            }
            if(co["RecDistrict"] != null)
            {
                District = co["RecDistrict"].ToString();
            }
            if(co["RecAddress"] != null)
            {
                Address = co["RecAddress"].ToString();
            }
            if(co["RecName"] != null)
            {
                Name = co["RecName"].ToString();
            }
            if(co["RecTel"] != null)
            {
                tel = co["RecTel"].ToString();
            }
            if(co["RecPhone"] != null)
            {
                phone = co["RecPhone"].ToString();
            }
            if(co["OID"] != null)
            {
                OID = int.Parse(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单号必填", "General");
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.UpdateOrder(ExAmount,Remark,InvTitle,Logistics,City,District,Address,Name,tel,phone,OID,username,CoID);
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
            if(!string.IsNullOrEmpty(ShopSit))
            {
                if (int.TryParse(ShopSit, out x))
                {
                    cp.ShopSit = int.Parse(ShopSit);
                }
            }
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
            int id = 0;
            var skuid = new List<int>();
            var data = new DataResult(1,null);
            if(co["OID"] != null)
            {
                id = int.Parse(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单号必填", "General");
            }
            if(co["SkuIDList"] != null)
            {
                skuid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["SkuIDList"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "商品ID必填", "General");
            }
            bool isQuick;
            if(co["isQuick"] != null)
            {
                string isquick = co["isQuick"].ToString();
                if(isquick.ToUpper() != "TRUE" && isquick.ToUpper() != "FALSE")
                {
                    data.s = -1;
                    data.d = "明细新增方式参数无效!";
                    return CoreResult.NewResponse(data.s, data.d, "General"); 
                }
                else
                {
                    isQuick = bool.Parse(isquick);
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "明细新增方式必填", "General");
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            data = OrderHaddle.InsertOrderDetail(id,skuid,CoID,username,isQuick);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/DeleteOrderDetail")]
        public ResponseResult DeleteOrderDetail([FromBodyAttribute]JObject co)
        {   
            int id=0,oid=0;
            if(co["ID"] != null)
            {
                id = int.Parse(co["ID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "明细ID必填", "General");
            }
            if(co["OID"] != null)
            {
                oid = int.Parse(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单号必填", "General");
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = new DataResult(1,null);
            bool isQuick;
            if(co["isQuick"] != null)
            {
                string isquick = co["isQuick"].ToString();
                if(isquick.ToUpper() != "TRUE" && isquick.ToUpper() != "FALSE")
                {
                    data.s = -1;
                    data.d = "明细删除方式参数无效!";
                    return CoreResult.NewResponse(data.s, data.d, "General"); 
                }
                else
                {
                    isQuick = bool.Parse(isquick);
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "明细删除方式必填", "General");
            }
            data = OrderHaddle.DeleteOrderDetail(id,oid,CoID,username,isQuick);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/UpdateOrderDetail")]
        public ResponseResult UpdateOrderDetail([FromBodyAttribute]JObject co)
        {   
            decimal price = -1;
            int qty = -1,id=0,oid=0;
            if(co["ID"] != null)
            {
                id = int.Parse(co["ID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单明细ID必填", "General");
            }
            if(co["OID"] != null)
            {
                oid = int.Parse(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单号必填", "General");
            }
            if(co["Price"] != null)
            {
                price = decimal.Parse(co["Price"].ToString());
                if(price < 0)
                {
                    return CoreResult.NewResponse(-1, "单价必须大于等于0", "General");
                }
            }
            if(co["Qty"] != null)
            {
                qty = int.Parse(co["Qty"].ToString());
                if(qty < 0)
                {
                    return CoreResult.NewResponse(-1, "数量必须大于等于0", "General");
                }
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            string SkuName = null;
            if(co["SkuName"] != null)
            {
                SkuName = co["SkuName"].ToString();
            }
            var data = new DataResult(1,null);
            bool isQuick;
            if(co["isQuick"] != null)
            {
                string isquick = co["isQuick"].ToString();
                if(isquick.ToUpper() != "TRUE" && isquick.ToUpper() != "FALSE")
                {
                    data.s = -1;
                    data.d = "明细修改方式参数无效!";
                    return CoreResult.NewResponse(data.s, data.d, "General"); 
                }
                else
                {
                    isQuick = bool.Parse(isquick);
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "明细修改方式必填", "General");
            }
            data = OrderHaddle.UpdateOrderDetail(id,oid,CoID,username,price,qty,SkuName,isQuick);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/InsertManualPay")]
        public ResponseResult InsertManualPay([FromBodyAttribute]JObject co)
        {   
            var data = new DataResult(1,null);
            var pay = new PayInfo();
            if(co["Payment"] != null)
            {
                pay.Payment = co["Payment"].ToString();
            }
            else
            {
                return CoreResult.NewResponse(-1, "支付方式必填", "General");
            }
            if(co["PayNbr"] != null)
            {
                pay.PayNbr = co["PayNbr"].ToString();
            }
            else
            {
                if(pay.Payment != "现金支付")
                {
                    return CoreResult.NewResponse(-1, "支付单号必填", "General");
                }
                
            }
            if(co["OID"] != null)
            {
                pay.OID = int.Parse(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单号必填", "General");
            }
            if(co["PayAccount"] != null)
            {
                pay.PayAccount = co["PayAccount"].ToString();
            }
            if(co.GetValue("PayDate") == null)
            {
                return CoreResult.NewResponse(-1, "支付日期必填", "General");
            }
            else
            {
                pay.PayDate = DateTime.Parse(co["PayDate"].ToString());
            }
            if(co.GetValue("PayAmount") == null)
            {
                return CoreResult.NewResponse(-1, "支付金额必填", "General");
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
            int OID = 0,payid = 0;
            if(co["OID"] != null)
            {
                OID = int.Parse(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单号必填", "General");
            }
            if(co["PayID"] != null)
            {
                payid = int.Parse(co["PayID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "付款ID必填", "General");
            }
            string username = GetUname();
            int CoID =  int.Parse(GetCoid());
            var data = OrderHaddle.CancleConfirmPay(OID,payid,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/ConfirmPay")]
        public ResponseResult ConfirmPay([FromBodyAttribute]JObject co)
        {   
            int OID = 0,payid = 0;
            if(co["OID"] != null)
            {
                OID = int.Parse(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单号必填", "General");
            }
            if(co["PayID"] != null)
            {
                payid = int.Parse(co["PayID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "付款ID必填", "General");
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.ConfirmPay(OID,payid,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/CanclePay")]
        public ResponseResult CanclePay([FromBodyAttribute]JObject co)
        {   
            int OID = 0,payid = 0;
            if(co["OID"] != null)
            {
                OID = int.Parse(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单号必填", "General");
            }
            if(co["PayID"] != null)
            {
                payid = int.Parse(co["PayID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "付款ID必填", "General");
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.CanclePay(OID,payid,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/QuickPay")]
        public ResponseResult QuickPay([FromBodyAttribute]JObject co)
        {   
            int OID = 0;
            if(co["OID"] != null)
            {
                OID = int.Parse(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单号必填", "General");
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.QuickPay(OID,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Order/GetAbnormalList")]
        public ResponseResult GetAbnormalList()
        {   
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.GetAbnormalList(CoID,7);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/TransferNormal")]
        public ResponseResult TransferNormal([FromBodyAttribute]JObject co)
        {   
            var oid = new List<int>();
            if(co["OID"] != null)
            {
                oid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单号必填", "General");
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.TransferNormal(oid,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Order/GetMergeOrd")]
        public ResponseResult GetMergeOrd(string OID)
        {   
            int x,oid = 0;
            var data = new DataResult(1,null);
            if (int.TryParse(OID, out x))
            {
                oid = int.Parse(OID);
            }
            else
            {
                data.s = -1;
                data.d = "参数无效!";
                return CoreResult.NewResponse(data.s, data.d, "General"); 
            }
            int CoID = int.Parse(GetCoid());
            data = OrderHaddle.GetMergeOrd(oid,CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/OrdMerger")]
        public ResponseResult OrdMerger([FromBodyAttribute]JObject co)
        {   
            int oid = 0;
            var merid = new  List<int>();
            if(co["OID"] != null)
            {
                oid = int.Parse(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "主订单号必填", "General");
            }
            if(co["MerID"] != null)
            {
                merid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["MerID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "合并子订单号必填", "General");
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.OrdMerger(oid,merid,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/CancleOrdMerge")]
        public ResponseResult CancleOrdMerge([FromBodyAttribute]JObject co)
        {   
            var oid = new List<int>();
            string Type = "";
            if(co["Type"] != null)
            {
                Type = co["Type"].ToString();
                if(Type.ToUpper() != "A" && Type.ToUpper() != "B")
                {
                    return CoreResult.NewResponse(-1, "请选择合并选项", "General");
                }
                if(Type.ToUpper() == "A")
                {
                    if(co["OID"] != null)
                    {
                        oid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["OID"].ToString());
                    }
                    else
                    {
                        return CoreResult.NewResponse(-1, "请选择需还原合并的订单", "General");
                    }
                }
            }
            else
            {   
                return CoreResult.NewResponse(-1, "请选择合并选项", "General");
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.CancleOrdMerge(oid,Type,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/OrdSplit")]
        public ResponseResult OrdSplit([FromBodyAttribute]JObject co)
        {   
            int oid = 0;
            var splitOrd = new List<SplitOrd>();
            if(co["OID"] != null)
            {
                oid = int.Parse(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单号必填", "General");
            }
            if(co["SplitOrd"] != null)
            {
                splitOrd = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SplitOrd>>(co["SplitOrd"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "拆分订单明细必填", "General");
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid()); 
            var data = OrderHaddle.OrdSplit(oid,splitOrd,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/ModifyFreight")]
        public ResponseResult ModifyFreight([FromBodyAttribute]JObject co)
        {   
            decimal freight = 0;
            var oid = new List<int>();
            if(co["Freight"] != null)
            {
                freight = decimal.Parse(co["Freight"].ToString());
            }
            if(co["OID"] != null)
            {
                oid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单号必填", "General");
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid()); 
            var data = OrderHaddle.ModifyFreight(oid,freight,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Order/GetInitData")]
        public ResponseResult GetInitData()
        {   
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.GetInitData(CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Order/GetStatusCount")]
        public ResponseResult GetStatusCount()
        {   
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.GetStatusCount(CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
        
        [HttpPostAttribute("/Core/Order/ImportOrderInsert")]
        public ResponseResult ImportOrderInsert([FromBodyAttribute]JObject co)
        {   
            var order = new ImportOrderInsert();
            if(co.GetValue("Order") != null)
            {
                order = Newtonsoft.Json.JsonConvert.DeserializeObject<ImportOrderInsert>(co["Order"].ToString());
            }   
            string username = GetUname();
            int CoID = int.Parse(GetCoid()); 
            var data = OrderHaddle.ImportOrderInsert(order,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/ImportOrderUpdate")]
        public ResponseResult ImportOrderUpdate([FromBodyAttribute]JObject co)
        {   
            var order = new ImportOrderUpdate();
            if(co.GetValue("Order") != null)
            {
                order = Newtonsoft.Json.JsonConvert.DeserializeObject<ImportOrderUpdate>(co["Order"].ToString());
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid()); 
            var data = OrderHaddle.ImportOrderUpdate(order,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/InsertGift")]
        public ResponseResult InsertGift([FromBodyAttribute]JObject co)
        {   
            int id = 0;
            var skuid = new List<int>();
            if(co["OID"] != null)
            {
                id = int.Parse(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单号必填", "General");
            }
            if(co["SkuIDList"] != null)
            {
                skuid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["SkuIDList"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "商品ID必填", "General");
            } 
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.InsertGift(id,skuid,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/ChangeOrderDetail")]
        public ResponseResult ChangeOrderDetail([FromBodyAttribute]JObject co)
        {   
            int id = 0,oid = 0,Skuid = 0;
            if(co["ID"] != null)
            {
                id = int.Parse(co["ID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单明细ID必填", "General");
            } 
            if(co["OID"] != null)
            {
                oid = int.Parse(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单号必填", "General");
            } 
            if(co["SkuID"] != null)
            {
                Skuid = int.Parse(co["SkuID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "商品ID必填", "General");
            } 
            
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.ChangeOrderDetail(id,oid,Skuid,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
        
        [HttpGetAttribute("/Core/Order/GetOrderSingle")]
        public ResponseResult GetOrderSingle(string OID)
        {   
            int x,oid = 0;
            var data = new DataResult(1,null);
            if (int.TryParse(OID, out x))
            {
                oid = int.Parse(OID);
            }
            else
            {
                data.s = -1;
                data.d = "参数无效!";
                return CoreResult.NewResponse(data.s, data.d, "General"); 
            }
            int CoID = int.Parse(GetCoid());
            data = OrderHaddle.GetOrderSingle(oid,CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/ModifyRemark")]
        public ResponseResult ModifyRemark([FromBodyAttribute]JObject co)
        {   
            int oid = 0;
            if(co["OID"] != null)
            {
                oid = int.Parse(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单号必填", "General");
            } 
            string Remark = "";
            if(co["SendMessage"] != null)
            {
                Remark = co["SendMessage"].ToString();
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.ModifyRemark(oid,CoID,username,Remark);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/ModifyAddress")]
        public ResponseResult ModifyAddress([FromBodyAttribute]JObject co)
        {   
            string Logistics = "",City = "",District = "",Address = "",Name = "",tel = "",phone = "";
            if(co["RecLogistics"] != null)
            {
                Logistics = co["RecLogistics"].ToString();
            }
            else
            {
                return CoreResult.NewResponse(-1, "省份必填", "General");
            } 
            if(co["RecCity"] != null)
            {
                City = co["RecCity"].ToString();
            }
            else
            {
                return CoreResult.NewResponse(-1, "市区必填", "General");
            } 
            if(co["RecDistrict"] != null)
            {
                District = co["RecDistrict"].ToString();
            }
            else
            {
                return CoreResult.NewResponse(-1, "县级市必填", "General");
            } 
            if(co["RecAddress"] != null)
            {
                Address = co["RecAddress"].ToString();
            }
            else
            {
                return CoreResult.NewResponse(-1, "地址必填", "General");
            } 
            if(co["RecName"] != null)
            {
                Name = co["RecName"].ToString();
            }
            else
            {
                return CoreResult.NewResponse(-1, "收货人必填", "General");
            } 
            if(co["RecTel"] != null)
            {
                tel = co["RecTel"].ToString();
            }
            if(co["RecPhone"] != null)
            {
                phone = co["RecPhone"].ToString();
            }
            int OID = 0;
            if(co["OID"] != null)
            {
                OID = int.Parse(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单号必填", "General");
            }                       
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.ModifyAddress(Logistics,City,District,Address,Name,tel,phone,OID,username,CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Order/GetExp")]
        public ResponseResult GetExp(string IsQuick,string Logistics,string City,string District)
        {   
            var data = new DataResult(1,null);
            bool isquick;
            if(IsQuick.ToUpper() == "TRUE" || IsQuick.ToUpper() == "FALSE")
            {
                isquick = bool.Parse(IsQuick);
                if(isquick == true)
                {
                    if(string.IsNullOrEmpty(Logistics) || string.IsNullOrEmpty(City) || string.IsNullOrEmpty(District))
                    {
                        data.s = -1;
                        data.d = "参数无效!";
                        return CoreResult.NewResponse(data.s, data.d, "General"); 
                    }
                }
            }
            else
            {
                data.s = -1;
                data.d = "参数无效!";
                return CoreResult.NewResponse(data.s, data.d, "General"); 
            }
            int CoID = int.Parse(GetCoid());
            data = OrderHaddle.GetExp(CoID,isquick,Logistics,City,District);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/SetExp")]
        public ResponseResult SetExp([FromBodyAttribute]JObject co)
        {   
            var OID = new List<int>();
            if(co["OID"] != null)
            {
                OID = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单号必填", "General");
            }   
            string ExpID = "",ExpName="";     
            if(co["ExpID"] != null)
            {
                ExpID = co["ExpID"].ToString();
            }
            else
            {
                return CoreResult.NewResponse(-1, "快递ID必填", "General");
            }        
            if(co["ExpName"] != null)
            {
                ExpName = co["ExpName"].ToString();
            }
            else
            {
                return CoreResult.NewResponse(-1, "快递名必填", "General");
            }        
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.SetExp(OID,CoID,ExpID,ExpName,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Order/GetWarehouse")]
        public ResponseResult GetWarehouse(string ID)
        {   
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.GetWarehouse(CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/SetWarehouse")]
        public ResponseResult SetWarehouse([FromBodyAttribute]JObject co)
        {   
            var OID = new List<int>();
            if(co["OID"] != null)
            {
                OID = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单号必填", "General");
            }   
            string WhID = "";
            if(co["WarehouseID"] != null)
            {
                WhID = co["WarehouseID"].ToString();
            }
            else
            {
                return CoreResult.NewResponse(-1, "仓库ID必填", "General");
            }   
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.SetWarehouse(OID,CoID,WhID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/ConfirmOrder")]
        public ResponseResult ConfirmOrder([FromBodyAttribute]JObject co)
        {   
            var oid = new List<int>();
            if(co["OID"] != null)
            {
                oid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单号必填", "General");
            }   
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.ConfirmOrder(oid,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Order/GetOrderListSingle")]
        public ResponseResult GetOrderListSingle(string OID)
        {   
            int x,oid = 0;
            var data = new DataResult(1,null);
            if (int.TryParse(OID, out x))
            {
                oid = int.Parse(OID);
            }
            else
            {
                data.s = -1;
                data.d = "参数无效!";
                return CoreResult.NewResponse(data.s, data.d, "General"); 
            }
            var oidList = new List<int>();
            oidList.Add(oid);
            int CoID = int.Parse(GetCoid());
            data = OrderHaddle.GetOrderListSingle(oidList,CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/InsertOrderAbnormal")]
        public ResponseResult InsertOrderAbnormal([FromBodyAttribute]JObject co)
        {   
            string OrderAbnormal = "";
            if(co["OrderAbnormal"] != null)
            {
                OrderAbnormal = co["OrderAbnormal"].ToString();
            }
            else
            {
                OrderAbnormal = null;
            }   
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.InsertOrderAbnormal(OrderAbnormal,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/TransferAbnormal")]
        public ResponseResult TransferAbnormal([FromBodyAttribute]JObject co)
        {   
            var oid = new List<int>();
            if(co["OID"] != null)
            {
                oid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单号必填", "General");
            }   
            int AbnormalStatus = 0;
            if(co["AbnormalStatus"] != null)
            {
                AbnormalStatus = int.Parse(co["AbnormalStatus"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "异常原因ID必填", "General");
            }   
            string AbnormalStatusDec = "";
            if(co["AbnormalStatusDec"] != null)
            {
                AbnormalStatusDec = co["AbnormalStatusDec"].ToString();
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.TransferAbnormal(oid,CoID,username,AbnormalStatus,AbnormalStatusDec);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Order/GetCancleList")]
        public ResponseResult GetCancleList()
        {   
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.GetAbnormalList(CoID,6);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/CancleOrder")]
        public ResponseResult CancleOrder([FromBodyAttribute]JObject co)
        {   
            var oid = new List<int>();
            if(co["OID"] != null)
            {
                oid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单号必填", "General");
            }   
            int CancleReason = 0;
            if(co["CancleReason"] != null)
            {
                CancleReason = int.Parse(co["CancleReason"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "取消原因必填", "General");
            }   
            string Remark = "";
            if(co["Remark"] != null)
            {
                Remark = co["Remark"].ToString();
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.CancleOrder(oid,CoID,username,CancleReason,Remark);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/DistributionPay")]
        public ResponseResult DistributionPay([FromBodyAttribute]JObject co)
        {   
            var oid = new List<int>();
            if(co["OID"] != null)
            {
                oid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单号必填", "General");
            }   
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.DistributionPay(oid,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/RestoreCancleOrder")]
        public ResponseResult RestoreCancleOrder([FromBodyAttribute]JObject co)
        {   
            var oid = new List<int>();
            if(co["OID"] != null)
            {
                oid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单号必填", "General");
            }   
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.RestoreCancleOrder(oid,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/ModifySku")]
        public ResponseResult ModifySku([FromBodyAttribute]JObject co)
        {   
            var oid = new List<int>();
            if(co["OID"] != null)
            {
                oid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单号必填", "General");
            }   
            int ModifySku = 0,DeleteSku = 0,AddSku = 0,AddQty = 1;
            decimal ModifyPrice = 0,AddPrice = 0;
            string AddType = string.Empty;
            if(co["ModifySku"] != null)
            {
                ModifySku =int.Parse(co["ModifySku"].ToString());
                if(co["ModifyPrice"] != null)
                {
                    ModifyPrice = decimal.Parse(co["ModifyPrice"].ToString());
                }
                else
                {
                    return CoreResult.NewResponse(-1, "修改商品单价必填", "General");
                }   
            }
            if(co["DeleteSku"] != null)
            {
                DeleteSku =int.Parse(co["DeleteSku"].ToString());
            }
            if(co["AddSku"] != null)
            {
                AddSku =int.Parse(co["AddSku"].ToString());
                if(co["AddPrice"] != null)
                {
                    AddPrice = decimal.Parse(co["AddPrice"].ToString());
                }
                else
                {
                    return CoreResult.NewResponse(-1, "新增商品单价必填", "General");
                }   
                if(co["AddQty"] != null)
                {
                    AddQty = int.Parse(co["AddQty"].ToString());
                }
            }
            if(co["AddType"] != null)
            {
                AddType = co["AddType"].ToString();
            }
            else
            {
                return CoreResult.NewResponse(-1, "添加商品规则必填", "General");
            }   
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.ModifySku(oid,ModifySku,ModifyPrice,DeleteSku,AddSku,AddPrice,AddQty,AddType,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/CancleConfirmOrder")]
        public ResponseResult CancleConfirmOrder([FromBodyAttribute]JObject co)
        {   
            var oid = new List<int>();
            if(co["OID"] != null)
            {
                oid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单号必填", "General");
            }   
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.CancleConfirmOrder(oid,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/DirectShip")]
        public ResponseResult DirectShip([FromBodyAttribute]JObject co)
        {   
            int oid = 0;
            if(co["OID"] != null)
            {
                oid = int.Parse(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单号必填", "General");
            }   
            string ExCode = "";
            if(co["ExCode"] != null)
            {
                ExCode = co["ExCode"].ToString();
            }
            else
            {
                return CoreResult.NewResponse(-1, "快递单号必填", "General");
            }   
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.DirectShip(oid,ExCode,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/CancleShip")]
        public ResponseResult CancleShip([FromBodyAttribute]JObject co)
        {   
            int oid = 0;
            if(co["OID"] != null)
            {
                oid = int.Parse(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单号必填", "General");
            }   
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.CancleShip(oid,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/InsertGiftMulti")]
        public ResponseResult InsertGiftMulti([FromBodyAttribute]JObject co)
        {   
            var id = new List<int>();
            var skuid = new List<int>();
            if(co["OID"] != null)
            {
                id = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单号必填", "General");
            }
            if(co["SkuIDList"] != null)
            {
                skuid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["SkuIDList"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "商品ID必填", "General");
            } 
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.InsertGiftMulti(id,skuid,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/MarkCustomAbnormal")]
        public ResponseResult MarkCustomAbnormal([FromBodyAttribute]JObject co)
        {   
            var Status = new List<int>();
            if(co["Status"] != null)
            {
                Status = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["Status"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "状态必填", "General");
            }
            DateTime OrdDateStart,OrdDateEnd;
            if(co["OrdDateStart"] != null)
            {
                string text = co["OrdDateStart"].ToString();
                DateTime x;
                if (DateTime.TryParse(text, out x))
                {
                    OrdDateStart = DateTime.Parse(text);
                    OrdDateStart = OrdDateStart.AddDays(-1);
                }
                else
                {
                    return CoreResult.NewResponse(-1, "订单开始日期参数异常", "General");
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单开始日期必填", "General");
            } 
            if(co["OrdDateEnd"] != null)
            {
                string text = co["OrdDateEnd"].ToString();
                DateTime x;
                if (DateTime.TryParse(text, out x))
                {
                    OrdDateEnd = DateTime.Parse(text);
                    OrdDateEnd = OrdDateEnd.AddDays(1);
                }
                else
                {
                    return CoreResult.NewResponse(-1, "订单结束日期参数异常", "General");
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单结束日期必填", "General");
            } 
            string GoodsCode="",SkuID="",SkuName="",Norm="",RecMessage="",SendMessage="",Abnormal="";
            if(co["GoodsCode"] != null)
            {
                GoodsCode = co["GoodsCode"].ToString();
            }
            if(co["SkuID"] != null)
            {
                SkuID = co["SkuID"].ToString();
            }
            if(co["SkuName"] != null)
            {
                SkuName = co["SkuName"].ToString();
            }
            if(co["Norm"] != null)
            {
                Norm = co["Norm"].ToString();
            }
            if(co["RecMessage"] != null)
            {
                RecMessage = co["RecMessage"].ToString();
            }
            if(co["SendMessage"] != null)
            {
                SendMessage = co["SendMessage"].ToString();
            }
            if(co["Abnormal"] != null)
            {
                Abnormal = co["Abnormal"].ToString();
            }
            else
            {
                return CoreResult.NewResponse(-1, "标记异常必填", "General");
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.MarkCustomAbnormal(Status,OrdDateStart,OrdDateEnd,GoodsCode,SkuID,SkuName,Norm,RecMessage,SendMessage,Abnormal,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/ComDisExchange")]
        public ResponseResult ComDisExchange([FromBodyAttribute]JObject co)
        {   
            var oid = new List<int>();
            if(co["OID"] != null)
            {
                oid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单单号必填", "General");
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.ComDisExchange(oid,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/SetOrdType")]
        public ResponseResult SetOrdType([FromBodyAttribute]JObject co)
        {   
            var oid = new List<int>();
            if(co["OID"] != null)
            {
                oid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单单号必填", "General");
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.SetOrdType(oid,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/CancleSetOrdType")]
        public ResponseResult CancleSetOrdType([FromBodyAttribute]JObject co)
        {   
            var oid = new List<int>();
            if(co["OID"] != null)
            {
                oid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单单号必填", "General");
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.CancleSetOrdType(oid,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Order/GetSupDistributor")]
        public ResponseResult GetSupDistributor()
        {   
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.GetSupDistributor(CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/SetSupDistributor")]
        public ResponseResult SetSupDistributor([FromBodyAttribute]JObject co)
        {   
            var oid = new List<int>();
            if(co["OID"] != null)
            {
                oid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["OID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单单号必填", "General");
            }
            int sd = 0;
            if(co["SupDistributor"] != null)
            {
                sd = int.Parse(co["SupDistributor"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "供销商ID必填", "General");
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.SetSupDistributor(oid,sd,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/CalGift")]
        public ResponseResult CalGift([FromBodyAttribute]JObject co)
        {   
            string OidType = "",DateType = "";
            bool IsSplit,IsDelGift,IsDelPrice0;
            var oid = new List<int>();
            if(co["OidType"] != null)
            {
                OidType = co["OidType"].ToString();
                if(OidType != "A" && OidType != "B")
                {
                    return CoreResult.NewResponse(-1, "订单范围参数异常", "General");
                }
                if(OidType == "A")
                {
                    if(co["OID"] != null)
                    {
                        oid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["OID"].ToString());
                    }
                    else
                    {
                        return CoreResult.NewResponse(-1, "订单单号必填", "General");
                    }
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单范围必填", "General");
            }
            if(co["DateType"] != null)
            {
                DateType = co["DateType"].ToString();
                if(DateType != "A" && DateType != "B")
                {
                    return CoreResult.NewResponse(-1, "日期排序方式参数异常", "General");
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "日期排序方式必填", "General");
            }
            if(co["IsSplit"] != null)
            {
                if(co["IsSplit"].ToString().ToUpper() !="TRUE" && co["IsSplit"].ToString().ToUpper() !="FALSE")
                {
                    return CoreResult.NewResponse(-1, "是否排除拆分订单参数异常", "General");
                }
                else
                {
                    IsSplit = bool.Parse(co["IsSplit"].ToString());
                }                
            }
            else
            {
                return CoreResult.NewResponse(-1, "是否排除拆分订单必填", "General");
            }
            if(co["IsDelGift"] != null)
            {
                if(co["IsDelGift"].ToString().ToUpper() !="TRUE" && co["IsDelGift"].ToString().ToUpper() !="FALSE")
                {
                    return CoreResult.NewResponse(-1, "是否删除原有赠品参数异常", "General");
                }
                else
                {
                    IsDelGift = bool.Parse(co["IsDelGift"].ToString());
                }                
            }
            else
            {
                return CoreResult.NewResponse(-1, "是否删除原有赠品必填", "General");
            }
            if(co["IsDelPrice"] != null)
            {
                if(co["IsDelPrice"].ToString().ToUpper() !="TRUE" && co["IsDelPrice"].ToString().ToUpper() !="FALSE")
                {
                    return CoreResult.NewResponse(-1, "是否删除零单价明细参数异常", "General");
                }
                else
                {
                    IsDelPrice0 = bool.Parse(co["IsDelPrice"].ToString());
                }                
            }
            else
            {
                return CoreResult.NewResponse(-1, "是否删除零单价明细必填", "General");
            }
            var ord = new List<OrderQuery>();
            if(OidType == "B")
            {
                var cp = new OrderParm();
                cp.CoID = int.Parse(GetCoid());
                if(co["ID"] != null)
                {
                    cp.ID = int.Parse(co["ID"].ToString());
                }
                if(co["SoID"] != null)
                {
                    cp.SoID = int.Parse(co["SoID"].ToString());
                }
                if(co["PayNbr"] != null)
                {
                    cp.PayNbr = co["PayNbr"].ToString();
                }
                if(co["BuyerShopID"] != null)
                {
                    cp.BuyerShopID = co["BuyerShopID"].ToString();
                }
                if(co["ExCode"] != null)
                {
                    cp.ExCode = co["ExCode"].ToString();
                }
                if(co["RecName"] != null)
                {
                    cp.RecName = co["RecName"].ToString();
                }
                if(co["RecPhone"] != null)
                {
                    cp.RecPhone = co["RecPhone"].ToString();
                }
                if(co["RecTel"] != null)
                {
                    cp.RecTel = co["RecTel"].ToString();
                }
                if(co["RecLogistics"] != null)
                {
                    cp.RecLogistics = co["RecLogistics"].ToString();
                }
                if(co["RecCity"] != null)
                {
                    cp.RecCity = co["RecCity"].ToString();
                }
                if(co["RecDistrict"] != null)
                {
                    cp.RecDistrict = co["RecDistrict"].ToString();
                }
                if(co["RecAddress"] != null)
                {
                    cp.RecAddress = co["RecAddress"].ToString();
                }                
                if(co["StatusList"] != null)
                {
                    string[] a = co["StatusList"].ToString().Split(',');
                    List<int> s = new List<int>();
                    foreach(var i in a)
                    {
                        s.Add(int.Parse(i));
                    }
                    cp.StatusList = s;
                }
                if(co["AbnormalStatusList"] != null)
                {
                    string[] a = co["AbnormalStatusList"].ToString().Split(',');
                    List<int> s = new List<int>();
                    foreach(var i in a)
                    {
                        s.Add(int.Parse(i));
                    }
                    cp.AbnormalStatusList = s;
                }
                if(co["IsRecMsgYN"] != null)
                {
                    cp.IsRecMsgYN = co["IsRecMsgYN"].ToString();
                }
                if(co["RecMessage"] != null)
                {
                    cp.RecMessage = co["RecMessage"].ToString();
                }
                if(co["IsSendMsgYN"] != null)
                {
                    cp.IsSendMsgYN = co["IsSendMsgYN"].ToString();
                }
                if(co["SendMessage"] != null)
                {
                    cp.SendMessage = co["SendMessage"].ToString();
                }
                if(co["Datetype"] != null)
                {
                    cp.Datetype = co["Datetype"].ToString();
                }
                if(co["DateStart"] != null)
                {
                    cp.DateStart = DateTime.Parse(co["DateStart"].ToString());
                }
                if(co["DateEnd"] != null)
                {
                    cp.DateEnd = DateTime.Parse(co["DateEnd"].ToString());
                }
                if(co["Skuid"] != null)
                {
                    cp.Skuid = co["Skuid"].ToString();
                }
                if(co["GoodsCode"] != null)
                {
                    cp.GoodsCode = co["GoodsCode"].ToString();
                }
                if(co["Ordqtystart"] != null)
                {
                    cp.Ordqtystart = int.Parse(co["Ordqtystart"].ToString());
                }
                if(co["Ordqtyend"] != null)
                {
                    cp.Ordqtyend = int.Parse(co["Ordqtyend"].ToString());
                }
                if(co["Ordamtstart"] != null)
                {
                    cp.Ordamtstart = decimal.Parse(co["Ordamtstart"].ToString());
                }
                if(co["Ordamtend"] != null)
                {
                    cp.Ordamtend = decimal.Parse(co["Ordamtend"].ToString());
                }
                if(co["Skuname"] != null)
                {
                    cp.Skuname = co["Skuname"].ToString();
                }
                if(co["Norm"] != null)
                {
                    cp.Norm = co["Norm"].ToString();
                }
                if(co["ShopStatus"] != null)
                {
                    string[] a = co["ShopStatus"].ToString().Split(',');
                    List<string> s = new List<string>();
                    foreach(var i in a)
                    {
                        s.Add(i);
                    }
                    cp.ShopStatus = s;
                }
                if(co["Osource"] != null)
                {
                    cp.OSource = int.Parse(co["Osource"].ToString());
                }
                if(co["Type"] != null)
                {
                    string[] a = co["Type"].ToString().Split(',');
                    List<int> s = new List<int>();
                    foreach(var i in a)
                    {
                        s.Add(int.Parse(i));
                    }
                    cp.Type = s;
                }
                if(co["IsCOD"] != null)
                {
                    cp.IsCOD = co["IsCOD"].ToString();
                }
                if(co["IsPaid"] != null)
                {
                    cp.IsPaid = co["IsPaid"].ToString();
                }
                if(co["IsShopSelectAll"] != null)
                {
                    cp.IsShopSelectAll = bool.Parse(co["IsShopSelectAll"].ToString());
                }
                if(co["ShopID"] != null)
                {
                    string[] a = co["ShopID"].ToString().Split(',');
                    List<int> s = new List<int>();
                    foreach(var i in a)
                    {
                        s.Add(int.Parse(i));
                    }
                    cp.ShopID = s;
                }
                if(co["IsDisSelectAll"] != null)
                {
                    cp.IsDisSelectAll = bool.Parse(co["IsDisSelectAll"].ToString());
                }
                if(co["Distributor"] != null)
                {
                    string[] a = co["Distributor"].ToString().Split(',');
                    List<string> s = new List<string>();
                    foreach(var i in a)
                    {
                        s.Add(i);
                    }
                    cp.Distributor = s;
                }
                if(co["ExID"] != null)
                {
                    string[] a = co["ExID"].ToString().Split(',');
                    List<int> s = new List<int>();
                    foreach(var i in a)
                    {
                        s.Add(int.Parse(i));
                    }
                    cp.ExID = s;
                }
                if(co["SendWarehouse"] != null)
                {
                    string[] a = co["SendWarehouse"].ToString().Split(',');
                    List<int> s = new List<int>();
                    foreach(var i in a)
                    {
                        s.Add(int.Parse(i));
                    }
                    cp.SendWarehouse = s;
                }
                if(co["Others"] != null)
                {
                    string[] a = co["Others"].ToString().Split(',');
                    List<int> s = new List<int>();
                    foreach(var i in a)
                    {
                        s.Add(int.Parse(i));
                    }
                    cp.Others = s;
                }
                cp.NumPerPage = 100000000;
                cp.PageIndex = 1;
                var res = OrderHaddle.GetOrderList(cp);
                if(res.s == 1)
                {
                    var rr = res.d as OrderData;
                    ord = rr.Ord;
                }
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.CalGift(OidType,oid,DateType,IsSplit,IsDelGift,IsDelPrice0,ord,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
    }
}  