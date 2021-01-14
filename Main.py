#!/usr/bin/python2
# -*- coding: utf-8 -*-
from Infra import init as Infra
from Oracle import init as Banco
from os import system

opc = 0
while opc != 3:
    print "1 - Infra" 
    print "2 - Banco" 
    print "3 - Sair" 
    opc = int(input())
    system("clear")
    if opc == 1:
        Infra()
    elif opc == 2:
       Banco()
