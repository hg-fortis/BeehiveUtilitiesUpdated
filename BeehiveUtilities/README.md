# Beehive Utilities
A few customisation options for beehives including amount of honey a hive can hold, time it takes to generate the honey and disabling the proximity build restrictions

## Manual Installation
To install this mod, you need to have BepInEx. After installing BepInEx, extract BeehiveUtilities.dll into games install **"\Valheim\BepInEx\plugins"**
IF you're getting an error in console when enabling "Show Alternate Text" or "Display Bee Status On Hover" extract all the files in **core** into **"\Valheim\BepInEx\core"**

## Config
Before the config file is generated, you must run the game with the mod installed. The config file is located at **"\Valheim\BepInEx\config\smallo.mods.beehiveutilities.cfg"**

#### There are serveral config options available;

| Config Option | Type | Default Value | Description |
|:-------------:|:-----------:|:-----------:|:-----------|
| Enable Mod | bool | true | Enable or disable the mod |
| Disable Proximity Check | bool | true | Disables the "Bees need more space" check |
| Show Alternate Text | bool | true | Show the amount of honey generated next to the Beehive name "Honey ( 0/4 )" instead of "( EMPTY )" or "( Honey x honeyAmount )" |
| Max Honey | int | 4 | The maximum amount of honey a beehive can generate (default is 4) |
| Minutes Per Creation | double | 20.0 | The amount of minutes it takes to generate 1 piece of honey (default is 20) |
| Remove Biome Check | bool | false | Allows beehives to work in any biome |
| Remove Night/Rain Check | bool | false | Allows beehives to work at night and during rain |
| Display Bee Status On Hover | bool | true | Shows the bee status on hover instead of having to press E, this also shows when there is honey in the hive |
| Spawn Honey In Front | bool | false | Spawns the honey in front of the hive instead of on top of it. Here is a picture to show which way is the front of the hive, it's the side where the thatch overlaps from the sides on top. [Example](https://i.imgur.com/zK5FT47.png) |

If you have any suggestions, feel free to let me know!

## Changelog

#### v1.1.1;
* Updated to be compatible with latest version of Valheim
* Fixed text color showing "<color=lime>" instead of the actual color
* Fixed bug where disabling show alternate text would still show if Bee Status was enabled

#### v1.1.0;
* Made 'Minutes Per Creation' a double instead of an int so you can do less then a minute
* Added a config option to remove the biome check to allow bees anywhere
* Added a config option to allow bees to continue to produce during the rain and at night
* Added a config option to show the status of the bees on hover instead of pressing 'E' when the hive is empty
* Added a config option to spawen the honey in front of the hive instead of on top of it

#### v1.0.1;
* Readme Update

#### v1.0.0;
* Initial release

## Images

![Showing](https://fivem.fail/gta5/Ped/SetIkTarget/c7X1yAKfOe.png)

[Discord Support](https://discord.gg/pTGSu8R7DW)