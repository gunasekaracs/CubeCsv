﻿using Microsoft.Data.Sqlite;
using System.IO;

namespace CubeCsv.Tests
{
    internal class CsvDbContext
    {
        string databaseFile = "CubeCsv.db";

        public void Seed()
        {
            if (File.Exists(databaseFile)) File.Delete(databaseFile);
            SqliteConnection connection = new($"Data Source={ databaseFile }");
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
            connection.Open();

            string sql =
            $@"
                CREATE TABLE ""{ CsvSqlConstants.EmployeesTableName }""
                (
                    { CsvSqlConstants.Id } INT IDENTITY(1,1) PRIMARY KEY,
                    { CsvSqlConstants.FirstName } NVARCHAR(255),
                    { CsvSqlConstants.LastName } NVARCHAR(255),
                    { CsvSqlConstants.Age } INT,
                    { CsvSqlConstants.DateOfBirth } DATETIME
                );
            ";

            SqliteCommand command = new(sql, connection);
            command.ExecuteNonQuery();

            sql = $@"INSERT INTO ""{ CsvSqlConstants.EmployeesTableName }"" 
                    ({ CsvSqlConstants.FirstName },{ CsvSqlConstants.LastName },{ CsvSqlConstants.Age },{ CsvSqlConstants.DateOfBirth }) 
                    VALUES 
                        ('Charith', 'Gunasekara', 42, '1980-07-20'),
                        ('Dilshan', 'Amarasinghe', 34, '1988-01-10'),
                        ('Kumanan', 'Panchalingam', 30, '1992-05-02'),
                        ('Azmina', 'Mohomadeen', 30, '1992-05-20'),
                        ('Eswar', 'Raj', 30, '1992-08-05')";

            command = new SqliteCommand(sql, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }
        public SqliteConnection CreateConnection()
        {
            return new SqliteConnection($"Data Source={ databaseFile }");
        }
    }
}
