using CoreModels;
using CoreModels.XyUser;
using Dapper;

namespace CoreData.CoreUser
{
    public static class UserHaddle
    {
        public static DataResult GetUserInfo(string account, string password)
        {
            var m = string.Empty;
            var s = false;
            var d = new User();

            var u = DbBase.UserDB.Query<User>("select id, password, name, companyid, roleid from user where account = @acc",new {acc = account}).AsList();
            if(u == null)
            {
                m = "账号不存在";
            }
            else if(u[0].PassWord.Equals(password))
            {
                m = "密码错误";
            }
            else
            {
                s = true;

            }
            
            return new DataResult(s, u[0], m);
        }
    }
}