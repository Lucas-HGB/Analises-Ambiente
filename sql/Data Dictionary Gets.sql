select sum(gets) "Data Dictionary Gets",
sum(getmisses) "Get Misses",
100*(sum(getmisses)/sum(gets)) "%Ratio (STAY UNDER 12%)"
from v$rowcache;
