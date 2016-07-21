# Pokemon GO Explorer

This repository contains a proof-of-concept of a Pokemon GO Explorer (for nearby Pokemons, Gyms and Pokestops) written in C#.

![alt tag](http://i.imgur.com/aQjtCtA.jpg)


___

# Usage

  - Set up the right dependencies (they're all in the Debug folder)
  - Edit the details contained in `Settings.cs`:
  ```
  internal static readonly string PTC_USERNAME = "Your PTC Username";
  internal static readonly string PTC_PASSWORD = "Your PTC Password";
  
  internal static readonly int EXPLORATION_STEPS = 30; // Number of steps (increase this to explore a wider area)
  
  internal static readonly string STARTING_LOCATION = "Starting Location"; // Example: Time Square, New York City
  ```
  - Build the project!
  

___

# Special thanks

  - Sodanakin for the wonderful GUI: https://www.reddit.com/user/sodanakin
  - FeroxRev for his Pokemon GO API: https://github.com/FeroxRev/Pokemon-Go-Rocket-API
  - GreatMaps for the Google Maps API implementation: https://greatmaps.codeplex.com/
  
___

# Notes

This is just a proof-of-concept... but the project is open source, so feel free to participate and push commits!
