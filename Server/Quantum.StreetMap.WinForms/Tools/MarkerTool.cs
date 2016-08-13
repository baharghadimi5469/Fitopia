#region

using System.Windows.Forms;
using Quantum.StreetMap.WinForms.Markers;

#endregion

namespace Quantum.StreetMap.WinForms.Tools
{
    public class MarkerTool : ToolBase
    {
        public MarkerTool(MyMap map) : base(map)
        {
            map.Cursor = Cursors.Arrow;
        }

        public override Cursor Cursor => Cursors.Cross;
        public override Keys ShortKey => Keys.P;

        public override void OnMouseAction(MapMouseEventArgs e)
        {
            var latLngPosition = e.Position;
            Map.AddMarker(new MyPinMarker(latLngPosition));
        }
    }
}