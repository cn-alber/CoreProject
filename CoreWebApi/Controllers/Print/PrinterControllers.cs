using Microsoft.AspNetCore.Mvc;
using CoreDate.CoreComm;
using CoreModels.XyComm;
using System;
using Newtonsoft.Json.Linq;
using CoreModels;
using System.Collections.Generic;



namespace CoreWebApi.Print
{
    ///<summary>
    ///打印基础配置
    ///</summary>
    public class PrinterController : ControllBase
    {
        
        [HttpGetAttribute("/core/printer/list")]
        public ResponseResult list(int type=0,string filter="",string enabled="All",int pageIndex=1,int pageSize=20)
        {           
            PrinterParam param = new PrinterParam();
            param.CoID = int.Parse(GetCoid());
            param.Type = type;
            param.Filter = filter;
            param.Enabled = enabled;
            param.PageIndex = pageIndex;
            param.PageSize = pageSize;
            var m = PrinterHaddle.getPrinterLst(param);          
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }

        [HttpGetAttribute("/core/printer/getPrintQuery")]
        public ResponseResult getPrintQuery(int id)
        {           
            var m = PrinterHaddle.getPrintQuery(id, GetCoid());          
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        
        [HttpPostAttribute("/core/printer/createPrinter")]
        public ResponseResult createPrinter([FromBodyAttribute]JObject lo)
        {                       
            var printer = Newtonsoft.Json.JsonConvert.DeserializeObject<PrinterInsert>(lo.ToString());
            printer.CoID = int.Parse(GetCoid());
            switch (printer.PrintType)
            {
                case 1:
                    printer.PrintName = "箱码打印";
                    break;
                case 2:
                    printer.PrintName = "快递单打印";
                    break;
                case 3:
                    printer.PrintName = "件码打印";
                    break;
                default:
                    printer.PrintName = "箱码打印";
                    break;
            }
            var m = PrinterHaddle.creatPrinter(printer);          
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }

        [HttpPostAttribute("/core/printer/modifyPrinter")]
        public ResponseResult modifyPrinter([FromBodyAttribute]JObject lo)
        {                       
            var printer = Newtonsoft.Json.JsonConvert.DeserializeObject<PrinterInsert>(lo.ToString());
            printer.CoID = int.Parse(GetCoid());
            switch (printer.PrintType)
            {
                case 1:
                    printer.PrintName = "箱码打印";
                    break;
                case 2:
                    printer.PrintName = "快递单打印";
                    break;
                case 3:
                    printer.PrintName = "件码打印";
                    break;
                default:
                    printer.PrintName = "箱码打印";
                    break;
            }
            var m = PrinterHaddle.modifyPrinter(printer);          
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }

        [HttpPostAttribute("/core/printer/delPrinter")]
        public ResponseResult delPrinter([FromBodyAttribute]JObject lo)
        {           
            var ids = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(lo["IDLst"].ToString());
            var m = PrinterHaddle.delPrinter(ids,GetCoid());          
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }


    }


}