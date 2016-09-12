using CoreModels;
using CoreModels.XyUser;
using Dapper;

namespace CoreData.CoreUser
{
    public static class UserHaddle
    {
        
        ///<summary>
        ///获取登陆信息
        ///</summary>
        public static DataResult GetUserInfo(string account, string password)
        {
            var m = string.Empty;
            var s = 0;
            var d = new User();

            var u = DbBase.UserDB.Query<User>("select id, password, name, companyid, roleid from user where account = @acc",new {acc = account}).AsList();
            if(u.Count == 0)
            {
                s = 2001;
            }
            else if(!u[0].PassWord.Equals(password))
            {
                s = 2002;
            }
            
            return new DataResult(s, s == 0 ? u[0] : null);
        }
    }
}