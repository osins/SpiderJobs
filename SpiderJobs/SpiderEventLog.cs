using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SpiderJobs
{
    public static class SpiderEventLog
    {
        public static void WriteLog(string sEvent)
        {
            WriteLog(sEvent, EventLogEntryType.Information);

        }

        public static void WriteWarningLog(string sEvent)
        {
            WriteLog(sEvent, EventLogEntryType.Warning);

        }

        public static void WriteLog(string sEvent, EventLogEntryType logType)
        {
            string sSource = "Spider Service Log";

            WriteSourceLog(sSource, sEvent, logType);
        }

        public static void WriteSourceLog(string sSource,string sEvent, EventLogEntryType logType)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(sEvent);

            Console.ForegroundColor = color;

            //string sLog = "Application";

            //if (!EventLog.SourceExists(sSource))
            //    EventLog.CreateEventSource(sSource, sLog);

            //EventLog.WriteEntry(sSource, sEvent,
            //    logType, 0);

        }
    }
}
