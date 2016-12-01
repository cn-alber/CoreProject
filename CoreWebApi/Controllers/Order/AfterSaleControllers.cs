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
    public class AfterSaleController : ControllBase
    {
        [HttpGetAttribute("/Core/AfterSale/GetInitASData")]
        public ResponseResult GetInitASData()
        {
            var data = AfterSaleHaddle.GetInitASData(int.Parse(GetCoid()));
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/AfterSale/GetAsList")]
        public ResponseResult GetAsList(string ExCode,string SoID,string OID,string ID,string BuyerShopID,string RecName,string Modifier,string RecPhone,string RecTel,
                                        string Creator,string Remark,string DateType,string DateStart,string DateEnd,string SkuID,string IsNoOID,string IsInterfaceLoad,
                                        string IsSubmitDis,string ShopID,string Status,string GoodsStatus,string Type,string OrdType,string ShopStatus,string RefundStatus,
                                        string Distributor,string IsSubmit,string IssueType,string Result,string SortField,string SortDirection,string PageIndex,string NumPerPage)
        {   
            int x;
            long l;
            var cp = new AfterSaleParm();
            cp.CoID = int.Parse(GetCoid());
            cp.ExCode = ExCode;
            if(!string.IsNullOrEmpty(SoID))
            {
                if (long.TryParse(SoID, out l))
                {
                    cp.SoID = long.Parse(SoID);
                }
            }
            if(!string.IsNullOrEmpty(OID))
            {
                if (int.TryParse(OID, out x))
                {
                    cp.OID = int.Parse(OID);
                }
            }
            if(!string.IsNullOrEmpty(ID))
            {
                if (int.TryParse(ID, out x))
                {
                    cp.ID = int.Parse(ID);
                }
            }
            cp.Modifier = Modifier;
            cp.BuyerShopID = BuyerShopID;
            cp.Creator = Creator;
            cp.RecName = RecName;
            cp.RecPhone = RecPhone;
            cp.RecTel = RecTel;
            cp.Remark = Remark;
            if(!string.IsNullOrEmpty(DateType))
            {
                if(DateType.ToUpper() == "REGISTERDATE" || DateType.ToUpper() == "MODIFYDATE" || DateType.ToUpper() == "CONFIRMDATE")
                {
                    cp.DateType = DateType;
                }
            }
            DateTime date;
            if (DateTime.TryParse(DateStart, out date))
            {
                cp.DateStart = DateTime.Parse(DateStart);
            }
            if (DateTime.TryParse(DateEnd, out date))
            {
                cp.DateEnd = DateTime.Parse(DateEnd);
            }
            cp.SkuID = SkuID;
            cp.IsNoOID = IsNoOID;
            cp.IsInterfaceLoad = IsInterfaceLoad;
            cp.IsSubmitDis = IsSubmitDis;
            if(!string.IsNullOrEmpty(ShopID))
            {
                if (int.TryParse(ShopID, out x))
                {
                    cp.ShopID = int.Parse(ShopID);
                }
            }
            if(!string.IsNullOrEmpty(Status))
            {
                if (int.TryParse(Status, out x))
                {
                    cp.Status = int.Parse(Status);
                }
            }
            cp.GoodsStatus = GoodsStatus;
            if(!string.IsNullOrEmpty(Type))
            {
                if (int.TryParse(Type, out x))
                {
                    cp.Type = int.Parse(Type);
                }
            }
            if(!string.IsNullOrEmpty(OrdType))
            {
                if (int.TryParse(OrdType, out x))
                {
                    cp.OrdType = int.Parse(OrdType);
                }
            }
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
            if(!string.IsNullOrEmpty(RefundStatus))
            {
                string[] a = RefundStatus.Split(',');
                List<string> s = new List<string>();
                foreach(var i in a)
                {
                    s.Add(i);
                }
                cp.RefundStatus = s;
            }
            if(!string.IsNullOrEmpty(Distributor))
            {
                if (int.TryParse(Distributor, out x))
                {
                    cp.Distributor = int.Parse(Distributor);
                }
            }
            cp.IsSubmit = IsSubmit;
            if(!string.IsNullOrEmpty(IssueType))
            {
                if (int.TryParse(IssueType, out x))
                {
                    cp.IssueType = int.Parse(IssueType);
                }
            }
            if(!string.IsNullOrEmpty(Result))
            {
                if (int.TryParse(Result, out x))
                {
                    cp.Result = int.Parse(Result);
                }
            }
            if(!string.IsNullOrEmpty(SortField))
            {
                if(CommHaddle.SysColumnExists(DbBase.CoreConnectString,"aftersale",SortField).s == 1)
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
            var data = AfterSaleHaddle.GetAsList(cp);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
    }
}