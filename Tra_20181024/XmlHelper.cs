using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Tra_20181024
{
    public class XmlHelper
    {
        public XmlDocument XmlDoc { get; set; }
        public XmlNode RootNode { get; set; }
        public IList<string> CategoriesList { get; set; }

        public XmlHelper(IList<string> categoriesList, string rootName)
        {
            CategoriesList = categoriesList;

            XmlDoc = new XmlDocument();
            RootNode = XmlDoc.CreateElement(rootName);
            XmlDoc.AppendChild(RootNode);
        }

        public void AppendParsedMatch(IList<string> matchedRecordsList, string elementName)
        {
            var newElement = XmlDoc.CreateElement(elementName);
            RootNode.AppendChild(newElement);

            for (int i = 0; i < matchedRecordsList.Count(); i++)
            {
                XmlNode category = XmlDoc.CreateElement(CategoriesList.ElementAt(i));
                category.InnerText = matchedRecordsList[i];
                newElement.AppendChild(category);
            }
        }

        public void ExportXml(string fileName)
        {
            XmlDoc.Save(fileName);
        }
    }
}
