#!/usr/bin/python2
# -*- coding: utf-8 -*-
from platform import system as arch
if arch().lower() == "windows":
    from Windows import init as infra
elif arch().lower() == "linux":
    from Linux import init as infra
from Oracle import init as Oracle

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
            infra()
        elif arch().lower() == "linux":
            infra()
    elif opc == 2:
       Oracle()
