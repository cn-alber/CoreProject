using System;
using System.Collections.Generic;

namespace CoreWebApi.ApiTask
{
    public static class Utility
    {
        public readonly static bool IS_DEBUGING = true;

        static char[] LibraryPathSeparator = new char[] { '|', ';' };

        public readonly static DateTime YEAR1970 = new DateTime(1970, 1, 1);
        public readonly static DateTime YEAR2000 = new DateTime(2000, 1, 1);


        static Utility()
        {
            string baseDc = AppContext.BaseDirectory;
            Console.WriteLine(baseDc);
            return;
            // Utility.WebDirectory = baseDc.Substring(0,baseDc.Length -14)+ "CoreWebApi.ApiTask\\";//
            // Utility.BaseDirectory = Path.Combine(Utility.WebDirectory, "Task");
            // Utility.TasksDirectory = Path.Combine(Utility.BaseDirectory, "tasks");
            // Utility.CacheDirectory = Path.Combine(Utility.BaseDirectory, "cache");
            // //Utility.LibraryDirectory = Path.Combine(Utility.BaseDirectory, "library");
            // Utility.LibraryDirectory = Path.Combine(Utility.WebDirectory, "bin")+ "\\Debug";

            // Utility.Log = new API.Core.Logger(Path.Combine(Utility.BaseDirectory, "log"), 30);
            // Utility.Settings = new API.Xml.FileDocument(Path.Combine(Utility.BaseDirectory, "api.config"), "configuration", true);

            // Utility.Assemblies = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            // //assemblies 加载Task/api.config 下系统dll，为 bin\Debug 目录下没有的dll文件
            // XmlNode assemblies = Utility.Settings.SelectNode("assemblies"); 
            // if (assemblies != null)
            // {
            //     foreach (XmlNode item in assemblies.ChildNodes)
            //     {
            //         if (item.Name == "add")
            //         {
            //             string assembly = item.Attributes["assembly"].InnerText;
            //             if (!Utility.Assemblies.ContainsKey(assembly))
            //             {
            //                 Utility.Assemblies.Add(assembly, assembly);
            //             }
            //         }
            //     }
            // }
            // //library 获取公共Dll程序集
            // string[] files = System.IO.Directory.GetFiles(Utility.LibraryDirectory, "*.dll", System.IO.SearchOption.TopDirectoryOnly); 
            // foreach (string file in files)
            // {
            //     string item = System.IO.Path.GetFileNameWithoutExtension(file);
            //     if (!Utility.Assemblies.ContainsKey(item))
            //     {
            //         Utility.Assemblies.Add(item, file);
            //     }
            // }
        }


        /// <summary>
        /// 应用程序Web目录(物理路径)
        /// </summary>
        public static string WebDirectory { get; private set; }

        /// <summary>
        /// 应用程序基目录(物理路径)
        /// </summary>
        public static string BaseDirectory { get; private set; }

        /// <summary>
        /// 公共程序集目录(物理路径)
        /// </summary>
        public static string LibraryDirectory { get; private set; }

        /// <summary>
        /// 公共缓存目录(物理路径)
        /// </summary>
        public static string CacheDirectory { get; private set; }

        /// <summary>
        /// Tasks基目录(物理路径)
        /// </summary>
        public static string TasksDirectory { get; private set; }


        // /// <summary>
        // /// 日志记录器
        // /// </summary>
        // public static API.ILogging Log { get; private set; }


        /// <summary>
        /// 公共程序集列表
        /// </summary>
        public static IDictionary<string, string> Assemblies { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bigchar"></param>
        /// <returns></returns>
        public static string TSubLeft(string str, int num)
        {
            if (string.IsNullOrEmpty(str))
                return "";

            return str.SubLeft(num);
        }



        /// <summary>
        /// 创建BaseApi类型实例
        /// </summary>
        /// <param name="job">ApiJob</param>
        /// <returns></returns>
        internal static TaskWrapper CreateApi(ApiJob job)
        {
            string taskName = job.ApiKey;            
            return CreateApiBySiteType(job);
        }


        /// <summary>
        /// 创建BaseApi类型实例
        /// </summary>
        /// <param name="job">ApiJob</param>
        /// <param name="baseLibraries">分类程序集目录</param>
        /// <param name="assemblyFile">程序集文件</param>
        /// <param name="typeName">完整类名</param>
        /// <param name="crossDomain">跨域运行</param>
        /// <returns></returns>
        static TaskWrapper CreateApiBySiteType(ApiJob job)
        {
            string taskName = job.ApiKey;
            BaseApi api = null;
            try
            {            
                switch ((int)job.ApiType)
                {
                    case 3 :
                        api = SwitchJos(job.ApiKey); 
                        break;
                    default:break;
                }              
                return new TaskWrapper(api);                
            }
            catch (Exception ex)
            {            
                throw new TypeLoadException(taskName, ex);
            }
        }

        static BaseApi SwitchJos(string name){

            BaseApi api = null;
            switch (name)
            {
                case "jos\\order_api_get":
                    api = new tasks.jos.order_api_get();
                    break;  
                case "jos\\refund_api_get":
                    api = new tasks.jos.refund_api_get();
                    break;     
                default: api = null; break; 
            }
            return api;
        }








    }
}
