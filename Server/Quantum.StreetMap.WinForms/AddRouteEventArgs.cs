using System;
using Quantum.StreetMap.WinForms.Routes;

namespace Quantum.StreetMap.WinForms
{
    public class AddRouteEventArgs : EventArgs
    {
        public bool Canceled { get; set; }
        public MyRoute Route { get; }

        public AddRouteEventArgs(MyRoute route)
        {
            Route = route;
        }
    }
}