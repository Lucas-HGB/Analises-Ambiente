#!/usr/bin/python2
# -*- coding: utf-8 -*-
from os import system

def cpu():
    system('cat /proc/cpuinfo | grep "model name"')

def mem():
    system("free -m")

def disk():
    system("df -h")

def process():
    system("top")

def reboot():
    system("last reboot")

def shutdown():
    system("last shutdown")

def sys_vers():
    system("cat /etc/redhat-version")

def print_menu():
    print "0  - Informações da CPU"
    print "1  - Memória"
    print "2  - Discos"
    print "3  - Processos"
    print "4  - Última reinicialização"
    print "5  - Último desligamento"
    print "6  - Versão RHEL"
    print "7  - Sair"

def init():
    opc = 0
    while opc != 6:
        print_menu()
        opc = int(input())
        print "\n\n" 
        if opc == 0:
            cpu()
        elif opc == 1:
            mem()
        elif opc == 2:
            disk()
        elif opc == 3:
            process()
        elif opc == 4:
            reboot()
        elif opc == 5:
            shutdown()
        elif opc == 6:
            sys_vers()
        elif opc == 7:
            break
        print "\n\n"
