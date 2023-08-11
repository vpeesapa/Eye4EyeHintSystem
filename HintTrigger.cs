using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintTrigger : MonoBehaviour
{
    [Header("List of all the hints related to the room")]
    [SerializeField] private List<Hint> hints;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            int numCompletedTasks = 0;

            foreach(Hint hint in hints)
            {
                if(!hint.completed)
                {
                    if(!hint.active)
                    {
                        HintManager.GetInstance().AddNewHint(hint);
                    }
                    else
                    {
                        HintManager.GetInstance().SetCurrentHint(hint);
                    }

                    break;
                }

                // If a hint was completed even before it was made active
                if(!HintManager.GetInstance().activeHints.Contains(hint) && !HintManager.GetInstance().completedHints.Contains(hint))
                {
                    HintManager.GetInstance().AddNewHint(hint);
                }

                numCompletedTasks += 1;
            }

            if(numCompletedTasks >= hints.Count)
            {
                // Disable the trigger when all the puzzles associated with the room are solved
                gameObject.SetActive(false);
            }
        }
    }
}
