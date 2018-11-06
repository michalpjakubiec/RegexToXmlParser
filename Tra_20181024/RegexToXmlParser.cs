using System.Collections.Generic;
using System.Xml;

namespace Tra_20181024
{
    public class RegexToXmlParser
    {
        public XmlDocument XmlDoc { get; set; }
        public XmlNode RootNode { get; set; }
        public RegexToXmlParser()
        {
            XmlDoc = new XmlDocument();
            RootNode = XmlDoc.CreateElement("products_list");
            XmlDoc.AppendChild(RootNode);
        }

        public void AddRecord(List<string> matchedRecordsList, List<string> categoriesList)
        {
            var newElement = XmlDoc.CreateElement("product");
            RootNode.AppendChild(newElement);

            for (int i = 0; i < matchedRecordsList.Count; i++)
            {
                XmlNode category = XmlDoc.CreateElement(categoriesList[i]);
                category.InnerText = matchedRecordsList[i];
                newElement.AppendChild(category);
            }
        }

        public void ExportAsXml(string fileName)
        {
            XmlDoc.Save(fileName);
        }
    }
}
