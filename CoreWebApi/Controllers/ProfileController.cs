using Microsoft.AspNetCore.Mvc;
using CoreData.CoreUser;
using Newtonsoft.Json.Linq;
using CoreModels.XyCore;


namespace CoreWebApi
{
    public class ProfileController : ControllBase
    {
       
        [HttpGetAttribute("/core/profile/refresh")]
        public ResponseResult refresh()
        {
            var roleid = GetRoleid();
            var coid = GetCoid();
            // var roleid = "1";
            // var coid = "1";
            var m = UserHaddle.GetRefreshList(roleid, coid,GetUname(),GetUid());
            return CoreResult.NewResponse(m.s, m.d, "Basic");
        }
        
        [HttpPostAttribute("/core/profile/lock")]
        public ResponseResult @lock()
        {

            var m = UserHaddle.lockuser(GetUid());
            return CoreResult.NewResponse(m.s, m.d, "Basic");
        }
        
        [HttpPostAttribute("/core/profile/unlock")]
        public ResponseResult @unlock([FromBodyAttribute]JObject lo)
        {
            var password = GetMD5(lo["password"].ToString(), "Xy@.");            
            var m = UserHaddle.unlockuser(GetUid(),password);  
            if(m.s == 0){
                return CoreResult.NewResponse(m.s, m.d, "Basic");
            }else{return CoreResult.NewResponse(m.s, m.d, "Indentity");}                              
        }

        //消息提醒
        [HttpPostAttribute("/core/profile/msg")]
        public ResponseResult msg([FromBodyAttribute]JObject lo)
        {
            var msgparam = new MsgParam();
            msgparam.IsRead = lo["readed"].ToString();             
            foreach (string  i in lo["levels"])
            {
                msgparam.LevelList += i+",";
            }
            msgparam.LevelList = msgparam.LevelList.Substring(0,msgparam.LevelList.Length - 1);

            msgparam.PageIndex = lo["page"] !=null? int.Parse(lo["page"].ToString()):1;
            msgparam.PageSize = 20;
                
            var uid = GetUid();
            var roleid = GetRoleid();
            var m = NotifyHaddle.GetMsgList(uid,roleid,msgparam);
            return CoreResult.NewResponse(m.s, m.d, "Basic");
        }

        [HttpGetAttribute("/core/profile/msgCount")]
        public ResponseResult msgCount()
        {
            
            var uid = GetUid();
            var roleid = GetRoleid();
            var m = NotifyHaddle.GetMsgCount(uid,roleid);
            return CoreResult.NewResponse(m.s, m.d, "Basic");
        }









        
    }
}