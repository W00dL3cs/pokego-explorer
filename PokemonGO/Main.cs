using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using PokemonGO.Specialized.Pokemon;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace PokemonGO
{
    public partial class Main : Form
    {
        private PointLatLng Position;
        private Specialized.Protocol.Manager Client;
        private Specialized.Pokemon.Manager Pokemons;
        private Specialized.Configuration.Manager Configuration;

        private bool IsExploring;
        private Thread Exploration;

        private delegate void ClearCallback();
        private delegate void MarkerCallback(GMarkerGoogle Marker);
        private delegate void WriteCallback(string Line, bool Date);
        
        public Main()
        {
            InitializeComponent();

            Init();
        }

        private void Init()
        {
            WriteLine("Pokemon GO Explorer");
            WriteLine("Copyright (C) 2016 - W00dL3cs");
            WriteLine();
            
            Configuration = new Specialized.Configuration.Manager("Configuration");

            Pokemons = new Specialized.Pokemon.Manager(Configuration.GetValue<string>("POKEMON_BLACKLIST"));
        }

        private void WriteLine()
        {
            WriteLine("", false);
        }

        private void WriteLine(string Line = "", bool Date = true)
        {
            if (txtHistory.InvokeRequired)
            {
                var Callback = new WriteCallback(WriteLine);

                Invoke(Callback, Line, Date);
            }
            else
            {
                txtHistory.AppendText(string.Format(((Date) ? "[{0}] - " : "") + "{1}{2}", DateTime.Now.ToShortTimeString(), Line, Environment.NewLine));
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CreateMap();

            AttemptLogin();
        }

        private void CreateMap()
        {
            gMapControl1.ShowCenter = false;
            gMapControl1.GrayScaleMode = true;
            gMapControl1.DragButton = MouseButtons.Left;
            GMaps.Instance.Mode = AccessMode.ServerOnly;

            gMapControl1.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            gMapControl1.SetPositionByKeywords(Configuration.GetValue<string>("STARTING_LOCATION"));

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
                
                Client = new Specialized.Protocol.Manager(Configuration.GetValue<int>("LOGIN_AUTH"), gMapControl1.Position.Lat, gMapControl1.Position.Lng);

                if (!await Client.PerformLogin(Configuration.GetValue<string>("LOGIN_USERNAME"), Configuration.GetValue<string>("LOGIN_PASSWORD")))
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

        private void AddMarker(GMarkerGoogle Marker)
        {
            try
            {
                if (gMapControl1.InvokeRequired)
                {
                    var Callback = new MarkerCallback(AddMarker);

                    Invoke(Callback, Marker);
                }
                else
                {
                    gMapControl1.Overlays.FirstOrDefault().Markers.Add(Marker);
                }
            }
            catch { }
        }

        private async void Explore()
        {
            var x = 0;
            var y = 0;
            var dx = 0;
            var dy = -1;

            IsExploring = true;

            var Step = Configuration.GetValue<int>("STEP_DELAY");
            var Clear = Configuration.GetValue<int>("CLEAR_DELAY");
            var Limit = Configuration.GetValue<int>("EXPLORATION_STEPS");
            
            try
            {
                for (int i = 1; i <= Limit; i++)
                {
                    var Destination = Specialized.Exploration.Helper.CalculateNextStep(Position.Lat, Position.Lng, Limit, ref x, ref y, ref dx, ref dy);

                    if (await Client.RequestMove(Destination.Lat, Destination.Lng))
                    {
                        WriteLine(string.Format("[#{0}] Moving to: ({1},{2}).", i, Destination.Lat, Destination.Lng));

                        SetPlayerPosition(Destination);

                        var Objects = await Client.GetNearbyData();

                        if (Objects != null)
                        {
                            foreach (var Pokemon in Objects.Pokemons)
                            {
                                if (Pokemons.AddPokemon(Pokemon))
                                {
                                    AddMarker(Pokemon.GetMarker());
                                }
                            }

                            foreach (var Marker in Objects.Forts.Where(Fort => Fort.Type == AllEnum.FortType.Gym).Select(Specialized.Forts.Utils.CreateMarker))
                            {
                                AddMarker(Marker);
                            }
                        }
                    }

                    Thread.Sleep(Step * 1000);
                }

                WriteLine(string.Format("Waiting {0} seconds before cleaning map...", Clear));

                Thread.Sleep(Clear * 1000);

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
                    Pokemons.Clear();

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

            WriteLine(string.Format("Scanning started! Number of steps: {0}.", Configuration.GetValue<int>("EXPLORATION_STEPS")));

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

        #region GUI
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x84:
                    base.WndProc(ref m);
                    if ((int)m.Result == 0x1)
                        m.Result = (IntPtr)0x2;
                    return;
            }
            base.WndProc(ref m);
        }

        private void Ex_Click(object sender, EventArgs e) //EXIT_APP
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e) //SquirBlue
        {
            this.BackgroundImage = Properties.Resources._7a50045ab03c115d698fb9f533f90f1c;
            Exit_Button.BackgroundImage = Properties.Resources._7a50045ab03c115d698fb9f533f90f1c;
            PokePoke.BackgroundImage = Properties.Resources._7a50045ab03c115d698fb9f533f90f1c;

            MapBorder.BackColor = Color.MediumTurquoise;
            label1.BackColor = Color.MediumTurquoise;
            txtHistory.BackColor = Color.MediumTurquoise;
            btnLogin.BackColor = Color.MediumTurquoise;
            btnPause.BackColor = Color.MediumTurquoise;
        }

        private void button2_Click(object sender, EventArgs e) //CharRed
        {
            this.BackgroundImage = Properties.Resources.e815a787fb770107c34238b202c40a1c;
            Exit_Button.BackgroundImage = Properties.Resources.e815a787fb770107c34238b202c40a1c;
            PokePoke.BackgroundImage = Properties.Resources.e815a787fb770107c34238b202c40a1c;

            MapBorder.BackColor = Color.Coral;
            label1.BackColor = Color.Coral;
            txtHistory.BackColor = Color.Coral;
            btnLogin.BackColor = Color.Coral;
            btnPause.BackColor = Color.Coral;
        }

        private void button3_Click(object sender, EventArgs e) //BulbGreen
        {
            this.BackgroundImage = Properties.Resources.f60536429bb5c705c7427136c92cea84;
            Exit_Button.BackgroundImage = Properties.Resources.f60536429bb5c705c7427136c92cea84;
            PokePoke.BackgroundImage = Properties.Resources.f60536429bb5c705c7427136c92cea84;

            MapBorder.BackColor = Color.LightGreen;
            label1.BackColor = Color.LightGreen;
            txtHistory.BackColor = Color.LightGreen;
            btnLogin.BackColor = Color.LightGreen;
            btnPause.BackColor = Color.LightGreen;
        }

        private void PokePoke_Click(object sender, EventArgs e)
        {
            if (label1.Visible == true)
            {
                label1.Visible = false;
            }
            else
            {
                label1.Visible = true;
            };
        }
        #endregion
    }
}
