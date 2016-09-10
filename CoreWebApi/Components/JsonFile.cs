using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace CoreWebApi
{
    public static class JsonFile
    {
        public static string GetBasicMessage(int Code)
        {
            var section = Configuration().GetSection("BasicCode");
            return section.GetValue<string>(Code.ToString());
        }
        public static string GetJson(string FileName, string Section)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile(FileName + ".json");
            var connectionStringConfig = builder.Build();
            return connectionStringConfig[Section];
        }

        public static List<T> GetJson<T>(string FileName, string Section)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile(FileName + ".json");
            var connectionStringConfig = builder.Build();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(connectionStringConfig[Section]);
        }

        public static IConfigurationRoot Configuration()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings" + ".json");
            return builder.Build();
        }

    }
}