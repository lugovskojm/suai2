using System;

namespace lab1forms.Services.Lab2
{
    /// <summary>
    /// Лабораторная работа №2, Вариант 10.
    /// Массив D → массив A:
    ///   Шаг 1: чётный индекс (1-based) → A[i] = D[i]^2
    ///           нечётный индекс        → A[i] = D[i] / i
    ///   Шаг 2: заменить первый элемент A, кратный 5, нулём
    ///   Шаг 3: все элементы A с нечётными индексами → i^2
    /// </summary>
    public static class Lab2Service
    {
        private static double CalcAi(double d, int oneBasedIdx)
            => oneBasedIdx % 2 == 0
               ? d * d
               : d / oneBasedIdx;

        private static void Step2(double[] a)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != 0.0 && Math.Abs(a[i] % 5.0) < 1e-9)
                { a[i] = 0.0; break; }
            }
        }

        private static void Step3(double[] a)
        {
            for (int i = 0; i < a.Length; i++)
            {
                int idx = i + 1;
                if (idx % 2 != 0) a[i] = (double)(idx * idx);
            }
        }

        public static double[] ProcessSequential(double[] D)
        {
            int n = D.Length;
            double[] A = new double[n];
            for (int i = 0; i < n; i++)
                A[i] = CalcAi(D[i], i + 1);
            Step2(A);
            Step3(A);
            return A;
        }

        public static double[] ProcessThreaded(double[] D)
        {
            int n = D.Length;
            double[] A = new double[n];
            int mid = n / 2;

            var t1 = new System.Threading.Thread(() =>
            {
                for (int i = 0; i < mid; i++)
                    A[i] = CalcAi(D[i], i + 1);
            });
            var t2 = new System.Threading.Thread(() =>
            {
                for (int i = mid; i < n; i++)
                    A[i] = CalcAi(D[i], i + 1);
            });

            t1.Start(); t2.Start();
            t1.Join();  t2.Join();

            Step2(A);
            Step3(A);
            return A;
        }
    }
}
