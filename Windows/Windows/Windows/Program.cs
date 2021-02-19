using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System.Diagnostics;
using System.Management;
using System.IO;


namespace Windows
{
    class Program
    {

        static int print_menu()
        {
            Console.WriteLine("1 - Discos");
            Console.WriteLine("2 - Rede");
            Console.WriteLine("3 - Host");
            Console.WriteLine("4 - Uso de Memória");
            Console.WriteLine("5 - Uso de CPU");
            Console.WriteLine("6 - Logs");
            Console.WriteLine("99 - Sair");
            int main_menu = Convert.ToInt32(Console.ReadLine());
            return main_menu;
        }


        static void Main(string[] args)
        {
            Hardware infra = new Hardware();
            bool exit = false;
            do
            {
                int main_menu = print_menu();
                switch (main_menu)
                {
                    case 1:
                        Console.Clear();
                        infra.list_disks();
                        // Disks
                        break;
                    case 2:
                        Console.Clear();
                        break;
                    case 3:
                        Console.Clear();
                        // Host Info
                        break;
                    case 4:
                        Console.Clear();
                        // Memory Usage
                        break;
                    case 5:
                        Console.Clear();
                        // CPU Usage
                        break;
                    case 6:
                        Console.Clear();
                        // Logs
                        Console.WriteLine("1 - Aplicação");
                        Console.WriteLine("2 - Sistema");
                        int logtype = Convert.ToInt32(Console.ReadLine());
                        Console.Clear();
                        switch (logtype)
                        {
                            case 1:
                                Console.WriteLine("--- Application Logs ---");
                                infra.readlogs("application");
                                break;
                            case 2:
                                Console.WriteLine("--- System Logs ---");
                                infra.readlogs("system");
                                break;
                        }
                        break;
                    case 99:
                        exit = true;
                        break;
                    default:
                        break;
                }
            } while (!exit);
        }
    }

    
    public class Hardware
    {

        public static decimal getPercentage(decimal total, decimal valor)
        {
            // Calc percentage
            decimal percentageValue = (valor * 100) / total;
            return percentageValue;
        }

        // Convert Bytes to respective Size Measurement
        public static string getReadableSize(Int64 value)
        {
            string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            if (value < 0) { return "-" + getReadableSize(-value); }
            if (value == 0) { return "0.0 bytes"; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }

        // Gets name of current machine for use in LogReader
        public string hostname { get { return Dns.GetHostName(); } set { hostname = Dns.GetHostName(); } }
         
        // Used to query WMI class
        private ManagementObjectSearcher search_wmi(string phrase)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(phrase);
            return searcher;
        }

        public void list_disks()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            Console.WriteLine("Partição" + String.Concat(Enumerable.Repeat("-", 5)) + "Tipo" + String.Concat(Enumerable.Repeat("-", 6)) + "Formato" + String.Concat(Enumerable.Repeat("-", 10)) +  "Espaço Total" + String.Concat(Enumerable.Repeat("-", 10)) + "Espaço Livre" + String.Concat(Enumerable.Repeat("-", 10)) + "Espaço Usado" + String.Concat(Enumerable.Repeat("-", 10)) + "Integridade");
            foreach (DriveInfo drive in allDrives)
            {
                if (drive.IsReady)
                {
                    string tipo = "";
                    string usedSpace = getReadableSize(drive.TotalSize - drive.AvailableFreeSpace);
                    // Disk type translation
                    String health = "";
                    if (Convert.ToString(drive.DriveType) == "Fixed") { tipo = "Interno"; } else if (Convert.ToString(drive.DriveType) == "Removable") { tipo = "Externo"; } else if (Convert.ToString(drive.DriveType) == "CDRom") { tipo = "CD/DVD"; } else if (Convert.ToString(drive.DriveType) == "Network") { tipo = " Rede"; }
                    if (tipo != " Rede")
                    {
                        String command = "chkdsk " + drive.Name.Substring(0, 2);
                        System.Diagnostics.Process processo = new System.Diagnostics.Process();
                        processo.StartInfo.FileName = "Powershell.exe";
                        processo.StartInfo.UseShellExecute = false;
                        processo.StartInfo.RedirectStandardOutput = true;
                        processo.StartInfo.Arguments = "/c " + command;
                        processo.StartInfo.Verb = "runas";
                        processo.Start();
                        string output = processo.StandardOutput.ReadToEnd();
                        processo.WaitForExit();
                        Console.WriteLine(output);
                        if (output == "Não há problemas")
                        {
                            health = "Íntrego";
                        }
                        else
                        {
                            health = "Não Íntegro";
                        }
                    }
                    decimal percentFree = decimal.Round(getPercentage(drive.TotalSize, drive.TotalSize - drive.TotalFreeSpace));
                    decimal percentUsed = decimal.Round(100 - percentFree);
                    Console.WriteLine(String.Concat(Enumerable.Repeat(" ", 5 - Convert.ToString(drive.Name).Length)) + drive.Name + String.Concat(Enumerable.Repeat(" ", 10 - Convert.ToString(drive.Name).Length)) + tipo + String.Concat(Enumerable.Repeat(" ", 13 - Convert.ToString(tipo).Length)) + drive.DriveFormat + String.Concat(Enumerable.Repeat(" ", 17 - Convert.ToString(drive.DriveFormat).Length)) + getReadableSize(drive.TotalSize) + String.Concat(Enumerable.Repeat(" ", 20 - Convert.ToString(getReadableSize(drive.TotalSize)).Length)) + $"{getReadableSize(drive.TotalFreeSpace)} {percentFree}%" + String.Concat(Enumerable.Repeat(" ", 18 - Convert.ToString(getReadableSize(drive.TotalFreeSpace)).Length)) + $"{usedSpace} {percentUsed}%" + String.Concat(Enumerable.Repeat(" ", 22 - (usedSpace.Length + Convert.ToString(percentUsed).Length))));
                    Console.Write(health);
                }
            }
        }

        public void readlogs(string logtype)
        {
            // Display specific log info
            void readSinglelog(EventLogEntry log)
            {
                Console.WriteLine("Source: " + log.Source);
                Console.WriteLine("User: " + log.UserName);
                Console.WriteLine("Entry Type: " + log.EntryType);
                Console.WriteLine("Category: " + log.Category);
                Console.WriteLine("Time Generated: " + log.TimeGenerated);
                Console.WriteLine("Time Written: " + log.TimeWritten);
                Console.WriteLine("Container: " + log.Container);
                Console.WriteLine("Index in Event Log: " + log.Index);
                Console.WriteLine("Instance ID: " + log.InstanceId);
                Console.WriteLine(" --- Message ---\n" + log.Message + "\n --- End Message ---");
            }

            EventLog[] logs_collect = EventLog.GetEventLogs(hostname);
            EventLogEntryCollection entries = logs_collect[0].Entries;
            if (logtype == "system") { entries = logs_collect[1].Entries; } else if (logtype == "application") { entries = logs_collect[0].Entries; }
            List<String> blacklist = new List<String>()
            // Blacklist of specific Logs to Hide
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
            List<long> added = new List<long>(); // List used to know which Log entry has already been added
            List<EventLogEntry> logs = new List<EventLogEntry>();
            Console.WriteLine("Index" + String.Concat(Enumerable.Repeat("-", 5)) + "ID" + String.Concat(Enumerable.Repeat("-", 15)) + "Name" + String.Concat(Enumerable.Repeat("-", 18)) + "Category" + String.Concat(Enumerable.Repeat("-", 12)) + "Time Generated");
            int count = 0; // Used for indexing Logs, easier to search later on
            foreach (EventLogEntry entry in entries)
            {
                if (!blacklist.Contains(entry.Source) && (!added.Contains(entry.InstanceId)))
                    // If log not in blacklist and log entry hasn't been added
                {
                    
                    Console.WriteLine(count + String.Concat(Enumerable.Repeat(" ", 7 - Convert.ToString(count).Length)) + entry.InstanceId + String.Concat(Enumerable.Repeat(" ", 20 - Convert.ToString(entry.InstanceId).Length)) + entry.Source + String.Concat(Enumerable.Repeat(" ", 22 - Convert.ToString(entry.Source).Length)) + entry.EntryType + String.Concat(Enumerable.Repeat(" ", 18 - Convert.ToString(entry.EntryType).Length)) + entry.TimeGenerated);
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
                if (idx.All(char.IsDigit)) { readSinglelog(logs[Convert.ToInt32(idx)]); } // If user passed in Int value to get specific Log info
                else { break; }
            } while (true); // Loop readSingleLog until user inputs non-numeric value.
        }
        
    }
}