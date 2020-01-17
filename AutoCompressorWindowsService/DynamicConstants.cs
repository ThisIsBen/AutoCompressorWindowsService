using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCompressorWindowsService
{
    class DynamicConstants
    {
        //to record the time interval for checking whether compression time comes
        public static int checkCompressionTimeInterval = 60 * 1000;

        //to record the txt file that contains the user's settings of the AutoCompressorWindowsService
        //public static string userAutoCompressorSettingsTxtFile = @"C:\Users\KNK07529\Desktop\AutoCompressorWindowsService\ユーザー操作\自動圧縮設定.txt";
        //public static string userAutoCompressorSettingsTxtFile = @"C:\Users\N180742\Desktop\AutoCompressorWindowsService\ユーザー操作\自動圧縮設定.txt";
        public static string userAutoCompressorSettingsTxtFile = @"C:\Users\KNK09087\Desktop\AutoCompressorWindowsService\ユーザー操作\自動圧縮設定.txt";
        //public static string userAutoCompressorSettingsTxtFile = @"C:\Program Files\AutoCompressorWindowsService\ユーザー操作\自動圧縮設定.txt";



        //to record the txt file that contains the user's settings of the AutoCompressorWindowsService
        //public static string backupDictJSONFile = @"C:\Users\KNK07529\Desktop\AutoCompressorWindowsService\ユーザー操作\圧縮済みフォルダー記録.json";
        //public static string backupDictJSONFile = @"C:\Users\N180742\Desktop\AutoCompressorWindowsService\ユーザー操作\圧縮済みフォルダー記録.json";
        public static string backupDictJSONFile = @"C:\Users\KNK09087\Desktop\AutoCompressorWindowsService\ユーザー操作\圧縮済みフォルダー記録.json";
        //public static string backupDictJSONFile = @"C:\Program Files\AutoCompressorWindowsService\ユーザー操作\圧縮済みフォルダー記録.json";
        /*
        //to record the txt file that contains the user's settings of the AutoCompressorWindowsService
        public static string backupDictXMLFile = @"C:\Users\KNK09087\source\repos\AutoCompressorWindowsService\ユーザー操作\圧縮済みフォルダー記録.xml";
        */





    }
}
