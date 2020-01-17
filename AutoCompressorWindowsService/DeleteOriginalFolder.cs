using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace AutoCompressorWindowsService
{
    class DeleteOriginalFolder
    {

        //Record which folders can be deleted (The folder's status is "圧縮して保存しました。" will be deleted)
        public static List<string> deletionList = new List<string>();

        //Wait until the specified folder is not in use, and then
        //delete it.
        public void deleteAfterCompress(string targetDirectory)
        {
            //Wait for all the files to be not in use
            waitForCompressFinish(targetDirectory);

            //Delete the folder 
            deleteAFolder( targetDirectory);

        }

        //Wait for all the files to be not in use
        public static async void waitForCompressFinish(string targetDirectory)
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

                            // wait for 1 second
                            await Task.Delay(1000);
                        }
                        //*/

                    }

                }



            }


           


        }

        //Delete a folder
        private void deleteAFolder(string targetDirectory)
        {
            var dir = new DirectoryInfo(targetDirectory);
            dir.Attributes = dir.Attributes & ~FileAttributes.ReadOnly;
            dir.Delete(true);



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
