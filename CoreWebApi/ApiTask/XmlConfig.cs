using System;
using System.IO;
using System.Text;
using System.Xml;

namespace CoreWebApi.ApiTask
{
    /// <summary>
    /// 基于XML的配置类
    /// </summary>
    public sealed class XmlConfig
    {
        private XmlDocument _xml;
        private XmlNode _cfg;

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FullPath
        {
            get;
            private set;
        }

        /// <summary>
        /// 根节点名
        /// </summary>
        public string RootName
        {
            get;
            private set;
        }

        /// <summary>
        /// 是否只读
        /// </summary>
        public bool IsReadOnly
        {
            get;
            private set;
        }

        /// <summary>
        /// 文件更新时间戳
        /// </summary>
        public DateTime UpdateTime
        {
            get;
            private set;
        }

        /// <summary>
        /// 是否已被更新
        /// </summary>
        /// <returns></returns>
        public bool Updated
        {
            get
            {
                DateTime dt = File.GetLastWriteTime(this.FullPath);
                if (dt > this.UpdateTime)
                {
                    return true;
                }
                return false;
            }
        }
        private bool _updated;


        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="fileName">配置文件物理路径</param>
        public XmlConfig(string fileName) : this(fileName, "config", false) { }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="fileName">配置文件物理路径</param>
        /// <param name="rootName">配置文件根节点名</param>
        public XmlConfig(string fileName, string rootName) : this(fileName, rootName, false) { }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="fileName">配置文件物理路径</param>
        /// <param name="isReadOnly">配置文件是否只读</param>
        public XmlConfig(string fileName, bool isReadOnly) : this(fileName, "config", isReadOnly) { }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="fileName">配置文件物理路径</param>
        /// <param name="rootName">配置文件根节点名</param>
        /// <param name="isReadOnly">配置文件是否只读</param>
        public XmlConfig(string fileName, string rootName, bool isReadOnly)
        {
            this.FullPath = fileName;

            if (rootName.StartsWith("/"))
                this.RootName = rootName;
            else
                this.RootName = "/" + rootName;

            this.IsReadOnly = isReadOnly;

            this.Load();
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        public void Load()
        {
            this.UpdateTime = File.GetLastWriteTime(this.FullPath);

            this._xml = new XmlDocument();
            byte[] byteArray = Encoding.UTF8.GetBytes(this.FullPath);
            this._xml.Load(new MemoryStream(byteArray));
            this._cfg = this._xml[this.RootName];//.SelectSingleNode(this.RootName);
            this._updated = false;
        }


        /// <summary>
        /// 获取或设置节点值
        /// </summary>
        /// <param name="key">XPath</param>
        /// <returns></returns>
        public string this[string key]
        {
            get
            {
                XmlNode node = this._cfg[key];//.SelectSingleNode(key);
                if (node == null)
                    return null;
                else
                    return node.InnerText;
            }

            set
            {
                if (!this.IsReadOnly)
                {
                    if (key != null)
                    {
                        key = key.Trim('/');
                    }

                    XmlNode node = this._cfg[key];//.SelectSingleNode(key);
                    if (node == null)
                    {
                        string[] keys = key.Split('/');

                        node = this._cfg;
                        foreach (string _key in keys)
                        { 
                            XmlNode _node = node[key];//.SelectSingleNode(_key);
                            if (_node == null)
                                node = node.AppendChild(this._xml.CreateElement(_key));
                            else
                                node = _node;
                        }
                    }

                    node.InnerText = value;
                    this._updated = true;
                }
            }
        }

        /// <summary>
        /// 选择匹配XPath表达式的第一个节点
        /// </summary>
        /// <param name="xpath">XPath</param>
        /// <returns></returns>
        public XmlNode SelectNode(string xpath)
        {
            return this._cfg[xpath];//.SelectSingleNode(xpath);
        }

        /// <summary>
        /// 选择匹配XPath表达式的节点列表
        /// </summary>
        /// <param name="xpath">XPath</param>
        /// <returns></returns>
        public XmlNodeList SelectNodes(string xpath)
        {
            return this._xml.GetElementsByTagName(xpath); //.SelectNodes(xpath);
        }


        /// <summary>
        /// 保存修改到配置文件
        /// </summary>
        public void Save()
        {
            if (!this.IsReadOnly && this._updated)
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(this.FullPath);
				this._xml.Save(new MemoryStream(byteArray));
                //this._xml.Save(this.FullPath);
                this._updated = false;
                this.UpdateTime = File.GetLastWriteTime(this.FullPath);
            }
        }

        /// <summary>
        /// 保存修改到配置文件，并重新加载新配置
        /// </summary>
        public void Update()
        {
            this.Save();
            this.Load();
        }
    }
}
