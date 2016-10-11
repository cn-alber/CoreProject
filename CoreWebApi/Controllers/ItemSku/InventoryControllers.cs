using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using CoreData.CoreCore;
using CoreModels.XyCore;
using System;
using System.Collections.Generic;
using CoreData.CoreComm;
using CoreData;
namespace CoreWebApi.XyCore
{
    [AllowAnonymous]
    public class InventoryController : ControllBase
    {
        #region 库存管理 - 获取库存查询
        [HttpGetAttribute("Core/XyCore/Inventory/InventQuery")]
        public ResponseResult InventQuery(string SkuID, string SkuName, string StockQtyb, string StockQtye, string WarehouseID, string Status, string PageIndex, string PageSize, string SortField, string SortDirection)
        {
            var cp = new InvQueryParam();
            int x;
            if (!string.IsNullOrEmpty(SkuID))//商品编码
            {
                cp.SkuID = SkuID;
            }
            if (!string.IsNullOrEmpty(SkuName))//商品名称
            {
                cp.SkuName = SkuName;
            }
            if (!string.IsNullOrEmpty(StockQtyb) && int.TryParse(StockQtyb, out x))//主仓实际库存数量起
            {
                cp.StockQtyb = int.Parse(StockQtyb);
            }
            if (!string.IsNullOrEmpty(StockQtye) && int.TryParse(StockQtye, out x))//主仓实际库存数量迄
            {
                cp.StockQtye = int.Parse(StockQtye);
            }
            if (!string.IsNullOrEmpty(WarehouseID) && int.TryParse(WarehouseID, out x))//商品仓库
            {
                cp.WarehouseID = int.Parse(WarehouseID);
            }
            if (!string.IsNullOrEmpty(Status) && int.TryParse(Status, out x))//库存状态:0.全部,1.充足,2.预警
            {
                cp.Status = int.Parse(Status);
            }
            if (int.TryParse(PageIndex, out x))//页码
            {
                cp.PageIndex = int.Parse(PageIndex);
            }
            if (int.TryParse(PageSize, out x))//每页笔数
            {
                cp.PageSize = int.Parse(PageSize);
            }
            //排序参数赋值
            if (!string.IsNullOrEmpty(SortField))
            {
                var res = CommHaddle.SysColumnExists(DbBase.CoreConnectString, "inventory", SortField);
                if (res.s == 1)
                {
                    cp.SortField = SortField;
                    if (!string.IsNullOrEmpty(SortDirection) && (SortDirection.ToUpper() == "DESC" || SortDirection.ToUpper() == "ASC"))
                    {
                        cp.SortDirection = SortDirection.ToUpper();
                    }
                }
            }
            cp.CoID = int.Parse(GetCoid());
            var Result = InventoryHaddle.GetInvQuery(cp);
            return CoreResult.NewResponse(Result.s, Result.d, "General");
        }
        #endregion

        #region 库存管理 - 库存明细查询
        [HttpGetAttribute("Core/XyCore/Inventory/InvDetailQuery")]
        public ResponseResult InvDetailQuery(string SkuID, string WarehouseID, string DocType, string DocDateB, string DocDateE, string PageIndex, string PageSize, string SortField, string SortDirection)
        {

            var cp = new InvQueryParam();
            int x;
            DateTime date;
            if (!string.IsNullOrEmpty(SkuID))//商品编码
            {
                cp.SkuID = SkuID;
            }
            if (!string.IsNullOrEmpty(WarehouseID) && int.TryParse(WarehouseID, out x))//商品仓库
            {
                cp.WarehouseID = int.Parse(WarehouseID);
            }
            if (!string.IsNullOrEmpty(DocType))//单据类型
            {
                cp.DocType = DocType;
            }
            if (!string.IsNullOrEmpty(DocDateB) && (DateTime.TryParse(DocDateB, out date)))//单据日期起
            {
                cp.DocDateB = DateTime.Parse(DocDateB);
            }
            if (!string.IsNullOrEmpty(DocDateE) && (DateTime.TryParse(DocDateE, out date)))//单据日期迄
            {
                cp.DocDateE = DateTime.Parse(DocDateE);
            }
            if (int.TryParse(PageIndex, out x))//页码
            {
                cp.PageIndex = int.Parse(PageIndex);
            }
            if (int.TryParse(PageSize, out x))//每页笔数
            {
                cp.PageSize = int.Parse(PageSize);
            }
            //排序参数赋值
            if (!string.IsNullOrEmpty(SortField))
            {
                var res = CommHaddle.SysColumnExists(DbBase.CoreConnectString, "inventory", SortField);
                if (res.s == 1)
                {
                    cp.SortField = SortField;
                    if (!string.IsNullOrEmpty(SortDirection) && (SortDirection.ToUpper() == "DESC" || SortDirection.ToUpper() == "ASC"))
                    {
                        cp.SortDirection = SortDirection.ToUpper();
                    }
                }
            }
            cp.CoID = int.Parse(GetCoid());
            var Result = InventoryHaddle.GetInvDetailQuery(cp);
            return CoreResult.NewResponse(Result.s, Result.d, "General");
        }
        #endregion
    }
}