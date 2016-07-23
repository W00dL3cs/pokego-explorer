using PokemonGo.RocketAPI.GeneratedCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGO.Specialized.Pokemon
{
    public class Manager
    {
        private List<int> Blacklist;
        private Dictionary<ulong, MapPokemon> Pokemons;

        public Manager(string Filter)
        {
            Blacklist = new List<int>();
            Pokemons = new Dictionary<ulong, MapPokemon>();

            InitFilter(Filter);
        }

        private void InitFilter(string Filter)
        {
            try
            {
                var Data = Filter.Split(',');

                foreach (var Pokemon in Data)
                {
                    int ID;

                    if (int.TryParse(Pokemon, out ID))
                    {
                        Blacklist.Add(ID);
                    }
                }
            }
            catch { }
        }

        internal bool AddPokemon(MapPokemon Pokemon)
        {
            try
            {
                Pokemons.Add(Pokemon.EncounterId, Pokemon);

                return (!Blacklist.Contains(Pokemon.GetID()));
            }
            catch { }

            return false;
        }

        internal void Clear()
        {
            Pokemons.Clear();

            // TODO: Insert data into database
        }
    }
}
