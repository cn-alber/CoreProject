using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
// using CoreData.CoreUser;
// using Microsoft.AspNetCore.Authorization;
using CoreModels.XyComm;
using System.Collections.Generic;
using CoreData.CoreComm;
using CoreData;
using CoreModels;
namespace CoreWebApi
{
    public class WarehouseController : ControllBase
    {
        [HttpPostAttribute("/Core/Warehouse/InsertWarehouse")]
        public ResponseResult InsertWarehouse([FromBodyAttribute]JObject co)
        {   
            var wh = Newtonsoft.Json.JsonConvert.DeserializeObject<Warehouse>(co["Warehouse"].ToString());
            string UserName = GetUname(); 
            string Company = co["Company"].ToString();
            int CoID = int.Parse(GetCoid());
            var res = WarehouseHaddle.IsWarehouseExist(wh.warehousename,CoID);
            if (bool.Parse(res.d.ToString()) == true)
            {
                return CoreResult.NewResponse(-1, "仓库已存在,不允许新增", "General"); 
            }
            var data = WarehouseHaddle.InsertWarehouse(wh,UserName,Company,CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Warehouse/UpdateWarehouse")]
        public ResponseResult UpdateWarehouse([FromBodyAttribute]JObject co)
        {   
            var wh = Newtonsoft.Json.JsonConvert.DeserializeObject<Warehouse>(co["Warehouse"].ToString());
            string UserName = GetUname(); 
            string Company = co["Company"].ToString();
            int CoID = int.Parse(GetCoid());
            var data = WarehouseHaddle.UpdateWarehouse(wh,UserName,Company,CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Warehouse/GetWarehouseSingle")]
        public ResponseResult GetWarehouseSingle(string ID)
        {   
            int x,id;
            var data = new DataResult(1,null);  
            if (int.TryParse(ID, out x))
            {
                id = int.Parse(ID);
                data = WarehouseHaddle.GetWarehouseSingle(id);
            }
            else
            {
                data.s = -1;
                data.d = "参数无效!";
            }
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Warehouse/IsWarehouseEnable")]
        public ResponseResult IsWarehouseEnable(string warehouseid)
        {   
            int x;
            var data = new DataResult(1,null);  
            if (int.TryParse(warehouseid, out x))
            {
                int CoID = int.Parse(GetCoid());
                int whid = int.Parse(warehouseid);
                data = WarehouseHaddle.IsWarehouseEnable(whid,CoID);
            }
            else
            {
                data.s = -1;
                data.d = "参数无效!";
            }
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Warehouse/GetWarehouseList")]
        public ResponseResult GetWarehouseList(string Enable,string Filter,string SortField,string SortDirection,string PageIndex,string NumPerPage)
        {   
            var cp = new WarehouseParm();
            cp.CoID = int.Parse(GetCoid());
            if(Enable.ToUpper() == "TRUE" || Enable.ToUpper() == "FALSE")
            {
                cp.Enable = Enable;
            }
            cp.Filter = Filter;
            if(CommHaddle.SysColumnExists(DbBase.CommConnectString,"warehouse",SortField).s == 1)
            {
                cp.SortField = SortField;
            }
            if(SortDirection.ToUpper() == "ASC")
            {
                cp.SortDirection = SortDirection;
            }
            int x;
            if (int.TryParse(NumPerPage, out x))
            {
                cp.NumPerPage = int.Parse(NumPerPage);
            }
            if (int.TryParse(PageIndex, out x))
            {
                cp.PageIndex = int.Parse(PageIndex);
            }
            var data = WarehouseHaddle.GetWarehouseList(cp);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Warehouse/UpdateWarehouseEnable")]
        public ResponseResult UpdateWarehouseEnable([FromBodyAttribute]JObject co)
        {   
            Dictionary<int,string> IDsDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int,string>>(co["IDsDic"].ToString());
            string Company = co["Company"].ToString();
            string UserName = GetUname(); 
            bool Enable = co["Enable"].ToString().ToUpper()=="TRUE"?true:false;
            int CoID = int.Parse(GetCoid());
            var data = WarehouseHaddle.UpdateWarehouseEnable(IDsDic,Company,UserName,CoID,Enable);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
    }
}