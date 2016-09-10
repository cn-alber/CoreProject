
namespace CoreWebApi
{
    public static class CoreResult
    {
        public static readonly string _fileName = "AppSettings";
        public static readonly string _section = "BasicCode";
        public static string NewData(int s, object d)
        {
            // var configurationSection = Configuration.GetSection("AppSettings");
            // var m = JsonFile.GetJson<BasicCode>(_fileName,_section).Find(q=>q.Code == s).Message;
            return Newtonsoft.Json.JsonConvert.SerializeObject(new ResponseResult(s, d, JsonFile.GetBasicMessage(s)));
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