col name FORMAT A35
col value format a40
select name,value from v$parameter where isdefault='FALSE';
