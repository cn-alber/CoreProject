using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
// using Microsoft.AspNetCore.Authorization;
using CoreModels.XyComm;
using CoreData.CoreComm;
using CoreModels.XyUser;
using System;
using CoreModels;

namespace CoreWebApi
{
    // [AllowAnonymous]
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
            var wh = Newtonsoft.Json.JsonConvert.DeserializeObject<WarehouseInsert>(co.ToString());            
            string UserName = GetUname(); 
            int CoID = int.Parse(GetCoid());
            var data = WarehouseHaddle.UpdateWarehouse(wh,UserName,CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Warehouse/GetWarehouseList")]
        public ResponseResult GetWarehouseList()
        {   
            int CoID = int.Parse(GetCoid());
            var data = WarehouseHaddle.GetWarehouseList(CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Warehouse/UpdateEnable")]
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
        
        [HttpGetAttribute("/Core/Warehouse/serviceCode")]
        public ResponseResult serviceCode(string cname)
        {   
            string CoID = GetCoid();
            var data = WarehouseHaddle.serviceCode(CoID,cname);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Warehouse/serviceCodeRebuild")]
        public ResponseResult serviceCodeRebuild()
        {   
            string CoID = GetCoid();
            var data = WarehouseHaddle.serviceCodeRebuild(CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Warehouse/askFor")]
        public ResponseResult askFor([FromBodyAttribute]JObject co)
        {   
            string code = co["code"]!=null?co["code"].ToString():"0";
            string otherRemark = co["otherRemark"]!=null?co["otherRemark"].ToString():"";            
            string CoID = GetCoid();
            var data = WarehouseHaddle.askFor(CoID,code,otherRemark);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Warehouse/Lst")]
        public ResponseResult storageLst()
        {   
            string CoID = GetCoid();
            var data = WarehouseHaddle.storageLst(CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Warehouse/selfList")]
        public ResponseResult selfList()
        {   
            string CoID = GetCoid();
            var data = WarehouseHaddle.selfList(CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Warehouse/openOtherWare")]
        public ResponseResult openOtherWare([FromBodyAttribute]JObject co)
        {   
            var data = new DataResult(1,null);
            var owr = Newtonsoft.Json.JsonConvert.DeserializeObject<openWareRequset>(co.ToString());
            if(string.IsNullOrEmpty(owr.pwd)|| owr.pwd.Length<6||owr.pwd.Length>18){
                data.s = -3107;
            }else if(string.IsNullOrEmpty(owr.username)){
                data.s = -3108;
            }else if(string.IsNullOrEmpty(owr.warename)||string.IsNullOrEmpty(owr.wareadmin)){
                data.s = -3110;
            }else{
                var uname = GetUname();
                string CoID = GetCoid();                
                var user = new UserEdit();
                user.Enable = true;
                user.Account = owr.username;
                user.PassWord =GetMD5(owr.pwd, "Xy@.");   
                data = WarehouseHaddle.openOtherWare(user,Convert.ToInt16(CoID),uname,owr.warename,owr.wareadmin);
            }        
                            
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Warehouse/editRemark")]
        public ResponseResult editRemark(string id,int type,string remark)
        {   
            string CoID = GetCoid();
            string uname = GetUname();
            var data = WarehouseHaddle.editRemark(CoID,uname,id,type,remark);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }





    }
}