using System;

namespace Robots_Lab2
{
    public class ObjectClass
    {
        private double[][] _objects;
        public double[][] Objects => _objects;
        
        private string _name;
        public string Name => _name;

        public ObjectClass(string[] objects, string name)
        {
            _objects = Init(objects);
            _name = name;
        }

        private double[][] Init(string[] mas)
        {
            var tmp = new double[mas.Length][];

            for (int i = 0; i < mas.Length; i++)
            {
                var tmpStr = mas[i].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var intOb = new double[tmpStr.Length];

                for (int j = 0; j < tmpStr.Length; j++)
                {
                    intOb[j] = (double)int.Parse(tmpStr[j]);
                }
                tmp[i] = intOb;
            }

            return tmp;
        }
    }
}