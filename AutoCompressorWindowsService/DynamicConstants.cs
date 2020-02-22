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
        //!!!!!!Don't set checkCompressionTimeInterval less than 60 seconds!!!!!!
        //Because it will start the compression process more than once and cause some 
        //unexpected problems.
        //For example, the 容量messageBox will keep showing up.
        public static int checkCompressionTimeInterval = 60 * 1000; //interval = 1 minute

        //to record the txt file that contains the user's settings of the AutoCompressorWindowsService
        //run on 鈴木さんPC
        //public static string userAutoCompressorSettingsTxtFile = @"C:\Users\KNK07529\Desktop\AutoCompressorWindowsService\ユーザー操作\自動圧縮設定.txt";

        //public static string userAutoCompressorSettingsTxtFile = @"C:\Users\N180742\Desktop\AutoCompressorWindowsService\ユーザー操作\自動圧縮設定.txt";
        public static string userAutoCompressorSettingsTxtFile = @"C:\Users\KNK09087\Documents\siga_Lets Note PC\AutoCompressorWindowsService\ユーザー操作\自動圧縮設定.txt";

        //run on the factory PC
        //public static string userAutoCompressorSettingsTxtFile = @"C:\Users\M171183.M17-1183\Desktop\AutoCompressorWindowsService\ユーザー操作\自動圧縮設定.txt";




        //to record the txt file that contains the user's settings of the AutoCompressorWindowsService
        //run on 鈴木さんPC
        //public static string backupDictJSONFile = @"C:\Users\KNK07529\Desktop\AutoCompressorWindowsService\ユーザー操作\圧縮済みフォルダー記録.json";
        
        //public static string backupDictJSONFile = @"C:\Users\N180742\Desktop\AutoCompressorWindowsService\ユーザー操作\圧縮済みフォルダー記録.json";
        //public static string backupDictJSONFile = @"C:\Users\KNK09087\Documents\siga_Lets Note PC\AutoCompressorWindowsService\ユーザー操作\圧縮済みフォルダー記録.json";
        public static string backupDictJSONFile = @"C:\Users\KNK09087\Desktop\AutoCompressorWindowsService\ユーザー操作\圧縮済みフォルダー記録.json";

        //run on the factory PC
        //public static string backupDictJSONFile = @"C:\Users\M171183.M17-1183\Desktop\AutoCompressorWindowsService\ユーザー操作\圧縮済みフォルダー記録.json";



        //wait for files to be ready to be deleted
        //when deleting the files if the files are still in use,
        //wait for this time interval and try to delete it again.
        public static int waitReadyToBeDeleteTimeInterval = 10 * 1000;//10 second


        //毎日圧縮するフォルダ数の最大限
        public static int oneDayMaxCompressFolderNum = 12;

        //output error message to a txt file and save it to NAS
        public static string errorMessageTxtFolderPath = @"\\10.30.50.15\share\圧縮ソフト_エラーメッセージ記録";

        //Retry times when error occurs during compression.
        public static int retryTimesWhenErrOccurs = 3;

        //The time interval between 2 retries when error occurs during compression
        public static int retryTimeInterval = 15 * 1000;
        /*
        //to record the txt file that contains the user's settings of the AutoCompressorWindowsService
        public static string backupDictXMLFile = @"C:\Users\KNK09087\source\repos\AutoCompressorWindowsService\ユーザー操作\圧縮済みフォルダー記録.xml";
        */





    }
}
