#region

using System;
using System.Drawing;

#endregion

namespace PathCalculator
{
    public struct MyPoint
    {
        public float X { get; set; }
        public float Y { get; set; }

        public MyPoint(double x, double y)
        {
            X = (float) x;
            Y = (float) y;
        }

        public MyPoint Rotate(double radians, MyPoint center)
        {
            var s = Math.Sin(radians);
            var c = Math.Cos(radians);

            var x = X - center.X;
            var y = Y - center.Y;
            return new MyPoint(x*c - y*s + center.X, x*s + y*c + center.Y);
        }

        public static implicit operator PointF(MyPoint p)
        {
            return new PointF((float) p.X, (float) p.Y);
        }
    }
}