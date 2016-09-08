
namespace CoreWebApi
{
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