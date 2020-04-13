using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            string line;

            string filePath = _projectPath + FileName;

            System.IO.StreamReader file =
                new System.IO.StreamReader(filePath);
            while ((line = file.ReadLine()) != null)
            {
                string[] coordinates = line.Split(' ');
                int.TryParse(coordinates[0], out int x);
                int.TryParse(coordinates[1], out int y);
                Point point = new Point();
                point.X = x;
                point.Y = y;
                Points.Add(point);
            }

            file.Close();
        }

        public void ComputeSequentialConvexHull()
        {
            Points = Points.OrderBy(point => point.X).ToList();
            Hull convexHull = SolveHull(Points);
        }

        private Hull SolveHull(List<Point> points)
        {
            if (points.Count <= 1)
            {
                Hull result = new Hull(points);
                return result;
            }
            else
            {
                List<List<Point>> sets = divideHull(points);

                Hull leftHull = SolveHull(sets[0]);
                Hull rightHull = SolveHull(sets[1]);

                return mergeHull(leftHull, rightHull);
            }
        }

        private Hull mergeHull(Hull leftHull, Hull rightHull)
        {
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
            //iterate through at least once
            bool firstRight = true;
            bool firstLeft = true;

            //get upper common tangent
            while (leftIndexChanged || rightIndexChanged || firstLeft || firstRight)
            {
                if (firstRight || leftIndexChanged)
                {
                    firstRight = false;
                    upperRight = getRightUpper(leftHull, rightHull, currentLeftIndex, currentRightIndex);
                    if (upperRight == currentRightIndex)
                    {
                        leftIndexChanged = false;
                        rightIndexChanged = false;
                    }
                    else
                    {
                        rightIndexChanged = true;
                        currentRightIndex = upperRight;
                    }
                }
                if (firstLeft || rightIndexChanged)
                {
                    firstLeft = false;
                    upperLeft = getLeftUpper(leftHull, rightHull, currentLeftIndex, currentRightIndex);
                    if (upperLeft == currentLeftIndex)
                    {
                        leftIndexChanged = false;
                        rightIndexChanged = false;
                    }
                    else
                    {
                        leftIndexChanged = true;
                        currentLeftIndex = upperLeft;
                    }
                }
            }

            //get lower common tangentt
            currentLeftIndex = rightMost;
            currentRightIndex = leftMost;

            leftIndexChanged = false;
            rightIndexChanged = false;
            //iterate through at least once
            firstRight = true;
            firstLeft = true;
            while (leftIndexChanged || rightIndexChanged || firstLeft || firstRight)
            {
                if (firstLeft || rightIndexChanged)
                {
                    firstLeft = false;
                    lowerLeft = getLeftLower(leftHull, rightHull, currentLeftIndex, currentRightIndex);
                    if (lowerLeft == currentLeftIndex)
                    {
                        leftIndexChanged = false;
                        rightIndexChanged = false;
                    }
                    else
                    {
                        leftIndexChanged = true;
                        currentLeftIndex = lowerLeft;
                    }
                }

                if (firstRight || leftIndexChanged)
                {
                    firstRight = false;
                    lowerRight = getRightLower(leftHull, rightHull, currentLeftIndex, currentRightIndex);
                    if (lowerRight == currentRightIndex)
                    {
                        leftIndexChanged = false;
                        rightIndexChanged = false;
                    }
                    else
                    {
                        rightIndexChanged = true;
                        currentRightIndex = lowerRight;
                    }
                }
            }

            //join points
            List<Point> resultPoints = new List<Point>();
            //add up to (and including) upperLeft
            for (int i = 0; i <= upperLeft; i++)
            {
                resultPoints.Add(leftHull.Points[i]);
            }
            //add up to lowerRight
            for (int i = upperRight; i != lowerRight; i ++)
            {
                if (i == rightHull.Points.Count)
                    break;
                resultPoints.Add(rightHull.Points[i]);
            }
            //add lowerRight
            resultPoints.Add(rightHull.Points[lowerRight]);
            //add from lowerLeft to beginning
            for (int i = lowerLeft; i != 0; i ++)
            {
                if (i == leftHull.Points.Count)
                    break;
                resultPoints.Add(leftHull.Points[i]);
            }

            return new Hull(resultPoints);
        }

        private int getLeftUpper(Hull left, Hull right, int leftIndex, int rightIndex)
        { //O(n)
            List<Point> leftPoints = left.Points;
            List<Point> rightPoints = right.Points;

            int prev = leftIndex == 0 ? leftPoints.Count() - 1 : leftIndex - 1;

            while (calculateSlope(rightPoints[rightIndex], leftPoints[prev]) <
                  calculateSlope(rightPoints[rightIndex], leftPoints[leftIndex]))
            {
                leftIndex = prev;
                prev = prev == 0 ? leftPoints.Count() - 1 : prev - 1;
            }
            return leftIndex;
        }

        private int getRightUpper(Hull left, Hull right, int leftIndex, int rightIndex)
        { //O(n)
            List<Point> leftPoints = left.Points;
            List<Point> rightPoints = right.Points;

            int next = rightIndex == rightPoints.Count() - 1 ? 0 : rightIndex + 1;

            while (calculateSlope(leftPoints[leftIndex], rightPoints[next]) >
                  calculateSlope(leftPoints[leftIndex], rightPoints[rightIndex]))
            {
                rightIndex = next;
                next = next == rightPoints.Count() - 1 ? 0 : next + 1;
            }

            return rightIndex;
        }

        private int getLeftLower(Hull left, Hull right, int leftIndex, int rightIndex)
        { //O(n)
            List<Point> leftPoints = left.Points;
            List<Point> rightPoints = right.Points;

            int next = leftIndex == leftPoints.Count() - 1 ? 0 : leftIndex + 1;

            while (calculateSlope(rightPoints[rightIndex], leftPoints[next]) >
                  calculateSlope(rightPoints[rightIndex], leftPoints[leftIndex]))
            {
                leftIndex = next;
                next = next == leftPoints.Count() - 1 ? 0 : next + 1;
            }
            return leftIndex;
        }

        private int getRightLower(Hull left, Hull right, int leftIndex, int rightIndex)
        { //O(n)
            List<Point> leftPoints = left.Points;
            List<Point> rightPoints = right.Points;

            int prev = rightIndex == 0 ? rightPoints.Count() - 1 : rightIndex - 1;

            while (calculateSlope(leftPoints[leftIndex], rightPoints[prev]) <
                  calculateSlope(leftPoints[leftIndex], rightPoints[rightIndex]))
            {
                rightIndex = prev;
                prev = prev == 0 ? rightPoints.Count() - 1 : prev - 1;
            }
            return rightIndex;
        }

        private List<List<Point>> divideHull(List<Point> points)
        {
            List<List<Point>> LeftAndRightHalves = new List<List<Point>>();
            LeftAndRightHalves.Add(points.Take(points.Count / 2).ToList()); //left
            LeftAndRightHalves.Add(points.Skip(points.Count / 2).ToList()); //right
            return LeftAndRightHalves;
        }

        public Double calculateSlope(Point left, Point right)
        {
            return -(right.Y - left.Y) / (right.X - left.X);
        }

        public static double DistanceFromPointToLineAB(Tuple<int, int> pointA, Tuple<int, int> pointB, Tuple<int, int> point)
        {
            // given a line based on two points, and a point away from the line,
            // find the perpendicular distance from the point to the line.
            return Math.Abs((pointB.Item1 - pointA.Item1) * (pointA.Item2 - point.Item2) - (pointA.Item1 - point.Item1) * (pointB.Item2 - pointA.Item2)) /
                    Math.Sqrt(Math.Pow(pointB.Item1 - pointA.Item1, 2) + Math.Pow(pointB.Item2 - pointA.Item2, 2));
        }
    }

    public class Hull
    {
        public List<Point> Points { get; private set; } = new List<Point>();
        public Point RightMostPoint { get; private set; }
        public Point LeftMostPoint { get; private set; }

        public Hull(List<Point> hullPoints)
        {
            Points = hullPoints;
            RightMostPoint = Points[Points.Count - 1];
            LeftMostPoint = Points[0];
        }

        public Hull() { }
    }

    public struct Point
    {
        public int X;
        public int Y;
    }
}
