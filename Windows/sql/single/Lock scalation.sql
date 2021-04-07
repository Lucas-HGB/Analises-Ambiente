select lock_escalation_desc,count(1) as QTE 
from sys.tables group by lock_escalation_desc
