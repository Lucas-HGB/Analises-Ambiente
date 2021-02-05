#!/usr/bin/python2
# -*- coding: UTF-8 -*-
from os import environ
from platform import system

global os
os = system().lower()

def clear_screen():
    if os == "windows":
        system("cls")
    else:
        system("clear")

global scripts
scripts = [
	"""set lines 160
col HOST_NAME for a60
select INSTANCE_NAME,STARTUP_TIME,STATUS,VERSION,HOST_NAME from v$instance
""",
	"select banner from v$version",
	"""Col value$ for a60
Set pages 2000
SELECT name,value$ FROM sys.props$;
""",
	"""Col name for a70
select * from v$controlfile;
""",
	"Select * from v$log;",
	"""set lines 158
set pages 100
column "Tablespace" format A25
column "Usado" format '999,990.00'
column "Livre" format '999,990.00'
column "Expansivel" format A12
column "Total" format '999,990.00'
column "Usado %" format '990.00'
column "Livre %" format '990.00'
column "Tipo Ger." format A12

select t.tablespace_name "Tablespace", round(ar.usado, 2) "Usado", round(decode(NVL2(cresc.tablespace, 0, sign(ar.Expansivel)),1,
                    (ar.livre + ar.expansivel), ar.livre), 2) "Livre", round(ar.alocado,2) "Alocado Mb",
                   NVL2(cresc.limite, 'ILIMITADO', round(ar.expansivel, 2)) "Expansivel",
                   round(decode(NVL2(cresc.tablespace, 0, sign(ar.Expansivel)), 1, ar.usado / (ar.total + ar.expansivel),
                    (ar.usado / ar.total)) * 100, 2) "Usado %", round(decode(NVL2(cresc.tablespace, 0, sign(ar.Expansivel)), 1,
                    (ar.livre + ar.expansivel) / (ar.total + ar.expansivel),
                    (ar.livre / ar.total)) * 100, 2) "Livre %", round(decode(NVL2(cresc.tablespace, 0, sign(ar.Expansivel)), 1,
                    (ar.total + ar.expansivel), ar.total), 2) "Total", t.Contents "Conteudo", t.Extent_Management "Tipo Ger."
       from dba_tablespaces t, (select df.tablespace_name tablespace,
               sum(nvl(df.user_bytes,0))/1024/1024 Alocado, (sum(df.bytes) - sum(NVL(df_fs.bytes, 0))) / 1024 / 1024 Usado,
               sum(NVL(df_fs.bytes, 0)) / 1024 / 1024 Livre,
               sum(decode(df.autoextensible, 'YES', decode(sign(df.maxbytes - df.bytes), 1, df.maxbytes - df.bytes, 0),
                          0)) / 1024 / 1024 Expansivel, sum(df.bytes) / 1024 / 1024 Total
       from dba_data_files df, (select tablespace_name, file_id, sum(bytes) bytes
                  from dba_free_space group by tablespace_name, file_id) df_fs
         where df.tablespace_name = df_fs.tablespace_name(+) and df.file_id = df_fs.file_id(+)
         group by df.tablespace_name
        union
        select tf.tablespace_name tablespace, sum(nvl(tf.user_bytes,0))/1024/1024 Alocado,        
               sum(tf_fs.bytes_used) / 1024 / 1024 Usado, sum(tf_fs.bytes_free) / 1024 / 1024 Livre,
               sum(decode(tf.autoextensible, 'YES', decode(sign(tf.maxbytes - tf.bytes), 1, tf.maxbytes - tf.bytes, 0),
                          0)) / 1024 / 1024 Expansivel, sum(tf.bytes) / 1024 / 1024 Total
          from dba_temp_files tf, V$TEMP_SPACE_HEADER tf_fs
         where tf.tablespace_name = tf_fs.tablespace_name and tf.file_id = tf_fs.file_id
         group by tf.tablespace_name) ar, (select df.tablespace_name tablespace, 'ILIMITADO' limite
          from dba_data_files df
         where df.maxbytes / 1024 / 1024 / 1024 > 30
           and df.autoextensible = 'YES'
         group by df.tablespace_name
        union
        select tf.tablespace_name tablespace, 'ILIMITADO' limite
          from dba_temp_files tf
         where tf.maxbytes / 1024 / 1024 / 1024 > 30
           and tf.autoextensible = 'YES'
         group by tf.tablespace_name) cresc
 where cresc.tablespace(+) = t.tablespace_name
   and ar.tablespace(+) = t.tablespace_name
 order by 7;
""",
	"""col name FORMAT A35
col value format a40
select name,value from v$parameter where isdefault='FALSE';
""",
	"""col "Buffer Hit Ratio (95%-100%)" format a27;
col "Dict Hit Ratio (85%-100%)" format a25;
col "Library Hit Ratio (95%-100%)" format a28;
col "RedoLog Wait (95%-100%)" format a23;
col "PGA Hit Ratio (95%-100%)" format a24;
with
a as
(
  select to_char(round(((1-(sum(decode(name,'physical reads',value,0))/(sum(decode(name,'db block gets',value,0))+(sum(decode(name,'consistent gets',value,0))))))*100),4),'99.99') || '%' as "Buffer Hit Ratio (95%-100%)"
  from v$sysstat
),
b as
(
  select to_char(round(((1-(sum(getmisses)/sum(gets)))*100),4),'999.99') || '%' as "Dict Hit Ratio (85%-100%)"
  from v$rowcache
),
c as
(
  select to_char(round(100-((((sum(reloads)/sum(pins))))),4),'999.99') || '%' as "Library Hit Ratio (95%-100%)"
  from v$librarycache
),
d as
(
  select to_char(round((100-(100*sum(decode(name,'redo log space requests',value,0))/sum(decode(name,'redo entries',value,0)))),4),'999.999') || '%' as "RedoLog Wait (95%-100%)"
  from sys.v_$sysstat
),
e as
(
  select to_char(round(value,4),'999.99') ||'%' "PGA Hit Ratio (95%-100%)"
  from sys.v_$pgastat
  where name = 'cache hit percentage'
)
select * from a,b,c,d,e; 
""",
	"""column "Executions" format 999,999,999,990
column "Cache Misses Executing" format 999,999,990
column "Data Dictionary Gets" format 999,999,999,999,999
column "Get Misses" format 999,999,999,999
select sum(pins) "Executions",
sum(reloads) "Cache Misses Executing",
(sum(reloads)/sum(pins)*100) "%Ratio (STAY UNDER 1%)"
from v$librarycache;

column NAMESPACE for a30
select
        NAMESPACE,
        GETS,
        GETHITS,
        round(GETHITRATIO*100,2) gethit_ratio,
        PINS,
        PINHITS,
        round(PINHITRATIO*100,2) pinhit_ratio,
        RELOADS,
        INVALIDATIONS
from    v$librarycache;
""",
	"""select sum(gets) "Data Dictionary Gets",
sum(getmisses) "Get Misses",
100*(sum(getmisses)/sum(gets)) "%Ratio (STAY UNDER 12%)"
from v$rowcache;
""",
	"""column parameter format a21
column pct_succ_gets format 999.9
column updates format 999,999,999
SELECT
        parameter
        , sum(gets)
        , sum(getmisses)
        , 100*sum(gets - getmisses) / sum(gets) pct_succ_gets
        , sum(modifications) updates
FROM V$ROWCACHE
        WHERE gets > 0
GROUP BY rollup(parameter)
/
""",
	"""column parameter format a21
column pct_succ_gets format 999.9
column updates format 999,999,999
SELECT
        parameter
        , sum(gets)
        , sum(getmisses)
        , sum(modifications) updates
        , to_char(100*sum(gets - getmisses) / sum(gets),'999D99') pct_succ_gets
FROM V$ROWCACHE
        WHERE gets > 0
GROUP BY rollup(parameter)
        HAVING 100*sum(gets - getmisses) / sum(gets) <= 60;
""",
	"""set lines 155
set pagesize 400

SELECT (1 - (Sum(misses) / Sum(gets))) * 100 "LATCH HIT (95%-100%)" FROM v$latch;

column "col1"  format a25               heading "BLOCOS BUFFER CACHE"
column "col2"  format 99,999,999,990    heading "Quantidade"
column "col3"  format 99,999,999,990    heading "TOTAL"

select
        decode(state,   0,'Nao Usado',
                        1,'Lido e Modificado',
                        2,'Lido e nao Modificado',
                        3,'Lido Correntemente',
                        'Outros')                       "col1",
        count(*)                                        "col2"
from
        x$bh
group by
        decode(state, 0,'Nao Usado',
                        1,'Lido e Modificado',
                        2,'Lido e nao Modificado',
                        3,'Lido Correntemente',
                        'Outros');
""",
"""declare
        object_mem number;
        shared_sql number;
        cursor_mem number;
        mts_mem number;
        used_pool_size number;
        free_mem number;
        pool_size varchar2(512);
begin

select sum(sharable_mem) into object_mem from v$db_object_cache;
select sum(250*users_opening) into cursor_mem from v$sqlarea;

select sum(value) into mts_mem from v$sesstat s, v$statname n
       where s.statistic#=n.statistic#
       and n.name='session uga memory max';

select bytes into free_mem from v$sgastat
        where name = 'free memory' and pool='shared pool';
used_pool_size := round(1.3*(object_mem+cursor_mem));
select value into pool_size from v$parameter where name='shared_pool_size';
  if pool_size > 0 then
        dbms_output.put_line ('Gerenciamento de memória Manual.');
        dbms_output.put_line ('Object mem:                            '||to_char (object_mem) || ' bytes');
        dbms_output.put_line ('Cursors:                            '||to_char (cursor_mem) || ' bytes');
        dbms_output.put_line ('Free memory:                            '||to_char (free_mem) || ' bytes ' ||
        '('|| to_char(round(free_mem/1024/1024,2)) || 'MB)');
        dbms_output.put_line ('Shared pool utilization (total):                            '||
        to_char(used_pool_size) || ' bytes ' || '(' ||
        to_char(round(used_pool_size/1024/1024,2)) || 'MB)');
        dbms_output.put_line ('Shared pool allocation (actual):                            '|| pool_size
        ||' bytes ' || '(' || to_char(round(pool_size/1024/1024,2)) || 'MB)');
        dbms_output.put_line ('Percentage Utilized:                            '||to_char
        (round(used_pool_size/pool_size*100)) || '%');
   else
        dbms_output.put_line ('Gerenciamento de memória Automático.');
        dbms_output.put_line ('Object mem:                             '||to_char (object_mem) || ' bytes');
        dbms_output.put_line ('Cursors:                                '||to_char (cursor_mem) || ' bytes');
        dbms_output.put_line ('Free memory:                            '||to_char (free_mem) || ' bytes ' ||
        '('|| to_char(round(free_mem/1024/1024,2)) || 'MB)');
   end if;
end;
/
set serveroutput on
/
"""
]




def get_oracle_configs():
	configs = {}
	if arch().lower() == "linux":
		try:
			with open(r"/etc/oratab", "r") as oratab:
				if args.debug:
					print "Reading /etc/oratab"
				lines = oratab.readlines()
				config_lines = [line for line in lines if line[0] != "#" and line != "\n"]
				for config in config_lines:
					configs[config.split(":")[0]] = (config.split(":")[1])
			return configs
		except Exception as excp:
			print excp

def get_oracle_configs():
	#Extrair SID em Linux usando etc/oratab
        configs = {}
        if os == "linux":
                with open(r"etc\oratab", "r") as oratab:
                        lines = oratab.readlines()
                        config_line = [line for line in lines if line[0] != "#" and line != "\n"]
                        for config in config_line:
                                configs[config.split(":")[0]] = (config.split(":")[1])
        elif os == "windows":
                print "Windows"
	return configs

class init_Banco():

	def __init__(self, user, password, instance, port = 1521, ip = "localhost"):
                self.configs = get_oracle_configs()
                self.set_environ(instance)
                self.connect(user, password, instance, ip, port)

        def conect(self, user, password, ip, port, instance):
                try:
                        self.connection = connect(user,password, "%s:%s/%s" %s (ip, port, instance), encoding="UTF-8")
                        self.cursor = self.connection.cursor()
                except Exception as excp:
                        print excp
        def set_environ(self, instance):
                environ["LD_LIBRARY_PATH"] = "%s/lib" % (self.configs[instance])

	def run_command(self, command):
		self.cursor.execute(command)
		output = self.cursor.fetchall()
		return output

def choose_instance():
        configs = get_oracle_configs()
        exit = False
        while not exit:
                for instance, count in zip(configs.keys(), len(configs.keys())):
                        print "%s - %s" % (count, instance)
                print "%s - Sair" % (count + 1)
                opc = input()
                if opc != count + 1:
                        Banco = init_Banco(ip = "localhost", user = raw_input("Database user:  "), password = raw_input("Password:  "), instance = configs.keys()[opc])
                        extract_info = 0
                        while extract_info != 14:
                                print_menu()
                                extract_info = input()
                                Banco.run_command(scripts[extract_info])
                elif opc == count + 1:
                        exit = True
                        break

def print_menu():
	print "0  - Dados da instância"
	print "1  - Versão dos produtos instalados"
	print "2  - Configurações de NLS da Instância"
	print "3  - Informações sobre os controlfiles"
	print "4  - Informações sobre Redologs"
	print "5  - Tablespaces"
	print "6  - Parâmetros da Instância"
	print "7  - Hits de Memória do Oracle"
	print "8  - Advisors"
	print "9  - Data Dictionary gets"
	print "10 - Table Scan"
	print "11 - Database Misses"
	print "12 - Blocos Buffer Cache"
	print "13 - Memória Oracle"
	print "14 - Sair\n"
	

def init():
	opc = 0
	choose_instance()

if __name__ == "__main__":
        init()
