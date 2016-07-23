using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.GeneratedCode;
using GMap.NET.WindowsForms.Markers;
using System.Drawing;

namespace PokemonGO.Specialized.Forts
{
    public static class Utils
    {
        internal static GMarkerGoogle CreateMarker(FortData Fort)
        {
            var Position = new GMap.NET.PointLatLng(Fort.Latitude, Fort.Longitude);

            var Result = new GMarkerGoogle(Position, (Bitmap)Bitmap.FromFile(string.Format("forts/{0}.png", (Fort.Type == AllEnum.FortType.Gym) ? (int)Fort.OwnedByTeam : 4)));

            Result.ToolTipText = MakeTooltip(Fort);

            return Result;
        }

        private static string MakeTooltip(FortData Fort)
        {
            var Result = string.Format("Owned by: {0}{1}", GetTeamName((int)Fort.OwnedByTeam), Environment.NewLine);

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
