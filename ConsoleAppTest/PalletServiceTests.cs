using MONOPOLYTestTaskConsole.Database;
using MONOPOLYTestTaskConsole.Models;
using MONOPOLYTestTaskConsole.Service;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppTest
{
    public class PalletServiceTests
    {
        private readonly string _connectionString = "Data Source=temp_test_db.db;";
        private readonly SQLiteConnection _connection;

        public PalletServiceTests()
        {
            _connection = new SQLiteConnection(_connectionString);
            _connection.Open();

            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Pallets (
                    ID INTEGER PRIMARY KEY,
                    Length INTEGER,
                    Width INTEGER,
                    Height INTEGER
                );
                CREATE TABLE IF NOT EXISTS Boxes (
                    ID INTEGER PRIMARY KEY,
                    PalletID INTEGER,
                    Weight INTEGER,
                    ExpirationDate TEXT,
                    FOREIGN KEY(PalletID) REFERENCES Pallets(ID)
                );";
                command.ExecuteNonQuery();
            }
        }

        [Fact]
        public void PalletService_AddPalletAndBoxToDatabase()
        {
            using (var transaction = _connection.BeginTransaction())
            {
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText = @"
                    INSERT INTO Pallets (ID, Length, Width, Height) VALUES (5, 120, 120, 160);
                    INSERT INTO Boxes (ID, PalletID, Weight, ExpirationDate) VALUES (5, 5, 20, '2023-12-31');
                    ";
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
            }

            using (var command = new SQLiteCommand("SELECT COUNT(*) FROM Pallets WHERE ID = 5;", _connection))
            {
                var result = Convert.ToInt32(command.ExecuteScalar());
                Assert.Equal(1, result);
            }

            using (var command = new SQLiteCommand("SELECT COUNT(*) FROM Boxes WHERE ID = 5;", _connection))
            {
                var result = Convert.ToInt32(command.ExecuteScalar());
                Assert.Equal(1, result);
            }
        }

        [Fact]
        public void PalletService_GetPalletsByMaxExpiration()
        {
            AddTestData();

            var palletIds = new List<int>();
            using (var command = new SQLiteCommand(@"
            SELECT Pallets.ID
            FROM Pallets
            JOIN Boxes ON Pallets.ID = Boxes.PalletID
            GROUP BY Pallets.ID
            ORDER BY MAX(Boxes.ExpirationDate) DESC;", _connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    palletIds.Add(reader.GetInt32(0));
                }
            }

            Assert.Equal(new[] { 3, 1, 2, 4 }, palletIds);
        }

        private void ClearDatabase()
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM Boxes; DELETE FROM Pallets;";
                command.ExecuteNonQuery();
            }
        }

        private void AddTestData()
        {
            ClearDatabase(); 

            using (var transaction = _connection.BeginTransaction())
            {
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText = @"
                    INSERT INTO Pallets (Length, Width, Height) VALUES (100, 100, 150);
                    INSERT INTO Boxes (PalletID, Weight, ExpirationDate) VALUES (1, 10, '2023-05-01');
                
                    INSERT INTO Pallets (Length, Width, Height) VALUES (100, 100, 150);
                    INSERT INTO Boxes (PalletID, Weight, ExpirationDate) VALUES (2, 8, '2023-04-01');
                
                    INSERT INTO Pallets (Length, Width, Height) VALUES (120, 120, 160);
                    INSERT INTO Boxes (PalletID, Weight, ExpirationDate) VALUES (3, 15, '2023-05-01');
                
                    INSERT INTO Pallets (Length, Width, Height) VALUES (120, 120, 160);
                    INSERT INTO Boxes (PalletID, Weight, ExpirationDate) VALUES (4, 15, '2023-03-01');
            ";
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
            }
        }

        private void ClearTestData()
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM Boxes; DELETE FROM Pallets;";
                command.ExecuteNonQuery();
            }
        }
    }
}