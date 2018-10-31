using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace Tra_20181024
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = "input.txt";

            var input = LoadLinesFromFile(fileName);

            string pattern = "(\\d?,|[\\s\\d]{10})([A-Z-\\d]+,|[[\\sA-Z-\\d-]{64})([A-Za-z\\s]+,|[\\sA-Za-z]{255})" +
                             "([\\d.]+,|.{9})([\\d.]+,|.{9})(\\d+,|.{4})(\\d+,|.{4})([\\d\\s-A-Za-z.]+,|.{128})" +
                             "(\\d,|\\d{1})(\\d+,|[\\d\\s]{3})(.+,|.{128})";

            List<string> categoriesList = new List<string>
            {
                "id","product_code","product_name","standard_cost","list_price","reorder_level","target_level",
                "quantity_per_unit","discontinued","minimum_reorder_quantity","category"
            };

            var regexToXmlParser = new RegexToXmParser();
            Regex regex = new Regex(pattern);

            int errorCount = 0;

            for (int i = 0; i < input.Count(); i++)
            {
                Match match = regex.Match(input[i]);

                if (match.Success)
                {
                    var parsedLine = ShowMatches(regex, match);

                    if (parsedLine.Count == categoriesList.Count)
                    {
                        regexToXmlParser.AddRecord(parsedLine, categoriesList);
                    }
                    else
                    {
                        Console.WriteLine($"ERROR \t matched records does not match categories count in line {i + 1}");
                        errorCount += 1;
                    }
                }
                else
                {
                    Console.WriteLine($"ERROR \t could not parse line  {i + 1}");
                    errorCount += 1;
                }
            }

            regexToXmlParser.ExportAsXml("output.xml");
            Console.WriteLine($"\nSuccessfully parsed {input.Count - errorCount}//{input.Count} lines");
        }

        private static List<string> LoadLinesFromFile(string fileName)
        {
            IEnumerable<string> input = new List<string>();

            try
            {
                input = File.ReadLines(fileName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return input.ToList();
        }

        private static List<string> ShowMatches(Regex r, Match m)
        {
            var parsedRecordsList = new List<string>();


            foreach (var group in r.GetGroupNames())
            {
                Group g = m.Groups[group];

                if (group != "0")
                {
                    var value = g.Value.TrimStart().TrimEnd(',');
                    //Console.WriteLine("\t{0}:\t{1}", group, value);
                    parsedRecordsList.Add(value);
                }
            }

            return parsedRecordsList;
        }
    }

    public class RegexToXmParser
    {
        public XmlDocument XmlDoc { get; set; }
        public XmlNode RootNode { get; set; }
        public RegexToXmParser()
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
