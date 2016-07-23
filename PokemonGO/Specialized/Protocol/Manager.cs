using PokemonGO.Specialized.Protocol.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGO.Specialized.Protocol
{
    public class Manager
    {
        private Settings Details;
        private Authentication Auth;
        private PokemonGo.RocketAPI.Client Client;

        internal Manager(int Authentication, double Latitude, double Longitude)
        {
            Auth = (Authentication)Authentication;

            Details = new Settings(Latitude, Longitude);

            Client = new PokemonGo.RocketAPI.Client(Details);
        }

        internal async Task<bool> PerformLogin(string Username, string Password)
        {
            try
            {
                if (Auth == Authentication.PTC)
                {
                    await Client.DoPtcLogin(Username, Password);
                }
                else
                {
                    await Client.DoGoogleLogin();
                }

                await Client.SetServer();

                // TODO: Check if the order is correct... and in case, if these operations are really required at login

                await Client.GetProfile();
                await Client.GetSettings();
                await Client.GetMapObjects();
                await Client.GetInventory();

                return true; // TODO: Real check
            }
            catch (Exception e)
            { }

            return false;
        }

        internal async Task<NearbyData> GetNearbyData()
        {
            try
            {
                var Objects = await Client.GetMapObjects();

                var Pokemons = Objects.MapCells.SelectMany(x => x.CatchablePokemons);

                var Forts = Objects.MapCells.SelectMany(x => x.Forts);

                return new NearbyData(Forts, Pokemons);
            }
            catch { }

            return default(NearbyData);
        }

        internal async Task<bool> RequestMove(double Latitude, double Longitude)
        {
            try
            {
                var Result = await Client.UpdatePlayerLocation(Latitude, Longitude, 0);

                return (Result != null); // TODO: Real check
            }
            catch { }

            return false;
        }
    }
}
