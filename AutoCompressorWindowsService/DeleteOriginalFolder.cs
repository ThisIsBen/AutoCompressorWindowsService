﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using System.Security;

namespace AutoCompressorWindowsService
{
    class DeleteOriginalFolder
    {

        //Record which folders can be deleted (The folder's status is "圧縮して保存しました。" will be deleted)
        public static List<string> deletionList = new List<string>();

        //Wait until the specified folder is not in use, and then
        //delete it.
        public static void deleteAfterCompress(string targetDirectory)
        {
            //Wait for all the files to be not in use
            waitUntilFolderNotInUse(targetDirectory);

            //Delete the folder 
            deleteAFolder( targetDirectory);

        }

        //Wait for all the files to be not in use
        public static async void waitUntilFolderNotInUse(string targetDirectory)
        {
            //Check whether the folder exists, if no, don't do anything
            if (Directory.Exists(targetDirectory))
            {
                // Recurse into subdirectories of this directory.
                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                foreach (string currentSubdirectory in subdirectoryEntries)
                {
                    // Check each file found in each subdirectory.
                    string[] fileEntries = Directory.GetFiles(currentSubdirectory);
                    foreach (string currentFileName in fileEntries)
                    {
                        FileInfo currentFileInfo = new FileInfo(currentFileName);

                        ///*
                        //if the file is in use wait for 1 second and check again
                        while (IsFileInUse(currentFileInfo) == true)
                        {

                            // wait for a while
                            await Task.Delay(DynamicConstants.waitReadyToBeDeleteTimeInterval);
                        }
                        //*/

                    }

                }



            }


           


        }

        //Delete a folder
        private static void deleteAFolder(string targetDirectory)
        {
            //Retry  when error occurs during compression.
            for (int retryTimes = 1; retryTimes <= DynamicConstants.retryTimesWhenErrOccurs; retryTimes++)
            {

                try
                {
                    var dir = new DirectoryInfo(targetDirectory);
                    dir.Attributes = dir.Attributes & ~FileAttributes.ReadOnly;
                    dir.Delete(true);

                    //end this function if no error occurs
                    break;
                }
                catch (UnauthorizedAccessException e)
                {
                    string errorMessage = "";

                    // If it's still within retry times limit
                    if (retryTimes < DynamicConstants.retryTimesWhenErrOccurs)
                    {

                        //display this error message on the GUI window to inform the user
                        errorMessage = targetDirectory + " フォルダーを圧縮完了しましたが、フォルダーを削除する途中でエラーが発生しました。\n今もう一度圧縮してみます。\n今回は " + retryTimes + "回目のRetryです。\n\n" + "エラーメッセージ：\n" + e.Message;

                        //output the error message to event log, 
                        //pop -up window,
                        //and the 圧縮ソフト_エラーメッセージ folder in NAS
                        Main.displayErrMsg(errorMessage);

                        //wait a while before starting next retry
                        Thread.Sleep(DynamicConstants.retryTimeInterval);

                        
                    }

                    //If it has reached the retry limit
                    else
                    {

                        //display this error message on the GUI window to inform the user
                        errorMessage = targetDirectory + " フォルダーを圧縮完了しましたが、フォルダーを削除する途中でエラーが発生しました。\n今回は " + retryTimes + "回目のRetryです。\nRetry回数の上限に達しましたので、圧縮ソフト(AutoCompressorWindowsService)が停止されました。\n\n" + "エラーメッセージ：\n" + e.Message + "\n\n" + "解決手順：\nStep1 自動圧縮設定.txtに指定された圧縮する目標フォルダーから " + targetDirectory + " を手動で削除する\n\nStep2 AutoCompressorWindowsServiceを再起動してください。\n";


                        //output the error message to event log, 
                        //pop -up window,
                        //and the 圧縮ソフト_エラーメッセージ folder in NAS
                        Main.displayErrMsg(errorMessage);



                        //Stop the AutoCompressorWindowsService
                        Main.stopWindowsService("AutoCompressorWindowsService");
                    }

                }
                catch (DirectoryNotFoundException e)
                {
                    string errorMessage = "";

                    // If it's still within retry times limit
                    if (retryTimes < DynamicConstants.retryTimesWhenErrOccurs)
                    {

                        //display this error message on the GUI window to inform the user
                        errorMessage = targetDirectory + " フォルダーを圧縮完了しましたが、フォルダーを削除する途中でエラーが発生しました。\n今もう一度圧縮してみます。\n今回は " + retryTimes + "回目のRetryです。\n\n" + "エラーメッセージ：\n" + e.Message;

                        //output the error message to event log, 
                        //pop -up window,
                        //and the 圧縮ソフト_エラーメッセージ folder in NAS
                        Main.displayErrMsg(errorMessage);

                        //wait a while before starting next retry
                        Thread.Sleep(DynamicConstants.retryTimeInterval);


                    }

                    //If it has reached the retry limit
                    else
                    {

                        //display this error message on the GUI window to inform the user
                        errorMessage = targetDirectory + " フォルダーを圧縮完了しましたが、フォルダーを削除する途中でエラーが発生しました。\n今回は " + retryTimes + "回目のRetryです。\nRetry回数の上限に達しましたので、圧縮ソフト(AutoCompressorWindowsService)が停止されました。\n\n" + "エラーメッセージ：\n" + e.Message + "\n\n" + "解決手順：\nStep1 自動圧縮設定.txtに指定された圧縮する目標フォルダーから " + targetDirectory + " を手動で削除する\n\nStep2 AutoCompressorWindowsServiceを再起動してください。\n";


                        //output the error message to event log, 
                        //pop -up window,
                        //and the 圧縮ソフト_エラーメッセージ folder in NAS
                        Main.displayErrMsg(errorMessage);



                        //Stop the AutoCompressorWindowsService
                        Main.stopWindowsService("AutoCompressorWindowsService");
                    }
                }

                catch (IOException e)
                {
                    string errorMessage = "";

                    // If it's still within retry times limit
                    if (retryTimes < DynamicConstants.retryTimesWhenErrOccurs)
                    {

                        //display this error message on the GUI window to inform the user
                        errorMessage = targetDirectory + " フォルダーを圧縮完了しましたが、フォルダーを削除する途中でエラーが発生しました。\n今もう一度圧縮してみます。\n今回は " + retryTimes + "回目のRetryです。\n\n" + "エラーメッセージ：\n" + e.Message;

                        //output the error message to event log, 
                        //pop -up window,
                        //and the 圧縮ソフト_エラーメッセージ folder in NAS
                        Main.displayErrMsg(errorMessage);

                        //wait a while before starting next retry
                        Thread.Sleep(DynamicConstants.retryTimeInterval);


                    }

                    //If it has reached the retry limit
                    else
                    {

                        //display this error message on the GUI window to inform the user
                        errorMessage = targetDirectory + " フォルダーを圧縮完了しましたが、フォルダーを削除する途中でエラーが発生しました。\n今回は " + retryTimes + "回目のRetryです。\nRetry回数の上限に達しましたので、圧縮ソフト(AutoCompressorWindowsService)が停止されました。\n\n" + "エラーメッセージ：\n" + e.Message + "\n\n" + "解決手順：\nStep1 自動圧縮設定.txtに指定された圧縮する目標フォルダーから " + targetDirectory + " を手動で削除する\n\nStep2 AutoCompressorWindowsServiceを再起動してください。\n";


                        //output the error message to event log, 
                        //pop -up window,
                        //and the 圧縮ソフト_エラーメッセージ folder in NAS
                        Main.displayErrMsg(errorMessage);



                        //Stop the AutoCompressorWindowsService
                        Main.stopWindowsService("AutoCompressorWindowsService");
                    }
                }

                catch (SecurityException e)
                {
                    string errorMessage = "";

                    // If it's still within retry times limit
                    if (retryTimes < DynamicConstants.retryTimesWhenErrOccurs)
                    {

                        //display this error message on the GUI window to inform the user
                        errorMessage = targetDirectory + " フォルダーを圧縮完了しましたが、フォルダーを削除する途中でエラーが発生しました。\n今もう一度圧縮してみます。\n今回は " + retryTimes + "回目のRetryです。\n\n" + "エラーメッセージ：\n" + e.Message;

                        //output the error message to event log, 
                        //pop -up window,
                        //and the 圧縮ソフト_エラーメッセージ folder in NAS
                        Main.displayErrMsg(errorMessage);

                        //wait a while before starting next retry
                        Thread.Sleep(DynamicConstants.retryTimeInterval);


                    }

                    //If it has reached the retry limit
                    else
                    {

                        //display this error message on the GUI window to inform the user
                        errorMessage = targetDirectory + " フォルダーを圧縮完了しましたが、フォルダーを削除する途中でエラーが発生しました。\n今回は " + retryTimes + "回目のRetryです。\nRetry回数の上限に達しましたので、圧縮ソフト(AutoCompressorWindowsService)が停止されました。\n\n" + "エラーメッセージ：\n" + e.Message + "\n\n" + "解決手順：\nStep1 自動圧縮設定.txtに指定された圧縮する目標フォルダーから " + targetDirectory + " を手動で削除する\n\nStep2 AutoCompressorWindowsServiceを再起動してください。\n";


                        //output the error message to event log, 
                        //pop -up window,
                        //and the 圧縮ソフト_エラーメッセージ folder in NAS
                        Main.displayErrMsg(errorMessage);



                        //Stop the AutoCompressorWindowsService
                        Main.stopWindowsService("AutoCompressorWindowsService");
                    }
                }

            }
        }

        //Check if the file is in use
        protected static bool IsFileInUse(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }

        //Clear the content of the deletionList
        public static void resetDeletionList()
        {
            deletionList.Clear();



        }


        //Delete A original folders after it finishes its compression process
        public static void deleteAOriginalFolderAfterCompress(string targetFolder)
        {
            
           
            //delete all the folders in the DeleteOriginalFolder.deletionList,
            //The status of the folders in the DeleteOriginalFolder.deletionList are all "圧縮して保存しました。"
            foreach (string delFolderName in DeleteOriginalFolder.deletionList) // Loop through List with foreach
            {


                //Wait for the compression to finish and delete the original folder
                deleteAfterCompress(targetFolder + "\\" + delFolderName);


                //Write the delete event to log
                //Add the status of the currently processed folder to the log message.
                Main.folderStatusAfterCompressLog += delFolderName + ": " + "削除しました。\n";
                




            }

            //Clear the content of the deletionList
            DeleteOriginalFolder.resetDeletionList();

            



        }



        //Wait until the specified folder is not in use, and then
        //delete it.
        public static void deleteAFileIfNotInUse(string targetFilePath)
        {


            //Wait for all the files to be not in use
            waitUntilFileNotInUse(targetFilePath);


            //Delete the file
            File.Delete(targetFilePath);





        }
        //Wait for all the files to be not in use
        public static async void waitUntilFileNotInUse(string targetFilePath)
        {
            //Check whether the folder exists, if no, don't do anything
            if (File.Exists(targetFilePath))
            {

                FileInfo currentFileInfo = new FileInfo(targetFilePath);


                //if the file is in use wait for a while and check again
                while (IsFileInUse(currentFileInfo) == true)
                {
                    //error occurs when delete an occupied file
                    // wait for a while if the file is being used by another process
                    await Task.Delay(DynamicConstants.waitReadyToBeDeleteTimeInterval);


                  
                }


            }











        }



        //Delete ALL original folders after compression finishes
        private void deleteWholeOriginalFolderAfterCompress(string targetFolder)
        {

          
            //delete all the folders in the DeleteOriginalFolder.deletionList,
            //The status of the folders in the DeleteOriginalFolder.deletionList are all "圧縮して保存しました。"
            foreach (string delFolderName in DeleteOriginalFolder.deletionList) // Loop through List with foreach
            {


                //Wait for the compression to finish and delete the original folder
                deleteAfterCompress(targetFolder + "\\" + delFolderName);


                //Write the delete event to log
                //Add the status of the currently processed folder to the log message.
                Main.folderStatusAfterCompressLog += delFolderName + ": " + "削除しました。\n";






            }

            //Clear the content of the deletionList
            DeleteOriginalFolder.resetDeletionList();

            



        }
        
        
        
        /*
        //Get folder size
        private static double GetFolderSizeInKB(string folderPath)
        {
            DirectoryInfo di = new DirectoryInfo(folderPath);
            return di.EnumerateFiles("*", SearchOption.AllDirectories).Sum(fi => fi.Length)/1024;
        }

        private static double GetZIPFileSizeInKB(string ZIPFilePath)
        {
            long ZIPFileSizeInKB = new System.IO.FileInfo(ZIPFilePath).Length/1024;
            return ZIPFileSizeInKB;
        }
        */
        /*
        private static void compareOriginalFolder_ZIPSize(string folderPath,string ZIPFilePath)
        {
            double originalFolderSize = GetFolderSizeInKB(folderPath);
            double ZIPFileSize = GetZIPFileSizeInKB(ZIPFilePath);
          
            if(ZIPFileSize > originalFolderSize * (1-DynamicConstants.compressRateForCheckZIPFinish) )
            {


            }

        }
        */

    }
}
