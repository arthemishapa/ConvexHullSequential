namespace ConvexHullSequentialAndParallel
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = "1TestInput.txt";
            //fileName = Console.ReadLine();
            ConvexHull convexHull = new ConvexHull(fileName);
            convexHull.ComputeSequentialConvexHull();
        }
    }
}
