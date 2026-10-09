using System;
using System.Text;

namespace lab1forms.Services.Lab1
{
    // Базовый интерфейс для всех сервисов
    public interface ILabService
    {
        string Execute(params object[] parameters);
    }

    // Сервис 1: Поразрядное сложение строк
    public class BitwiseStringAdditionService : ILabService
    {
        public string Execute(params object[] parameters)
        {
            if (parameters.Length < 2)
                return "Ошибка: требуется два параметра (фамилия и имя)";

            string surname = parameters[0].ToString();
            string name = parameters[1].ToString();

            StringBuilder result = new StringBuilder();
            result.AppendLine("=== Поразрядное сложение строк ===\n");
            result.AppendLine($"Фамилия: {surname} (длина: {surname.Length})");
            result.AppendLine($"Имя: {name} (длина: {name.Length})\n");

            int maxLength = Math.Max(surname.Length, name.Length);
            
            for (int i = 0; i < maxLength; i++)
            {
                char char1 = i < surname.Length ? surname[i] : '\0';
                char char2 = i < name.Length ? name[i] : '\0';

                int code1 = (int)char1;
                int code2 = (int)char2;
                int sum = code1 + code2;

                result.AppendLine($"Позиция {i + 1}:");
                
                // Показываем символы или "пусто"
                string str1 = i < surname.Length ? $"'{char1}'" : "[пусто]";
                string str2 = i < name.Length ? $"'{char2}'" : "[пусто]";
                
                result.AppendLine($"  {str1} (код: {code1}) + {str2} (код: {code2}) = {sum}");
                
                // Выводим результат в разных форматах
                if (sum <= 255)
                    result.AppendLine($"  Результат: символ '{(char)sum}' (ASCII {sum})");
                else
                    result.AppendLine($"  Результат: код {sum} (0x{sum:X4})");
                
                result.AppendLine();
            }

            return result.ToString();
        }
    }

    // Сервис 2: Поиск чисел с N делителями
    public class DivisorCountService : ILabService
    {
        public string Execute(params object[] parameters)
        {
            if (parameters.Length < 1)
                return "Ошибка: требуется параметр N (количество делителей)";

            if (!int.TryParse(parameters[0].ToString(), out int n) || n <= 0)
                return "Ошибка: N должно быть положительным числом";

            StringBuilder result = new StringBuilder();
            result.AppendLine($"=== Поиск чисел с {n} делителями (от 1 до 200) ===\n");

            bool found = false;

            for (int num = 1; num <= 200; num++)
            {
                int divisorCount = CountDivisors(num);
                
                if (divisorCount == n)
                {
                    result.AppendLine($"Число {num}: делители = {GetDivisors(num)} (количество: {divisorCount})");
                    found = true;
                }
            }

            if (!found)
                result.AppendLine($"Числа с {n} делителями не найдены.");

            return result.ToString();
        }

        private int CountDivisors(int number)
        {
            int count = 0;
            for (int i = 1; i <= number; i++)
            {
                if (number % i == 0)
                    count++;
            }
            return count;
        }

        private string GetDivisors(int number)
        {
            StringBuilder divisors = new StringBuilder();
            for (int i = 1; i <= number; i++)
            {
                if (number % i == 0)
                {
                    if (divisors.Length > 0)
                        divisors.Append(", ");
                    divisors.Append(i);
                }
            }
            return divisors.ToString();
        }
    }

    // Сервис 3: Поиск пифагоровых троек (A + B² = C²)
    public class PythagoreanTriplesService : ILabService
    {
        public string Execute(params object[] parameters)
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("=== Поиск чисел A, B, C (1-20): A + B² = C² ===\n");

            bool found = false;

            for (int a = 1; a <= 20; a++)
            {
                for (int b = 1; b <= 20; b++)
                {
                    for (int c = 1; c <= 20; c++)
                    {
                        if (a + b * b == c * c)
                        {
                            result.AppendLine($"A = {a}, B = {b}, C = {c}");
                            result.AppendLine($"  Проверка: {a} + {b}² = {a} + {b * b} = {a + b * b}");
                            result.AppendLine($"  C² = {c}² = {c * c}");
                            result.AppendLine($"  {a + b * b} = {c * c} ✓\n");
                            found = true;
                        }
                    }
                }
            }

            if (!found)
                result.AppendLine("Решения не найдены.");

            return result.ToString();
        }
    }
}
