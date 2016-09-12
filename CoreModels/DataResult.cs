
namespace CoreModels
{
    public class DataResult
    {
        public DataResult(bool S, object D, string M)
        {
            s = S;
            d = D;
            m = M;
        }

        public bool s { get; set; }
        public object d { get; set; }
        public string m { get; set; }
    }
}