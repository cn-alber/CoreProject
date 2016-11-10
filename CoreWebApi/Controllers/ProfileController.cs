using Microsoft.AspNetCore.Mvc;
using CoreData.CoreUser;
using Newtonsoft.Json.Linq;
using CoreModels.XyCore;
using System.Collections.Generic;
using System;

namespace CoreWebApi
{
    
    public class ProfileController : ControllBase
    {
       
        [HttpGetAttribute("/core/profile/refresh")]
        public ResponseResult refresh()
        {
            var roleid = GetRoleid();
            var coid = GetCoid();
            var uid = GetUid();
            var m = UserHaddle.GetRefreshList(roleid, coid,GetUname(),uid);
            return CoreResult.NewResponse(m.s, m.d, "Indentity");
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
            var msgparam  =Newtonsoft.Json.JsonConvert.DeserializeObject<MsgParam>(lo.ToString());
            msgparam.PageIndex = msgparam.PageIndex < 1 ? 1 : Math.Max(msgparam.PageIndex,1);
            msgparam.PageSize = msgparam.PageSize < 1 ? 20 : Math.Min(msgparam.PageSize,100);
            msgparam.SortDirection=" DESC "; 
            msgparam.LevelList = msgparam.levels.Count >0 ? string.Join(",",msgparam.levels.ToArray()):"5"; 
            
            var uid = GetUid();
            var coid = GetCoid();
            var roleid = GetRoleid();
            var m = NotifyHaddle.GetMsgList(uid,roleid,coid,msgparam);
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
            var coid = GetCoid();
            var m = NotifyHaddle.GetMsgCount(uid,coid,roleid);
            return CoreResult.NewResponse(m.s, m.d, "Basic");
        }

        /// <summary>
		/// 消息新增
		/// </summary>
        [HttpPostAttribute("/core/profile/msgadd")]
        public ResponseResult msgadd([FromBodyAttribute]JObject lo)
        {
            var msg =Newtonsoft.Json.JsonConvert.DeserializeObject<MsgModel>(lo.ToString());
      
            var cid = GetCoid();
            var uid = GetUid();
            var uname = GetUname();
            var m =  NotifyHaddle.MsgAdd(msg.content,msg.level,string.Join(",",msg.roletype.ToArray()),msg.appoint,cid,uid,uname); 
            return CoreResult.NewResponse(m.s, m.d, "Basic");    
        }

        [HttpPostAttribute("/core/profile/msgread")]
        public ResponseResult msgread([FromBodyAttribute]JObject lo)
        {           
            if(!checkInt(String.Join(",",lo["ids"]))) return CoreResult.NewResponse(-1009, null, "Basic");
            var readids =   Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(lo["ids"].ToString());             
            var m = NotifyHaddle.MsgRead(readids,GetUid(),GetUname());
            return CoreResult.NewResponse(m.s,m.d, "Basic");    
        }

        // [HttpGetAttribute("/core/profile/index")]
        // public ResponseResult index()
        // {
        //     var roleid = GetRoleid();
        //     var coid = GetCoid();
        //     var uid = GetUid();
        //     var m = UserHaddle.GetIndexIntro();
        //     return CoreResult.NewResponse(m.s, m.d, "Indentity");
        // }








        
    }
}