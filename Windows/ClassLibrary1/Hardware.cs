using System;
using System.Management;
using System.IO;
using System.Linq;

using System.Diagnostics;

namespace ClassLibrary
{
    public class Hardware
    {

        public void host()
        {
            ManagementObjectCollection cpu = Search_Wmi("Win32_Processor");
            foreach (ManagementObject obj in cpu)
            {
                // Informações da CPU
                Console.WriteLine($"Modelo: {obj["Name"]}");
                Console.WriteLine($"Nr de Cores: {obj["NumberOfCores"]}");
                Console.WriteLine($"Nr de Processadores lógicos: {obj["NumberOfLogicalProcessors"]}");
                Console.WriteLine($"Sistema: {obj["AddressWidth"]} bits");
                Console.WriteLine($"Load Percentage: {obj["LoadPercentage"]}");
            }
            ManagementObjectCollection computer = Search_Wmi("Win32_ComputerSystem");
            foreach (ManagementObject obj in computer)
            {
                // Informações do Computador
                Console.WriteLine($"Memória: {GetReadableSize(long.Parse(obj["TotalPhysicalMemory"].ToString()))}");
                Console.WriteLine($"Hostname: {obj["Domain"]}/{obj["Caption"]}");
                int func_dominio_int = Int32.Parse(obj["DomainRole"].ToString()); String func_dominio = "";
                // Translates função no domínio de número para "Human-readable"
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

        public static decimal GetPercentage(decimal total, decimal valor)
        {
            // Calc percentage
            decimal percentageValue = (valor * 100) / total;
            return percentageValue;
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

        // Used to query WMI class
        private ManagementObjectCollection Search_Wmi(string classe)
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
                    string usedSpace = GetReadableSize(drive.TotalSize - drive.AvailableFreeSpace);
                    // Disk type translation
                    if (Convert.ToString(drive.DriveType) == "Fixed") { tipo = "Interno"; } else if (Convert.ToString(drive.DriveType) == "Removable") { tipo = "Externo"; } else if (Convert.ToString(drive.DriveType) == "CDRom") { tipo = "CD/DVD"; } else if (Convert.ToString(drive.DriveType) == "Network") { tipo = " Rede"; }
                    decimal percentUsed = decimal.Round(GetPercentage(drive.TotalSize, drive.TotalSize - drive.TotalFreeSpace));
                    decimal percentFree = decimal.Round(100 - percentUsed);
                    Console.WriteLine(String.Concat(Enumerable.Repeat(" ", 5 - Convert.ToString(drive.Name).Length)) + drive.Name + String.Concat(Enumerable.Repeat(" ", 10 - Convert.ToString(drive.Name).Length)) + tipo + String.Concat(Enumerable.Repeat(" ", 13 - Convert.ToString(tipo).Length)) + drive.DriveFormat + String.Concat(Enumerable.Repeat(" ", 17 - Convert.ToString(drive.DriveFormat).Length)) + GetReadableSize(drive.TotalSize) + String.Concat(Enumerable.Repeat(" ", 20 - Convert.ToString(GetReadableSize(drive.TotalSize)).Length)) + $"{GetReadableSize(drive.TotalFreeSpace)} {percentFree}%" + String.Concat(Enumerable.Repeat(" ", 22 - $"{GetReadableSize(drive.TotalFreeSpace)} {percentFree}%".Length)) + $"{usedSpace} {percentUsed}%" + String.Concat(Enumerable.Repeat(" ", 18 - $"{usedSpace} {percentUsed}%".Length)));
                }
            }
        }
    }
}

