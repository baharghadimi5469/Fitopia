using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using Quantum.StreetMap.WinForms.Routes;

namespace Quantum.StreetMap.WinForms
{
    public partial class NetworkConfig : Office2007Form
    {
        public MyRoute Route
        {
            get { return networkProperties1.Route; }
            set { networkProperties1.Route = value; }
        }

        public NetworkConfig()
        {
            InitializeComponent();
        }

        private void buttonXClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void networkProperties1_Load(object sender, EventArgs e)
        {

        }

        private void buttonXOk_Click(object sender, EventArgs e)
        {
            DialogResult=DialogResult.OK;
            Close();
        }
    }
}
