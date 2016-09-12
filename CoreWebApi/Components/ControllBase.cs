using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi
{
    public class ControllBase : Controller
    {
        #region 账号相关
        ///<summary>
        ///获取用户ID
        ///</summary>
        public string GetUid()
        {
            return HttpContext.User.Claims.FirstOrDefault(q => q.Type.Equals("uid")).Value;
        }

        ///<summary>
        ///获取用户名
        ///</summary>
        public string GetUname()
        {
            return HttpContext.User.Claims.FirstOrDefault(q => q.Type.Equals("uname")).Value;
        }

        ///<summary>
        ///获取公司ID
        ///</summary>
        public string GetCoid()
        {
            return HttpContext.User.Claims.FirstOrDefault(q => q.Type.Equals("coid")).Value;
        }

        ///<summary>
        ///获取公司ID
        ///</summary>
        public string GetRoleid()
        {
            return HttpContext.User.Claims.FirstOrDefault(q => q.Type.Equals("roleid")).Value;
        }

        #endregion


        #region MD5
        public string GetMD5(string sDataIn, string move)
        {

            using (var md5 = MD5.Create())
            {
                byte[] bytValue, bytHash;
                bytValue = System.Text.Encoding.UTF8.GetBytes(move + sDataIn);
                bytHash = md5.ComputeHash(bytValue);
                md5.Dispose();
                string sTemp = "";
                for (int i = 0; i < bytHash.Length; i++)
                {
                    sTemp += bytHash[i].ToString("x").PadLeft(2, '0');
                }
                return sTemp;
            }
        }
        #endregion
    }
}