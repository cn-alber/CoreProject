using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using CoreModels.XyCore;
using CoreData.CoreCore;
using System;
using CoreData.CoreComm;
using CoreData;
using System.Collections.Generic;
namespace CoreWebApi
{
    [AllowAnonymous]
    public class BatchController : ControllBase
    {
        [HttpGetAttribute("/Core/Batch/GetBatchInit")]
        public ResponseResult GetBatchInit()
        {
            var data = BatchHaddle.GetBatchInit(int.Parse(GetCoid()));
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Batch/GetBatchList")]
        public ResponseResult GetBatchList(string ID,string Status,string Remark,string PickorID,string DateStart,string Dateend,string Task,string Type,string SortField,
                                           string SortDirection,string PageIndex,string NumPerPage)
        {   
            int x;
            var cp = new BatchParm();
            cp.CoID = int.Parse(GetCoid());
            if(!string.IsNullOrEmpty(ID))
            {
                if (int.TryParse(ID, out x))
                {
                    cp.ID = int.Parse(ID);
                }
            }
            if(!string.IsNullOrEmpty(Status))
            {
                string[] a = Status.Split(',');
                List<int> s = new List<int>();
                foreach(var i in a)
                {
                    s.Add(int.Parse(i));
                }
                cp.Status = s;
            }
            if(!string.IsNullOrEmpty(PickorID))
            {
                string[] a = PickorID.Split(',');
                List<int> s = new List<int>();
                foreach(var i in a)
                {
                    s.Add(int.Parse(i));
                }
                cp.PickorID = s;
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
            cp.Remark = Remark;
            cp.Task = Task;
            DateTime date;
            if (DateTime.TryParse(DateStart, out date))
            {
                cp.DateStart = DateTime.Parse(DateStart);
            }
            if (DateTime.TryParse(Dateend, out date))
            {
                cp.DateEnd = DateTime.Parse(Dateend);
            }
            if(!string.IsNullOrEmpty(SortField))
            {
                if(CommHaddle.SysColumnExists(DbBase.CoreConnectString,"batch",SortField).s == 1)
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
            var data = BatchHaddle.GetBatchList(cp);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Batch/GetConfigure")]
        public ResponseResult GetConfigure(string Type)
        {
            if(!string.IsNullOrEmpty(Type))
            {
                Type = Type.ToUpper();
                if(Type!= "A" && Type!= "B" && Type!= "C" && Type!= "D" && Type!= "E" && Type!= "F" && Type!= "G" && Type!= "H")
                {
                    return CoreResult.NewResponse(-1, "参数类型无效", "General"); 
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "参数类型必填", "General"); 
            }
            var data = BatchHaddle.GetConfigure(int.Parse(GetCoid()),Type);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Batch/SetConfigure")]
        public ResponseResult SetConfigure([FromBodyAttribute]JObject co)
        {   
            string Type = "",TypeValue="";
            if(co["Type"] != null)
            {
                Type = co["Type"].ToString();
                if(!string.IsNullOrEmpty(Type))
                {
                    Type = Type.ToUpper();
                    if(Type!= "A" && Type!= "B" && Type!= "C" && Type!= "D" && Type!= "E" && Type!= "F" && Type!= "G" && Type!= "H")
                    {
                        return CoreResult.NewResponse(-1, "参数类型无效", "General"); 
                    }
                }
                else
                {
                    return CoreResult.NewResponse(-1, "参数类型必填", "General"); 
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "参数类型必填", "General"); 
            }
            if(co["TypeValue"] != null)
            {
                TypeValue = co["TypeValue"].ToString();
                if(!string.IsNullOrEmpty(TypeValue))
                {
                    if(Type == "A" || Type == "B" || Type == "C" || Type == "D" || Type == "E")
                    {
                        int x,y;
                        if (int.TryParse(TypeValue, out x))
                        {
                            y = int.Parse(TypeValue);
                            if((Type == "A" || Type == "B") && y <= 0)
                            {
                                return CoreResult.NewResponse(-1, "单件单批/多件单批订单数必须大于零", "General"); 
                            }
                            if((Type == "C" || Type == "D" || Type == "E") && y < 0)
                            {
                                return CoreResult.NewResponse(-1, "数量必须大于等于零", "General"); 
                            }
                        }
                        else
                        {
                            return CoreResult.NewResponse(-1, "参数值无效", "General"); 
                        }
                    }
                    if(Type == "F" || Type == "G")
                    {
                        if(TypeValue.Contains("A"))
                        {
                            TypeValue = "A";
                        }
                    }
                    if(Type == "H")
                    {
                        if (TypeValue.ToUpper() != "TRUE" && TypeValue.ToUpper() != "FALSE")
                        {
                            return CoreResult.NewResponse(-1, "参数值无效", "General"); 
                        }
                    }
                }
                else
                {
                    return CoreResult.NewResponse(-1, "参数值必填", "General"); 
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "参数值必填", "General"); 
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = BatchHaddle.SetConfigure(CoID,Type,TypeValue);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Batch/ModifyRemark")]
        public ResponseResult ModifyRemark([FromBodyAttribute]JObject co)
        {   
            string Remark = "";
            var id = new List<int>();
            if(co["Remark"] != null)
            {
                Remark = co["Remark"].ToString();
            }
            if(co["ID"] != null)
            {
                id = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["ID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "批次ID必填", "General");
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = BatchHaddle.ModifyRemark(CoID,id,username,Remark);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Batch/ModifyRemarkAll")]
        public ResponseResult ModifyRemarkAll([FromBodyAttribute]JObject co)
        {   
            string Remark = "";
            var id = new List<int>();
            if(co["Remark"] != null)
            {
                Remark = co["Remark"].ToString();
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = BatchHaddle.ModifyRemarkAll(CoID,username,Remark);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Batch/MarkPrint")]
        public ResponseResult MarkPrint([FromBodyAttribute]JObject co)
        {   
            var id = new List<int>();
            if(co["ID"] != null)
            {
                id = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["ID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "批次ID必填", "General");
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = BatchHaddle.MarkPrint(CoID,id,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Batch/CancleMarkPrint")]
        public ResponseResult CancleMarkPrint([FromBodyAttribute]JObject co)
        {   
            var id = new List<int>();
            if(co["ID"] != null)
            {
                id = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["ID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "批次ID必填", "General");
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = BatchHaddle.CancleMarkPrint(CoID,id,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Batch/GetPickorInit")]
        public ResponseResult GetPickorInit()
        {
            var data = BatchHaddle.GetPickorInit(int.Parse(GetCoid()));
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Batch/GetPickorByRole")]
        public ResponseResult GetPickorByRole(string RoleID)
        {
            int x,role;
            if(!string.IsNullOrEmpty(RoleID))
            {
                if (int.TryParse(RoleID, out x))
                {
                    role = int.Parse(RoleID);
                }
                else
                {
                    return CoreResult.NewResponse(-1, "角色ID无效", "General"); 
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "角色ID必填", "General"); 
            }
            var data = BatchHaddle.GetPickorByRole(int.Parse(GetCoid()),role);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Batch/SetPickor")]
        public ResponseResult SetPickor([FromBodyAttribute]JObject co)
        {   
            var id = new List<int>();
            if(co["ID"] != null)
            {
                id = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["ID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "批次ID必填", "General");
            }
            var Pickor = new List<int>();
            if(co["Pickor"] != null)
            {
                Pickor = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["Pickor"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "拣货人员必填", "General");
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = BatchHaddle.SetPickor(CoID,id,Pickor,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Batch/ReSetPickor")]
        public ResponseResult ReSetPickor([FromBodyAttribute]JObject co)
        {   
            var id = new List<int>();
            if(co["ID"] != null)
            {
                id = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["ID"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "批次ID必填", "General");
            }
            var Pickor = new List<int>();
            if(co["Pickor"] != null)
            {
                Pickor = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(co["Pickor"].ToString());
            }
            else
            {
                return CoreResult.NewResponse(-1, "拣货人员必填", "General");
            }
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = BatchHaddle.ReSetPickor(CoID,id,Pickor,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Batch/GetOrdCount")]
        public ResponseResult GetOrdCount()
        {
            var data = BatchHaddle.GetOrdCount(int.Parse(GetCoid()));
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Batch/SetSingleOrd")]
        public ResponseResult SetSingleOrd([FromBodyAttribute]JObject co)
        {   
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = BatchHaddle.SetSingleOrd(CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Batch/GetStrategySimple")]
        public ResponseResult GetStrategySimple(string Type)
        {
            int x,type;
            if(!string.IsNullOrEmpty(Type))
            {
                if (int.TryParse(Type, out x))
                {
                    type = int.Parse(Type);
                }
                else
                {
                    return CoreResult.NewResponse(-1, "批次类型参数无效", "General"); 
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "批次类型必填", "General"); 
            }
            var data = BatchHaddle.GetStrategySimple(int.Parse(GetCoid()),type);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Batch/InsertStrategy")]
        public ResponseResult InsertStrategy([FromBodyAttribute]JObject co)
        {   
            var strategy = new BatchStrategy();
            if(co["Type"] != null)
            {
                string Text = co["Type"].ToString();
                if(!string.IsNullOrEmpty(Text))
                {
                    int x,y;
                    if (int.TryParse(Text, out x))
                    {
                        y = int.Parse(Text);
                        if(y != 0 && y != 1)
                        {
                            return CoreResult.NewResponse(-1, "批次类型参数异常", "General"); 
                        }
                        else
                        {
                            strategy.Type = y;
                        }
                    }
                    else
                    {
                        return CoreResult.NewResponse(-1, "批次类型参数异常", "General"); 
                    }
                }
                else
                {
                    return CoreResult.NewResponse(-1, "批次类型必填", "General"); 
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "批次类型必填", "General"); 
            }
            if(co["StrategyName"] != null)
            {
                string Text = co["StrategyName"].ToString();
                if(!string.IsNullOrEmpty(Text))
                {
                    strategy.StrategyName = Text;
                }
                else
                {
                    return CoreResult.NewResponse(-1, "策略名称必填", "General"); 
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "策略名称必填", "General"); 
            }
            if(co["SkuIn"] != null)
            {
                strategy.SkuIn = co["SkuIn"].ToString();
            }
            if(co["SkuNotIn"] != null)
            {
                strategy.SkuNotIn = co["SkuNotIn"].ToString();
            }
            if(co["OrdGift"] != null)
            {
                string Text = co["OrdGift"].ToString();
                if(!string.IsNullOrEmpty(Text))
                {
                    int x,y;
                    if (int.TryParse(Text, out x))
                    {
                        y = int.Parse(Text);
                        if(y != 0 && y != 1 && y != 2)
                        {
                            return CoreResult.NewResponse(-1, "订单包含赠品参数异常", "General"); 
                        }
                        else
                        {
                            strategy.OrdGift = y;
                        }
                    }
                    else
                    {
                        return CoreResult.NewResponse(-1, "订单包含赠品参数异常", "General"); 
                    }
                }
                else
                {
                    return CoreResult.NewResponse(-1, "订单包含赠品必填", "General"); 
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "订单包含赠品必填", "General"); 
            }
            if(co["KindIDIn"] != null)
            {
                strategy.KindIDIn = co["KindIDIn"].ToString();
            }
            if(co["PCodeIn"] != null)
            {
                strategy.PCodeIn = co["PCodeIn"].ToString();
            }
            if(co["ExpPrint"] != null)
            {
                string Text = co["ExpPrint"].ToString();
                if(!string.IsNullOrEmpty(Text))
                {
                    int x,y;
                    if (int.TryParse(Text, out x))
                    {
                        y = int.Parse(Text);
                        if(y != 0 && y != 1 && y != 2)
                        {
                            return CoreResult.NewResponse(-1, "限定打印快递单参数异常", "General"); 
                        }
                        else
                        {
                            strategy.ExpPrint = y;
                        }
                    }
                    else
                    {
                        return CoreResult.NewResponse(-1, "限定打印快递单参数异常", "General"); 
                    }
                }
                else
                {
                    return CoreResult.NewResponse(-1, "限定打印快递单必填", "General"); 
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "限定打印快递单必填", "General"); 
            }
            if(co["ExpressIn"] != null)
            {
                string Text = co["ExpressIn"].ToString();
                string[] a = Text.Split(',');
                int i = 0,j = 0;
                foreach(var aa in a)
                {
                    if(aa == "B")
                    {
                        i ++;
                    }
                    if(aa == "A")
                    {
                        j ++;
                    }
                }
                if(i > 0)
                {
                    strategy.ExpressIn = "B";
                }
                else if(j > 0)
                {
                    strategy.ExpressIn = "A";
                }
                else
                {
                    strategy.ExpressIn = co["ExpressIn"].ToString();
                }
            }
            if(co["DistributorIn"] != null)
            {
                strategy.DistributorIn = co["DistributorIn"].ToString();
            }
            if(co["ShopIn"] != null)
            {
                string Text = co["ShopIn"].ToString();
                string[] a = Text.Split(',');
                int i = 0,j = 0;
                foreach(var aa in a)
                {
                    if(aa == "B")
                    {
                        i ++;
                    }
                    if(aa == "A")
                    {
                        j ++;
                    }
                }
                if(i > 0)
                {
                    strategy.ShopIn = "B";
                }
                else if(j > 0)
                {
                    strategy.ShopIn = "A";
                }
                else
                {
                    strategy.ShopIn = co["ShopIn"].ToString();
                }
            }
            if(co["AmtMin"] != null)
            {
                string Text = co["AmtMin"].ToString();
                if(!string.IsNullOrEmpty(Text))
                {
                    decimal x;
                    if (decimal.TryParse(Text, out x))
                    {
                        strategy.AmtMin = Text;
                    }
                }
            }
            if(co["AmtMax"] != null)
            {
                string Text = co["AmtMax"].ToString();
                if(!string.IsNullOrEmpty(Text))
                {
                    decimal x;
                    if (decimal.TryParse(Text, out x))
                    {
                        strategy.AmtMax = Text;
                    }
                }
            }
            if(co["PayDateStart"] != null)
            {
                string Text = co["PayDateStart"].ToString();
                if(!string.IsNullOrEmpty(Text))
                {
                    DateTime x;
                    if (DateTime.TryParse(Text, out x))
                    {
                        strategy.PayDateStart = Text;
                    }
                }
            }
            if(co["PayDateEnd"] != null)
            {
                string Text = co["PayDateEnd"].ToString();
                if(!string.IsNullOrEmpty(Text))
                {
                    DateTime x;
                    if (DateTime.TryParse(Text, out x))
                    {
                        strategy.PayDateEnd = Text;
                    }
                }
            }
            if(co["RecMessage"] != null)
            {
                strategy.RecMessage = co["RecMessage"].ToString();
            }
            if(co["SendMessage"] != null)
            {
                strategy.SendMessage = co["SendMessage"].ToString();
            }
            if(co["PrioritySku"] != null)
            {
                strategy.PrioritySku = co["PrioritySku"].ToString();
            }
            if(co["OrdQty"] != null)
            {
                string Text = co["OrdQty"].ToString();
                if(!string.IsNullOrEmpty(Text))
                {
                    int x;
                    if (int.TryParse(Text, out x))
                    {
                        strategy.OrdQty = Text;
                    }
                }
            }
            strategy.CoID = int.Parse(GetCoid());
            var data = BatchHaddle.InsertStrategy(strategy);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Batch/GetStrategyEdit")]
        public ResponseResult GetStrategyEdit(string ID)
        {
            int x,id;
            if(!string.IsNullOrEmpty(ID))
            {
                if (int.TryParse(ID, out x))
                {
                    id = int.Parse(ID);
                }
                else
                {
                    return CoreResult.NewResponse(-1, "策略ID参数无效", "General"); 
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "策略ID必填", "General"); 
            }
            var data = BatchHaddle.GetStrategyEdit(id,int.Parse(GetCoid()));
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Batch/UpdateStrategy")]
        public ResponseResult UpdateStrategy([FromBodyAttribute]JObject co)
        {   
            int ID;
            string StrategyName = null,SkuIn = null,SkuNotIn = null,OrdGift = null,KindIDIn = null,PCodeIn = null,ExpPrint = null,ExpressIn = null,DistributorIn = null,ShopIn = null,
                   AmtMin = null,AmtMax = null,PayDateStart = null, PayDateEnd = null,RecMessage = null,SendMessage = null,PrioritySku=null,OrdQty=null;
            if(co["ID"] != null)
            {
                string Text = co["ID"].ToString();
                if(!string.IsNullOrEmpty(Text))
                {
                    int x;
                    if (int.TryParse(Text, out x))
                    {
                        ID = int.Parse(Text);
                    }
                    else
                    {
                        return CoreResult.NewResponse(-1, "策略ID参数异常", "General"); 
                    }
                }
                else
                {
                    return CoreResult.NewResponse(-1, "策略ID必填", "General"); 
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "策略ID必填", "General"); 
            }
            if(co["StrategyName"] != null)
            {
                StrategyName = co["StrategyName"].ToString();
                if(string.IsNullOrEmpty(StrategyName))
                {
                    return CoreResult.NewResponse(-1, "策略名称必填", "General"); 
                }
            }
            if(co["SkuIn"] != null)
            {
                SkuIn = co["SkuIn"].ToString();
            }
            if(co["SkuNotIn"] != null)
            {
                SkuNotIn = co["SkuNotIn"].ToString();
            }
            if(co["OrdGift"] != null)
            {
                OrdGift = co["OrdGift"].ToString();
                if(!string.IsNullOrEmpty(OrdGift))
                {
                    int x,y;
                    if (int.TryParse(OrdGift, out x))
                    {
                        y = int.Parse(OrdGift);
                        if(y != 0 && y != 1 && y != 2)
                        {
                            return CoreResult.NewResponse(-1, "订单包含赠品参数异常", "General"); 
                        }
                    }
                    else
                    {
                        return CoreResult.NewResponse(-1, "订单包含赠品参数异常", "General"); 
                    }
                }
                else
                {
                    return CoreResult.NewResponse(-1, "订单包含赠品参数异常", "General"); 
                }
            }
            if(co["KindIDIn"] != null)
            {
                KindIDIn = co["KindIDIn"].ToString();
            }
            if(co["PCodeIn"] != null)
            {
                PCodeIn = co["PCodeIn"].ToString();
            }
            if(co["ExpPrint"] != null)
            {
                ExpPrint = co["ExpPrint"].ToString();
                if(!string.IsNullOrEmpty(ExpPrint))
                {
                    int x,y;
                    if (int.TryParse(ExpPrint, out x))
                    {
                        y = int.Parse(ExpPrint);
                        if(y != 0 && y != 1 && y != 2)
                        {
                            return CoreResult.NewResponse(-1, "限定打印快递单参数异常", "General"); 
                        }
                    }
                    else
                    {
                        return CoreResult.NewResponse(-1, "限定打印快递单参数异常", "General"); 
                    }
                }
                else
                {
                    return CoreResult.NewResponse(-1, "限定打印快递单参数异常", "General"); 
                }
            }
            if(co["ExpressIn"] != null)
            {
                ExpressIn = co["ExpressIn"].ToString();
                string[] a = ExpressIn.Split(',');
                int i = 0,j = 0;
                foreach(var aa in a)
                {
                    if(aa == "B")
                    {
                        i ++;
                    }
                    if(aa == "A")
                    {
                        j ++;
                    }
                }
                if(i > 0)
                {
                    ExpressIn = "B";
                }
                else if(j > 0)
                {
                    ExpressIn = "A";
                }
            }
            if(co["DistributorIn"] != null)
            {
                DistributorIn = co["DistributorIn"].ToString();
            }
            if(co["ShopIn"] != null)
            {
                ShopIn = co["ShopIn"].ToString();
                string[] a = ShopIn.Split(',');
                int i = 0,j = 0;
                foreach(var aa in a)
                {
                    if(aa == "B")
                    {
                        i ++;
                    }
                    if(aa == "A")
                    {
                        j ++;
                    }
                }
                if(i > 0)
                {
                    ShopIn = "B";
                }
                else if(j > 0)
                {
                    ShopIn = "A";
                }
            }
            if(co["AmtMin"] != null)
            {
                string Text = co["AmtMin"].ToString();
                if(!string.IsNullOrEmpty(Text))
                {
                    decimal x;
                    if (decimal.TryParse(Text, out x))
                    {
                        AmtMin = Text;
                    }
                }
            }
            if(co["AmtMax"] != null)
            {
                string Text = co["AmtMax"].ToString();
                if(!string.IsNullOrEmpty(Text))
                {
                    decimal x;
                    if (decimal.TryParse(Text, out x))
                    {
                        AmtMax = Text;
                    }
                }
            }
            if(co["PayDateStart"] != null)
            {
                string Text = co["PayDateStart"].ToString();
                if(!string.IsNullOrEmpty(Text))
                {
                    DateTime x;
                    if (DateTime.TryParse(Text, out x))
                    {
                        PayDateStart = Text;
                    }
                }
            }
            if(co["PayDateEnd"] != null)
            {
                string Text = co["PayDateEnd"].ToString();
                if(!string.IsNullOrEmpty(Text))
                {
                    DateTime x;
                    if (DateTime.TryParse(Text, out x))
                    {
                        PayDateEnd = Text;
                    }
                }
            }
            if(co["RecMessage"] != null)
            {
                RecMessage = co["RecMessage"].ToString();
            }
            if(co["SendMessage"] != null)
            {
                SendMessage = co["SendMessage"].ToString();
            }
            if(co["PrioritySku"] != null)
            {
                PrioritySku = co["PrioritySku"].ToString();
            }
            if(co["OrdQty"] != null)
            {
                string Text = co["OrdQty"].ToString();
                if(!string.IsNullOrEmpty(Text))
                {
                    int x;
                    if (int.TryParse(Text, out x))
                    {
                        OrdQty = Text;
                    }
                }
            }
            var data = BatchHaddle.UpdateStrategy(ID,int.Parse(GetCoid()),StrategyName,SkuIn,SkuNotIn,OrdGift,KindIDIn,PCodeIn,ExpPrint,ExpressIn,DistributorIn,ShopIn,AmtMin,AmtMax,
                                                  PayDateStart,PayDateEnd,RecMessage,SendMessage,PrioritySku,OrdQty);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Batch/DeleteStrategy")]
        public ResponseResult DeleteStrategy([FromBodyAttribute]JObject co)
        {   
            int ID,Type;
            if(co["Type"] != null)
            {
                string Text = co["Type"].ToString();
                if(!string.IsNullOrEmpty(Text))
                {
                    int x;
                    if (int.TryParse(Text, out x))
                    {
                        Type = int.Parse(Text);
                        if(Type != 0 && Type != 1)
                        {
                            return CoreResult.NewResponse(-1, "批次类型参数异常", "General"); 
                        }
                    }
                    else
                    {
                        return CoreResult.NewResponse(-1, "批次类型参数异常", "General"); 
                    }
                }
                else
                {
                    return CoreResult.NewResponse(-1, "批次类型必填", "General"); 
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "批次类型必填", "General"); 
            }
            if(co["ID"] != null)
            {
                string Text = co["ID"].ToString();
                if(!string.IsNullOrEmpty(Text))
                {
                    int x;
                    if (int.TryParse(Text, out x))
                    {
                        ID = int.Parse(Text);
                    }
                    else
                    {
                        return CoreResult.NewResponse(-1, "策略ID参数异常", "General"); 
                    }
                }
                else
                {
                    return CoreResult.NewResponse(-1, "策略ID必填", "General"); 
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "策略ID必填", "General"); 
            }
            var data = BatchHaddle.DeleteStrategy(ID,int.Parse(GetCoid()),Type);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Batch/SetSingleOrdStrategy")]
        public ResponseResult SetSingleOrdStrategy([FromBodyAttribute]JObject co)
        {   
            int ID;
            if(co["ID"] != null)
            {
                string Text = co["ID"].ToString();
                if(!string.IsNullOrEmpty(Text))
                {
                    int x;
                    if (int.TryParse(Text, out x))
                    {
                        ID = int.Parse(Text);
                    }
                    else
                    {
                        return CoreResult.NewResponse(-1, "策略ID参数异常", "General"); 
                    }
                }
                else
                {
                    return CoreResult.NewResponse(-1, "策略ID必填", "General"); 
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "策略ID必填", "General"); 
            }
            var data = BatchHaddle.SetSingleOrdStrategy(int.Parse(GetCoid()),GetUname(),ID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Batch/SetMultiOrd")]
        public ResponseResult SetMultiOrd([FromBodyAttribute]JObject co)
        {   
            string username = GetUname();
            int CoID = int.Parse(GetCoid());
            var data = BatchHaddle.SetMultiOrd(CoID,username);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
        [HttpPostAttribute("/Core/Batch/SetMultiOrdStrategy")]
        public ResponseResult SetMultiOrdStrategy([FromBodyAttribute]JObject co)
        {   
            int ID;
            if(co["ID"] != null)
            {
                string Text = co["ID"].ToString();
                if(!string.IsNullOrEmpty(Text))
                {
                    int x;
                    if (int.TryParse(Text, out x))
                    {
                        ID = int.Parse(Text);
                    }
                    else
                    {
                        return CoreResult.NewResponse(-1, "策略ID参数异常", "General"); 
                    }
                }
                else
                {
                    return CoreResult.NewResponse(-1, "策略ID必填", "General"); 
                }
            }
            else
            {
                return CoreResult.NewResponse(-1, "策略ID必填", "General"); 
            }
            var data = BatchHaddle.SetMultiOrdStrategy(int.Parse(GetCoid()),GetUname(),ID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
    }
}