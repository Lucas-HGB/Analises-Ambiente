#!/usr/bin/python2
# -*- coding: utf-8 -*-
from os import system
from wmi import WMI
from Convert import math_size

global manager
manager = WMI()

def mem_use():
    printed = []
    blacklist = [
        "WindowsInternal.ComposableShell.Experiences.TextInput.InputApp.exe",
        "StartMenuExperienceHost.exe",
        "svchost.exe",
        "WirelessKB850NotificationService.exe",
        "System Idle Process",
        "System"
    ]
    print "PID" + "-"*15 + "Process name" + "-"*15 + "Memory Usage"
    for process in manager.Win32_Process():
        if  process.Name not in blacklist and process.Name not in printed:
            if ".exe" in process.Name:
                name = process.Name[0:-4]
                print  str(process.ProcessId) + " "*(18 - len(str(process.ProcessId))) + str(name) + " "*(27 - len(name)) + str(process.WorkingSetSize)
            else:
                print  str(process.ProcessId) + " "*(18 - len(str(process.ProcessId))) + str(process.Name) + " "*(27 - len(process.Name)) + str(process.WorkingSetSize)
            printed.append(process.Name)

def disks():
    for physical_disk in manager.Win32_DiskDrive ():
        for partition in physical_disk.associators ("Win32_DiskDriveToDiskPartition"):
            for logical_disk in partition.associators ("Win32_LogicalDiskToPartition"):
                print "Hardware" + "-"*(len(physical_disk.Caption)-1) + "Partition" + "-"*(len(physical_disk.Caption)-1) + "Total Space" + "-"*(len(physical_disk.Caption)-1) + "Free Space"
                print physical_disk.Caption + " "*10 + logical_disk.Caption + " "*23 + math_size(logical_disk.Size) + " "*21 + math_size(logical_disk.FreeSpace)



def print_menu():
    print "\n"
    print "0 - Uso de memória"
    print "1 - Discos"

def init():
    exit = False
    while not exit:
        print_menu()
        opc = input()
        print "\n"
        if opc == 0:
            mem_use()
        elif opc == 1:
            disks()


if __name__ == "__main__":
    mem_use()