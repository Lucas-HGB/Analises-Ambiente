#!/usr/bin/python2
# -*- coding: UTF-8 -*-
from os import system, listdir

user = None
password = None

def get_instances():
    #Extrair SID em Linux usando etc/oratab
    instances = []
    try:
        with open(r"/etc/oratab", "r") as oratab:
            [instances.append(line.split(":")[0]) for line in oratab.readlines() if line[0] != "#" and line != "\n"]
    except IOError:
            print("Oratab Not Found!")
    return instances

def list_instances():
    system("clear")
    instances = get_instances()
    if len(instances) > 1:
      print "Selecione uma instância"
      for instance,count in zip(instances, range(len(instances))):
          print "%s - %s" % (str(count), instance)
      opc = input()
      system("clear")
      return instance[opc]
    else: return instances[0]

def run_command(scriptFile):
    if not user: 
      output = system("cat './sql/%s' | sqlplus -s / as sysdba"%(scriptFile))
    else:
      output = system("cat './sql/%s' | sqlplus -s %s/%s"%(scriptFile, user, password))
    return output

def print_menu():
    print "0  - Inserir usuario e senha do banco (caso SYSDBA nao funcione)"
    for script, count in zip(listdir("sql"), range(len(listdir("sql")))):
      if count + 1 < 10:
        print "%s  - %s" % (str(count + 1), script[0:-4])
      else:
        print "%s - %s" % (str(count + 1), script[0:-4])
    print "%s - Sair" % str(count + 2)
    opc = input()
    try:
      if opc != 0:
        script = listdir("sql")[opc-1]
        return script
      else:
        return "insert"
    except IndexError:
      return None
	

def init():
    global user, password
    instance = list_instances()
    while True:
        scriptFile = print_menu()
        system("clear")
        if scriptFile != None and scriptFile != "insert":
            run_command(scriptFile)
        elif scriptFile == None:
            break
        elif scriptFile == "insert":
          user, password = raw_input("User: "), raw_input("Password: ")

if __name__ == "__main__":
    init()
