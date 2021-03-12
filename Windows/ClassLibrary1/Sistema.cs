using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Diagnostics;

namespace ClassLibrary
{
    public class Sistema
    {
        // Gets name of current machine for use in LogReader
        public string hostname { get { return Dns.GetHostName(); } set { hostname = Dns.GetHostName(); } }

        public void rede()
        {
            // Executes ipconfig /all command and prints output
            string command = "ipconfig /all";
            Process processo = new Process();
            processo.StartInfo.FileName = "powershell.exe";
            processo.StartInfo.UseShellExecute = false;
            processo.StartInfo.RedirectStandardOutput = true;
            processo.StartInfo.Arguments = "/c " + command;
            processo.Start();
            string output = processo.StandardOutput.ReadToEnd();
            processo.WaitForExit();
            Console.WriteLine(output);
        }

        public void readlogs(string logtype)
        {
            // Display specific log info
            void readSinglelog(EventLogEntry log)
            {
                Console.WriteLine("Source: " + log.Source);
                Console.WriteLine("Entry Type: " + log.EntryType);
                Console.WriteLine("Category: " + log.Category);
                Console.WriteLine("Time Generated: " + log.TimeGenerated);
                Console.WriteLine("Time Written: " + log.TimeWritten);
                Console.WriteLine("Index in Event Log: " + log.Index);
                Console.WriteLine("Instance ID: " + log.InstanceId);
                Console.WriteLine(" --- Message ---\n" + log.Message + "\n --- End Message ---");
            }

            EventLog[] logs_collect = EventLog.GetEventLogs(hostname);
            EventLogEntryCollection entries = logs_collect[0].Entries;
            if (logtype == "system") { entries = logs_collect[8].Entries; } else if (logtype == "application") { entries = logs_collect[0].Entries; } else if (logtype == "security") { entries = logs_collect[6].Entries; } else if (logtype == "hardware") { entries = logs_collect[1].Entries; }
            List<String> blacklist = new List<String>() // Blacklist of specific Logs to Hide
                {
                // Application
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
                    "Microsoft-Windows-Defrag",
                    "Microsoft-Windows-RPC-Events",
                // System
                    "Microsoft-Windows-Hyper-V-VmSwitch",
                    "Microsoft-Windows-WindowsUpdateClient",
                    "Microsoft-Windows-StartupRepair",
                    "Microsoft-Windows-Kernel-Power",
                    "Microsoft-Windows-Kernel-Power",
                    "Microsoft-Windows-GroupPolicy",
                    "Microsoft-Windows-DNS-Client",
                    "Microsoft-Windows-DriverFrameworks-UserMode",
                    "Microsoft-Windows-Directory-Services-SAM",
                    "Microsoft-Windows-Kernel-General",
                    "Microsoft-Windows-TaskScheduler",
                    "Microsoft-Windows-UserModePowerService",
                    "Microsoft-Windows-Hyper-V-Hypervisor",
                    "Microsoft-Windows-Time-Service",
                    "Microsoft-Windows-DHCPv6-Client",
                    "Microsoft-Windows-Kernel-Processor-Power",
                    "Microsoft-Windows-WLAN-AutoConfig"
                   };
            List<long> added = new List<long>(); // List used to know which Log entry has already been added
            List<EventLogEntry> logs = new List<EventLogEntry>();
            Console.WriteLine("Index" + String.Concat(Enumerable.Repeat("-", 5)) + "ID" + String.Concat(Enumerable.Repeat("-", 15)) + "Name" + String.Concat(Enumerable.Repeat("-", 30)) + "Category" + String.Concat(Enumerable.Repeat("-", 12)) + "Time Generated");
            int count = 0; // Used for indexing Logs, easier to search later on
            try
            {
                foreach (EventLogEntry entry in entries)
                {
                    if (!blacklist.Contains(entry.Source) && (!added.Contains(entry.InstanceId)))
                    // If log not in blacklist and log entry hasn't been added
                    {
                        //Console.WriteLine(entry.Source);
                        Console.WriteLine(count + String.Concat(Enumerable.Repeat(" ", 7 - Convert.ToString(count).Length)) + entry.InstanceId + String.Concat(Enumerable.Repeat(" ", 20 - Convert.ToString(entry.InstanceId).Length)) + entry.Source + String.Concat(Enumerable.Repeat(" ", 34 - Convert.ToString(entry.Source).Length)) + entry.EntryType + String.Concat(Enumerable.Repeat(" ", 18 - Convert.ToString(entry.EntryType).Length)) + entry.TimeGenerated);
                        added.Add(entry.InstanceId);
                        logs.Add(entry);
                        count += 1;
                    }
                }
                do
                {
                    Console.WriteLine("\n\nPlease insert log Index or enter any non-numeric value to exit");
                    string idx = Console.ReadLine();
                    Console.WriteLine("\n\n");
                    try
                    {
                        if (idx.All(char.IsDigit)) { readSinglelog(logs[Convert.ToInt32(idx)]); } // If user passed in Int value to get specific Log info 
                        else { break; }
                    }
                    catch (FormatException) { break; }

                } while (true); // Loop readSingleLog until user inputs non-numeric value.
            }
            catch (System.Security.SecurityException) { Console.WriteLine("Permission error"); }

        }
    }



}
