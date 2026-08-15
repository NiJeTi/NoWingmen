# NoWingmen

A [BepInEx](https://github.com/BepInEx/BepInEx) plugin for **Nuclear Option** that adds different visualising
options for your wingmen and friends.

> [!NOTE]
> It only has visual effect thus can be used by any client without affecting anything on the server.

## Features

- **Wing** — mark any player of your faction as a wingman with a hotkey (`P` by default).
- **Friends** — automatically mark your Steam friends flying for the same faction.
- **Target selection** — highlight enemies your wing (or the whole team) has already selected.
- **Lock prevention** — skip enemies already selected by your wing when cycling targets (`O` to toggle).

## Screenshots

Marks on the HUD:

![HUD](./assets/wingman_hud.jpg)

Marks and lines on the map:

![Map](./assets/wingman_map.jpg)

## Installation

1. Install [BepInEx 5](https://github.com/BepInEx/BepInEx) into your Nuclear Option folder.
2. Drop `NoWingmen.dll` into `BepInEx/plugins`.
3. Launch the game once to generate `BepInEx/config/NoWingmen.cfg`.

## License

[Apache-2.0](./LICENSE)
