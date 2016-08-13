#region

using System;
using System.Drawing;
using System.Windows.Forms;
using DevComponents.AdvTree;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Rendering;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.ObjectModel;
using Quantum.StreetMap.WinForms.Routes;

#endregion

namespace Quantum.StreetMap.WinForms
{
    public partial class MainForm : Office2007Form
    {
        public MainForm()
        {
            Office2007ColorTable = eOffice2007ColorScheme.Silver;
            InitializeComponent();
            myMap.MapProvider = OpenStreetMapProvider.Instance;
            myMap.Position = new PointLatLng(35.6892, 51.3890);
            myMap.MaxZoom = 20;
            myMap.MinZoom = 1;
            myMap.Zoom = 15;
            myMap.DragButton = MouseButtons.Middle;
            myMap.ShowCenter = false;
            myMap.MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;
            myMap.MapScaleInfoEnabled = true;
            myMap.ScalePen = new Pen(Color.Black) { Width = 2 };

            myMap.Overlay.Routes.CollectionChanged += Routes_CollectionChanged1;
        }

        private Node FindNodeByTag(AdvTree tree, object tag)
        {
            foreach (Node node in tree.Nodes)
            {
                if (node.Tag == tag)
                    return node;
            }
            return null;
        }

        private bool _second;

        private void Routes_CollectionChanged1(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_second)
            {
                _second = false;
                return;
            }
            _second = true;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (MyRoute newItem in e.NewItems)
                    {
                        var node = new Node(newItem.Name)
                        {
                            Tag = newItem
                        };
                        newItem.PropertyChanged += NewItem_PropertyChanged;
                        advTreeComponents.Nodes.Add(node);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (MyRoute oldItem in e.OldItems)
                    {
                        var node = FindNodeByTag(advTreeComponents, oldItem);
                        oldItem.PropertyChanged -= NewItem_PropertyChanged;
                        if (node != null)
                            advTreeComponents.Nodes.Remove(node);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    advTreeComponents.Nodes.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void NewItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var node = FindNodeByTag(advTreeComponents, sender);
            if (node == null)
                return;
            node.Text = (node.Tag as MyRoute)?.RouteName;
        }

        private void buttonXSelector_Click(object sender, EventArgs e)
        {
            buttonXRoad.Checked = buttonXSelector.Checked = buttonXSign.Checked = buttonXRuler.Checked = false;

            var buttonX = sender as ButtonX;
            if (buttonX == null)
                return;
            buttonX.Checked = true;
            var tool = buttonX.Tag?.ToString().ToLower();
            switch (tool)
            {
                case "selector":
                    myMap.ToolMode = MyMap.Mode.Select;
                    break;
                case "roadsign":
                    myMap.ToolMode = MyMap.Mode.RoadSign;
                    break;
                case "marker":
                    myMap.ToolMode = MyMap.Mode.Marker;
                    break;
                case "route":
                    myMap.ToolMode = MyMap.Mode.Road;
                    break;
                case "ruler":
                    myMap.ToolMode = MyMap.Mode.Ruler;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void buttonXRuler_CheckedChanged(object sender, EventArgs e)
        {
            //            (sender as ButtonX).BackColor = (sender as ButtonX).Checked ? Color.GreenYellow : Color.Transparent;
        }
    }
}