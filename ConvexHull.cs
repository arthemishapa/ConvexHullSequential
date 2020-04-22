using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ConvexHullSequentialAndParallel
{
    public class ConvexHull
    {
        private const string _projectPath = @"D:\\Faculty\\Semester 2\\PTPP\\ConvexHullSequential\\";
        public List<Point> Points { get; set; } = new List<Point>();
        private string FileName { get; set; }

        public ConvexHull(string fileName)
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

        public void ComputeSequentialConvexHull()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            Points = Points.OrderBy(point => point.X).ToList();
            Hull convexHull = SolveHull(Points);
            timer.Stop();
            string filePath = _projectPath + "1ActualOutput.txt";
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (Point p in convexHull.Points)
                    writer.WriteLine(p.X + " " + p.Y);
            }
            Debug.WriteLine("First test finished in: " + timer.Elapsed);

            FileName = "2TestInput.txt";
            ReadFromFile();
            timer = new Stopwatch();
            timer.Start();
            Points = Points.OrderBy(point => point.X).ToList();
            convexHull = SolveHull(Points);
            timer.Stop();
            filePath = _projectPath + "2ActualOutput.txt";
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (Point p in convexHull.Points)
                    writer.WriteLine(p.X + " " + p.Y);
            }
            Debug.WriteLine("Second test finished in: " + timer.Elapsed);

            FileName = "3TestInput.txt";
            ReadFromFile();
            timer = new Stopwatch();
            timer.Start();
            Points = Points.OrderBy(point => point.X).ToList();
            convexHull = SolveHull(Points);
            timer.Stop();
            filePath = _projectPath + "3ActualOutput.txt";
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (Point p in convexHull.Points)
                    writer.WriteLine(p.X + " " + p.Y);
            }
            Debug.WriteLine("Third test finished in: " + timer.Elapsed);

            FileName = "4TestInput.txt";
            ReadFromFile();
            timer = new Stopwatch();
            timer.Start();
            Points = Points.OrderBy(point => point.X).ToList();
            convexHull = SolveHull(Points);
            timer.Stop();
            filePath = _projectPath + "4ActualOutput.txt";
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (Point p in convexHull.Points)
                    writer.WriteLine(p.X + " " + p.Y);
            }
            Debug.WriteLine("Fourth test finished in: " + timer.Elapsed);
        }

        private Hull SolveHull(List<Point> points)
        {
            if (points.Count < 6)
            {
                Hull result = BruteHull(points);
                return result;
            }
            else
            {
                List<List<Point>> sets = DivideHull(points);

                Hull leftHull = SolveHull(sets[0]);
                Hull rightHull = SolveHull(sets[1]);

                return MergeHull(leftHull, rightHull);
            }
        }

        private Hull BruteHull(List<Point> hull)
        {
            if (hull.Count() <= 3)
                return new Hull(hull.Distinct().OrderBy(x => x.X).ToList()); 
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
                return new Hull(hull.Distinct().OrderBy(x => x.X).ToList()); ;
            return new Hull(result.Distinct().OrderBy(x => x.X).ToList());
        }

        private Hull MergeHull(Hull leftHull, Hull rightHull)
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
            // Line AB represented as a1x + b1y + c1 = 0 
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

    public class Hull
    {
        public List<Point> Points { get; private set; } = new List<Point>();

        public Hull(List<Point> hullPoints)
        {
            Points = hullPoints;
        }

        public Hull() { }
    }

    public struct Point
    {
        public double X;
        public double Y;
    }
}