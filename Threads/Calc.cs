using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Threads
{
    internal class Calc
    {
        public static int GetSum(int[]? arr)
        {
            int sum1 = arr.Sum();
            return sum1;
        }
    }
}
