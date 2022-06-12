using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace СonsistentAndParallel
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("1. Переконатись в правильності роботи послідовного алгоритму множення матриць");
            Console.WriteLine("2. Переконатись в правильності роботи послідовного алгоритму множення матриць\n" +
                "   за допомогою власних матриць з файлів .csv");
            Console.WriteLine("3. Провести виміри часу роботи послідовного алгоритму");
            Console.WriteLine("4. Створити файли .csv з матрицями заданих розмірностей");
            Console.WriteLine("5. Переконатись в правильності роботи паралельного алгоритму " +
                "(версія стрічкового алгоритму №1)");
            Console.WriteLine("6. Переконатись в правильності роботи паралельного алгоритму " +
                "(версія стрічкового алгоритму №2)");
            Console.WriteLine("7. Провести виміри часу роботи паралельного алгоритму " +
                 "(версія стрічкового алгоритму №1)");
            Console.WriteLine("8. Провести виміри часу роботи паралельного алгоритму " +
                 "(версія стрічкового алгоритму №2)");
            Console.WriteLine("9. Провести виміри часу роботи паралельного алгоритму в залежності від кількості потоків" +
                "(версія стрічкового алгоритму №1)");
            Console.WriteLine("10. Провести виміри часу роботи паралельного алгоритму в залежності від кількості потоків" +
                "(версія стрічкового алгоритму №2)");

            int choice = int.Parse(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    {
                        WorkTestWithRandomGen();
                        break;
                    }
                case 2:
                    {
                        WorkTestFromFile();
                        break;
                    }
                case 3:
                    {
                        TimeMeasuring();
                        break;
                    }
                case 4:
                    {
                        CreateMatrixFile();
                        break;
                    }
                case 5:
                    {
                        StripedVersionOneTest();
                        break;
                    }
                case 6:
                    {
                        StripedVersionTwoTest();
                        break;
                    }
                case 7:
                    {
                        StripedVersionOneTimeMeasuring();
                        break;
                    }
                case 8:
                    {
                        StripedVersionTwoTimeMeasuring();
                        break;
                    }
                case 9:
                    {
                        StripedOneWithThreads();
                        break;
                    }
                case 10:
                    {
                        StripedTwoWithThreads();
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Необхідно вибрати номер зі списку");
                        break;
                    }
            }
        }

        static void StripedTwoWithThreads()
        {

                int startRank = 2000;
                Console.WriteLine("Введіть кількість ітерацій досліду");
                int nIterations = int.Parse(Console.ReadLine());
                Console.WriteLine("Введить кількість повторень для усереднення результатів досліду");
                int avgIterations = int.Parse(Console.ReadLine());

                if (startRank <= 0 | nIterations <= 0 | avgIterations <= 0) throw new Exception();
                int n = 0;
                long[] times = new long[nIterations];
                for (int i = 2; i <= Math.Pow(2, nIterations); i *= 2)
                {
                    
                    long time = 0;
                    for (int j = 0; j < avgIterations; j++)
                    {
                        Matrix A = new Matrix(startRank, startRank);
                        Matrix B = new Matrix(startRank, startRank);
                        Matrix C = new Matrix(startRank, startRank);
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        C = Matrix.StripedMultiplicationVersionTwo(A, B, C, i);
                        stopwatch.Stop();
                        time += stopwatch.ElapsedMilliseconds;
                    }
                    time /= avgIterations;
                    times[n] = time;
                    n++;
                    Console.WriteLine($"Множення матриць розмірності {startRank} x {startRank} з використанням {i} потоків було завершено за {time}мс.");
                }
                string results = string.Join(",", times);
                File.WriteAllText($"./results_parallel_v2_threads.csv", results);

        }

        static void StripedOneWithThreads()
        {
            try
            {
                int startRank = 2000;
                Console.WriteLine("Введіть кількість ітерацій досліду");
                int nIterations = int.Parse(Console.ReadLine());
                Console.WriteLine("Введить кількість повторень для усереднення результатів досліду");
                int avgIterations = int.Parse(Console.ReadLine());

                if (startRank <= 0 | nIterations <= 0 | avgIterations <= 0) throw new Exception();
                int n = 0;
                long[] times = new long[nIterations];
                for (int i = 2; i <= Math.Pow(2, nIterations); i *= 2)
                {
                    
                    long time = 0;
                    for (int j = 0; j < avgIterations; j++)
                    {
                        Matrix A = new Matrix(startRank, startRank);
                        Matrix B = new Matrix(startRank, startRank);
                        Matrix C = new Matrix(startRank, startRank);
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        C = Matrix.StripedMultiplicationVersionOne(A, B, C, i);
                        stopwatch.Stop();
                        time += stopwatch.ElapsedMilliseconds;
                    }
                    time /= avgIterations;
                    times[n] = time;
                    n++;
                    Console.WriteLine($"Множення матриць розмірності {startRank} x {startRank} з використанням {i} потоків було завершено за {time}мс.");
                }
                string results = string.Join(",", times);
                File.WriteAllText($"./results_parallel_v1_threads.csv", results);
            }
            catch (Exception)
            {
                Console.WriteLine("Стартовий ранг, к-сть ітерацій та потоків мають бути більше 0");
            }
        }

        static void StripedVersionOneTest()
        {
            try
            {
                Console.WriteLine("Вкажіть назву файлу .csv з матицею A:");
                string path1 = Console.ReadLine();
                Console.WriteLine("Вкажіть назву файлу .csv з матицею В:");
                string path2 = Console.ReadLine();

                Console.WriteLine("Матриця А:");
                Matrix matrix1 = new Matrix($"./{path1}");
                matrix1.DrawMatrix();
                Console.WriteLine();
                Console.WriteLine("Матриця В:");
                Matrix matrix2 = new Matrix($"./{path2}");
                matrix2.DrawMatrix();
                Matrix matrix3 = new Matrix(matrix1.GetRowSize(), matrix2.GetColumnSize());
                Console.WriteLine();
                Console.WriteLine("Реузльтуюча матриця С:");
                matrix3 = Matrix.StripedMultiplicationVersionOne(matrix1, matrix2, matrix3, 6);
                matrix3.DrawMatrix();
            }
            catch (Exception)
            {
                Console.WriteLine("Файли не були знайдені або");
                Console.WriteLine("К-сть стовпців в першій матриці має дорівнювати к-сті рядків в другій");
                Console.WriteLine("Перезапустіть додаток та вкажіть правильні файли з правильними матрицями");
            }
        }

        static void StripedVersionTwoTest()
        {
            try
            {
                Console.WriteLine("Вкажіть назву файлу .csv з матицею A:");
                string path1 = Console.ReadLine();
                Console.WriteLine("Вкажіть назву файлу .csv з матицею В:");
                string path2 = Console.ReadLine();

                Console.WriteLine("Матриця А:");
                Matrix matrix1 = new Matrix($"./{path1}");
                matrix1.DrawMatrix();
                Console.WriteLine();
                Console.WriteLine("Матриця В:");
                Matrix matrix2 = new Matrix($"./{path2}");
                matrix2.DrawMatrix();
                Matrix matrix3 = new Matrix(matrix1.GetRowSize(), matrix2.GetColumnSize());
                Console.WriteLine();
                Console.WriteLine("Реузльтуюча матриця С:");
                matrix3 = Matrix.StripedMultiplicationVersionTwo(matrix1, matrix2, matrix3, 6);
                matrix3.DrawMatrix();
            }
            catch (Exception)
            {
                Console.WriteLine("Файли не були знайдені або");
                Console.WriteLine("К-сть стовпців в першій матриці має дорівнювати к-сті рядків в другій");
                Console.WriteLine("Перезапустіть додаток та вкажіть правильні файли з правильними матрицями");
            }
        }


        static void StripedVersionOneTimeMeasuring()
        {
            try
            {
                Console.WriteLine("Введіть стартовий ранг матриць");
                int startRank = int.Parse(Console.ReadLine());
                Console.WriteLine("Введіть кількість ітерацій досліду");
                int nIterations = int.Parse(Console.ReadLine());
                Console.WriteLine("Введить кількість повторень для усереднення результатів досліду");
                int avgIterations = int.Parse(Console.ReadLine());
                Console.WriteLine("Введіть кількість потоків для використання");
                int numThreads = int.Parse(Console.ReadLine());
                if (startRank <= 0 | nIterations <= 0 | avgIterations <= 0 | numThreads <= 0) throw new Exception();

                long[] times = new long[nIterations];
                for (int i = 1; i <= nIterations; i++)
                {
                    long time = 0;
                    for (int j = 0; j < avgIterations; j++)
                    {
                        Matrix A = new Matrix(i * startRank, i * startRank);
                        Matrix B = new Matrix(i * startRank, i * startRank);
                        Matrix C = new Matrix(i * startRank, i * startRank);
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        C = Matrix.StripedMultiplicationVersionOne(A, B, C, numThreads);
                        stopwatch.Stop();
                        time += stopwatch.ElapsedMilliseconds;
                    }
                    time /= avgIterations;
                    times[i - 1] = time;
                    Console.WriteLine($"Множення матриць розмірності {i * startRank} x {i * startRank} було завершено за {time}мс.");
                }
                string results = string.Join(",", times);
                File.WriteAllText($"./results_parallel_v1_threads_{numThreads}_iters_{nIterations}.csv", results);
            }
            catch (Exception)
            {
                Console.WriteLine("Стартовий ранг, к-сть ітерацій та потоків мають бути більше 0");
            }
        }


        static void StripedVersionTwoTimeMeasuring()
        {
            try
            {
                Console.WriteLine("Введіть стартовий ранг матриць");
                int startRank = int.Parse(Console.ReadLine());
                Console.WriteLine("Введіть кількість ітерацій досліду");
                int nIterations = int.Parse(Console.ReadLine());
                Console.WriteLine("Введить кількість повторень для усереднення результатів досліду");
                int avgIterations = int.Parse(Console.ReadLine());
                Console.WriteLine("Введіть кількість потоків для використання");
                int numThreads = int.Parse(Console.ReadLine());
                if (startRank <= 0 | nIterations <= 0 | avgIterations <= 0 | numThreads <= 0) throw new Exception();

                long[] times = new long[nIterations];
                for (int i = 1; i <= nIterations; i++)
                {
                    long time = 0;
                    for (int j = 0; j < avgIterations; j++)
                    {
                        Matrix A = new Matrix(i * startRank, i * startRank);
                        Matrix B = new Matrix(i * startRank, i * startRank);
                        Matrix C = new Matrix(i * startRank, i * startRank);
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        C = Matrix.StripedMultiplicationVersionTwo(A, B, C, numThreads);
                        stopwatch.Stop();
                        time += stopwatch.ElapsedMilliseconds;
                    }
                    time /= avgIterations;
                    times[i - 1] = time;
                    Console.WriteLine($"Множення матриць розмірності {i * startRank} x {i * startRank} було завершено за {time}мс.");
                }
                string results = string.Join(",", times);
                File.WriteAllText($"./results_parallel_v2_threads_{numThreads}_iters_{nIterations}.csv", results);
            }
            catch (Exception)
            {
                Console.WriteLine("Стартовий ранг, к-сть ітерацій та потоків мають бути більше 0");
            }
        }

        static void WorkTestWithRandomGen()
        {
            try
            {
                Console.WriteLine("Вкажіть розмірності матриці A:");
                int m = int.Parse(Console.ReadLine());
                int n = int.Parse(Console.ReadLine());

                Console.WriteLine("Вкажіть розмірності матриці B:");
                int k = int.Parse(Console.ReadLine());
                int i = int.Parse(Console.ReadLine());

                if (m <= 0 | n <= 0 | k <= 0 | i <= 0) throw new Exception("Розмірності матриць мають бути більше 0");
                if (n != k) throw new Exception("К-сть стовпців в першій матриці має дорівнювати к-сті рядків в другій");                
                Matrix matrix1 = new Matrix(m, n);
                Matrix matrix2 = new Matrix(k, i);
                Matrix matrix3 = Matrix.MultiplyWithTest(matrix1, matrix2);
            }
            catch
            {
                Console.WriteLine("Розмірності матриць мають бути більше 0");
                Console.WriteLine("К-сть стовпців в першій матриці має дорівнювати к-сті рядків в другій");
                Console.WriteLine("Перезапустіть додаток та введіть правильні числа");
            }
        }

        static void WorkTestFromFile()
        {
            try
            {
                Console.WriteLine("Вкажіть назву файлу .csv з матицею A:");
                string path1 = Console.ReadLine();
                Console.WriteLine("Вкажіть назву файлу .csv з матицею В:");
                string path2 = Console.ReadLine();

                Matrix matrix1 = new Matrix($"./{path1}");
                Matrix matrix2 = new Matrix($"./{path2}");
                Matrix matrix3 = Matrix.MultiplyWithTest(matrix1, matrix2);
            }
            catch (Exception)
            {
                Console.WriteLine("Файли не були знайдені або");
                Console.WriteLine("К-сть стовпців в першій матриці має дорівнювати к-сті рядків в другій");
                Console.WriteLine("Перезапустіть додаток та вкажіть правильні файли з павильними матрицями");
            }

        }

        static void TimeMeasuring()
        {
            try
            {
                Console.WriteLine("Введіть стартовий ранг матриць");
                int startRank = int.Parse(Console.ReadLine());
                Console.WriteLine("Введіть кількість ітерацій досліду");
                int nIterations = int.Parse(Console.ReadLine());
                Console.WriteLine("Введить кількість повторень для усереднення результатів досліду");
                int avgIterations = int.Parse(Console.ReadLine());

                if (startRank <= 0 | nIterations <= 0 | avgIterations <= 0) throw new Exception();

                long[] times = new long[nIterations];
                for (int i = 1; i <= nIterations; i++)
                {
                    long time = 0;
                    for (int j = 0; j < avgIterations; j++)
                    {
                        Matrix A = new Matrix(i * startRank, i * startRank);
                        Matrix B = new Matrix(i * startRank, i * startRank);
                        Matrix C = new Matrix(i * startRank, i * startRank);
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        C = Matrix.Multiply(A, B, C);
                        stopwatch.Stop();
                        time += stopwatch.ElapsedMilliseconds;
                    }
                    time /= avgIterations;
                    times[i - 1] = time;
                    Console.WriteLine($"Множення матриць розмірності {i * startRank} x {i * startRank} було завершено за {time}мс.");
                }
                string results = string.Join(",", times);
                File.WriteAllText("./results.csv", results);
            }
            catch (Exception)
            {
                Console.WriteLine("Стартовий ранг, к-сть ітерацій та потоків мають бути більше 0");
            }
        }

        static void CreateMatrixFile()
        {
            try
            {
                Console.WriteLine("Вкажіть розмірності матриці A:");
                int m = int.Parse(Console.ReadLine());
                int n = int.Parse(Console.ReadLine());

                Console.WriteLine("Вкажіть розмірності матриці B:");
                int k = int.Parse(Console.ReadLine());
                int i = int.Parse(Console.ReadLine());

                if (m <= 0 | n <= 0 | k <= 0 | i <= 0) throw new Exception("Розмірності матриць мають бути більше 0");

                Matrix matrix1 = new Matrix(m, n);
                Matrix matrix2 = new Matrix(k, i);
                matrix1.WriteMatrixToFile("./matrix1.csv");
                matrix2.WriteMatrixToFile("./matrix2.csv");
                Console.WriteLine("Матриці успішно створені та записані у файл matrix1.csv, matrix2.csv");
            }
            catch (Exception)
            {
                Console.WriteLine("Розмірності матриць мають бути більше 0");
                Console.WriteLine("Перезапустіть додаток та введіть правильні числа");
            }
        }

    }
}
