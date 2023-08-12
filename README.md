# Eye 4 Eye Hint System

## About Repository
This repository contains scripts created for the dynamically layered hint system in the puzzle adventure game, [Eye 4 Eye](https://store.steampowered.com/app/2269450/Eye_4_Eye/), written in C# for Unity.

All scripts were written by me!

## Credits
Eye 4 Eye is developed by __Team Eye 4 Eye__ and published by __Yi Dai__.

## Design Overview
Given that Eye 4 Eye is primarily a puzzle game, we felt it would be appropriate to create a hint system to make the game accessible to a wider audience.

Since the game takes place in a Victorian mansion with puzzles isolated in separate rooms, hints for every puzzle were tracked with the help of triggers associated with each room. If the player entered a room for the first time, the hint associated with that room would be added to a list of active hints managed by the _HintManager.cs_ script. Every subsequent entrances into the room while the associated puzzle is incomplete would prompt the aforementioned manager script to set the associated hint as the current hint to keep track of.

The hint system also supports layered hints: hints that progressively get more explicit as more requests are made for that hint. As players continue to request for the same hint, the game would respond with different dialogue. This can be customized in-editor by designers, allowing them to tweak parameters that keep track of the number of times a dialogue has been triggered before moving on to the next dialogue in the list.

In terms of visualizing the whole system, it helped to associate hints to the individual puzzles and have "subhints" contain hints for subtasks related to that puzzle.

In the initial iteration of the hint system, the game had a timer, which after crossing a certain threshold (a time specified by the Eye 4 Eye designers on the Unity Editor), prompted the playable character, Hazel, to say some dialogue to nudge the player one step closer to solving the puzzle. Additionally, players were also given control to request for a hint by pressing "__H__" on the keyboard.

After multiple rounds of playtesting, we came to realize that having hints operate on a timer took agency away from the player and re-furbished the timer to display a prompt in the HUD instead. This prompt behaves as a reminder for the player that they can request a hint if needed.

## Scripts Overview
1. [__HintManager.cs__](https://github.com/vpeesapa/Eye4EyeHintSystem/blob/main/HintManager.cs): A __singleton__ class that manages the hint system by keeping track of the current hint, active hints, and completed hints. This is the only class that interacts with the rest of the game and also contains functionality to disable/enable the hint system wherever necessary.
2. [__HintTrigger.cs__](https://github.com/vpeesapa/Eye4EyeHintSystem/blob/main/HintTrigger.cs): A helper class that contains the list of hints associated with a particular room and its puzzles in the Victorian mansion. This class regularly interacts with the _HintManager_ class to maintain the hints that need to be tracked.
3. [__Hint.cs__](https://github.com/vpeesapa/Eye4EyeHintSystem/blob/main/Hint.cs): A __ScriptableObject__ class that contains information for the overall hints for a puzzle. This class keeps track of whether the hint is active and/or complete (both of which are booleans) and also contains a list of "subhints" pertaining to smaller tasks related to the puzzle.
4. [__SubHint.cs__](https://github.com/vpeesapa/Eye4EyeHintSystem/blob/main/SubHint.cs): A __ScriptableObject__ class that contains information for hints related to a subtask in a puzzle. This is the class that contains a list of dialogue nodes that the _HintManager_ class will use when interacting with the game's dialogue system. Additionally, this script also keeps track of whether the subtask has been completed or not.
5. [__HintPromptPanel.cs__](https://github.com/vpeesapa/Eye4EyeHintSystem/blob/main/HintPromptPanel.cs): A helper class that interacts with the game's UI system to display the reminder prompt when alerted by the _HintManager_ class.
6. [__HintData.cs__](https://github.com/vpeesapa/Eye4EyeHintSystem/blob/main/HintData.cs): A helper class that interacts with the game's save/load system and contains data from the _Hint_ class that must be saved.
7. [__SubHintData.cs__](https://github.com/vpeesapa/Eye4EyeHintSystem/blob/main/SubHintData.cs): A helper class that interacts with the game's save/load system and contains data from the _SubHint_ class that must be saved.