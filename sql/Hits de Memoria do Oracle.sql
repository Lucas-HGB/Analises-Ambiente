col "Buffer Hit Ratio (95%-100%)" format a27;
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
  where name = 'cache hit percentadf.ge'
)
select * from a,b,c,d,e; 
