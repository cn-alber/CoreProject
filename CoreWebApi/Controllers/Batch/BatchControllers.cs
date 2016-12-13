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
    }
}