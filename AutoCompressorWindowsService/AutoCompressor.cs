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


        //Compress a folder to create a ZIP file
        public void createZIPFile(string targetFolderPath,string storagePathWithZIPFilename,string currentFolderName)
        {
           
            //if there is a zip file with the same name exists,
            //do nothing to avoid overwrite old zip file
            //else, compress and create zip file as usual
            if (File.Exists(storagePathWithZIPFilename) == false)
            {
               
                //Retry  when error occurs during compression.
                for (int retryTimes=1; retryTimes <= DynamicConstants.retryTimesLimit; retryTimes++)
                {

               

                    try
                    {
                        //indicate that which folder is being compressed currently.
                        EventLogHandler.outputLog(currentFolderName + ": " + "圧縮途中です。\n");


                        //compress the folder to a ZIP file
                        ZipFile.CreateFromDirectory(targetFolderPath, storagePathWithZIPFilename, CompressionLevel.Optimal, false);

                        //compress next folder if no error occurs
                        break;
                    }
                    catch (UnauthorizedAccessException e)
                    {

                        string errorMessage="";


                        //If it's still within retry times limit
                        if (retryTimes< DynamicConstants.retryTimesLimit)
                        {
                            ////Ben Not sure if auto deleting the not compress-complete ZIP file  works

                            //delete the zip file that has not finished its compression due to the error.
                            DeleteOriginalFolder.deleteAFileIfNotInUse(storagePathWithZIPFilename);

                            //display this error message on the GUI window to inform the user
                            errorMessage = currentFolderName + " フォルダー圧縮途中でエラーが発生しました。\n今もう一度圧縮してみます。\n今回は "+retryTimes+"回目のRetryです。\n\n"+"エラーメッセージ：\n" + e.Message ;

                            //output the error message to event log, 
                            //pop -up window,
                            //and the 圧縮ソフト_エラーメッセージ folder in NAS
                            ReportErrorMsg.displayErrMsg("フォルダー圧縮", errorMessage);
                           


                            //wait a while before starting next retry
                            Thread.Sleep(DynamicConstants.retryTimeInterval);
                        }

                        //If it has reached the retry limit
                        else
                        {
                            //delete the zip file that has not finished its compression due to the error.
                            DeleteOriginalFolder.deleteAFileIfNotInUse(storagePathWithZIPFilename);

                            //display this error message on the GUI window to inform the user
                            errorMessage = currentFolderName + " フォルダー圧縮途中でエラーが発生しました。今回は " + retryTimes + "回目のRetryです。\nRetry回数の上限に達しましたので、\n圧縮途中の"+ currentFolderName + ".zipが自動的に削除され,\n圧縮ソフト(AutoCompressorWindowsService)が停止されました。\n\n" + "エラーメッセージ：\n" + e.Message + "\n\n" + "解決手順：\nStep1 エラーを解決する\n\nStep2 圧縮ソフト(AutoCompressorWindowsService)を再起動してください。\n";

                            //output the error message to event log, 
                            //pop -up window,
                            //and the 圧縮ソフト_エラーメッセージ folder in NAS
                            ReportErrorMsg.displayErrMsg("フォルダー圧縮", errorMessage);



                            //Stop the AutoCompressorWindowsService
                            Main.stopWindowsService("AutoCompressorWindowsService");
                        }

                        
                        
                    }
                    catch (DirectoryNotFoundException e)
                    {
                        string errorMessage = "";


                        //If it's still within retry times limit
                        if (retryTimes < DynamicConstants.retryTimesLimit)
                        {


                            //delete the zip file that has not finished its compression due to the error.
                            DeleteOriginalFolder.deleteAFileIfNotInUse(storagePathWithZIPFilename);

                            //display this error message on the GUI window to inform the user
                            errorMessage = currentFolderName + " フォルダー圧縮途中でエラーが発生しました。\n今もう一度圧縮してみます。\n今回は " + retryTimes + "回目のRetryです。\n\n" + "エラーメッセージ：\n" + e.Message;

                            //output the error message to event log, 
                            //pop -up window,
                            //and the 圧縮ソフト_エラーメッセージ folder in NAS
                            ReportErrorMsg.displayErrMsg("フォルダー圧縮", errorMessage);



                            //wait a while before starting next retry
                            Thread.Sleep(DynamicConstants.retryTimeInterval);
                        }

                        //If it has reached the retry limit
                        else
                        {
                            //delete the zip file that has not finished its compression due to the error.
                            DeleteOriginalFolder.deleteAFileIfNotInUse(storagePathWithZIPFilename);

                            //display this error message on the GUI window to inform the user
                            errorMessage = currentFolderName + " フォルダー圧縮途中でエラーが発生しました。今回は " + retryTimes + "回目のRetryです。\nRetry回数の上限に達しましたので、\n圧縮途中の" + currentFolderName + ".zipが自動的に削除され,\n圧縮ソフト(AutoCompressorWindowsService)が停止されました。\n\n" + "エラーメッセージ：\n" + e.Message + "\n\n" + "解決手順：\nStep1 エラーを解決する\n\nStep2 圧縮ソフト(AutoCompressorWindowsService)を再起動してください。\n";

                            //output the error message to event log, 
                            //pop -up window,
                            //and the 圧縮ソフト_エラーメッセージ folder in NAS
                            ReportErrorMsg.displayErrMsg("フォルダー圧縮", errorMessage);



                            //Stop the AutoCompressorWindowsService
                            Main.stopWindowsService("AutoCompressorWindowsService");
                        }
                    }
                    catch (ArgumentNullException e)
                    {
                        string errorMessage = "";


                        //If it's still within retry times limit
                        if (retryTimes < DynamicConstants.retryTimesLimit)
                        {


                            //delete the zip file that has not finished its compression due to the error.
                            DeleteOriginalFolder.deleteAFileIfNotInUse(storagePathWithZIPFilename);

                            //display this error message on the GUI window to inform the user
                            errorMessage = currentFolderName + " フォルダー圧縮途中でエラーが発生しました。\n今もう一度圧縮してみます。\n今回は " + retryTimes + "回目のRetryです。\n\n" + "エラーメッセージ：\n" + e.Message;

                            //output the error message to event log, 
                            //pop -up window,
                            //and the 圧縮ソフト_エラーメッセージ folder in NAS
                            ReportErrorMsg.displayErrMsg("フォルダー圧縮", errorMessage);


                            //wait a while before starting next retry
                            Thread.Sleep(DynamicConstants.retryTimeInterval);
                        }

                        //If it has reached the retry limit
                        else
                        {
                            //delete the zip file that has not finished its compression due to the error.
                            DeleteOriginalFolder.deleteAFileIfNotInUse(storagePathWithZIPFilename);

                            //display this error message on the GUI window to inform the user
                            errorMessage = currentFolderName + " フォルダー圧縮途中でエラーが発生しました。今回は " + retryTimes + "回目のRetryです。\nRetry回数の上限に達しましたので、\n圧縮途中の" + currentFolderName + ".zipが自動的に削除され,\n圧縮ソフト(AutoCompressorWindowsService)が停止されました。\n\n" + "エラーメッセージ：\n" + e.Message + "\n\n" + "解決手順：\nStep1 エラーを解決する\n\nStep2 圧縮ソフト(AutoCompressorWindowsService)を再起動してください。\n";

                            //output the error message to event log, 
                            //pop -up window,
                            //and the 圧縮ソフト_エラーメッセージ folder in NAS
                            ReportErrorMsg.displayErrMsg("フォルダー圧縮", errorMessage);



                            //Stop the AutoCompressorWindowsService
                            Main.stopWindowsService("AutoCompressorWindowsService");
                        }
                    }
                    catch (ArgumentException e)
                    {
                        string errorMessage = "";


                        //If it's still within retry times limit
                        if (retryTimes < DynamicConstants.retryTimesLimit)
                        {


                            //delete the zip file that has not finished its compression due to the error.
                            DeleteOriginalFolder.deleteAFileIfNotInUse(storagePathWithZIPFilename);

                            //display this error message on the GUI window to inform the user
                            errorMessage = currentFolderName + " フォルダー圧縮途中でエラーが発生しました。\n今もう一度圧縮してみます。\n今回は " + retryTimes + "回目のRetryです。\n\n" + "エラーメッセージ：\n" + e.Message;

                            //output the error message to event log, 
                            //pop -up window,
                            //and the 圧縮ソフト_エラーメッセージ folder in NAS
                            ReportErrorMsg.displayErrMsg("フォルダー圧縮", errorMessage);



                            //wait a while before starting next retry
                            Thread.Sleep(DynamicConstants.retryTimeInterval);
                        }

                        //If it has reached the retry limit
                        else
                        {
                            //delete the zip file that has not finished its compression due to the error.
                            DeleteOriginalFolder.deleteAFileIfNotInUse(storagePathWithZIPFilename);

                            //display this error message on the GUI window to inform the user
                            errorMessage = currentFolderName + " フォルダー圧縮途中でエラーが発生しました。今回は " + retryTimes + "回目のRetryです。\nRetry回数の上限に達しましたので、\n圧縮途中の" + currentFolderName + ".zipが自動的に削除され,\n圧縮ソフト(AutoCompressorWindowsService)が停止されました。\n\n" + "エラーメッセージ：\n" + e.Message + "\n\n" + "解決手順：\nStep1 エラーを解決する\n\nStep2 圧縮ソフト(AutoCompressorWindowsService)を再起動してください。\n";

                            //output the error message to event log, 
                            //pop -up window,
                            //and the 圧縮ソフト_エラーメッセージ folder in NAS
                            ReportErrorMsg.displayErrMsg("フォルダー圧縮", errorMessage);



                            //Stop the AutoCompressorWindowsService
                            Main.stopWindowsService("AutoCompressorWindowsService");
                        }
                    }
                    catch (PathTooLongException e)
                    {
                        string errorMessage = "";


                        //If it's still within retry times limit
                        if (retryTimes < DynamicConstants.retryTimesLimit)
                        {


                            //delete the zip file that has not finished its compression due to the error.
                            DeleteOriginalFolder.deleteAFileIfNotInUse(storagePathWithZIPFilename);

                            //display this error message on the GUI window to inform the user
                            errorMessage = currentFolderName + " フォルダー圧縮途中でエラーが発生しました。\n今もう一度圧縮してみます。\n今回は " + retryTimes + "回目のRetryです。\n\n" + "エラーメッセージ：\n" + e.Message;

                            //output the error message to event log, 
                            //pop -up window,
                            //and the 圧縮ソフト_エラーメッセージ folder in NAS
                            ReportErrorMsg.displayErrMsg("フォルダー圧縮", errorMessage);



                            //wait a while before starting next retry
                            Thread.Sleep(DynamicConstants.retryTimeInterval);
                        }

                        //If it has reached the retry limit
                        else
                        {
                            //delete the zip file that has not finished its compression due to the error.
                            DeleteOriginalFolder.deleteAFileIfNotInUse(storagePathWithZIPFilename);

                            //display this error message on the GUI window to inform the user
                            errorMessage = currentFolderName + " フォルダー圧縮途中でエラーが発生しました。今回は " + retryTimes + "回目のRetryです。\nRetry回数の上限に達しましたので、\n圧縮途中の" + currentFolderName + ".zipが自動的に削除され,\n圧縮ソフト(AutoCompressorWindowsService)が停止されました。\n\n" + "エラーメッセージ：\n" + e.Message + "\n\n" + "解決手順：\nStep1 エラーを解決する\n\nStep2 圧縮ソフト(AutoCompressorWindowsService)を再起動してください。\n";

                            //output the error message to event log, 
                            //pop -up window,
                            //and the 圧縮ソフト_エラーメッセージ folder in NAS
                            ReportErrorMsg.displayErrMsg("フォルダー圧縮", errorMessage);



                            //Stop the AutoCompressorWindowsService
                            Main.stopWindowsService("AutoCompressorWindowsService");
                        }
                    }
                    catch (IOException e)
                    {
                        string errorMessage = "";


                        //If it's still within retry times limit
                        if (retryTimes < DynamicConstants.retryTimesLimit)
                        {


                            //delete the zip file that has not finished its compression due to the error.
                            DeleteOriginalFolder.deleteAFileIfNotInUse(storagePathWithZIPFilename);

                            //display this error message on the GUI window to inform the user
                            errorMessage = currentFolderName + " フォルダー圧縮途中でエラーが発生しました。\n今もう一度圧縮してみます。\n今回は " + retryTimes + "回目のRetryです。\n\n" + "エラーメッセージ：\n" + e.Message;

                            //output the error message to event log, 
                            //pop -up window,
                            //and the 圧縮ソフト_エラーメッセージ folder in NAS
                            ReportErrorMsg.displayErrMsg("フォルダー圧縮", errorMessage);



                            //wait a while before starting next retry
                            Thread.Sleep(DynamicConstants.retryTimeInterval);
                        }

                        //If it has reached the retry limit
                        else
                        {
                            //delete the zip file that has not finished its compression due to the error.
                            DeleteOriginalFolder.deleteAFileIfNotInUse(storagePathWithZIPFilename);

                            //display this error message on the GUI window to inform the user
                            errorMessage = currentFolderName + " フォルダー圧縮途中でエラーが発生しました。今回は " + retryTimes + "回目のRetryです。\nRetry回数の上限に達しましたので、\n圧縮途中の" + currentFolderName + ".zipが自動的に削除され,\n圧縮ソフト(AutoCompressorWindowsService)が停止されました。\n\n" + "エラーメッセージ：\n" + e.Message + "\n\n" + "解決手順：\nStep1 エラーを解決する\n\nStep2 圧縮ソフト(AutoCompressorWindowsService)を再起動してください。\n";

                            //output the error message to event log, 
                            //pop -up window,
                            //and the 圧縮ソフト_エラーメッセージ folder in NAS
                            ReportErrorMsg.displayErrMsg("フォルダー圧縮", errorMessage);



                            //Stop the AutoCompressorWindowsService
                            Main.stopWindowsService("AutoCompressorWindowsService");
                        }
                    }
                    catch (NotSupportedException e)
                    {
                        string errorMessage = "";


                        //If it's still within retry times limit
                        if (retryTimes < DynamicConstants.retryTimesLimit)
                        {


                            //delete the zip file that has not finished its compression due to the error.
                            DeleteOriginalFolder.deleteAFileIfNotInUse(storagePathWithZIPFilename);

                            //display this error message on the GUI window to inform the user
                            errorMessage = currentFolderName + " フォルダー圧縮途中でエラーが発生しました。\n今もう一度圧縮してみます。\n今回は " + retryTimes + "回目のRetryです。\n\n" + "エラーメッセージ：\n" + e.Message;

                            //output the error message to event log, 
                            //pop -up window,
                            //and the 圧縮ソフト_エラーメッセージ folder in NAS
                            ReportErrorMsg.displayErrMsg("フォルダー圧縮", errorMessage);



                            //wait a while before starting next retry
                            Thread.Sleep(DynamicConstants.retryTimeInterval);
                        }

                        //If it has reached the retry limit
                        else
                        {
                            //delete the zip file that has not finished its compression due to the error.
                            DeleteOriginalFolder.deleteAFileIfNotInUse(storagePathWithZIPFilename);

                            //display this error message on the GUI window to inform the user
                            errorMessage = currentFolderName + " フォルダー圧縮途中でエラーが発生しました。今回は " + retryTimes + "回目のRetryです。\nRetry回数の上限に達しましたので、\n圧縮途中の" + currentFolderName + ".zipが自動的に削除され,\n圧縮ソフト(AutoCompressorWindowsService)が停止されました。\n\n" + "エラーメッセージ：\n" + e.Message + "\n\n" + "解決手順：\nStep1 エラーを解決する\n\nStep2 圧縮ソフト(AutoCompressorWindowsService)を再起動してください。\n";

                            //output the error message to event log, 
                            //pop -up window,
                            //and the 圧縮ソフト_エラーメッセージ folder in NAS
                            ReportErrorMsg.displayErrMsg("フォルダー圧縮", errorMessage);


                            //Stop the AutoCompressorWindowsService
                            Main.stopWindowsService("AutoCompressorWindowsService");
                        }
                    }

                }




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

                                    //compress the folder
                                    createZIPFile(currentFolderFullPath, ZIPStorageFolder + "\\" + currentFolderName + ".zip", currentFolderName);

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


       

    }
}
