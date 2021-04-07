set pages 2000;
set lines 300;
col owner for a40
col tablespace_name for a40
SELECT owner,tablespace_name, round(Sum(bytes)/1024/1024,0)  AS total_size_mb FROM dba_segments WHERE owner like upper('%') GROUP BY owner,tablespace_name order by owner,tablespace_name,total_size_mb desc
/
 
