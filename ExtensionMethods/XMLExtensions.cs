using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ExtensionMethods
{
    public static class XMLExtensions
    {
        /// <summary>
        /// XML is hard!  At least the methods are.  XmlElement, XmlDocument, XmlNode.
        /// Most of the time I'm setting true or false, so make a helper.
        /// </summary>
        /// <param name="targetNode"></param>
        /// <param name="targetDoc"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        public static void SetAttributeOn(this XmlNode targetNode, XmlDocument targetDoc, string attributeName)
        {
            XmlAttribute newOrExistingAttribute = targetDoc.CreateAttribute(attributeName);
            newOrExistingAttribute.Value = "True";
            targetNode.Attributes.SetNamedItem(newOrExistingAttribute);
        }

        public static void SetAttribute(this XmlNode targetNode, XmlDocument targetDoc, string attributeName, string attributeValue)
        {
            XmlAttribute newOrExistingAttribute = targetDoc.CreateAttribute(attributeName);
            newOrExistingAttribute.Value = attributeValue;
            targetNode.Attributes.SetNamedItem(newOrExistingAttribute);
        }
    }
}
