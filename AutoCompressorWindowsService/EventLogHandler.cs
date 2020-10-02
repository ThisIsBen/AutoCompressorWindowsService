using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCompressorWindowsService
{
    //Output log to イベントビューアー
    class EventLogHandler
    {

        static System.Diagnostics.EventLog eventLog;

        // Create an EventLog in イベントビューアー for monitoring this app
        public static void createEventlog(string eventLogSource, string eventLogName)
        {


            // Check if the event source exists. If not create it.
            if (!System.Diagnostics.EventLog.SourceExists(eventLogSource))
            {
                System.Diagnostics.EventLog.CreateEventSource(eventLogSource, eventLogName);
            }

            // Create an instance of EventLog
            eventLog = new System.Diagnostics.EventLog();
            eventLog.Source = eventLogSource;
        }

        //Output log to the イベントビューアー
        public static void outputLog(string logContent)
        {
            eventLog.WriteEntry(logContent);

        }
    }
}
