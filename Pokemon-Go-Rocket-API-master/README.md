# Pokemon-Go-Rocket-API

![alt tag](https://github.com/Spegeli/Pokemon-Go-Rocket-API/blob/master/Screenshot.png)

A Pokemon Go bot in C#

## About

Chat about this Repository via Discord: https://discord.gg/VsVrjgr

**GitHub issues will be deleted if they are not API related.**

## Features
* PTC Login / Google
* Use Humanlike Walking with 10 km/h (instead of Teleport)
* Farm Pokestops
* Farm all Pokemon near your
* Transfer duplicate Pokemon (keep the best of everyone) (ignore favorite marked) (ignore Pokemons from NotToTransfer list configurable via Settings.cs)
* Evolve Pokemon (only on StartUp) (Pokemon Type configurable via Settings.cs)
* Use best Pokeball & Berry (depending on Pokemon CP)
* Random Task Delays
* Throws away unneeded items (configurable via Settings.cs)
* Statistic in the Header:
![alt tag](https://github.com/Spegeli/Pokemon-Go-Rocket-API/blob/master/StatisticScreenshot.png)
* Very color and useful Logging (so you every time up2date what currently happened)
* and many more ;-)

## ToDo
* Auto Update the Bot

## Setting it up
Note: You need some basic Computer Expierience, if you need help somewhere, ask the community and do not spam us via private messages. **The Issue Tracker is not for help!**


1. Download and Install [Visual Studio 2015](https://go.microsoft.com/fwlink/?LinkId=691979&clcid=0x407)
2. Download [this Repository](https://github.com/NecronomiconCoding/Pokemon-Go-Bot/archive/master.zip)
3. Open Pokemon Go Rocket API.sln
4. On the right hand side, double click on UserSettings.settings
5. Enter the DefaultLatitude and DefaultLongitude [can be found here](http://mondeca.com/index.php/en/any-place-en)
6. Select the AuthType (Google or Ptc for Pokémon Trainer Club)
7. If selected Ptc , enter the Username and Password of your Account
8. Right click on PokemonGo.RocketAPI.Console and Set it as Startup Project
9. Press CTRL + F5 and follow the Instructions
10. Have fun! 

## License
This Project is licensed as GNU (GNU GENERAL PUBLIC LICENSE v3) 

You can find all necessary Information [here](https://github.com/NecronomiconCoding/Pokemon-Go-Bot/blob/master/LICENSE.md)

## Credits
Thanks to Ferox hard work on the API & Console we are able to manage something like this. Without him that would have been nothing. <3

Thanks to everyone who contributed via Pull Requests!