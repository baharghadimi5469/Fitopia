#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace PathCalculator
{
    public struct MyPath
    {
        public List<MyPoint> Points { get; }


        public MyPath(IEnumerable<MyPoint> points) : this()
        {
            const double epsilon = 0.1;

            Points = new List<MyPoint>(points);
            for (int i = 0; i < Points.Count-1; i++)
            {
                if (Math.Abs(Points[i].X - Points[i + 1].X) < epsilon &&
                    Math.Abs(Points[i].Y - Points[i + 1].Y) < epsilon)
                {
                    Points.RemoveAt(i + 1);
                    i--;
                }
            }
        }


        public MyPath GetParallelPath(double distance)
        {
            if (Points.Count < 2)
                return new MyPath(new MyPoint[0]);
            var pointsList = new List<MyPoint>();

            var linesList = new List<MyLine>();
            for (var i = 0; i < Points.Count - 1; i++)
            {

                linesList.Add(new MyLine(Points[i], Points[i + 1]));
            }

            //            foreach (var ln in linesList)
            //            {
            //                var myLine = ln.GetParallelLine(distance);
            //                if (double.IsInfinity(myLine.B) ||
            //                    double.IsInfinity(myLine.M) ||
            //                    double.IsNaN(myLine.B) ||
            //                    double.IsNaN(myLine.M) ||
            //                    double.IsNaN(2000 * myLine.M + myLine.B))
            //                    continue;
            //                pointsList.Add(new MyPoint(0, myLine.B));
            //                pointsList.Add(new MyPoint(2000, 2000 * myLine.M + myLine.B));
            //            }
            //            return new MyPath(pointsList);

            // first point 
            var teta = Math.Atan2(Points[1].Y - Points[0].Y, Points[1].X - Points[0].X);
            var firstPoint = new MyPoint(Points[0].X - Math.Sin(teta) * distance,
                Points[0].Y + Math.Cos(teta) * distance);
            pointsList.Add(firstPoint);
            //////////////
            for (var i = 0; i < linesList.Count - 1; i++)
            {
                var l1 = linesList[i].GetParallelLine(distance);
                var l2 = linesList[i + 1].GetParallelLine(distance);

                var point = l1.GetCrosPosition(l2);
                pointsList.Add(point);
            }

            // last point 
            var plast = Points[Points.Count - 1];
            var plast1 = Points[Points.Count - 2];
            teta = Math.Atan2(plast.Y - plast1.Y, plast.X - plast1.X);
            var lastPoint = new MyPoint(plast.X - Math.Sin(teta) * distance,
                plast.Y + Math.Cos(teta) * distance);
            pointsList.Add(lastPoint);
            //////////////


            return new MyPath(pointsList);
        }
    }
}