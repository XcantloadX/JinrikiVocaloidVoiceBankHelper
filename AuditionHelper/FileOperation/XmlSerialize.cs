using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace JinrikiVocaloidVBHelper.FileOperation
{
    /// <summary>
    /// XML 序列化助手
    /// </summary>
    public class XmlSerialize
    {
        /// <summary>
        /// 将对象序列化为 XML 文本
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToXML(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            using (var ms = new MemoryStream())
            {
                using (XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8) { Formatting = Formatting.Indented })
                {
                    serializer.Serialize(writer, obj);
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }

        /// <summary>
        /// 将 XML 文本反序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml">XML 文本</param>
        /// <returns></returns>
        public static T ToObject<T>(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(xml.GetType());
            try
            {
                var mySerializer = new XmlSerializer(typeof(T));
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
                {
                    using (var sr = new StreamReader(ms, Encoding.UTF8))
                    {
                        return (T)mySerializer.Deserialize(sr);
                    }
                }
            }
            catch(Exception e)
            {
                //return default(T);
                throw e;
            }
        }
    }
}
