using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreData.CoreApi;
using CoreModels;
using CoreModels.XyApi.Tmall;
using CoreModels.XyCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CoreWebApi.Api.Tmall{   
    [AllowAnonymous]
    public class TmItemControllers : ControllBase{

        #region  获取店铺在售商品
        [HttpGetAttribute("/core/Api/TmItem/onsaleGet")]
        public ResponseResult onsaleGet(string page,string pageSize,string start_modified="",string end_modified=""){
            var m = new DataResult(1,null);    
            var coid = GetCoid();
            var uname = GetUname();
            m = TmallItemHaddle.onsaleGet(page,pageSize,start_modified,end_modified);
            dynamic items = m.d as dynamic;
            var tasks = new Task[10];
            int i= 0;
            TmallSku sku = new TmallSku();
            foreach (var item in items)
            {
                sku.CoID = int.Parse(coid);
                sku.Creator = uname;
                
                
                Console.WriteLine(item.outer_id);
            }


            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion


    }
}