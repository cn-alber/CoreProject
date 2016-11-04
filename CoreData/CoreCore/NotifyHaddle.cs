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
        public static DataResult GetMsgList(string uid,string roleid,string coid,MsgParam msgp){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){    
                try{    
                    string sqlpart = "";
                    string totalsql = ""; 
                    var totallist = new List<NotifyMsg>();

                    GetMsgCount(uid,coid,roleid);//更新未读消息
                    
                    if(!string.IsNullOrEmpty(msgp.IsRead))                
                        sqlpart = msgp.IsRead =="0" ? "ms.Readed = false AND " : "ms.Readed = true AND " ;   

                    string wheresql = sqlpart+"  (ms.Uid = "+uid+" )  AND m.`Level` in (" + msgp.LevelList + ") GROUP BY m.`Create` "+ msgp.SortDirection;
                    
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
        
        public static DataResult GetMsgCount(string uid,string coid,string roleid){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{                
                    //获取未关联到 msrelationshiop 
                    var hasDate = conn.Query<int>("SELECT MsgMaskId FROM msgmask WHERE uid = "+uid).AsList();
                    if(hasDate.Count > 0){                    
                        var unrelation = conn.Query<int>(@"SELECT m.Id FROM 
                                                                userwebmsg as m 
                                                        LEFT JOIN msgmask as mk ON mk.Uid = "+uid+@" 
                                                        where (m.RoleType like '%"+roleid+"%' OR m.Appoint = '"+coid+"' OR m.Appoint = '0' ) AND m.Id > mk.MsgMaskId ORDER BY m.Id ASC").AsList();
                        if(unrelation.Count>0){
                            string sql = "";
                            //未关联的信息在用户登录后发送给用户
                            foreach(int i in unrelation){
                                sql += @"INSERT INTO msgrelationshiop(msgrelationshiop.Uid,msgrelationshiop.MsgId,msgrelationshiop.Readed,msgrelationshiop.ReadTime) 
                                                VALUES("+uid+","+i.ToString()+",0,NOW());";
                            }
                            string  lastmask = unrelation.Max().ToString();// unrelation[unrelation.Count-1].ToString();    
                            //标记最后获取消息位置
                            sql += "UPDATE msgmask SET msgmask.MsgMaskId = "+lastmask+" WHERE msgmask.Uid = "+uid;
                            conn.Execute(sql);
                        }  
                    }else{
                        //初次登陆，未在 msgmask中有记录。浏览 userwebmsg 表获取ID
                        var ids = conn.Query<int>(@"SELECT m.Id FROM                                                             
                                                        userwebmsg as m                                                      
                                                    WHERE m.Appoint = '"+coid+"' OR m.Appoint = '0' OR m.RoleType LIKE '%"+roleid+"%';").AsList() ;
                        if(ids.Count>0){
                            string sql = "";
                            //未关联的信息在用户登录后发送给用户
                            foreach(int i in ids){
                                sql += "INSERT INTO msgrelationshiop(msgrelationshiop.Uid,msgrelationshiop.MsgId,msgrelationshiop.Readed,msgrelationshiop.ReadTime) VALUES("+uid+","+i.ToString()+",0,NOW());";
                            }
                            string  lastmask = ids.Max().ToString(); //ids[ids.Count-1].ToString();    
                            //标记最后获取消息位置
                            sql += "INSERT msgmask SET msgmask.MsgMaskId = "+lastmask+" , msgmask.Uid = "+uid;
                            Console.WriteLine(sql);
                            conn.Execute(sql);
                        }
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

        public static DataResult MsgAdd(string content,int level,string roletype,int appoint,string cid,string uid,string uname){
        
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                int rnt =  conn.Execute("INSERT INTO userwebmsg(userwebmsg.CoId,userwebmsg.Content,userwebmsg.`Create`,userwebmsg.Creater,userwebmsg.`Level`,userwebmsg.RoleType,userwebmsg.Appoint)"+ 
                                    "VALUES("+cid+",'"+content+"',NOW(),"+uid+","+level+",'"+roletype+"','"+appoint+"');");
                //获取最新插入ID                                  
                // var lastid = conn.QueryFirst<int>("SELECT LAST_INSERT_ID()");                 
                // //更新 msgrelationshiop 和 msgmask
                // string sql = "INSERT INTO msgrelationshiop(msgrelationshiop.Uid,msgrelationshiop.MsgId,msgrelationshiop.Readed,msgrelationshiop.ReadTime,msgrelationshiop.Uname) VALUES("+uid+","+lastid.ToString()+",0,NOW(),'"+uname+"');"+
                //             "UPDATE msgmask SET msgmask.MsgMaskId = "+lastid.ToString()+" WHERE msgmask.Uid = "+uid+";"; 
                // rnt = conn.Execute(sql);
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
                    Console.WriteLine(sql);
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
    
        public static DataResult MsgPoint(string content,string level,string appoint,string cid,string uid,string uname){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                int rnt =  conn.Execute("INSERT INTO userwebmsg(userwebmsg.CoId,userwebmsg.Content,userwebmsg.`Create`,userwebmsg.Creater,userwebmsg.`Level`,userwebmsg.`Appoint`)"+ 
                                    "VALUES("+cid+",'"+content+"',NOW(),"+uid+","+level+",'"+appoint+"');");
                                                  
                // var lastid = conn.QueryFirst<int>("SELECT LAST_INSERT_ID()");                 
                
                // string sql = @"INSERT INTO msgrelationshiop(
                //                     msgrelationshiop.Uid,
                //                     msgrelationshiop.MsgId,
                //                     msgrelationshiop.Readed,
                //                     msgrelationshiop.ReadTime,
                //                     msgrelationshiop.Uname) 
                //                 VALUES("+uid+","+lastid.ToString()+",0,NOW(),'"+uname+"');";
                //             //"UPDATE msgmask SET msgmask.MsgMaskId = "+lastid.ToString()+" WHERE msgmask.Uid = "+uid+";"; 
                // rnt = conn.Execute(sql);
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