select database_name, max(backup_finish_date) backup_finish_date
from msdb.dbo.backupset group by database_name order by 2
