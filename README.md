# NoWingmen

A [BepInEx](https://github.com/BepInEx/BepInEx) plugin for **Nuclear Option** that adds different visualising
options for teammates.

> [!NOTE]
> It only has visual effect thus can be used by any client without affecting anything on the server.

## Features

- **Wing** — mark any player of your faction as a wingman with a hotkey.
- **Friends** — automatically mark your Steam friends flying for the same faction.
- **Teammates** — automatically mark every other player aircraft of your faction.
- **Target selection** — highlight enemies your teammates have already selected.
- **Lock prevention** — skip enemies already selected by others when cycling targets.

Hotkeys are bound in the game's own **Controls** menu, under the **Gameplay** section:
![Controls](./assets/controls.png)

## Screenshots

Marks on the HUD:

![HUD](./assets/wingman_hud.jpg)

Marks and lines on the map:

![Map](./assets/wingman_map.jpg)

## Installation

1. Install [BepInEx 5](https://github.com/BepInEx/BepInEx) into your Nuclear Option folder.
2. Install [Extra Input Framework](https://github.com/Assassin1076/NuclearOptionInputFramework/releases) into `BepInEx/plugins`.
4. Put `NoWingmen.dll` into `BepInEx/plugins`.

## License

[Apache-2.0](./LICENSE)
