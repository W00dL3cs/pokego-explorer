using GMap.NET.WindowsForms.Markers;
using PokemonGo.RocketAPI.GeneratedCode;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGO.Specialized.Pokemon
{
    public static class Extensions
    {
        private static readonly DateTime Origin = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static int GetID(this MapPokemon Pokemon)
        {
            try
            {
                return (int)Pokemon.PokemonId;
            }
            catch { }

            return -1;
        }

        public static string GetName(this MapPokemon Pokemon)
        {
            try
            {
                return Pokemon.PokemonId.ToString();
            }
            catch { }

            return null;
        }

        public static GMarkerGoogle GetMarker(this MapPokemon Pokemon)
        {
            var Position = new GMap.NET.PointLatLng(Pokemon.Latitude, Pokemon.Longitude);

            var Result = new GMarkerGoogle(Position, (Bitmap)Bitmap.FromFile(string.Format("icons/{0}.png", Pokemon.GetID())));

            //Result.Size = new Size(Settings.POKEMON_IMAGE_SIZE, Settings.POKEMON_IMAGE_SIZE);

            Result.ToolTipText = MakeTooltip(Pokemon);

            return Result;
        }

        private static string MakeTooltip(MapPokemon Pokemon)
        {
            var Result = string.Format("{0} - #{1}{2}", Pokemon.GetName(), Pokemon.GetID(), Environment.NewLine);

            Result += string.Format("Latitude: {0}. {3}Longitude: {1}. {2}", Pokemon.Latitude, Pokemon.Longitude, Environment.NewLine, Environment.NewLine);

            Result += string.Format("Expiration date: {0}.", (Pokemon.ExpirationTimestampMs > 0) ? Origin.AddMilliseconds(Pokemon.ExpirationTimestampMs).ToLocalTime().ToShortTimeString() : "Never");

            return Result;
        }
    }
}
