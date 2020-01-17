using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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
                ZipFile.CreateFromDirectory(targetFolderPath, storagePathWithZIPFilename);

                //record that the folder is compressed successfully in the "compressedFolderNameDict" dictionary
                compressedFolderNameDict[currentFolderName] = "圧縮して保存しました。";

                //Add the status of the currently processed folder to the log message.
                Main.folderStatusAfterCompressLog += currentFolderName + ": " + "圧縮して保存しました。\n";

                //Add the folder to the deletion list
                //The folders in the deletion list will be deleted after all the compression process has done.
                DeleteOriginalFolder.deletionList.Add(currentFolderName);
            }

            
            //If there is a zip file with the same name exists,
            //do nothing to avoid overwrite old zip file
            //record the repeated folder in the "compressedFolderNameDict" dictionary
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
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == NASDriveName)
                {
                    //display drive name
                    //AutoFolderCompressorMessageBox("ディスク名前：" + drive.Name.ToString() , "Message from AutoCompressorWS");
                    //eventLog1.WriteEntry("ディスク名前：" + drive.Name.ToString());


                    //return drive.TotalFreeSpace;
                    return Math.Round(double.Parse(drive.TotalFreeSpace.ToString()) / 1073741824, 2);
                }
            }
            return -1;
        }


        //Compress the folders created over XX days ago (何日前のデータを圧縮して保存するか)　
        //and have not been compressed in the target folder(圧縮する目標フォルダー)
        public void compressFolder(string targetFolder,string ZIPStorageFolder,
            string folderOverNDays)
        {
            //get the date from folderOverNDays
            DateTime olderThanThisDate = DateTime.Now.AddDays(-1*Int32.Parse(folderOverNDays));




            //Get creation time of each folder
            foreach (string currentFolderFullPath in Directory.GetDirectories(targetFolder))
            {
                

                //Get creation time of each folder
                DateTime currentFolderCreationDate = Directory.GetLastWriteTime(currentFolderFullPath);


                //get folder name
                string currentFolderName = currentFolderFullPath.Remove(0, targetFolder.Length+1);

                if (currentFolderCreationDate <= olderThanThisDate)
                {
                    //if the folder name does not exist in the dictionary, 
                    //add the folder name to a dictionary
                    if (!compressedFolderNameDict.ContainsKey(currentFolderName))
                    {
                        compressedFolderNameDict.Add(currentFolderName, "");

                        //compress the folder
                        createZIPFile(currentFolderFullPath, ZIPStorageFolder + "\\" + currentFolderName + ".zip", currentFolderName);
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

                }
            }

            
        }
    
       

    }
}
