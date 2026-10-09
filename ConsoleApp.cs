using System;
using lab1forms.Services.Lab1;

namespace lab1forms
{
    
    public class ConsoleApp
    {
        public static void Run()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== ЛАБОРАТОРНАЯ РАБОТА №1 (Консольная версия) ===\n");

            BitwiseStringAdditionService bitwiseService = new BitwiseStringAdditionService();
            DivisorCountService divisorService = new DivisorCountService();
            PythagoreanTriplesService pythagoreanService = new PythagoreanTriplesService();

            while (true)
            {
                Console.WriteLine("\nВыберите задание:");
                Console.WriteLine("1 - Поразрядное сложение строк");
                Console.WriteLine("2 - Найти числа с N делителями (1-200)");
                Console.WriteLine("3 - Найти A + B² = C² (1-20)");
                Console.WriteLine("0 - Выход");
                Console.Write("\nВаш выбор: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("\nВведите фамилию: ");
                        string surname = Console.ReadLine();
                        Console.Write("Введите имя: ");
                        string name = Console.ReadLine();
                        Console.WriteLine("\n" + bitwiseService.Execute(surname, name));
                        break;

                    case "2":
                        Console.Write("\nВведите количество делителей N: ");
                        string n = Console.ReadLine();
                        Console.WriteLine("\n" + divisorService.Execute(n));
                        break;

                    case "3":
                        Console.WriteLine("\n" + pythagoreanService.Execute());
                        break;

                    case "0":
                        Console.WriteLine("\nДо свидания!");
                        return;

                    default:
                        Console.WriteLine("\nНеверный выбор! Попробуйте снова.");
                        break;
                }

                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}
