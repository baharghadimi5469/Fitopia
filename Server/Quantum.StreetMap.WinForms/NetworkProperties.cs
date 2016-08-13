#region

using System;
using System.Linq;
using System.Windows.Forms;
using MyRoute = Quantum.StreetMap.WinForms.Routes.MyRoute;

#endregion

namespace Quantum.StreetMap.WinForms
{
    public partial class NetworkProperties : UserControl
    {
        private MyRoute _route;

        public NetworkProperties()
        {
            InitializeComponent();

            comboBoxExType.Items.AddRange(Enum.GetNames(typeof(MyRoute.RoadType)).Select(x => x as object).ToArray());

            
        }

        public MyRoute Route
        {
            get { return _route; }
            set
            {
                _route = value;

                if (_route != null)
                {
                    doubleInputAverageSpeed.Value = _route.AverageSpeed;
                    doubleInputMaxSpeed.Value = _route.MaxSpeed;
                    doubleInputMinSpeed.Value = _route.MinSpeed;
                    comboBoxExType.SelectedItem = _route.Type.ToString();
                    doubleInputWidth.Value = _route.LaneWidth;
                    integerInputLanes.Value = _route.LanesCount;
                    textBoxXName.Text = _route.RouteName;
                    doubleInputTotalLength.Text = _route.Distance.ToString("##.0");
                }
            }
        }

        private void textBoxXName_TextChanged(object sender, EventArgs e)
        {
            Route.RouteName = textBoxXName.Text;
        }

        private void integerInputLanes_ValueChanged(object sender, EventArgs e)
        {
            Route.LanesCount = integerInputLanes.Value;
        }

        private void doubleInputWidth_ValueChanged(object sender, EventArgs e)
        {
            Route.LaneWidth = doubleInputWidth.Value;
        }

        private void comboBoxExType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Route.Type = (MyRoute.RoadType) Enum.Parse(typeof(MyRoute.RoadType), comboBoxExType.SelectedItem.ToString());
        }

        private void doubleInputMinSpeed_ValueChanged(object sender, EventArgs e)
        {
            Route.MinSpeed = (int) doubleInputMinSpeed.Value;
        }

        private void doubleInputAverageSpeed_ValueChanged(object sender, EventArgs e)
        {
            Route.AverageSpeed = (int) doubleInputAverageSpeed.Value;
        }

        private void doubleInputMaxSpeed_ValueChanged(object sender, EventArgs e)
        {
            Route.MaxSpeed = (int) doubleInputMaxSpeed.Value;
        }

        private void buttonXReverse_Click(object sender, EventArgs e)
        {
            Route.Reverse();
        }
    }
}