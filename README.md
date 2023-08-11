# Eye 4 Eye Hint System

## About Repository
This repository contains scripts created for the dynamically layered Hint System in the puzzle adventure game, [Eye 4 Eye](https://store.steampowered.com/app/2269450/Eye_4_Eye/), written in C# for Unity.

All scripts were written by me!

## Design Overview
Given that Eye 4 Eye is primarily a puzzle game, we felt it would be appropriate to create a hint system to make the game accessible to a wider audience.

Since the game takes place in a Victorian mansion with puzzles isolated in separate rooms, hints for every puzzle were tracked with the help of triggers associated with each room. If the player entered a room for the first time, the hint associated with that room would be added to a list of active hints managed by the _HintManager.cs_ script. Every subsequent entrances into the room while the associated puzzle is not completed would prompt the aforementioned manager script to set the associated hint as the current hint to keep track of.

In the initial iteration of the hint system, the game had a timer, which when crossing a certain treshold (a time specified by the Eye 4 Eye designers on the Unity Editor), prompted the playable character, Hazel, to say some dialogue to nudge the player one step closer to solving the puzzle. Additionally, players were also given control to request for a hint by pressing "__H__" on the keyboard.

After multiple rounds of playtesting, we came to realize that having hints operate on a timer took agency away from the player and re-furbished the timer to display a prompt in the HUD instead. This prompt reminded the player that they can request a hint if needed and allowed the players to learn.