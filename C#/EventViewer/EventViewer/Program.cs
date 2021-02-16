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
        static void readAlllogs(string logtype) 
        {
            EventLog[] logs_collect = EventLog.GetEventLogs("wdb-suporte04");
            EventLogEntryCollection entries = logs_collect[0].Entries;
            if (logtype == "system") 
            {
                entries = logs_collect[1].Entries;
            }
            else if (logtype == "application")
            {
                entries = logs_collect[0].Entries;
            }
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
                "Microsoft-Windows-WMI",
                "VSS",
                "Desktop Window Manager",
                "Application Error",
                "gupdate",
                "Application Hang",
                "System Restore",
                "Chkdsk",
                "Microsoft-Windows-Winsrv",
                ".NET Runtime Optimization Service",
                "Microsoft-Windows-Perflib",
                "Microsoft-Windows-CAPI2",
                "Microsoft-Windows-Defrag"
            };
            List<long> added = new List<long>();
            List<EventLogEntry> logs = new List<EventLogEntry>();
            Console.WriteLine("Index" + String.Concat(Enumerable.Repeat("-", 5))+ "ID" + String.Concat(Enumerable.Repeat("-", 20)) + "Name" + String.Concat(Enumerable.Repeat("-", 18)) + "Category" + String.Concat(Enumerable.Repeat("-", 12)) + "Time Generated");
            int count = -1;
            foreach (EventLogEntry entry in entries)
            {
                if (!blacklist.Contains(entry.Source) && (!added.Contains(entry.InstanceId)))
                {
                    count += 1;
                    Console.WriteLine(count + String.Concat(Enumerable.Repeat(" ", 7 - Convert.ToString(count).Length)) + entry.InstanceId + String.Concat(Enumerable.Repeat(" ", 25 - Convert.ToString(entry.InstanceId).Length)) + entry.Source + String.Concat(Enumerable.Repeat(" ", 22 - Convert.ToString(entry.Source).Length)) + entry.EntryType + String.Concat(Enumerable.Repeat(" ", 18 - Convert.ToString(entry.EntryType).Length)) + entry.TimeGenerated);
                    added.Add(entry.InstanceId);
                    logs.Add(entry);
                }
            }
            Console.WriteLine("Please insert log Index or press any non-numeric key to exit");
            string opc = Console.ReadLine();
            if (opc.All(char.IsDigit)) 
            {
                readlog(logs[Convert.ToInt32(opc)], Convert.ToInt32(opc));
            }
        }
        static void readlog(EventLogEntry log, int log_index) 
        {
            Console.WriteLine("Source: " + log.Source);
            Console.WriteLine("User: " + log.UserName);
            Console.WriteLine("Entry Type: " + log.EntryType);
            Console.WriteLine("Category: " + log.Category);
            Console.WriteLine("Time Generated: " + log.TimeGenerated);
            Console.WriteLine("Time Written: " + log.TimeWritten);
            Console.WriteLine("Container: " + log.Container);
            Console.WriteLine("Event ID: " + log.EventID);
            Console.WriteLine("Index in Event Log: " + log.Index);
            Console.WriteLine("Instance ID: " + log.InstanceId);
            Console.WriteLine(" --- Message ---\n" + log.Message + "\n --- End Message ---");
            Console.ReadLine();
        }

        static void Main(string[] args)
        {
            Console.WriteLine("1 - Sistema");
            Console.WriteLine("2 - Aplicação");
            int opc = Convert.ToInt32(Console.ReadLine());
            switch (opc) 
            {
                case 1:
                    readAlllogs("system");
                    break;
                case 2:
                    readAlllogs("application");
                    break;
                default:
                    break;
            }
        }
    }
}
