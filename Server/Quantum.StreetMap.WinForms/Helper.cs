using System;
using GMap.NET;

namespace Quantum.StreetMap.WinForms
{
    public static class Helper
    {
        public enum GeoCodeCalcMeasurement
        {
            Miles = 0,
            Kilometers = 1
        }

        public const double EarthRadiusInMiles = 3956.0;
        public const double EarthRadiusInKilometers = 6367.0;

        public static double Fraction(double n1)
        {
            return n1 - (int)n1;
        }

        public static double ToRadian(double val)
        {
            return val * (Math.PI / 180);
        }

        public static double DiffRadian(double val1, double val2)
        {
            return ToRadian(val2) - ToRadian(val1);
        }

        public static double CalcDistance(double lat1, double lng1, double lat2, double lng2)
        {
            return CalcDistance(lat1, lng1, lat2, lng2, GeoCodeCalcMeasurement.Miles);
        }

        public static double CalcDistance(double lat1, double lng1, double lat2, double lng2, GeoCodeCalcMeasurement m)
        {
            var radius = EarthRadiusInMiles;

            if (m == GeoCodeCalcMeasurement.Kilometers)
            {
                radius = EarthRadiusInKilometers;
            }
            return radius * 2 *
                   Math.Asin(Math.Min(1,
                       Math.Sqrt(Math.Pow(Math.Sin(DiffRadian(lat1, lat2) / 2.0), 2.0) +
                                 Math.Cos(ToRadian(lat1)) * Math.Cos(ToRadian(lat2)) *
                                 Math.Pow(Math.Sin(DiffRadian(lng1, lng2) / 2.0), 2.0))));
        }

        public static double Distance(this GPoint p1, GPoint p2)
        {
            return Math.Sqrt(Math.Pow(p2.Y - p1.Y, 2) + Math.Pow(p2.X - p1.X, 2));
        }
    }
}