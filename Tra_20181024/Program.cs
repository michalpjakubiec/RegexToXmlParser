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

            const string pattern = "(?<id>\\d+,|[\\s\\d]{10}|(?!\\\"id\\\":\\\")[\\d]+(?!\\\",))" +
                                   "(?<product_code>[A-Z-\\d]+,|[[\\sA-Z-\\d-]{64})" +
                                   "(?<product_name>[A-Za-z\\s]+,|[\\sA-Za-z]{255})" +
                                   "(?<standard_cost>[\\d.]+,|.{9})" +
                                   "(?<list_price>[\\d.]+,|.{9})" +
                                   "(?<reorder_level>\\d+,|.{4})" +
                                   "(?<target_level>\\d+,|.{4})" +
                                   "(?<quantity_per_unit>[\\d\\s-A-Za-z.]+,|.{128})" +
                                   "(?<discontinued>\\d,|\\d{1})" +
                                   "(?<minimum_reorder_quantity>\\d+,|[\\d\\s]{3})" +
                                   "(?<category>.+,|.{128})";

            Regex regex = new Regex(pattern);

            var input = LoadLinesFromFile(filePath);
            List<string> groupsNamesList = GetGroupNamesAsListFromRegex(regex);

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
            Console.Write("Do you want to view the output file? y\\n: ");

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

        private static List<string> GetGroupNamesAsListFromRegex(Regex r)
        {
            List<string> groupsNamesList = new List<string>();
            string[] names = r.GetGroupNames();

            foreach (var name in names)
            {
                if (name != "0")
                {
                    groupsNamesList.Add(name);
                }
            }

            return groupsNamesList;
        }

        private static List<string> MatchGroupsInLine(Regex r, Match m)
        {
            var parsedRecordsList = new List<string>();
            string[] names = r.GetGroupNames();

            foreach (var name in names)
            {
                Group group = m.Groups[name];

                if (name != "0")
                {
                    var value = group.Value.TrimStart().TrimEnd(',');
                    parsedRecordsList.Add(value);
                    //Console.WriteLine($"\t{name,-50}{value,-60}");
                }
            }
            //Console.WriteLine();

            return parsedRecordsList;
        }
    }
}
