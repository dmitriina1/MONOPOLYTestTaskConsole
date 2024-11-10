using MONOPOLYTestTaskConsole.Database;
using MONOPOLYTestTaskConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MONOPOLYTestTaskConsole.Service
{
    public class PalletService
    {
        private readonly string _databaseFilePath;

        public PalletService(string databaseFilePath)
        {
            _databaseFilePath = databaseFilePath;
        }

        public void InitializeDatabase()
        {
            using (var context = new DatabaseContext(_databaseFilePath))
            {
                context.CreateTables();
            }
        }

        public List<Pallet> GetSortedPallets()
        {
            using (var context = new DatabaseContext(_databaseFilePath))
            {
                List<Pallet> pallets = context.GetPallets();

                return pallets
                    .Where(p => p.ExpirationDate.HasValue)
                    .OrderBy(p => p.ExpirationDate.GetValueOrDefault())
                    .ThenBy(p => p.Weight)
                    .ToList();
            }
        }

        public List<Pallet> GetTopPalletsByMaxExpiration()
        {
            using (var context = new DatabaseContext(_databaseFilePath))
            {
                List<Pallet> pallets = context.GetPallets();

                return pallets
                    .Where(p => p.Boxes.Any())
                    .OrderByDescending(p => p.Boxes.Max(box => box.ExpirationDate))
                    .ThenBy(p => p.Volume)
                    .Take(3)
                    .ToList();
            }
        }

        public void FillDatabaseWithSampleData()
        {
            try
            {
                using (var context = new DatabaseContext(_databaseFilePath))
                {
                    var pallet1 = new Pallet(1, 100, 100, 150);
                    pallet1.AddBox(new Box(1, 50, 50, 50, 10, productionDate: new DateTime(2023, 1, 1)));
                    pallet1.AddBox(new Box(2, 50, 50, 50, 15, productionDate: new DateTime(2023, 1, 10)));
                    context.AddPallet(pallet1);

                    var pallet2 = new Pallet(2, 100, 100, 150);
                    pallet2.AddBox(new Box(3, 50, 50, 50, 12, expirationDate: new DateTime(2023, 4, 1)));
                    context.AddPallet(pallet2);

                    var pallet3 = new Pallet(3, 120, 120, 160);
                    pallet3.AddBox(new Box(4, 60, 60, 60, 20, productionDate: new DateTime(2023, 2, 15)));
                    context.AddPallet(pallet3);

                    Console.WriteLine("Данные успешно добавлены в базу.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении данных в базу: {ex.Message}");
            }
        }

        public void AddPalletFromConsole()
        {
            try
            {
                using (var context = new DatabaseContext(_databaseFilePath))
                {
                    Console.WriteLine("Введите параметры паллеты:");

                    double palletWidth = PromptForDouble("Ширина паллеты: ");
                    double palletHeight = PromptForDouble("Высота паллеты: ");
                    double palletDepth = PromptForDouble("Глубина паллеты: ");

                    var pallet = new Pallet(0, palletWidth, palletHeight, palletDepth);

                    bool addingBoxes = true;
                    while (addingBoxes)
                    {
                        Console.WriteLine("\nДобавьте коробку на паллету:");

                        double boxWidth = PromptForDouble("Ширина коробки: ");
                        double boxHeight = PromptForDouble("Высота коробки: ");
                        double boxDepth = PromptForDouble("Глубина коробки: ");
                        double boxWeight = PromptForDouble("Вес коробки: ");

                        DateTime? expirationDate = PromptForOptionalDate("Введите срок годности коробки (в формате dd.MM.yyyy) или оставьте пустым: ");

                        DateTime? productionDate = null;
                        if (!expirationDate.HasValue)
                        {
                            productionDate = PromptForRequiredDate("Введите дату производства коробки (в формате dd.MM.yyyy): ");
                        }

                        if (boxWidth <= palletWidth && boxDepth <= palletDepth)
                        {
                            var box = new Box(0, boxWidth, boxHeight, boxDepth, boxWeight, productionDate, expirationDate);
                            pallet.AddBox(box);
                        }
                        else
                        {
                            Console.WriteLine("Ошибка: размеры коробки превышают размеры паллеты. Пожалуйста, введите размеры коробки снова.");
                            continue;
                        }

                        Console.Write("Добавить еще одну коробку на паллету? (y/n): ");
                        addingBoxes = Console.ReadLine().ToLower() == "y";
                    }

                    context.AddPallet(pallet);
                    Console.WriteLine("Паллеты и коробки успешно добавлены в базу.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении данных в базу: {ex.Message}");
            }
        }

        private double PromptForDouble(string prompt)
        {
            double value;
            while (true)
            {
                Console.Write(prompt);
                if (double.TryParse(Console.ReadLine(), out value))
                {
                    return value;
                }
                Console.WriteLine("Ошибка: введите корректное числовое значение.");
            }
        }

        private DateTime? PromptForOptionalDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    return null;
                }

                if (DateTime.TryParseExact(input, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
                {
                    return dateTime;
                }
                Console.WriteLine("Ошибка: неверный формат даты. Пожалуйста, введите дату в формате dd.MM.yyyy или оставьте пустым.");
            }
        }

        private DateTime PromptForRequiredDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
                {
                    return dateTime;
                }
                Console.WriteLine("Ошибка: неверный формат даты. Пожалуйста, введите дату в формате dd.MM.yyyy.");
            }
        }

        public void DisplayPallets(List<Pallet> pallets)
        {
            foreach (var pallet in pallets)
            {
                Console.WriteLine($"Палета ID: {pallet.ID}, Вес: {pallet.Weight} кг, Срок годности: {pallet.ExpirationDate?.ToString("dd.MM.yyyy")}, Объем: {pallet.Volume} м³");
            }
        }
    }
}