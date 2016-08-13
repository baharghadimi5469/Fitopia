#region

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Windows.Forms;
using GMap.NET;
using Quantum.StreetMap.WinForms.Routes;

#endregion

namespace Quantum.StreetMap.WinForms.Tools
{
    public class RouteTool : ToolBase
    {
        private MyRoute _addingRoute;

        public RouteTool(MyMap map) : base(map)
        {
            Map.Cursor = Cursors.Cross;
        }

        public override Cursor Cursor => Cursors.Cross;
        public override Keys ShortKey => Keys.R;

        public override void OnCancel()
        {
            if (_addingRoute != null)
            {
                _addingRoute.RemoveLastPoint();

                var configFrm = new NetworkConfig { Route = _addingRoute };

                if (configFrm.ShowDialog() == DialogResult.Cancel)
                    Map.RemoveRoute(_addingRoute);
            }
            _addingRoute = null;
        }

        public override void OnMouseAction(MapMouseEventArgs e)
        {
            var currentPosition = e.Position;
            if (_addingRoute == null)
            {
                _addingRoute = new MyRoute(Map, new List<PointLatLng> { currentPosition });

                Map.AddRoute(_addingRoute);
            }

            _addingRoute.AddPoint(currentPosition);
        }

        public override void OnCursorMove(MapMouseEventArgs e)
        {
//            if (_addingRoute != null)
//            {
//                var currentPosition = e.Position;
//                _addingRoute.SetLastPoint(currentPosition);
//            }
//            Map.Invalidate();
        }
    }
}