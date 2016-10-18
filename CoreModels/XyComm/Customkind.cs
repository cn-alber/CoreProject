using System.Collections.Generic;
namespace CoreModels.XyComm
{
    public class CustomKind
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public string KindName { get; set; }
        public string FullName { get; set; }
        public int ParentID { get; set; }
        public int CoID { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
    }

    public class CustomKindData
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public string KindName { get; set; }
        public string FullName { get; set; }
        public int ParentID { get; set; }
        public int CoID { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public List<CustomKindData> Children { get; set; }
    }
}