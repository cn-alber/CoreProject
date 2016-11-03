using CoreModels.XyUser;
using Dapper;
using System;
using System.Data;
using System.Threading.Tasks;

namespace CoreData.CoreUser
{
 public static class LogComm
 {

     public static int InsertUserLog(string Name, string LogType, string Contents, string UserName, int CoID, DateTime Time)
     {
        string sqlCommandText = @"INSERT INTO Log(Name,LogType,Contents,UserName,CoID,LogDate) VALUES(
            @Name,
            @LogType,
            @Contents,
            @UserName,
            @CoID,
            @LogDate
        )";
        var log = new Log();
        log.Name = Name;
        log.LogType = LogType;
        log.Contents = Contents;
        log.UserName = UserName;
        log.CoID = CoID;
        log.LogDate = Time;
        int result = DbBase.UserDB.Execute(sqlCommandText,log);
        return result;//DbBase.UserDB.
     }
     ///<summary>
     ///异步记录log
     ///</summary>
     public static void InsertLog(string Name, string LogType, string Contents, string UserName, int CoID){
        var task = Task.Factory.StartNew(() =>{
            string sqlCommandText = @"INSERT INTO Log(Name,LogType,Contents,UserName,CoID,LogDate) VALUES(
                @Name,
                @LogType,
                @Contents,
                @UserName,
                @CoID,
                Now()
            )";
            var log = new Log();
            log.Name = Name;
            log.LogType = LogType;
            log.Contents = Contents;
            log.UserName = UserName;
            log.CoID = CoID;
            int result = DbBase.UserDB.Execute(sqlCommandText,log);                
        });
        
     }



      public static int InsertUserLogTran(IDbTransaction Trans,string Name, string LogType, string Contents, string UserName, int CoID, DateTime Time)
     {
        string sqlCommandText = @"INSERT INTO Log(Name,LogType,Contents,UserName,CoID,LogDate) VALUES(
            @Name,
            @LogType,
            @Contents,
            @UserName,
            @CoID,
            @LogDate
        )";
        var log = new Log();
        log.Name = Name;
        log.LogType = LogType;
        log.Contents = Contents;
        log.UserName = UserName;
        log.CoID = CoID;
        log.LogDate = Time;
        int result = DbBase.UserDB.Execute(sqlCommandText,log,Trans);
        return result;//DbBase.UserDB.
     }
 }

}