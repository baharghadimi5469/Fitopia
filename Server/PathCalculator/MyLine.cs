#region

using System;

#endregion

namespace PathCalculator
{
    public struct MyLine
    {
        public double B { get; }
        public double M { get; }

        private readonly double _const;
        private readonly double _corrector;

        public MyLine(MyPoint p1, MyPoint p2)
        {
            _const = 0.0;
            _corrector = 1;
            M = (p2.Y - p1.Y)/(p2.X - p1.X);
            B = p1.Y - M*p1.X;

            if (double.IsInfinity(M))
                _const = p2.X;
            if (Math.Abs(M) < double.Epsilon)
                _const = p2.Y;

            if (p2.X < p1.X)
                _corrector = -1;
        }

        public MyLine(double b, double m, double xy = 0.0)
        {
            _const = xy;
            _corrector = 1;
            B = b;
            M = m;
        }

        public MyLine GetParallelLine(double distance)
        {
            distance *= _corrector;
            var b = distance*Math.Sqrt(M*M + 1) + B;
            var m = M;
            if (double.IsInfinity(m))
                return new MyLine(b, m, _const);
            return new MyLine(b, m);
        }

        public MyPoint GetCrosPosition(MyLine l)
        {
            var x = (l.B - B)/(M - l.M);
            var y = (M*l.B - l.M*B)/(M - l.M);
            if (double.IsInfinity(M))
            {
                x = _const;
                y = l.M*x + l.B;
            }
            if (double.IsInfinity(l.M))
            {
                x = l._const;
                y = M*x + B;
            }


            return new MyPoint(x, y);
        }
    }
}