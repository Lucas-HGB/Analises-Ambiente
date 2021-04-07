column parameter format a21
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
