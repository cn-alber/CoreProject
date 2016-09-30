using Microsoft.AspNetCore.Mvc;
using CoreData.CoreUser;
using Microsoft.AspNetCore.Authorization;


namespace CoreWebApi
{
    [AllowAnonymous]
     public class AdminController : ControllBase
     {
         [HttpGetAttribute("/core/admin/menus")]
         public ResponseResult getmenus()
         {
            if(!checkIsAdmin() ){ return CoreResult.NewResponse(-1008, null, "Basic");}  
            var roleid = GetRoleid();
            var coid = GetCoid();        
            var m = UserHaddle.GetMenuList(roleid, coid);
            return CoreResult.NewResponse(m.s, m.d, "Indentity");           
         }







     }

}