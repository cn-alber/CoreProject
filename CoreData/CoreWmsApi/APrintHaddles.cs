using System.Collections.Generic;
using System.Linq;
using System.Data;
using CoreModels.WmsApi;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Text;
// using System.Drawing;

namespace CoreData.CoreWmsApi
{
    public static class APrintHelper
    {
        public static int TempHeight = 0;
        #region 箱码打印
        public static string GetBigBoxPrint(List<ApiBoxPrint> boxLst)
        {
            StringBuilder builder = new StringBuilder();
            int CNHight = 24;
            int CNWight = 12;
            string CNFont = "宋体";
            builder.AppendLine(AZebraHelper.ZPL_Start());
            builder.AppendLine(AZebraHelper.ZPL_PageSet(0,0));
            builder.AppendLine(AZebraHelper.ZPL_SetLabel(600, 600));
            builder.AppendLine(AZebraHelper.ZPL_DrawBarcode(50, 20, 2, 2, 80, "Y", boxLst[0].BoxCode));//箱码
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(10, 130, 550, 1, 1));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("货号", CNFont, 10, 150, 0, CNHight, CNWight, 1, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("颜色", CNFont, 200, 150, 0, CNHight, CNWight, 1, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("尺码", CNFont, 320, 150, 0, CNHight, CNWight, 1, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("数量", CNFont, 450, 150, 0, CNHight, CNWight, 1, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(16, 190, 550, 1, 1));
            TempHeight = 190 + 32;
            foreach (var b in boxLst)
            {
                builder.AppendLine(AZebraHelper.ZPL_DrawCHText(b.SkuID, CNFont, 10, TempHeight, 0, CNHight, CNWight, 0, 0));
                builder.AppendLine(AZebraHelper.ZPL_DrawCHText(b.ColorName, CNFont, 200, TempHeight, 0, CNHight, CNWight, 0, 0));
                builder.AppendLine(AZebraHelper.ZPL_DrawCHText(b.SizeName, CNFont, 320, TempHeight, 0, CNHight, CNWight, 0, 0));
                builder.AppendLine(AZebraHelper.ZPL_DrawCHText(b.Nums.ToString(), CNFont, 450, TempHeight, 0, CNHight, CNWight, 0, 0));
                //builder.AppendLine(AZebraHelper.ZPL_DrawENText(b.Nums.ToString(), "F", 450, TempHeight, "N", 30, 30));
                TempHeight += 32;
            }
            builder.AppendLine(AZebraHelper.ZPL_End());
            return builder.ToString();
        }
        #endregion


         #region 出货单打印
        public static string GetSOutPrintEMS(ASaleOutPrint ASaleOut)
        {
            StringBuilder builder = new StringBuilder();
            int CNHight = 22;
            int CNWight = 12;
            int CNstartY = 0;
            string CNFont = "宋体";
            string picPath = string.Empty;
            // Bitmap img = null;
            if (ASaleOut.ExpName == "EMS")
                picPath = "/res/images/Express/EMSEXP.bmp";
            if (ASaleOut.ExpName == "圆通速递")
                picPath = "/res/images/Express/YTEXP.bmp";
            if (ASaleOut.ExpName == "申通E物流")
                picPath = "/res/images/Express/STEXP.bmp";
            if (!string.IsNullOrEmpty(picPath))
            {
                // img = GraphTransform.SmallPicWidth(picPath, 60);
                // if (img != null)
                // {
                //     var imgCode = GraphTransform.ConvertImageToCode(img);
                //     var t = ((img.Size.Width / 8 + ((img.Size.Width % 8 == 0) ? 0 : 1)) * img.Size.Height).ToString(); //图形中的总字节数 
                //     var w = (img.Size.Width / 8 + ((img.Size.Width % 8 == 0) ? 0 : 1)).ToString(); //每行的字节数 
                //     string zpl = string.Format("~DGR:imgName.GRF,{0},{1},{2}", t, w, imgCode); //发送给打印机 
                //     builder.AppendLine(zpl);                   
                // }
            }
            builder.AppendLine(AZebraHelper.ZPL_Start());
            builder.AppendLine(AZebraHelper.ZPL_PageSet(0, 0));
            builder.AppendLine(AZebraHelper.ZPL_SetLabel(800, 1500));
            // if (!string.IsNullOrEmpty(picPath)&& img != null)
            // {
            //     builder.AppendLine("^FO10,10^XGR:imgName.GRF,1,1^FS");
            //     builder.AppendLine("^FO10,911^XGR:imgName.GRF,1,1^FS");
            // }
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(3, 66, 780, 1, 1));//1横线
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(3, 155, 780, 1, 1));//2
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(3, 240, 780, 1, 1));//3
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(614, 290, 166, 1, 1));//3.1
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(3, 398, 614, 1, 1));//4
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(3, 601, 780, 1, 1));//5
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(3, 718, 780, 1, 1));//6
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(3, 1000, 780, 1, 1));//7
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(3, 1145, 614, 1, 1));//8
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(3, 1250, 780, 1, 1));//9
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(50, 240, 1, 360, 1));//竖线1
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(614, 240, 1, 360, 1));//竖线2
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(420, 730, 1, 150, 1));//竖线3
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(630, 730, 1, 150, 1));//竖线4
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(50, 1000, 1, 250, 1));//竖线5
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(614, 1000, 1, 250, 1));//竖线6

            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.RecLogistics, CNFont, 60, 80, 0, 65, 35, 0, 0));//省份
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.RecCity, CNFont, 322, 80, 0, 65, 35, 0, 0));//市
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("收", CNFont, 24, 306, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("件", CNFont, 24, 338, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.RecName, CNFont, 92, 258, 0, CNHight, CNWight, 0, 0));//收件人
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.RecPhone, CNFont, 252, 258, 0, CNHight, CNWight, 0, 0));//收件人电话
            string RecAddress = ASaleOut.RecLogistics + " " + ASaleOut.RecCity + " " + ASaleOut.RecDistrict + " " + ASaleOut.RecAddress +" "+ASaleOut.RecZip;
            CNstartY = 285;
            builder.AppendLine(AZebraHelper.ZPL_ConvertCHText(RecAddress, CNFont, 92,ref CNstartY, 0, CNHight, CNWight, 0, 0, 27, 18));//收件地址
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("寄", CNFont, 24, 443, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("件", CNFont, 24, 483, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.Sender, CNFont, 92, 420, 0, CNHight, CNWight, 0, 0));//寄件人
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.SendPhone, CNFont, 252, 420, 0, CNHight, CNWight, 0, 0));//寄件人电话
            string SendAddress = ASaleOut.SendLogistics + " " + ASaleOut.SendCity + " " + ASaleOut.SendDistrict + " " + ASaleOut.SendAddress;
            CNstartY = 445;
            builder.AppendLine(AZebraHelper.ZPL_ConvertCHText(SendAddress, CNFont, 92,ref CNstartY, 0, CNHight, CNWight, 0, 0, 27, 20));//寄件地址
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("服务:", CNFont, 661, 258, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("保价金额:", CNFont, 629, 314, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("保价费用:", CNFont, 629, 346, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawBarcode(100, 614, 2, 2, 60, "Y", ASaleOut.ExCode));//快递单号
            CNstartY = 728;
            builder.AppendLine(AZebraHelper.ZPL_ConvertCHText(ASaleOut.SendRemark, CNFont, 10,ref CNstartY, 0, CNHight, CNWight, 0, 0, 27, 16));//寄件备注
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("签收人", CNFont, 445, 750, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("时间", CNFont, 445, 806, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawBarcode(274, 887, 2, 2, 60, "Y", ASaleOut.ExCode));//快递单号
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("收", CNFont, 24, 1040, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("件", CNFont, 24, 1072, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.RecName, CNFont, 92, 1005, 0, CNHight, CNWight, 0, 0));//收件人
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.RecPhone, CNFont, 252, 1005, 0, CNHight, CNWight, 0, 0));//收件人电话
            CNstartY = 1037;
            builder.AppendLine(AZebraHelper.ZPL_ConvertCHText(RecAddress, CNFont, 92,ref CNstartY, 0, CNHight, CNWight, 0, 0, 27, 18));//收件地址
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("寄", CNFont, 24, 1169, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("件", CNFont, 24, 1209, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.Sender, CNFont, 92, 1157, 0, CNHight, CNWight, 0, 0));//寄件人
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.SendPhone, CNFont, 252, 1157, 0, CNHight, CNWight, 0, 0));//寄件人电话
            CNstartY = 1189;
            builder.AppendLine(AZebraHelper.ZPL_ConvertCHText(SendAddress, CNFont, 92,ref CNstartY, 0, CNHight, CNWight, 0, 0, 27, 20));//寄件地址
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(DateTime.Now.ToString("MM-dd hh:mm:ss"), CNFont, 624, 1209, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.OutSkuLst.Count.ToString() + "件.赠(" + ASaleOut.GiftQty.ToString() + ")", CNFont, 572, 1260, 0, CNHight, CNWight, 0, 0));//出货件数
            TempHeight = 1260 + 22;
            foreach (var s in ASaleOut.OutSkuLst)
            {
                builder.AppendLine(AZebraHelper.ZPL_DrawCHText(s.SkuID+" "+ s.ColorName+ s.SizeName, CNFont, 80, TempHeight, 0, CNHight, CNWight, 0, 0));
                builder.AppendLine(AZebraHelper.ZPL_DrawCHText(s.OutQty.ToString(), CNFont, 572, TempHeight, 0, CNHight, CNWight, 0, 0));
                //builder.AppendLine(AZebraHelper.ZPL_DrawENText(s.OutQty.ToString(), "0", 572, TempHeight, "N", 30, 30));
                TempHeight += 32;
            }
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("内部单号:", CNFont, 50, 1403, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.OID.ToString(), CNFont, 161, 1403, 0, CNHight, CNWight, 0, 0));
            //builder.AppendLine(AZebraHelper.ZPL_DrawENText(ASaleOut.OID.ToString(), "0", 161, 1403, "N", 20, 20));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("线上:", CNFont, 298, 1403, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.SoID.ToString(), CNFont, 362, 1403, 0, CNHight, CNWight, 0, 0));
            //builder.AppendLine(AZebraHelper.ZPL_DrawENText(ASaleOut.SoID.ToString(), "0", 362, 1403, "N", 20, 20));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("已检视", CNFont, 621, 1403, 0, CNHight, CNWight, 0, 0));

            builder.AppendLine(AZebraHelper.ZPL_End());
            return builder.ToString();
        }
        #endregion

        #region 顺丰速运打印
        public static string GetSOutPrintSF(ASaleOutPrint ASaleOut)
        {
            StringBuilder builder = new StringBuilder();
            int CNHight = 20;
            int CNWight = 12;
            string CNFont = "宋体";
            string picPath = string.Empty;
            int thickness = 2;
            int CNstartY;
            builder.AppendLine(AZebraHelper.ZPL_Start());
            builder.AppendLine(AZebraHelper.ZPL_PageSet(0, 0));
            builder.AppendLine(AZebraHelper.ZPL_SetLabel(800, 1500));
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(3, 16, 780, 1, thickness));//1横线
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(3, 64, 234, 1, thickness));//1.1
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(3, 121, 780, 1, thickness));//2
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(576, 177, 209, 1, thickness));//2.1
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(3, 250, 573, 1, thickness));//3
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(3, 403, 780, 1, thickness));//4
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(3, 442, 780, 1, thickness));//5
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(3, 476, 780, 1, thickness));//6
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(3, 508, 780, 1, thickness));//7
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(495, 580, 288, 1, thickness));//7.1
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(3, 620, 780, 1, thickness));//8
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(495, 693, 288, 1, thickness));//8.1
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(3, 726, 780, 1, thickness));//9
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(3, 766, 780, 1, thickness));//10
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(3, 968, 403, 1, thickness));//11
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(3, 1169, 780, 1, thickness));//12
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(3, 16, 1, 1153, thickness));//竖线1
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(237, 16, 1, 105, thickness));//2
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(576, 121, 1, 322, thickness));//3
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(164, 442, 1, 65, thickness));
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(330, 442, 1, 65, thickness));
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(495, 442, 1, 284, thickness));//4
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(660, 442, 1, 138, thickness));//5
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(660, 620, 1, 73, thickness));//6
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(402, 766, 1, 403, thickness));//7
            builder.AppendLine(AZebraHelper.ZPL_DrawRectangle(783, 16, 1, 1153, thickness));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("顺 丰 速 运", CNFont,11, 28, 0, 40, 18, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawBarcode(260, 28, 2, 2, 65, "Y", ASaleOut.ExCode));//快递单号
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("寄件方信息:", CNFont, 11, 135, 0, CNHight, CNWight, 0, 0));
            string SendAddress = ASaleOut.SendLogistics + " " + ASaleOut.SendCity + " " + ASaleOut.SendDistrict;                
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(SendAddress, CNFont, 11, 165, 0, CNHight, CNWight, 0, 0));
            CNstartY = 195;
            builder.AppendLine(AZebraHelper.ZPL_ConvertCHText(ASaleOut.SendAddress, CNFont, 11, ref CNstartY, 0, CNHight, CNWight, 0, 0, 30, 22));//寄件地址
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.Sender, CNFont, 11, CNstartY, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.SendPhone, CNFont, 220, CNstartY, 0, CNHight, CNWight, 0, 0));
            string RecAddress = ASaleOut.RecLogistics + " " + ASaleOut.RecCity + " " + ASaleOut.RecDistrict;
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("收件方信息:", CNFont, 11, 258, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(RecAddress, CNFont, 11, 290, 0, CNHight, CNWight, 0, 0));
            CNstartY = 320;
            builder.AppendLine(AZebraHelper.ZPL_ConvertCHText(ASaleOut.RecAddress, CNFont, 11, ref CNstartY, 0, CNHight, CNWight, 0, 0, 30, 22));//收件地址
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.RecName, CNFont, 11, CNstartY, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.RecPhone, CNFont, 220, CNstartY, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("原寄地:", CNFont, 591, 141, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("目的地:", CNFont, 591, 193, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("托寄物信息:"+ASaleOut.OutSkuLst[0].GoodsName, CNFont, 11, 411, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("件数", CNFont, 60, 447, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("实际重量", CNFont, 204, 447, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("计费重量", CNFont, 370, 447, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("费用", CNFont, 555, 447, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("费用合计", CNFont, 685, 447, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.OrdQty.ToString(), CNFont, 67, 483, 0, CNHight, CNWight, 0, 0));//键鼠
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.RealWeight.ToString(), CNFont, 220, 483, 0, CNHight, CNWight, 0, 0));//实际重量
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.ExWeight.ToString(), CNFont, 390, 483, 0, CNHight, CNWight, 0, 0));//计费重量
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.ExCost.ToString(), CNFont, 559, 483, 0, CNHight, CNWight, 0, 0));//邮费
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.Amount.ToString(), CNFont, 688, 483, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("付费方式:", CNFont, 27, 524, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("月结账号:", CNFont, 27, 560, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.SoID.ToString(), CNFont, 50, 653, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("内部单号:"+ ASaleOut.OID.ToString(), CNFont, 50, 693, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("寄方签名:", CNFont, 511, 516, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.Sender, CNFont, 511, 556, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("收件员:", CNFont, 680, 516, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("寄件日期:"+DateTime.Now.ToString("MM-dd hh:mm:ss"), CNFont, 503, 588, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("收件签名:", CNFont, 511, 623, 0, CNHight, CNWight, 0, 0));//收件签名
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("派件员:", CNFont, 680, 623, 0, CNHight, CNWight, 0, 0));//派件人
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("派件日期:", CNFont, 503, 701, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("顺丰速运:95338", CNFont, 27, 737, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("顺丰速运:" + ASaleOut.ExCode, CNFont, 325, 737, 0, CNHight, CNWight, 0, 0));

            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("寄件方信息:", CNFont, 11, 778, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(SendAddress, CNFont, 11, 808, 0, CNHight, CNWight, 0, 0));
            CNstartY = 838;
            builder.AppendLine(AZebraHelper.ZPL_ConvertCHText(ASaleOut.SendAddress, CNFont, 11, ref CNstartY, 0, CNHight, CNWight, 0, 0, 30, 16));//寄件地址
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.Sender, CNFont, 11, CNstartY, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.SendPhone, CNFont, 11, CNstartY + 30, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("收件方信息:", CNFont, 11, 980, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(RecAddress, CNFont, 11, 1010, 0, CNHight, CNWight, 0, 0));
            CNstartY = 1040;
            builder.AppendLine(AZebraHelper.ZPL_ConvertCHText(ASaleOut.RecAddress, CNFont, 11, ref CNstartY, 0, CNHight, CNWight, 0, 0, 30, 16));//收件地址
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.RecName, CNFont, 11, CNstartY, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.RecPhone, CNFont, 11, CNstartY + 30, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.RecZip, CNFont, 325, 1145, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("总共", CNFont, 410, 782, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.OrdQty.ToString()+ "件.赠(" + ASaleOut.GiftQty.ToString() + ")", CNFont, 688, 782, 0, CNHight, CNWight, 0, 0));
            TempHeight = 782 + 32;
            foreach (var s in ASaleOut.OutSkuLst)
            {
                builder.AppendLine(AZebraHelper.ZPL_DrawCHText(s.SkuID , CNFont, 410, TempHeight, 0, CNHight, CNWight, 0, 0));
                builder.AppendLine(AZebraHelper.ZPL_DrawCHText(s.OutQty.ToString(), CNFont, 688, TempHeight, 0, CNHight, CNWight, 0, 0));
                TempHeight += 32;
            }          

            builder.AppendLine(AZebraHelper.ZPL_End());
            return builder.ToString();
        }
        #endregion

        #region 京东快递
        public static string GetSOutPrintJD(ASaleOutPrint ASaleOut)
        {
            StringBuilder builder = new StringBuilder();
            int CNHight = 24;
            int CNWight = 12;
            string CNFont = "宋体";
            int CNstartY;
            builder.AppendLine(AZebraHelper.ZPL_Start());
            builder.AppendLine(AZebraHelper.ZPL_PageSet(0, 0));
            builder.AppendLine(AZebraHelper.ZPL_SetLabel(800, 1500));
            builder.AppendLine(AZebraHelper.ZPL_DrawBarcode(88, 65, 2, 2, 80, "Y", ASaleOut.ExCode));//快递单号
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("收方信息:", CNFont, 8, 193, 0, CNHight, CNWight, 1, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("姓名:"+ASaleOut.RecName, CNFont, 8, 223, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("电话:" + ASaleOut.RecPhone, CNFont, 202, 223, 0, CNHight, CNWight, 0, 0));
            string RecAddress = ASaleOut.RecLogistics + " " + ASaleOut.RecCity + " " + ASaleOut.RecDistrict;
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("地址:" + RecAddress, CNFont, 8, 253, 0, CNHight, CNWight, 0, 0));
            CNstartY = 283;
            builder.AppendLine(AZebraHelper.ZPL_ConvertCHText(ASaleOut.SendAddress, CNFont, 69, ref CNstartY, 0, CNHight, CNWight, 0, 0, 30, 16));//收件地址 
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("寄方信息:", CNFont, 8, 355, 0, CNHight, CNWight, 1, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("姓名:" + ASaleOut.Sender, CNFont, 8, 385, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("电话:" + ASaleOut.SendPhone, CNFont, 202, 385, 0, CNHight, CNWight, 0, 0));
            string SendAddress = ASaleOut.SendLogistics + " " + ASaleOut.SendCity + " " + ASaleOut.SendDistrict;
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("地址:" + SendAddress, CNFont, 8, 415, 0, CNHight, CNWight, 0, 0));
            CNstartY = 445;
            builder.AppendLine(AZebraHelper.ZPL_ConvertCHText(ASaleOut.SendAddress, CNFont, 69, ref CNstartY, 0, CNHight, CNWight, 0, 0, 30, 16));//寄件地址 
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("代收金额:", CNFont, 556, 193, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("代收金额:", CNFont, 556, 580, 0, CNHight, CNWight, 0, 0));
            if (ASaleOut.ExpName == "京东到付")
            {
                builder.AppendLine(AZebraHelper.ZPL_DrawCHText(""+ASaleOut.Amount.ToString() + "元", CNFont, 556, 223, 0, CNHight, CNWight, 0, 0));
                builder.AppendLine(AZebraHelper.ZPL_DrawCHText(ASaleOut.Amount.ToString() + "元", CNFont, 556, 610, 0, CNHight, CNWight, 0, 0));
            }
                
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("客户签字:", CNFont, 556, 290, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("总共:"+ASaleOut.OrdQty.ToString()+"件.赠("+ASaleOut.GiftQty.ToString()+")", CNFont, 508, 371, 0, CNHight, CNWight, 0, 0));
            TempHeight = 371 + 32;
            foreach (var s in ASaleOut.OutSkuLst)
            {
                builder.AppendLine(AZebraHelper.ZPL_DrawCHText(s.SkuID+":"+ s.OutQty.ToString(), CNFont, 508, TempHeight, 0, CNHight, CNWight, 0, 0));                
                TempHeight += 32;
            }
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("运单号:" + ASaleOut.ExCode, CNFont, 137, 532, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("内单号:" + ASaleOut.OID, CNFont, 516, 532, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("订单号:" + ASaleOut.SoID, CNFont, 8, 580, 0, CNHight, CNWight, 0, 0));

            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("姓名:" + ASaleOut.RecName, CNFont, 8, 610, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("电话:" + ASaleOut.RecPhone, CNFont, 202, 610, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("地址:" + RecAddress, CNFont, 8, 640, 0, CNHight, CNWight, 0, 0));
            CNstartY = 640+30;
            builder.AppendLine(AZebraHelper.ZPL_ConvertCHText(ASaleOut.SendAddress, CNFont, 69, ref CNstartY, 0, CNHight, CNWight, 0, 0, 30, 16));//收件地址 

            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("寄方信息:", CNFont, 8, 726, 0, CNHight, CNWight, 1, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("姓名:" + ASaleOut.Sender, CNFont, 8, 756, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("电话:" + ASaleOut.SendPhone, CNFont, 202, 756, 0, CNHight, CNWight, 0, 0));
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText("地址:" + SendAddress, CNFont, 8, 786, 0, CNHight, CNWight, 0, 0));
            CNstartY = 786+30;
            builder.AppendLine(AZebraHelper.ZPL_ConvertCHText(ASaleOut.SendAddress, CNFont, 69, ref CNstartY, 0, CNHight, CNWight, 0, 0, 30, 16));//寄件地址 
            builder.AppendLine(AZebraHelper.ZPL_DrawCHText(DateTime.Now.ToString("MM-dd hh:mm:ss"), CNFont, 524, 756, 0, CNHight, CNWight, 0, 0));//寄件时间
            builder.AppendLine(AZebraHelper.ZPL_End());
            return builder.ToString();
        }
        #endregion
    }
}