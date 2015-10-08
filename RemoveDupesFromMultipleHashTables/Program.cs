using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoveDupesFromMultipleHashTables
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> filePathList = new List<string>();

            string stationListCsv = ConfigurationManager.AppSettings["stationList"];
            List<string> stationList = stationListCsv.Split(',').ToList();

            string filePathStructure = ConfigurationManager.AppSettings["FilePathStructure"];

            foreach (string station in stationList)
            {
                string fullPath = filePathStructure.Replace("[station]", station.ToLower());
                filePathList.Add(fullPath);
            }

            Dictionary<string, string> allProcessedHtDictionary = new Dictionary<string, string>();

            foreach (string file in filePathList)
            {
                Dictionary<string, string> thisStationsDictionary = ReadDictionaryItems(file);
                Dictionary<string, string> uniqueValuesToWriteToFile = new Dictionary<string, string>();

                foreach (KeyValuePair<string, string> kvp in thisStationsDictionary)
                {
                    if (!allProcessedHtDictionary.ContainsKey(kvp.Key))
                    {
                        allProcessedHtDictionary.Add(kvp.Key, kvp.Value);
                        uniqueValuesToWriteToFile.Add(kvp.Key, kvp.Value);
                    }
                }
                File.WriteAllLines(file, uniqueValuesToWriteToFile.Select(x => x.Key + ";" + x.Value).ToArray());
            }
        }




        public static Dictionary<string, string> ReadDictionaryItems(string location)
        {
            Dictionary<string, string> table = new Dictionary<string, string>();

            if (File.Exists(location))
            {
                try
                {
                    string[] lines = File.ReadAllLines(location);
                    foreach (string line in lines)
                    {
                        string[] keyValuePair = line.Split(';');
                        if (!table.ContainsKey(keyValuePair[0]))
                        {
                            table.Add(keyValuePair[0], keyValuePair[1]);
                        }
                        else
                        {
                            //Log.Error("Hashtable already contains key " + keyValuePair[0].ToString());
                        }
                        if (string.IsNullOrEmpty(keyValuePair[1]))
                        {
                            //Log.Error("The key " + keyValuePair[0] + " does not have an associated value.");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error Reading Dictionary Items from " + location);
                }

            }
            else
            {
            }
            //Log.Info("Found " + table.Count + " processed items in the  " + location + " dictionary.");

            Console.WriteLine("Found " + table.Count + " Items from " + location);
            return table;
        }

    }
}
