# Pokemon GO Explorer

This repository contains a proof-of-concept of a Pokemon GO Explorer (for nearby Pokemons, Gyms and Pokestops) written in C#.


___

# Usage

  - Set up the right dependencies
  - Edit the details contained in `Settings.cs`:
  ```
  internal static readonly string PTC_USERNAME = "Your PTC Username";
  internal static readonly string PTC_PASSWORD = "Your PTC Password";
  
  internal static readonly int EXPLORATION_STEPS = 30; // Number of steps (increase this to explore a wider area)
  
  internal static readonly string STARTING_LOCATION = "Starting Location"; // Example: Time Square, New York City
  ```
  - Build the project!
