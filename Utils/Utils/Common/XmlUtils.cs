using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Utils.Common
{
    public class XmlUtils
    {
        /// <summary>
        /// 创建一个空的DOM对象
        /// </summary>
        /// <returns></returns>
        public static XmlDocument CreateNewXmlDoc()
        {
            return CreateNewXmlDoc(string.Empty);
        }

        /// <summary>
        /// 创建一个含有根节点的空的DOM对象
        /// </summary>
        /// <param name="strRootName"></param>
        /// <returns></returns>
        public static XmlDocument CreateNewXmlDoc(string strRootName)
        {
            try
            {
                var xDoc = new XmlDocument();

                //建立Xml的定义声明
                XmlDeclaration dec = xDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                xDoc.AppendChild(dec);

                if (!string.IsNullOrEmpty(strRootName))
                {
                    XmlElement rootEl = xDoc.CreateElement(strRootName);
                    xDoc.AppendChild(rootEl);
                }

                return xDoc;
            }
            catch (Exception ex)
            {
                throw new Exception("创建DOM对象失败", ex);
            }
        }

        /// <summary>
        /// 设置节点数据，节点存在则修改，节点不存在则新建
        /// </summary>
        /// <param name="xDoc"></param>
        /// <param name="fatherNode"></param>
        /// <param name="strChildNodeName"></param>
        /// <param name="strChildNodeText"></param>
        public static void SetNewNode(XmlDocument xDoc, XmlElement fatherNode, string strChildNodeName, string strChildNodeText)
        {
            var node = fatherNode.SelectSingleNode(strChildNodeName);
            if (null == node)
            {
                AddNewNode(xDoc, fatherNode, strChildNodeName, strChildNodeText);
            }
            else
            {
                SetNodeInnertext(node, strChildNodeText);
            }
        }

        /// <summary>
        /// 根据文件的路径，加载XML文档的DOM对象
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <returns></returns>
        public static XmlDocument GetXmlDocumnet(string strFilePath)
        {
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(strFilePath);
                return xmlDoc;
            }
            catch (Exception e)
            {
                throw new Exception("加载XML文件异常：" + strFilePath, e);
            }
        }

        /// <summary>
        /// 加载一段字符串为XML的DOM对象
        /// </summary>
        /// <param name="strXmlText"></param>
        /// <returns></returns>
        public static XmlDocument GetXmlDocFromString(string strXmlText)
        {
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(strXmlText);
                return xmlDoc;
            }
            catch (Exception e)
            {
                throw new Exception("加载XML文件异常：", e);
            }
        }

        /// <summary>  
        /// 将XmlDocument转化为string  
        /// </summary>  
        /// <param name="xmlDoc"></param>  
        /// <returns></returns>  
        public static string ConvertXmlToString(XmlDocument xmlDoc)
        {
            System.IO.MemoryStream stream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(stream, null);
            writer.Formatting = Formatting.Indented;
            xmlDoc.Save(writer);
            StreamReader sr = new StreamReader(stream, System.Text.Encoding.UTF8);
            stream.Position = 0;
            string xmlString = sr.ReadToEnd();
            sr.Close();
            stream.Close();
            return xmlString;
        }

        /// <summary>
        /// 创建新节点对象
        /// </summary>
        /// <param name="xDoc">DOM对象</param>
        /// <param name="strName">节点名称</param>
        /// <returns></returns>
        public static XmlElement CreateElement(XmlDocument xDoc, string strName)
        {
            try
            {
                return xDoc.CreateElement(strName);
            }
            catch (Exception ex)
            {
                throw new Exception("创建节点异常", ex);
            }
        }

        /// <summary>
        /// 将子节点添加到父亲节点下
        /// </summary>
        /// <param name="fatherNode"></param>
        /// <param name="childNode"></param>
        public static void AddNewNode(XmlNode fatherNode, XmlNode childNode)
        {
            try
            {
                if (null != fatherNode && null != childNode)
                {
                    fatherNode.AppendChild(childNode);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("插入节点异常", ex);
            }
        }

        /// <summary>
        /// 向节点下添加新的子节点对象
        /// </summary>
        /// <param name="xDoc"></param>
        /// <param name="fatherNode"></param>
        /// <param name="strChildNodeName"></param>
        public static void AddNewNode(XmlDocument xDoc, XmlNode fatherNode, string strChildNodeName)
        {
            AddNewNode(xDoc, fatherNode, strChildNodeName, string.Empty);
        }

        /// <summary>
        /// 向节点下添加新的子节点对象
        /// </summary>
        /// <param name="xDoc"></param>
        /// <param name="fatherNode"></param>
        /// <param name="strChildNodeName"></param>
        /// <param name="strChildNodeText"></param>
        public static void AddNewNode(XmlDocument xDoc, XmlNode fatherNode, string strChildNodeName, string strChildNodeText)
        {
            try
            {
                XmlNode childNode = xDoc.CreateElement(strChildNodeName);
                if (!string.IsNullOrEmpty(strChildNodeText))
                {
                    childNode.InnerText = strChildNodeText;
                }

                fatherNode.AppendChild(childNode);
            }
            catch (Exception ex)
            {
                throw new Exception("添加新的节点异常", ex);
            }
        }

        /// <summary>
        /// 获取XML文件的全部内容
        /// </summary>
        /// <param name="strXmlFilePath"></param>
        /// <returns></returns>
        public static string GetAllXmlText(string strXmlFilePath)
        {
            XmlDocument xDoc = GetXmlDocumnet(strXmlFilePath);
            return GetAllXmlText(xDoc);
        }

        /// <summary>
        /// 获取XML文件的全部内容
        /// </summary>
        /// <param name="xDoc"></param>
        /// <returns></returns>
        public static string GetAllXmlText(XmlDocument xDoc)
        {
            if (null != xDoc)
            {
                return xDoc.InnerXml;
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取XML字符串片段的根节点对象
        /// </summary>
        /// <param name="strXmlText"></param>
        /// <returns></returns>
        public static XmlElement GetRootElFromString(string strXmlText)
        {
            try
            {
                XmlDocument xDoc = GetXmlDocFromString(strXmlText);
                return GetRootEl(xDoc);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("获取XML根节点对象异常。解析XML字符串:\r\n{0}", strXmlText), ex);
            }
        }

        /// <summary>
        /// 获取XML对象的根节点对象
        /// </summary>
        /// <param name="strXmlFilePath"></param>
        /// <returns></returns>
        public static XmlElement GetRootEl(string strXmlFilePath)
        {
            if (!FileUtils.IsExist(strXmlFilePath))
            {
                return null;
            }

            XmlDocument xDoc = GetXmlDocumnet(strXmlFilePath);
            return GetRootEl(xDoc);
        }

        /// <summary>
        /// 获取XML对象的根节点对象
        /// </summary>
        /// <param name="xDoc"></param>
        /// <returns></returns>
        public static XmlElement GetRootEl(XmlDocument xDoc)
        {
            try
            {
                return (XmlElement)xDoc.SelectSingleNode("./*");
            }
            catch (Exception ex)
            {
                throw new Exception("获取XML文件根节点对象异常", ex);
            }
        }

        /// <summary>
        /// 获取节点的内容值
        /// </summary>
        /// <param name="xPath"></param>
        /// <param name="fatherEl"></param>
        /// <returns></returns>
        public static string GetNodeText(string xPath, XmlElement fatherEl)
        {
            var element = (XmlElement)fatherEl.SelectSingleNode(xPath);
            return GetNodeText(element);
        }

        /// <summary>
        /// 获取节点的内容值
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string GetNodeText(XmlElement element)
        {
            return GetNodeText(element, false);
        }

        /// <summary>
        /// 获取节点的子对象内容。带有子标记对信息。
        /// <para>element:节点对象</para>
        /// <para>isContainOrgNode:是否包含节点标记</para>
        /// <para>1）isContainOrgNode = True</para>
        /// <para>返回： &lt;arg&gt;内容&lt;/arg&gt; </para>
        /// <para>2) isContainOrgNode = False</para>
        /// <para>返回： 内容</para>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="isContainOrgNode"></param>
        /// <returns></returns>
        public static string GetNodeText(XmlElement element, bool isContainOrgNode)
        {
            try
            {
                if (null != element)
                {
                    if (!isContainOrgNode)
                    {
                        return element.InnerXml;
                    }

                    string name = element.Name;
                    var sb = new StringBuilder();
                    sb.Append("<" + name + ">");
                    sb.Append(element.InnerXml);
                    sb.Append("</" + name + ">");

                    return sb.ToString();
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception("获取节点内容异常", ex);
            }
        }

        /// <summary>
        /// 获取节点的内容值
        /// </summary>
        /// <param name="xPath"></param>
        /// <param name="fatherEl"></param>
        /// <returns></returns>
        public static string GetNodesText(string xPath, XmlNode fatherEl)
        {
            var sb = new StringBuilder();
            XmlNodeList element = fatherEl.SelectNodes(xPath);
            if (null != element)
            {
                foreach (XmlElement xn in element)
                {
                    sb.Append(GetNodeText(xn));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 获取节点中的属性值内容
        /// </summary>
        /// <param name="node"></param>
        /// <param name="strAttName"></param>
        /// <returns></returns>
        public static string GetAttribute(XmlElement node, string strAttName)
        {
            try
            {
                if (null == node || string.IsNullOrEmpty(strAttName))
                {
                    return string.Empty;
                }

                return node.GetAttribute(strAttName);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("获取节点属性信息异常。AttributeName={0}", strAttName), ex);
            }
        }

        /// <summary>
        /// 为节点添加属性值
        /// </summary>
        /// <param name="node"></param>
        /// <param name="strAttName"></param>
        /// <param name="strAttValue"></param>
        /// <returns></returns>
        public static void SetAttribute(XmlElement node, string strAttName, string strAttValue)
        {
            try
            {
                if (null == node || string.IsNullOrEmpty(strAttName))
                {
                    return;
                }

                node.SetAttribute(strAttName, strAttValue);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("添加属性值异常。Name={0}-Value={1}", strAttName, strAttValue), ex);
            }
        }

        /// <summary>
        /// 设置一个指定节点的内容
        /// </summary>
        /// <param name="node"></param>
        /// <param name="strText"></param>
        public static void SetNodeInnertext(XmlNode node, string strText)
        {
            try
            {
                if (null != node)
                {
                    node.InnerText = strText;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("设置节点内容出错。Text={0}", strText), ex);
            }
        }

        /// <summary>
        /// 向指定节点中，添加数据信息内容
        /// </summary>
        /// <param name="xPath"></param>
        /// <param name="strText"></param>
        /// <param name="fatherEl"></param>
        public static void SetNodeInnertext(string xPath, string strText, XmlElement fatherEl)
        {
            XmlNode node = fatherEl.SelectSingleNode(xPath);
            SetNodeInnertext(node, strText);
        }

        /// <summary>
        /// 获取单个节点对象
        /// </summary>
        /// <param name="xDoc"></param>
        /// <param name="xPath"></param>
        /// <returns></returns>
        public static XmlElement SelectElement(XmlDocument xDoc, string xPath)
        {
            try
            {
                if (null == xDoc || string.IsNullOrEmpty(xPath))
                {
                    return null;
                }

                return (XmlElement)xDoc.SelectSingleNode(xPath);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("获取节点异常。XPath={0}", xPath), ex);
            }
        }

        /// <summary>
        /// 获取单个节点对象
        /// </summary>
        /// <param name="el"></param>
        /// <param name="xPath"></param>
        /// <returns></returns>
        public static XmlElement SelectElement(XmlElement el, string xPath)
        {
            try
            {
                if (null == el || string.IsNullOrEmpty(xPath))
                {
                    return null;
                }

                return (XmlElement)el.SelectSingleNode(xPath);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("获取节点异常。XPath={0}", xPath), ex);
            }
        }

        /// <summary>
        /// 获取节点列表
        /// </summary>
        /// <param name="fatherEl"></param>
        /// <param name="xPath"></param>
        /// <returns></returns>
        public static XmlNodeList SelectNodes(XmlElement fatherEl, string xPath)
        {
            try
            {
                if (null == fatherEl || string.IsNullOrEmpty(xPath))
                {
                    return null;
                }

                return fatherEl.SelectNodes(xPath);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("获取节点异常。XPath={0}", xPath), ex);
            }
        }

        /// <summary>
        /// DOM对象的数据保存
        /// </summary>
        /// <param name="xDoc"></param>
        /// <param name="strFilePath"></param>
        public static void Save(XmlDocument xDoc, string strFilePath)
        {
            try
            {
                if (null != xDoc)
                {
                    xDoc.Save(strFilePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("保存文件异常。FilePath=" + strFilePath, ex);
            }
        }

        /// <summary>
        /// 删除但前节点下的所有子节点
        /// </summary>
        /// <param name="xPath"></param>
        /// <param name="fatherEl"></param>
        public static void RemoveAllChidren(string xPath, XmlElement fatherEl)
        {
            XmlElement el = SelectElement(fatherEl, xPath);
            RemoveAllChidren(el);
        }

        /// <summary>
        /// 删除但前节点下的所有子节点
        /// </summary>
        /// <param name="el"></param>
        public static void RemoveAllChidren(XmlElement el)
        {
            if (null != el)
            {
                el.RemoveAll();
            }
        }

        /// <summary>
        /// 从DOM对象中删除指定的节点对象
        /// </summary>
        /// <param name="xDoc"></param>
        /// <param name="el"></param>
        public static void RemoveElemment(XmlDocument xDoc, XmlElement el)
        {
            try
            {
                if (null != el && null != xDoc)
                {
                    xDoc.RemoveChild(el);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("删除节点异常。Eleemnt={0}", el), ex);
            }
        }

        /// <summary>
        /// 删除指定的子节点
        /// </summary>
        /// <param name="xPath"></param>
        /// <param name="fatherEl"></param>
        public static void RemoveElemment(string xPath, XmlElement fatherEl)
        {
            XmlElement childEl = SelectElement(fatherEl, xPath);
            RemoveElemment(childEl, fatherEl);
        }

        /// <summary>
        /// 删除指定的子节点
        /// </summary>
        /// <param name="el"></param>
        /// <param name="fatherEl"></param>
        public static void RemoveElemment(XmlElement el, XmlElement fatherEl)
        {
            try
            {
                if (null != el)
                {
                    fatherEl.RemoveChild(el);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("删除节点异常。FatherElement={0} - Element={1}", fatherEl, el), ex);
            }

        }

        /// <summary>
        /// 向DocXML中插入另一个DocXml的节点
        /// sunxx 20131014
        /// </summary>
        /// <param name="xDoc"></param>
        /// <param name="rootImport"></param>
        /// <param name="strNewNodeName"></param>
        public static void InsertNewDocNode(XmlDocument xDoc, XmlElement rootImport, string strNewNodeName)
        {
            try
            {
                if (xDoc != null && rootImport != null && !string.IsNullOrEmpty(strNewNodeName))
                {
                    XmlNode newBook = xDoc.ImportNode(rootImport, true);
                    var newNode = CreateElement(xDoc, strNewNodeName);
                    newNode.AppendChild(newBook);

                    XmlElement rootEl = GetRootEl(xDoc);
                    rootEl.AppendChild(newNode);
                }
            }
            catch (Exception ex)
            {

                throw new Exception(string.Format("合并节点信息异常。XmlDocument={0} - Element={1}", xDoc, rootImport), ex);
            }
        }

    }
}
