using System;
using System.Globalization;

namespace Robots_Lab2
{
    public class ObjectClass : IObjectTest, IObjectTrain
    {
        private double[][] _objects;
        public double[][] Objects => _objects;
        
        private string _name;
        public string Name => _name;

        public int SuccessfulCount { get; set; }
        public int ErrorCount { get; set; }
        public double[] H { get; set; }

        public ObjectClass(int[] numObj, string[] objects, string name)
        {
            _objects = Init(numObj, objects);
            _name = name;
        }

        private double[][] Init(int[] numObj,string[] mas)
        {
            var tmp = new double[mas.Length][];

            for (int i = 0; i < mas.Length; i++)
            {
                var tmpStr = mas[i].Split(";", StringSplitOptions.RemoveEmptyEntries);
                var intOb = new double[numObj.Length];

                for (int j = 0; j < numObj.Length; j++)
                {
                    var str = tmpStr[numObj[j] - 1];

                    intOb[j] = double.Parse(str.Replace(".",","), NumberStyles.Any);
                }
                tmp[i] = intOb;
            }

            return tmp;
        }
    }
}