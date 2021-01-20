#!/usr/bin/python2
# -*- coding: utf-8 -*-
from os import system
from wmi import WMI
from Converters import math_size

global manager
manager = WMI()

def mem_use():
    #system("tasklist | sort /R /+58")
    print "PID-----Process name-----Memory Usage"
    for process in manager.Win32_Process():
        print  str(process.ProcessId) + " " + str(process.Name) + " " + str(process.HandleCount)

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
    init()