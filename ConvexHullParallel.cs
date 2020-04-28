using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConvexHullSequentialAndParallel
{
    public class ConvexHullParallel
    {
        private const string _projectPath = @"D:\\Faculty\\Semester 2\\PTPP\\ConvexHullSequential\\";
        public List<Point> Points { get; set; } = new List<Point>();
        //public List<Hull> results { get; set; } = new List
        private string FileName { get; set; }

        public ConvexHullParallel(string fileName)
        {
            FileName = fileName;
            ReadFromFile();
        }

        private void ReadFromFile()
        {
            if(Points.Count() > 0)
                Points = new List<Point>();
            string line;

            string filePath = _projectPath + FileName;

            System.IO.StreamReader file =
                new System.IO.StreamReader(filePath);
            while ((line = file.ReadLine()) != null)
            {
                string[] coordinates = line.Split(' ');
                double.TryParse(coordinates[0], out double x);
                double.TryParse(coordinates[1], out double y);
                Point point = new Point();
                point.X = x;
                point.Y = y;
                Points.Add(point);
            }

            file.Close();
        }

        public void ComputeParallelConvexHull()
        {
            #region non-scalable version
            //Stopwatch timer = new Stopwatch();

            //Points = Points.OrderBy(point => point.X).ToList();
            //List<List<Point>> sets = DivideHull(Points);

            //Hull rightHalf = new Hull(); 
            //Hull leftHalf = new Hull();
            //Thread t1 = new Thread(() => leftHalf = SolveHull(sets[0]));
            //Thread t2 = new Thread(() => rightHalf = SolveHull(sets[1]));
            //timer.Start();
            //t1.Start();
            //t2.Start();

            //t1.Join();
            //t2.Join();

            //timer.Stop();
            //Hull finalResult = MergeHull(leftHalf, rightHalf);

            //string filePath = _projectPath + "1ActualParallelOutput.txt";
            //using (StreamWriter writer = new StreamWriter(filePath))
            //{
            //    foreach (Point p in finalResult.Points)
            //        writer.WriteLine(p.X + " " + p.Y);
            //}
            //Debug.WriteLine("First parallel test finished in: " + timer.Elapsed);

            //FileName = "2TestInput.txt";
            //ReadFromFile();
            //timer = new Stopwatch();

            //Thread t3 = new Thread(() => leftHalf = SolveHull(sets[0]));
            //Thread t4 = new Thread(() => rightHalf = SolveHull(sets[1]));
            //timer.Start();
            //t3.Start();
            //t4.Start();

            //t3.Join();
            //t4.Join();

            //timer.Stop();
            //finalResult = MergeHull(leftHalf, rightHalf);

            //filePath = _projectPath + "2ActualParallelOutput.txt";
            //using (StreamWriter writer = new StreamWriter(filePath))
            //{
            //    foreach (Point p in finalResult.Points)
            //        writer.WriteLine(p.X + " " + p.Y);
            //}
            //Debug.WriteLine("Second parallel test finished in: " + timer.Elapsed);

            //FileName = "3TestInput.txt";
            //ReadFromFile();
            //timer = new Stopwatch();

            //Thread t5 = new Thread(() => leftHalf = SolveHull(sets[0]));
            //Thread t6 = new Thread(() => rightHalf = SolveHull(sets[1]));
            //timer.Start();
            //t5.Start();
            //t6.Start();

            //t5.Join();
            //t6.Join();

            //timer.Stop();
            //finalResult = MergeHull(leftHalf, rightHalf);

            //filePath = _projectPath + "3ActualParallelOutput.txt";
            //using (StreamWriter writer = new StreamWriter(filePath))
            //{
            //    foreach (Point p in finalResult.Points)
            //        writer.WriteLine(p.X + " " + p.Y);
            //}
            //Debug.WriteLine("Third parallel test finished in: " + timer.Elapsed);

            //FileName = "4TestInput.txt";
            //ReadFromFile();
            //timer = new Stopwatch();

            //Thread t7 = new Thread(() => leftHalf = SolveHull(sets[0]));
            //Thread t8 = new Thread(() => rightHalf = SolveHull(sets[1]));
            //timer.Start();
            //t7.Start();
            //t8.Start();

            //t7.Join();
            //t8.Join();

            //timer.Stop();
            //finalResult = MergeHull(leftHalf, rightHalf);

            //filePath = _projectPath + "4ActualParallelOutput.txt";
            //using (StreamWriter writer = new StreamWriter(filePath))
            //{
            //    foreach (Point p in finalResult.Points)
            //        writer.WriteLine(p.X + " " + p.Y);
            //}
            //Debug.WriteLine("Fourth parallel test finished in: " + timer.Elapsed);
            #endregion non-scalable version

            #region scalable version

            #region Test 1
            FileName = "1TestInput.txt";
            ReadFromFile();

            Stopwatch timer = new Stopwatch();
            string filePath = _projectPath + "1ActualParallelOutput.txt";

            double powerOfTwo = Math.Log(Points.Count(), 2);
            int.TryParse(Math.Truncate(powerOfTwo).ToString(), out int decimalValue);
            int.TryParse(Math.Truncate(Math.Pow(2, decimalValue)).ToString(), out int numberOfThreads);
            Points = Points.OrderBy(point => point.X).ToList();
            List<List<Point>> pointsForThreads = new List<List<Point>>();
            pointsForThreads.Add(Points);
            while (pointsForThreads.Count() < 1)
            {
                List<List<Point>> newPointsForThreads = new List<List<Point>>();
                foreach (List<Point> points in pointsForThreads)
                {
                    List<List<Point>> sets = DivideHull(points);
                    newPointsForThreads.Add(sets[0]);
                    newPointsForThreads.Add(sets[1]);
                }
                pointsForThreads = new List<List<Point>>();
                pointsForThreads.AddRange(newPointsForThreads);
            }

            Thread[] threads = new Thread[pointsForThreads.Count()];
            List<Hull> results = new List<Hull>(pointsForThreads.Count());
            for (int i = 0; i < pointsForThreads.Count(); i++)
            {
                int index = i;
                threads[i] = new Thread(() => results.Add(SolveHull(pointsForThreads[index], index)));
            }

            timer.Start();

            for (int i = 0; i < pointsForThreads.Count(); i++)
            {
                threads[i].Start();
            }

            for (int i = 0; i < pointsForThreads.Count(); i++)
            {
                threads[i].Join();
            }

            results = results.OrderBy(x => x.ThreadNr).ToList();
            Hull finalResult = results[0];
            for (int i = 1; i < pointsForThreads.Count(); i++)
            {
                finalResult = MergeHull(finalResult, results[i], i);
            }

            timer.Stop();

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (Point p in finalResult.Points)
                    writer.WriteLine(p.X + " " + p.Y);
            }
            Debug.WriteLine("First parallel test finished in: " + timer.Elapsed);
            #endregion Test 1

            #region Test 2
            FileName = "2TestInput.txt";
            ReadFromFile();

            timer = new Stopwatch();
            filePath = _projectPath + "2ActualParallelOutput.txt";

            powerOfTwo = Math.Log(Points.Count(), 2);
            int.TryParse(Math.Truncate(powerOfTwo).ToString(), out decimalValue);
            int.TryParse(Math.Truncate(Math.Pow(2, decimalValue)).ToString(), out numberOfThreads);
            Points = Points.OrderBy(point => point.X).ToList();
            pointsForThreads = new List<List<Point>>();
            pointsForThreads.Add(Points);
            while (pointsForThreads.Count() < 1)
            {
                List<List<Point>> newPointsForThreads = new List<List<Point>>();
                foreach (List<Point> points in pointsForThreads)
                {
                    List<List<Point>> sets = DivideHull(points);
                    newPointsForThreads.Add(sets[0]);
                    newPointsForThreads.Add(sets[1]);
                }
                pointsForThreads = new List<List<Point>>();
                pointsForThreads.AddRange(newPointsForThreads);
            }

            threads = new Thread[pointsForThreads.Count()];
            results = new List<Hull>(pointsForThreads.Count());
            for (int i = 0; i < pointsForThreads.Count(); i++)
            {
                int index = i;
                threads[i] = new Thread(() => results.Add(SolveHull(pointsForThreads[index], index)));
            }

            timer.Start();

            for (int i = 0; i < pointsForThreads.Count(); i++)
            {
                threads[i].Start();
            }

            for (int i = 0; i < pointsForThreads.Count(); i++)
            {
                threads[i].Join();
            }

            results = results.OrderBy(x => x.ThreadNr).ToList();
            finalResult = results[0];
            for (int i = 1; i < pointsForThreads.Count(); i++)
            {
                finalResult = MergeHull(finalResult, results[i], i);
            }

            timer.Stop();

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (Point p in finalResult.Points)
                    writer.WriteLine(p.X + " " + p.Y);
            }
            Debug.WriteLine("Second parallel test finished in: " + timer.Elapsed);
            #endregion Test 2

            #region Test 3
            FileName = "3TestInput.txt";
            ReadFromFile();

            timer = new Stopwatch();
            filePath = _projectPath + "3ActualParallelOutput.txt";

            powerOfTwo = Math.Log(Points.Count(), 2);
            int.TryParse(Math.Truncate(powerOfTwo).ToString(), out decimalValue);
            int.TryParse(Math.Truncate(Math.Pow(2, decimalValue)).ToString(), out numberOfThreads);
            Points = Points.OrderBy(point => point.X).ToList();
            pointsForThreads = new List<List<Point>>();
            pointsForThreads.Add(Points);
            while (pointsForThreads.Count() < decimalValue / 8)
            {
                List<List<Point>> newPointsForThreads = new List<List<Point>>();
                foreach (List<Point> points in pointsForThreads)
                {
                    List<List<Point>> sets = DivideHull(points);
                    newPointsForThreads.Add(sets[0]);
                    newPointsForThreads.Add(sets[1]);
                }
                pointsForThreads = new List<List<Point>>();
                pointsForThreads.AddRange(newPointsForThreads);
            }

            threads = new Thread[pointsForThreads.Count()];
            results = new List<Hull>(pointsForThreads.Count());
            for (int i = 0; i < pointsForThreads.Count(); i++)
            {
                int index = i;
                threads[i] = new Thread(() => results.Add(SolveHull(pointsForThreads[index], index)));
            }

            timer.Start();

            for (int i = 0; i < pointsForThreads.Count(); i++)
            {
                threads[i].Start();
            }

            for (int i = 0; i < pointsForThreads.Count(); i++)
            {
                threads[i].Join();
            }

            results = results.OrderBy(x => x.ThreadNr).ToList();
            finalResult = results[0];
            for (int i = 1; i < pointsForThreads.Count(); i++)
            {
                finalResult = MergeHull(finalResult, results[i], i);
            }

            timer.Stop();

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (Point p in finalResult.Points)
                    writer.WriteLine(p.X + " " + p.Y);
            }
            Debug.WriteLine("Third parallel test finished in: " + timer.Elapsed);
            #endregion Test 3

            #region Test 4
            FileName = "4TestInput.txt";
            ReadFromFile();

            timer = new Stopwatch();
            filePath = _projectPath + "4ActualParallelOutput.txt";

            powerOfTwo = Math.Log(Points.Count(), 2);
            int.TryParse(Math.Truncate(powerOfTwo).ToString(), out decimalValue);
            int.TryParse(Math.Truncate(Math.Pow(2, decimalValue)).ToString(), out numberOfThreads);
            Points = Points.OrderBy(point => point.X).ToList();
            pointsForThreads = new List<List<Point>>();
            pointsForThreads.Add(Points);
            while (pointsForThreads.Count() < decimalValue / 4)
            {
                List<List<Point>> newPointsForThreads = new List<List<Point>>();
                foreach (List<Point> points in pointsForThreads)
                {
                    List<List<Point>> sets = DivideHull(points);
                    newPointsForThreads.Add(sets[0]);
                    newPointsForThreads.Add(sets[1]);
                }
                pointsForThreads = new List<List<Point>>();
                pointsForThreads.AddRange(newPointsForThreads);
            }

            threads = new Thread[pointsForThreads.Count()];
            results = new List<Hull>(pointsForThreads.Count());
            for (int i = 0; i < pointsForThreads.Count(); i++)
            {
                int index = i;
                threads[i] = new Thread(() => results.Add(SolveHull(pointsForThreads[index], index)));
            }

            timer.Start();

            for (int i = 0; i < pointsForThreads.Count(); i++)
            {
                threads[i].Start();
            }

            for (int i = 0; i < pointsForThreads.Count(); i++)
            {
                threads[i].Join();
            }

            results = results.OrderBy(x => x.ThreadNr).ToList();
            finalResult = results[0];
            for (int i = 1; i < pointsForThreads.Count(); i++)
            {
                finalResult = MergeHull(finalResult, results[i], i);
            }

            timer.Stop();

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (Point p in finalResult.Points)
                    writer.WriteLine(p.X + " " + p.Y);
            }
            Debug.WriteLine("Fourth parallel test finished in: " + timer.Elapsed);
            #endregion Test 4

            #endregion scalable version
        }

        private Hull SolveHull(List<Point> points, int threadNr)
        {
            if (points.Count < 6)
            {
                Hull result = BruteHull(points, threadNr);
                return result;
            }
            else
            {
                List<List<Point>> sets = DivideHull(points);

                Hull leftHull = SolveHull(sets[0], threadNr);
                Hull rightHull = SolveHull(sets[1], threadNr);

                return MergeHull(leftHull, rightHull, threadNr);
            }
        }

        private Hull BruteHull(List<Point> hull, int threadNr)
        {
            if (hull.Count() <= 3)
                return new Hull(hull.Distinct().OrderBy(x => x.X).ToList(), threadNr); 
            List<Point> result = new List<Point>();

            for (int i = 0; i < hull.Count(); i++)
            {
                for (int j = i + 1; j < hull.Count(); j++)
                {
                    int pos = 0, neg = 0;
                    for (int k = 0; k < hull.Count(); k++)
                    {
                        int sign = GetSide(hull[i], hull[j], hull[k]);
                        if (sign <= 0)
                            neg++;
                        if (sign >= 0)
                            pos++;
                    }
                    if (pos == hull.Count() || neg == hull.Count())
                    {
                        result.Add(hull[i]);
                        result.Add(hull[j]);
                    }
                }
            }
            if(result.Count() == 0)
                return new Hull(hull.Distinct().OrderBy(x => x.X).ToList(), threadNr);
            return new Hull(result.Distinct().OrderBy(x => x.X).ToList(), threadNr);
        }

        private Hull MergeHull(Hull leftHull, Hull rightHull, int threadNr)
        {
            Hull mergedHull = new Hull();

            int rightMost = leftHull.Points.Count - 1;
            int leftMost = 0;

            int currentLeftIndex = rightMost;
            int currentRightIndex = leftMost;

            int upperLeft = -1;
            int upperRight = -1;
            int lowerLeft = -1;
            int lowerRight = -1;

            bool leftIndexChanged = false;
            bool rightIndexChanged = false;

            bool finishedUpperTangent = false;
            while (!finishedUpperTangent)
            {
                while (!isUpperTangentForPolygon(leftHull.Points[currentLeftIndex], rightHull.Points[currentRightIndex], rightHull.Points.Where(point => point.X != rightHull.Points[currentRightIndex].X && point.Y != rightHull.Points[currentRightIndex].Y).ToList()))
                {
                    rightIndexChanged = true;
                    int nextIndex = currentRightIndex == rightHull.Points.Count() - 1 ? 0 : currentRightIndex + 1;
                    currentRightIndex = nextIndex;
                }

                while (!isUpperTangentForPolygon(leftHull.Points[currentLeftIndex], rightHull.Points[currentRightIndex], leftHull.Points.Where(point => point.X != leftHull.Points[currentLeftIndex].X && point.Y != leftHull.Points[currentLeftIndex].Y).ToList()))
                {
                    leftIndexChanged = true;
                    int prevIndex = currentLeftIndex == 0 ? leftHull.Points.Count() - 1 : currentLeftIndex - 1;
                    currentLeftIndex = prevIndex;
                }

                if (!leftIndexChanged && !rightIndexChanged)
                {
                    upperLeft = currentLeftIndex;
                    upperRight = currentRightIndex;
                    finishedUpperTangent = true;
                }

                leftIndexChanged = false;
                rightIndexChanged = false;
            }

            currentLeftIndex = rightMost;
            currentRightIndex = leftMost;

            leftIndexChanged = false;
            rightIndexChanged = false;

            bool finishedLowerTangent = false;
            while (!finishedLowerTangent)
            {
                while (!isLowerTangentForPolygon(leftHull.Points[currentLeftIndex], rightHull.Points[currentRightIndex], rightHull.Points.Where(point => point.X != rightHull.Points[currentRightIndex].X && point.Y != rightHull.Points[currentRightIndex].Y).ToList()))
                {
                    rightIndexChanged = true;
                    int nextIndex = currentRightIndex == rightHull.Points.Count() - 1 ? 0 : currentRightIndex + 1;
                    currentRightIndex = nextIndex;
                }

                while (!isLowerTangentForPolygon(leftHull.Points[currentLeftIndex], rightHull.Points[currentRightIndex], leftHull.Points.Where(point => point.X != leftHull.Points[currentLeftIndex].X && point.Y != leftHull.Points[currentLeftIndex].Y).ToList()))
                {
                    leftIndexChanged = true;
                    int prevIndex = currentLeftIndex == 0 ? leftHull.Points.Count() - 1 : currentLeftIndex - 1;
                    currentLeftIndex = prevIndex;
                }

                if (!leftIndexChanged && !rightIndexChanged)
                {
                    lowerLeft = currentLeftIndex;
                    lowerRight = currentRightIndex;
                    finishedLowerTangent = true;
                }

                leftIndexChanged = false;
                rightIndexChanged = false;
            }

            //add up to (and including) upperLeft
            List<Point> result = new List<Point>();
            for (int i = 0; i <= upperLeft; i++)
            {
                result.Add(leftHull.Points[i]);
            }

            for (int i = upperRight; i != lowerRight; i++)
            {
                if (i == rightHull.Points.Count())
                    break;
                result.Add(rightHull.Points[i]);
            }
            //add lowerRight
            result.Add(rightHull.Points[lowerRight]);
            //add from lowerLeft to beginning
            for (int i = lowerLeft; i != 0; i++)
            {
                if (i == leftHull.Points.Count())
                    break;
                result.Add(leftHull.Points[i]);
            }
            mergedHull.Points.AddRange(result.Distinct().ToList());
            mergedHull.ThreadNr = threadNr;
            return mergedHull;
        }

        private static bool isUpperTangentForPolygon(Point A, Point B, List<Point> polygon)
        {
            if (polygon == null || !polygon.Any()) return false;

            foreach (Point p in polygon)
            {
                int sign = GetSide(A, B, p);
                if (sign == 0)
                    continue;
                if (sign == 1)
                    return false;
            }

            return true;
        }

        private static bool isLowerTangentForPolygon(Point A, Point B, List<Point> polygon)
        {
            if (polygon == null || !polygon.Any()) return false;

            foreach (Point p in polygon)
            {
                int sign = GetSide(A, B, p);
                if (sign == 0)
                    continue;
                if (sign != 1)
                    return false;
            }

            return true;
        }

        private static int GetSide(Point A, Point B, Point queryP)
        {
            double x1 = A.X, x2 = B.X;
            double y1 = A.Y, y2 = B.Y;

            double a1 = y1 - y2;
            double b1 = x2 - x1;
            double c1 = x1 * y2 - y1 * x2;
            return Math.Sign(a1 * queryP.X + b1 * queryP.Y + c1);
            //return Math.Sign((B.X - A.X) * (queryP.Y - A.Y) - (B.Y - A.Y) * (queryP.X - A.X));
        }

        private List<List<Point>> DivideHull(List<Point> points)
        {
            List<List<Point>> LeftAndRightHalves = new List<List<Point>>();
            LeftAndRightHalves.Add(points.Take(points.Count / 2).ToList()); //left
            LeftAndRightHalves.Add(points.Skip(points.Count / 2).ToList()); //right
            return LeftAndRightHalves;
        }

        public double calculateSlope(Point left, Point right)
        {
            return (right.Y - left.Y) / (right.X - left.X);
        }
    }
}
