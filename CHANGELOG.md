# Changelog

## [1.2.0] - 2026-02-02

### Added

- Add local doubles tourney game mode ([`5e03cec`](https://github.com/avgduck/TourneyMod/commit/5e03cecd640da3f73d3e1c50cfaad3f493f88658))
- Add crew battle tourney game mode ([`91d6b52`](https://github.com/avgduck/TourneyMod/commit/91d6b52d6a46676add4d7bd68f0d58a303be1ec3))
- Add configs to select different ruleset for each tourney game mode ([`e404e2a`](https://github.com/avgduck/TourneyMod/commit/e404e2a5cb279f35c1b63699790049d9bf060d7b))
- Add custom ruleset preview screen to tournament menu ([`aa0af14`](https://github.com/avgduck/TourneyMod/commit/aa0af14c15efb7effa99a6e64d9dee76a9229e28))
- Add current set overview screen to tournament menu ([`6039748`](https://github.com/avgduck/TourneyMod/commit/60397488d817c35c4d4ee91c58c35a4b0b9ef959))
- Add forced game options to rulesets ([`e9805c6`](https://github.com/avgduck/TourneyMod/commit/e9805c6a875fe9d29057b5ec3fc1173d78a67d87))
- Add automatic winner character lock and add options to rulesets ([`40b1065`](https://github.com/avgduck/TourneyMod/commit/40b10655fbaed86c29959fb9a6b2397e238be535))
- Add automatic timeout handling and tiebreaker matches ([`b516de1`](https://github.com/avgduck/TourneyMod/commit/b516de1edaf69c9786f66b9dc008f84197ff1dc3))
- Add score override menu to tourney lobby screen ([`8ffb38e`](https://github.com/avgduck/TourneyMod/commit/8ffb38e62031bf8b56a6d52b190395610380aedb))
- Display team color on player cursors ([`c2cbd2b`](https://github.com/avgduck/TourneyMod/commit/c2cbd2bb74646da0d585157163fc8b1071235f57))
- Add dependency on [CharacterReroll](https://thunderstore.io/c/lethal-league-blaze/p/avg_duck/CharacterReroll/) mod ([`a9fb8fe`](https://github.com/avgduck/TourneyMod/commit/a9fb8fef3db6d82ab217152b3a803dd2190a04d4))

### Removed

- Remove ruleset preview in ModMenu ([`674f11b`](https://github.com/avgduck/TourneyMod/commit/674f11b9f36c0d4bd3c77709835f6f6aa964aebe))
- Remove vote reset button on tourney lobby screen ([`8ffb38e`](https://github.com/avgduck/TourneyMod/commit/8ffb38e62031bf8b56a6d52b190395610380aedb))

### Changed

- Enable tourney set persisting after leaving lobby screen ([`c13f7af`](https://github.com/avgduck/TourneyMod/commit/c13f7af097da7ff33e5220f4da27b91545dab154))
- Block entering other game modes when tourney set is active ([`c13f7af`](https://github.com/avgduck/TourneyMod/commit/c13f7af097da7ff33e5220f4da27b91545dab154))
- Change set count text on lobby screen to use team color ([`3680604`](https://github.com/avgduck/TourneyMod/commit/3680604d1b8983883aa439c32d18546292530533))

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

[1.2.0]: https://github.com/avgduck/TourneyMod/releases/tag/v1.2.0
[1.1.0]: https://github.com/avgduck/TourneyMod/releases/tag/v1.1.0
[1.0.1]: https://github.com/avgduck/TourneyMod/releases/tag/v1.0.1
[1.0.0]: https://github.com/avgduck/TourneyMod/releases/tag/v1.0.0