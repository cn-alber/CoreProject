using CoreData.CoreApi;
using CoreModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi.Api.Tmall{   
    [AllowAnonymous]
    public class TmCaiNiaoControllers : ControllBase{
        
        #region
        [HttpGetAttribute("/core/Api/TmCaiNiao/cntmsMailnoGet")]
        public ResponseResult cntmsMailnoGet(){
            var m = new DataResult(1,null);
            
            m = CaiNiaoHaddle.cntmsMailnoGet();
            
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion
        
    }

}