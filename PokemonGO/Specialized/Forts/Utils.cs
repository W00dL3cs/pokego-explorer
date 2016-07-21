using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.GeneratedCode;
using GMap.NET.WindowsForms.Markers;
using static PokemonGo.RocketAPI.GeneratedCode.MapObjectsResponse.Types.Payload.Types;
using System.Drawing;

namespace PokemonGO.Specialized.Forts
{
    public static class Utils
    {
        internal static GMarkerGoogle CreateMarker(PokemonFortProto Fort)
        {
            var Position = new GMap.NET.PointLatLng(Fort.Latitude, Fort.Longitude);

            var Result = new GMarkerGoogle(Position, (Bitmap)Bitmap.FromFile(string.Format("forts/{0}.png", (Fort.FortType != 1) ? Fort.Team : 4)));

            Result.ToolTipText = MakeTooltip(Fort);

            return Result;
        }

        private static string MakeTooltip(PokemonFortProto Fort)
        {
            var Result = string.Format("Owned by: {0}{1}", GetTeamName(Fort.Team), Environment.NewLine);

            Result += string.Format("Prestige: {0}.", Fort.GymPoints);

            return Result;
        }

        private static string GetTeamName(int Team)
        {
            if (Team == 1)
            {
                return "Team Mystic";
            }
            else if (Team == 2)
            {
                return "Team Valor";
            }
            else if (Team == 3)
            {
                return "Team Instinct";
            }

            return "None";
        }
    }
}
