using System.Threading.Tasks;
using CoreModels;
using CoreModels.XyUser;

namespace CoreData.CoreUser
{

    public static class IndexHaddle{
        public static DataResult IndexContent(){
            var result = new DataResult(1,null);
            var not = new Notice2();
            var tasks = new Task[1];
            tasks[0] = Task.Factory.StartNew(()=>{
                not = NoticeHaddle.GetNoticeLst().d as Notice2;
            });
            Task.WaitAll(tasks);
            result.d= new {
                notice = new {
                    intro =  not.Title
                }
            };

            return result;
        }




    }



}