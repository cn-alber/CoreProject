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
    public class GiftController : ControllBase
    {
        [HttpGetAttribute("/Core/Gift/GetShopInitData")]
        public ResponseResult GetShopInitData()
        {   
            var data = GiftHaddle.GetShopInitData(int.Parse(GetCoid()));
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Gift/InsertGiftRule")]
        public ResponseResult InsertGiftRule([FromBodyAttribute]JObject co)
        {   
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var gift = new GiftRule();
            if(co["GiftName"] != null)
            {
                string text = co["GiftName"].ToString();
                if (!string.IsNullOrEmpty(text))
                {
                    gift.GiftName = co["GiftName"].ToString();
                }
                else
                {
                    return CoreResult.NewResponse(-1, "规则名称必填", "General"); 
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "规则名称必填", "General"); 
            }
            if(co["Priority"] != null)
            {
                string text = co["Priority"].ToString();
                int x;
                if (int.TryParse(text, out x))
                {
                    gift.Priority = int.Parse(text);
                }
                else
                {
                    return CoreResult.NewResponse(-1, "优先级参数异常", "General"); 
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "优先级必填", "General"); 
            }
            if(co["DateFrom"] != null)
            {
                string text = co["DateFrom"].ToString();
                DateTime x;
                if (DateTime.TryParse(text, out x))
                {
                    gift.DateFrom = DateTime.Parse(text);
                }
                else
                {
                    return CoreResult.NewResponse(-1, "开始日期参数异常", "General"); 
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "开始日期必填", "General"); 
            }
            if(co["DateTo"] != null)
            {
                string text = co["DateTo"].ToString();
                DateTime x;
                if (DateTime.TryParse(text, out x))
                {
                    gift.DateTo = DateTime.Parse(text);
                }
                else
                {
                    return CoreResult.NewResponse(-1, "结束日期参数异常", "General"); 
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "结束日期必填", "General"); 
            }
            if(co["AppointSkuID"] != null)
            {
                gift.AppointSkuID = co["AppointSkuID"].ToString();
            }
            if(co["AppointGoodsCode"] != null)
            {
                gift.AppointGoodsCode = co["AppointGoodsCode"].ToString();
            }
            if(co["ExcludeSkuID"] != null)
            {
                gift.ExcludeSkuID = co["ExcludeSkuID"].ToString();
            }
            if(co["ExcludeGoodsCode"] != null)
            {
                gift.ExcludeGoodsCode = co["ExcludeGoodsCode"].ToString();
            }
            if(co["AmtMin"] != null)
            {
                string text = co["AmtMin"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    gift.AmtMin = text;
                }
            }
            if(co["AmtMax"] != null)
            {
                string text = co["AmtMax"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    gift.AmtMax = text;
                }
            }
            if(co["QtyMin"] != null)
            {
                string text = co["QtyMin"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    gift.QtyMin = text;
                }
            }
            if(co["QtyMax"] != null)
            {
                string text = co["QtyMax"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    gift.QtyMax = text;
                }
            }
            if(co["IsSkuIDValid"] != null)
            {
                gift.IsSkuIDValid = bool.Parse(co["IsSkuIDValid"].ToString());
            }
            if(co["DiscountRate"] != null)
            {
                string text = co["DiscountRate"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    gift.DiscountRate = text;
                }
            }
            if(co["AppointShop"] != null)
            {
                gift.AppointShop = co["AppointShop"].ToString();
            }
            if(co["OrdType"] != null)
            {
                gift.OrdType = co["OrdType"].ToString();
            }
            if(co["IsStock"] != null)
            {
                gift.IsStock = bool.Parse(co["IsStock"].ToString());
            }
            if(co["IsAdd"] != null)
            {
                gift.IsAdd = bool.Parse(co["IsAdd"].ToString());
            }
            if(co["QtyEach"] != null)
            {
                string text = co["QtyEach"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    gift.QtyEach = text;
                }
            }
            if(co["AmtEach"] != null)
            {
                string text = co["AmtEach"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    gift.AmtEach = text;
                }
            }
            if(co["IsMarkGift"] != null)
            {
                gift.IsMarkGift = bool.Parse(co["IsMarkGift"].ToString());
            }
            if(co["MaxGiftQty"] != null)
            {
                string text = co["MaxGiftQty"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    gift.MaxGiftQty = text;
                }
            }
            gift.Enable = true;
            gift.CoID = CoID;
            gift.Creator = username;
            gift.Modifier = username;
            if(co["GiftNo"] != null)
            {
                string text = co["GiftNo"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    gift.GiftNo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(co["GiftNo"].ToString());
                }
                else
                {
                    return CoreResult.NewResponse(-1, "赠品必填", "General"); 
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "赠品必填", "General"); 
            }
            var data = GiftHaddle.InsertGiftRule(gift,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Gift/GetGiftLog")]
        public ResponseResult GetGiftLog(string ID)
        {   
            int id = 0;
            if(!string.IsNullOrEmpty(ID))
            {
                int x;
                if (int.TryParse(ID, out x))
                {
                    id= int.Parse(ID);
                }
                else
                {
                    return CoreResult.NewResponse(-1, "赠品规则ID参数异常", "General"); 
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "赠品规则ID参数异常", "General"); 
            }
            var data = GiftHaddle.GetGiftLog(id,int.Parse(GetCoid()));
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Gift/DisableRule")]
        public ResponseResult DisableRule([FromBodyAttribute]JObject co)
        {   
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            int id = 0;
            if(co["ID"] != null)
            {
                string text = co["ID"].ToString();
                int x;
                if (int.TryParse(text, out x))
                {
                    id = int.Parse(text);
                }
                else
                {
                    return CoreResult.NewResponse(-1, "规则ID参数异常", "General"); 
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "规则ID必输", "General"); 
            }
            var data = GiftHaddle.DisableRule(id,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Gift/EnableRule")]
        public ResponseResult EnableRule([FromBodyAttribute]JObject co)
        {   
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            int id = 0;
            if(co["ID"] != null)
            {
                string text = co["ID"].ToString();
                int x;
                if (int.TryParse(text, out x))
                {
                    id = int.Parse(text);
                }
                else
                {
                    return CoreResult.NewResponse(-1, "规则ID参数异常", "General"); 
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "规则ID必输", "General"); 
            }
            var data = GiftHaddle.EnableRule(id,CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Gift/GetRuleEdit")]
        public ResponseResult GetRuleEdit(string ID)
        {   
            int id = 0;
            if(!string.IsNullOrEmpty(ID))
            {
                int x;
                if (int.TryParse(ID, out x))
                {
                    id= int.Parse(ID);
                }
                else
                {
                    return CoreResult.NewResponse(-1, "赠品规则ID参数异常", "General"); 
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "赠品规则ID参数异常", "General"); 
            }
            var data = GiftHaddle.GetRuleEdit(id,int.Parse(GetCoid()));
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Gift/UpdateGiftRule")]
        public ResponseResult UpdateGiftRule([FromBodyAttribute]JObject co)
        {   
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var gift = new GiftRuleEdit();
            if(co["ID"] != null)
            {
                string text = co["ID"].ToString();
                int x;
                if (int.TryParse(text, out x))
                {
                    gift.ID = int.Parse(text);
                }
                else
                {
                    return CoreResult.NewResponse(-1, "ID参数异常", "General"); 
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "ID必填", "General"); 
            }
            if(co["GiftName"] != null)
            {
                string text = co["GiftName"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    gift.GiftName = text;
                }
            }
            if(co["Priority"] != null)
            {
                string text = co["Priority"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    gift.Priority = int.Parse(text);
                }
            }
            if(co["DateFrom"] != null)
            {
                string text = co["DateFrom"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    gift.DateFrom = DateTime.Parse(text);
                }
            }
            if(co["DateTo"] != null)
            {
                string text = co["DateTo"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    gift.DateTo = DateTime.Parse(text);
                }
            }
            if(co["AppointSkuID"] != null)
            {
                string text = co["AppointSkuID"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    gift.AppointSkuID = text;
                }
            }
            if(co["AppointGoodsCode"] != null)
            {
                string text = co["AppointGoodsCode"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    gift.AppointGoodsCode = text;
                }
            }
            if(co["ExcludeSkuID"] != null)
            {
                string text = co["ExcludeSkuID"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    gift.ExcludeSkuID = text;
                }
            }
            if(co["ExcludeGoodsCode"] != null)
            {
                string text = co["ExcludeGoodsCode"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    gift.ExcludeGoodsCode = text;
                }
            }
            if(co["AmtMin"] != null)
            {
                string text = co["AmtMin"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    gift.AmtMin = text;
                }
            }
            if(co["AmtMax"] != null)
            {
                string text = co["AmtMax"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    gift.AmtMax = text;
                }
            }
            if(co["QtyMin"] != null)
            {
                string text = co["QtyMin"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    gift.QtyMin = text;
                }
            }
            if(co["QtyMax"] != null)
            {
                string text = co["QtyMax"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    gift.QtyMax = text;
                }
            }
            string IsSkuIDValid = null;
            if(co["IsSkuIDValid"] != null)
            {
                string text = co["IsSkuIDValid"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    IsSkuIDValid = text;
                    gift.IsSkuIDValid = bool.Parse(text);
                }
            }
            if(co["DiscountRate"] != null)
            {
                string text = co["DiscountRate"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    gift.DiscountRate = text;
                }
            }
            if(co["AppointShop"] != null)
            {
                string text = co["AppointShop"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    gift.AppointShop = text;
                }
            }
            if(co["OrdType"] != null)
            {
                string text = co["OrdType"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    gift.OrdType = text;
                }
            }
            string IsStock = null,IsAdd = null;
            if(co["IsStock"] != null)
            {
                string text = co["IsStock"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    IsStock = text;
                    gift.IsStock = bool.Parse(text);
                }
            }
            if(co["IsAdd"] != null)
            {
                string text = co["IsAdd"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    IsAdd = text;
                    gift.IsAdd = bool.Parse(text);
                }
            }
            if(co["QtyEach"] != null)
            {
                string text = co["QtyEach"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    gift.QtyEach = text;
                }
            }
            if(co["AmtEach"] != null)
            {
                string text = co["AmtEach"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    gift.AmtEach = text;
                }
            }
            string IsMarkGift = null;
            if(co["IsMarkGift"] != null)
            {
                string text = co["IsMarkGift"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    IsMarkGift = text;
                    gift.IsMarkGift = bool.Parse(text);
                }
            }
            if(co["MaxGiftQty"] != null)
            {
                string text = co["MaxGiftQty"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    gift.MaxGiftQty = text;
                }
            }
            if(co["GiftNo"] != null)
            {
                string text = co["GiftNo"].ToString();
                if(!string.IsNullOrEmpty(text))
                {
                    gift.GiftNo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(co["GiftNo"].ToString());
                }
            }
            var data = GiftHaddle.UpdateGiftRule(gift,CoID,username,IsSkuIDValid,IsStock,IsAdd,IsMarkGift);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Gift/GetGiftRuleList")]
        public ResponseResult GetGiftRuleList(string ID,string GiftNo,string GiftName,string DateFrom,string DateTo,string AppointSkuID,string ExcludeSkuID,string AmtMinStart,
                                              string AmtMinEnd,string AmtMaxStart,string AmtMaxEnd,string QtyMinStart,string QtyMinEnd,string QtyMaxStart,string QtyMaxEnd,
                                              string IsEnable,string IsDisable,string QtyEachStart,string QtyEachEnd,string AmtEachStart,string AmtEachEnd,string CreateDateStart,
                                              string CreateDateEnd,string AppointShop,string SortField,string SortDirection,string PageIndex,string NumPerPage)
        {   
            int x;
            var cp = new GiftRuleParm();
            cp.CoID = int.Parse(GetCoid());
            if(!string.IsNullOrEmpty(ID))
            {
                if (int.TryParse(ID, out x))
                {
                    cp.ID = int.Parse(ID);
                }
            }
            cp.GiftNo = GiftNo;
            cp.GiftName = GiftName;
            DateTime date;
            if (DateTime.TryParse(DateFrom, out date))
            {
                cp.DateFrom = DateTime.Parse(DateFrom);
            }
            if (DateTime.TryParse(DateTo, out date))
            {
                cp.DateTo = DateTime.Parse(DateTo);
            }
            if (DateTime.TryParse(CreateDateStart, out date))
            {
                cp.CreateDateStart = DateTime.Parse(CreateDateStart);
            }
            if (DateTime.TryParse(CreateDateEnd, out date))
            {
                cp.CreateDateEnd = DateTime.Parse(CreateDateEnd);
            }
            cp.AppointSkuID = AppointSkuID;
            cp.ExcludeSkuID = ExcludeSkuID;
            cp.AmtMinStart = AmtMinStart;
            cp.AmtMinEnd = AmtMinEnd;
            cp.AmtMaxStart = AmtMaxStart;
            cp.AmtMaxEnd = AmtMaxEnd;
            cp.QtyMinStart = QtyMinStart;
            cp.QtyMinEnd = QtyMinEnd;
            cp.QtyMaxStart = QtyMaxStart;
            cp.QtyMaxEnd = QtyMaxEnd;
            cp.QtyEachStart = QtyEachStart;
            cp.QtyEachEnd = QtyEachEnd;
            cp.AmtEachStart = AmtEachStart;
            cp.AmtEachEnd = AmtEachEnd;
            cp.AppointShop = AppointShop;
            bool b;
            if (bool.TryParse(IsEnable, out b))
            {
                cp.IsEnable = bool.Parse(IsEnable);
            }
            if (bool.TryParse(IsDisable, out b))
            {
                cp.IsDisable = bool.Parse(IsDisable);
            }
            if(!string.IsNullOrEmpty(SortField))
            {
                if(CommHaddle.SysColumnExists(DbBase.CommConnectString,"gift",SortField).s == 1)
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
            var data = GiftHaddle.GetGiftRuleList(cp);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
    }
}