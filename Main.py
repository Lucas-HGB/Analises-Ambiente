#!/usr/bin/python2
# -*- coding: utf-8 -*-

## Lib imports
from os import system

## Custom imports
from Oracle import init as Oracle
from Linux import init as Infra


def clear_screen():
    system("clear")

while True:
    clear_screen()
    print "1 - Infra" 
    print "2 - Banco" 
    print "3 - Sair" 
    try:
        opc = int(input())
        clear_screen()
    except NameError:
        print "Favor inserir um número de 1-3"
    if opc == 1:
        Infra()
    elif opc == 2:
       Oracle()
    elif opc == 3:
        break
