using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.GeneratedCode;
using PokemonGo.RocketAPI.Logic.Utils;
using PokemonGo.RocketAPI.Helpers;

namespace PokemonGo.RocketAPI.Logic.Utils
{
    class Statistics
    {
        public static int _totalExperience;
        public static int _totalPokemons;
        public static int _totalItemsRemoved;
        public static int _totalPokemonsTransfered;
        public static int _totalStardust;
        public static string _currentLevelInfos;
        public static int Currentlevel = -1;

        public static DateTime _initSessionDateTime = DateTime.Now;

        public static double _getSessionRuntime()
        {
            return ((DateTime.Now - _initSessionDateTime).TotalSeconds) / 3600;
        }

        public void addExperience(int xp)
        {
            _totalExperience += xp;
        }

        public static async Task<string> _getcurrentLevelInfos(Inventory _inventory)
        {
            var stats = await _inventory.GetPlayerStats();
            var output = string.Empty;
            PlayerStats stat = stats.FirstOrDefault();
            if (stat != null)
            {
                var _ep = (stat.NextLevelXp - stat.PrevLevelXp) - (stat.Experience - stat.PrevLevelXp);
                var _hours = Math.Round(_ep / (_totalExperience / _getSessionRuntime()),2);

                output = $"{stat.Level} (LvLUp in {_hours}hours // EXP required: {_ep})";
            }
            return output;
        }

        public void increasePokemons()
        {
            _totalPokemons += 1;
        }

        public void getStardust(int stardust)
        {
            _totalStardust = stardust;
        }

        public void addItemsRemoved(int count)
        {
            _totalItemsRemoved += count;
        }

        public void increasePokemonsTransfered()
        {
            _totalPokemonsTransfered += 1;
        }

        public async void updateConsoleTitle(Inventory _inventory)
        {
            _currentLevelInfos = await _getcurrentLevelInfos(_inventory);
            Console.Title = ToString();
        }

        public override string ToString()
        {           
            return string.Format("{0} - LvL: {1:0}    EXP/H: {2:0.0} EXP   P/H: {3:0.0} Pokemon(s)   Stardust: {4:0}   Pokemon Transfered: {5:0}   Items Removed: {6:0}", "Statistics", _currentLevelInfos, _totalExperience / _getSessionRuntime(), _totalPokemons / _getSessionRuntime(), _totalStardust, _totalPokemonsTransfered, _totalItemsRemoved);
        }
    }
}