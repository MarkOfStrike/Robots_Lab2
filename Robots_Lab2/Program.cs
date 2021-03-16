using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Robots_Lab2
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"D:/Desktop/test.txt";

            var objects = GetObjects(path);

            var avg = GetAvg(objects);
            int ij = avg[0].Length;

            var w = GetW(ij, objects, avg);
            var a = Matrix.Inverse(w);





        }

        static double[] TableH(IList<ObjectClass> objects)
        {
            var h = new double[objects.Count];



            return h;
        }

        static double CalculateH(int p, int n, int g, double[] w, double[] a, double[] avg)
        {
            var result = 0d;

            var sum = 0d;

            for (int i = 1; i < p; i++)
            {
                var bk = CalculateB(n, g, p, a, avg);
                sum += bk;
                result += bk * w[i];
            }

            var b0 = GetB0(sum);

            return 0;
        }

        static double CalculateB(int n, int g,int p, double[] a, double[] avg)
        {
            var sum = 0d;

            for (int i = 0; i < p; i++)
            {
                sum += a[i] * avg[i];
            }

            sum *= n - g;

            return sum;
        }

        private static double GetB0(double sum)
        {



            return -0.5 * sum; ;
        }

        static List<ObjectClass> GetObjects(string folderPath)
        {
            var objects = new List<ObjectClass>();

            //ЗАМЕНИТЬ НА ПАПКУ С ФАЙЛАМИ!!!
            using (var str = new StreamReader(folderPath, Encoding.UTF8))
            {
                var tmpObj = str.ReadToEnd().Split("\n", StringSplitOptions.RemoveEmptyEntries);

                int name = 1;

                foreach (var obj in tmpObj)
                {
                    objects.Add(new ObjectClass(obj.Split("|", StringSplitOptions.RemoveEmptyEntries), $"{name}"));
                    name++;
                }
            }

            return objects;
        }

        static double[][] GetAvg(IList<ObjectClass> objects)
        {
            var avgMas = new double[objects.Count][];

            for (int a = 0; a < objects.Count; a++)
            {
                var o = objects[a].Objects;
                var mas = new double[o[0].Length];

                for (int s = 0; s < mas.Length; s++)
                {
                    mas[s] = objects[a].Objects.ToList().Sum(sm => sm[s]) / objects[a].Objects.Length;
                }

                avgMas[a] = mas;

            }

            return avgMas;
        }

        static double[][] GetW(int n, IList<ObjectClass> objects, double[][] avg)
        {
            var resultW = new double[n][];

            for (int i = 0; i < n; i++)
            {
                var w = new double[n];

                for (int j = 0; j < w.Length; j++)
                {
                    var resultK = 0d;

                    for (int k = 0; k < objects.Count; k++)
                    {
                        var obj = objects[k].Objects;

                        for (int m = 0; m < obj.Length; m++)
                        {
                            var xi = obj[m][i];
                            var xI = avg[k][i];

                            var xj = obj[m][j];
                            var xJ = avg[k][j];

                            resultK += (xi - xI) * (xj - xJ);
                        }
                    }

                    w[j] = resultK;
                }

                resultW[i] = w;
            }

            return resultW;
        }

    }
}