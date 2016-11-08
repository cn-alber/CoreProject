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
using System.Collections.Generic;

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
            string uid = GetUid();
            string uname = GetUname();       
            var data = WarehouseHaddle.askFor(CoID,code,otherRemark,uid,uname);
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
                string uid = GetUid();
                data = WarehouseHaddle.editRemark(CoID,uname,m.id,m.remark,uid);
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
                string uid = GetUid();
                data = WarehouseHaddle.passThird(m.id, CoID, uname,uid);
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
                string uid = GetUid();
                data = WarehouseHaddle.wareCancle(m.id, CoID, uname,uid);
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
                string uid = GetUid();
                data = WarehouseHaddle.wareGiveUp(m.id, CoID, uname,uid);
            }        
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Warehouse/WarePloyList")]
        public ResponseResult WarePloyList(string id="")
        {   
            var data = new DataResult(1,null);
            string CoID = GetCoid();
            data.d = Newtonsoft.Json.JsonConvert.DeserializeObject<List<wareploylist>>(
                                            Newtonsoft.Json.JsonConvert.SerializeObject(WarehouseHaddle.WarePloyList(CoID)));

            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Warehouse/editploy")]
        public ResponseResult editploy(string id="")
        {   
            string CoID = GetCoid();
            var data = WarehouseHaddle.editploy(CoID,id);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Warehouse/getPloySetting")]
        public ResponseResult getPloySetting(string id="")
        {               
            string CoID = GetCoid();
            var data = WarehouseHaddle.getPloySetting(CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }


        [HttpPostAttribute("/Core/Warehouse/createploy")]
        public ResponseResult createploy([FromBodyAttribute]JObject co)
        {   
            var data = new DataResult(1,null);
            WarePloyRequest w = Newtonsoft.Json.JsonConvert.DeserializeObject<WarePloyRequest>(co.ToString());
            if(w.MinNum<0 || w.MaxNum<0){
                data.s = -3111;
            }else if(w.MaxNum<w.MinNum){
                data.s = -3112;
            }else{
                string CoID = GetCoid();
                string uname =GetUname();
                data = WarehouseHaddle.createploy(CoID,w,uname);    
            }

            
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Warehouse/modifyploy")]
        public ResponseResult modifyploy([FromBodyAttribute]JObject co)
        {   
            var data = new DataResult(1,null);
            WarePloyRequest w = Newtonsoft.Json.JsonConvert.DeserializeObject<WarePloyRequest>(co.ToString());
            if(w.MinNum<0 || w.MaxNum<0){
                data.s = -3111;
            }else if(w.MaxNum<w.MinNum){
                data.s = -3112;
            }else{
                string CoID = GetCoid();
                string uname = GetUname();
                data = WarehouseHaddle.modifyploy(CoID,w,uname);
            }
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Warehouse/chooseWare")]
        public ResponseResult chooseWare()
        {           
            List<SkuQuery> skus = new List<SkuQuery>();
            var s1 = new SkuQuery();
            var s2 = new SkuQuery();
            var s3 = new SkuQuery();
            s1.GoodsCode = "N3L4F51001";
            s2.GoodsCode = "N3L4F51002";
            s3.GoodsCode = "N3L4F51004";
            s1.SkuID = "N3L4F51001025190";
            s2.SkuID = "N3L4F51002025191";
            s3.SkuID = "N3L4F51003025193";
            skus.Add(s1);
            //skus.Add(s2);
            //skus.Add(s3);

            string CoID = GetCoid();            
            var data = WarehouseHaddle.chooseWare(CoID,"8",110000,1,1,1,skus);
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

        [HttpGetAttribute("/Core/Warehouse/wareSettingGet")]
        public ResponseResult wareSettingGet()
        {   
            var param = new CoreSkuParam();
            string CoID = GetCoid();
            var data = WarehouseHaddle.wareSettingGet(CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpGetAttribute("/Core/Warehouse/warePersonSet")]
        public ResponseResult warePersonSet()
        {               
            string CoID = GetCoid();
            var data = WarehouseHaddle.warePersonSet(CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }



        [HttpPostAttribute("/Core/Warehouse/modifyWareSetting")]
        public ResponseResult modifyWareSetting([FromBodyAttribute]JObject co)
        {   
            var w = Newtonsoft.Json.JsonConvert.DeserializeObject<ware_m_setting>(co.ToString());
            string CoID = GetCoid();
            string uname = GetUname();
            var data = WarehouseHaddle.modifyWareSetting(w,CoID,uname);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }





    }
}