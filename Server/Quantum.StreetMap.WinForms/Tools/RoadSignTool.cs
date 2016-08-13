#region

using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Quantum.StreetMap.WinForms.Routes;
using TrafficSignLibrary;
using TrafficSignLibrary.Contract;

#endregion

namespace Quantum.StreetMap.WinForms.Tools
{
    public class RoadSignTool : ToolBase
    {
        public RoadSignTool(MyMap map) : base(map)
        {
        }

        public override Cursor Cursor => Cursors.Cross;
        public override Keys ShortKey => Keys.S;

        public override void OnMouseAction(MapMouseEventArgs e)
        {
            var gMapRoute = Map.HoveringRoute;
            if (gMapRoute == null)
                return;
            if (!gMapRoute.IsMouseOver) return;
            var pos = gMapRoute.FromPointToPosition(e.LocalPosition);

            var inputFrm = new DistanceInput();
            if (inputFrm.ShowDialog() == DialogResult.Cancel)
                return;

//            var hash = GetCrypt(gMapRoute.AverageSpeed, inputFrm.DistanceToExit, (RoadTypeEntryEnum) gMapRoute.Type);
//            new FrmMain(hash, gMapRoute.AverageSpeed, inputFrm.DistanceToExit, (RoadTypeEntryEnum) gMapRoute.Type)
//                .ShowDialog();
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
                                (((int) roadTypeEntryEnum).ToString() + (roadTypeEntryEnum as object))))));
        }
    }
}