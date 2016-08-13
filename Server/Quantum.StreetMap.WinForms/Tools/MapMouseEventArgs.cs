#region

using System.Drawing;
using System.Windows.Forms;
using GMap.NET;

#endregion

namespace Quantum.StreetMap.WinForms.Tools
{
    public class MapMouseEventArgs
    {
        public MouseButtons Buttons { get; set; }
        public PointLatLng Position { get; set; }
        public Point LocalPosition { get; set; }
    }
}