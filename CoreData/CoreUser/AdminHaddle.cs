using System.Collections.Generic;
using System.Linq;
using CoreModels;
using CoreModels.XyUser;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Text;

namespace CoreData.CoreUser
{
    public static class AdminHaddle
    {
  
        ///<summary>
        ///获取菜单列表
        ///</summary>
        public static DataResult GetMenuList(string roleid, string coid)
        {
            var s = 1;
            var cname = "menus" + coid + roleid;

            //获取菜单缓存
            // var parent = CacheBase.Get<List<Menu>>(cname);
            // if (parent == null)
            // {
                var parent = GetMenu(roleid, coid);
                if (parent == null)
                {
                    s = -2004;
                }
                else
                {
                    //无缓存，添加缓存
                    //CacheBase.Set<List<Menu>>(cname, parent);
                }
            //}
            return new DataResult(s, s == 1 ? parent : null);
        }

        ///<summary>
        ///获取菜单列表数据
        ///</summary>
        public static List<Menu> GetMenu(string roleid, string coid)
        {
            var parent = new List<Menu>();
            using (var conn = new MySqlConnection(DbBase.UserConnectString))
            {
                try
                {
                    //获取权限列表
                    var role = UserHaddle.GetRole(roleid, coid);
                    if (role.s > 1) return null;
                    var r = role.d as Role;
                    //"select name,NewIcon,NewIconPre,NavigateUrl,ParentID from menus where viewpowerid in (" + r.ViewList + ") order by ParentID,sortindex"
                    string sql = "select menus.id, menus.`Name` as `name`,NewUrl as router,SortIndex as `order`, menus.Remark, ParentID ,power.Title as access from menus "+
                                "LEFT JOIN power on power.ID = menus.ViewPowerID where viewpowerid in (" + r.ViewList + ") order by ParentID,sortindex"; 

                    var child = conn.Query<Menu>(sql).AsList();
                    if (child.Count == 0)
                    {
                        return null;
                    }
                    var pidarray = (from c in child select c.parentid).Distinct().ToArray();
                    var pid = string.Join(",", pidarray);
                    //"select id,name,NewIcon,NewIconPre,NavigateUrl,ParentID from menus where id in (" + pid + ") order by sortindex"
                    sql = "select menus.id, menus.`Name` as `name`,NewUrl as router,SortIndex as `order`, menus.Remark, ParentID ,power.Title as access from menus "+
                                "LEFT JOIN power on power.ID = menus.ViewPowerID where menus.id in (" + pid + ") order by sortindex"; 
                    
                    parent = conn.Query<Menu>(sql).AsList();

                    foreach (var p in parent)
                    {
                        p.children = (from c in child where c.parentid == p.id select c).ToList();
                    }
                }
                catch
                {
                    conn.Dispose();
                    return null;
                }
            }
            return parent;
        }

        public static DataResult CreatMenu(string name,string router,string icon,string order,string remark,string parentid,string accessid,string uname,string coid){            
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.UserConnectString) ){
                try
                {
                    string sql = "INSERT menus SET menus.`Name`='"+name+"',menus.NewUrl='"+router+"',menus.NewIcon='"+icon+"',"+
                                 "menus.SortIndex='"+order+"',menus.Remark='"+remark+"',menus.ParentID="+parentid+",menus.ViewPowerID="+accessid;
                    int rnt = conn.Execute(sql);
                    if(rnt > 0){
                        result.s = 1;
                    }else{
                        result.s = -2014;
                    }
                    //LogComm.InsertUserLog("新增菜单", "menu", "菜单名"+name ,uname, coid, DateTime.Now);
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d= e.Message; 
                    conn.Dispose();
                }
            }
            return result;
        }

  
        public static DataResult modifyMenu(string id,string name,string router,string icon,string order,string remark,string parentid,string accessid,string uname,string coid){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.UserConnectString) ){
                try
                {
        
                    //LogComm.InsertUserLog("编辑菜单", "menu", "菜单名"+name ,uname, coid, DateTime.Now);
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d= e.Message; 
                    conn.Dispose();
                }
            }
            return result;
        }
    
        public static DataResult demo(int sys_id){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.UserConnectString) ){
                try
                {
        

                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d= e.Message; 
                    conn.Dispose();
                }
            }
            return result;
        }

  
  
  
  
  
    }
}