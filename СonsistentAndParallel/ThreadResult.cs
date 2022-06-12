using System;
using System.Collections.Generic;
using System.Text;

namespace СonsistentAndParallel
{
    class ThreadResult
    {
        public double[,] rows;
        public int[] rows_numbers;

        public ThreadResult(double[,] rows, int[] rows_numbers)
        {
            this.rows = rows;
            this.rows_numbers = rows_numbers;
        }
    }
}
