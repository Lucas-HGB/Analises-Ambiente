using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace ClassLibrary
{
    public class MySQL
    {
        List<string> commands = new List<string>()
        {
            // CHECK BACKUP
            @"select database_name, max(backup_finish_date) backup_finish_date
from msdb.dbo.backupset group by database_name order by 2
",
            // ISOLATION LEVEL
            @"SELECT name,is_read_committed_snapshot_on FROM
sys.databases
",
            // INDICES
            @"SELECT CONVERT (varchar, getdate(), 126) AS runtime, 
 mig.index_group_handle, mid.index_handle, 
 CONVERT (decimal (28,1), migs.avg_total_user_cost * migs.avg_user_impact * (migs.user_seeks + migs.user_scans)) AS improvement_measure, 
 'CREATE INDEX workdb_index_' + CONVERT (varchar, mig.index_group_handle) + '_' + CONVERT (varchar, mid.index_handle) 
 + ' ON ' + mid.statement 
 + ' (' + ISNULL (mid.equality_columns,'') 
 + CASE WHEN mid.equality_columns IS NOT NULL AND mid.inequality_columns IS NOT NULL THEN ',' ELSE '' END + ISNULL (mid.inequality_columns, '')
 + ')' 
 + ISNULL (' INCLUDE (' + mid.included_columns + ')', '') AS create_index_statement, 
 migs.*, mid.database_id, mid.[object_id]
FROM sys.dm_db_missing_index_groups mig
INNER JOIN sys.dm_db_missing_index_group_stats migs ON migs.group_handle = mig.index_group_handle
INNER JOIN sys.dm_db_missing_index_details mid ON mig.index_handle = mid.index_handle
WHERE CONVERT (decimal (28,1), migs.avg_total_user_cost * migs.avg_user_impact * (migs.user_seeks + migs.user_scans)) > 10
ORDER BY migs.avg_total_user_cost * migs.avg_user_impact * (migs.user_seeks + migs.user_scans) DESC
PRINT ''
GO
",
            // VERSÃO
            @"select @@version",
            // LOCK ESCALATION
            @"select lock_escalation_desc,count(1) as QTE 
from sys.tables group by lock_escalation_desc
",
            // 10 MAIORES TABELAS
            @"SELECT top 10
 t.NAME AS Entidade,
 p.rows AS Registros,
 SUM(a.total_pages) * 8 AS EspacoTotalKB,
 SUM(a.used_pages) * 8 AS EspacoUsadoKB,
 (SUM(a.total_pages) - SUM(a.used_pages)) * 8 AS EspacoNaoUsadoKB
FROM
 sys.tables t
INNER JOIN
 sys.indexes i ON t.OBJECT_ID = i.object_id
INNER JOIN
 sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id
INNER JOIN
 sys.allocation_units a ON p.partition_id = a.container_id
LEFT OUTER JOIN
 sys.schemas s ON t.schema_id = s.schema_id
WHERE
 t.NAME NOT LIKE 'dt%'
 AND t.is_ms_shipped = 0
 AND i.OBJECT_ID > 255
GROUP BY
 t.Name, s.Name, p.Rows
ORDER BY
 EspacoTotalKB DESC
",
            // FRAGMENTAÇÂO
            @"CREATE TABLE #fraglist (
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
"
        };
        private MySqlConnection connection = new MySqlConnection();
        private List<String> databases = new List<String>();
        public MySQL()
        {
            bool login_error = false;
            do
            {
                string database = "mysql";
                string user = "root";
                string password = "l40215007";
                connection = new MySqlConnection($"server=localhost;database={database};uid={user};pwd={password};");
                try
                {
                    connection.Open();
                    login_error = false;
                }
                catch (MySqlException) { Console.WriteLine("Wrong user/password!"); login_error = true; }
            } while (login_error);

        }

        private static int Print_Menu()
        {
            int main_menu = 0;
            Console.WriteLine("1 - Mudar Database");
            try
            {
                main_menu = Convert.ToInt32(Console.ReadLine());
            }
            catch (FormatException) { Console.Clear(); Console.WriteLine("Please insert numeric value!"); }
            return main_menu;
        }
        private String Run_Command(String command)
        {
            var cmd = new MySqlCommand(command, connection);
            String output = cmd.ExecuteScalar().ToString();
            return output;
        }
        public void Main()
        {
            do
            {
                int main_menu = Print_Menu();
                switch (main_menu)
                {
                    case 1:
                        // Mudar database
                        String output = Run_Command(commands[0]);
                        Console.WriteLine(output);
                        break;
                    case 2:
                        // Default scripts
                        break;
                    case 3:
                        // Custom script
                        // Read from file or type?
                        break;
                }
            } while (true);
        }

    }
}
