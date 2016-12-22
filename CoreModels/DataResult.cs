
namespace CoreModels
{
    public class DataResult
    {
        public DataResult(int S, object D)
        {
            s = S;
            d = D;
        }
        public DataResult(int S, object D, string m)
        {
            s = S;
            d = D;
        }
                

        public int s { get; set; }
        public object d { get; set; }
        public string m{get;set;}
    }
}