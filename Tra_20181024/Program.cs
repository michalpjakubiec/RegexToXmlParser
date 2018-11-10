using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Tra_20181024
{
    class Program
    {
        static void Main(string[] args)
        {
            const string filePath = "t_1a.txt";

            RegexToXmlParser parser = new RegexToXmlParser();

            const string pattern = @"(?:{""id"":""|^\s*)(?<id>\d{1,10})(?:"",""product_code"":""|,|\s*)(?<product_code>[A-Z-\d]{1,64})(?:"",""product_name"":""|,|\s*|)(?<product_name>(?:\s?[A-Za-z]){1,255})(?:"",""standard_cost"":""|,|\s*)(?<standard_cost>[\d.]{1,9})(?:"",""list_price"":""|,|\s*)(?<list_price>[\d.]{1,9})(?:"",""reorder_level"":""|,|\s*)(?<reorder_level>\d{1,4})(?:"",""target_level"":""|,|\s*)(?<target_level>\d{1,4})(?:"",""quantity_per_unit"":""|,|\s*)(?<quantity_per_unit>[\d\s-A-Za-z.]{1,128}(?<!\d|\s))(?:"",""discontinued"":""|,|\s*)(?<discontinued>\d{1})(?:"",""minimum_reorder_quantity"":""|,|\s*)(?<minimum_reorder_quantity>\d{1,3})(?:"",""category"":""|,|\s*)(?<category>[A-Za-z&\s]{1,128})(?:""}|,|$)";

            Regex regex = new Regex(pattern);

            var input = LoadLinesFromFile(filePath);
            List<string> groupsNamesList = GetGroupsNamesAsList(regex);

            int errorCount = 0;

            for (int i = 0; i < input.Count(); i++)
            {
                Match match = regex.Match(input[i]);

                if (match.Success)
                {
                    var parsedLine = MatchGroupsInLine(regex, match);

                    if (parsedLine.Count == groupsNamesList.Count)
                    {
                        parser.AddRecord(parsedLine, groupsNamesList);
                    }
                    else
                    {
                        Console.WriteLine($"ERROR \t the number of records does not match the number of categories in line {i + 1}");
                        errorCount += 1;
                    }
                }
                else
                {
                    Console.WriteLine($"ERROR \t could not parse line  {i + 1}");
                    errorCount += 1;
                }
            }

            Console.WriteLine($"Successfully parsed {input.Count - errorCount}/{input.Count} lines");

            parser.ExportAsXml("output.xml");

            Console.WriteLine();
            Console.Write(@"Do you want to view the output file? y\n: ");

            var choice = Console.Read();

            if (choice == 121 | choice == 89)
            {
                Process.Start("output.xml");
            }
        }

        private static List<string> LoadLinesFromFile(string fileName)
        {
            IEnumerable<string> input = new List<string>();

            if (File.Exists(fileName))
            {
                input = File.ReadLines(fileName);
            }
            else
            {
                Console.WriteLine("Input file not found. Quitting...");
                Environment.Exit(0);
            }

            return input.ToList();
        }

        private static List<string> GetGroupsNamesAsList(Regex r)
        {
            List<string> groupsNamesList = new List<string>();
            string[] groupNames = r.GetGroupNames();

            foreach (var name in groupNames)
            {
                if (name.All(char.IsDigit) == false)
                {
                    groupsNamesList.Add(name);
                }
            }

            return groupsNamesList;
        }

        private static List<string> MatchGroupsInLine(Regex r, Match m)
        {
            var parsedRecordsList = new List<string>();
            string[] groupNames = r.GetGroupNames();

            foreach (var name in groupNames)
            {
                Group group = m.Groups[name];

                if (name.All(char.IsDigit) == false)
                {
                    var value = group.Value.TrimStart();
                    parsedRecordsList.Add(value);
                    //Console.WriteLine($"\t{name,-50}{value,-60}");
                }
            }
            //Console.WriteLine();

            return parsedRecordsList;
        }
    }
}