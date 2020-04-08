using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace AutoCompressorWindowsService
{
    class AutoCompressor
    {
        
        //compressedFolderName dictionary is used to record all the folder name that has been compressed.
        public Dictionary<string, string> compressedFolderNameDict = new Dictionary<string, string>();

        //to indicate if the current compression is failed
        private static bool IsCompressionFailed = false;

        //to record the error message during compression
        private static string compressionErrorMessage;

        //Compress a folder to create a ZIP file
        public void createZIPFile(string targetFolderPath,string storageFolderName,string currentFolderName)
        {

            string storagePathWithZIPFilename=storageFolderName + "\\" + currentFolderName + ".zip";
            //if there is a zip file with the same name exists,
            //do nothing to avoid overwrite old zip file
            //else, compress and create zip file as usual
            if (File.Exists(storagePathWithZIPFilename) == false)
            {


                //indicate that which folder is being compressed currently.
                EventLogHandler.outputLog(currentFolderName + ": " + "圧縮途中です。\n");

                try
                {
                    //compress the folder to a ZIP file
                    ZipFile.CreateFromDirectory(targetFolderPath, storagePathWithZIPFilename, CompressionLevel.Optimal, false);

                }

                catch (Exception e)
                {
                    //display this error message on the GUI window to inform the user
                    string errorMessage = currentFolderName + " フォルダー圧縮途中でエラーが発生しました。\n圧縮ソフト(AutoCompressorWindowsService)が停止されました。\n\n" + "エラーメッセージ：\n" + e.Message + "\n\n" + "解決手順：\nStep1 圧縮できていない" + currentFolderName + ".zipを手動で削除する\n\nStep2 エラーを解決する\n\nStep3 圧縮ソフト(AutoCompressorWindowsService)を再起動してください。\n";

                    //output the error message to event log, 
                    //pop -up window,
                    //and the 圧縮ソフト_エラーメッセージ folder in NAS
                    ReportErrorMsg.displayPopUpErrMsg("フォルダー圧縮", errorMessage);

                    //wait a while before starting next retry
                    Thread.Sleep(DynamicConstants.retryTimeInterval);

                    //output error message to a txt file in 圧縮ソフトエラーメッセージ folder in NAS
                    ReportErrorMsg.outputErrorMessageTxt("フォルダー圧縮", errorMessage, DynamicConstants.errorMessageTxtFolderPath);


                    //Stop the AutoCompressorWindowsService
                    Main.stopWindowsService("AutoCompressorWindowsService");

                }

                    
                    /*
                    ////////////////Retry  when error occurs during compression.
                    for (int retryTimes=1; retryTimes <= DynamicConstants.retryTimesLimit; retryTimes++)
                    {

                        //delelte the not-complete zip file which is created 
                        //in the previous compression error before retry  
                        if (retryTimes > 1)
                        {

                             ////Delete incomplete ZIP file created in the previous failed compression
                             ///////////////////////////////////
                            //delete the not-complete zip file which is created 
                            //in the previous compression error before retry               
                            //DeleteOriginalFolder.deleteAFileIfNotInUse(storagePathWithZIPFilename);

                            //indicate that the not-complete zip file has been deleted.
                            //EventLogHandler.outputLog("圧縮できていない"+currentFolderName + "を自動的に削除しました。\n");
                             ///////////////////////////////////

                            ////Rename the output ZIP file name, and do the compression(The incomplete ZIP file created in the previous failed compression is not deleted)
                            string retryFolderName;
                            if (retryTimes == 1)
                            {
                                retryFolderName = currentFolderName;

                            }
                            else
                            {
                                retryFolderName = currentFolderName+ "_retry" + retryTimes;
                            }
                            //rename the output ZIP file when retry
                            //because we can not delete the incomplete ZIP file created in the previous compression
                            EventLogHandler.outputLog("圧縮できていない" + retryFolderName + "のZIPファイル名を" + currentFolderName + "_retry" + retryTimes + "に変更し、\nもう一度圧縮してみます。\n");

                            //indicate that which folder is being compressed currently.
                            EventLogHandler.outputLog(currentFolderName + "_retry" + retryTimes + ": " + "圧縮途中です。\n");




                            string manuallyDelInCompleteZIPFileMsg= "圧縮できていない" + retryFolderName + "を手動で削除してください。\n";

                            ReportErrorMsg.displayPopUpErrMsg("フォルダー圧縮", manuallyDelInCompleteZIPFileMsg);

                            //output error message to a txt file in 圧縮ソフトエラーメッセージ folder in NAS
                            //if there is network connection
                            ReportErrorMsg.outputErrorMessageTxt("フォルダー圧縮", manuallyDelInCompleteZIPFileMsg, DynamicConstants.errorMessageTxtFolderPath);

                            //rename the output ZIP file for this retry
                            storagePathWithZIPFilename = storageFolderName + "\\" + currentFolderName + "_retry" + retryTimes + ".zip";
                        }
                        else
                        {
                            //indicate that which folder is being compressed currently.
                            EventLogHandler.outputLog(currentFolderName + ": " + "圧縮途中です。\n");
                        }



                            //use a thread to compress the folderto a ZIP file
                            //so that we can kill the thread and delete the incomplete ZIP file for retry
                            //when the compression is stopped due to a network error.
                            Thread compressAFolderThread = new Thread(() => compressAFolderTask(targetFolderPath, storagePathWithZIPFilename));

                            //start the compressAFolderThread to compress the folder
                            compressAFolderThread.Start();

                            //wait for the compressAFolderThread to finish compressing the folder
                            compressAFolderThread.Join();

                            //if an error occurs during compression
                            //we output error message and retry 
                            if(IsCompressionFailed==true)
                            {
                                string errorMessage = "";


                                //If it's still within retry times limit
                                if (retryTimes < DynamicConstants.retryTimesLimit)
                                {

                                    //reset the IsCompressionFailed to false
                                    IsCompressionFailed = false;
                                    //display this error message on the GUI window to inform the user
                                    errorMessage = currentFolderName + " フォルダー圧縮途中でエラーが発生しました。\n今もう一度圧縮してみます。\n今回は " + retryTimes + "回目のRetryです。\n\n" + "エラーメッセージ：\n" + compressionErrorMessage;

                                    //output the error message to event log, 
                                    //pop -up window,
                                    //and the 圧縮ソフト_エラーメッセージ folder in NAS
                                    ReportErrorMsg.displayPopUpErrMsg("フォルダー圧縮", errorMessage);

                                    //wait a while before starting next retry
                                    Thread.Sleep(DynamicConstants.retryTimeInterval);

                                    //output error message to a txt file in 圧縮ソフトエラーメッセージ folder in NAS
                                    //if there is network connection
                                    ReportErrorMsg.outputErrorMessageTxt("フォルダー圧縮", errorMessage, DynamicConstants.errorMessageTxtFolderPath);

                                }

                                //If it has reached the retry limit
                                else
                                {




                                    //display this error message on the GUI window to inform the user
                                    errorMessage = currentFolderName + " フォルダー圧縮途中でエラーが発生しました。今回は " + retryTimes + "回目のRetryです。\nRetry回数の上限に達しましたので、\n圧縮ソフト(AutoCompressorWindowsService)が停止されました。\n\n" + "エラーメッセージ：\n" + compressionErrorMessage + "\n\n" + "解決手順：\nStep1 圧縮できていない" + currentFolderName + ".zipを手動で削除する\n\nStep2 エラーを解決する\n\nStep3 圧縮ソフト(AutoCompressorWindowsService)を再起動してください。\n";

                                    //output the error message to event log, 
                                    //pop -up window,
                                    //and the 圧縮ソフト_エラーメッセージ folder in NAS
                                    ReportErrorMsg.displayPopUpErrMsg("フォルダー圧縮", errorMessage);

                                    //wait a while before starting next retry
                                    Thread.Sleep(DynamicConstants.retryTimeInterval);

                                    //output error message to a txt file in 圧縮ソフトエラーメッセージ folder in NAS
                                    ReportErrorMsg.outputErrorMessageTxt("フォルダー圧縮", errorMessage, DynamicConstants.errorMessageTxtFolderPath);


                                    //Stop the AutoCompressorWindowsService
                                    Main.stopWindowsService("AutoCompressorWindowsService");
                                }
                            }

                            //if the compression is done successfully
                            //proceed to do the following works
                            else
                            {
                                //compress the next folder if no error occurs
                                break;
                            }






                    }
                    */



                //record that the folder is compressed successfully in the "compressedFolderNameDict" dictionary
                compressedFolderNameDict[currentFolderName] = "圧縮して保存しました。";

                //Add the status of the currently processed folder to the log message.
                Main.folderStatusAfterCompressLog += currentFolderName + ": " + "圧縮して保存しました。\n";

                //Add the folder to the deletion list
                //The folders in the deletion list will be deleted after all the compression process has done.
                DeleteOriginalFolder.deletionList.Add(currentFolderName);
            }


            //If there is a zip file with the same name exists,
            //1. Do nothing to avoid overwrite old zip file
            //record the repeated folder in the "compressedFolderNameDict" dictionary

            //2. The original file will not be deleted, even though the user set the 
            //圧縮して保存したら、自動でフォルダーを削除するか (Yes or No を入力してください): to "Yes"
            else
            {

                compressedFolderNameDict[currentFolderName] = "設定された保存先に既に同じ名前のZIPファイルが存在しています、圧縮しません。";

                //Add the status of the currently processed folder to the log message.
                Main.folderStatusAfterCompressLog += currentFolderName + ": " + "設定された保存先に既に同じ名前のZIPファイルが存在しています、圧縮しません。\n";
            }
            


        }

        //the task that compressAFolderThread will execute to compress a folder

        //use a thread to compress the folderto a ZIP file
        //so that we can kill the thread and delete the incomplete ZIP file
        //when the compression is stopped due to a network error.

        public void compressAFolderTask(string targetFolderPath, string storagePathWithZIPFilename)
        {
            try
            {
                //compress the folder to a ZIP file
                ZipFile.CreateFromDirectory(targetFolderPath, storagePathWithZIPFilename, CompressionLevel.Optimal, false);

            }
            catch(Exception e)
            {
                //to record that current compression failed
                IsCompressionFailed = true;

                //record the error message during compression
                compressionErrorMessage = e.Message;

                //abort the thread to release the folder and the zip file it holds
                //so that we can delete the zip file for the retry
                Thread.CurrentThread.Abort();

            }




        }


        //Get free space of the drive selected by the user.
        public double getDiskFreeSpace(string NASDriveName)
        {
            //if it is a local drive
            //get free space of the local drive
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == NASDriveName)
                {
                    //display drive name
                    //AutoFolderCompressorMessageBox("ディスク名前：" + drive.Name.ToString() , "Message from AutoCompressorWS");
                    // EventLogHandler.outputLog("ディスク名前：" + drive.Name.ToString());


                    //return the free space of a local drive in GB
                    return Math.Round(double.Parse(drive.TotalFreeSpace.ToString()) / 1073741824, 2);
                }
            }

            //if it is a remote drive
            //get free space of the remote drive
            //NASDriveName will be something like \\10.30.50.15
            long availableRemoteDriveSpace = CheckRemoteDriveFreeSpace.getRemoteDriveFreeSpace(NASDriveName);
            if (availableRemoteDriveSpace != -1)
            {
                //return the free space of a remote drive in GB
                return Math.Round(double.Parse(availableRemoteDriveSpace.ToString()) / 1073741824, 2); ;

            }

            //if it is not a local drive, nor a remote drive
            //return -1 as an error message.
            return -1;
        }


        //Compress the folders created over XX days ago (何日前のデータを圧縮して保存するか)　
        //and have not been compressed in the target folder(圧縮する目標フォルダー)
        public void compressFolder(string targetFolder, string ZIPStorageFolder,
            string folderOverNDays, string deleteAfterCompressOption)
        {
            

                //get the date from folderOverNDays
                DateTime olderThanThisDate = DateTime.Now.AddDays(-1 * Int32.Parse(folderOverNDays));
                
                //count the number of folder to be compressed in today's compression 
                int countTodayCompressFolderNum = 1;



            //Get creation time of each folder
            foreach (string currentFolderFullPath in Directory.GetDirectories(targetFolder))
                {
                    //Get creation time of each folder
                    DateTime currentFolderCreationDate = Directory.GetLastWriteTime(currentFolderFullPath);

                   


                    //get folder name
                    string currentFolderName = currentFolderFullPath.Remove(0, targetFolder.Length + 1);

                    if (currentFolderCreationDate <= olderThanThisDate)
                    {
                        //if the folder name does not exist in the dictionary, 
                        //add the folder name to a dictionary
                        if (!compressedFolderNameDict.ContainsKey(currentFolderName))
                        {
                            //keep the number of folder to be compressed in today's compression 
                            //below the maximum setting
                            if (countTodayCompressFolderNum <= DynamicConstants.oneDayMaxCompressFolderNum)
                            {


                                    //accumulate the number of folder to be compressed in today's compression
                                    countTodayCompressFolderNum = countTodayCompressFolderNum + 1;

                            //compressedFolderNameDict.Add(currentFolderName, "");

                            /*
                             //check compress order 
                             EventLogHandler.outputLog("Start to compress "+currentFolderName );
                            */

                                    //If the user specified the 毎日何時に圧縮一時停止するのか（24時間制） and 毎日何時に圧縮再開するのか（24時間制）
                                    if (ReadInUserSettings.IsPauseTimeSet())
                                    {
                                        //圧縮一時停止する
                                        //圧縮再開する時間になったら、再開する
                                        pauseTheCompressionProgram();

                                    }

                                    //compress the folder
                                    createZIPFile(currentFolderFullPath, ZIPStorageFolder , currentFolderName);

                                    /*
                                    //check compress order 
                                     EventLogHandler.outputLog("Finish compressing " + currentFolderName);
                                    */


                                    //If the user set圧縮して保存したら、自動でフォルダーを削除するか (Yes or No を入力してください):
                                    //to be "Yes" in the 自動圧縮設定.txt,
                                    //we delete the original folder after compression
                                    if (deleteAfterCompressOption == "DeleteAfterCompress")
                                    {
                                        // Delete A original folders after it finishes its compression process
                                        DeleteOriginalFolder.deleteAOriginalFolderAfterCompress(targetFolder);
                                    }

                                }
                            }



                        //if the folder name has existed in the dictionary,
                        //do nothing to avoid overwriting old zip file, and
                        //record the repeated folder in the "compressedFolderNameDict" dictionary
                        else
                        {

                            compressedFolderNameDict[currentFolderName] = "既に圧縮したことがあります、圧縮しません。";

                            //Add the status of the currently processed folder to the log message.
                            Main.folderStatusAfterCompressLog += currentFolderName + ": " + "既に圧縮したことがあります、圧縮しません。\n";
                        }



                        //Output the log that records the status of all the folders that go through the compression process
                        //to the イベントビューアー
                        if (Main.folderStatusAfterCompressLog != "")
                        {


                             EventLogHandler.outputLog(Main.folderStatusAfterCompressLog);

                        }


                        //Reset the content of folderStatusAfterCompressLog
                        Main.resetFolderStatusAfterCompressLog();

                    }
                }


            

        }

        //圧縮一時停止する
        //圧縮再開する時間になったら、再開する
        public void pauseTheCompressionProgram()
        {
            //if a day has passed since the program started running
            if (DynamicConstants.progamStartDate < DateTime.Today)
            {
                //reset the DynamicConstants.HasPauseHappened
                //to pause the program for the new day
                DynamicConstants.HasPauseHappened = false;
            }

            //Check whether it is later than the pause time(under the condition that pause has not happened in 今日の圧縮 ) 
            //or within an hour to reach the pause time set by the user
            if ((DynamicConstants.HasPauseHappened == false && DateTime.Now >= ReadInUserSettings.getPauseStartTime())
                       || (ReadInUserSettings.getPauseStartTime().Subtract(DateTime.Now).Minutes < 60 && ReadInUserSettings.getPauseStartTime().Subtract(DateTime.Now).Minutes >= 0))
            {
                EventLogHandler.outputLog("設定された圧縮一時停止する時間：" + ReadInUserSettings.getUserSpecifiedPauseStartTime + "より遅くなった、\nまたは一時停止する時間まであと一時間以内になったので、圧縮は一時停止する。\n設定された再開時間：" + ReadInUserSettings.getUserSpecifiedPauseEndTime + "になったら、自動的に再開する。\n");


                // pause the program until the resume time set by the user comes
                Thread.Sleep(ReadInUserSettings.getPauseTimePeriod());

                EventLogHandler.outputLog("今は設定された再開時間：" + ReadInUserSettings.getUserSpecifiedPauseEndTime + "です、\n圧縮は自動的に再開する。\n");

                //set the DynamicConstants.HasPauseHappened to true
                //so that it will not pause again within today
                DynamicConstants.HasPauseHappened = true;
            }
        }
       

    }
}
