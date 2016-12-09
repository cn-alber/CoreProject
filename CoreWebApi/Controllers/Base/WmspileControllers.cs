using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
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

    public class WmspileControllers: ControllBase{
        [HttpGetAttribute("/Core/Wmspile/getPileList")]
        public ResponseResult getPileList(int wareid,string area="",string row="",string col="",string storey="",string cell="")
        {   
            var data = new DataResult(1,null);
            string UserName = GetUname(); 
            if(wareid == 0) {
                data.s = -1;
                data.d = "仓库ID参数错误";
            } else {
                string CoID = GetCoid();
                var insertM = new PileInsert();
                data = WmspileHaddle.getPileList(CoID,wareid.ToString() ,area, row,col,storey,cell);
            }
            
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
        [HttpGetAttribute("/Core/Wmspile/InsertPile")]
        public ResponseResult InsertPile([FromBodyAttribute]JObject co)
        {   
            var insertM = Newtonsoft.Json.JsonConvert.DeserializeObject<PileInsert>(co.ToString());
            string UserName = GetUname(); 
            //string Company = co["Company"].ToString();
            string CoID = GetCoid();
            // var insertM = new PileInsert();
            // insertM.WarehouseID = 138;
            // insertM.WarehouseName = "南极云商常熟次品1仓4";
            // insertM.Type = 5;
            // insertM.area = area;
            // insertM.row = row.Split(',');
            // insertM.col = col.Split(',');
            // insertM.storey = null;
            // insertM.cell = null;
            var data = WmspileHaddle.insertpile(insertM,UserName,CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Wmspile/DeletePile")]
        public ResponseResult DeletePile([FromBodyAttribute]JObject co)
        {   
            var IDLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(co["IDLst"].ToString());
            string UserName = GetUname(); 
            //string Company = co["Company"].ToString();
            string CoID = GetCoid();
            var insertM = new PileInsert();
            var data = WmspileHaddle.deletepile(IDLst,CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }

        [HttpPostAttribute("/Core/Wmspile/pileOrder")]
        public ResponseResult pileOrder([FromBodyAttribute]JObject co)
        {   
            var editorder = Newtonsoft.Json.JsonConvert.DeserializeObject<editOrder>(co.ToString());
            string CoID = GetCoid();
            var insertM = new PileInsert();
            var data = WmspileHaddle.pileOrder(editorder,CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }


    }
}