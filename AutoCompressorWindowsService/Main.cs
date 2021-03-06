﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;

using System.Threading.Tasks;
using System.Timers;
using System.Threading;

namespace AutoCompressorWindowsService
{
    public partial class Main : ServiceBase
    {
        //create timer 
        private System.Timers.Timer timer;
        //create AutoCompressor object
        private AutoCompressor autoCompressorObj;
        //create ReadInUserSettings object
        private ReadInUserSettings readInUserSettings;



       

        //It is used to accumulate the status of all the folders that go through the compression process
        //We will output it to the イベントビューアー.
        //As a result the manager can check which folder has been compressed and 
        //which folder has been deleted.
        public static string folderStatusAfterCompressLog = "";



        public Main()
        {
            InitializeComponent();

            this.AutoLog = false;

           
            // Create an EventLog for monitoring this app
            EventLogHandler.createEventlog("AutoCompressorWindowsServiceSource", "AutoCompressorWindowsServiceLog");

        }



        //Set up a timer that ticks every 1 minute.        
        protected override void OnStart(string[] args)
        {
            //set up AutoCompressor object
            autoCompressorObj = new AutoCompressor();

            //set up ReadInUserSettings object and 
            //read in the user's AutoCompressor settings from a txt file
            readInUserSettings = new ReadInUserSettings();

            //When the AutoCompressorWindowsService is activated,
            //recover the content of the Dictionary that records 
            //which folder has been compressed 
            recoverDictFromFile();

            //set up timer
            timer = new System.Timers.Timer();

            timer.Elapsed += new ElapsedEventHandler(AutoFolderCompressorTimer_Elapsed);

            timer.Interval = DynamicConstants.checkCompressionTimeInterval;

            timer.Start();


           


           
        }

        //When the AutoCompressorWindowsService is deactivated,
        //Stop and reset the timer
        protected override void OnStop()
        {

            timer.Stop();

            timer = null;

        }

        //Avoid overlapping timer calls when timer interval is set to less than 1 minute 
        private static readonly object _lockObject = new object();

        //Every time the timer ticks, 
        //Check whether the compression time set by the user comes
        //If yes,
        //Compress folders according to the user's settings
        //Check the free disk space 
        private void AutoFolderCompressorTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Stop the timer for checking whether compression time has come
           
            //If we do not stop the timer, the program will start again
            //before all the compression process ends. 
            //Some task will be done again and maybe some error will occur.
            timer.Stop();



            //Step 2 Activate the compression automatically 
            //according to the compression time set by the user

            //Lock the timer tick callback function to 
            //avoid overlapping timer calls 
            //when timer interval is set to less than 1 minute 
            lock (_lockObject)
            {

               

                
                    //Check whether the compression time set by the user comes
                    if (IsCompressionTime())
                    {

                       
                       






                        //If the user set圧縮して保存したら、自動でフォルダーを削除するか (Yes or No を入力してください):
                        //to be "Yes" in the 自動圧縮設定.txt,
                        //we delete the original folder after compression
                        if (whetherDeleteAfterCompress() == true)
                        {
                            //Compress folders according to the user's settings
                            compressFolderAccordingToSettings("DeleteAfterCompress");

                        }

                        //If the user set圧縮して保存したら、自動でフォルダーを削除するか (Yes or No を入力してください):
                        //to be "No" in the 自動圧縮設定.txt,
                        //we do not do anything after compression
                        else
                        {
                            //Compress folders according to the user's settings
                            compressFolderAccordingToSettings();

                        }




                        //Check the free disk space 
                        checkFreeDiskSpace();


                        //Every time after AutoCompressorWindowsService finishes all its work,
                        //back up the content of the Dictionary that records 
                        //which folder has been compressed to 圧縮済みフォルダー記録.json              
                        backupDictToFile();


                        EventLogHandler.outputLog("今日の圧縮全部完成しました。\n");

                        //sleep until 10 seconds before the next compression time
                        Thread.Sleep(sleepUntilNextDayCompression());

                       


                    }
                
                

            }

            //Start the timer again after all the compression process finishes
            timer.Start();
            
        }

        //Check whether the user wants to delete folders automatically after AutoCompressorWindowsService compresses the folders.
        private bool whetherDeleteAfterCompress()
        {
           
            if (readInUserSettings.getWhetherDeleteFolderAfterCompress.ToUpper() == "YES")
            {
                return true;
            }

            return false;
        }

        TimeSpan compressionTime;
        //Check whether the compression time set by the user comes
        private bool IsCompressionTime()
        {
            
            string[] userCompressionTime = readInUserSettings.getCompressionTime.Split(':');
            int userCompressionTimeHour = int.Parse(userCompressionTime[0]);
            int userCompressionTimeMinute = int.Parse(userCompressionTime[1]);

            //compose the compression time set by the user as TimeSpan 
            compressionTime = new TimeSpan(userCompressionTimeHour, userCompressionTimeMinute, 0); //10 o'clock
            //get current time
            TimeSpan now = DateTime.Now.TimeOfDay;
           
            //Check whether it is compression time (Hour,Second) now
            //if yes, return true
            if ((compressionTime.Hours == now.Hours) && (compressionTime.Minutes == now.Minutes))
            {
                return true;

            }

            return false;
        }

        //sleep until 10 seconds before the next compression time
        public TimeSpan sleepUntilNextDayCompression()
        {

            DateTime nextCompressionTime;

            TimeSpan timeSpanUntilNextCompression;

            //if the compression took more a day to finish,
            //and the next compression time is later than current time on the same day.
            if (DateTime.Today+compressionTime >= DateTime.Now)
            {
                 nextCompressionTime = DateTime.Today + compressionTime;
                 
            }

            //if the compression finished within a day,
            //and current time is earlier than the next compression time on the next day.
            else
            {
                nextCompressionTime = DateTime.Today.AddDays(1) + compressionTime;

            }

            //calculate the sleep duration to sleep until 10 seconds before the next compression time
            timeSpanUntilNextCompression = nextCompressionTime.AddSeconds(-10) - DateTime.Now;

            return timeSpanUntilNextCompression;

        }


        //Compress folders according to the user's settings
        private void compressFolderAccordingToSettings(string deleteAfterCompressOption="")
        {
            autoCompressorObj.compressFolder(readInUserSettings.getTargetFolder, readInUserSettings.getZIPStorageFolder,readInUserSettings.getFolderOverNDays, deleteAfterCompressOption);

            
            
            

        }

        //Check the free disk space 
        private void checkFreeDiskSpace()
        {
            double remainingFreeDiskSpace = autoCompressorObj.getDiskFreeSpace(readInUserSettings.getNASDriveName);
            //Output the log to record that the remaining free space of the disk has been checked
            string checkedNASDriveName=readInUserSettings.getNASDriveName.Split(':')[0];
             EventLogHandler.outputLog(checkedNASDriveName+"ドライブ空き容量を確認しました。");

            if (remainingFreeDiskSpace < Convert.ToDouble(readInUserSettings.getFreeSpaceLimit))
            {

                showMsgBoxFromWS(checkedNASDriveName + "ドライブ空き容量不足。フォルダーを圧縮して保存後、残り" + remainingFreeDiskSpace.ToString() + "GB", "Message from AutoCompressorWindowsService");
                //Output the log to record that the remaining free space of the disk is not enough
                 EventLogHandler.outputLog(checkedNASDriveName + "ドライブ空き容量不足。フォルダーを圧縮して保存後、残り" + remainingFreeDiskSpace.ToString() + "GB");

            }

        }







        //Recover the content of the Dictionary from 圧縮済みフォルダー記録.json that records 
        //which folder has been compressed
        public void recoverDictFromFile()
        {



            /*
            //recover 圧縮済みフォルダーの記録 to a dictionary from a xml file
            Backup_RecoverDict.recoverDictFromXMLFile(DynamicConstants.backupDictXMLFile, autoCompressorObj.compressedFolderNameDict);
            */
            //recover 圧縮済みフォルダーの記録 to a dictionary from a json file(圧縮済みフォルダー記録.json)
            autoCompressorObj.compressedFolderNameDict=Backup_RecoverDict.recoverDictFromJSONFile(DynamicConstants.backupDictJSONFile);

        }


        //Back up the content of the Dictionary that records 
        //which folder has been compressed to 圧縮済みフォルダー記録.json
        public void backupDictToFile()
        {
            /*
            //Save the content of the 圧縮済みフォルダーの記録dictionary to a xml file
            Backup_RecoverDict.backupDictToXMLFile(DynamicConstants.backupDictXMLFile, autoCompressorObj.compressedFolderNameDict);
            */

            //Save the content of the 圧縮済みフォルダーの記録dictionary to a json file(圧縮済みフォルダー記録.json)
            Backup_RecoverDict.backupDictToJSONFile(DynamicConstants.backupDictJSONFile, autoCompressorObj.compressedFolderNameDict);
        }



        //Clear the content of the folderStatusAfterCompressLog
        public static void resetFolderStatusAfterCompressLog()
        {
            folderStatusAfterCompressLog = "";
        }




        //Stop the 圧縮ソフト（AutoCompressorWindowsService）：
        //Stop the Windows Service and it waits until the service is stopped or a timeout(60sec) occurs.
        public static void stopWindowsService(string serviceName)
        {
            EventLogHandler.outputLog("エラーが発生しましたので、AutoCompressorWindowsServiceは停止されました。");

            //timeout for stopping the Windows Service
            //Force shut down the Windows Service after this timeout.
            int timeoutMilliseconds = 60 *1000;

            ServiceController service = new ServiceController(serviceName);
           
            TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

            service.Stop();
            service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

           

        }

      

        public static IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;
        
        //Display GUI message window 
        public static void showMsgBoxFromWS(string message, string title) { int resp = 0; WTSSendMessage(WTS_CURRENT_SERVER_HANDLE, WTSGetActiveConsoleSessionId(), title, title.Length, message, message.Length, 0, 0, out resp, false); }
        [DllImport("kernel32.dll", SetLastError = true)] public static extern int WTSGetActiveConsoleSessionId();[DllImport("wtsapi32.dll", SetLastError = true)] public static extern bool WTSSendMessage(IntPtr hServer, int SessionId, String pTitle, int TitleLength, String pMessage, int MessageLength, int Style, int Timeout, out int pResponse, bool bWait);



    }
}
