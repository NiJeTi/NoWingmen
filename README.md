# NoWingmen

A [BepInEx](https://github.com/BepInEx/BepInEx) plugin for **Nuclear Option** that adds different visualising options
for teammates.

> [!NOTE]
> It only has visual effect thus can be used by any client without affecting anything on the server.

## Features

- **Wing** - mark any player of your faction as a wingman: pause, right-click them on the leaderboard, and pick *Toggle
  in wing*.
- **Teammates** - automatically mark every other player aircraft of your faction.
- **Target selection** - mark the enemies your wing and your teammates have already selected, and draw a line on the map
  from each of them to their target.
- **Wing screen** - a *WNG* page on the map's MFD: mod options and your wing roster.
- **Lock prevention** - skip enemies already selected by others when cycling targets. Toggled in
  flight by a hotkey.

Marks are color-coded and can be customized in game's **Accessibility** settings
in **Advanced** mode under **NoWingmen Marks** section:
![Color settings](./assets/color_settings.png)

The lock prevention hotkey is bound in the game's own **Controls** menu, under the **Gameplay** section:
![Controls](./assets/controls.png)

## Screenshots

Right-click menu and wing highlighting on the leaderboard:

![Leaderboard](./assets/leaderboard.png)

Marks on the HUD:

![HUD](./assets/wingman_hud.jpg)

Wing screen, marks and target lines on the map:

![Map](./assets/wingman_map.png)

## Installation

### Nuclear Option Mod Manager

1. Open [NOMM](https://github.com/Combat787/NOMM).
2. Search for `NoWingmen`.
3. Click toggle to install.

### Manual

1. Install [BepInEx 5](https://github.com/BepInEx/BepInEx) into your Nuclear Option folder.
2. Install [Extra Input Framework](https://github.com/Assassin1076/NuclearOptionInputFramework/releases) into
   `BepInEx/plugins`.
3. Put `NoWingmen.dll` into `BepInEx/plugins`.

## License

[Apache-2.0](./LICENSE)
