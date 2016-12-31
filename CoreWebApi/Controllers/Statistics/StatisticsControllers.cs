using Microsoft.AspNetCore.Mvc;
using CoreDate.CoreComm;
using CoreModels.XyComm;
using System;
using Newtonsoft.Json.Linq;
using CoreData.CoreCore;


//  print_sys_types   系统预设模板
//  print_syses         根据系统预设模板(print_sys_types.type) 生成的 “系统模板”
//  print_use

namespace CoreWebApi.Statistics
{
    /// <summary>
	/// 打印模块 - 系统模块相关 
	/// </summary>    
    public class StatisticsController : ControllBase
    {
        #region 最近7天商销量
        [HttpGetAttribute("/core/Statistics/getOrderData")]
        public ResponseResult getOrderData()
        {
            var m = StatisticsHaddle.getOrderData(GetRoleid());
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

        #region 最近7天商销量
        [HttpGetAttribute("/core/Statistics/lastSales_7")]
        public ResponseResult lastsales_7()
        {
            var m = StatisticsHaddle.lastSales_7(GetCoid());
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

        #region 最近7天地域销售
        [HttpGetAttribute("/core/Statistics/lastAreaSale_7")]
        public ResponseResult lastAreaSale_7()
        {
            var m = StatisticsHaddle.lastAreaSale_7(GetCoid());
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion
        
        #region 最近7天地域销售
        [HttpGetAttribute("/core/Statistics/lastSales_15")]
        public ResponseResult lastSales_15()
        {
            var m = StatisticsHaddle.lastSales_15(GetCoid());
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion



    }


}