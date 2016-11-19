using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using CoreModels.XyComm;
// using CoreData.CoreCore;
// using System;
using CoreData.CoreComm;
using CoreData;
// using System.Collections.Generic;
using CoreModels;

namespace CoreWebApi
{
    [AllowAnonymous]
    public class ExpressController : ControllBase
    {
        [HttpGetAttribute("/Core/Express/GetExpressList")]
        public ResponseResult GetExpressList(string SortField,string SortDirection,string PageIndex,string NumPerPage)
        {   
            int x;
            if(!string.IsNullOrEmpty(SortField))
            {
                if(CommHaddle.SysColumnExists(DbBase.CommConnectString,"express",SortField).s != 1)
                {
                    SortField = "";
                }
            }
            else
            {
                SortField = "";
            }
            if(!string.IsNullOrEmpty(SortDirection))
            {
                 if(SortDirection.ToUpper() != "ASC" && SortDirection.ToUpper() != "DESC")
                {
                    SortDirection = "";
                }
            }
            else
            {
                SortDirection = "";
            }
            int num = 20,index = 1;
            if (int.TryParse(NumPerPage, out x))
            {
                num = int.Parse(NumPerPage);
            }
            if (int.TryParse(PageIndex, out x))
            {
                index = int.Parse(PageIndex);
            }
            int CoID = int.Parse(GetCoid());
            var data = ExpressHaddle.GetExpressList(CoID,SortField,SortDirection,index,num);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Express/InsertExpress")]
        public ResponseResult InsertExpress([FromBodyAttribute]JObject co)
        {   
            string ExpID = co["ExpID"].ToString();
            string ExpName = co["ExpName"].ToString();
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = ExpressHaddle.InsertExpress(CoID,ExpID,ExpName,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Express/GetExpressEdit")]
        public ResponseResult GetExpressEdit(string ID)
        {   
            int x,id = 0;
            var data = new DataResult(1,null);
            if (int.TryParse(ID, out x))
            {
                id = int.Parse(ID);
            }
            else
            {
                data.s = -1;
                data.d = "参数无效!";
                return CoreResult.NewResponse(data.s, data.d, "General"); 
            }
            int CoID = int.Parse(GetCoid());
            data = ExpressHaddle.GetExpressEdit(CoID,id);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Express/UpdateExpress")]
        public ResponseResult UpdateExpress([FromBodyAttribute]JObject co)
        {   
            var exp = new ExpressEdit();
            int x;
            decimal y;
            var data = new DataResult(1,null);
            string Text = co["ID"].ToString();
            if (int.TryParse(Text, out x))
            {
                exp.ID = int.Parse(Text);
            }
            else
            {
                data.s = -1;
                data.d = "快递ID参数异常!";
                return CoreResult.NewResponse(data.s, data.d, "General"); 
            }
            Text = co["Enable"].ToString();
            if(Text.ToUpper() == "TRUE" || Text.ToUpper() == "FALSE")
            {
                exp.Enable = bool.Parse(Text);
            }
            else
            {
                data.s = -1;
                data.d = "启用状态参数异常!";
                return CoreResult.NewResponse(data.s, data.d, "General"); 
            }
            Text = co["Priority"].ToString();
            if(string.IsNullOrEmpty(Text))
            {
                exp.Priority = null;
            }
            else
            {
                if (int.TryParse(Text, out x))
                {
                    exp.Priority = Text;
                }
                else
                {
                    data.s = -1;
                    data.d = "优先级参数异常!";
                    return CoreResult.NewResponse(data.s, data.d, "General"); 
                }
            }
            Text = co["FreightFirst"].ToString();
            if(string.IsNullOrEmpty(Text))
            {
                data.s = -1;
                data.d = "运费优先参数异常!";
                return CoreResult.NewResponse(data.s, data.d, "General"); 
            }
            else
            {
                if (int.TryParse(Text, out x))
                {
                    exp.FreightFirst = Text;
                }
                else
                {
                    data.s = -1;
                    data.d = "运费优先参数异常!";
                    return CoreResult.NewResponse(data.s, data.d, "General"); 
                }
            }
            Text = co["OrdAmtStart"].ToString();
            if(string.IsNullOrEmpty(Text))
            {
                exp.OrdAmtStart = null;
            }
            else
            {
                if (decimal.TryParse(Text, out y))
                {
                    exp.OrdAmtStart = Text;
                }
                else
                {
                    data.s = -1;
                    data.d = "订单金额大于等于参数异常!";
                    return CoreResult.NewResponse(data.s, data.d, "General"); 
                }
            }
            Text = co["OrdAmtEnd"].ToString();
            if(string.IsNullOrEmpty(Text))
            {
                exp.OrdAmtEnd = null;
            }
            else
            {
                if (decimal.TryParse(Text, out y))
                {
                    exp.OrdAmtEnd = Text;
                }
                else
                {
                    data.s = -1;
                    data.d = "订单金额小于等于参数异常!";
                    return CoreResult.NewResponse(data.s, data.d, "General"); 
                }
            }
            Text = co["IsCOD"].ToString();
            if(Text.ToUpper() == "TRUE" || Text.ToUpper() == "FALSE")
            {
                exp.IsCOD = bool.Parse(Text);
            }
            else
            {
                data.s = -1;
                data.d = "支持货到付款参数异常!";
                return CoreResult.NewResponse(data.s, data.d, "General"); 
            }
            Text = co["IgnoreArrival"].ToString();
            if(Text.ToUpper() == "TRUE" || Text.ToUpper() == "FALSE")
            {
                exp.IgnoreArrival = bool.Parse(Text);
            }
            else
            {
                data.s = -1;
                data.d = "忽略到达判断参数异常!";
                return CoreResult.NewResponse(data.s, data.d, "General"); 
            }
            Text = co["ExpCalMethod"].ToString();
            if(string.IsNullOrEmpty(Text))
            {
                data.s = -1;
                data.d = "自动配快递计算方式参数异常!";
                return CoreResult.NewResponse(data.s, data.d, "General"); 
            }
            else
            {
                if (int.TryParse(Text, out x))
                {
                    exp.ExpCalMethod = Text;
                }
                else
                {
                    data.s = -1;
                    data.d = "自动配快递计算方式参数异常!";
                    return CoreResult.NewResponse(data.s, data.d, "General"); 
                }
            }
            Text = co["UseProbability"].ToString();
            if(string.IsNullOrEmpty(Text))
            {
                exp.UseProbability = null;
            }
            else
            {
                if (int.TryParse(Text, out x))
                {
                    exp.UseProbability = Text;
                }
                else
                {
                    data.s = -1;
                    data.d = "采用概率参数异常!";
                    return CoreResult.NewResponse(data.s, data.d, "General"); 
                }
            }
            Text = co["OnlineOrder"].ToString();
            if(Text.ToUpper() == "TRUE" || Text.ToUpper() == "FALSE")
            {
                exp.OnlineOrder = bool.Parse(Text);
            }
            else
            {
                data.s = -1;
                data.d = "在线下单参数异常!";
                return CoreResult.NewResponse(data.s, data.d, "General"); 
            }
            exp.ExpName = co["ExpName"].ToString();
            exp.PriorityLogistics = co["PriorityLogistics"].ToString();
            exp.PrioritySku = co["PrioritySku"].ToString();
            exp.LimitedShop = co["LimitedShop"].ToString();
            exp.LimitedWarehouse = co["LimitedWarehouse"].ToString();
            exp.DisableArea = co["DisableArea"].ToString();
            exp.DisableSku = co["DisableSku"].ToString();
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            data = ExpressHaddle.UpdateExpress(CoID,exp,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
        [HttpGetAttribute("/Core/Express/GetExpressSimple")]
        public ResponseResult GetExpressSimple(string ID)
        {   
            int CoID = int.Parse(GetCoid());
            var data = ExpressHaddle.GetExpressSimple(CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
    }
}