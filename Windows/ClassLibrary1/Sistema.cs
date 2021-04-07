using System;
using System.Collections.Generic;
using System.Management;
using System.Linq;
using System.Net;
using System.Diagnostics;
using System.Runtime.InteropServices;


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


            private ManagementObjectCollection Search_Wmi(string classe)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher($"SELECT * FROM {classe}");
            return searcher.Get();
        }

        // Convert Bytes to respective Size Measurement
        public static string GetReadableSize(Int64 value)
        {
            string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            if (value < 0) { return "-" + GetReadableSize(-value); }
            if (value == 0) { return "0.0 bytes"; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }
        public void MemoryUsageCustom() {
            ManagementObjectCollection TaskCollection = Search_Wmi("Win32_PerfRawData_PerfProc_Process");
            List<String> blacklist = new List<String>() // Blacklist of specific Logs to Hide
                {
                   "Idle",
                   "svchost",
                   "SearchFilterHost",
                   "SearchFilterHost",
                   "ShellExperienceHost",
                   "RuntimeBroker",
                   "RtkAudioService64",
                   "ServiceHub.ThreadedWaitDialog",
                   "SearchProtocolHost",
                   "StartMenuExperienceHost",
                   "ServiceHub.VSDetouredHost",
                   "SecurityHealthService",
                   "Memory Compression",
                   "IntelCpHDCPSvc",
                   "SettingSyncHost",
                   "YourPhone",
                   "ApplicationFrameHost",
                   "ServiceHub.SettingsHost",
                   "ServiceHub.DataWarehouseHost",
                   "MicrosoftEdgeCP",
                   "OneApp.IGCC.WinService",
                   "ServiceHub.Host.CLR.x86",
                   "Microsoft.ServiceHub.Controller",
                   "ServiceHub.RoslynCodeAnalysisService",
                   "TextInputHost",
                   "MicrosoftEdgeSH",
                   "fontdrvhost",
                   "igfxCUIService",
                   "SecurityHealthSystray",
                   "ServiceHub.IdentityHost",
                   "StandardCollector.Service",
                   "ScriptedSandbox64",
                   "dllhost",
                   "WmiPrvSE"
                };
            Process[] processList = Process.GetProcesses();
            Console.WriteLine("PID" + String.Concat(Enumerable.Repeat("-", 5)) + "Name" + String.Concat(Enumerable.Repeat("-", 32)) + "Memory Usage");
            foreach (Process processo in processList)
            {
                if (!blacklist.Contains(processo.ProcessName))
                {
                    Console.WriteLine(processo.Id + String.Concat(Enumerable.Repeat(" ", 8 - Convert.ToString(processo.Id).Length)) + processo.ProcessName + String.Concat(Enumerable.Repeat(" ", 38 - processo.ProcessName.Length)) + GetReadableSize(Convert.ToInt32(processo.PrivateMemorySize64)));
                }
            }
        }
    }
}
