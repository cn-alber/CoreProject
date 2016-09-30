using Microsoft.AspNetCore.Mvc;
using CoreData.CoreUser;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;

namespace CoreWebApi
{
    [AllowAnonymous]
     public class AdminController : ControllBase
     {
         [HttpGetAttribute("/core/admin/menus")]
         public ResponseResult getmenus()
         {
            //if(!checkIsAdmin() ){ return CoreResult.NewResponse(-1008, null, "Basic");}  
            var roleid = GetRoleid();
            var coid = GetCoid();        
            var m = AdminHaddle.GetMenuList(roleid, coid);
            return CoreResult.NewResponse(m.s, m.d, "Indentity");           
         }
         //创建菜单
         [HttpPostAttribute("/core/admin/createmenus")]
         public ResponseResult createmenus([FromBodyAttribute]JObject lo)
         {

             var name = lo["name"].ToString();
             var router = lo["router"].ToString();
             var icon = lo["icon"].ToString();
             var order = lo["order"].ToString();
             var remark = lo["remark"].ToString();
             var parentid = lo["parentid"].ToString();
             var accessid = lo["accessid"].ToString();

             var uname = GetUname();
             var coid = GetCoid();        
             var m = AdminHaddle.CreatMenu(name,router,icon,order,remark,parentid,accessid,uname,coid);
             return CoreResult.NewResponse(m.s, m.d, "Indentity");           
         }
         //编辑菜单
         [HttpPostAttribute("/core/admin/modifymenus")]
         public ResponseResult modifymenus([FromBodyAttribute]JObject lo)
         {
             var id = lo["id"].ToString();
             var name = lo["name"].ToString();
             var router = lo["router"].ToString();
             var icon = lo["icon"].ToString();
             var order = lo["order"].ToString();
             var remark = lo["remark"].ToString();
             var parentid = lo["parentid"].ToString();
             var accessid = lo["accessid"].ToString();

             var uname = GetUname();
             var coid = GetCoid();        
             var m = AdminHaddle.modifyMenu(id,name,router,icon,order,remark,parentid,accessid,uname,coid);
            return CoreResult.NewResponse(m.s, m.d, "Indentity");           
         }









     }

}