# Changelog

## [1.1.0] - 2025-12-21

### Added

- Add "tournament" menu to main menu for custom tourney game modes ([`4e6837a`](https://github.com/avgduck/TourneyMod/commit/4e6837a77385be51c5194591fe0d884e0f760658))
- Add default ruleset `all_stages` with forced free pick mode ([`002a853`](https://github.com/avgduck/TourneyMod/commit/002a853859f6e59c7b12f4b00cace64a4b9905cb))
- Extend custom stage select screen to be used in all game modes with ruleset `all_stages` ([`002a853`](https://github.com/avgduck/TourneyMod/commit/002a853859f6e59c7b12f4b00cace64a4b9905cb))
- Add ruleset option to enable random stage select ([`ad440cf`](https://github.com/avgduck/TourneyMod/commit/ad440cf20cc7c2f13a061792fb3dd28f6dcc380c))
- Enable quickly choosing random stage select with player "menu" input (start button on controller) ([`7056ba8`](https://github.com/avgduck/TourneyMod/commit/7056ba8984b9fcc7a9003cd14f5f233b1a0ff3bf))
- Add currently selected ruleset preview UI in [ModMenu](https://thunderstore.io/c/lethal-league-blaze/p/MrGentle/ModMenu/) ([`bf939f3`](https://github.com/avgduck/TourneyMod/commit/bf939f3f7b449f22ed0937f9b1f1ca50d19232a0))
- Add menu sound effects to custom stage select UI ([`0f55d6b`](https://github.com/avgduck/TourneyMod/commit/0f55d6bf714a779f1b8bc74af44a7428d6b6a4e1))
- Display stage sizes for 2D stages ([`45650a0`](https://github.com/avgduck/TourneyMod/commit/45650a0c6fc8683a6a2436395334e0ccbd29757b))
- Add logging to stage strike process ([`e461261`](https://github.com/avgduck/TourneyMod/commit/e4612613a9e41b114196f415f653e5b2f80d052e))
- Add selected and played characters to the set tracker match history and logs ([`966f3b7`](https://github.com/avgduck/TourneyMod/commit/966f3b7799a9766362fef987642d9a1e3ac24f7e))
- Add dependency on [CursorSpeed](https://thunderstore.io/c/lethal-league-blaze/p/Daioutzu/CursorSpeed/) mod ([`6bd046d`](https://github.com/avgduck/TourneyMod/commit/6bd046d93cef442b1eed582e9d0fcfc4b405d9a9))
- Add skip XP/currency animations post-match feature from [QuickRematch](https://thunderstore.io/c/lethal-league-blaze/p/Daioutzu/QuickRematch/) ([`0c0f930`](https://github.com/avgduck/TourneyMod/commit/0c0f9304862efff10c8dd77ecf1ee92573389033))

### Removed

- Remove snapping cursors to center screen when stage strike UI opens ([`b5cbc75`](https://github.com/avgduck/TourneyMod/commit/b5cbc758a0d4af4952e693dbcae9b916e22b87f5))
- Remove verbose description of all available rulesets in [ModMenu](https://thunderstore.io/c/lethal-league-blaze/p/MrGentle/ModMenu/) ([`bf939f3`](https://github.com/avgduck/TourneyMod/commit/bf939f3f7b449f22ed0937f9b1f1ca50d19232a0))

### Changed

- **Breaking:** Change ruleset `id` to be set from the ruleset file name instead of a JSON field ([`f167bcc`](https://github.com/avgduck/TourneyMod/commit/f167bccdec81c05b679d3021fd32f952c83fb2f4))
- Move stage striking and set count overlay functionality to custom "local 1v1" tourney game mode ([`9f04433`](https://github.com/avgduck/TourneyMod/commit/9f04433de6965c2aff7fcc53c5f98123b8f91360))
- Rework stage strike layouts to support up to 17 total stages (all 3D and 2D vanilla stages) in rulesets ([`ad440cf`](https://github.com/avgduck/TourneyMod/commit/ad440cf20cc7c2f13a061792fb3dd28f6dcc380c))
- Change displayed stage names on custom stage select screen to those from [LLBModdingLib](https://thunderstore.io/c/lethal-league-blaze/p/Glomzubuk/LLBModdingLib/) ([`d1b3aa6`](https://github.com/avgduck/TourneyMod/commit/d1b3aa614cd131858481f8fa8440eee16cfc2062))
- Enable leaving stage strike menu with player "back" input ([`7056ba8`](https://github.com/avgduck/TourneyMod/commit/7056ba8984b9fcc7a9003cd14f5f233b1a0ff3bf))

### Fixed

- Fix stage select UI interaction area not matching the visible stage buttons ([`59f9514`](https://github.com/avgduck/TourneyMod/commit/59f95144f203cf4251f58c812364cf5387c7717b))
- Fix player's "reset set count" vote remaining after player leaves lobby ([`e8b6790`](https://github.com/avgduck/TourneyMod/commit/e8b679082db0312dec8e18fb80ae7db28b8d62f8))
- Prevent duplicate rulesets from loading ([`f167bcc`](https://github.com/avgduck/TourneyMod/commit/f167bccdec81c05b679d3021fd32f952c83fb2f4))
- Prevent loading when incompatible [StageSelect](https://thunderstore.io/c/lethal-league-blaze/p/Daioutzu/StageSelect/) mod is installed ([`6bd046d`](https://github.com/avgduck/TourneyMod/commit/6bd046d93cef442b1eed582e9d0fcfc4b405d9a9))

## [1.0.1] - 2025-11-15

### Fixed

- Fix ruleset directory structure in package ([`930c4db`](https://github.com/avgduck/TourneyMod/commit/930c4db72952ae9bd5d9d7083ac9adb9c3ce6ea7))

## [1.0.0] - 2025-11-15

_Initial release_

[1.1.0]: https://github.com/avgduck/TourneyMod/releases/tag/v1.1.0
[1.0.1]: https://github.com/avgduck/TourneyMod/releases/tag/v1.0.1
[1.0.0]: https://github.com/avgduck/TourneyMod/releases/tag/v1.0.0