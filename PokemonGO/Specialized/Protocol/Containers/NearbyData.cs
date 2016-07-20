using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.GeneratedCode;
using static PokemonGo.RocketAPI.GeneratedCode.MapObjectsResponse.Types.Payload.Types;

namespace PokemonGO.Specialized.Protocol.Containers
{
    public class NearbyData
    {
        internal IEnumerable<PokemonFortProto> Forts;
        internal IEnumerable<MapPokemonProto> Pokemons;

        public NearbyData(IEnumerable<PokemonFortProto> Forts, IEnumerable<MapPokemonProto> Pokemons)
        {
            this.Forts = Forts;
            this.Pokemons = Pokemons;
        }
    }
}
