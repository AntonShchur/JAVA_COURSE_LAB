using System;
using System.Collections.Generic;
using System.Text;

namespace СonsistentAndParallel
{
    class Strip
    {
        public Matrix B { get; }
        public double[] row { get; }
        public int row_number { get; }

        public Strip(Matrix B, double[] row, int row_number)
        {
            this.B = B;
            this.row = row;
            this.row_number = row_number;
        }
    }
}
