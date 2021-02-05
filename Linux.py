#!/usr/bin/python2
# -*- coding: utf-8 -*-
from os import system

def hostname():
    system("hostname")

def hostnamectl():
    system("hostnamectl")

def sys_vers():
    system("cat /etc/redhat-version")

def rede():
    system("ifconfig")

def cpu():
        system('cat /proc/cpuinfo | grep "model name" | sort -u')
        system('cat /proc/cpuinfo | grep "model name" | wc -l')
        system('cat /proc/cpuinfo | grep "physical id" | sort -u |wc -l')

def memoria_em_gb():
    system("free -mg")


def linux_ver():
    system("cat /proc/version")
    
def swap():
    system("cat /proc/swaps")

def uso_de_swap():
    system("grep Swap /proc/meminfo")

def discos():
    system("df -h")

def io_discos():
    system("iostat")

def status_server():
    system("vmstat 1 5")

def top_processamento():
    system("htop")

def reboot():
    system("last reboot")

def shutdown():
    system("last shutdown")

def print_menu():
    print "0  - Hostname"
    print "1  - Dados da máquina (RedHat >= 7)"
    print "2  - Versao RHEL"
    print "3  - Configurações de Rede"
    print "4  - CPU"
    print "5  - Memória em GB"
    print "6  - SWAP"
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
            hostname()
        elif opc == 1:
            hostnamectl()
        elif opc == 2:
            sys_ver()
        elif opc == 3:
            rede()
        elif opc == 4:
            cpu()
        elif opc == 5:
            memoria_em_gb()
        elif opc == 6:
            swap()
        elif opc == 7:
            uso_de_swap()
        elif opc == 8:
            discos()
        elif opc == 9:
            io_discos()
        elif opc == 10:
            status_server()
        elif opc == 11:
            top_processamento()
        elif opc == 12:
            reboot()
        elif opc == 13:
            shutdown()
        elif opc == 14:
            break
        print "\n\n"
if __name__ == "__main__":
    init()