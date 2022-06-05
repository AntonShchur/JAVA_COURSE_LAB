using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Сonsistent
{
    class Matrix
    {
        private int n_rows;
        private int n_columns;
        private double[,] matrix;

        public Matrix(int n_rows, int n_columns)
        {
            this.n_rows = n_rows;
            this.n_columns = n_columns;
            this.matrix = new double[n_rows, n_columns];

            Random random = new Random();
            for (int i = 0; i < n_rows; i++)
            {
                for (int j = 0; j < n_columns; j++)
                {
                    this.matrix[i, j] = random.Next(-100, 100);
                }
            }
        }

        public Matrix(string path_to_csv)
        {  
            int n_rows = 0;
            int n_cols = 0;
            List<string> numbers = new List<string>();
            using (var reader = new System.IO.StreamReader($"{path_to_csv}"))
            {


                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Split(',');
                    n_cols = line.Length;
                    n_rows += 1;
                    foreach (var number in line)
                    {
                        numbers.Add(number);
                    }
                }
            }

            this.n_rows = n_rows;
            this.n_columns = n_cols;
            this.matrix = new double[n_rows, n_cols];
            int k = 0;
            for (int i = 0; i < n_rows; i++)
            {
                for (int j = 0; j < n_cols; j++)
                {
                    this.matrix[i, j] = double.Parse(numbers[k]);
                    k++;
                }
            }
        }
        public void WriteMatrixToFile(string path)
        {
            File.WriteAllText(path, string.Empty);
            for(int i=0; i < n_rows; i++)
            {
                List<double> numbers = new List<double>();
                
                for (int j=0; j < n_columns; j++)
                {
                    numbers.Add(matrix[i, j]);
                }
                File.AppendAllText(path, string.Join(',', numbers) + "\n");
            }
        }

        public void DrawMatrix()
        {
            for (int i = 0; i < n_rows; i++)
            {
                for (int j = 0; j < n_columns; j++)
                {
                    Console.Write(matrix[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        public static Matrix MultiplyWithTest(Matrix A, Matrix B)
        {
            if (A.n_columns != B.n_rows)
            {
                throw new Exception("Для можливості множення необхідно, щоб к-сть" +
                    " рядків першої матриці співпадала з к-стю стовпців другої");
            };
            Matrix C = new Matrix(A.n_rows, B.n_columns);
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            C = Matrix.Multiply(A, B, C);
            stopWatch.Stop();
            long elapsedTime = stopWatch.ElapsedMilliseconds;
            Console.WriteLine("Вхідна матриця А:");
            A.DrawMatrix();
            Console.WriteLine("Вхідна матриця B:");
            B.DrawMatrix();
            Console.WriteLine("Результуюча матриця С:");
            C.DrawMatrix();
            Console.WriteLine($"Час, затрачений на операцію послідовного множення {elapsedTime}мс.");
            return C;
        }

        public static Matrix Multiply(Matrix A, Matrix B, Matrix C)
        {
            for (int i = 0; i < A.n_rows; i++)
            {

                for (int j = 0; j < B.n_columns; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < B.n_rows; k++)
                    {
                        sum += A.matrix[i, k] * B.matrix[k, j];
                    }
                    C.matrix[i, j] = sum;
                }

            }
            return C;
        }
    }
}
