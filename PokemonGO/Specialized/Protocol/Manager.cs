using PokemonGO.Specialized.Protocol.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PokemonGo.RocketAPI.GeneratedCode.MapObjectsResponse.Types.Payload.Types;

namespace PokemonGO.Specialized.Protocol
{
    public class Manager
    {
        private PokemonGo.RocketAPI.Client Client;

        internal Manager(double Latitude, double Longitude)
        {
            Client = new PokemonGo.RocketAPI.Client(Latitude, Longitude);
        }

        internal async Task<bool> PerformLogin(string Username, string Password)
        {
            try
            {
                await Client.LoginPtc(Username, Password);

                var Result = await Client.GetServer();

                if (Result != null)
                {
                    // TODO: Check if the order is correct... and in case, if these operations are really required at login

                    await Client.GetProfile();
                    await Client.GetSettings();
                    await Client.GetMapObjects();
                    await Client.GetInventory();
                }

                return (Result != null); // TODO: Real check
            }
            catch { }

            return false;
        }

        internal async Task<NearbyData> GetNearbyData()
        {
            try
            {
                var Objects = await Client.GetMapObjects();

                var Pokemons = Objects.Payload[0].Profile.SelectMany(x => x.MapPokemon);

                var Forts = Objects.Payload[0].Profile.SelectMany(x => x.Fort);

                return new NearbyData(Forts, Pokemons);
            }
            catch { }

            return default(NearbyData);
        }

        internal async Task<bool> RequestMove(double Latitude, double Longitude)
        {
            try
            {
                var Result = await Client.UpdatePlayerLocation(Latitude, Longitude);

                return (Result != null); // TODO: Real check
            }
            catch { }

            return false;
        }
    }
}
