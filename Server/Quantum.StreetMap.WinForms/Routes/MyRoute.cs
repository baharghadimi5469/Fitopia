#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;
using PathCalculator;
using Quantum.StreetMap.WinForms.Properties;

#endregion

namespace Quantum.StreetMap.WinForms.Routes
{
    public class MyRoute : GMapRoute, INotifyPropertyChanged
    {
        public enum RoadType
        {
            [Description("Express-way")]
            Expressway,
            [Description("Free-way")]
            Freeway,
            [Description("Street")]
            Street
        }

        private static readonly Pen SelectionPen;
        private static readonly Brush SelectionBrush;
        private readonly Pen _arrowPen;

        private readonly Pen _mouseOverpPen;
        private readonly Pen _roadMarkerPen;
        private readonly Pen _routePen;
        private Color _foreColor;
        private bool _isChangingPosition;
        private bool _isSelected;

        private int _lanesCount;
        private double _laneWidth;

        private Color _mouseOverColor;

        private IEnumerable<GraphicsPath> _paths;
        private Color _roadMarkerColor;
        private float _roadMarkerWidth;
        private string _routeName;
        private RoadType _type;

        public readonly GMapMarker TooltipMarker = new GMarkerCross(PointLatLng.Empty);

        public GMapToolTip ToolTip { get; set; }

        static MyRoute()
        {
            SelectionPen = new Pen(Color.Red);
            SelectionBrush = new SolidBrush(Color.Red);
        }

        public MyRoute(GMapControl map, IEnumerable<PointLatLng> points) : base(new PointLatLng[0], "new route")
        {
            IsHitTestVisible = false;
            Points.AddRange(points);
            Map = map;

            _lanesCount = 5;
            _laneWidth = 4.3;
            _roadMarkerPen = new Pen(Color.White)
            {
                DashStyle = DashStyle.Custom,
                DashPattern = new[] { 20f, 12 }
            };
            _routePen = new Pen(Color.FromArgb(255, 76, 81, 84));
            _mouseOverpPen = new Pen(Color.FromArgb(80, Color.Yellow));

            _arrowPen = new Pen(Settings.Default.DirectionArrowColor, 4) { EndCap = LineCap.ArrowAnchor };



            ToolTip = new GMapRoundedToolTip(TooltipMarker);
        }

        public RoadType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                switch (_type)
                {
                    case RoadType.Expressway:
                        ForeColor = Color.FromArgb(17, 98, 3);
                        break;
                    case RoadType.Freeway:
                        ForeColor = Color.FromArgb(11, 56, 148);
                        break;
                    case RoadType.Street:
                        ForeColor = Color.FromArgb(255, 76, 81, 84);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public string RouteName
        {
            get { return _routeName; }
            set
            {
                _routeName = value;
                OnPropertyChanged();
            }
        }

        public int MaxSpeed { get; set; }
        public int AverageSpeed { get; set; }
        public int MinSpeed { get; set; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value == _isSelected) return;
                _isSelected = value;
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
                _routePen.Color = value;
                OnPropertyChanged();
            }
        }

        public Color RoadMarkerColor
        {
            get { return _roadMarkerColor; }
            set
            {
                if (value.Equals(_roadMarkerColor)) return;
                _roadMarkerColor = value;
                _roadMarkerPen.Color = value;
                OnPropertyChanged();
            }
        }

        public Color MouseOverColor
        {
            get { return _mouseOverColor; }
            set
            {
                if (value.Equals(_mouseOverColor)) return;
                _mouseOverColor = value;
                _mouseOverpPen.Color = value;
                OnPropertyChanged();
            }
        }

        public float RoadMarkerWidth
        {
            get { return _roadMarkerWidth; }
            set
            {
                if (value.Equals(_roadMarkerWidth)) return;
                _roadMarkerWidth = value;
                _roadMarkerPen.Width = value;
                OnPropertyChanged();
            }
        }

        public int LanesCount
        {
            get { return _lanesCount; }
            set
            {
                if (value == _lanesCount) return;
                _lanesCount = value;
                OnPropertyChanged();
            }
        }

        public double LaneWidth
        {
            get { return _laneWidth; }
            set
            {
                if (value.Equals(_laneWidth)) return;
                _laneWidth = value;
                OnPropertyChanged();
            }
        }

        public double RouteWidth => LaneWidth * LanesCount * PixelsPerMeter;

        public double PixelsPerMeter
            => 1.0 / Map.MapProvider.Projection.GetGroundResolution((int)Map.Zoom, Map.Position.Lat);

        public bool IsChangingPosition
        {
            get { return _isChangingPosition; }
            set
            {
                if (value == _isChangingPosition) return;
                _isChangingPosition = value;
                OnPropertyChanged();
            }
        }

        public GMapControl Map { get; set; }

        protected double PathWidth
        {
            get
            {
                var groundResolution = Map.MapProvider.Projection.GetGroundResolution((int)Map.Zoom, Map.Position.Lat);
                return _laneWidth / groundResolution * _lanesCount;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;



        public void AddPoint(Point point)
        {
            var latLng = Map.FromLocalToLatLng(point.X, point.Y);
            AddPoint(latLng);
            CalculatePoints();
            Invalidate();
        }

        public void AddPoint(PointLatLng point)
        {
            Points.Add(point);
            CalculatePoints();
            Invalidate();
        }

        public void SetLastPoint(Point point)
        {
            Points[Points.Count - 1] = Map.FromLocalToLatLng(point.X, point.Y);
            CalculatePoints();
            Invalidate();
        }

        public void RemoveLastPoint()
        {
            if (Points.Count >= 1)
                Points.RemoveAt(Points.Count - 1);
            CalculatePoints();
            Overlay.Control.UpdateRouteLocalPosition(this);
            Invalidate();
        }

        private void CalculatePoints()
        {
            _paths = CreateRoadMarking(LocalPoints.Select(x => new PointF(x.X, x.Y)), 4, _lanesCount * _laneWidth / 2.0);
        }

        public void Invalidate()
        {
            Overlay.Control.UpdateRouteLocalPosition(this);
            Overlay.Control.Invalidate();
        }

        private IEnumerable<PointF> MapPoints()
        {
            return Points.Select(Overlay.Control.FromLatLngToLocal).Select(x => new PointF(x.X, x.Y));
        }

        private IEnumerable<GraphicsPath> CreateRoadMarking(IEnumerable<PointF> localPath, int count, double laneWidth)
        {
            laneWidth = laneWidth * PixelsPerMeter;
            var retValue = new List<GraphicsPath>();

            var width = count * laneWidth;
            var mPath = new MyPath(localPath.Select(x => new MyPoint(x.X, x.Y)));

            for (var i = 1; i < count; i++)
            {
                var distance = i * laneWidth - width / 2.0;
                var parallelPath = mPath.GetParallelPath(distance).Points.Select(x => new PointF(x.X, x.Y)).ToArray();
                if (parallelPath.Length <= 0)
                    continue;
                var path = new GraphicsPath();
                path.AddLines(parallelPath);
                retValue.Add(path);
            }

            return retValue;
        }

        public void Reverse()
        {
            Points.Reverse();
            Invalidate();
        }

        public ObservableCollection<RouteElement> Elements { get; } = new ObservableCollection<RouteElement>();

        public override void OnRender(Graphics g)
        {
            _mouseOverpPen.Width = _routePen.Width = Math.Max((float)(_lanesCount * _laneWidth * PixelsPerMeter), 1f);

            _roadMarkerPen.DashPattern = new[]
            {
                (float) (5*PixelsPerMeter),
                (float) (5*PixelsPerMeter)
            };
            _roadMarkerPen.Width = (float)(0.2 * PixelsPerMeter);
            Stroke = _routePen;
            base.OnRender(g);
            if (_isSelected)
            {
                try
                {
                    _paths = CreateRoadMarking(LocalPoints.Select(x => new PointF(x.X, x.Y)), 4,
                        _lanesCount * _laneWidth / 2.0);
                    if (_paths != null)
                    {
                        foreach (var path in _paths)
                            g.DrawPath(SelectionPen, path);

                        foreach (var localPoint in LocalPoints)
                            g.FillEllipse(SelectionBrush, new RectangleF(localPoint.X - 3, localPoint.Y - 3, 6, 6));
                    }
                }
                catch (OverflowException)
                {
                }
            }
            else if (IsHitTestVisible && IsMouseOver)
            {
                Stroke = _mouseOverpPen;
                base.OnRender(g);
            }


            if (_roadMarkerPen.Width < 0.05)
                return;
            var markerPaths = CreateRoadMarking(LocalPoints.Select(x => new PointF(x.X, x.Y)), _lanesCount, _laneWidth);
            foreach (var path in markerPaths)
                g.DrawPath(_roadMarkerPen, path);
            try
            {
                DrawDirectionArrows(g);
            }
            catch (OverflowException)
            {
            }

            foreach (var routeElement in Elements)
            {
                routeElement.Render(g);
            }

            var txt = $"{RouteName} \r\n {Distance.ToString("##.00")}Km";
            TooltipMarker.ToolTipText = txt;
            if (ToolTip != null && IsVisible && IsMouseOver)
                ToolTip.OnRender(g);

        }

        private void DrawDirectionArrows(Graphics g)
        {
            if (LocalPoints.Count < 2)
                return;
            var arrowLength = PixelsPerMeter * 10;
            var length = LocalPoints[0].Distance(LocalPoints[1]);
            var p = new GPoint((long)(LocalPoints[0].X + (LocalPoints[1].X - LocalPoints[0].X) / length * arrowLength),
                (long)(LocalPoints[0].Y + (LocalPoints[1].Y - LocalPoints[0].Y) / length * arrowLength));

            DrawArrows(g, LocalPoints[0], p);

            length = LocalPoints[LocalPoints.Count - 1].Distance(LocalPoints[LocalPoints.Count - 2]);
            p =
                new GPoint(
                    (long)
                        (LocalPoints[LocalPoints.Count - 1].X +
                         (LocalPoints[LocalPoints.Count - 2].X - LocalPoints[LocalPoints.Count - 1].X) / length *
                         arrowLength),
                    (long)
                        (LocalPoints[LocalPoints.Count - 1].Y +
                         (LocalPoints[LocalPoints.Count - 2].Y - LocalPoints[LocalPoints.Count - 1].Y) / length *
                         arrowLength));

            DrawArrows(g, p, LocalPoints[LocalPoints.Count - 1]);
        }

        private void DrawArrows(Graphics g, GPoint p1, GPoint p2)
        {
            _arrowPen.Width = (float)(PixelsPerMeter * _laneWidth / 4.0);
            var paths = CreateRoadMarking(new[] { p1, p2 }.Select(x => new PointF(x.X, x.Y)), _lanesCount + 1, _laneWidth);

            foreach (var path in paths)
            {
                g.DrawPath(_arrowPen, path);
            }
        }


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Invalidate();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public float FromPointToPosition(PointF point)
        {
            var min = RouteWidth / 2.0;
            var pos = new PointF();
            var index = -1;
            var lpoints = Points.Select(x => Map.FromLatLngToLocal(x)).ToArray();
            for (var i = 0; i < LocalPoints.Count - 1; i++)
            {
                PointF p;
                var distance = FindDistanceToSegment(point, new PointF(lpoints[i].X, lpoints[i].Y),
                    new PointF(lpoints[i + 1].X, lpoints[i + 1].Y), out p);
                if (distance < min)
                {
                    index = i;
                    min = distance;
                    pos = p;
                }
            }
            if (index == -1)
                return float.NaN;
            var retValue = 0.0;
            for (var i = 0; i < Points.Count - 1 && i < index; i++)
                retValue += Map.MapProvider.Projection.GetDistance(Points[i], Points[i + 1]);

            var lastPositio = Map.FromLocalToLatLng((int)pos.X, (int)pos.Y);
            retValue += Map.MapProvider.Projection.GetDistance(Points[index], lastPositio);

            return (float)retValue;
        }

        public PointF FromPositionToPoint(double position, out double angle)
        {
            for (var i = 0; i < LocalPoints.Count - 1; i++)
            {
                var distance = Map.MapProvider.Projection.GetDistance(Points[i], Points[i + 1]);
                if (distance > position)
                {
                    var p0 = Map.FromLatLngToLocal(Points[i]);
                    var p1 = Map.FromLatLngToLocal(Points[i + 1]);
                    var dx = p1.X - p0.X;
                    var dy = p1.Y - p0.Y;
                    var x = position / distance * dx + p0.X;
                    var y = position / distance * dy + p0.Y;
                    angle = Math.Atan2(dy, dx);
                    return new PointF((float)x, (float)y);
                }
            }
            angle = 0;
            return PointF.Empty;
        }

        protected double FindDistanceToSegment(PointF pt, PointF p1, PointF p2, out PointF closest)
        {
            var dx = p2.X - p1.X;
            var dy = p2.Y - p1.Y;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                closest = p1;
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            var t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) / (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                closest = new PointF(p1.X, p1.Y);
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                closest = new PointF(p2.X, p2.Y);
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                closest = new PointF(p1.X + t * dx, p1.Y + t * dy);
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }

            return Math.Sqrt(dx * dx + dy * dy);
        }

        public void AddElement(RouteElement element)
        {
            Invalidate();
        }
    }
}