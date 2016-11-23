using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using CoreData;
using CoreData.CoreCore;
using CoreData.CoreComm;
using CoreModels;
using CoreModels.XyCore;
using System;
using System.Collections.Generic;

namespace CoreWebApi.XyCore
{
    [AllowAnonymous]
    public class InventoryController : ControllBase
    {

        #region 库存管理 - 获取库存查询(分仓)
        [HttpGetAttribute("Core/XyCore/Inventory/InventQueryByWh")]
        public ResponseResult InventQueryByWh(string GoodsCode, string SkuID, string SkuName, string Norm, string StockQtyb, string StockQtye, string WarehouseID, string Status, string PageIndex, string PageSize, string SortField, string SortDirection)
        {
            var cp = new InvQueryParam();
            int x;
            if (!string.IsNullOrEmpty(GoodsCode))//款式编码
            {
                cp.GoodsCode = GoodsCode;
            }
            if (!string.IsNullOrEmpty(SkuID))//商品编码
            {
                cp.SkuID = SkuID;
            }
            if (!string.IsNullOrEmpty(SkuName))//商品名称
            {
                cp.SkuName = SkuName;
            }
            if (!string.IsNullOrEmpty(Norm))//颜色及规格
            {
                cp.Norm = Norm;
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
            var Result = InventoryHaddle.GetInvQueryByWh(cp);
            return CoreResult.NewResponse(Result.s, Result.d, "General");
        }
        #endregion
        #region 库存管理 - 商品库存查询(不分仓)
        [HttpGetAttribute("Core/XyCore/Inventory/InventQuery")]
        public ResponseResult InventQuery(string GoodsCode, string SkuID, string SkuName, string Norm, string StockQtyb, string StockQtye, string Status, string PageIndex, string PageSize, string SortField, string SortDirection)
        {
            var cp = new InvQueryParam();
            int x;
            if (!string.IsNullOrEmpty(GoodsCode))//款式编码
            {
                cp.GoodsCode = GoodsCode;
            }
            if (!string.IsNullOrEmpty(GoodsCode))//款式编码
            {
                cp.GoodsCode = GoodsCode;
            }
            if (!string.IsNullOrEmpty(SkuID))//商品编码
            {
                cp.SkuID = SkuID;
            }
            if (!string.IsNullOrEmpty(SkuName))//商品名称
            {
                cp.SkuName = SkuName;
            }
            if (!string.IsNullOrEmpty(Norm))//颜色及规格
            {
                cp.Norm = Norm;
            }
            if (!string.IsNullOrEmpty(StockQtyb) && int.TryParse(StockQtyb, out x))//主仓实际库存数量起
            {
                cp.StockQtyb = int.Parse(StockQtyb);
            }
            if (!string.IsNullOrEmpty(StockQtye) && int.TryParse(StockQtye, out x))//主仓实际库存数量迄
            {
                cp.StockQtye = int.Parse(StockQtye);
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
        #region 库存管理-商品库存查询 - 库存明细查询
        [HttpGetAttribute("Core/XyCore/Inventory/InvDetailQuery")]
        public ResponseResult InvDetailQuery(string SkuautoID, string WarehouseID, string DocType, string DocDateB, string DocDateE, string PageIndex, string PageSize, string SortField, string SortDirection)
        {

            var cp = new InvQueryParam();
            int x;
            DateTime date;
            if (!string.IsNullOrEmpty(SkuautoID))//商品编码
            {
                cp.SkuID = SkuautoID;
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
                var res = CommHaddle.SysColumnExists(DbBase.CoreConnectString, "invinoutitem", SortField);
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
                cp.SortField = "ID";
                cp.SortDirection = "DESC";
            }
            cp.CoID = int.Parse(GetCoid());
            var Result = InventoryHaddle.GetInvDetailQuery(cp);
            return CoreResult.NewResponse(Result.s, Result.d, "General");
        }
        #endregion

        #region 商品库存查询 - 修改现有库存-查询单笔库存明细(分仓)
        [HttpGetAttribute("Core/XyCore/Inventory/InventorySingle")]
        public ResponseResult InventorySingle(string ID)
        {
            var res = new DataResult(1, null);
            int invID = 0;
            if (!string.IsNullOrEmpty(ID) && int.TryParse(ID, out invID))
            {
                invID = int.Parse(ID);
            }
            if (invID > 0)
            {
                string CoID = GetCoid();
                res = InventoryHaddle.GetInventorySingle(invID, CoID);
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 商品库存查询 - 修改现有库存 - 产生盘点交易(分仓)
        [HttpPostAttribute("Core/XyCore/Inventory/UptStockQtySingle")]
        public ResponseResult UptStockQtySingle([FromBodyAttribute]JObject obj)
        {
            int ID = 0, InvQty = 0, Type = 0;
            var res = new DataResult(1, null);
            if (obj["ID"] != null && int.TryParse(obj["ID"].ToString(), out ID) &&
            obj["InvQty"] != null && int.TryParse(obj["InvQty"].ToString(), out InvQty) &&
            obj["Type"] != null && int.TryParse(obj["Type"].ToString(), out ID))
            {
                ID = int.Parse(obj["ID"].ToString());
                InvQty = int.Parse(obj["InvQty"].ToString());
                Type = int.Parse(obj["Type"].ToString());
                string CoID = GetCoid();
                string UserName = GetUname();
                if (ID > 0 && Type == 1)
                {
                    int sfc_type = 2;
                    int inv_type = 1401;
                    res = InventoryHaddle.SetTakeStockQty(ID, sfc_type, inv_type, InvQty, CoID, UserName);
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

        #region 商品库存查询 - 批量操作 - 库存清理(分仓)
        [HttpPostAttribute("Core/XyCore/Inventory/ClearInvStockQty")]
        public ResponseResult ClearInvStockQty([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            var IDLst = new List<int>();
            if (obj["IDLst"] != null)
            {
                IDLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(obj["IDLst"].ToString());
            }
            string CoID = GetCoid();
            string UserName = GetUname();
            int sfc_type = 2;
            int inv_type = 1401;
            res = InventoryHaddle.ClearInvQty(IDLst, sfc_type, inv_type, CoID, UserName);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region  商品库存查询 - 批量操作 - 清除零库存资料(分仓) 
        [HttpPostAttribute("Core/XyCore/Inventory/DeleteZeroSkuD")]
        public ResponseResult DeleteZeroSkuD([FromBodyAttribute]JObject obj)
        {
            string CoID = GetCoid();
            string UserName = GetUname();
            var IDLst = new List<int>();
            if (obj["IDLst"] != null)
            {
                IDLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(obj["IDLst"].ToString());
            }
            var res = InventoryHaddle.DelZeroSku(IDLst, 1, CoID, UserName);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region  商品库存查询 - 批量操作 - 清除零库存资料(主仓) 
        [HttpPostAttribute("Core/XyCore/Inventory/DeleteZeroSkuM")]
        public ResponseResult DeleteZeroSkuM([FromBodyAttribute]JObject obj)
        {
            string CoID = GetCoid();
            string UserName = GetUname();
            var IDLst = new List<int>();
            if (obj["IDLst"] != null)
            {
                IDLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(obj["IDLst"].ToString());
            }
            var res = InventoryHaddle.DelZeroSku(IDLst, 1, CoID, UserName);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region  商品库存查询 - 修改安全库存-单笔(分仓)


        #endregion



        #region 商品库存查询 - 修改安全库存 - 查询库存明细
        [HttpGetAttribute("Core/XyCore/Inventory/InvSafeQtyLst")]
        public ResponseResult InvSafeQtyLst(string GoodsCode, string WarehouseID)
        {
            var res = new DataResult(1, null);
            int wid = 0;
            if (!string.IsNullOrEmpty(WarehouseID) && int.TryParse(WarehouseID, out wid))//商品仓库
            {
                wid = int.Parse(WarehouseID);
            }
            if (string.IsNullOrEmpty(GoodsCode) || wid == 0)
            {
                res.s = -1;
                res.d = "参数异常";
            }
            else
            {
                int CoID = int.Parse(GetCoid());
                string UserName = GetUname();
                res = InventoryHaddle.GetInvSafeQtyLst(GoodsCode, wid, CoID, UserName);
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 商品库存查询 - 修改安全库存 - 单笔(分仓)
        [HttpPostAttribute("Core/XyCore/Inventory/UptInvSafeQty")]
        public ResponseResult UptInvSafeQty([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            var inv = Newtonsoft.Json.JsonConvert.DeserializeObject<InventParams>(obj.ToString());
            inv.CoID = GetCoid();
            inv.Modifier = GetUname();
            inv.ModifyDate = DateTime.Now.ToString();
            res = InventoryHaddle.UptSafeQty(inv);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 商品库存查询 - 修改安全库存 - 单笔(主仓) 
        [HttpPostAttribute("Core/XyCore/Inventory/UptInvMainSafeQty")]
        public ResponseResult UptInvMainSafeQty([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            var inv = Newtonsoft.Json.JsonConvert.DeserializeObject<InventParams>(obj.ToString());
            if (inv.Type <= 0)
            {
                res.s = -1;
                res.d = "参数异常";
            }
            else
            {
                inv.CoID = GetCoid();
                inv.Modifier = GetUname();
                inv.ModifyDate = DateTime.Now.ToString();
                res = InventoryHaddle.UptMainSafeQty(inv);
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 商品库存查询 - 修改安全库存 - 多笔(主仓) 
        [HttpPostAttribute("Core/XyCore/Inventory/UptInvMainLstSafeQty")]
        public ResponseResult UptInvMainLstSafeQty([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            var InvLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<InventParams>>(obj["InvLst"].ToString());
            string CoID = GetCoid();
            string UserName = GetUname();
            res = InventoryHaddle.UptMainLstSafeQty(InvLst, CoID, UserName);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 商品库存查询 - 清空安全库存 - 所有商品 (主仓)        
        [HttpPostAttribute("Core/XyCore/Inventory/ClearInvMainSafeQty")]
        public ResponseResult ClearInvMainSafeQty([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            string CoID = GetCoid();
            string UserName = GetUname();
            res = InventoryHaddle.ClearSafeQty(CoID, UserName);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 商品库存查询 - 修改虚拟库存 - 单笔
        [HttpPostAttribute("Core/XyCore/Inventory/UptInvMainVirtualQty")]
        public ResponseResult UptInvMainVirtualQty([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            var inv = Newtonsoft.Json.JsonConvert.DeserializeObject<InventParams>(obj.ToString());
            if (inv.VirtualQty < 0)
            {
                res.s = -1;
                res.d = "参数异常";
            }
            else
            {
                inv.CoID = GetCoid();
                inv.Modifier = GetUname();
                inv.ModifyDate = DateTime.Now.ToString();
                res = InventoryHaddle.UptVirtualQty(inv);
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 商品库存查询 - 修改虚拟库存 - 多笔
        [HttpPostAttribute("Core/XyCore/Inventory/UptInvMainLstVirtualQty")]
        public ResponseResult UptInvMainLstVirtualQty([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            if (obj["InvLst"] == null)
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var InvLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<InventParams>>(obj["InvLst"].ToString());
                string CoID = GetCoid();
                string UserName = GetUname();
                res = InventoryHaddle.UptLstVirtualQty(InvLst, CoID, UserName);
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }

        #endregion





        #region 商品库存查询 - 更新商品名称--【停用，商品名称直接根据skuautoid获取，不需要更新】
        [HttpPostAttribute("Core/XyCore/Inventory/UpdateInvSkuName")]
        public ResponseResult UpdateInvSkuName()
        {
            int CoID = int.Parse(GetCoid());
            string UserName = GetUname();
            var res = InventoryHaddle.UptInvSkuName(CoID, UserName);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion         

        #region 商品库存盘点----測試
        [HttpPostAttribute("Core/XyCore/Inventory/SetInvQty")]
        public ResponseResult SetInvQty()
        {
            var InvLst = new List<SetInvQtyExcel>();
            var inv = new SetInvQtyExcel();
            inv.WarehouseID = 30;
            inv.WarehouseName = "dss";
            inv.SkuID = "N9L5F58781052165";
            inv.Name = "南极人男士羊毛衫";
            inv.Norm = "深红;165";
            inv.StockQty = 935;
            inv.SetQty = 1000;
            InvLst.Add(inv);
            InvLst.Add(inv);
            // var inv1 = new SetInvQtyExcel();
            // inv1.WarehouseID = 30;
            // inv1.WarehouseName = "dss";
            // inv1.SkuID = "N9L5F58781051170";
            // inv1.Name = "南极人男士羊毛衫";
            // inv1.Norm = "兰黑;170";
            // inv1.StockQty=936;
            // inv1.SetQty = 900;
            // InvLst.Add(inv1);
            var inv2 = new SetInvQtyExcel();
            inv2.WarehouseID = 30;
            inv2.WarehouseName = "dss";
            inv2.SkuID = "N9L5F58781025165";
            inv2.Name = "南极人男士羊毛衫";
            inv2.Norm = "墨绿;165";
            inv2.StockQty = 10459;
            inv2.SetQty = 900;
            InvLst.Add(inv2);
            // var inv3 = new SetInvQtyExcel();
            // inv3.WarehouseID = 30;
            // inv3.WarehouseName = "dss";
            // inv3.SkuID = "N9L5F58781025170";
            // inv3.Name = "南极人男士羊毛衫";
            // inv3.Norm = "墨绿;170";
            // inv3.StockQty=937;
            // inv3.SetQty = 1000;
            // InvLst.Add(inv3);
            int CoID = int.Parse(GetCoid());
            string UserName = GetUname();
            var res = InventoryHaddle.CreateInvSetTemp(InvLst, CoID, UserName);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion   

        #region  商品期初库存----測試
        [HttpPostAttribute("Core/XyCore/Inventory/InvInit")]
        public ResponseResult InvInit()
        {
            var InvLst = new List<InitInvQtyExcel>();
            var inv = new InitInvQtyExcel();
            inv.SkuID = "N9L5F58781052165";
            inv.SkuName = "南极人男士羊毛衫";
            inv.ColorName = "深红";
            inv.SizeName = "165";
            inv.StockQty = 1000;
            InvLst.Add(inv);
            var inv1 = new InitInvQtyExcel();
            inv1.SkuID = "N9L5F58781051170";
            inv1.SkuName = "南极人男士羊毛衫";
            inv1.ColorName = "兰黑";
            inv1.SizeName = "170";
            inv1.StockQty = 1000;
            InvLst.Add(inv1);
            var inv2 = new InitInvQtyExcel();
            inv2.SkuID = "N9L5F58781025165";
            inv2.SkuName = "南极人男士羊毛衫";
            inv2.ColorName = "墨绿";
            inv2.SizeName = "165";
            inv2.StockQty = 1000;
            InvLst.Add(inv2);

            string RblType = "0";
            int WarehouseID = 100;
            string WarehouseName = "fdsa存储仓B";
            int CoID = int.Parse(GetCoid());
            string UserName = GetUname();
            var res = InventoryHaddle.CreateInvInitTemp(RblType, InvLst, WarehouseID, WarehouseName, CoID, UserName);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion
    }
}