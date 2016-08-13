using System;
using System.Drawing;
using System.Runtime.Serialization;
using GMap.NET;

namespace Quantum.StreetMap.WinForms.Routes
{
    public class RoadSign : RouteElement
    {
        public decimal DistanceToExit { get; set; }


        public override void Render(Graphics g)
        {
            double m;
            var position = Route.FromPositionToPoint(PositionFromStart, out m);
            if (position == PointF.Empty)
                return;
            g.ResetTransform();
            var w = Route.PixelsPerMeter;
            g.TranslateTransform(position.X, position.Y);
            g.RotateTransform((float)(m * 180 / Math.PI));
            g.FillRectangle(Brushes.YellowGreen, (float)(-w / 2), (float)-(Route.RouteWidth / 2),
                (float)w, (float)Route.RouteWidth);
            g.ResetTransform();
        }


        public RoadSign(float pos, MyRoute route) : base(pos, route)
        {
        }

        public RoadSign(SerializationInfo info, StreamingContext context, MyRoute route) : base(info, context, route)
        {
        }
    }
}