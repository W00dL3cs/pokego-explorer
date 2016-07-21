using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace PokemonGO
{
    public partial class Main : Form
    {
        private PointLatLng Position;
        private Specialized.Protocol.Manager Client;

        private bool IsExploring;
        private Thread Exploration;

        private delegate void ClearCallback();
        private delegate void WriteCallback(string Line);

        public Main()
        {
            InitializeComponent();
        }

        private void WriteLine(string Line)
        {
            if (txtHistory.InvokeRequired)
            {
                var Callback = new WriteCallback(WriteLine);

                Invoke(Callback, Line);
            }
            else
            {
                txtHistory.AppendText(string.Format("[{0}] - {1}{2}", DateTime.Now, Line, Environment.NewLine));
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CreateMap();

            AttemptLogin();
        }

        private void CreateMap()
        {
            gMapControl1.GrayScaleMode = true;
            gMapControl1.DragButton = MouseButtons.Left;
            GMaps.Instance.Mode = AccessMode.ServerOnly;

            gMapControl1.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            gMapControl1.SetPositionByKeywords(Settings.STARTING_LOCATION);

            Position = gMapControl1.Position;

            var Overlay = new GMapOverlay("Markers");
            var Marker = new GMarkerGoogle(gMapControl1.Position, GMarkerGoogleType.blue_small);

            Overlay.Markers.Add(Marker);
            gMapControl1.Overlays.Add(Overlay);

            SetPlayerPosition(Position);
        }

        private async void AttemptLogin()
        {
            try
            {
                WriteLine("Performing login operations...");

                Specialized.Controls.Helper.SetVisible(btnLogin, false);
                Specialized.Controls.Helper.SetVisible(btnPause, false);
                
                Client = new Specialized.Protocol.Manager(gMapControl1.Position.Lat, gMapControl1.Position.Lng);

                if (!await Client.PerformLogin(Settings.PTC_USERNAME, Settings.PTC_PASSWORD))
                {
                    WriteLine("Login failed! Please try again.");

                    Specialized.Controls.Helper.SetVisible(btnLogin);
                    
                    return;
                }

                WriteLine("Logged in!");

                gMapControl1.GrayScaleMode = false;

                Specialized.Controls.Helper.SetVisible(btnPause);
            }
            catch (Exception ex)
            {
                WriteLine("Error! Message: " + ex.Message);
            }
        }

        private void CreateThread()
        {
            Exploration = new Thread(Explore);

            Exploration.Start();
        }

        private async void Explore()
        {
            var x = 0;
            var y = 0;
            var dx = 0;
            var dy = -1;

            IsExploring = true;

            try
            {
                for (int i = 1; i <= Settings.EXPLORATION_STEPS; i++)
                {
                    var Destination = Specialized.Exploration.Helper.CalculateNextStep(Position.Lat, Position.Lng, Settings.EXPLORATION_STEPS, ref x, ref y, ref dx, ref dy);

                    if (await Client.RequestMove(Destination.Lat, Destination.Lng))
                    {
                        WriteLine(string.Format("Step #{0}. Moving to: ({1},{2}).", i, Destination.Lat, Destination.Lng));

                        SetPlayerPosition(Destination);

                        var Objects = await Client.GetNearbyData();

                        foreach (var Marker in Objects.Pokemons.Select(Specialized.Pokemon.Utils.CreateMarker))
                        {
                            gMapControl1.Overlays.FirstOrDefault().Markers.Add(Marker); // TODO: Handle Pokemons in another class (perform operations of storing etc)
                        }

                        foreach (var Marker in Objects.Forts.Where(Fort => Fort.FortType != 1).Select(Specialized.Forts.Utils.CreateMarker))
                        {
                            gMapControl1.Overlays.FirstOrDefault().Markers.Add(Marker);
                        }
                    }

                    Thread.Sleep(Settings.STEP_DELAY * 1000);
                }

                WriteLine(string.Format("Waiting {0} seconds before cleaning map...", Settings.CLEAR_DELAY));

                Thread.Sleep(Settings.CLEAR_DELAY * 1000);

                WriteLine("Cleaning map...");

                ClearMap();

                WriteLine("Exploration complete.");

                SetPlayerPosition(Position);

                Specialized.Controls.Helper.SetEnabled(btnPause);

                IsExploring = false;
            }
            catch (Exception e)
            {
            }
        }

        private void SetPlayerPosition(PointLatLng Position)
        {
            try
            {
                var Marker = gMapControl1.Overlays.FirstOrDefault().Markers.FirstOrDefault();

                Marker.Position = Position;
                Marker.ToolTipText = string.Format("Latitude: {0}{1}Longitude: {2}.", Position.Lat, Environment.NewLine, Position.Lng);
            }
            catch { }
        }

        private void ClearMap()
        {
            if (gMapControl1.InvokeRequired)
            {
                var Callback = new ClearCallback(ClearMap);

                Invoke(Callback);
            }
            else
            {
                try
                {
                    var Overlay = gMapControl1.Overlays.FirstOrDefault();

                    if (Overlay != null)
                    {
                        var Player = Overlay.Markers.FirstOrDefault();

                        Overlay.Markers.Clear();

                        Overlay.Markers.Add(Player);
                    }
                }
                catch { }
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            CreateThread();

            WriteLine(string.Format("Scanning started! Number of steps: {0}. Location: ({1},{2}).", Settings.EXPLORATION_STEPS, Position.Lat, Position.Lng));

            Specialized.Controls.Helper.SetEnabled(btnPause, false);
        }

        private void gMapControl1_DoubleClick(object sender, EventArgs e)
        {
            ChangeLocation((MouseEventArgs)e);
        }

        private void gMapControl1_Click(object sender, EventArgs e)
        {
            var Args = (MouseEventArgs)e;

            if (Args.Button == MouseButtons.Right)
            {
                ChangeLocation(Args);
            }
        }

        private void ChangeLocation(MouseEventArgs e)
        {
            if (!IsExploring)
            {
                Position = gMapControl1.FromLocalToLatLng(e.X, e.Y);

                SetPlayerPosition(Position);
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            AttemptLogin();
        }
    }
}
