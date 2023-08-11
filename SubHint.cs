using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HintType {
    NONE,
    INTERACT,
    EXAMINE,
    PICKUP,
    CRAFT,
    SEARCH,
    KEYPAD_DISPLAY,
    KEYPAD_SUCCESS,
    NOTE_READ,
    NOTE_PICKUP,
    LOCK_INTERACT,
    LOCK_UNLOCK,
    FUSEBOX_ON
};

[CreateAssetMenu(fileName = "New Subhint",menuName = "Hint/SubHint")]
public class SubHint : ScriptableObject
{
    [HideInInspector] public Hint mainHint;

    [HideInInspector] public int currentDialogueIndex = 0;

    [HideInInspector] public int timesTriggered = 0;

    [HideInInspector] public SubHintData subHintData = new SubHintData();

    public bool completed = false;

    public HintType hintType = HintType.NONE;

    public int stepCount = 0;

    public bool markPreviousAsComplete = false;

    public List<string> hintDialogueNodes = new List<string>();

    private void UpdateCurrentDialogueIndex(bool buttonPressed)
    {
        if(timesTriggered > 0 && currentDialogueIndex < hintDialogueNodes.Count - 1)
        {
            if(timesTriggered >= stepCount || buttonPressed)
            {
                // Move to the next easier hint dialogue
                timesTriggered = 0;
                subHintData.timesTriggered = 0;

                currentDialogueIndex += 1;
                subHintData.currentDialogueIndex += 1;
            }
        }

        timesTriggered += 1;
        subHintData.timesTriggered += 1;
    }

    public void CompleteSubTask()
    {
        completed = true;
        subHintData.completed = true;

        if(markPreviousAsComplete)
        {
            // Mark the previous hints as complete if they are not complete
            int index = mainHint.subHints.IndexOf(this);

            for(int i = 0;i < index;i++)
            {
                SubHint currentSubHint = mainHint.subHints[i];
                if(!currentSubHint.completed)
                {
                    currentSubHint.completed = true;
                    currentSubHint.subHintData.completed = true;
                }
            }
        }

        mainHint.TryEndHint();
    }

    public string GetCurrentDialogue(bool buttonPressed)
    {
        UpdateCurrentDialogueIndex(buttonPressed);
        return hintDialogueNodes[currentDialogueIndex];
    }
}