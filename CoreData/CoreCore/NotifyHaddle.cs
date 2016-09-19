using CoreModels;
using CoreModels.XyCore;
using Dapper;

namespace CoreData.CoreUser
{
    public static class NotifyHaddle
    {
        public static DataResult GetMsgList(string uid,string roleid,MsgParam msgp){
            int s = 0;
            string sqlpart = "";
            if(!string.IsNullOrEmpty(msgp.IsRead))                
                sqlpart = msgp.IsRead =="0" ? "ms.Readed = false AND " : "ms.Readed = true AND " ;
            
            var notifyMsg = DbBase.CoreDB.Query<NotifyMsg>(
                        "SELECT m.`Level` as MsgLevel,m.Content as Msg,m.`Create` as CreateDate,ms.Readed as Isreaded ,ms.ReadTime as ReadDate,GROUP_CONCAT(ms.uname) as Reador FROM userwebmsg as m "+
                        "LEFT JOIN  msgrelationshiop as ms  ON  m.Id = ms.MsgId "+
                        "WHERE "+sqlpart+"  (ms.Uid = @uid OR m.RoleType like '%"+roleid+"%' ) AND m.`Level` in (" + msgp.LevelList + ") "+
                        "GROUP BY m.`Create` LIMIT @pbegin,@pend",
                        new{
                            uid        = int.Parse(uid),                                                     
                            pbegin     =(msgp.PageIndex -1)*msgp.PageSize,
                            pend       = msgp.PageIndex*msgp.PageSize
                            }                       
                        ).AsList();

            return new DataResult(s,notifyMsg);
        }
        
        public static DataResult GetMsgCount(string uid,string roleid){
            int s = 0;
            //获取未关联到 msrelationshiop 
            var unrelation = DbBase.CoreDB.Query<int>("SELECT m.Id FROM userwebmsg as m LEFT JOIN msgmask as mk ON mk.Uid = 1 where m.RoleType like '%1%' AND m.Id > mk.MsgMaskId ORDER BY m.Id ASC").AsList();
            if(unrelation.Count>0){
                string sql = "";
                //未关联的信息在用户登录后发送给用户
                foreach(int i in unrelation){
                    sql += "INSERT INTO msgrelationshiop(msgrelationshiop.Uid,msgrelationshiop.MsgId,msgrelationshiop.Readed,msgrelationshiop.ReadTime) VALUES("+uid+","+i.ToString()+",0,NOW());";
                }
                string  lastmask = unrelation[unrelation.Count-1].ToString();    

                //标记最后获取消息位置
                sql += "UPDATE msgmask SET msgmask.MsgMaskId = "+lastmask+" WHERE msgmask.Uid = "+uid;
                DbBase.CoreDB.Execute(sql);
            }    
            var msgcount = DbBase.CoreDB.Query<int>("SELECT COUNT(*) FROM msgrelationshiop as ms WHERE ms.Readed = FALSE;").AsList();
            return new DataResult(s,msgcount);
        }







    }
}