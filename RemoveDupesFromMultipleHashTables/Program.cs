using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace RemoveDupesFromMultipleHashTables
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger("RemoveDupesFromHashTablesLog");
        
        static void Main(string[] args)
        {
            List<string> filePathList = new List<string>();

            List<string> stationList = GetListFromFile(ConfigurationManager.AppSettings["AllStationsWithProcessedFilePaths"]);
            Log.Info("Station List: " + string.Join(",", stationList));

            string filePathStructure = ConfigurationManager.AppSettings["FilePathStructure"];

            foreach (string station in stationList)
            {
                string fullPath = filePathStructure.Replace("[station]", station.ToLower());

                if (!File.Exists(fullPath))
                {
                    fullPath = fullPath.Replace(@"Tegnavision Archiver\",
                        @"Tegnavision Archiver\Completed\");
                }

                filePathList.Add(fullPath);
            }

            Dictionary<string, string> allProcessedHtDictionary = new Dictionary<string, string>();

            foreach (string file in filePathList)
            {
                Log.Info("Getting processed list from " + file);
                Dictionary<string, string> thisStationsDictionary = ReadDictionaryItems(file);
                Dictionary<string, string> uniqueValuesToWriteToFile = new Dictionary<string, string>();

                foreach (KeyValuePair<string, string> kvp in thisStationsDictionary)
                {
                    if (!allProcessedHtDictionary.ContainsKey(kvp.Key))
                    {
                        allProcessedHtDictionary.Add(kvp.Key, kvp.Value);
                        uniqueValuesToWriteToFile.Add(kvp.Key, kvp.Value);
                    }
                    else
                    {
                        Log.Info("Key " + kvp.Key + " was already in another Hash Table. Deleting from " + file);
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
                        if (!table.ContainsKey(keyValuePair[0]) && !string.IsNullOrEmpty(keyValuePair[1]))
                        {
                            table.Add(keyValuePair[0], keyValuePair[1]);
                        }
                        else
                        {
                            Log.Error("Hashtable already contains key " + keyValuePair[0].ToString());
                        }
                        if (string.IsNullOrEmpty(keyValuePair[1]))
                        {
                            Log.Error("The key " + keyValuePair[0] + " does not have an associated value.");
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Error Reading Dictionary Items from " + location);
                }

            }
            else
            {
            }
            Log.Info("Found " + table.Count + " Items from " + location);
            return table;
        }



        public static List<string> GetListFromFile(string filePath)
        {
            Log.Info("Trying to get station list from " + filePath);
            List<string> newList = new List<string>();
            
            if (File.Exists(filePath))
            {
                try
                {
                    using (StreamReader r = new StreamReader(filePath))
                    {
                        string line;
                        while ((line = r.ReadLine()) != null)
                        {
                            newList.Add(line);
                        }
                    }
                    int numberOfItems = newList.Count();
                    if (numberOfItems == 0)
                    {
                        Log.Info("No items in " + filePath);
                    }
                    else
                    {
                        Log.Info("Found " + numberOfItems + " from " + filePath);
                    }
                    return newList;
                }
                catch (Exception ex)
                {
                    Log.Error("Could not generate list of items from the selected file. Error: " + ex.Message +
                              ", Inner Exception:" + ex.InnerException);
                    return newList;
                }
            }
            return newList;

        } 


    }
}
