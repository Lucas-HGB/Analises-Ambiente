using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace EventViewer
{
    class Program
    {
        static void Main(string[] args)
        {
            EventLog[] logs;
            logs = EventLog.GetEventLogs("wdb-suporte04");
            Console.WriteLine(logs[0]);
            EventLogEntryCollection entries = logs[0].Entries;
            List<String> blacklist = new List<String>()
            {
                "ESENT",
                "Group Policy Printers",
                "Windows Search Service",
                "Software Protection Platform Service",
                "MsiInstaller",
                "Microsoft-Windows-RestartManager",
                "Microsoft-Windows-User Profiles Service",
                "Microsoft-Windows-System-Restore",
                "SecurityCenter",
                "edgeupdate",
                "outlook",
                "Windows Search Service Profile Notification",
                "Windows Error Reporting",
                "Microsoft-Windows-AppModel-State",
                "Microsoft-Windows-WMI"
            };

            foreach (EventLogEntry entry in entries)
            {
                if (!blacklist.Contains(entry.Source))
                {
                    Console.WriteLine(entry.Source);
                }
            }
            Console.ReadLine();
        }
    }
}
