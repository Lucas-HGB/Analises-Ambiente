CREATE TABLE #fraglist (
ObjectName CHAR (255),
ObjectId INT,
IndexName CHAR (255),
IndexId INT,
Lvl INT,
CountPages INT,
CountRows INT,
MinRecSize INT,
MaxRecSize INT,
AvgRecSize INT,
ForRecCount INT,
Extents INT,
ExtentSwitches INT,
AvgFreeBytes INT,
AvgPageDensity INT,
ScanDensity DECIMAL,
BestCount INT,
ActualCount INT,
LogicalFrag DECIMAL,
ExtentFrag DECIMAL)

INSERT INTO #fraglist 
(ObjectName,
ObjectId,
IndexName,
IndexId,
Lvl,
CountPages,
CountRows,
MinRecSize,
MaxRecSize,
AvgRecSize,
ForRecCount,
Extents,
ExtentSwitches,
AvgFreeBytes,
AvgPageDensity,
ScanDensity,
BestCount,
ActualCount,
LogicalFrag,
ExtentFrag)

EXEC ('DBCC SHOWCONTIG WITH TABLERESULTS')
Select * from #fraglist order by countpages desc
Drop table #fraglist
