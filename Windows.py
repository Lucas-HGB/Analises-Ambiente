#!/usr/bin/python2
# -*- coding: UTF-8 -*-
from os import system
from platform import system as arch
from subprocess import Popen, PIPE
from wmi import WMI
from Convert import math_size

def clear_screen():
    if arch().lower() == "windows":
        system("cls")
    else:
        system("clear")

class Hardware():

    def __init__(self):
        self.manager = WMI()
        self.processor = self.manager.Win32_Processor()
        self.computer = self.manager.Win32_ComputerSystem()
        self.disks = self.manager.Win32_DiskDrive()

    def cpu_mem(self):
        print "Modelo: " + self.processor[0].Name
        print "Nr de Cores: " + str(self.processor[0].NumberofCores)
        print "Nr de processadores logicos: " + str(self.processor[0].NumberofLogicalProcessors)
        print "Sistema: " + str(self.processor[0].AddressWidth) + " bits"
        print "Load Percentage: " + str(self.processor[0].LoadPercentage)
        print "Memoria : " + math_size(self.computer[0].TotalPhysicalMemory)


    def list_disks(self):
        ## To-do:
        # Checar integridade, https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-logicaldisk
        print "Performing Disk check, please wait"
        print "\n"
        print "Hardware" + "-"*(25) + "Partition" + "-"*(10) + "Total Space" + "-"*(10) + "Free Space" + "-"*(15) + "Used Space" + "-"*(15) + "Integridade"
        for physical_disk in self.disks:
            for partition in physical_disk.associators("Win32_DiskDriveToDiskPartition"):
                for logical_disk in partition.associators("Win32_LogicalDiskToPartition"):
                    used_space = math_size(float(logical_disk.Size) - float(logical_disk.FreeSpace))
                    percent_free = round((float(str(logical_disk.FreeSpace).split()[0]) * 100) / float(str(logical_disk.Size).split()[0]), 1)
                    percent_used =  round((100 - percent_free), 1)
                    #status = str(logical_disk.Chkdsk())[1]
                    status = str(logical_disk.BlockSize)
                    print physical_disk.Caption + " "*(37 - len(physical_disk.Caption)) + logical_disk.Caption + " "*(14) + math_size(logical_disk.Size) + " "*(20 - len(logical_disk.Size)) + "%s (%s%s)" % (math_size(logical_disk.FreeSpace), percent_free, r"%") + " "*(23 - len(logical_disk.FreeSpace) - len(str(percent_free)) + 1) + "%s (%s%s)" % (used_space, percent_used, r"%") + " "*(28 - len(str(used_space)) - len(str(percent_used)) + 1) + status

class System():

    def __init__(self):
        self.manager = WMI()
        self.IDs = []
        self.Names = []
        self.WorkingSets = []
        self.PercentProcessorTimes = []
        self.computer = self.manager.Win32_ComputerSystem()


    def print_descending_mem(self):
        for num in range(len(self.WorkingSets)):
            index = self.WorkingSets.index(max(self.WorkingSets))
            print str(self.IDs[index]) + " "*(18 - len(str(self.IDs[index]))) + str(self.Names[index]) + " "*(27 - len(self.Names[index])) + str(self.WorkingSets[index]) + " "*(27 - len(str(self.WorkingSets[index]))) + str(self.PercentProcessorTimes[index])
            self.WorkingSets.pop(index)

    def mem_use(self):
        ## To-do:
        # Calc correct value for memory use per process
        printed = []
        blacklist = [
            "System Idle Process",
            "System",
            "WirelessKB850NotificationService",
            "WindowsInternal.ComposableShell.Experiences.TextInput.InputApp"
        ]
        print "PID" + "-"*15 + "Process name" + "-"*15 + "Memory Usage" + "-"*15 + "CPU Usage"
        for process in self.manager.Win32_PerfFormattedData_perfProc_Process():
            if process.Name not in blacklist and process.Name not in printed and "#" not in process.Name:
                self.IDs.append(int(process.IDprocess))
                self.Names.append(process.Name)
                self.WorkingSets.append(int(process.WorkingSet))
                self.PercentProcessorTimes.append(int(process.PercentProcessorTime))
                printed.append(process.Name)
        self.print_descending_mem()

    def print_descending_cpu_usage(self):
        printed = []
        for num in range(len(self.PercentProcessorTimes)):
            index = self.PercentProcessorTimes.index(max(self.PercentProcessorTimes))
            if index not in printed:
                print str(self.IDs[index]) + " "*(18 - len(str(self.IDs[index]))) + str(self.Names[index]) + " "*(27 - len(self.Names[index])) + str(self.WorkingSets[index]) + " "*(27 - len(str(self.WorkingSets[index]))) + str(self.PercentProcessorTimes[index])
            self.PercentProcessorTimes.pop(index)
            printed.append(index)

    def cpu_use(self):
        ## To-do:
        # Calc correct value for CPU usage per process
        printed = []
        blacklist = [
            "System Idle Process",
            "System",
            "WirelessKB850NotificationService",
            "WindowsInternal.ComposableShell.Experiences.TextInput.InputApp"
        ]
        print "PID" + "-"*15 + "Process name" + "-"*15 + "Memory Usage" + "-"*15 + "CPU Usage"
        for process in self.manager.Win32_PerfFormattedData_perfProc_Process():
            if process.Name not in blacklist and process.Name not in printed and "#" not in process.Name:
                self.IDs.append(int(process.IDprocess))
                self.Names.append(process.Name)
                self.WorkingSets.append(int(process.WorkingSet))
                self.PercentProcessorTimes.append(int(process.PercentProcessorTime))
                printed.append(process.Name)
        self.print_descending_cpu_usage()

    def rede(self):
        ## To-do:
        # Check way to clear screen after using this config, why 
        system("ipconfig /all")
        print "\n\n"

    def host(self):
        print "Hostname: " + self.computer[0].Domain + "/" + self.computer[0].Caption
        print "Modelo: " + self.computer[0].Model
        print "Arquitetura: " + self.computer[0].SystemType
        print "Usuario: " + self.computer[0].Username.split("'\'")[-1]
        system('(cscript  "C:\Windows\System32\slmgr.vbs" /dli)')

    def logs(self):
        exit = False
        while not exit:
            print "1 - Sistema"
            print "2 - Aplicação"
            opc = input()
            if opc == 1:
                print "---Sistema---"
                print "Número de eventos: " + str(self.manager.Win32_NTEventLogFile()[-2].NumberOfRecords)
            elif opc == 2:
                print "---Aplicação---"
                print "Número de eventos: " + str(self.manager.Win32_NTEventLogFile()[0].NumberOfRecords)
                print "----Eventos----"
                print self.manager.Win32_NTEventLogFile()[0]

def print_menu():
    print "\n"
    print "1 - Discos"
    print "2 - Rede"
    print "3 - Host"
    print "4 - Uso de memória"
    print "5 - Uso de CPU"
    print "6 - Logs"
    print "99 - Sair"

def init():
    clear_screen()
    sys = System()
    hardware = Hardware()
    exit = False
    while not exit:
        print_menu()
        opc = input()
        clear_screen()
        if opc == 1:
            hardware.list_disks()
        elif opc == 2:
            sys.rede()
        elif opc == 3:
            hardware.cpu_mem()
            sys.host()
        elif opc == 4:
            sys.mem_use()
        elif opc == 5:
            sys.cpu_use()
        elif opc == 6:
            sys.logs()
        elif opc == 99:
            exit = True
            break


if __name__ == "__main__":
    sys = System()
    hardware = Hardware()
    hardware.list_disks()
