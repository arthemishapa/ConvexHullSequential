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
        public List<Tuple<int, int>> Points { get; set; } = new List<Tuple<int, int>>();
        private List<Tuple<int, int>> MinMaxPoints { get; set; }
        private float MinMaxSlope { get; set; }
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
                Tuple<int, int> point = new Tuple<int, int>(x, y);
                Points.Add(point);
            }

            file.Close();
        }

        public void ComputeSequentialConvexHull()
        {
            Points = Points.OrderBy(x => x.Item1).ToList();
            MinMaxPoints = new List<Tuple<int, int>>();
            MinMaxPoints.Add(Points[0]);
            Min
            MinMaxSlope = (Points[Points.Count - 1].Item2 - Points[0].Item2) / (Points[Points.Count - 1].Item1 - Points[0].Item1);
        }

        private int GetPointPositionInRelationToLine(Tuple<int, int> point)
        {
            int d = (point.Item1 - MinMaxPoints[0].Item1) * (MinMaxPoints[1].Item2 - MinMaxPoints[0].Item2) - (point.Item2 - MinMaxPoints[0].Item2) * (MinMaxPoints[1].Item1 - MinMaxPoints[0].Item1);

            if (d > 0) return 1; //right
            else if (d == 0) return 0; //online
            else return -1; // left
        }

        private int GetDistanceBetweenPointAndLine(Tuple<int, int> pointA, Tuple<int, int> pointB, Tuple<int, int> point)
        {
            // ax+by+c=0
            int a = pointA.Item2 - pointB.Item2;
            int b = pointB.Item1 - pointA.Item1;
            int c = pointA.Item1 * pointB.Item2 - pointB.Item1 * pointA.Item2;

            return()
        }
    }
}
