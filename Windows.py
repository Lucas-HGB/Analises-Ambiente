#!/usr/bin/python2
# -*- coding: UTF-8 -*-
from os import system
from platform import system as arch
from subprocess import Popen, PIPE
from wmi import WMI
from Convert import math_size
from win32evtlog import ReadEventLog, OpenEventLog, EVENTLOG_BACKWARDS_READ, EVENTLOG_SEQUENTIAL_READ, GetNumberOfEventLogRecords, CloseEventLog
from win32evtlogutil import SafeFormatMessage

def clear_screen():
    system("cls")

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
        status = "0"
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
                    #status = str(physical_disk.HealthStatus)
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
        # Check way to clear screen after using this func
        system("ipconfig /all")
        print "\n\n"

    def host(self):
        print "Hostname: " + self.computer[0].Domain + "/" + self.computer[0].Caption
        print "Modelo: " + self.computer[0].Model
        print "Arquitetura: " + self.computer[0].SystemType
        print "Usuario: " + self.computer[0].Username.split("'\'")[-1]
        system('(cscript  "C:\Windows\System32\slmgr.vbs" /dli)')

class Logs():

    def __init__(self, logtype):
        self.flags = EVENTLOG_BACKWARDS_READ|EVENTLOG_SEQUENTIAL_READ
        self.events = []
        self.logtype = logtype


    def read_logs(self):
        blacklist = [
        ## Aplicação
        "MsiInstaller",
        "Windows Search Service Profile Notification",
        "Group Policy Printers",
        "Outlook"
        "VSS",
        "Software Protection Platform Service",
        "Windows Search Service",
        "Windows Search Service",
        "ESENT",
        "Desktop Window Manager",
        "Windows Error Reporting",
        "Group Policy Drive Maps",
        "SecurityCenter",
        "Wlclntfy",
        "edgeupdate",
        "System Restore",

        ## Sistema
        "Service Control Manager"
        ]
        self.hand = OpenEventLog("localhost",self.logtype)
        added = []
        print "ID" + "-"*20 + "Name" + "-"*15 + "Category" + "-"*12 + "Time Generated"
        events = [0]
        count = 0
        while len(events) > 0:
            events = [f for f in ReadEventLog(self.hand, self.flags, 0) if f]
            for event in events:
                 count += 1
            if event.EventID not in added and "microsoft" not in event.SourceName.lower().split("-") and event.SourceName not in blacklist and event.EventType >= 2:
                print str(event.EventID).replace("-", "") + " "*(22 - len(str(event.EventID).replace("-", ""))) + event.SourceName + " "*(22 - len(str(event.SourceName))) + str(event.EventType) + " "*(16 - len(str(event.EventType))) + str(event.TimeGenerated)
                self.events.append(event.SourceName)
                added.append(event.EventID)
            if len(events) == 0:
                break
            

    def read_log(self, eventid):
        self.hand = OpenEventLog("localhost",self.logtype)
        added = []
        events = 1
        while events:
            events = [f for f in ReadEventLog(self.hand, self.flags, 0) if f]
            for event in events:
                if event.EventID == eventid and event.EventID not in added:
                    added.append(event.EventID)
                    try:
                        print SafeFormatMessage(event, self.logtype)
                    except UnicodeEncodeError:
                        print "Unicode error while reading log"
                        continue

    def __str__(self):
        phrase = tipo + " events: " + str(GetNumberOfEventLogRecords(OpenEventLog(hand))) + "\n"
        return phrase

    def get_common_events(self):
        count = 0


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
    while True:
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
            while True:
                print "1 - Sistema"
                print "2 - Aplicação"
                print "3 - Sair"
                opc = input()
                if opc == 1:
                    logs = Logs("System")
                    logs.read_logs()
                    print "Insira ID de um log específico para examina-lo:"
                    print "Ou digite 'exit' para sair"
                    ler = raw_input()
                    if ler == "exit":
                        break
                    else:
                        logs.read_log(int(ler))
                elif opc == 2:
                    logs = Logs("Application")
                    logs.read_logs()
                    print "Insira ID de um log específico para examina-lo:"
                    print "Ou digite 'exit' para sair"
                    ler = raw_input()
                    if ler == "exit":
                        break
                    else:
                        logs.read_log(int(ler))
                elif opc == 3:
                    break
        elif opc == 99:
            break


if __name__ == "__main__":
    init()
