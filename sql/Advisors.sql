column "Executions" format 999,999,999,990
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
