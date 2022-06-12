using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections;

namespace СonsistentAndParallel
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

        public int GetRowSize()
        {
            return n_rows;
        }
        public int GetColumnSize()
        {
            return n_columns;
        }

        public double[] GetRow(int n)
        {
            double[] arr = new double[n_columns];
            for(int i=0; i < n_columns; i++)
            {
                arr[i] = matrix[n, i];
            }
            return arr;
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

        public static Matrix StripedMultiplicationVersionOne(Matrix A, Matrix B, Matrix C, int num_threads)
        {
            List<object> data_args = new List<object>();
            for (int i = 0; i < A.n_rows; i++)
            {
                data_args.Add(new Strip(B, A.GetRow(i), i));
            }
            Hashtable hashtable = Hashtable.Synchronized(new Hashtable());
            Parallel.ForEach(data_args, new ParallelOptions { MaxDegreeOfParallelism = num_threads },
                arguments =>
                {
                    Strip strip = (Strip)arguments;
                    hashtable.Add(strip.row_number, MakeComputationVersionOne(strip.row, strip.B));
                });
            foreach(int key in hashtable.Keys)
            {
                double[] str = (double[])hashtable[key]; 
                for(int i=0; i < str.Length; i++)
                {
                    C.matrix[key, i] = str[i];
                } 
            }            
            return C;
        }

       
        public static Matrix StripedMultiplicationVersionTwo(Matrix A, Matrix B, Matrix C, int num_threads)
        {
            List<object> data_args = new List<object>();
            Hashtable hashtable = Hashtable.Synchronized(new Hashtable());
            int length = A.n_rows;
            int num = num_threads;
            if (num_threads > length)
                num = length;
            int[] chunks = GetChunks(length, num);
            int k = 0;
            foreach(int chunk in chunks)
            {
                double[,] rows = new double[chunk, A.n_columns];
                List<int> rows_numbers = new List<int>();
                for (int i=0; i < chunk; i++)
                {
                    for (int j = 0; j < A.n_columns; j++)
                        rows[i, j] = A.matrix[k, j];
                    rows_numbers.Add(k);
                    k++;
                }
                data_args.Add(new AggStrip(B, rows, rows_numbers.ToArray()));
            }
            Task<ThreadResult>[] threads = new Task<ThreadResult>[num];
            for(int i=0; i < threads.Length; i++)
            {
                AggStrip strip = (AggStrip)data_args[i];
                threads[i] = Task<ThreadResult>.Factory.StartNew(() => MakeComputationVersionThree(strip.rows, strip.B, strip.rows_numbers));
                
            }
            Task.WaitAll(threads);
            foreach (var thread in threads)
            {
                ThreadResult result = thread.Result;
                
                int l = 0;
                foreach (int row in result.rows_numbers)
                {
                    for (int i = 0; i < result.rows.GetLength(1); i++)
                    {
                        C.matrix[row, i] = result.rows[l, i];
                    }
                    l++;
                }
            }
            return C;
        }

        public static double[] MakeComputationVersionOne(double[] row_a, Matrix B)
        {
            
            double[] result = new double[row_a.Length];
            
            for (int j = 0; j < B.GetColumnSize(); j++)
            {
                for (int i = 0; i < row_a.Length; i++)
                {
                    result[j] += row_a[i] * B.matrix[i, j];
                }

            }

            return result;
        }

        public static double[,] MakeComputationVersionTwo(double[,] rows, Matrix B)
        {
            
            int height = rows.GetLength(0);
            int width = rows.GetLength(1);
            double[,] result = new double[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < B.n_columns; j++)
                {
                    for (int k = 0; k < B.n_rows; k++)
                    {
                        result[i, j] += rows[i, k] * B.matrix[k, j];
                    }
                }
            }
            return result;
        }

        public static ThreadResult MakeComputationVersionThree(double[,] rows, Matrix B, int[] rows_numbers)
        {
            
            int height = rows.GetLength(0);
            int width = rows.GetLength(1);
            double[,] result = new double[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < B.n_columns; j++)
                {
                    for (int k = 0; k < B.n_rows; k++)
                    {
                        result[i, j] += rows[i, k] * B.matrix[k, j];
                    }
                }
            }
            ThreadResult thread_result = new ThreadResult(result, rows_numbers);
            return thread_result;
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

        private static int[] GetChunks(int length, int numChunks)
        {
            int additionChunk = 1;
            if (length % numChunks == 0)
            {
                additionChunk = 0;
            }
            int chunkLength = length / (numChunks - additionChunk);
            int partChunklength = length % (numChunks- additionChunk);
            int nFullChunks = numChunks - additionChunk;
            int nPartChunks = additionChunk;
            int[] chunks = new int[numChunks];
            int k = 0;
            for (int i = 0; i < nFullChunks; i++)
            {
                chunks[k] = chunkLength;
                k++;
            }
            for (int i = 0; i < nPartChunks; i++)
            {
                chunks[k] = partChunklength;
                k++;
            }
            return chunks;
        }
    }
}
