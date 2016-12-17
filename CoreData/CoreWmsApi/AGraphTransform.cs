using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
// using System.Drawing;
// using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Data;
namespace CoreData.CoreWmsApi
{
    public static class AGraphTransform
    {
         //// <summary>
        /// 按比例缩小图片，自动计算宽度
        /// </summary>
        /// <param name="strOldPic">源图文件名(包括路径)</param>
        /// <param name="strNewPic">缩小后保存为文件名(包括路径)</param>
        /// <param name="intHeight">缩小至高度</param>
        //public void SmallPicWidth(string strOldPic, string strNewPic, int intHeight)
        // public static Bitmap SmallPicWidth(string picPath, int intHeight)
        // {
        //     System.Drawing.Bitmap objPic, objNewPic;
        //     if (string.IsNullOrEmpty(picPath)) return null;
        //     string path = System.Web.HttpContext.Current.Server.MapPath(picPath);//获取img文件夹的路径          
        //     FileStream fs = new FileStream(@path, FileMode.Open, FileAccess.Read);
        //     try
        //     {
        //         objPic = new System.Drawing.Bitmap(fs);
        //         int intWidth = Convert.ToInt32(Convert.ToDouble(intHeight) / Convert.ToDouble(objPic.Height) * objPic.Width);
        //         //int intWidth = int.Parse(((float)intHeight / objPic.Height) * objPic.Width);
        //         objNewPic = new System.Drawing.Bitmap(objPic, intWidth, intHeight);
        //         //objNewPic.Save(strNewPic);
        //     }
        //     catch (Exception exp) { throw exp; }
        //     finally
        //     {
        //         //objPic = null;
        //         //objNewPic = null;
        //     }
        //     return objNewPic;
        // }

        /// <summary>  
        /// 序列化图片  
        /// </summary>  
        /// <param name="img">Bitmap</param>  
        /// <returns></returns>  
        // public static string ConvertImageToCode(Bitmap img)
        // {
        //     var sb = new StringBuilder();
        //     long clr = 0, n = 0;
        //     int b = 0;
        //     for (int i = 0; i < img.Size.Height; i++)
        //     {
        //         for (int j = 0; j < img.Size.Width; j++)
        //         {
        //             b = b * 2;
        //             clr = img.GetPixel(j, i).ToArgb();
        //             string s = clr.ToString("X");

        //             if (s.Substring(s.Length - 6, 6).CompareTo("BBBBBB") < 0)
        //             {
        //                 b++;
        //             }
        //             n++;
        //             if (j == (img.Size.Width - 1))
        //             {
        //                 if (n < 8)
        //                 {
        //                     b = b * (2 ^ (8 - (int)n));

        //                     sb.Append(b.ToString("X").PadLeft(2, '0'));
        //                     b = 0;
        //                     n = 0;
        //                 }
        //             }
        //             if (n >= 8)
        //             {
        //                 sb.Append(b.ToString("X").PadLeft(2, '0'));
        //                 b = 0;
        //                 n = 0;
        //             }
        //         }
        //         sb.Append(System.Environment.NewLine);
        //     }
        //     return sb.ToString();
        // }

    }
}