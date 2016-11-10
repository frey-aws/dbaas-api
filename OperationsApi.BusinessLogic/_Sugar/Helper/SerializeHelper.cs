using System;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using Newtonsoft.Json;

namespace OperationsApi.BusinessLogic
{
    /// <summary>
    /// SerializeHelper:  Why NewtonSoft?  It's faster, easier to use, and has more options than standard .NET DataContractSerializer.
    /// </summary>
    public static class SerializeHelper
    {
        #region JSON

        #region Serialize

        public static T GetObject<T>(string stringIn)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(stringIn);
            }
            catch (Exception exception)
            {
                // ExceptionHelper.Capture(exception);
                return Activator.CreateInstance<T>();       // return an empty object
            }
        }

        #endregion

        #region Deserialize

        public static string GetJson<T>(T t)
        {
            try
            {
                return JsonConvert.SerializeObject(t);
            }
            catch (Exception exception)
            {
                // ExceptionHelper.Capture(exception);
                return "Serialization failed. " + exception.Message;
            }
        }

        #endregion

        #endregion

        #region XML

        public static string GetJsonFromXml(string xml)
        {
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.LoadXml(xml);
            }
            catch (Exception exception)
            {
                // ExceptionHelper.Capture(exception);
                return "Serialization failed. " + exception.Message;
            }

            return JsonConvert.SerializeXmlNode(doc);
        }

        public static XmlDocument GetXmlFromJson(string json)
        {
            XmlDocument doc = new XmlDocument();

            try
            {
                return (XmlDocument)JsonConvert.DeserializeXmlNode(json);
            }
            catch (Exception exception)
            {
                // ExceptionHelper.Capture(exception);                
            }
            return doc;
        }

        public static string GetXmlString<T>(T t)
        {
            string xmlResult = string.Empty;

            try
            {
                using (StringWriter sw = new StringWriter())
                {
                    XmlWriter writer = XmlWriter.Create(sw);

                    XmlSerializer serializer = new XmlSerializer(t.GetType());
                    serializer.Serialize(writer, t);
                    xmlResult = writer.ToString();

                }
            }
            catch (Exception exception)
            {
                // ExceptionHelper.Capture(exception);
                return "Serialization failed. " + exception.Message;
            }

            return xmlResult;
        }

        public static XmlDocument GetXmlDocument<T>(T t)
        {
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.LoadXml(GetXmlString(t));
            }
            catch (Exception exception)
            {
                // ExceptionHelper.Capture(exception);                                
            }

            return doc;
        }

        #endregion
    }
}
