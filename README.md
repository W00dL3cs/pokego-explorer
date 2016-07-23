# Pokemon GO Explorer

This repository contains a proof-of-concept of a Pokemon GO Explorer (for nearby Pokemons, Gyms and Pokestops) written in C#.

![alt tag](http://i.imgur.com/aQjtCtA.jpg)

___

# Features

  - [x] Pokemon Trainer Club (PTC) / Google Authentication
  - [x] Scan nearby area for Pokemons
  - [x] Change location using mouse (double-click or right-click)
  - [x] Updated API used for Client <-> Server communication
  - [x] Simple configuration file
  - [x] Pokemons Blacklist
  - [x] Pokemons Database
  - [ ] Advanced exploring algorithm
  
___

# Usage

  1. Set up the right dependencies (they're all in the `bin/Debug` folder)
  2. Edit the details contained in the file `Configuration` (found in the `bin/Debug` folder)
  3. Build the project!
  
#### Note

  When using Google Auth, remember the device code gets copied to your clipboard: just paste it when asked.

___

# Special thanks

  - Sodanakin for the wonderful GUI: https://www.reddit.com/user/sodanakin
  - FeroxRev for his Pokemon GO API: https://github.com/FeroxRev/Pokemon-Go-Rocket-API
  - GreatMaps for the Google Maps API implementation: https://greatmaps.codeplex.com/
  
___

# Notes

This is just a proof-of-concept... but the project is open source, so feel free to participate and push commits!
