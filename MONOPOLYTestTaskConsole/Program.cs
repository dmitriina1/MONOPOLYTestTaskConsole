using MONOPOLYTestTaskConsole.Models;
using MONOPOLYTestTaskConsole.Service;
using System;
using System.Collections.Generic;

namespace MONOPOLYTestTaskConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string databaseFilePath = "WarehouseItems.db";

            var palletService = new PalletService(databaseFilePath);

            palletService.InitializeDatabase();

            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("Меню:");
                Console.WriteLine("1. Добавить паллету и коробки");
                Console.WriteLine("2. Показать отсортированные паллеты");
                Console.WriteLine("3. Показать топ-3 паллеты по максимальному сроку годности коробок");
                Console.WriteLine("4. Заполнить базу данными для теста");
                Console.WriteLine("5. Выйти");
                Console.Write("Выберите действие: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        palletService.AddPalletFromConsole();
                        break;

                    case "2":
                        List<Pallet> sortedPallets = palletService.GetSortedPallets();
                        palletService.DisplayPallets(sortedPallets);
                        break;

                    case "3":
                        List<Pallet> topPallets = palletService.GetTopPalletsByMaxExpiration();
                        palletService.DisplayPallets(topPallets);
                        break;

                    case "4":
                        palletService.FillDatabaseWithSampleData();
                        break;

                    case "5":
                        exit = true;
                        break;

                    default:
                        Console.WriteLine("Неверный выбор, попробуйте снова.");
                        break;
                }

                if (choice != "5")
                {
                    Console.WriteLine("Нажмите любую клавишу, чтобы продолжить");
                    Console.ReadKey();
                }
            }

            Console.WriteLine("Программа завершена.");
        }
    }
}