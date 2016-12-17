using Microsoft.AspNetCore.Mvc;
using CoreDate.CoreComm;
using CoreModels.XyComm;
using System;
using Newtonsoft.Json.Linq;
using CoreModels;


//  print_sys_types   系统预设模板
//  print_syses         根据系统预设模板(print_sys_types.type) 生成的 “系统模板”
//  print_use

namespace CoreWebApi.Print
{
    /// <summary>
	/// 打印模块 - 系统模块相关 
	/// </summary>    
    public class PrintDataController : ControllBase
    {
        #region 销售单
        [HttpGetAttribute("/core/print/data/saleForm")]
        public ResponseResult saleOutForm(string ID,string OID)
        {
            int x= 0; 
            int oid=0;
            int id=0;
            var m = new DataResult(1,null);
            if (int.TryParse(OID, out x))
            {
                oid = int.Parse(OID);
            }
            else
            {
                m.s = -1;
                m.d = "参数无效!";              
            }
            if (int.TryParse(ID, out x))
            {
                id = int.Parse(ID);
            }
            else
            {
                m.s = -1;
                m.d = "参数无效!";              
            }
            m = PrintDataHaddle.getSaleForm(id,oid,GetCoid());
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

        
        



    }


}