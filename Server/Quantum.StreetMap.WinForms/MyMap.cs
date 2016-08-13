#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using Quantum.StreetMap.WinForms.Markers;
using Quantum.StreetMap.WinForms.Routes;
using Quantum.StreetMap.WinForms.Tools;
using TrafficSignLibrary;
using TrafficSignLibrary.Contract;

#endregion

namespace Quantum.StreetMap.WinForms
{
    public class RoadConnection
    {
        public MyRoute Route1 { get; set; }
        public MyRoute Route2 { get; set; }
        public int SourceRouteLaneStart { get; set; }
        public int SourceRouteLaneCount { get; set; }
        public int DestinationRouteLaneStart { get; set; }
        public int DestinationRouteLaneCount { get; set; }
    }

    public sealed class MyMap : GMapControl
    {


        public enum Mode
        {
            Ruler,
            Select,
            Road,
            RoadSign,
            Connection,
            Marker
        }

        private readonly Font _font = new Font(FontFamily.GenericSansSerif, 18);

        private readonly GMapOverlay _myOverlay;

        private MyRoute _addingRoute;
        private PointLatLng? _end;

        private PointLatLng? _start;
        private Mode _toolMode;

        public MyMap()
        {
            MapProvider.BypassCache = true;
            _myOverlay = new GMapOverlay("signOverlay");

            Overlays.Add(_myOverlay);
            Initialize();

            DoubleBuffered = true;
            SelectionPen = new Pen(Color.Transparent);

            OnRouteEnter += MyMap_OnRouteEnter;
            OnRouteClick += MyMap_OnRouteClick;
        }

        public Mode ToolMode
        {
            get { return _toolMode; }
            set
            {
                _toolMode = value;
                _end = _start = null;
                switch (_toolMode)
                {
                    case Mode.Ruler:
                        ChangeHover(false);
                        break;
                    case Mode.Select:
                        ChangeHover(true);
                        break;
                    case Mode.Road:
                        ChangeHover(false);
                        break;
                    case Mode.RoadSign:
                        ChangeHover(false);
                        break;
                    case Mode.Connection:
                        ChangeHover(true);
                        break;
                    case Mode.Marker:
                        ChangeHover(false);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                Invalidate();
            }
        }

        private void ChangeHover(bool value)
        {
            foreach (var gMapMarker in _myOverlay.Markers)
                gMapMarker.IsHitTestVisible = value;
            foreach (var gMapRoute in _myOverlay.Routes)
                gMapRoute.IsHitTestVisible = value;
        }


        public IEnumerable<MyRoute> Routes => _myOverlay.Routes.Cast<MyRoute>();

        public GMapOverlay Overlay => _myOverlay;

        public MyRoute[] SelectedRoute => _myOverlay.Routes.Cast<MyRoute>().Where(x => x?.IsSelected ?? false).ToArray();

        public MyRoute HoveringRoute => _myOverlay.Routes.Cast<MyRoute>().FirstOrDefault(x => x?.IsMouseOver ?? false);

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (SelectedRoute.Length > 0)
            {
                new NetworkConfig { Route = SelectedRoute[0] }.Show();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
                Cancel();
        }

        private void MyMap_OnRouteClick(GMapRoute item, MouseEventArgs e)
        {
            foreach (var gMapRoute1 in _myOverlay.Routes)
            {
                var gMapRoute = (MyRoute)gMapRoute1;
                gMapRoute.IsSelected = false;
            }
            var route = item as MyRoute;
            if (e.Button == MouseButtons.Left && route != null)
                route.IsSelected = true;
        }

        private void MyMap_OnRouteEnter(GMapRoute item)
        {
        }

        private void Initialize()
        {
        }

        public void AddMarker(MyPinMarker marker)
        {
            _myOverlay.Markers.Add(marker);
        }

        public void AddRoute(MyRoute route)
        {
            _myOverlay.Routes.Add(route);
        }

        private int _selectedJoint;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _end = _start = FromLocalToLatLng(e.X, e.Y);

            switch (ToolMode)
            {
                case Mode.Ruler:
                    break;
                case Mode.Select:
                    if (e.Button == MouseButtons.Left)
                    {
                        _selectedJoint = -1;

                        if (ModifierKeys != Keys.Control)
                            foreach (var myRoute in SelectedRoute)
                                myRoute.IsSelected = false;
                        if (HoveringRoute != null)
                        {
                            HoveringRoute.IsSelected = true;
                            for (var i = 0; i < HoveringRoute.LocalPoints.Count; i++)
                            {
                                var localPoint = FromLatLngToLocal(HoveringRoute.Points[i]);
                                var r = new RectangleF(localPoint.X - 5, localPoint.Y - 5, 10, 10);
                                if (r.Contains(e.Location))
                                    _selectedJoint = i;
                            }
                        }
                    }
                    break;
                case Mode.Road:
                    if (e.Button == MouseButtons.Left)
                    {
                        var currentPosition = _start.Value;
                        if (_addingRoute == null)
                        {
                            _addingRoute = new MyRoute(this, new List<PointLatLng> { currentPosition });

                            AddRoute(_addingRoute);
                        }
                        _addingRoute.AddPoint(currentPosition);
                    }

                    if (e.Button == MouseButtons.Right)
                    {
                        if (_addingRoute != null)
                        {
                            _addingRoute.RemoveLastPoint();

                            var configFrm = new NetworkConfig { Route = _addingRoute };

                            if (configFrm.ShowDialog() == DialogResult.Cancel)
                                RemoveRoute(_addingRoute);
                        }
                        _addingRoute = null;
                    }

                    break;
                case Mode.RoadSign:
                    if (e.Button == MouseButtons.Left)
                    {
                        var gMapRoute = HoveringRoute;
                        if (gMapRoute == null)
                            return;
                        if (!gMapRoute.IsMouseOver) return;
                        var pos = gMapRoute.FromPointToPosition(e.Location);

                        var inputFrm = new DistanceInput();
                        if (inputFrm.ShowDialog() == DialogResult.Cancel)
                            return;

                        var hash = GetCrypt(gMapRoute.AverageSpeed, inputFrm.DistanceToExit,
                            (RoadTypeEntryEnum)gMapRoute.Type);
                        new FrmMain(hash, gMapRoute.AverageSpeed, inputFrm.DistanceToExit,
                            (RoadTypeEntryEnum)gMapRoute.Type)
                            .ShowDialog();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string GetCrypt(int speed, int distanceToExit, RoadTypeEntryEnum roadTypeEntryEnum)
        {
            return
                Encoding.Unicode.GetString(
                    SHA512.Create()
                        .ComputeHash(
                            Encoding.Unicode.GetBytes(string.Join(",",
                                speed.ToString() + speed + (speed as object) + (speed as object) +
                                (distanceToExit.ToString() + (distanceToExit as object) + (distanceToExit as object)) +
                                (((int)roadTypeEntryEnum).ToString() + (roadTypeEntryEnum as object))))));
        }

        private void Cancel()
        {
            _selectedJoint = -1;
            _start = _end = null;
            Invalidate();
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Cancel();

            base.OnPreviewKeyDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (HoveringRoute != null)
            {
                HoveringRoute.TooltipMarker.Position = FromLocalToLatLng(e.X, e.Y);
                UpdateMarkerLocalPosition(HoveringRoute.TooltipMarker);
            }
            switch (ToolMode)
            {
                case Mode.Ruler:
                    _end = FromLocalToLatLng(e.X, e.Y);
                    Invalidate();
                    break;
                case Mode.Select:
                    var selectedRoute = SelectedRoute.FirstOrDefault();
                    if (selectedRoute == null)
                        return;

                    if (_selectedJoint >= 0)
                        selectedRoute.Points[_selectedJoint] = FromLocalToLatLng(e.X, e.Y);

                    selectedRoute.Invalidate();
                    break;
                case Mode.Road:
                    _addingRoute?.SetLastPoint(e.Location);
                    break;
                case Mode.RoadSign:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_start != null && _end != null)
                DrawRuler(e.Graphics, _start.Value, _end.Value);
        }

        private void DrawRuler(Graphics g, PointLatLng start, PointLatLng end)
        {
            var p0 = FromLatLngToLocal(start);
            var p1 = FromLatLngToLocal(end);


            g.DrawLine(Pens.Red, p0.X, p0.Y, p1.X, p1.Y);

            var len = MapProvider.Projection.GetDistance(start, end).ToString("0.##") + " Km";

            var size = g.MeasureString(len, _font);
            g.DrawString(len, _font, Brushes.Red, Width - size.Width - 10, Height - size.Height - 10);
        }

        public void RemoveRoute(MyRoute route)
        {
            _myOverlay.Routes.Remove(route);
        }
    }
}