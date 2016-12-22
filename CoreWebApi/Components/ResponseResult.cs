using System;
using Newtonsoft.Json;
namespace CoreWebApi
{
    public static class CoreResult
    {
        public static readonly string _fileName = "AppSettings";
        public static readonly string _section = "BasicCode";
        public static string NewData(int s, object d, string Section)
        {
            // var configurationSection = Configuration.GetSection("AppSettings");
            // var m = JsonFile.GetJson<BasicCode>(_fileName,_section).Find(q=>q.Code == s).Message;  
            Newtonsoft.Json.Converters.IsoDateTimeConverter timeFormat = new Newtonsoft.Json.Converters.IsoDateTimeConverter();  
            timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";       
            return Newtonsoft.Json.JsonConvert.SerializeObject(new ResponseResult(s, d, JsonFile.GetMessage(s, Section)),Formatting.None, timeFormat);
        }

        public static ResponseResult NewResponse(int s, object d, string Section)
        {
            // var configurationSection = Configuration.GetSection("AppSettings");
            // var m = JsonFile.GetJson<BasicCode>(_fileName,_section).Find(q=>q.Code == s).Message;
            var m = "";
            if (s == 1)
            { // 操作成功不返回信息
                m = "";
            }
            else if (s == -1)
            {
                m = d.ToString(); //try catch报错时，显示exception msg
                d = null;
            }
            else
            {
                m = JsonFile.GetMessage(s, Section);
            }

            return new ResponseResult(s, d, m);
        }
    }
    public class ResponseResult
    {
        public ResponseResult(int S, object D, string M)
        {
            s = S;
            d = D;
            m = M;
        }

        public int s { get; set; }
        public object d { get; set; }
        public string m { get; set; }
    }
}