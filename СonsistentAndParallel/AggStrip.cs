using System;
using System.Collections.Generic;
using System.Text;

namespace СonsistentAndParallel
{
    class AggStrip
    {
        public Matrix B { get; }
        public double[,] rows { get; }
        public int[] rows_numbers { get; }

        public AggStrip(Matrix B, double[,] rows, int[] rows_numbers)
        {
            this.B = B;
            this.rows = rows;
            this.rows_numbers = rows_numbers;
        }
    }
}
