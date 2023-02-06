using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;

namespace JinrikiVocaloidVBHelper.FileOperation
{
    /// <summary>
    /// XML 序列化助手
    /// </summary>
    public class XmlSerialize
    {

        private static DataContractSerializer GetSerializer<T>()
        {
            return new DataContractSerializer(typeof(T));
        }

        /// <summary>
        /// 将对象序列化为 XML 文本
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToXML<T>(T obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            using (var ms = new MemoryStream())
            {
                using (XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8) { Formatting = Formatting.Indented })
                {
                    GetSerializer<T>().WriteObject(writer.BaseStream, obj);
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }

        /// <summary>
        /// 将 XML 文本反序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlStr">XML 文本</param>
        /// <returns></returns>
        public static T ToObject<T>(string xmlStr)
        {
            try
            {
                var mySerializer = new XmlSerializer(typeof(T));
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xmlStr)))
                {
                    using (var sr = new StreamReader(ms, Encoding.UTF8))
                    {
                        return (T)GetSerializer<T>().ReadObject(sr.BaseStream);
                    }
                }
            }
            catch (Exception e)
            {
                //return default(T);
                throw e;
            }
        }
    }
}
