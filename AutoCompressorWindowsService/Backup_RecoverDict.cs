using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace AutoCompressorWindowsService
{
    class Backup_RecoverDict
    {
        //Recover the content from the xml file to a dictionary
        //Recover the 圧縮済みフォルダーの記録 to a dictionary
        public static void recoverDictFromXMLFile (string XMLSrcPath, Dictionary<string, string> dict)
        {
            XmlDocument xmlDoc = new XmlDocument();

            if (File.Exists(XMLSrcPath) == true)
            {
                //load all the organ of the corresponding part of body.
                xmlDoc.Load(XMLSrcPath);

                //if XML file is not empty
                if (xmlDoc.InnerText != "")
                {

                    //create XElement xDoc to keep the content of xmlDoc for locate specific tag and modify it later.
                    XElement xDoc = XElement.Load(new XmlNodeReader(xmlDoc));

                    foreach (var ele in xDoc.Elements())
                    {
                        
                        dict.Add(ele.Name.LocalName.Replace("_"," "), ele.Value);
                    }
                }
            }
        }


        //Save the content of the dictionary to a xml file
        //Save the content of the 圧縮済みフォルダーの記録dictionary to a xml file
        public static void backupDictToXMLFile(string xmlStorePath, Dictionary<string, string> dict)
        {
            
            XElement xDoc = new XElement("root",
           dict.Select(kv => new XElement(Regex.Replace(kv.Key, @"\s", "_"), kv.Value)));



            xDoc.Save(xmlStorePath);


        }

        //Recover the content from a json file to a dictionary
        //Recover the 圧縮済みフォルダーの記録 to a dictionary from a json file
        public static Dictionary<string, string> recoverDictFromJSONFile(string jsonFilePath)
        {
            var text = File.ReadAllText(jsonFilePath);
            Dictionary<string, string> jsonDict = new Dictionary<string, string>();
            return jsonDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
        }

        //Save the content of the dictionary to a json file
        //Save the content of the 圧縮済みフォルダーの記録dictionary to a json file
        public async static void backupDictToJSONFile(string jsonFilePath, Dictionary<string, string> dict)
        {




            using (StreamWriter file = new StreamWriter(jsonFilePath, false))
            {
                //轉成JSON格式
                string jsonFormatString = JsonConvert.SerializeObject(dict, Newtonsoft.Json.Formatting.Indented);
                // Can write either a string or char array
                await file.WriteAsync(jsonFormatString);
            }

        }


    }
}
