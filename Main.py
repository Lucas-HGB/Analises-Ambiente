#!/usr/bin/python2
# -*- coding: utf-8 -*-
from platform import system as arch
global os
global os = arch().lower()
if os == "windows":
    from Windows import init as infra
elif os == "linux":
    from Linux import init as infra
from Oracle import init as Oracle

from os import system

def clear_screen():
    if os == "windows":
        system("cls")
    elif os == "linux":
        system("clear")
    else:
        print "Operating system not recognized"

opc = 0
while opc != 3:
    print "1 - Infra" 
    print "2 - Banco" 
    print "3 - Sair" 
    opc = int(input())
    clear_screen()
    if opc == 1:
        if os == "windows":
            infra()
        elif os == "linux":
            infra()
    elif opc == 2:
       Oracle()
