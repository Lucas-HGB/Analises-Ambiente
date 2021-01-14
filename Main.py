#!/usr/bin/python2
# -*- coding: utf-8 -*-
from Linux import init as Infra_Linux
from Windows import init as Infra_Windows
from Oracle import init as Banco
from platform import system as arch
from os import system

opc = 0
while opc != 3:
    print "1 - Infra" 
    print "2 - Banco" 
    print "3 - Sair" 
    opc = int(input())
    system("clear")
    if opc == 1:
        if arch().lower() == "windows":
            Infra_Windows()
        elif arch().lower() == "linux":
            Infra_Linux()
    elif opc == 2:
       Banco()
