#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using GMap.NET;
using GMap.NET.WindowsForms;

#endregion

namespace Quantum.StreetMap.WinForms.Markers
{
    public class MyPinMarker : GMapMarker, INotifyPropertyChanged
    {
        public enum MarkerShape
        {
            Star,
            GoogleMarker,
            Rectangle,
            Pin
        }

        public static Pen SelectionPen;

        private readonly SolidBrush _fillBrush;
        private readonly SolidBrush _mouseOverBrush;
        private Color _foreColor;


        private bool _isSelected;
        private Color _mouseOverColor;
        private string _name;
        private MarkerShape _shape;

        public MyPinMarker(PointLatLng pos) : base(pos)
        {
            Name = "new marker";
            _fillBrush = new SolidBrush(Color.Red);
            _mouseOverBrush = new SolidBrush(Color.Yellow);
            _shape = MarkerShape.Rectangle;


            Size = new Size(20, 20);
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                if (_isSelected == value)
                    return;
                OnPropertyChanged();
            }
        }

        public Color ForeColor
        {
            get { return _foreColor; }
            set
            {
                if (value.Equals(_foreColor)) return;
                _foreColor = value;
                _fillBrush.Color = value;
                OnPropertyChanged();
            }
        }

        public Color MouseOverColor
        {
            get { return _fillBrush.Color; }
            set
            {
                if (value.Equals(_mouseOverColor)) return;
                _mouseOverColor = value;
                _mouseOverBrush.Color = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                if (_name == value)
                    return;
                OnPropertyChanged();
            }
        }

        public MarkerShape Shape
        {
            get { return _shape; }
            set
            {
                if (value == _shape) return;
                _shape = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ForeColor));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override void OnRender(Graphics g)
        {
            switch (Shape)
            {
                case MarkerShape.Star:
                    break;
                case MarkerShape.GoogleMarker:
                    break;
                case MarkerShape.Rectangle:
                    DrawRectMarker(g);
                    break;
                case MarkerShape.Pin:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DrawRectMarker(Graphics g)
        {
            if (IsMouseOver)
            {
                g.FillRectangle(_mouseOverBrush, LocalPosition.X - Size.Width/2,
                    LocalPosition.Y - Size.Height/2,
                    Size.Width, Size.Height);
            }
            else
            {
                g.FillRectangle(_mouseOverBrush, LocalPosition.X - Size.Width/2,
                    LocalPosition.Y - Size.Height/2,
                    Size.Width, Size.Height);
            }

            if (IsSelected)
            {
                g.DrawRectangle(SelectionPen, LocalPosition.X - Size.Width/2,
                    LocalPosition.Y - Size.Height/2,
                    Size.Width, Size.Height);
            }
        }




        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Overlay.Control.Invalidate();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}