using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robots_Lab2
{
    public interface IObjectTest
    {
        double[][] Objects { get; }
        string Name { get; }
        int SuccessfulCount { get; set; }
        int ErrorCount { get; set; }
    }
}
