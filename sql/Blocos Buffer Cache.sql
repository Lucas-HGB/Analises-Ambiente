set lines 155
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
