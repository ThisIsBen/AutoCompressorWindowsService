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


                if (line == null)
                {
                    break;
                }

                if (line == String.Empty || Regex.Replace(line, @"\s", "") == "")
                {
                    continue;
                }

                //handle non-empty line
                txtFileContentList.Add(line.Trim());


            } while (true);


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

    }
}
