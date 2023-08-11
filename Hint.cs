using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hint",menuName = "Hint/MainHint")]
public class Hint : ScriptableObject
{
    //public event Action<Hint> OnHintNotRequired;

    [HideInInspector] public HintData hintData = new HintData();

    public bool active = false;

    public bool completed = false;

    public List<SubHint> subHints = new List<SubHint>();

    public void TryEndHint()
    {
        int totalSubHints = subHints.Count;
        int subHintsCompleted = 0;

        foreach(SubHint subHint in subHints)
        {
            if(subHint.completed)
            {
                // The task is not yet completed because a subtask is not yet completed
                subHintsCompleted += 1;
            }
        }

        // Mark the main hint as complete either if all the subhints are completed or the last subhint is completed
        if(subHintsCompleted == totalSubHints || subHints[subHints.Count - 1].completed)
        {
            completed = true;
            active = false;
            hintData.completed = true;
            hintData.active = false;

            EventCenter.GetInstance().EventTrigger<Hint>(name,this);
            //OnHintNotRequired?.Invoke(this);
        }
    }

    private void OnEnable()
    {
        foreach(SubHint subHint in subHints)
        {
            subHint.mainHint = this;
            subHint.currentDialogueIndex = 0;
            subHint.timesTriggered = 0;

            subHint.subHintData.currentDialogueIndex = 0;
            subHint.subHintData.timesTriggered = 0;

            hintData.subHintData.Add(subHint.subHintData);
        }

        hintData.hintName = name;
    }

    private void OnDisable()
    {
        foreach(SubHint subHint in subHints)
        {
            subHint.completed = false;
            subHint.currentDialogueIndex = 0;
            subHint.timesTriggered = 0;
        }

        active = false;
        completed = false;
    }
}