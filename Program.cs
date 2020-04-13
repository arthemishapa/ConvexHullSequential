using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvexHullSequentialAndParallel
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName;
            fileName = Console.ReadLine();
            ConvexHull convexHull = new ConvexHull(fileName);
            convexHull.ComputeSequentialConvexHull();
        }
    }
}
