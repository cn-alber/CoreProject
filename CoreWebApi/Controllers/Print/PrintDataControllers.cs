using Microsoft.AspNetCore.Mvc;
using CoreDate.CoreComm;
using CoreModels.XyComm;
using System;
using Newtonsoft.Json.Linq;
using CoreModels;
using System.Collections.Generic;


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
        
        [HttpGetAttribute("/core/print/data/withType")]
        public ResponseResult withType(int withType,string ids)
        {
            var m = new DataResult(1,null);
            switch (withType)
            {
                case 1: 
                    m = PrintDataHaddle.getSaleForm(new List<string>(ids.Split(',')),GetCoid());
                    break;
                default:
                    m.s = -1;
                    m.d = "该类型暂无数据";
                    break;
            }
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        
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
            //m = PrintDataHaddle.getSaleForm(id,oid,GetCoid());
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

        #region 采购单
        [HttpGetAttribute("/core/print/data/purchaseForm")]
        public ResponseResult purchaseForm(string ID,string OID)
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
            m = PrintDataHaddle.getPurchaseForm(id,GetCoid());
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion
        



    }


}