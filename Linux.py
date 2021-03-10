#!/usr/bin/python2
# -*- coding: utf-8 -*-
from os import system

def hostname():
    system("hostname")

def hostnamectl():
    system("hostnamectl")

def rede():
    system("ifconfig")

def cpu():
    system("bash ./Bash/CPU.sh")

def memoria():
    system("free -m")

def sys_ver():
    system("cat /etc/*-release")

def linux_ver():
    system("cat /proc/version")
    
def swap():
    system("cat /proc/swaps")

def uso_de_swap():
    system("cat /proc/meminfo | grep Swap")

def discos():
    system("df -h")

def io_discos():
    system("iostat")

def status_server():
    system("vmstat 1 5")

def top():
    system("top")

def reboot():
    system("last reboot | head -n 1")

def shutdown():
    system("last shutdown")

def print_menu():
    print "0  - Hostname"
    print "1  - Dados da máquina (RedHat >= 7)"
    print "2  - Versao OS"
    print "3  - Configurações de Rede"
    print "4  - CPU"
    print "5  - Memória"
    print "6  - Partição SWAP"
    print "7  - Uso de Swap"
    print "8  - Discos"
    print "9  - IO Discos"
    print "10 - Coleta de Status do Server"
    print "11 - Top processamento"
    print "12 - Last shutdown"
    print "13 - Last reboot"
    print "14 - Sair"

def init():
    opc = 0
    while opc != 14:
        print_menu()
        opc = int(input())
        print "\n\n" 
        if opc == 0:
            system("clear")
            hostname()
        elif opc == 1:
            system("clear")
            hostnamectl()
        elif opc == 2:
            system("clear")
            sys_ver()
        elif opc == 3:
            system("clear")
            rede()
        elif opc == 4:
            system("clear")
            cpu()
        elif opc == 5:
            system("clear")
            memoria()
        elif opc == 6:
            system("clear")
            swap()
        elif opc == 7:
            system("clear")
            uso_de_swap()
        elif opc == 8:
            system("clear")
            discos()
        elif opc == 9:
            system("clear")
            io_discos()
        elif opc == 10:
            system("clear")
            status_server()
        elif opc == 11:
            system("clear")
            top()
        elif opc == 12:
            system("clear")
            shutdown()
        elif opc == 13:
            system("clear")
            reboot()
        elif opc == 14:
            break
        print "\n\n"

if __name__ == "__main__":
    init()