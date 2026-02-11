using Microsoft.Data.Sqlite;

var connString = "Data Source=E:\\diploma_maga\\1\\crime-scene-service-main\\crime-scene.db";
using var conn = new SqliteConnection(connString);
conn.Open();

var cmd = conn.CreateCommand();
cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name";

using var reader = cmd.ExecuteReader();
Console.WriteLine("Tables in database:");
while (reader.Read())
{
    Console.WriteLine($"  - {reader.GetString(0)}");
}

// Check if examiners table exists and count rows
cmd = conn.CreateCommand();
cmd.CommandText = "SELECT COUNT(*) FROM examiners";
try 
{
    var count = cmd.ExecuteScalar();
    Console.WriteLine($"\nExaminers table has {count} rows");
}
catch (Exception ex)
{
    Console.WriteLine($"\nError checking examiners: {ex.Message}");
}
