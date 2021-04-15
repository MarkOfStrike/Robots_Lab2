using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Robots_Lab2;

namespace Robots_Lab2_Form
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void PrintLineToConsole(string text = null)
        {
            if (text != null) RtbConsole.Text += text;
            RtbConsole.Text += Environment.NewLine;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var pathWin = "D:/Desktop/Data";
            var pathLin = "/home/aksios/Data";

            var trainPath = Path.Combine(pathWin, "Train");
            var testPath = Path.Combine(pathWin, "Test");

            var numObjects = new int[] { 2, 4, 6, 8, 10 };

            PrintLineToConsole("Номера всех признаков: " + string.Join(", ", numObjects));

            var objects = GetObjects<IObjectTrain>(trainPath, numObjects);
            Train(objects);

            PrintLineToConsole("Коэффициенты:");
            foreach (var obj in objects)
            {
                PrintLineToConsole($"H ({obj.Name}): {string.Join(", ", obj.H)})");
            }


            PrintLineToConsole("------");
            PrintLineToConsole("Начинаем обучение с помощью обучающей выборки и тестирование на самой же обучающей выборке...");
            LearnAndTest(trainPath, trainPath, numObjects, "Train");
            PrintLineToConsole("------");

            PrintLineToConsole();

            PrintLineToConsole("------");
            PrintLineToConsole("Начинаем обучение с помощью обучающей выборки и тестирование выборке для тестирования...");
            LearnAndTest(trainPath, testPath, numObjects, "Test");
            PrintLineToConsole("------");

        }

        void LearnAndTest(string trainPath, string testPath, int[] numObjects, string nameChart)
        {
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

                PrintLineToConsole($"С признаками: {string.Join(", ", nums)}. Количество успешно распознанных: {success}. Количество не распознанных: {error}. Успех: {luck}%");
                chart1.Series[nameChart].Points.AddXY(i + 1, luck);
            }

            PrintLineToConsole($"По итогу самый информативный признак: {numinfo}, а самый не информативный: {numNotinfo}.");
        }

        void Test(List<IObjectTest> testObjects, List<IObjectTrain> objects)
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

        void Train(List<IObjectTrain> train)
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

        double GetH(double b0, double[] bk, double[] b)
        {
            var res = 0d;

            for (int i = 0; i < bk.Length; i++)
            {
                res += bk[i] * b[i];
            }

            return b0 + res;
        }

        double GetH(double[] H, double[] x)
        {
            var res = H[0];

            for (int i = 0; i < x.Length; i++)
            {
                res += H[i + 1] * x[i];
            }

            return res;
        }

        double GetB0(double[] bk, double[] X)
        {
            var res = 0d;

            for (int i = 0; i < bk.Length; i++)
            {
                res += bk[i] * X[i];
            }


            return -0.5 * res;
        }

        double GetBiK(double[] a, double[] X, int n, int g, int p)
        {
            var sum = 0d;


            for (int i = 0; i < p; i++)
            {
                sum += a[i] * X[i];
            }

            return (n - g) * sum;
        }

        List<T> GetObjects<T>(string folderPath, int[] nums) where T : class
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

            return objects;
        }

        double[][] GetAvg(List<IObjectTrain> objects)
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

        double[][] GetW(int n, List<IObjectTrain> objects, double[][] avg)
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
