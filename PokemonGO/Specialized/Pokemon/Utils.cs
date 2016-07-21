using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PokemonGo.RocketAPI.GeneratedCode.MapObjectsResponse.Types.Payload.Types;
using PokemonGo.RocketAPI.GeneratedCode;
using System.Drawing;

namespace PokemonGO.Specialized.Pokemon
{
    public static class Utils
    {
        internal static GMarkerGoogle CreateMarker(MapPokemonProto Pokemon)
        {
            var ID = GetPokemonID(Pokemon.PokedexTypeId.ToString());

            var Position = new GMap.NET.PointLatLng(Pokemon.Latitude, Pokemon.Longitude);

            var Result = new GMarkerGoogle(Position, (Bitmap)Bitmap.FromFile(string.Format("icons/{0}.png", ID)));

            Result.Size = new Size(Settings.POKEMON_IMAGE_SIZE, Settings.POKEMON_IMAGE_SIZE);

            Result.ToolTipText = MakeTooltip(ID, Pokemon);

            return Result;
        }

        private static string MakeTooltip(int ID, MapPokemonProto Pokemon)
        {
            var Result = string.Format("{0} - #{1}{2}", GetPokemonName(Pokemon.PokedexTypeId.ToString()), ID, Environment.NewLine);

            Result += string.Format("Latitude: {0}. {3}Longitude: {1}. {2}", Pokemon.Latitude, Pokemon.Longitude, Environment.NewLine, Environment.NewLine);

            Result += string.Format("Expiration date: {0}.", (Pokemon.ExpirationTimeMs > 0) ? new DateTime(Pokemon.ExpirationTimeMs).ToShortTimeString() : "Never");

            return Result;
        }

        private static int GetPokemonID(string Data)
        {
            return int.Parse(Data.Substring(1, 4));
        }

        private static string GetPokemonName(string Data)
        {
            return Data.Substring(Data.LastIndexOf("Pokemon") + 7);
        }
    }
}
