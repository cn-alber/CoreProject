using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
            var s = 0;

            var u = DbBase.UserDB.Query<User>("select id, password, name, companyid, roleid, enable, islocked from user where account = @acc", new { acc = account }).AsList();
            if (u.Count == 0)
            {
                s = 2001;
            }
            else if(!u[0].Enable)
            {
                s = 2005;
            }
            else if (!u[0].PassWord.Equals(password))
            {
                s = 2002;
            }

            return new DataResult(s, s == 0 ? u[0] : null);
        }

        ///<summary>
        ///获取授权列表
        ///</summary>
        public static DataResult GetRole(string roleid, string coid)
        {
            var s = 0;
            var cname = "role" + coid + roleid;
            var cu = CacheBase.Get<Role>(cname);
            if (cu == null)
            {
                var u = DbBase.UserDB.Query<Role>("select * from role where id = @rid and companyid = @coid", new { rid = roleid, coid = coid }).AsList();
                if (u.Count == 0)
                {
                    s = 2003;
                }
                else
                {
                    cu = u[0];

                    CacheBase.Set<Role>(cname, cu);
                }
            }
            return new DataResult(s, cu);
        }

        public static User GetUser(string uid){
            var u = DbBase.UserDB.Query<User>("select id, password, name, companyid, roleid, enable, islocked from user where ID = @uid", new { uid = uid }).First();
            return u;
        }

        

        ///<summary>
        ///获取菜单列表
        ///</summary>
        public static DataResult GetMenuList(string roleid, string coid)
        {
            var s = 0;
            var cname = "menus" + coid + roleid;

            //获取菜单缓存
            var parent = CacheBase.Get<List<Menu>>(cname);
            if (parent == null)
            {
                parent = GetMenu(roleid, coid);
                if (parent == null)
                {
                    s = 2004;
                }
                else
                {
                    //无缓存，添加缓存
                    CacheBase.Set<List<Menu>>(cname, parent);
                }
            }
            return new DataResult(s, s == 0 ? parent : null);
        }

        ///<summary>
        ///获取菜单列表数据
        ///</summary>
        public static List<Menu> GetMenu(string roleid, string coid)
        {
            var parent = new List<Menu>();
            try
            {
                //获取权限列表
                var role = GetRole(roleid, coid);
                if (role.s > 0) return null;
                var r = role.d as Role;

                var child = DbBase.UserDB.Query<Menu>("select name,NewIcon,NewIconPre,NavigateUrl,ParentID from menus where viewpowerid in (" + r.ViewList + ") order by ParentID,sortindex").AsList();
                if (child.Count == 0)
                {
                    return null;
                }
                var pidarray = (from c in child select c.ParentID).Distinct().ToArray();
                var pid = string.Join(",", pidarray);
                parent = DbBase.UserDB.Query<Menu>("select id,name,NewIcon,NewIconPre,NavigateUrl,ParentID from menus where id in (" + pid + ") order by sortindex").AsList();

                foreach (var p in parent)
                {
                    p.Data = (from c in child where c.ParentID == p.ID select c).ToList();
                }
            }
            catch
            {
                return null;
            }
            return parent;
        }


        ///<summary>
        ///获取菜单列表(避免与上面部分代码冲突)
        ///</summary>
        public static DataResult GetRefreshList(string roleid, string coid,string uname,string uid)
        {
            var s = 0;
            var cname = "menus" + coid + roleid;

            //获取菜单缓存
            CacheBase.Remove(cname);
            var parent = CacheBase.Get<List<Refresh>>(cname);
            var parentRefresh = new List<Refresh>();
        

            if (parent == null)
            {
                parent = GetRefresh(roleid, coid);
                if (parent == null)
                {
                    s = 2004;
                }
                else
                {
                    var reslut = new {
                        isLocked = false,
                        permissionMenus = parent,
                        user = new {
                            name = uname,
                            avatar = "/path/avatarx80.png",
                            uid = uid,
                            key = "",
                            sign_time = "",
                        }
                    };            
                    return new DataResult(s, reslut);

                    //无缓存，添加缓存
                    //CacheBase.Set<List<Menu>>(cname, parent);
                }
            }
            return new DataResult(s, s == 0 ? parent : null);
        }

        ///<summary>
        ///获取菜单列表数据
        ///</summary>
        public static List<Refresh> GetRefresh(string roleid, string coid)
        {
            var parent = new List<Refresh>();
            try
            {
                //获取权限列表
                var role = GetRole(roleid, coid);
                if (role.s > 0) return null;
                var r = role.d as Role;

                var child = DbBase.UserDB.Query<Refresh>("select id,name,CASE NewIcon  WHEN NewIconPre IS NOT NULL  THEN CONCAT(NewIcon,',','') ELSE CONCAT(NewIconPre,',','fa') END AS icons ,NavigateUrl as path,ParentID from menus where viewpowerid in (" + r.ViewList + ") order by ParentID,sortindex").AsList();
                foreach(var c in child){
                    c.icon = c.icons.Split(',');
                }

                if (child.Count == 0)
                {
                    return null;
                }
                var pidarray = (from c in child select c.parentID).Distinct().ToArray();
                var pid = string.Join(",", pidarray);
                parent = DbBase.UserDB.Query<Refresh>("select id,name,CASE NewIcon  WHEN NewIconPre IS NOT NULL  THEN CONCAT(NewIcon,',','') ELSE CONCAT(NewIconPre,',','fa') END AS icons ,NavigateUrl as path,ParentID from menus where id in (" + pid + ") order by sortindex").AsList();
                
                foreach (var p in parent)
                {
                    p.type = 2;
                    p.icon = p.icons.Split(',');
                    p.data = (from c in child where c.parentID == p.id select c).ToList();
                }
            }
            catch
            {
                return null;
            }
            return parent;
        }

        public static DataResult lockuser(string uid){
            int s = 0;
            try{
              int rnt =  DbBase.UserDB.Execute("UPDATE `user` SET `user`.IsLocked = 1 WHERE `user`.ID = "+uid);    
              if(rnt<1)  s = -1;
            
            }catch{s = -1;}
            
            return new DataResult(s,null);
        }

        public static DataResult unlockuser(string uid,string password){
            int s = 0;
            try{
                int rnt = DbBase.UserDB.Execute("UPDATE `user` SET `user`.IsLocked = 0 WHERE `user`.ID = @uid AND `user`.`PassWord` = @p ",new { uid = int.Parse(uid) ,p = password }); 
                if(rnt == 0) s = 2002;
            }catch{ s = 2002;}
            return new DataResult(s,null);
        }

        public static DataResult editPwd(string uid,string oldp,string newp,string reNewPwd){
            int s = 0;
            string regexstr = @".{6,18}";
            if (string.IsNullOrEmpty(oldp)) {  s = 2006; }
            if (string.IsNullOrEmpty(newp)) {  s = 2012; }
            if (!Regex.IsMatch(newp, regexstr)){ s=2007; }
            if (newp != reNewPwd) {  s=2010; }
            if(s == 0){
                try{
                    int rnt = DbBase.UserDB.Execute("UPDATE `user` SET `user`.`PassWord`= @newp WHERE `user`.ID = @uid AND `user`.`PassWord` = @p ",
                                                new { newp = newp ,uid = int.Parse(uid) ,p = oldp }); 
                    if(rnt == 0) s = 2002;
                }catch{ s = 2002;} 
            }
 
            return new DataResult(s,null);
        }








    }
}