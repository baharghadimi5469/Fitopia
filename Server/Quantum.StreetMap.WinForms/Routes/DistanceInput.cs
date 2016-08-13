#region

using System;
using System.Windows.Forms;
using DevComponents.DotNetBar;

#endregion

namespace Quantum.StreetMap.WinForms.Routes
{
    public partial class DistanceInput : Office2007Form
    {
        public DistanceInput()
        {
            InitializeComponent();
        }

        public int DistanceToExit { get; set; }

        private void button2_Click(object sender, EventArgs e)
        {
            DistanceToExit = numericUpDown1.Value;
        }
    }
}