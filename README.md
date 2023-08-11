# Eye 4 Eye Hint System

## About Repository
This repository contains scripts created for the dynamically layered hint system in the puzzle adventure game, [Eye 4 Eye](https://store.steampowered.com/app/2269450/Eye_4_Eye/), written in C# for Unity.

All scripts were written by me!

## Credits
Eye 4 Eye is developed by __Team Glasses__ and published by __Yi Dai__.

## Design Overview
Given that Eye 4 Eye is primarily a puzzle game, we felt it would be appropriate to create a hint system to make the game accessible to a wider audience.

Since the game takes place in a Victorian mansion with puzzles isolated in separate rooms, hints for every puzzle were tracked with the help of triggers associated with each room. If the player entered a room for the first time, the hint associated with that room would be added to a list of active hints managed by the _HintManager.cs_ script. Every subsequent entrances into the room while the associated puzzle is incomplete would prompt the aforementioned manager script to set the associated hint as the current hint to keep track of.

The hint system also supports layered hints: hints that progressively get more explicit as more requests are made for that hint. As players continue to request for the same hint, the game would respond with different dialogue. This can be customized in-editor by designers, allowing them to tweak parameters that keep track of the number of times a dialogue has been triggered before moving on to the next dialogue in the list.

In the initial iteration of the hint system, the game had a timer, which after crossing a certain treshold (a time specified by the Eye 4 Eye designers on the Unity Editor), prompted the playable character, Hazel, to say some dialogue to nudge the player one step closer to solving the puzzle. Additionally, players were also given control to request for a hint by pressing "__H__" on the keyboard.

After multiple rounds of playtesting, we came to realize that having hints operate on a timer took agency away from the player and re-furbished the timer to display a prompt in the HUD instead. This prompt behaves as a reminder for the player that they can request a hint if needed.

## Scripts Overview
1. [HintManager.cs](https://github.com/vpeesapa/Eye4EyeHintSystem/blob/main/HintManager.cs)
2. [HintTrigger.cs](https://github.com/vpeesapa/Eye4EyeHintSystem/blob/main/HintTrigger.cs)
3. [Hint.cs](https://github.com/vpeesapa/Eye4EyeHintSystem/blob/main/Hint.cs)
4. [SubHint.cs](https://github.com/vpeesapa/Eye4EyeHintSystem/blob/main/SubHint.cs)
5. [HintPromptPanel.cs](https://github.com/vpeesapa/Eye4EyeHintSystem/blob/main/HintPromptPanel.cs)
6. [HintData.cs](https://github.com/vpeesapa/Eye4EyeHintSystem/blob/main/HintData.cs)
7. [SubHintData.cs](https://github.com/vpeesapa/Eye4EyeHintSystem/blob/main/SubHintData.cs)