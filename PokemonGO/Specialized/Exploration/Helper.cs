using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGO.Specialized.Exploration
{
    public static class Helper
    {
        internal static GMap.NET.PointLatLng CalculateNextStep(double Latitude, double Longitude, int Steps, ref int X, ref int Y, ref int DX, ref int DY)
        {
            double NEXT_X = 0;
            double NEXT_Y = 0;

            if ((X <= (Steps / 2) && X > (-Steps / 2)) && (Y <= (Steps / 2) && Y > (-Steps / 2)))
            {
                NEXT_X = (X * 0.0025) + Latitude;
                NEXT_Y = (Y * 0.0025) + Longitude;
            }

            if (X == Y || (X < 0 && X == -Y) || (X > 0 && X == 1 - Y))
            {
                Switch(ref DX, ref DY);

                DX *= -1;

                /*int Temp = DX;

                DX = -DY;
                DY = Temp;
                */
            }

            X += DX;
            Y += DY;

            return new GMap.NET.PointLatLng(NEXT_X, NEXT_Y);
        }

        private static void Switch(ref int First, ref int Second)
        {
            int Temp = First;

            First = Second;
            Second = Temp;
        }
    }
}