#!/usr/bin/python2
# -*- coding: utf-8 -*-
from os import system

def mem_use():
    system("tasklist | sort /R /+58")

def disks():
    system('wmic /node:"%COMPUTERNAME%" LogicalDisk Where DriveType="3" Get DeviceID,FreeSpace, Size')



def print_menu():
    print "\n"
    print "0 - Uso de memória"
    print "1 - Discos"
    print "\n"

def init():
    exit = False
    while not exit:
        print_menu()
        opc = input()
        if opc == 0:
            mem_use()
        elif opc == 1:
            disks()


if __name__ == "__main__":
    init()