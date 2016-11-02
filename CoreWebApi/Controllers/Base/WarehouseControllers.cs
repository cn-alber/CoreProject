using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
// using Microsoft.AspNetCore.Authorization;
using CoreModels.XyComm;
using CoreData.CoreComm;
using CoreModels.XyUser;
using System;
using CoreModels;
using CoreData.CoreUser;
using CoreData.CoreCore;
using CoreModels.XyCore;

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
    

        [HttpPostAttribute("/Core/Warehouse/serviceCodeRebuild")]
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
        public ResponseResult storageLst(string[] contains ,string[] status)
        {   
            string CoID = GetCoid();
            var data = WarehouseHaddle.storageLst(CoID,contains,status);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }


        [HttpPostAttribute("/Core/Warehouse/openOtherWare")]
        public ResponseResult openOtherWare([FromBodyAttribute]JObject co) //
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
                user.Name = owr.wareadmin;
                user.PassWord =GetMD5(owr.pwd, "Xy@.");
                var res = CompanyHaddle.IsComExist(owr.warename);
                if (bool.Parse(res.d.ToString()) == true)       //判断公司名称是否存在
                {
                    return CoreResult.NewResponse(-1, "仓库名已存在", "General"); 
                }
                var hasUser = UserHaddle.ExistUser(user.Account, 0, int.Parse(CoID));
                if(hasUser.s != 1){                             //判断用户是否存在
                    return CoreResult.NewResponse(hasUser.s, hasUser.d, "General"); 
                }
                var com = new Company();                
                com.name = owr.warename;
                var comRes = CompanyHaddle.InsertCompany(com,uname,int.Parse(CoID),user);                  
                data = WarehouseHaddle.openOtherWare(Convert.ToInt16(comRes.d),CoID);
            }                                    
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Warehouse/editRemark")]
        public ResponseResult editRemark([FromBodyAttribute]JObject co)
        {   
            var data = new DataResult(1,null);
            var m = Newtonsoft.Json.JsonConvert.DeserializeObject<editRemarkRequest>(co.ToString());
            if(string.IsNullOrEmpty(m.id)){
                data.s = -3008;
            }else{
                string CoID = GetCoid();
                string uname = GetUname();
                data = WarehouseHaddle.editRemark(CoID,uname,m.id,m.remark);
            }
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Warehouse/passThird")]
        public ResponseResult passThird([FromBodyAttribute]JObject co)
        {   
            var data = new DataResult(1,null);
            var m = Newtonsoft.Json.JsonConvert.DeserializeObject<editRemarkRequest>(co.ToString());
            if(string.IsNullOrEmpty(m.id)){
                data.s = -3008;
            }else{
                string CoID = GetCoid();
                string uname = GetUname();
                data = WarehouseHaddle.passThird(m.id, CoID, uname);
            }        
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Warehouse/wareCancle")]
        public ResponseResult wareCancle([FromBodyAttribute]JObject co)
        {   
            var data = new DataResult(1,null);
            var m = Newtonsoft.Json.JsonConvert.DeserializeObject<editRemarkRequest>(co.ToString());
            if(string.IsNullOrEmpty(m.id)){
                data.s = -3008;
            }else{
                string CoID = GetCoid();
                string uname = GetUname();
                data = WarehouseHaddle.passThird(m.id, CoID, uname);
            }        
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Warehouse/wareGiveUp")]
        public ResponseResult wareGiveUp([FromBodyAttribute]JObject co)
        {   
            var data = new DataResult(1,null);
            var m = Newtonsoft.Json.JsonConvert.DeserializeObject<editRemarkRequest>(co.ToString());
            if(string.IsNullOrEmpty(m.id)){
                data.s = -3008;
            }else{
                string CoID = GetCoid();
                string uname = GetUname();
                data = WarehouseHaddle.wareGiveUp(m.id, CoID, uname);
            }        
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Warehouse/editploy")]
        public ResponseResult editploy(string id="")
        {   
            string CoID = GetCoid();
            var data = WarehouseHaddle.editploy(CoID,id);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Warehouse/createploy")]
        public ResponseResult createploy([FromBodyAttribute]JObject co)
        {   
            WarePloy w = Newtonsoft.Json.JsonConvert.DeserializeObject<WarePloy>(co.ToString());
            string CoID = GetCoid();
            var data = WarehouseHaddle.createploy(CoID,w);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Warehouse/getWareSku")]
        public ResponseResult getWareSku(string GoodsCode="",string GoodsName="", string Filter="", int PageIndex= 1, int PageSize = 20, string SortField="", string SortDirection="")
        {   
            var param = new CoreSkuParam();
            param.GoodsCode = GoodsCode;
            param.GoodsName = GoodsName;
            param.Filter = Filter;
            param.PageIndex = PageIndex;
            param.PageSize = PageSize;
            param.SortField = SortField;
            param.SortDirection = SortDirection;
            string CoID = GetCoid();
            var data = CoreSkuHaddle.getWareSku(param,CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Warehouse/getWareGoods")]
        public ResponseResult getWareGoods(string GoodsCode="",string GoodsName="", string Filter="", int PageIndex= 1, int PageSize = 20, string SortField="", string SortDirection="")
        {   
            var param = new CoreSkuParam();
            param.GoodsCode = GoodsCode;
            param.GoodsName = GoodsName;
            param.Filter = Filter;
            param.PageIndex = PageIndex;
            param.PageSize = PageSize;
            param.SortField = SortField;
            param.SortDirection = SortDirection;
            string CoID = GetCoid();
            var data = CoreSkuHaddle.getWareGoods(param,CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }





    }
}