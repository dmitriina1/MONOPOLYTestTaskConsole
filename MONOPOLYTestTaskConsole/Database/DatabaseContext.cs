using MONOPOLYTestTaskConsole.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MONOPOLYTestTaskConsole.Database
{
    public class DatabaseContext : IDisposable
    {
        private readonly SQLiteConnection _connection;

        public DatabaseContext(string databaseFilePath)
        {
            _connection = new SQLiteConnection($"Data Source={databaseFilePath};Version=3;");
            _connection.Open();
        }

        public void CreateTables()
        {
            string palletTableQuery = @"
                CREATE TABLE IF NOT EXISTS Pallets (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Width REAL,
                    Height REAL,
                    Depth REAL,
                    Weight REAL
                );";

            string boxTableQuery = @"
                CREATE TABLE IF NOT EXISTS Boxes (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    PalletID INTEGER,
                    Width REAL,
                    Height REAL,
                    Depth REAL,
                    Weight REAL,
                    ProductionDate TEXT,
                    ExpirationDate TEXT,
                    FOREIGN KEY(PalletID) REFERENCES Pallets(ID)
                );";

            using (var command = new SQLiteCommand(palletTableQuery, _connection))
            {
                command.ExecuteNonQuery();
            }

            using (var command = new SQLiteCommand(boxTableQuery, _connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void AddPallet(Pallet pallet)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                var palletCommand = new SQLiteCommand("INSERT INTO Pallets (Width, Height, Depth, Weight) VALUES (@Width, @Height, @Depth, @Weight);", _connection);
                palletCommand.Parameters.AddWithValue("@Width", pallet.Width);
                palletCommand.Parameters.AddWithValue("@Height", pallet.Height);
                palletCommand.Parameters.AddWithValue("@Depth", pallet.Depth);
                palletCommand.Parameters.AddWithValue("@Weight", pallet.Weight);
                palletCommand.ExecuteNonQuery();

                int palletId = (int)_connection.LastInsertRowId;
                foreach (var box in pallet.Boxes)
                {
                    var boxCommand = new SQLiteCommand("INSERT INTO Boxes (PalletID, Width, Height, Depth, Weight, ProductionDate, ExpirationDate) VALUES (@PalletID, @Width, @Height, @Depth, @Weight, @ProductionDate, @ExpirationDate);", _connection);
                    boxCommand.Parameters.AddWithValue("@PalletID", palletId);
                    boxCommand.Parameters.AddWithValue("@Width", box.Width);
                    boxCommand.Parameters.AddWithValue("@Height", box.Height);
                    boxCommand.Parameters.AddWithValue("@Depth", box.Depth);
                    boxCommand.Parameters.AddWithValue("@Weight", box.Weight);
                    boxCommand.Parameters.AddWithValue("@ProductionDate", box.ProductionDate?.ToString("yyyy-MM-dd"));
                    boxCommand.Parameters.AddWithValue("@ExpirationDate", box.ExpirationDate?.ToString("yyyy-MM-dd"));
                    boxCommand.ExecuteNonQuery();
                }

                transaction.Commit();
            }
        }

        public List<Pallet> GetPallets()
        {
            var pallets = new List<Pallet>();
            using (var command = new SQLiteCommand("SELECT * FROM Pallets;", _connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var pallet = new Pallet(reader.GetInt32(0), reader.GetDouble(1), reader.GetDouble(2), reader.GetDouble(3));
                    pallets.Add(pallet);
                }
            }

            foreach (var pallet in pallets)
            {
                using (var command = new SQLiteCommand("SELECT * FROM Boxes WHERE PalletID = @PalletID;", _connection))
                {
                    command.Parameters.AddWithValue("@PalletID", pallet.ID);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var box = new Box(
                                reader.GetInt32(0),
                                reader.GetDouble(2),
                                reader.GetDouble(3),
                                reader.GetDouble(4),
                                reader.GetDouble(5),
                                productionDate: reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6),
                                expirationDate: reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7)
                            );
                            pallet.AddBox(box);
                        }
                    }
                }
            }
            return pallets;
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}