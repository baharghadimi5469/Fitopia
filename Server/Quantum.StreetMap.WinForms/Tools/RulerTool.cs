using System.Drawing;
using System.Windows.Forms;
using GMap.NET;

namespace Quantum.StreetMap.WinForms.Tools
{
    public class RulerTool : ToolBase
    {
        private PointLatLng? _start;
        private PointLatLng? _end;
        private readonly Font _font;

        public RulerTool(MyMap map) : base(map)
        {
            _font = new Font(FontFamily.GenericSansSerif, 18);
        }

        public override void OnCancel()
        {
            _start = null;
        }

        public override void OnMouseAction(MapMouseEventArgs e)
        {
            if (e.Buttons == MouseButtons.Left)
            {
                _end = _start = e.Position;
            }
        }

        public override void OnCursorMove(MapMouseEventArgs e)
        {
            if (e.Buttons != MouseButtons.Left)
            {
                _start = null;
                return;
            }
            _end = e.Position;
            Map.Invalidate();
        }

        public override void Draw(Graphics g)
        {
            if (_start != null && _end != null)
            {

                var p0 = Map.FromLatLngToLocal(_start.Value);
                var p1 = Map.FromLatLngToLocal(_end.Value);


                g.DrawLine(Pens.Red, p0.X, p0.Y, p1.X, p1.Y);

                var len = Map.MapProvider.Projection.GetDistance(_start.Value, _end.Value).ToString("0.##") + " Km";

                var size = g.MeasureString(len, _font);
                                g.DrawString(len, _font, Brushes.Red, Map.Width - size.Width - 10, Map.Height - size.Height - 10 );
            }
        }

        public override Cursor Cursor => Cursors.Arrow;
        public override Keys ShortKey => Keys.R;
    }
}