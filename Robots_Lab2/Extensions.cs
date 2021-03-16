using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robots_Lab2
{
    public static class Extensions
    {
        public static string AsString(this double[][] mas)
        {
            string s = "";
            for (int i = 0; i < mas.Length; ++i)
            {
                for (int j = 0; j < mas[i].Length; ++j)
                    s += mas[i][j].ToString() + " ";
                s += Environment.NewLine;
            }
            return s;
        }

        public static double Round(this double source, int count)
        {
            return Math.Round(source, count);
        }
    }
}
