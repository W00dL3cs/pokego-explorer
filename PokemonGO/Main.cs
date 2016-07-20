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

namespace PokemonGO
{
    public partial class Main : Form
    {
        private PointLatLng Original;

        private Specialized.Protocol.Manager Client;
        
        public Main()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            CreateMap();

            Client = new Specialized.Protocol.Manager(gMapControl1.Position.Lat, gMapControl1.Position.Lng);

            if (await Client.PerformLogin(Settings.PTC_USERNAME, Settings.PTC_PASSWORD))
            {
                var Exploration = new Thread(new ThreadStart(Explore));

                Exploration.Start();
            }
        }

        private async void Explore()
        {
            try
            {
                while (true)
                {
                    int X = 0;
                    int Y = 0;

                    int DX = 0;
                    int DY = -1;

                    for (int i = 0; i < Settings.EXPLORATION_STEPS; i++)
                    {
                        var Destination = Specialized.Exploration.Helper.CalculateNextStep(Original.Lat, Original.Lng, Settings.EXPLORATION_STEPS, ref X, ref Y, ref DX, ref DY);

                        if (await Client.RequestMove(Destination.Lat, Destination.Lng))
                        {
                            gMapControl1.Overlays.FirstOrDefault().Markers.FirstOrDefault().Position = Destination;

                            var Objects = await Client.GetNearbyPokemons();

                            foreach (var Pokemon in Objects.Pokemons)
                            {
                                var Marker = Specialized.Pokemon.Utils.CreateMarker(Pokemon);

                                gMapControl1.Overlays.FirstOrDefault().Markers.Add(Marker);
                            }

                            foreach (var Fort in Objects.Forts)
                            {
                                if (Fort.FortType != 1)
                                {
                                    var Marker = Specialized.Forts.Utils.CreateMarker(Fort);

                                    gMapControl1.Overlays.FirstOrDefault().Markers.Add(Marker);
                                }
                            }
                        }

                        Thread.Sleep(5 * 1000);
                    }

                    Thread.Sleep(60 * 1000);

                    ClearMap();
                }
            }
            catch (Exception e)
            {
            }
        }

        private delegate void ClearCallback();

        private void ClearMap()
        {
            if (gMapControl1.InvokeRequired)
            {
                ClearCallback Callback = new ClearCallback(ClearMap);

                Invoke(Callback);
            }
            else
            {
                var Overlay = gMapControl1.Overlays.FirstOrDefault();

                if (Overlay != null)
                {
                    for (int i = 1; i < Overlay.Markers.Count; i++)
                    {
                        Overlay.Markers.RemoveAt(i);
                    }
                }
            }
        }

        private void CreateMap()
        {
            gMapControl1.DragButton = MouseButtons.Left;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            gMapControl1.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;

            gMapControl1.SetPositionByKeywords(Settings.STARTING_LOCATION);

            Original = gMapControl1.Position;

            GMapOverlay Overlay = new GMapOverlay("Markers");

            var Position = new GMarkerGoogle(gMapControl1.Position, GMarkerGoogleType.green);
            Position.ToolTipText = "Current Position";

            Overlay.Markers.Add(Position);

            gMapControl1.Overlays.Add(Overlay);
        }
    }
}
