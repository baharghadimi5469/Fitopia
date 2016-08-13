#region

using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Quantum.StreetMap.WinForms.Tools
{
    public abstract class ToolBase
    {
        protected ToolBase(MyMap map)
        {
            Map = map;
        }

        public abstract Cursor Cursor { get; }
        public abstract Keys ShortKey { get; }
        public MyMap Map { get; }

        public virtual void OnMouseAction(MapMouseEventArgs e)
        {
        }

        public virtual void OnCancel()
        {
        }

        public virtual void OnCursorMove(MapMouseEventArgs e)
        {
        }

        public virtual void Draw(Graphics g)
        {
        }
    }
}