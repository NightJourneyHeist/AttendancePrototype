using System.IO;
using Microsoft.Data.Sqlite;
using System.Threading.Tasks;

public static class DatabaseManager
{
    public static void InitializeDatabase(string dbFileName, string columnName)
    {
        using (var connection = new SqliteConnection($"Data Source={dbFileName}"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            // Wrapping {columnName} in square brackets allows spaces and symbols safely!
            command.CommandText = $@"
                CREATE TABLE IF NOT EXISTS AttendanceList (
                    NUMBER INTEGER PRIMARY KEY AUTOINCREMENT,
                    NAME TEXT NOT NULL,
                    [{columnName}] BOOLEAN NOT NULL DEFAULT 0
                );
            ";
            command.ExecuteNonQuery();
        }
    }

    public static void SeedNamesIfNeeded(string dbFileName)
    {
        using (var connection = new SqliteConnection($"Data Source={dbFileName}"))
        {
            connection.Open();

            var checkCommand = connection.CreateCommand();
            checkCommand.CommandText = "SELECT COUNT(*) FROM AttendanceList;";
            long count = (long)(checkCommand.ExecuteScalar() ?? 0);

            if (count > 0) return;
        }

        string filePath = Path.Combine(AppContext.BaseDirectory, "Attendance.txt");
        if (File.Exists(filePath))
        {
            string[] names = File.ReadAllLines(filePath);

            using (var connection = new SqliteConnection($"Data Source={dbFileName}"))
            {
                connection.Open();

                foreach (string name in names)
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                        INSERT INTO AttendanceList (Name) 
                        VALUES ($name);
                    ";
                    command.Parameters.AddWithValue("$name", name);
                    command.ExecuteNonQuery();
                }
            }
        }
    }

    public static async Task<bool> UpdateAttendanceAsync(string dbFileName, string columnName, int targetNumber)
    {
        using (var connection = new SqliteConnection($"Data Source={dbFileName}"))
        {
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            
            command.CommandText = $@"
                UPDATE AttendanceList 
                SET [{columnName}] = 1 
                WHERE NUMBER = $number;
            ";
            command.Parameters.AddWithValue("$number", targetNumber);
            
            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }

    public static async Task<int?> GetAttendanceStatusAsync(string dbFileName, string columnName, int targetNumber)
    {
        using (var connection = new SqliteConnection($"Data Source={dbFileName}"))
        {
            await connection.OpenAsync();
            var command = connection.CreateCommand();
        
            command.CommandText = $@"
                SELECT [{columnName}] 
                FROM AttendanceList 
                WHERE NUMBER = $number;
            ";
            command.Parameters.AddWithValue("$number", targetNumber);
        
            var result = await command.ExecuteScalarAsync();
        
            if (result != null && result != DBNull.Value)
            {
                return Convert.ToInt32(result); // Returns 0 or 1
            }
        
            return null; // Student number not found
        }
    }
}