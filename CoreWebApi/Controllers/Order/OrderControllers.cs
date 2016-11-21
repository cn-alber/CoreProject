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
            ord.BuyerShopID = co["BuyerShopID"].ToString();
            ord.RecName = co["RecName"].ToString();
            ord.RecLogistics = co["RecLogistics"].ToString();
            ord.RecCity = co["RecCity"].ToString();
            ord.RecDistrict = co["RecDistrict"].ToString();
            ord.RecAddress = co["RecAddress"].ToString();
            ord.RecPhone = co["RecPhone"].ToString();
            ord.RecTel = co["RecTel"].ToString();
            ord.RecMessage = co["RecMessage"].ToString();
            ord.SendMessage = co["SendMessage"].ToString();
            string text = co["ODate"].ToString();
            DateTime x;
            if (DateTime.TryParse(text, out x))
            {
                ord.ODate = DateTime.Parse(text);
            }
            text = co["SoID"].ToString();
            long y;
            if (long.TryParse(text, out y))
            {
                ord.SoID = long.Parse(text);
            }
            text = co["ExAmount"].ToString();
            decimal z;
            if (decimal.TryParse(text, out z))
            {
                ord.ExAmount = text;
            }
            text = co["ShopID"].ToString();
            int i;
            if (int.TryParse(text, out i))
            {
                ord.ShopID = int.Parse(text);
            }       
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
            decimal ExAmount = decimal.Parse(co["ExAmount"].ToString());
            string Remark = co["SendMessage"].ToString();
            string InvTitle = co["InvoiceTitle"].ToString();
            string Logistics = co["RecLogistics"].ToString();
            string City = co["RecCity"].ToString();
            string District = co["RecDistrict"].ToString();
            string Address = co["RecAddress"].ToString();
            string Name = co["RecName"].ToString();
            string tel = co["RecTel"].ToString();
            string phone = co["RecPhone"].ToString();
            int OID = int.Parse(co["OID"].ToString());
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
            // long soid = long.Parse(co["SoID"].ToString());
            List<int> skuid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["SkuIDList"].ToString());
            var data = new DataResult(1,null);
            bool isQuick;
            string isquick = co["isQuick"].ToString();
            if(isquick.ToUpper() != "TRUE" && isquick.ToUpper() != "FALSE")
            {
                data.s = -1;
                data.d = "参数无效!";
                return CoreResult.NewResponse(data.s, data.d, "General"); 
            }
            else
            {
                isQuick = bool.Parse(isquick);
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            data = OrderHaddle.InsertOrderDetail(id,skuid,CoID,username,isQuick);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/DeleteOrderDetail")]
        public ResponseResult DeleteOrderDetail([FromBodyAttribute]JObject co)
        {   
            int id = int.Parse(co["ID"].ToString());
            int oid = int.Parse(co["OID"].ToString());
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = new DataResult(1,null);
            bool isQuick;
            string isquick = co["isQuick"].ToString();
            if(isquick.ToUpper() != "TRUE" && isquick.ToUpper() != "FALSE")
            {
                data.s = -1;
                data.d = "参数无效!";
                return CoreResult.NewResponse(data.s, data.d, "General"); 
            }
            else
            {
                isQuick = bool.Parse(isquick);
            }
            data = OrderHaddle.DeleteOrderDetail(id,oid,CoID,username,isQuick);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/UpdateOrderDetail")]
        public ResponseResult UpdateOrderDetail([FromBodyAttribute]JObject co)
        {   
            int id = int.Parse(co["ID"].ToString());
            int oid = int.Parse(co["OID"].ToString());
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
            string SkuName = co["SkuName"].ToString();
            var data = new DataResult(1,null);
            bool isQuick;
            string isquick = co["isQuick"].ToString();
            if(isquick.ToUpper() != "TRUE" && isquick.ToUpper() != "FALSE")
            {
                data.s = -1;
                data.d = "参数无效!";
                return CoreResult.NewResponse(data.s, data.d, "General"); 
            }
            else
            {
                isQuick = bool.Parse(isquick);
            }
            data = OrderHaddle.UpdateOrderDetail(id,oid,CoID,username,price,qty,SkuName,isQuick);
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
            int payid = int.Parse(co["PayID"].ToString());
            string username = GetUname();
            int CoID =  int.Parse(GetCoid());
            var data = OrderHaddle.CancleConfirmPay(OID,payid,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/ConfirmPay")]
        public ResponseResult ConfirmPay([FromBodyAttribute]JObject co)
        {   
            int OID = int.Parse(co["OID"].ToString());
            int payid = int.Parse(co["PayID"].ToString());
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.ConfirmPay(OID,payid,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/CanclePay")]
        public ResponseResult CanclePay([FromBodyAttribute]JObject co)
        {   
            int OID = int.Parse(co["OID"].ToString());
            int payid = int.Parse(co["PayID"].ToString());
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.CanclePay(OID,payid,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/QuickPay")]
        public ResponseResult QuickPay([FromBodyAttribute]JObject co)
        {   
            int OID = int.Parse(co["OID"].ToString());
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.QuickPay(OID,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Order/GetAbnormalList")]
        public ResponseResult GetAbnormalList()
        {   
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.GetAbnormalList(CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/TransferNormal")]
        public ResponseResult TransferNormal([FromBodyAttribute]JObject co)
        {   
            List<int> oid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["OID"].ToString());
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
            int oid = int.Parse(co["OID"].ToString());
            List<int> merid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["MerID"].ToString());
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.OrdMerger(oid,merid,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/CancleOrdMerge")]
        public ResponseResult CancleOrdMerge([FromBodyAttribute]JObject co)
        {   
            List<int> oid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["OID"].ToString());
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.CancleOrdMerge(oid,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/OrdSplit")]
        public ResponseResult OrdSplit([FromBodyAttribute]JObject co)
        {   
            int oid = int.Parse(co["OID"].ToString());
            List<SplitOrd> splitOrd = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SplitOrd>>(co["SplitOrd"].ToString());
            string username = GetUname();
            int CoID = int.Parse(GetCoid()); 
            var data = OrderHaddle.OrdSplit(oid,splitOrd,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/ModifyFreight")]
        public ResponseResult ModifyFreight([FromBodyAttribute]JObject co)
        {   
            decimal freight = decimal.Parse(co["Freight"].ToString());
            List<int> oid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["OID"].ToString());
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
            ImportOrderInsert order = Newtonsoft.Json.JsonConvert.DeserializeObject<ImportOrderInsert>(co["Order"].ToString());
            string username = GetUname();
            int CoID = int.Parse(GetCoid()); 
            var data = OrderHaddle.ImportOrderInsert(order,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/ImportOrderUpdate")]
        public ResponseResult ImportOrderUpdate([FromBodyAttribute]JObject co)
        {   
            ImportOrderUpdate order = Newtonsoft.Json.JsonConvert.DeserializeObject<ImportOrderUpdate>(co["Order"].ToString());
            string username = GetUname();
            int CoID = int.Parse(GetCoid()); 
            var data = OrderHaddle.ImportOrderUpdate(order,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/InsertGift")]
        public ResponseResult InsertGift([FromBodyAttribute]JObject co)
        {   
            int id = int.Parse(co["OID"].ToString());
            List<int> skuid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["SkuIDList"].ToString());
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.InsertGift(id,skuid,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/ChangeOrderDetail")]
        public ResponseResult ChangeOrderDetail([FromBodyAttribute]JObject co)
        {   
            int id = int.Parse(co["ID"].ToString());
            int oid = int.Parse(co["OID"].ToString());
            int Skuid = int.Parse(co["SkuID"].ToString());
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
            int oid = int.Parse(co["OID"].ToString());
            string Remark = co["SendMessage"].ToString();
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.ModifyRemark(oid,CoID,username,Remark);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/ModifyAddress")]
        public ResponseResult ModifyAddress([FromBodyAttribute]JObject co)
        {   
            string Logistics = co["RecLogistics"].ToString();
            string City = co["RecCity"].ToString();
            string District = co["RecDistrict"].ToString();
            string Address = co["RecAddress"].ToString();
            string Name = co["RecName"].ToString();
            string tel = co["RecTel"].ToString();
            string phone = co["RecPhone"].ToString();
            int OID = int.Parse(co["OID"].ToString());
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
            List<int> OID = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["OID"].ToString());
            string ExpID = co["ExpID"].ToString();
            string ExpName = co["ExpName"].ToString();
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
            List<int> OID = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["OID"].ToString());
            string WhID = co["WarehouseID"].ToString();
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.SetWarehouse(OID,CoID,WhID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Order/ConfirmOrder")]
        public ResponseResult ConfirmOrder([FromBodyAttribute]JObject co)
        {   
            List<int> oid = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["OID"].ToString());
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = OrderHaddle.ConfirmOrder(oid,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }


        [HttpPostAttribute("/Core/Order/ModifySku")]
        public ResponseResult ModifySku([FromBodyAttribute]JObject co)
        {   
            List<int> OID = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["OID"].ToString());
            var data = new DataResult(1,null);
            int ModifySku = 0,DeleteSku = 0,AddSku = 0,AddQty = 1,x;
            decimal ModifyPrice = 0,AddPrice = 0,y;
            string AddType = string.Empty;
            string Text = co["ModifySku"].ToString();
            if(!string.IsNullOrEmpty(Text))
            {
                if (int.TryParse(Text, out x))
                {
                    ModifySku = int.Parse(Text);
                }
                else
                {
                    data.s = -1;
                    data.d = "修改的商品ID参数无效!";
                    return CoreResult.NewResponse(data.s, data.d, "General"); 
                }
            }   
            if(ModifySku > 0)
            {
                Text = co["ModifyPrice"].ToString();
                if(string.IsNullOrEmpty(Text))
                {
                    data.s = -1;
                    data.d = "修改的商品单价必须有值!";
                    return CoreResult.NewResponse(data.s, data.d, "General"); 
                }
                else
                {
                    if (decimal.TryParse(Text, out y))
                    {
                        ModifyPrice = decimal.Parse(Text);
                    }
                    else
                    {
                        data.s = -1;
                        data.d = "修改的商品单价参数无效!";
                        return CoreResult.NewResponse(data.s, data.d, "General"); 
                    }
                }   
            }
            Text = co["DeleteSku"].ToString();
            if(!string.IsNullOrEmpty(Text))
            {
                if (int.TryParse(Text, out x))
                {
                    DeleteSku = int.Parse(Text);
                }
                else
                {
                    data.s = -1;
                    data.d = "删除的商品ID参数无效!";
                    return CoreResult.NewResponse(data.s, data.d, "General"); 
                }
            }   
            Text = co["AddSku"].ToString();
            if(!string.IsNullOrEmpty(Text))
            {
                if (int.TryParse(Text, out x))
                {
                    AddSku = int.Parse(Text);
                }
                else
                {
                    data.s = -1;
                    data.d = "新增的商品ID参数无效!";
                    return CoreResult.NewResponse(data.s, data.d, "General"); 
                }
            }   
            if(AddSku > 0)
            {
                Text = co["AddQty"].ToString();
                if(!string.IsNullOrEmpty(Text))
                {
                    if (int.TryParse(Text, out x))
                    {
                        AddQty = int.Parse(Text);
                    }
                    else
                    {
                        data.s = -1;
                        data.d = "新增的商品数量参数无效!";
                        return CoreResult.NewResponse(data.s, data.d, "General"); 
                    }
                }   
                Text = co["AddPrice"].ToString();
                if(string.IsNullOrEmpty(Text))
                {
                    data.s = -1;
                    data.d = "新增的商品单价必须有值!";
                    return CoreResult.NewResponse(data.s, data.d, "General"); 
                }
                else
                {
                    if (decimal.TryParse(Text, out y))
                    {
                        AddPrice = decimal.Parse(Text);
                    }
                    else
                    {
                        data.s = -1;
                        data.d = "新增的商品单价参数无效!";
                        return CoreResult.NewResponse(data.s, data.d, "General"); 
                    }
                }   
            }
            Text = co["AddType"].ToString();
            if(Text.ToUpper() == "A" || Text.ToUpper() == "B")
            {
                AddType = Text;
            }
            else
            {
                data.s = -1;
                data.d = "添加商品规则参数无效!";
                return CoreResult.NewResponse(data.s, data.d, "General"); 
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            data = OrderHaddle.ModifySku(OID,ModifySku,ModifyPrice,DeleteSku,AddSku,AddPrice,AddQty,AddType,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
    }
}  