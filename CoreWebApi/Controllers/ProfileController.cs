using Microsoft.AspNetCore.Mvc;
using CoreData.CoreUser;
using Newtonsoft.Json.Linq;
using CoreModels.XyCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace CoreWebApi
{
    [AllowAnonymous]
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

        /// <summary>
		/// 消息列表
		/// </summary>
        [HttpPostAttribute("/core/profile/msg")]
        public ResponseResult msg([FromBodyAttribute]JObject lo)
        {
            var msgparam = new MsgParam();
            msgparam.IsRead = lo["readed"].ToString();
            if(string.IsNullOrEmpty(lo["levels"].ToString())){
                msgparam.LevelList = "";
            }else{
                foreach (string  i in lo["levels"])
                {
                    msgparam.LevelList += i+",";
                }
                msgparam.LevelList = msgparam.LevelList.Substring(0,msgparam.LevelList.Length - 1);
            }             

            msgparam.PageIndex = lo["page"] !=null? int.Parse(lo["page"].ToString()):1;
            msgparam.PageSize = 20;
                
            var uid = GetUid();
            var roleid = GetRoleid();
            var m = NotifyHaddle.GetMsgList(uid,roleid,msgparam);
            return CoreResult.NewResponse(m.s, m.d, "Basic");
        }

        /// <summary>
		/// 消息提醒数
		/// </summary>
        [HttpGetAttribute("/core/profile/msgCount")]
        public ResponseResult msgCount()
        {
            
            var uid = GetUid();
            var roleid = GetRoleid();
            var m = NotifyHaddle.GetMsgCount(uid,roleid);
            return CoreResult.NewResponse(m.s, m.d, "Basic");
        }

        /// <summary>
		/// 消息新增
		/// </summary>
        [HttpPostAttribute("/core/profile/msgadd")]
        public ResponseResult msgadd([FromBodyAttribute]JObject lo)
        {
            var content =  lo["content"].ToString();
            var level =  lo["level"].ToString();
            var roletype =  "";
            if(!string.IsNullOrEmpty(lo["roletype"].ToString())){               
                foreach (string  i in lo["roletype"])
                {
                    roletype += i+",";
                }
                roletype = roletype.Substring(0,roletype.Length - 1);
            }        
            var cid = GetCoid();
            var uid = GetUid();
            var uname = GetUname();
            var m =  NotifyHaddle.MsgAdd(content,level,roletype,cid,uid,uname);
            return CoreResult.NewResponse(m.s, m.d, "Basic");    
        }

        [HttpPostAttribute("/core/profile/msgread")]
        public ResponseResult msgread([FromBodyAttribute]JObject lo)
        {           
            
            var readids =   Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(lo["selectIds"].ToString());             
            var m = NotifyHaddle.MsgRead(readids,GetUid(),GetUname());
            return CoreResult.NewResponse(m.s,m.d, "Basic");    
        }








        
    }
}