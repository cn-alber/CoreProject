using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.AspNetCore.Authorization;
//using System.Web.Http;
//using System.
namespace CoreWebApi
{
    public class ExpressController : ControllBase
    {
        [AllowAnonymous]
        [HttpPostAttribute("/Core/Express")]
        public ResponseResult Get()
        {
            Encoding encoding = Encoding.UTF8;
            //string url = "https://www.kuaidi100.com/network/www/searchapi.do";
            // HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(url, UriKind.RelativeOrAbsolute));
            // webRequest.Method = "POST";
            // webRequest.ContentType = "application/x-www-form-urlencoded";
            // webRequest.Accept = "text/html, application/xhtml+xml, */*";
            // webRequest.KeepAlive = true;
            // webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko/20100101 Firefox/22.0";
            // webRequest.AllowAutoRedirect = true;
            // webRequest.Referer = "https://www.kuaidi100.com/network.shtml";
            // webRequest.ProtocolVersion = HttpVersion.Version11;
            // webRequest.Headers.Add("Origin", "https://www.kuaidi100.com");
            // webRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");

            // ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            // ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;

            // IDictionary<string, string> postData = new Dictionary<string, string>();
            // postData.Add("method", "searchnetwork");
            // postData.Add("area", "广东省 - 深圳市 - 福田区");
            // postData.Add("company", "");
            // postData.Add("keyword", "赛格");
            // postData.Add("offset", "0");
            // postData.Add("size", "8");
            // postData.Add("from", "");
            // postData.Add("auditStatus", "0");
            // StringBuilder data = new StringBuilder(string.Empty);
            // foreach (KeyValuePair<string, string> keyValuePair in postData)
            // {
            //     data.AppendFormat("{0}={1}&", keyValuePair.Key, keyValuePair.Value);
            // }
            // string para = data.Remove(data.Length - 1, 1).ToString();

            // byte[] bytePosts = Encoding.UTF8.GetBytes(para);
            // webRequest.ContentLength = bytePosts.Length;
            // using (Stream requestStream = webRequest.GetRequestStream())
            // {
            //     requestStream.Write(bytePosts, 0, bytePosts.Length);
            //     requestStream.Close();
            // }
            // HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            // using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            // {
            //     return HttpResponseMessageParse.toJson(reader.ReadToEnd());
            // }
            //return new string[] { "value1", "value2" };
            return CoreResult.NewResponse(0, null ,"Basic"); 
        }
    }
}