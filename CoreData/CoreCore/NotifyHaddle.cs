using System;
using System.Collections.Generic;
using System.Linq;
using CoreModels;
using CoreModels.XyCore;
using Dapper;
using MySql.Data.MySqlClient;

namespace CoreData.CoreUser
{
    public static class NotifyHaddle
    {
        public static DataResult GetMsgList(string uid,string roleid,MsgParam msgp){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){    
                try{    
                    string sqlpart = "";
                    string totalsql = ""; 
                    var totallist = new List<NotifyMsg>();

                    GetMsgCount(uid,roleid);//更新未读消息
                    
                    if(!string.IsNullOrEmpty(msgp.IsRead))                
                        sqlpart = msgp.IsRead =="0" ? "ms.Readed = false AND " : "ms.Readed = true AND " ;   

                    string wheresql = sqlpart+"  (ms.Uid = "+uid+" OR m.RoleType like '%"+roleid+"%' ) AND m.`Level` in (" + msgp.LevelList + ") GROUP BY m.`Create` "+ msgp.SortDirection;
                    
                    if(msgp.PageIndex == 1){//pageindex 不为 1 时，不再传total 
                            totalsql = "SELECT m.`Id`, m.`Level` as MsgLevel,m.Content as Msg,m.`Create` as CreateDate,ms.Readed as Isreaded ,ms.ReadTime as ReadDate,GROUP_CONCAT(ms.uname) as Reador FROM userwebmsg as m "+
                                "LEFT JOIN  msgrelationshiop as ms  ON  m.Id = ms.MsgId WHERE "+wheresql;
                            Console.WriteLine(totalsql);                                
                            totallist = conn.Query<NotifyMsg>(totalsql).AsList();
                    }
                    wheresql += " limit "+(msgp.PageIndex -1)*msgp.PageSize +" ,"+ msgp.PageIndex*msgp.PageSize;            
                    var notifyMsg = conn.Query<NotifyMsg>(
                                "SELECT m.`Id`, m.`Level` as MsgLevel,m.Content as Msg,m.`Create` as CreateDate,ms.Readed as Isreaded ,ms.ReadTime as ReadDate,GROUP_CONCAT(ms.uname) as Reador FROM userwebmsg as m "+
                                "LEFT JOIN  msgrelationshiop as ms  ON  m.Id = ms.MsgId WHERE "+wheresql).AsList();
                    if (notifyMsg != null)
                    {
                        if(msgp.PageIndex == 1){
                            result.d = new {
                                list = notifyMsg,
                                page = msgp.PageIndex,
                                pageSize = msgp.PageSize,
                                pageTotal =  Math.Ceiling(decimal.Parse(totallist.Count.ToString())/decimal.Parse(msgp.PageSize.ToString())),
                                total = totallist.Count
                            };
                        }else{
                            result.d = new {
                                list = notifyMsg,
                                page = msgp.PageIndex,
                            };
                        }                    
                    }
                }catch(Exception e){
                    result.s = -1;
                    result.d= e.Message; 
                    conn.Dispose();
                }   
            }
            return result;
        }
        
        public static DataResult GetMsgCount(string uid,string roleid){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{                
                    //获取未关联到 msrelationshiop 
                    var unrelation = conn.Query<int>("SELECT m.Id FROM userwebmsg as m LEFT JOIN msgmask as mk ON mk.Uid = 1 where m.RoleType like '%1%' AND m.Id > mk.MsgMaskId ORDER BY m.Id ASC").AsList();
                    if(unrelation.Count>0){
                        string sql = "";
                        //未关联的信息在用户登录后发送给用户
                        foreach(int i in unrelation){
                            sql += "INSERT INTO msgrelationshiop(msgrelationshiop.Uid,msgrelationshiop.MsgId,msgrelationshiop.Readed,msgrelationshiop.ReadTime) VALUES("+uid+","+i.ToString()+",0,NOW());";
                        }
                        string  lastmask = unrelation[unrelation.Count-1].ToString();    
                        //标记最后获取消息位置
                        sql += "UPDATE msgmask SET msgmask.MsgMaskId = "+lastmask+" WHERE msgmask.Uid = "+uid;
                        conn.Execute(sql);
                    }    
                    var msgcount =0; //conn.Query<int>("SELECT COUNT(*) FROM msgrelationshiop as ms WHERE ms.Readed = FALSE;").AsList();
                    result.d = msgcount;
                }catch(Exception e){
                    result.s = -1;
                    result.d= e.Message; 
                    conn.Dispose(); 
                }
            }
            return result;
        }

        public static DataResult MsgAdd(string content,string level,string roletype,string cid,string uid,string uname){
        
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                int rnt =  conn.Execute("INSERT INTO userwebmsg(userwebmsg.CoId,userwebmsg.Content,userwebmsg.`Create`,userwebmsg.Creater,userwebmsg.`Level`,userwebmsg.RoleType)"+ 
                                    "VALUES("+cid+",'"+content+"',NOW(),"+uid+","+level+",'"+roletype+"');");
                //获取最新插入ID                                  
                var lastid = conn.Query<int>("SELECT um.Id FROM userwebmsg as um ORDER BY um.`Create` DESC LIMIT 0,1").AsList()[0];                 
                //更新 msgrelationshiop 和 msgmask
                string sql = "INSERT INTO msgrelationshiop(msgrelationshiop.Uid,msgrelationshiop.MsgId,msgrelationshiop.Readed,msgrelationshiop.ReadTime,msgrelationshiop.Uname) VALUES("+uid+","+lastid.ToString()+",1,NOW(),'"+uname+"');"+
                            "UPDATE msgmask SET msgmask.MsgMaskId = "+lastid.ToString()+" WHERE msgmask.Uid = "+uid+";"; 
                rnt = conn.Execute(sql);
                if(rnt == 0) result.s = -1;
                }catch(Exception e){
                    result.s = -1;
                    result.d= e.Message; 
                    conn.Dispose();  
                }    
            }    
            return result;
        }

        public static DataResult MsgRead(List<int> ReadIds,string uid,string uname){            
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{                                
                    string sql = "";
                    foreach(int i in ReadIds){
                        sql += "UPDATE msgrelationshiop SET msgrelationshiop.Readed = 1,msgrelationshiop.Uname='"+uname+"' WHERE msgrelationshiop.Uid = "+uid+" AND msgrelationshiop.MsgId = "+i.ToString()+";";                        
                    }

                    sql += "UPDATE msgmask SET msgmask.MsgMaskId = "+ReadIds.Max().ToString()+" WHERE msgmask.Uid = "+uid+" AND msgmask.MsgMaskId < "+ReadIds.Max().ToString();
                    
                    int rnt = conn.Execute(sql);
                    if(rnt == 0) result.s = -1;
                }catch(Exception e){
                    result.s = -1;
                    result.d= e.Message; 
                    conn.Dispose();  
                } 
            }
            return result;
        }







    }
}