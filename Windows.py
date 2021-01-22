#!/usr/bin/python2
# -*- coding: utf-8 -*-
from os import system
from platform import system as arch
from wmi import WMI
from Convert import math_size

def clear_screen():
    if arch().lower() == "windows":
        system("cls")
    else:
        system("clear")

class System_Manager():

    def __init__(self):
        self.manager = WMI()
        self.IDs = []
        self.Names = []
        self.WorkingSets = []
        self.PercentProcessorTimes = []


    def print_descending_mem(self):
        for num in range(len(self.IDs)):
            index = self.WorkingSets.index(max(self.WorkingSets))
            print str(self.IDs[index]) + " "*(18 - len(str(self.IDs[index]))) + str(self.Names[index]) + " "*(27 - len(self.Names[index])) + str(self.WorkingSets[index]) + " "*(27 - len(str(self.WorkingSets[index]))) + str(self.PercentProcessorTimes[index])
            self.WorkingSets.pop(index)

    def mem_use(self):
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
    

    def disks(self):
        for physical_disk in self.manager.Win32_DiskDrive ():
            for partition in physical_disk.associators ("Win32_DiskDriveToDiskPartition"):
                for logical_disk in partition.associators ("Win32_LogicalDiskToPartition"):
                    print "Hardware" + "-"*(len(physical_disk.Caption)-1) + "Partition" + "-"*(len(logical_disk.Caption)+10) + "Total Space" + "-"*(len(logical_disk.size)-1) + "Free Space" + "-"*(len(logical_disk.FreeSpace)-1) + "Used Space"
                    print physical_disk.Caption + " "*11 + logical_disk.Caption + " "*16 + math_size(logical_disk.Size) + " "*13 + math_size(logical_disk.FreeSpace) + " "*13 + math_size(logical_disk.UsedSpace)



def print_menu():
    print "\n"
    print "0 - Uso de memória"
    print "1 - Discos"

def init():
    manager = System_Manager()
    exit = False
    while not exit:
        print_menu()
        opc = input()
        clear_screen()
        if opc == 0:
            manager.mem_use()
        elif opc == 1:
            manager.disks()


if __name__ == "__main__":
    manager = System_Manager()
    manager.disks()