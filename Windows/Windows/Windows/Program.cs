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
            Sistema sys = new Sistema();
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
                        sys.rede();
                        break;
                    case 3:
                        Console.Clear();
                        infra.host();
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
                                sys.readlogs("application");
                                break;
                            case 2:
                                Console.WriteLine("--- System Logs ---");
                                sys.readlogs("system");
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

        public void host()
        {
            ManagementObjectCollection cpu = search_wmi("Win32_Processor");
            foreach (ManagementObject obj in cpu)
            {

                Console.WriteLine($"Modelo: {obj["Name"]}");
                Console.WriteLine($"Nr de Cores: {obj["NumberOfCores"]}");
                Console.WriteLine($"Nr de Processadores lógicos: {obj["NumberOfLogicalProcessors"]}");
                Console.WriteLine($"Sistema: {obj["AddressWidth"]} bits");
                Console.WriteLine($"Load Percentage: {obj["LoadPercentage"]}");
            }
            ManagementObjectCollection computer = search_wmi("Win32_ComputerSystem");
            foreach (ManagementObject obj in computer)
            {
                Console.WriteLine($"Memória: {getReadableSize(long.Parse(obj["TotalPhysicalMemory"].ToString()))}");
                Console.WriteLine($"Hostname: {obj["Domain"]}/{obj["Caption"]}");
                int func_dominio_int = Int32.Parse(obj["DomainRole"].ToString());
                String func_dominio = "";
                if (func_dominio_int == 0) { func_dominio = "Standalone Worksation"; }
                else if (func_dominio_int == 1) { func_dominio = "Member Worksation"; }
                else if (func_dominio_int == 2) { func_dominio = "Standalone Server"; }
                else if (func_dominio_int == 3) { func_dominio = "Member Server"; }
                else if (func_dominio_int == 4) { func_dominio = "Backup Domain Controller"; }
                else if (func_dominio_int == 5) { func_dominio = "Primary Domain Controller"; }
                Console.WriteLine($"Função do Domínio: {func_dominio}");
                Console.WriteLine($"Modelo: {obj["Model"]}");
                Console.WriteLine($"Arquitetura: {obj["SystemType"]}");
                Console.WriteLine($"Usuário: {obj["Username"]}");
                string command = "(cscript  'C:\\Windows\\System32\\slmgr.vbs' /dli)"; Process processo = new Process();
                processo.StartInfo.FileName = "powershell.exe";
                processo.StartInfo.UseShellExecute = false;
                processo.StartInfo.RedirectStandardOutput = true;
                processo.StartInfo.Arguments = "/c " + command;
                processo.Start();
                string output = processo.StandardOutput.ReadToEnd();
                processo.WaitForExit();
                Console.WriteLine(output);
            }
        }

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

        // Used to query WMI class
        private ManagementObjectCollection search_wmi(string classe)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher($"SELECT * FROM {classe}");
            return searcher.Get();
        }

        public void list_disks()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            Console.WriteLine("Partição" + String.Concat(Enumerable.Repeat("-", 5)) + "Tipo" + String.Concat(Enumerable.Repeat("-", 6)) + "Formato" + String.Concat(Enumerable.Repeat("-", 10)) + "Espaço Total" + String.Concat(Enumerable.Repeat("-", 10)) + "Espaço Livre" + String.Concat(Enumerable.Repeat("-", 10)) + "Espaço Usado");
            foreach (DriveInfo drive in allDrives)
            {
                if (drive.IsReady)
                {
                    string tipo = "";
                    string usedSpace = getReadableSize(drive.TotalSize - drive.AvailableFreeSpace);
                    // Disk type translation

                    if (Convert.ToString(drive.DriveType) == "Fixed") { tipo = "Interno"; } else if (Convert.ToString(drive.DriveType) == "Removable") { tipo = "Externo"; } else if (Convert.ToString(drive.DriveType) == "CDRom") { tipo = "CD/DVD"; } else if (Convert.ToString(drive.DriveType) == "Network") { tipo = " Rede"; }
                    //String health = "";
                    //if (tipo != " rede")
                    //{
                    //    string command = "chkdsk " + drive.name.substring(0, 2);
                    //    system.diagnostics.process processo = new system.diagnostics.process();
                    //    processo.startinfo.filename = "powershell.exe";
                    //    processo.startinfo.useshellexecute = false;
                    //    processo.startinfo.redirectstandardoutput = true;
                    //    processo.startinfo.arguments = "/c " + command;
                    //    processo.startinfo.verb = "runas";
                    //    processo.start();
                    //    string output = processo.standardoutput.readtoend();
                    //    processo.waitforexit();
                    //    console.writeline(output);
                    //    if (output == "não há problemas")
                    //    {
                    //        health = "íntrego";
                    //    }
                    //    else
                    //    {
                    //        health = "não íntegro";
                    //    }
                    //}
                    decimal percentUsed = decimal.Round(getPercentage(drive.TotalSize, drive.TotalSize - drive.TotalFreeSpace));
                    decimal percentFree = decimal.Round(100 - percentUsed);
                    Console.WriteLine(String.Concat(Enumerable.Repeat(" ", 5 - Convert.ToString(drive.Name).Length)) + drive.Name + String.Concat(Enumerable.Repeat(" ", 10 - Convert.ToString(drive.Name).Length)) + tipo + String.Concat(Enumerable.Repeat(" ", 13 - Convert.ToString(tipo).Length)) + drive.DriveFormat + String.Concat(Enumerable.Repeat(" ", 17 - Convert.ToString(drive.DriveFormat).Length)) + getReadableSize(drive.TotalSize) + String.Concat(Enumerable.Repeat(" ", 20 - Convert.ToString(getReadableSize(drive.TotalSize)).Length)) + $"{getReadableSize(drive.TotalFreeSpace)} {percentFree}%" + String.Concat(Enumerable.Repeat(" ", 22 - $"{getReadableSize(drive.TotalFreeSpace)} {percentFree}%".Length)) + $"{usedSpace} {percentUsed}%" + String.Concat(Enumerable.Repeat(" ", 18 - $"{usedSpace} {percentUsed}%".Length)));
                }
            }
        }
    }

public class Sistema
    {
        // Gets name of current machine for use in LogReader
        public string hostname { get { return Dns.GetHostName(); } set { hostname = Dns.GetHostName(); } }

        public void rede()
        {
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
                try
                {
                    if (idx.All(char.IsDigit)) { readSinglelog(logs[Convert.ToInt32(idx)]); } // If user passed in Int value to get specific Log info 
                    else { break; }
                } catch (FormatException) { break; }

            } while (true); // Loop readSingleLog until user inputs non-numeric value.
        }
    }
       
        

}