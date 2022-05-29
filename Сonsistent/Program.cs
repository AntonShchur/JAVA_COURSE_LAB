using System;
using System.Diagnostics;
using System.IO;

namespace Сonsistent
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("1. Переконатись в правильності роботи послідовного алгоритму множення матриць");
            Console.WriteLine("2. Переконатись в правильності роботи послідовного алгоритму множення матриць\n" +
                "за допомогою власних матриць з файлів .csv");
            Console.WriteLine("3. Провести виміри часу роботи послідовного алгоритму");
            Console.WriteLine("4. Створити файли .csv з матрицями заданих розмірностей");
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
                default:
                    {
                        Console.WriteLine("Необхідно вибрати номер зі списку");
                        break;
                    }
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
                if (n != k) throw new Exception("К-сть стовпців в першій матриці має дорівнювати к-сті рядків в другій");                Matrix matrix1 = new Matrix(m, n);
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
            int startRank = 100;
            int nIterations = 10;
            int avgIterations = 5;
            long[] times = new long[nIterations];
            for(int i = 1; i <= nIterations; i++)
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
