using System.Drawing;
using System.Runtime.Serialization;
using GMap.NET;
using GMap.NET.WindowsForms;

namespace Quantum.StreetMap.WinForms.Routes
{
    public abstract class RouteElement 
    {
        public MyRoute Route { get; }
        public float PositionFromStart { get; set; }

        public abstract void Render(Graphics g);

        public RouteElement(float pos, MyRoute route) 
        {
            PositionFromStart = pos;
            Route = route;
        }

    }
}