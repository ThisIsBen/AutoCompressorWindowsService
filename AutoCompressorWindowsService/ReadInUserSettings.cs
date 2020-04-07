using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutoCompressorWindowsService
{
    class ReadInUserSettings
    {
        
        
        
        //user AutoCompressor settings(These settings will be read from a txt file)
        private string targetFolder ;
        private string ZIPStorageFolder ;
        private string compressionTime ;
        private string folderOverNDays ;
        private string freeSpaceLimit ;
        private string NASDriveName;
        private string whetherDeleteFolderAfterCompress;

        private static string pauseStartTime="";
        private static string pauseEndTime="";


        //Assign the values read from the 自動圧縮設定.txt for the settings of the AutoCompressor
        public ReadInUserSettings()
        {
            
            //read in the user's AutoCompressor settings from a txt file
            List<string> userAutoCompressorSettingsList = readInUserSettingsFromTxt();
            targetFolder = userAutoCompressorSettingsList[0];
            ZIPStorageFolder = userAutoCompressorSettingsList[1];
            compressionTime = userAutoCompressorSettingsList[2];
            folderOverNDays = userAutoCompressorSettingsList[3];
            freeSpaceLimit = userAutoCompressorSettingsList[4];
            NASDriveName = userAutoCompressorSettingsList[5];
            whetherDeleteFolderAfterCompress = userAutoCompressorSettingsList[6];


            if (userAutoCompressorSettingsList.Count > 8)
            {
                //毎日何時に圧縮一時停止するのか（24時間制）
                pauseStartTime = userAutoCompressorSettingsList[7];
            
          
                //毎日何時に圧縮再開するのか（24時間制）
                pauseEndTime = userAutoCompressorSettingsList[8];
            }
            



        }

        

        //Read in the user's AutoCompressor settings from the 自動圧縮設定.txt 
        public List<string> readInUserSettingsFromTxt()
        {
            StreamReader input = new StreamReader(DynamicConstants.userAutoCompressorSettingsTxtFile);
            String line = null;
            List<string> txtFileContentList = new List<string>();
            List<string> userAutoCompressorSettingsList = new List<string>();

            do
            {
                line = input.ReadLine();

                //when the reader reaches the end of the file
                if (line == null)
                {
                    break;
                }

                //ignore white space
                if (line == String.Empty || Regex.Replace(line, @"\s", "") == "")
                {
                    continue;
                }

                //trim the white space in the front and in the back of a non-empty line
                txtFileContentList.Add(line.Trim());


            } while (true);

            //store the content read from the file to a list
            for (int i = 1; i < txtFileContentList.Count; i = i + 2)
            {

                userAutoCompressorSettingsList.Add(txtFileContentList[i]);

            }

            return userAutoCompressorSettingsList;


        }


        //class member variables getter

        //Return the value of 圧縮する目標フォルダー
        public string getTargetFolder
        {
            
            get { return this.targetFolder; }
        }

        //Return the value of 圧縮されたZIPフォルダーの保存ところ
        public string getZIPStorageFolder
        {

            get { return this.ZIPStorageFolder; }
        }

        //Return the value of 毎日何時に圧縮するか（24時間制）
        public string getCompressionTime
        {

            get { return this.compressionTime; }
        }

        //Return the value of 何日前のデータを圧縮して保存するか
        public string getFolderOverNDays
        {

            get { return this.folderOverNDays; }
        }

        //Return the value of NASの空き容量のアラーム（単位：GB）
        public string getFreeSpaceLimit
        {

            get { return this.freeSpaceLimit; }
        }

        //Return the value of NASの空き容量確認ドライブ(例：　C:\)
        public string getNASDriveName
        {

            get { return this.NASDriveName; }
        }

        //Return the value of 圧縮して保存したら、自動でフォルダーを削除するか (Yes or No を入力してください)
        public string getWhetherDeleteFolderAfterCompress
        {

            get { return this.whetherDeleteFolderAfterCompress; }
        }
        //Return the value of 圧縮して保存したら、自動でフォルダーを削除するか (Yes or No を入力してください)
        public static string getUserSpecifiedPauseEndTime
        {

            get { return pauseEndTime; }
        }

        public static string getUserSpecifiedPauseStartTime
        {

            get { return pauseStartTime; }
        }
        public static bool IsPauseTimeSet()
        {
            if (pauseStartTime != "" && pauseEndTime != "")
            {
                return true;
            }

            return false;
        }

        //Return the value of 毎日何時に圧縮一時停止するのか（24時間制）
        public static DateTime getPauseStartTime()
        {
             
                string[] pauseStartTimeStr = pauseStartTime.Split(':');
                int pauseStartTimeHour = int.Parse(pauseStartTimeStr[0]);
                int pauseStartTimeMinute = int.Parse(pauseStartTimeStr[1]);

                //compose the pause time set by the user as TimeSpan 
                TimeSpan pauseStartTimeSpan = new TimeSpan(pauseStartTimeHour, pauseStartTimeMinute, 0);

                DateTime pauseStartDateTime = DateTime.Today + pauseStartTimeSpan;

                return pauseStartDateTime;
           

        }
        //Return the time period that the program should pause
        public static TimeSpan getPauseTimePeriod()
        {
            
                string[] pauseEndTimeStr = pauseEndTime.Split(':');
                int pauseEndTimeHour = int.Parse(pauseEndTimeStr[0]);
                int pauseEndTimeMinute = int.Parse(pauseEndTimeStr[1]);

                //compose the resume time set by the user as TimeSpan 
                TimeSpan pauseEndTimeSpan = new TimeSpan(pauseEndTimeHour, pauseEndTimeMinute, 0);


                TimeSpan now = DateTime.Now.TimeOfDay;

                DateTime pauseEndDateTime;
                if (now.Hours > pauseEndTimeSpan.Hours)
                {
                    pauseEndDateTime = DateTime.Today.AddDays(1) + pauseEndTimeSpan;
                }
                else
                {
                    pauseEndDateTime = DateTime.Today + pauseEndTimeSpan;
                }



                TimeSpan pauseTimePeriod = pauseEndDateTime - DateTime.Now;



                return pauseTimePeriod;
           
        }
}
}
