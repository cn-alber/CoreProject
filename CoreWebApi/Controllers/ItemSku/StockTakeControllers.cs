using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using CoreData.CoreCore;
using CoreModels.XyCore;
using System;
using System.Collections.Generic;
using CoreData.CoreComm;
using CoreData;
using CoreModels;
namespace CoreWebApi.XyCore

{
    [AllowAnonymous]
    public class StockTakeControllers : ControllBase
    {
        #region 库存盘点-表头查询
        [HttpGetAttribute("Core/XyCore/StockTake/StockTakeMainLst")]
        public ResponseResult StockTakeMainLst(string WhID, string DateF, string DateT, string Status, string Skuautoid, string PageIndex, string PageSize, string SortField, string SortDirection)
        {
            var cp = new Sfc_item_param();
            int x;
            DateTime dt;
            if (int.TryParse(WhID, out x))
            {
                cp.WhID = WhID;
            }
            if (DateTime.TryParse(DateF, out dt))
            {
                string date = Convert.ToDateTime(DateF).ToString("yyyy-MM-dd");
                cp.DateF = date;
            }
            if (DateTime.TryParse(DateT, out dt))
            {
                string date = Convert.ToDateTime(DateF).ToString("yyyy-MM-dd") + " " + "23:59:59";
                cp.DateT = DateT;
            }
            if (!string.IsNullOrEmpty(Status) && int.TryParse(Status, out x))
            {
                cp.Status = Status;
            }
            if (int.TryParse(Skuautoid, out x))
            {
                cp.Skuautoid = Skuautoid;
            }
            if (int.TryParse(PageIndex, out x))
            {
                cp.PageIndex = int.Parse(PageIndex);
            }
            if (int.TryParse(PageSize, out x))
            {
                cp.PageSize = int.Parse(PageSize);
            }
            //排序参数赋值
            if (!string.IsNullOrEmpty(SortField))
            {
                var res = CommHaddle.SysColumnExists(DbBase.CoreConnectString, "sfc_main", SortField);
                if (res.s == 1)
                {
                    cp.SortField = SortField;
                    if (!string.IsNullOrEmpty(SortDirection) && (SortDirection.ToUpper() == "DESC" || SortDirection.ToUpper() == "ASC"))
                    {
                        cp.SortDirection = SortDirection.ToUpper();
                    }
                }
            }
            else
            {
                cp.SortField = "CreateDate";
                cp.SortDirection = "DESC";
            }
            cp.CoID = GetCoid();
            var Result = StockTakeHaddle.GetStockTakeMain(cp);
            return CoreResult.NewResponse(Result.s, Result.d, "General");
        }
        #endregion

        #region 库存盘点-明细查询
        [HttpGetAttribute("Core/XyCore/StockTake/StockTakeItemLst")]
        public ResponseResult StockTakeItemLst(string ParentID)
        {
            var res = new DataResult(1, null);
            int x;
            if (int.TryParse(ParentID, out x))
            {
                var cp = new Sfc_item_param();
                cp.CoID = GetCoid();
                cp.ParentID = int.Parse(ParentID);
                res = StockTakeHaddle.GetStockTakeItem(cp);
            }
            else
            {
                res.s = -1;
                res.d = "无效单数ParentID";
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }

        #endregion


        #region 库存盘点 - 新增盘点表头        
        [HttpPostAttribute("Core/XyCore/StockTake/InsertTakeMain")]
        public ResponseResult InsertTakeMain([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            int x;
            if (obj["WhID"] != null && obj["Parent_WhID"] != null)
            {
                string WhID = obj["WhID"].ToString();
                string Parent_WhID = obj["Parent_WhID"].ToString();
                if (!int.TryParse(WhID, out x) && !int.TryParse(Parent_WhID, out x))
                {
                    res.s = -1;
                    res.d = "无效参数";
                }
                else
                {
                    string CoID = GetCoid();
                    string UserName = GetUname();
                    res = StockTakeHaddle.InsertStockTakeMain(WhID, Parent_WhID, 2, CoID, UserName);
                }
            }
            else
            {
                res.s = -1;
                res.d = "无效参数";
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 库存盘点 - 新增盘点子表
        [HttpPostAttribute("Core/XyCore/StockTake/InsertTakeItem")]
        public ResponseResult InsertTakeItem([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            int x;
            if (obj["ParentID"] != null && obj["SkuIDLst"] != null)
            {
                var ParentID = obj["ParentID"].ToString();
                var SkuIDLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(obj["SkuIDLst"].ToString());
                if (!int.TryParse(ParentID, out x))
                {
                    res.s = -1;
                    res.d = "无效参数";
                }
                else
                {
                    string CoID = GetCoid();
                    string UserName = GetUname();
                    res = StockTakeHaddle.InsertStockTakeItem(ParentID, SkuIDLst, 2, CoID, UserName);
                }
            }
            else
            {
                res.s = -1;
                res.d = "无效参数";
            }

            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 库存盘点 - 修改保存盘点数量
        [HttpPostAttribute("Core/XyCore/StockTake/SaveTakeQty")]
        public ResponseResult SaveTakeQty([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            int x;
            if (!(obj["ID"] != null && obj["InvQty"] != null && int.TryParse(obj["ID"].ToString(), out x) && int.TryParse(obj["InvQty"].ToString(), out x)))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                string ID = obj["ID"].ToString();
                string InvQty = obj["InvQty"].ToString();
                string CoID = GetCoid();
                string UserName = GetUname();
                res = StockTakeHaddle.SaveStockTakeQty(ID, InvQty, CoID, UserName);
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 库存盘点 - 确认生效
        [HttpPostAttribute("Core/XyCore/StockTake/CheckTake")]
        public ResponseResult CheckTake([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            int x;
            if (!(obj["ID"] != null && int.TryParse(obj["ID"].ToString(), out x)))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                string CoID = GetCoid();
                string UserName = GetUname();
                string ID = obj["ID"].ToString();
                res = StockTakeHaddle.CheckStockTake(ID, CoID, UserName);
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 库存盘点 - 作废盘点单
        [HttpPostAttribute("Core/XyCore/StockTake/UnCheckTake")]
        public ResponseResult UnCheckTake([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            int x;
            if (!(obj["ID"] != null && int.TryParse(obj["ID"].ToString(), out x)))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                string CoID = GetCoid();
                string UserName = GetUname();
                string ID = obj["ID"].ToString();
                res = StockTakeHaddle.UnCheckStockTake(ID, 1, CoID, UserName);
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 保存备注 - 查询
        [HttpGetAttribute("Core/XyCore/StockTake/TakeRemarkQuery")]
        public ResponseResult TakeRemarkQuery(string ID)
        {
            var res = new DataResult(1, null);
            int x;
            if (!int.TryParse(ID, out x))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                string CoID = GetCoid();
                res = StockTakeHaddle.StockTakeRemarkQuery(ID, CoID);
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 保存备注 - 更新
        [HttpPostAttribute("Core/XyCore/StockTake/UptTakeRemark")]
        public ResponseResult UptTakeRemark([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            int x;
            if (!(obj["ID"] != null && int.TryParse(obj["ID"].ToString(), out x)&&obj["Remark"]!=null))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                string CoID = GetCoid();
                string UserName = GetUname();
                string ID = obj["ID"].ToString();
                string Remark = obj["Remark"].ToString();
                res = StockTakeHaddle.UptStockTakeRemark(ID, Remark, CoID, UserName);
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion
    }
}