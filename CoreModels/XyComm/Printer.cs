using System;

namespace CoreModels.XyComm
{
    public partial class Printer
    {
        public int ID { get; set; }
        public int CoID { get; set; }
        public int PrintType { get; set; }
        public string PrintName { get; set; }
        public string PrintID { get; set; }
        public string IPAddress { get; set; }
        public bool IsDefault { get; set; }
        public bool Enabled { get; set; }

    }
}