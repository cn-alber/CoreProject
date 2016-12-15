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
    }
}