using System.Collections.Generic;
using CoreData.CoreApi;
using CoreModels;
using CoreModels.XyApi.Tmall;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CoreWebApi.Api.Tmall{   
    [AllowAnonymous]
    public class TmCaiNiaoControllers : ControllBase{
        
        #region  菜鸟电子面单的云打印申请电子面单号
        [HttpPostAttribute("/core/Api/TmCaiNiao/WaybillIIGet")]
        public ResponseResult WaybillIIGet([FromBodyAttribute]JObject obj){
            var m = new DataResult(1,null);
            var waybill = Newtonsoft.Json.JsonConvert.DeserializeObject<WaybillCloudPrintApplyNewRequest>(obj.ToString());
            m = CaiNiaoHaddle.WaybillIIGet(waybill);            
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region 通过面单号查询电子面单信息
        [HttpPostAttribute("/core/Api/TmCaiNiao/waybillIIQueryByCode")]
        public ResponseResult waybillIIQueryByCode([FromBodyAttribute]JObject obj){
            var m = new DataResult(1,null);            
            var waybill = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WaybillDetailQueryByWaybillCodeRequest>>(obj.ToString());
            m = CaiNiaoHaddle.waybillIIQueryByCode(waybill);            
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        
    }

}