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
            //var path = @"D:/Desktop/test.txt";
            var path = @"D:\Desktop\Data";

            var trainPath = "D:/Desktop/Data/Train";
            var testPath = "D:/Desktop/Data/Test";

            var numObjects = new int[] { 2, 4, 6, 8, 10 };
            //var numObjects = new int[] { 1,2,3,4 };

            //var objects = GetObjects<IObjectTrain>(trainPath, numObjects);

            //Train(objects);

            //var testObjects = GetObjects<IObjectTest>(testPath, numObjects);
            //var testTrainObject = GetObjects<IObjectTest>(testPath, numObjects);

            //Test(testObjects, objects);

            var info = 0d;
            var numinfo = 0;

            var notinfo = 100d;
            var numNotinfo = 0;

            for (int i = -1; i < numObjects.Length; i++)
            {
                var isZero = i >= 0;

                var nums = numObjects;

                if (isZero)
                {
                    var tmp = numObjects.ToList();
                    tmp.RemoveAt(i);
                    nums = tmp.ToArray();
                }

                var objects = GetObjects<IObjectTrain>(trainPath, nums);
                Train(objects);

                var testObjects = GetObjects<IObjectTest>(testPath, nums);
                //var testObjects = GetObjects<IObjectTest>(trainPath, nums);
                Test(testObjects, objects);

                var allCount = (double)testObjects.Sum(s => s.Objects.GetLength(0));
                var success = (double)testObjects.Sum(s => s.SuccessfulCount);
                var error = (double)testObjects.Sum(s => s.ErrorCount);

                var luck = success / allCount * 100d;

                if (isZero)
                {
                    if (luck > info)
                    {
                        info = luck;
                        numNotinfo = numObjects[i];
                    }

                    if (luck < notinfo)
                    {
                        notinfo = luck;
                        numinfo = numObjects[i];
                    }
                }

                Console.WriteLine($"С признаками: {string.Join(", ", nums)}. Количество успешно распознанных: {success}. Количество не распознанных: {error}. Успех: {luck}%");
            }

            Console.WriteLine($"По итогу самый информативный признак: {numinfo}, а самый не информативный: {numNotinfo}.");

            //var avg = GetAvg(objects);
            //int ij = avg[0].Length;

            //var w = GetW(ij, objects, avg);
            //var a = Matrix.Inverse(w);



            //Console.WriteLine(w.AsString());
            //Console.WriteLine(a.AsString());

            //var list = new List<double>();



            //var p = objects[0].Objects[0].Length;
            //var n = objects.Sum(s => s.Objects.GetLength(0));
            //var g = objects.Count;

            //var hs = new double[g][];


            //for (int i = 0; i < g; i++)
            //{
            //    var bks = new List<double>();

            //    for (int j = 0; j < p; j++)
            //    {
            //        bks.Add(GetBiK(a[j], avg[i], n, g, p));
            //    }

            //    var b0 = GetB0(bks.ToArray(), avg[i]);

            //    var hdsd = new double[p];

            //    for (int f = 0; f < p; f++)
            //    {
            //        hdsd[f] = GetH(b0, bks.ToArray(), objects[i].Objects[f]);
            //    }

            //    hs[i] = hdsd;
            //}

            //for (int i = 0; i < 4; i++)
            //{
            //    var res = GetBiK(a[i], avg[0], 15, 3, 4);
            //    list.Add(res);
            //}

            //var b0 = GetB0(list.ToArray(), avg[0]);


            //var h = GetH(b0, list.ToArray(), objects[0].Objects[0]);



            var sd = "";

        }

        private static void Test(List<IObjectTest> testObjects, List<IObjectTrain> objects)
        {
            var count = objects.Count;

            for (int i = 0; i < count; i++)
            {
                var obj = testObjects[i].Objects;


                for (int j = 0; j < obj.GetLength(0); j++)
                {
                    var h = new List<double>();

                    foreach (var hs in objects)
                    {
                        h.Add(GetH(hs.H, obj[j]));
                    }

                    var max = h.Max();

                    if (h.ToList().IndexOf(max) == i)
                    {
                        testObjects[i].SuccessfulCount++;
                    }
                    else
                    {
                        testObjects[i].ErrorCount++;
                    }

                }

            }

        }

        static void Train(List<IObjectTrain> train)
        {
            var avg = GetAvg(train);
            int ij = avg[0].Length;

            var w = GetW(ij, train, avg);
            var a = Matrix.Inverse(w);

            var p = train[0].Objects[0].Length;
            var n = train.Sum(s => s.Objects.GetLength(0));
            var g = train.Count;

            var hs = new double[g][];


            for (int i = 0; i < g; i++)
            {
                var bks = new List<double>();

                for (int j = 0; j < p; j++)
                {
                    bks.Add(GetBiK(a[j], avg[i], n, g, p));
                }

                var b0 = GetB0(bks.ToArray(), avg[i]);

                var hdsd = new double[p];

                for (int f = 0; f < p; f++)
                {
                    hdsd[f] = GetH(b0, bks.ToArray(), train[i].Objects[f]);
                }

                var resLs = new List<double>();
                resLs.Add(b0);
                resLs.AddRange(bks);

                train[i].H = resLs.ToArray();
            }

        }

        static double GetH(double b0, double[] bk, double[] b)
        {
            var res = 0d;

            for (int i = 0; i < bk.Length; i++)
            {
                res += bk[i] * b[i];
            }

            return b0 + res;
        }

        static double GetH(double[] H, double[] x)
        {
            var res = H[0];

            for (int i = 0; i < x.Length; i++)
            {
                res += H[i + 1] * x[i];
            }

            return res;
        }

        static double GetB0(double[] bk, double[] X)
        {
            var res = 0d;

            for (int i = 0; i < bk.Length; i++)
            {
                res += bk[i] * X[i];
            }


            return -0.5 * res;
        }

        static double GetBiK(double[] a, double[] X, int n, int g, int p)
        {
            var sum = 0d;


            for (int i = 0; i < p; i++)
            {
                sum += a[i] * X[i];
            }

            return (n - g) * sum;
        }

        static List<T> GetObjects<T>(string folderPath, int[] nums) where T : class
        {
            var objects = new List<T>();

            foreach (var file in Directory.GetFiles(folderPath))
            {
                using (var str = new StreamReader(file, Encoding.UTF8))
                {
                    var tmpObj = str.ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                    objects.Add((T)(object)new ObjectClass(nums, tmpObj, $"{Path.GetFileNameWithoutExtension(file)}"));
                }
            }

            //ЗАМЕНИТЬ НА ПАПКУ С ФАЙЛАМИ!!!
            //using (var str = new StreamReader(folderPath, Encoding.UTF8))
            //{
            //    var tmpObj = str.ReadToEnd().Split("\n", StringSplitOptions.RemoveEmptyEntries);

            //    int name = 1;

            //    foreach (var obj in tmpObj)
            //    {
            //        objects.Add((T)(object)new ObjectClass(nums, obj.Split("|", StringSplitOptions.RemoveEmptyEntries), $"{name}"));
            //        name++;
            //    }
            //}

            return objects;
        }

        static double[][] GetAvg(List<IObjectTrain> objects)
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

        static double[][] GetW(int n, List<IObjectTrain> objects, double[][] avg)
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