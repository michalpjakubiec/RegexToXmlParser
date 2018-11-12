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
            bool verbose = false;

            const string filePath = "t_1a.txt";
            const string pattern = @"(?:{""id"":""|^\s*)(?<id>\d{1,10})" +
                                   @"(?:"",""product_code"":""|,|\s*)(?<product_code>[A-Z-\d]{1,64})" +
                                   @"(?:"",""product_name"":""|,|\s*|)(?<product_name>(?:\s?[A-Za-z]){1,255})" +
                                   @"(?:"",""standard_cost"":""|,|\s*)(?<standard_cost>[\d.]{1,9})" +
                                   @"(?:"",""list_price"":""|,|\s*)(?<list_price>[\d.]{1,9})" +
                                   @"(?:"",""reorder_level"":""|,|\s*)(?<reorder_level>\d{1,4})" +
                                   @"(?:"",""target_level"":""|,|\s*)(?<target_level>\d{1,4})" +
                                   @"(?:"",""quantity_per_unit"":""|,|\s*)(?<quantity_per_unit>[\d\s-A-Za-z.]{1,128}(?<!\d|\s))" +
                                   @"(?:"",""discontinued"":""|,|\s*)(?<discontinued>\d{1})" +
                                   @"(?:"",""minimum_reorder_quantity"":""|,|\s*)(?<minimum_reorder_quantity>\d{1,3})" +
                                   @"(?:"",""category"":""|,|\s*)(?<category>[A-Za-z&\s]{1,128})(?:""}|,|$)";

            //const string patternCsv = @"(?<id>\d+),(?<product_code>[A-Z-\d]+),(?<product_name>[A-Za-z\s]+),(?<standard_cost>[\d.]+),(?<list_price>[\d.]+),(?<reorder_level>\d+),(?<target_level>\d+),(?<quantity_per_unit>[\d\s-A-Za-z.]+),(?<discontinued>\d),(?<minimum_reorder_quantity>\d+),(?<category>[A-Za-z\s&]+),"
            //const string patternColumns = @"(?<id>[\s\d]{10})(?<product_code>[[\sA-Z-\d-]{64})(?<product_name>[\sA-Za-z]{255})(?<standard_cost>[\s\d.]{9})(?<list_price>[\s\d.]{9})(?<reorder_level>[\s\d]{4})(?<target_level>[\s\d]{4})(?<quantity_per_unit>[\d\s-A-Za-z.]{128})(?<discontinued>\d{1})(?<minimum_reorder_quantity>[\d\s]{3})(?<category>[A-Za-z\s&]{128})"

            Regex regex = new Regex(pattern);

            var input = ReadLinesToList(filePath);
            var groupsNamesList = regex.GetGroupNames().Where(x => x != "0").ToList();

            XmlHelper xmlHelper = new XmlHelper(groupsNamesList, "products_list");

            int errorCount = 0;

            for (int i = 0; i < input.Count(); i++)
            {
                Match match = regex.Match(input.ElementAt(i));

                if (match.Success)
                {
                    Console.Write(verbose == true ? $"Match {i + 1}\n" : "");

                    var parsedMatch = new List<string>();

                    for (int j = 1; j < match.Groups.Count; j++)
                    {
                        Console.Write(verbose == true ? $"\t{match.Groups[j]}\n" : "");
                        parsedMatch.Add(match.Groups[j].Value);
                    }

                    xmlHelper.AppendParsedMatch(parsedMatch, "product");
                }
                else
                {
                    Console.WriteLine($"ERROR: could not parse line {i + 1}");
                    errorCount++;
                }
            }

            Console.WriteLine();
            Console.WriteLine($"Successfully parsed {input.Count() - errorCount}/{input.Count()} lines.");
            Console.Write(errorCount != 0 ? $"There was {errorCount} error(s).\n" : "");

            xmlHelper.ExportXml("output.xml");

            Console.WriteLine();
            Console.Write(@"Do you want to view the output file? y/n: ");

            if (Console.ReadKey().Key == ConsoleKey.Y)
                Process.Start("output.xml");
        }

        private static IList<string> ReadLinesToList(string fileName)
        {
            IList<string> input = new List<string>();

            if (File.Exists(fileName))
            {
                input = File.ReadLines(fileName).ToList();
            }
            else
            {
                Console.WriteLine("Input file not found. Quitting...");
                Environment.Exit(2);
            }

            return input;
        }
    }
}