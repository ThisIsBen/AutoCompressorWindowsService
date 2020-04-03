using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AutoCompressorWindowsService
{
    class ReportErrorMsg
    {

        //文字コード(ここでは、Shift JIS)
        private static Encoding fileEncoding = Encoding.GetEncoding("shift_jis");


        //To display error message to the user
        public static void displayPopUpErrMsg(string errorSource,string errorMessage)
        {
            //output an error message to event log to indicate that which folder
            //can not be accessed.
            EventLogHandler.outputLog(errorMessage);

           
            Main.showMsgBoxFromWS(errorMessage, "Message from AutoCompressorWindowsService");

           

        }

        //output error message to a txt file in 圧縮ソフトエラーメッセージ folder in NAS
        public static  void outputErrorMessageTxt(string errorSource, string errorMessage, string outputErrMsgTxtFolderPath)
        {
            //check if the outputErrMsgTxtFolderPath is set to be a network drive
            if (IsANetworkDrive(outputErrMsgTxtFolderPath))
            {
                // The 圧縮ソフトエラーメッセージ folder is set to be at the network drive

                //only output the error message when there is network connection
                //when the 圧縮ソフトエラーメッセージ folder is set to be at the network drive
                if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                {
                    //output error message to a txt file in 圧縮ソフトエラーメッセージ folder in NAS
                    File.WriteAllText(outputErrMsgTxtFolderPath + "\\" + DateTime.Now.ToString("yyyy_MM_dd--HH_mm_ss") + errorSource + "エラー.txt", errorMessage, fileEncoding);
                }
            }
            // The 圧縮ソフトエラーメッセージ folder is set to be at the local drive
            else
            {
                //output error message to a txt file in 圧縮ソフトエラーメッセージ folder located in local drive
                File.WriteAllText(outputErrMsgTxtFolderPath + "\\" + DateTime.Now.ToString("yyyy_M_dd--HH_mm_ss") + errorSource + "エラー.txt", errorMessage, fileEncoding);
            }
            
        }

        //check if the outputErrMsgTxtFolderPath is set to be a network drive
        private static bool IsANetworkDrive(string outputErrMsgTxtFolderPath)
        {
            if(outputErrMsgTxtFolderPath[0]== '\\' && outputErrMsgTxtFolderPath[0]== '\\')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
