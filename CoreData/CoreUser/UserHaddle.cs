using System.Collections.Generic;
using System.Linq;
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

    }
}