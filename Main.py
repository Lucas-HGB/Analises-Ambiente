#!/usr/bin/python2
# -*- coding: utf-8 -*-
from Linux import init as Infra_Linux
from Windows import init as Infra_Windows
from Oracle import init as Oracle
from platform import system as arch
from os import system

def clear_screen():
    if arch().lower() == "windows":
        system("cls")
    else:
        system("clear")

opc = 0
while opc != 3:
    print "1 - Infra" 
    print "2 - Banco" 
    print "3 - Sair" 
    opc = int(input())
    clear_screen()
    if opc == 1:
        if arch().lower() == "windows":
            Infra_Windows()
        elif arch().lower() == "linux":
            Infra_Linux()
    elif opc == 2:
       Oracle()
