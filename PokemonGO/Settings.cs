using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGO
{
    internal static class Settings
    {
        internal static readonly string PTC_USERNAME = "Your PTC Username";
        internal static readonly string PTC_PASSWORD = "Your PTC Password";

        internal static readonly int EXPLORATION_STEPS = 30; // Number of steps (increase this to explore a wider area)
        internal static readonly int STEP_DELAY = 1; // Seconds between steps
        internal static readonly int CLEAR_DELAY = 30; // Seconds between clearing map

        internal static readonly string STARTING_LOCATION = "Starting Location"; // Example: Time Square, New York City

        internal static readonly int POKEMON_IMAGE_SIZE = 40;
    }
}
