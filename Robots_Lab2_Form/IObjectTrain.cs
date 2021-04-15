using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robots_Lab2
{
    public interface IObjectTrain
    {
        double[][] Objects { get; }

        string Name { get; }

        double[] H { get; set; }
    }
}
