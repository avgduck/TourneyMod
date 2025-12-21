# [TourneyMod](https://thunderstore.io/c/lethal-league-blaze/p/avg_duck/TourneyMod/)

Adds many tournament-related features for Tournament Organizers and players! Currently designed for LAN tournaments only (local 1v1 mode), with support for online tournaments planned for a future version.

_Note: This mod is NOT compatible with [Daioutzu's StageSelect](https://thunderstore.io/c/lethal-league-blaze/p/Daioutzu/StageSelect/)!_

## General Features

### Redesigned Stage Select

All game modes from training to ranked now feature a completely reimagined stage select screen! It's navigated using the player cursors, powered by the resolution-based cursor speed fixes in [Daioutzu's CursorSpeed](https://thunderstore.io/c/lethal-league-blaze/p/Daioutzu/CursorSpeed/).

![Custom stage select preview](https://raw.githubusercontent.com/avgduck/TourneyMod/master/preview-images/custom%20stage%20select.gif)

For quicker friendly matches and training, pressing the "menu" input (START button on controllers) will instantly select the random 3D stage option.

### Tournament Game Modes

Custom tournament game modes now have a dedicated home on the main menu! This enables the game to be played normally outside of tournament settings.

![Tourney menu preview](https://raw.githubusercontent.com/avgduck/TourneyMod/master/preview-images/tourney%20mode%20menu.gif)

The menu currently features a Local 1v1 tourney mode, with previews for planned doubles and crew battle modes as well as online support!

## Tourney Mode Features

### Dynamic Set Count

Manually keep track of game wins no longer! TourneyMod keeps track of what game in the set the players are on, who won each set and on what stage, and displays the current set score!

![Dynamic set count preview](https://raw.githubusercontent.com/avgduck/TourneyMod/master/preview-images/dynamic%20set%20count.gif)

A new set can be started by creating a new lobby, or by voting via the reset button.

### Integrated Stage Striking

Have trouble remembering the legal stage list? Avoid stage striking because it takes too long outside the game? Never again! Stage striking is now fully integrated inside the custom stage select screen!

![Stage banning preview](https://raw.githubusercontent.com/avgduck/TourneyMod/master/preview-images/stage%20banning.gif)

This system automatically bans counterpick stages on game 1, and utilizes the set tracker to automatically process DSR bans! It supports specifying a legal stage list of any number of neutral/counterpick stages as well as a  custom ban order tailored to your tournament!

In case of a ruleset error or a Gentleman's agreement, players can also vote to toggle Free Pick mode, bypassing the ban system!

![Free Pick mode preview](https://raw.githubusercontent.com/avgduck/TourneyMod/master/preview-images/free%20pick%20mode.gif)

Free Pick Mode preserves the set history, so it can be toggled back off to restore all calculated and manual bans.

### Custom Rulesets

The legal stage list as well as striking rules are specified in their own ruleset files, and can quickly be swapped between in the mod config or with [MrGentle's ModMenu](https://thunderstore.io/c/lethal-league-blaze/p/MrGentle/ModMenu/). ModMenu will show a list of all loaded rulesets as well as a preview of the currently selected ruleset for easy reference!

![ModMenu preview](https://raw.githubusercontent.com/avgduck/TourneyMod/master/preview-images/ModMenu%20preview.gif)

The mod comes packaged with a set of default rulesets, but by creating a new file you can create any number of your own rulesets! A [guide to ruleset files](https://github.com/avgduck/TourneyMod/wiki/Custom-Rulesets) can be found on the wiki.

## Planned Features

TourneyMod is planned to be expanded to enhance the entire tournament experience. This includes:

* Local doubles tourney mode
* Local crew battle mode
* Online support for tourney mode striking and set tracking
* Online resuming with saved stock counts after desync

and more! Be on the lookout for future updates, and feel free to submit any bug reports or feature requests to the [TourneyMod issues page](https://github.com/avgduck/TourneyMod/issues).

[![Common Changelog](https://common-changelog.org/badge.svg)](https://common-changelog.org)