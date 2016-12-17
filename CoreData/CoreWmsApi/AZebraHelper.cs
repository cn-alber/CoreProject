
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Web;
using System.Text;
using System.IO;
using System.Diagnostics;
// using System.Drawing;
// using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Data;
using Dapper;
using MySql.Data.MySqlClient;


namespace CoreData.CoreWmsApi
{
    public static class AZebraHelper
    {
        /// <summary>
        /// 指令开始命令
        /// </summary>
        /// <returns></returns>
        public static string ZPL_Start()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("^XA");//指令块的开始
            builder.AppendLine("^MD30");//MD是设置色带颜色的深度
            return builder.ToString();
        }
        /// <summary>
        /// 指令结束命令
        /// </summary>
        /// <returns></returns>
        public static string ZPL_End()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("^XZ");//指令块的结束
            return builder.ToString();
        }

        /// <summary>  
        ///  设置打印标签纸边距  
        /// </summary>  
        /// <param name="printX">标签纸边距x坐标</param>  
        /// <param name="printY">标签纸边距y坐标</param>  
        /// <returns></returns>  
        public static string ZPL_PageSet(int printX, int printY)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("^LH" + printX + "," + printY);//定义条码纸边距
            return builder.ToString();
        }

        /// <summary>  
        /// 打印凭条设置  
        /// </summary>  
        /// <param name="width">凭条宽度</param>  
        /// <param name="height">凭条高度</param>  
        /// <returns>返回ZPL命令</returns>  
        public static string ZPL_SetLabel(int width, int height)
        {
            //ZPL条码设置命令：^PW640^LL480  
            string sReturn = "^PW{0}^LL{1}";
            return string.Format(sReturn, width, height);
        }

        /// <summary>  
        ///  打印矩形  
        /// </summary>  
        /// <param name="px">起点X坐标</param>  
        /// <param name="py">起点Y坐标</param>  
        /// <param name="thickness">边框宽度</param>  
        /// <param name="width">矩形宽度，0表示打印一条竖线</param>  
        /// <param name="height">矩形高度，0表示打印一条横线</param>  
        /// <returns>返回ZPL命令</returns>  
        public static string ZPL_DrawRectangle(int px, int py, int width, int height, int thickness)
        {
            //ZPL矩形命令：^FO50,50^GB300,200,2^FS  
            string sReturn = "^FO{0},{1}^GB{2},{3},{4}^FS";
            return string.Format(sReturn, px, py, width, height, thickness);
        }

        /// <summary>  
        /// 打印英文  
        /// </summary>  
        /// <param name="EnText">待打印文本</param>  
        /// <param name="ZebraFont">打印机字体 A-Z</param>  
        /// <param name="px">起点X坐标</param>  
        /// <param name="py">起点Y坐标</param>  
        /// <param name="Orient">旋转角度N = normal，R = rotated 90 degrees (clockwise)，I = inverted 180 degrees，B = read from bottom up, 270 degrees</param>  
        /// <param name="Height">字体高度</param>  
        /// <param name="Width">字体宽度</param>  
        /// <returns>返回ZPL命令</returns>  
        public static string ZPL_DrawENText(string EnText, string ZebraFont, int px, int py, string Orient, int Height, int Width)
        {
            //ZPL打印英文命令：^FO50,50^A0N,32,25^FDZEBRA^FS
            string sReturn = "^FO{1},{2}^A" + ZebraFont + "{3},{4},{5}^FD{0}^FS";
            return string.Format(sReturn, EnText, px, py, Orient, Height, Width);
        }

        /// <summary>  
        /// 中文处理,返回ZPL命令  
        /// </summary>  
        /// <param name="ChineseText">待转变中文内容</param>  
        /// <param name="FontName">字体名称</param>  
        /// <param name="startX">X坐标</param>  
        /// <param name="startY">Y坐标</param>  
        /// <param name="Orient">旋转角度0,90,180,270</param>  
        /// <param name="Height">字体高度</param>  
        /// <param name="Width">字体宽度，通常是0</param>  
        /// <param name="IsBold">1 变粗,0 正常</param>  
        /// <param name="IsItalic">1 斜体,0 正常</param>  
        /// <returns></returns>  
        public static string ZPL_DrawCHText(string ChineseText, string FontName, int startX, int startY, int Orient, int Height, int Width, int IsBold, int IsItalic)
        {
            StringBuilder sResult = new StringBuilder();
            StringBuilder hexbuf = new StringBuilder(4 * 1024);
            int count = GETFONTHEX(ChineseText, FontName, "Temp1", Orient, Height, Width, IsBold, IsItalic, hexbuf);
            if (count > 0)
            {
                string sEnd = "^FO" + startX.ToString() + "," + startY.ToString() + "^XGTemp1" + ",1,1^FS ";
                sResult.AppendLine(hexbuf.ToString() + sEnd);
                //sResult.AppendLine(hexbuf.ToString().Replace("OUTSTR01", "OUTSTR") + sEnd);
            }
            return sResult.ToString();
        }
        /// <summary>
        /// 需回车换行处理的中文文本
        /// </summary>
        /// <param name="ChineseText">待转变中文内容</param>
        /// <param name="FontName">字体名称</param>
        /// <param name="startX">X坐标</param>
        /// <param name="startY">Y坐标</param>
        /// <param name="Orient">旋转角度0,90,180,270</param>
        /// <param name="Height">字体高度</param>
        /// <param name="Width">字体宽度，通常是0</param>
        /// <param name="IsBold">1 变粗,0 正常</param>
        /// <param name="IsItalic">1 斜体,0 正常</param>
        /// <param name="TempHeight">Y坐标间隔高度</param>
        /// <param name="nums">限定字符长度</param>
        /// <returns></returns>
        public static string ZPL_ConvertCHText(string ChineseText, string FontName, int startX, ref int startY, int Orient, int Height, int Width, int IsBold, int IsItalic, int TempHeight, int nums)
        {
            string CHStr = string.Empty;
            if (ChineseText.Length > nums)
            {
                int Len = ChineseText.Length;
                string Text = ChineseText.ToString();
                while (Len > 0)
                {
                    int spot = 0;
                    if (Len > nums)
                    {
                        spot = nums;
                        Len = Len - nums;
                    }
                    else
                    {
                        spot = Len;
                        Len = 0;
                    }
                    CHStr = CHStr + ZPL_DrawCHText(Text.Substring(0, spot), FontName, startX, startY, Orient, Height, Width, IsBold, IsItalic);
                    Text = Text.Substring(spot, Text.Length - spot);
                    startY = startY + TempHeight;
                }
            }
            else
            {
                CHStr = ZPL_DrawCHText(ChineseText, FontName, startX, startY, Orient, Height, Width, IsBold, IsItalic);
                startY = startY + TempHeight;
            }
            return CHStr;
        }


        //public string ZPL_DrawImage()
        //{


        //}

        /// <summary>  
        /// 打印条形码（128码）  
        /// </summary>  
        /// <param name="px">起点X坐标</param>  
        /// <param name="py">起点Y坐标</param>  
        /// <param name="width">基本单元宽度 1-10</param>  
        /// <param name="ratio">宽窄比 2.0-3.0 增量0.1</param>  
        /// <param name="barheight">条码高度</param>  
        /// <param name="barcode">条码内容</param>  
        /// <returns>返回ZPL命令</returns>  
        public static string ZPL_DrawBarcode(int px, int py, int width, int ratio, int barheight, string Printbar, string barcode)
        {
            //ZPL打印英文命令：^FO50,260^BY1,2^BCN,100,Y,N^FDSMJH2000544610^FS  
            string sReturn = "^FO{0},{1}^BY{2},{3}^BCN,{4}," + Printbar + ",N^FD{5}^FS";
            return string.Format(sReturn, px, py, width, ratio, barheight, barcode);
        }
        /// <summary>
        /// 打印二维条码
        /// </summary>
        /// <param name="Orient">旋转角度N = normal，R = rotated 90 degrees (clockwise)，I = inverted 180 degrees，B = read from bottom up</param>
        /// <param name="ratio">放大系数1=150dpi,2=200dpi,3=300dpi,6=600dpi</param>
        /// <returns></returns>

        public static string ZPL_DrawQRcode(int px, int py, string Orient, int ratio, string QRText)
        {
            string sReturn = "^F0{0},{1}^B0{2},{3},N,0,N,1,0^FD{4}^FS";
            return string.Format(sReturn, px, py, Orient, ratio, QRText);

        }

        ///// <summary>  
        ///// 中文处理  
        ///// </summary>  
        ///// <param name="ChineseText">待转变中文内容</param>  
        ///// <param name="FontName">字体名称</param>  
        ///// <param name="Orient">旋转角度0,90,180,270</param>  
        ///// <param name="Height">字体高度</param>  
        ///// <param name="Width">字体宽度，通常是0</param>  
        ///// <param name="IsBold">1 变粗,0 正常</param>  
        ///// <param name="IsItalic">1 斜体,0 正常</param>  
        ///// <param name="ReturnPicData">返回的图片字符</param>  
        ///// <returns></returns>  
        //[DllImport(@"Fnthex32.dll",CharSet = CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
        //public static extern int GETFONTHEX(
        //    string ChineseText,
        //    string FontName,
        //    int Orient,
        //    int Height,
        //    int Width,
        //    int IsBold,
        //    int IsItalic,
        //    StringBuilder ReturnPicData          
        //    );

        /// <summary>  
        /// 中文处理  
        /// </summary>  
        /// <param name = "ChineseText" > 待转变中文内容 </ param >
        /// < param name="FontName">字体名称</param>  
        /// <param name = "FileName" > 返回的图片字符重命 </ param >
        /// < param name="Orient">旋转角度0,90,180,270</param>  
        /// <param name = "Height" > 字体高度 </ param >
        /// < param name="Width">字体宽度，通常是0</param>  
        /// <param name = "IsBold" > 1 变粗,0 正常</param>  
        /// <param name = "IsItalic" > 1 斜体,0 正常</param>  
        /// <param name = "ReturnPicData" > 返回的图片字符 </ param >
        /// < returns ></ returns >
        [DllImport("Fnthex32.dll")]
        public static extern int GETFONTHEX(
                              string ChineseText,
                              string FontName,
                              string FileName,
                              int Orient,
                              int Height,
                              int Width,
                              int IsBold,
                              int IsItalic,
                              StringBuilder ReturnBarcodeCMD);


        #region 扩展  
        /// <summary>  
        /// 毫米转像素 取整  
        /// </summary>  
        /// <param name="mm">毫米</param>  
        /// <param name="dpi">打印DPI 如300</param>  
        /// <returns></returns>  
        public static double mm2px(double mm, double dpi)
        {
            double px = (mm / 25.4) * dpi;
            return Math.Round(px, 0, MidpointRounding.AwayFromZero);
        }

        /// <summary>  
        /// 像素转毫米 取整  
        /// </summary>  
        /// <param name="px">像素</param>  
        /// <param name="dpi">打印DPI 如300</param>  
        /// <returns></returns>  
        public static double px2mm(double px, double dpi)
        {
            //像素转换成毫米公式：(宽度像素/水平DPI)*25.4;  
            double mm = (px / dpi) * 25.4;
            return Math.Round(mm, 0, MidpointRounding.AwayFromZero);
        }

        /// <summary>  
        /// 生成zpl命令 且执行  
        /// </summary>  
        /// <param name="path">zpl文件路径</param>  
        /// <param name="zpl">zpl命令</param>  
        public static void CmdDos(string path, string zpl)
        {
            // FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            // StreamWriter sw = new StreamWriter(fs, Encoding.Default);//ANSI编码格式    
            // if (File.Exists(path))
            // {
            //     sw.Write(zpl);
            //     sw.Flush();
            //     sw.Dispose();
            // }
            // fs.Dispose();
            // RunCmd("print /d:COM1 " + path + " ");
        }

        /// <summary>  
        /// 运行Dos命令  
        /// </summary>  
        /// <param name="command"></param>  
        /// <returns></returns>  
        private static bool RunCmd(string command)
        {
            //实例一个Process类，启动一个独立进程    
            Process p = new Process();
            //Process类有一个StartInfo属性，這個是ProcessStartInfo类，包括了一些属性和方法，下面我們用到了他的几个属性：    
            p.StartInfo.FileName = "cmd.exe";//设定程序名    
            p.StartInfo.Arguments = "/c " + command;//设定程式执行参数    
            p.StartInfo.UseShellExecute = false;//关闭Shell的使用    
            p.StartInfo.RedirectStandardInput = true;//重定向标准输入    
            p.StartInfo.RedirectStandardOutput = true;//重定向标准输出    
            p.StartInfo.RedirectStandardError = true;//重定向错误输出    
            p.StartInfo.CreateNoWindow = true;//设置不显示窗口    
            //p.StandardInput.WriteLine(command);//也可以用这种方式输入要执行的命令    
            //p.StandardInput.WriteLine("exit");//不过要记得加上Exit要不然下一行程式执行的時候会当机    
            try
            {
                p.Start();//开始进程    
                return true;
            }
            catch
            {
            }
            finally
            {
                if (p != null)
                    p.Dispose();
            }
            return false;
        }
        #endregion


        /* 
         *  打印中文先引用Fnthex32.dll 
            dll控件常规安装方法（仅供参考）： 
            下面是32系统的注册bat文件 
            可将下面的代码保存为“注册.bat“，放到dll目录，就会自动完成dll注册(win98不支持)。 
            @echo 开始注册 
            copy Fnthex32.dll %windir%\system32\ 
            regsvr32 %windir%\system32\Fnthex32.dll /s 
            @echo Fnthex32.dll注册成功 
            @pause 
 
            下面是64系统的注册bat文件 
            @echo 开始注册 
            copy Fnthex32.dll %windir%\SysWOW64\ 
            regsvr32 %windir%\SysWOW64\Fnthex32.dll /s 
            @echo Fnthex32.dll注册成功 
            @pause 
         *  
         *  
            ZebraHelper zh = new ZebraHelper(); 
            StringBuilder builder = new StringBuilder();             
            builder.AppendLine(zh.ZPL_Start()); 
            builder.AppendLine(zh.ZPL_PageSet(40, 80)); 
            builder.AppendLine(zh.ZPL_DrawCHText("上善若水 厚德载物", "宋体", 40, 40, 0, 32, 0, 1, 0)); 
            builder.AppendLine(zh.ZPL_DrawBarcode(40, 150, 3, 2, 40, "111112222233333")); 
            builder.AppendLine(zh.ZPL_DrawENText("111112222233333", "A", 30, 205, "N", 30, 50)); 
            builder.AppendLine(zh.ZPL_DrawRectangle(20,20,2,700,700)); 
            builder.AppendLine(zh.ZPL_End()); 
            string a = builder.ToString(); 
            //打印 
            zh.CmdDos("c:\\c.txt", a);          
         */
    }
}