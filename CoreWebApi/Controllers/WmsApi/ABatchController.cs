using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using CoreModels;
using CoreModels.WmsApi;
using CoreData.CoreWmsApi;
using System;
using System.Collections.Generic;
namespace CoreWebApi
{
    [AllowAnonymous]
    public class ABatchController : ControllBase
    {
        #region 获取批次任务
        [HttpGetAttribute("Core/ABatch/CallBatch")]
        public ResponseResult CallBatch(string Type, string Status)
        {
            var res = new DataResult(1, null);
            int x;
            if (!(!string.IsNullOrEmpty(Type) && int.TryParse(Type, out x) && !string.IsNullOrEmpty(Status) && int.TryParse(Status, out x)))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = new ABatchParams();
                cp.CoID = int.Parse(GetCoid());
                cp.Pickor = GetUid();
                cp.Type = int.Parse(Type);
                cp.Status = int.Parse(Status);
                res = ABatchHaddles.GetBatchTask(cp);
            }
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion

        #region 根据任务ID获取批次任务
        [HttpGetAttribute("Core/ABatch/CallBatchByID")]
        public ResponseResult CallBatchByID(string Type, string Status, string ID)
        {
            var res = new DataResult(1, null);
            int x;
            if (!(!string.IsNullOrEmpty(Type) && int.TryParse(Type, out x) &&
                 !string.IsNullOrEmpty(Status) && int.TryParse(Status, out x) &&
                 !string.IsNullOrEmpty(ID) && int.TryParse(ID, out x)))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = new ABatchParams();
                cp.CoID = int.Parse(GetCoid());
                cp.Type = int.Parse(Type);
                cp.Status = int.Parse(Status);
                if (!string.IsNullOrEmpty(ID))
                {
                    cp.ID = int.Parse(ID);
                }
                res = ABatchHaddles.GetBatchTaskByID(cp);
            }
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion

        #region 获取批次任务数量
        [HttpGetAttribute("Core/ABatch/CallBatchNum")]
        public ResponseResult CallBatchNum()
        {
            var res = new DataResult(1, null);
            int x;
            // if (string.IsNullOrEmpty(Status) || !string.IsNullOrEmpty(Status) && int.TryParse(Status, out x))
            // {
            //     res.s = -1;
            //     res.d = "无效参数";
            // }
            // else
            // {
            var cp = new ABatchParams();
            cp.CoID = int.Parse(GetCoid());
            cp.Pickor = GetUid();
            // cp.Status = int.Parse(Status);
            res = ABatchHaddles.GetBatchNum(cp);
            // }
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion

        #region 拣货至出货暂存仓 
        [HttpPostAttribute("Core/ABatch/SetBatchOut")]
        public ResponseResult SetBatchOut([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            int x;
            if (!(obj["BatchID"] != null && int.TryParse(obj["BatchID"].ToString(), out x) &&
                obj["BatchtaskID"] != null && int.TryParse(obj["BatchtaskID"].ToString(), out x))
                || obj["PCode"] == null || obj["SkuAuto"] == null
                || obj["BatchID"] == null || obj["BatchtaskID"] == null)
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = new AShelfSet();
                cp.CoID = int.Parse(GetCoid());
                cp.Creator = GetUname();
                cp.CreateDate = DateTime.Now.ToString();
                cp.PCode = obj["PCode"].ToString();
                cp.BatchID = int.Parse(obj["BatchID"].ToString());
                cp.BatchtaskID = int.Parse(obj["BatchID"].ToString());
                AShelvesHaddles.SetOffShelfPile(cp);
            }
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion

        #region 手动结单&标记缺货
        [HttpPostAttribute("Core/ABatch/SetBatchNoQty")]
        public ResponseResult SetBatchNoQty([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            int x;
            if (!(obj["Type"] != null && int.TryParse(obj["Type"].ToString(), out x) &&
               obj["ID"] != null && int.TryParse(obj["ID"].ToString(), out x))
               || obj["Type"] == null || obj["ID"] == null)
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = new ABatchParams();
                cp.CoID = int.Parse(GetCoid());
                cp.Pickor = GetUid();
                cp.Type = int.Parse(obj["Type"].ToString());

            }
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion

        #region 订单分拣-扫描件码显示可以绑定的分拣格
        [HttpGetAttribute("Core/ABatch/GetBatchSortCode")]
        public ResponseResult GetBatchSortCode(string BarCode)
        {
            var res = new DataResult(1, null);
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }

        [HttpPostAttribute("Core/ABatch/SetBatchSortCode")]
        public ResponseResult SetBatchSortCode([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion
    }
}