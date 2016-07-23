using System.Collections.Generic;
using PokemonGo.RocketAPI.GeneratedCode;

namespace PokemonGO.Specialized.Protocol.Containers
{
    public class NearbyData
    {
        internal IEnumerable<FortData> Forts;
        internal IEnumerable<MapPokemon> Pokemons;

        public NearbyData(IEnumerable<FortData> Forts, IEnumerable<MapPokemon> Pokemons)
        {
            this.Forts = Forts;
            this.Pokemons = Pokemons;
        }
    }
}
