#region

using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#endregion

namespace Quantum.StreetMap.WinForms.Tools
{
    public class SelectorTool : ToolBase
    {
        private int _selectedJoint;

        public SelectorTool(MyMap map) : base(map)
        {
        }

        public override Cursor Cursor => Cursors.Arrow;
        public override Keys ShortKey => Keys.S;

        public override void OnMouseAction(MapMouseEventArgs e)
        {
            _selectedJoint = -1;

            if (Control.ModifierKeys != Keys.Control)
                foreach (var myRoute in Map.SelectedRoute)
                    myRoute.IsSelected = false;
            if (Map.HoveringRoute != null)
            {
                Map.HoveringRoute.IsSelected = true;
                for (var i = 0; i < Map.HoveringRoute.LocalPoints.Count; i++)
                {
                    var localPoint = Map.FromLatLngToLocal(Map.HoveringRoute.Points[i]);
                    var r = new RectangleF(localPoint.X - 5, localPoint.Y - 5, 10, 10);
                    if (r.Contains(e.LocalPosition))
                        _selectedJoint = i;
                }
            }
        }

        public override void OnCursorMove(MapMouseEventArgs e)
        {
            var selectedRoute = Map.SelectedRoute.FirstOrDefault();
            if (selectedRoute == null)
                return;

            if (_selectedJoint >= 0)
                selectedRoute.Points[_selectedJoint] = e.Position;

            selectedRoute.Invalidate();
        }

        public override void OnCancel()
        {
            _selectedJoint = -1;
        }
    }
}