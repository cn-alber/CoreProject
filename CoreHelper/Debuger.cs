using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace CoreHelper
{
    public class Debuger 
    {
        static private void push(dynamic obj) 
        {
            UdpClient client = new UdpClient();
            //System.Net.IPAddress remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
            //var ip = HttpContext.Features.Get()?.RemoteIpAddress?.ToString();
            //obj.ip = remoteIpAddress.ToString();
            // if (typeof(obj) == "object") {
            // } else {
            //     obj = new {method="log", data=obj, ip=realIP};
            // }
            byte[] bytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(obj));
            client.SendAsync(bytes, bytes.Length, "192.168.30.180", 6000);
            //Console.WriteLine("text-------------------------------------------------------");
            client.Dispose();
        }

        /**
        单条提信息
        */
        static public void log(dynamic obj)
        {
            var oo = new {method="log", data=obj};
            push(oo);
        }

        /**
        单条提示信息
        */
        static public void info(dynamic obj)
        {
            var oo = new {method="info", data=obj};
            push(oo);
        }
        /**
        单条失败信息
        */
        static public void error(dynamic obj)
        {
            var oo = new {method="error", data=obj};
            push(oo);
        }
        
        /**
        单条警告信息
        */
        static public void warning(dynamic obj)
        {
            var oo = new {method="warning", data=obj};
            push(oo);
        }

        /**
        单条成功信息
        */
        static public void success(dynamic obj)
        {
            var oo = new {method="success", data=obj};
            push(oo);
        }

        /**
        单条成功信息
        */
        static public void exception(dynamic obj)
        {
            var oo = new {method="exception", data=obj};
            push(oo);
        }

        /**
        单条成功信息
        */
        static public void sql(dynamic obj)
        {
            var oo = new {method="sql", data=obj};
            push(oo);
        }

        /**
        组输出
        * 当 obj 为数组的时候
        obj = [
            {method: 'log|success|error|warning|info', data: mixed}, //如果需要指定输出标记，则需要类此Object
            '提示信息', //也可以string，默认为 log 输出
        ]
        */
        static public void group(List<dynamic> obj, string title="")
        {
            var oo = new {method="group", data=obj, title=title};
            push(oo);
        }
    }
}