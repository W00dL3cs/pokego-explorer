using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PokemonGo.RocketAPI.GeneratedCode;

namespace PokemonGO
{
    public partial class Main : Form
    {
        private PointLatLng _currentLatLng;
        private Specialized.Protocol.Manager _client;
        private bool _pausedScanning;
        private bool _customLatSet;
        private PointLatLng _customLatLng;

        public Main()
        {
            InitializeComponent();
        }

        private async void AttemptLogin()
        {
            try
            {
                btnLogin.Visible = false;
                txtHistory.AppendText(Environment.NewLine + "Attempting login...");
                _client = new Specialized.Protocol.Manager(gMapControl1.Position.Lat, gMapControl1.Position.Lng);
                if (!await _client.PerformLogin(Settings.PTC_USERNAME, Settings.PTC_PASSWORD))
                {
                    btnLogin.Visible = true;
                    txtHistory.AppendText(Environment.NewLine + "Login Failed!");
                    AttemptLogin();
                    return;
                }
                btnLogin.Visible = false;
                txtHistory.AppendText(Environment.NewLine + "Logged In!");
                gMapControl1.GrayScaleMode = false;
                var exploration = new Thread(Explore);
                exploration.Start();
            }
            catch (Exception ex)
            {
                txtHistory.AppendText(Environment.NewLine + ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CreateMap();
            AttemptLogin();
        }
        
        private async void Explore()
        {
            var x = 0;
            var y = 0;
            var dx = 0;
            var dy = -1;

            try
            {
                if (_customLatSet)
                {
                    //txtHistory.AppendText(Environment.NewLine + "Manual scan started!");
                    var destination = Specialized.Exploration.Helper.CalculateNextStep(_customLatLng.Lat, _customLatLng.Lng, Settings.EXPLORATION_STEPS, ref x, ref y, ref dx, ref dy);
                    if (await _client.RequestMove(destination.Lat, destination.Lng))
                    {
                        gMapControl1.Overlays.FirstOrDefault().Markers.FirstOrDefault().Position = destination;

                        var objects = await _client.GetNearbyPokemons();

                        foreach (var pokemon in objects.Pokemons)
                        {
                            var marker = Specialized.Pokemon.Utils.CreateMarker(pokemon);
                            gMapControl1.Overlays.FirstOrDefault().Markers.Add(marker);
                        }

                        foreach (var fort in objects.Forts)
                        {
                            if (fort.FortType == 1) continue;
                            var marker = Specialized.Forts.Utils.CreateMarker(fort);
                            gMapControl1.Overlays.FirstOrDefault().Markers.Add(marker);
                        }

                    }
                    _customLatSet = false;
                }
                else
                {
                    //txtHistory.AppendText(Environment.NewLine + "Auto scan started!");
                    while (!_pausedScanning)
                    {
                        for (var i = 0; i < Settings.EXPLORATION_STEPS; i++)
                        {
                            var Destination = Specialized.Exploration.Helper.CalculateNextStep(_currentLatLng.Lat, _currentLatLng.Lng, Settings.EXPLORATION_STEPS, ref x, ref y, ref dx, ref dy);

                            if (await _client.RequestMove(Destination.Lat, Destination.Lng))
                            {
                                gMapControl1.Overlays.FirstOrDefault().Markers.FirstOrDefault().Position = Destination;
                                var objects = await _client.GetNearbyPokemons();
                                foreach (var marker in objects.Pokemons.Select(Specialized.Pokemon.Utils.CreateMarker))
                                {
                                    gMapControl1.Overlays.FirstOrDefault().Markers.Add(marker);
                                }
                                foreach (var marker in from fort in objects.Forts where fort.FortType != 1 select Specialized.Forts.Utils.CreateMarker(fort))
                                {
                                    gMapControl1.Overlays.FirstOrDefault().Markers.Add(marker);
                                }
                            }
                            Thread.Sleep(Settings.STEP_DELAY*1000);
                            if (_pausedScanning)
                            {
                                break;
                            }
                        }
                        Thread.Sleep(Settings.CLEAR_DELAY*1000);
                        ClearMap();
                    }
                }
            }
            catch (Exception ex)
            {
                //txtHistory.AppendText(Environment.NewLine + e.Message);
            }
        }

        private delegate void ClearCallback();

        private void ClearMap()
        {
            if (gMapControl1.InvokeRequired)
            {
                var callback = new ClearCallback(ClearMap);
                Invoke(callback);
            }
            else
            {
                var Overlay = gMapControl1.Overlays.FirstOrDefault();
                if (Overlay == null) return;
                for (var i = 1; i < Overlay.Markers.Count; i++)
                {
                    Overlay.Markers.RemoveAt(i);
                }
            }
        }

        private void CreateMap()
        {
            gMapControl1.DragButton = MouseButtons.Left;
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            gMapControl1.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            gMapControl1.GrayScaleMode = true;
            gMapControl1.SetPositionByKeywords(Settings.STARTING_LOCATION);
            _currentLatLng = gMapControl1.Position;

            var overlay = new GMapOverlay("Markers");
            var position = new GMarkerGoogle(gMapControl1.Position, GMarkerGoogleType.blue_small)
            {
                ToolTipText = "Current scan location (" + _currentLatLng.Lat + ", " + _currentLatLng.Lng + ")"
            };

            overlay.Markers.Add(position);
            gMapControl1.Overlays.Add(overlay);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearMap();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            _pausedScanning = !_pausedScanning;
            btnPause.Text = _pausedScanning ? "Start Scanning" : "Pause Scanning";
            if (!_pausedScanning)
            {
                Explore();
            }
        }

        private void gMapControl1_Click(object sender, EventArgs e)
        {
            var mouseEventArgs = (MouseEventArgs) e;
            if (mouseEventArgs.Button != MouseButtons.Left) return;
            if (!_pausedScanning) return;

            var lat = gMapControl1.FromLocalToLatLng(mouseEventArgs.X, mouseEventArgs.Y).Lat;
            var lng = gMapControl1.FromLocalToLatLng(mouseEventArgs.X, mouseEventArgs.Y).Lng;

            _customLatSet = true;
            _customLatLng = new PointLatLng
            {
                Lat = lat,
                Lng = lng
            };

            Explore();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            AttemptLogin();
        }
    }
}
