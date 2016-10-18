using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using CoreModels.XyComm;
using CoreData.CoreComm;
namespace CoreWebApi
{
    [AllowAnonymous]
    public class WarehouseController : ControllBase
    {
        [HttpPostAttribute("/Core/Warehouse/InsertWarehouse")]
        public ResponseResult InsertWarehouse([FromBodyAttribute]JObject co)
        {   
            var wh = Newtonsoft.Json.JsonConvert.DeserializeObject<WarehouseInsert>(co["Warehouse"].ToString());
            string UserName = GetUname(); 
            string Company = co["Company"].ToString();
            int CoID = int.Parse(GetCoid());
            var data = WarehouseHaddle.InsertWarehouse(wh,UserName,Company,CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Warehouse/UpdateWarehouse")]
        public ResponseResult UpdateWarehouse([FromBodyAttribute]JObject co)
        {   
            var wh = Newtonsoft.Json.JsonConvert.DeserializeObject<WarehouseInsert>(co["Warehouse"].ToString());
            string UserName = GetUname(); 
            string Company = co["Company"].ToString();
            int CoID = int.Parse(GetCoid());
            var data = WarehouseHaddle.UpdateWarehouse(wh,UserName,Company,CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Warehouse/GetWarehouseList")]
        public ResponseResult GetWarehouseList()
        {   
            int CoID = int.Parse(GetCoid());
            var data = WarehouseHaddle.GetWarehouseList(CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Warehouse/UpdateWarehouseEnable")]
        public ResponseResult UpdateWarehouseEnable([FromBodyAttribute]JObject co)
        {   
            int id = int.Parse(co["ID"].ToString());
            string Company = co["Company"].ToString();
            string UserName = GetUname(); 
            bool Enable = co["Enable"].ToString().ToUpper()=="TRUE"?true:false;
            int CoID = int.Parse(GetCoid());
            var data = WarehouseHaddle.UpdateWarehouseEnable(id,Company,UserName,CoID,Enable);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
    }
}