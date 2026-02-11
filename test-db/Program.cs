using System;
using Microsoft.Data.Sqlite;

var connString = @"Data Source=E:\diploma_maga\1\crime-scene-service-main\crime-scene.db";
Console.WriteLine($"Connecting to: {connString}");

using var conn = new SqliteConnection(connString);
await conn.OpenAsync();
Console.WriteLine("Connected successfully!");

var cmd = conn.CreateCommand();
cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name";

using var reader = await cmd.ExecuteReaderAsync();
Console.WriteLine("\nTables in database:");
var hasRows = false;
while (await reader.ReadAsync())
{
    hasRows = true;
    Console.WriteLine($"  - {reader.GetString(0)}");
}

if (!hasRows)
{
    Console.WriteLine("  (no tables found)");
}

// Check specifically for examiners table
cmd = conn.CreateCommand();
cmd.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='examiners'";
var examinerTableExists = Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
Console.WriteLine($"\nExaminers table exists: {examinerTableExists}");
