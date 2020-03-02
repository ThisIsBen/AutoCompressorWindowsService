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
         //To display error message to the user
        public static void displayErrMsg(string errorSource,string errorMessage)
        {
            //output an error message to event log to indicate that which folder
            //can not be accessed.
            EventLogHandler.outputLog(errorMessage);

            Console.WriteLine(errorMessage);
            //Main.showMsgBoxFromWS(errorMessage, "Message from AutoCompressorWindowsService");

            //output error message to a txt file in 圧縮ソフト_エラーメッセージ folder in NAS
            outputErrorMessageTxt(errorSource,errorMessage, DynamicConstants.errorMessageTxtFolderPath);

        }

        //output error message to a txt file in 圧縮ソフトエラーメッセージ folder in NAS
        public static void outputErrorMessageTxt(string errorSource, string errorMessage, string outputErrMsgTxtFolderPath)
        {
            //output error message to a txt file in 圧縮ソフトエラーメッセージ folder in NAS
            File.WriteAllText(outputErrMsgTxtFolderPath + "\\" + DateTime.Now.ToString("yyyy_M_dd--HH_mm_ss") + errorSource+"エラー.txt", errorMessage);

        }


    }
}
