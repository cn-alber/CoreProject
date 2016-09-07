using System;

namespace CoreModels.XyUser
{
    public class User
    {
        public int  ID { get; set; }
        public string  Account  { get; set; }
        public string  SecretID  { get; set; }
        public string  Name  { get; set; }
        public string  PassWord  { get; set; }
        public bool  Enable  { get; set; }
        public string  Email  { get; set; }
        public string  Gender  { get; set; }
        public string  Mobile  { get; set; }
        public string  QQ  { get; set; }
        public int?  CompanyID  { get; set; }
        public int?  RoleID  { get; set; }
        public string  Creator  { get; set; }
        public DateTime?  CreateDate  { get; set; }
    }
}
