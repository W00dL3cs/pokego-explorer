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
        private bool _scanningStopped = true;
        private bool _customLatSet;
        private bool _customSearchSingleLocation;
        private PointLatLng _customLatLng;

        public Main()
        {
            InitializeComponent();
        }

        private async void AttemptLogin()
        {
            try
            {
                btnPause.Text = _scanningStopped ? "Start Auto Scanning" : "Stop Auto Scanning";
                btnLogin.Visible = false;
                txtHistory.AppendText(Environment.NewLine + "Attempting to login...");
                _client = new Specialized.Protocol.Manager(gMapControl1.Position.Lat, gMapControl1.Position.Lng);
                if (!await _client.PerformLogin(Settings.PTC_USERNAME, Settings.PTC_PASSWORD))
                {
                    btnLogin.Visible = true;
                    txtHistory.AppendText(Environment.NewLine + "Login failed!");
                    AttemptLogin();
                    return;
                }
                btnLogin.Visible = false;
                txtHistory.AppendText(Environment.NewLine + "Logged in!");
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

        private async void ExploreArea()
        {
            var x = 0;
            var y = 0;
            var dx = 0;
            var dy = -1;

            var destination = Specialized.Exploration.Helper.CalculateNextStep(_currentLatLng.Lat, _currentLatLng.Lng, Settings.EXPLORATION_STEPS, ref x, ref y, ref dx, ref dy);
            if (await _client.RequestMove(destination.Lat, destination.Lng))
            {
                gMapControl1.Overlays.FirstOrDefault().Markers.FirstOrDefault().Position = destination;
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
                    if (_customSearchSingleLocation)
                    {
                        var destination = Specialized.Exploration.Helper.CalculateNextStep(_currentLatLng.Lat, _currentLatLng.Lng, Settings.EXPLORATION_STEPS, ref x, ref y, ref dx, ref dy);

                        if (await _client.RequestMove(destination.Lat, destination.Lng))
                        {
                            gMapControl1.Overlays.FirstOrDefault().Markers.FirstOrDefault().Position = destination;
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
                    }
                    else
                    {
                        for (var i = 0; i < Settings.EXPLORATION_STEPS; i++)
                        {
                            var destination = Specialized.Exploration.Helper.CalculateNextStep(_currentLatLng.Lat, _currentLatLng.Lng, Settings.EXPLORATION_STEPS, ref x, ref y, ref dx, ref dy);

                            if (await _client.RequestMove(destination.Lat, destination.Lng))
                            {
                                gMapControl1.Overlays.FirstOrDefault().Markers.FirstOrDefault().Position = destination;
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
                            Thread.Sleep(Settings.STEP_DELAY * 1000);
                        }
                    }
                    _customLatSet = false;
                }
                else
                {
                    while (!_scanningStopped)
                    {
                        for (var i = 0; i < Settings.EXPLORATION_STEPS; i++)
                        {
                            if (_scanningStopped)
                            {
                                continue;
                            }
                            var destination = Specialized.Exploration.Helper.CalculateNextStep(_currentLatLng.Lat, _currentLatLng.Lng, Settings.EXPLORATION_STEPS, ref x, ref y, ref dx, ref dy);

                            if (await _client.RequestMove(destination.Lat, destination.Lng))
                            {
                                gMapControl1.Overlays.FirstOrDefault().Markers.FirstOrDefault().Position = destination;
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
                            Thread.Sleep(Settings.STEP_DELAY * 1000);
                        }
                        if (_scanningStopped) continue;
                        Thread.Sleep(Settings.CLEAR_DELAY*1000);
                        ClearMap();
                    }
                }
            }
            catch (Exception ex)
            {
                // Ignore
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
                for (var x = 0; x < 10; x++)
                {
                    var overlay = gMapControl1.Overlays.FirstOrDefault();
                    if (overlay == null) return;
                    for (var i = 1; i < overlay.Markers.Count; i++)
                    {
                        overlay.Markers.RemoveAt(i);
                    }
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
                ToolTipText = _currentLatLng.Lat + ", " + _currentLatLng.Lng
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
            _scanningStopped = !_scanningStopped;
            btnPause.Text = _scanningStopped ? "Start Auto Scanning" : "Stop Auto Scanning";
            if (!_scanningStopped)
            {
                txtHistory.AppendText(Environment.NewLine + "Auto scanning " + Settings.EXPLORATION_STEPS + " steps near " + _currentLatLng.Lat + ", " + _currentLatLng.Lng);
                Explore();
            }
            else
            {
                txtHistory.AppendText(Environment.NewLine + "Auto scanning stopped.");
            }
        }

        private void CustomLocation(bool searchSingleLocation, MouseEventArgs e)
        {
            var mouseEventArgs = e;
            if (!_scanningStopped) return;

            var lat = gMapControl1.FromLocalToLatLng(mouseEventArgs.X, mouseEventArgs.Y).Lat;
            var lng = gMapControl1.FromLocalToLatLng(mouseEventArgs.X, mouseEventArgs.Y).Lng;

            _customLatSet = true;
            if (_customSearchSingleLocation)
            {
                txtHistory.AppendText(Environment.NewLine + "Scanning once at " + lat + ", " + lng);
            }
            else
            {
                txtHistory.AppendText(Environment.NewLine + "Scanning " + Settings.EXPLORATION_STEPS + " steps near " + lat + ", " + lng);
            }
            _customSearchSingleLocation = searchSingleLocation;
            _customLatLng = new PointLatLng
            {
                Lat = lat,
                Lng = lng
            };
            _currentLatLng = _customLatLng;

            Explore();
        }

        private void gMapControl1_DoubleClick(object sender, EventArgs e)
        {
            CustomLocation(false, (MouseEventArgs)e);
        }

        private void gMapControl1_Click(object sender, EventArgs e)
        {
            var mouseEventArgs = (MouseEventArgs)e;
            if (mouseEventArgs.Button != MouseButtons.Right) return;
            CustomLocation(true, (MouseEventArgs)e);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            AttemptLogin();
        }
    }
}
