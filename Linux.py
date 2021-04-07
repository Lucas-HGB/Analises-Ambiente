#!/usr/bin/python2
# -*- coding: utf-8 -*-
from os import system

def __hostname():
    system("hostname")

def __hostnamectl():
    system("hostnamectl")

def __rede():
    system("ifconfig")

def __cpu():
    system('cat /proc/cpuinfo | grep "model name" | sort -u')
    system('physicalCores=$(cat /proc/cpuinfo | grep "physical id" | sort -u | wc -l) && echo "Cores físicos: $physicalCores"')
    system('logicalCores=$(cat /proc/cpuinfo | grep "model name" | wc -l) && echo "Cores lógicos: $logicalCores"')

def __memoria():
    system("free -m")

def __sys_ver():
    system("cat /etc/*-release")

def __linux_ver():
    system("cat /proc/version")
    
def __swap():
    system("cat /proc/swaps")

def __uso_de_swap():
    system("cat /proc/meminfo | grep Swap")

def __discos():
    system("df -h")

def __io_discos():
    system("iostat")

def __status_server():
    system("vmstat 1 5")

def __top():
    system("top")

def __reboot():
    system("last reboot | head -n 1")

def __shutdown():
    system("last shutdown")

def __print_menu():
    print "1  - Hostname"
    print "2  - Dados da máquina (RedHat >= 7)"
    print "3  - Versao OS"
    print "4  - Configurações de Rede"
    print "5  - CPU"
    print "6  - Memória"
    print "7  - Partição SWAP"
    print "8  - Uso de Swap"
    print "9  - Discos"
    print "10  - IO Discos"
    print "11 - Coleta de Status do Server"
    print "12 - Top processamento"
    print "13 - Last shutdown"
    print "14 - Last reboot"
    print "15 - Sair"

def init():
    opc = 0
    while opc != 15:
        __print_menu()
        opc = int(input()) - 1
        system("clear")
        print "\n\n" 
        if opc == 0:
            __hostname()
        elif opc == 1:
            __hostnamectl()
        elif opc == 2:
            __sys_ver()
        elif opc == 3:
            __rede()
        elif opc == 4:
            __cpu()
        elif opc == 5:
            __memoria()
        elif opc == 6:
            __swap()
        elif opc == 7:
            __uso_de_swap()
        elif opc == 8:
            __discos()
        elif opc == 9:
            __io_discos()
        elif opc == 10:
            __status_server()
        elif opc == 11:
            __top()
        elif opc == 12:
            __shutdown()
        elif opc == 13:
            __reboot()
        elif opc == 14:
            break
        print "\n\n"

if __name__ == "__main__":
    init()