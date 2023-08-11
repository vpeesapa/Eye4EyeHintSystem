using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    /*
     * For testing only: show hints inside inspector, otherwise have them hidden
     */
    public Hint currentHint;
    public List<Hint> activeHints;
    public List<Hint> completedHints;

    [HideInInspector] public bool enabledInSettings = true;

    [Header("Time (in seconds) for the next hint to be triggered by the timer")]
    [SerializeField] private float hintTimeLimit;

    [Header("Time (in seconds) the hint key should be disabled for after requesting for a hint")]
    [SerializeField] private float buttonPressTimeLimit;

    [Header("Time limit (in seconds) for the hint prompt popup")]
    [SerializeField] private float hintPromptTimeLimit = 25;

    private float currentTimer;
    private bool isTimerRunning;
    private bool pressedHintThisFrame;
    private bool displayingPrompt;
    private bool hintTriggered;

    private const string hintPromptPanelPath = "Hint/HintPrompt";

    private static HintManager instance;

    private void OnEnable()
    {
        EventCenter.GetInstance().AddEventListener("PressedHint",PressedHint);

        /* The following code is for debugging purposes only */

        if(activeHints.Count > 0)
        {
            foreach(Hint activeHint in activeHints)
            {
                activeHint.active = true;
                EventCenter.GetInstance().AddEventListener<Hint>(activeHint.name,RemoveHint);
                //activeHint.OnHintNotRequired += RemoveHint;
            }

            currentHint = activeHints[0];
        }
    }

    private void OnDisable()
    {
        EventCenter.GetInstance().RemoveEventListener("PressedHint",PressedHint);

        /* The following code is for debugging purposes only */

        if(activeHints.Count > 0)
        {
            foreach (Hint activeHint in activeHints)
            {
                activeHint.active = false;
                EventCenter.GetInstance().RemoveEventListener<Hint>(activeHint.name,RemoveHint);
                //activeHint.OnHintNotRequired -= RemoveHint;

                foreach(SubHint subHint in activeHint.subHints)
                {
                    subHint.completed = false;
                }
            }

            activeHints.Clear();
        }

        if(completedHints.Count > 0)
        {
            foreach(Hint completedHint in completedHints)
            {
                completedHint.active = false;
                completedHint.completed = false;

                foreach(SubHint subHint in completedHint.subHints)
                {
                    subHint.completed = false;
                }
            }

            completedHints.Clear();
        }
    }

    private void Awake()
    {
        if(instance == null)
        {
            // Only one instance of HintManager is ever required, so making it into a singleton
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        currentTimer = 0.0f;
        isTimerRunning = false;
        pressedHintThisFrame = false;
        displayingPrompt = false;
        hintTriggered = false;

        DisableHintSystem(PlayerPrefs.GetInt("hintenable", 1) == 0);
        enabledInSettings = PlayerPrefs.GetInt("hintenable", 1) == 1;

        SaveLoadManager saveLoadInstance = SaveLoadManager.GetInstance();
        if(saveLoadInstance != null)
        {
            if(!saveLoadInstance.newGameStarted)
            {
                Hint[] allHints = Resources.FindObjectsOfTypeAll<Hint>();
                foreach(Hint hint in allHints)
                {
                    if(saveLoadInstance.CurrentHintDataExists())
                    {
                        HintData currentHintData = saveLoadInstance.LoadCurrentHintData();

                        if(currentHintData.hintName == hint.name)
                        {
                            hint.hintData = currentHintData;
                            hint.active = currentHintData.active;
                            hint.completed = currentHintData.completed;

                            for(int i = 0;i < currentHintData.subHintData.Count;i++)
                            {
                                hint.subHints[i].subHintData = currentHintData.subHintData[i];
                                hint.subHints[i].completed = currentHintData.subHintData[i].completed;
                                hint.subHints[i].timesTriggered = currentHintData.subHintData[i].timesTriggered;
                                hint.subHints[i].currentDialogueIndex = currentHintData.subHintData[i].currentDialogueIndex;
                            }

                            currentHint = hint;
                        }
                    } 

                    if(saveLoadInstance.ActiveHintDataExists(hint.name))
                    {
                        HintData activeHintData = saveLoadInstance.LoadActiveHintsData(hint.name);

                        hint.hintData = activeHintData;
                        hint.active = activeHintData.active;
                        hint.completed = activeHintData.completed;

                        for(int i = 0;i < activeHintData.subHintData.Count;i++)
                        {
                            hint.subHints[i].subHintData = activeHintData.subHintData[i];
                            hint.subHints[i].completed = activeHintData.subHintData[i].completed;
                            hint.subHints[i].timesTriggered = activeHintData.subHintData[i].timesTriggered;
                            hint.subHints[i].currentDialogueIndex = activeHintData.subHintData[i].currentDialogueIndex;
                        }
                        
                        EventCenter.GetInstance().AddEventListener<Hint>(hint.name,RemoveHint);

                        activeHints.Add(hint);
                    }
                    else if(saveLoadInstance.CompletedHintDataExists(hint.name))
                    {
                        HintData completedHintData = saveLoadInstance.LoadCompletedHintsData(hint.name);

                        hint.hintData = completedHintData;
                        hint.active = completedHintData.active;
                        hint.completed = completedHintData.completed;

                        for(int i = 0;i < completedHintData.subHintData.Count;i++)
                        {
                            hint.subHints[i].subHintData = completedHintData.subHintData[i];
                            hint.subHints[i].completed = completedHintData.subHintData[i].completed;
                            hint.subHints[i].timesTriggered = completedHintData.subHintData[i].timesTriggered;
                            hint.subHints[i].currentDialogueIndex = completedHintData.subHintData[i].currentDialogueIndex;
                        }

                        completedHints.Add(hint);
                    }
                }

                hintTriggered = saveLoadInstance.GetHintTriggered();
            }
        }
    }

    private void Update()
    {
        if(!pressedHintThisFrame && InputMgr.GetInstance().hint.WasPressedThisFrame())
        {
            // Reset the hint system timer every time the player requests for a hint
            EventCenter.GetInstance().EventTrigger("PressedHint");

            TriggerDialogue(true);
        }
    }

    private void FixedUpdate()
    {
        if(isTimerRunning)
        {
            currentTimer += Time.fixedDeltaTime;

            if(currentTimer >= hintTimeLimit)
            {
                //TriggerDialogue(false);
                if(!displayingPrompt)
                {
                    Debug.LogError("Show hint prompt");

                    // Display prompt
                    UIMgr.GetInstance().ShowPanel<HintPromptPanel>(hintPromptPanelPath,null,E_UI_Layer.POPUP);
                    displayingPrompt = true;
                    currentTimer = 0.0f;
                }
            }

            if(currentTimer >= hintPromptTimeLimit)
            {
                if(displayingPrompt)
                {
                    Debug.LogError("Hide hint prompt");

                    // Hide the prompt
                    UIMgr.GetInstance().HidePanel(hintPromptPanelPath);
                    displayingPrompt = false;
                    currentTimer = 0.0f;
                }
            }
        }
    }

    private void OnApplicationQuit()
    {
        ResetHints();
    }

    // Function to add a new hint when the player enters the room for the first time
    public void AddNewHint(Hint newHint)
    {
        newHint.active = true;
        newHint.hintData.active = true;

        EventCenter.GetInstance().AddEventListener<Hint>(newHint.name,RemoveHint);
        //newHint.OnHintNotRequired += RemoveHint;

        // Check if the hint is not required since all of its tasks have already been completed
        newHint.TryEndHint();

        // If there are some subtasks remaining, add the hint into the list of active hints and make it the current hint to track
        if(!newHint.completed)
        {
            activeHints.Add(newHint);
            currentHint = newHint;
            
            if(!isTimerRunning)
            {
                currentTimer = 0.0f;
                isTimerRunning = true;
            }

            if(SaveLoadManager.GetInstance() != null)
            {
                SaveLoadManager.GetInstance().AddActiveHintData(newHint.name,newHint.hintData);
                SaveLoadManager.GetInstance().AddCurrentHintData(currentHint.hintData);
            }
        }
    }

    // Function to set the current hint depending on where the player is
    public void SetCurrentHint(Hint hint)
    {
        currentHint = hint;

        if(SaveLoadManager.GetInstance() != null)
        {
            SaveLoadManager.GetInstance().AddCurrentHintData(currentHint.hintData);
        }
    }

    // Function to disable the hint system wherever necessary (e.g. when UI elements are open)
    public void DisableHintSystem(bool disable)
    {
        if(enabledInSettings)
        {
            if(disable)
            {
                // Disable timer
                isTimerRunning = false;
                InputMgr.GetInstance().hint.Disable();

                UIMgr.GetInstance().HidePanel(hintPromptPanelPath);
            }
            else
            {
                InputMgr.GetInstance().hint.Enable();

                if (currentHint != null)
                {
                    // Enable timer
                    isTimerRunning = true;

                    if(displayingPrompt)
                    {
                        UIMgr.GetInstance().ShowPanel<HintPromptPanel>(hintPromptPanelPath,null,E_UI_Layer.POPUP);
                    }
                }
            }
        }
    }

    public void ResetHints()
    {
        if(activeHints.Count > 0)
        {
            foreach(Hint activeHint in activeHints)
            {
                activeHint.active = false;
                EventCenter.GetInstance().RemoveEventListener<Hint>(activeHint.name,RemoveHint);
                //activeHint.OnHintNotRequired -= RemoveHint;

                foreach(SubHint subHint in activeHint.subHints)
                {
                    subHint.completed = false;
                    subHint.currentDialogueIndex = 0;
                    subHint.timesTriggered = 0;
                }
            }

            activeHints.Clear();
        }

        if(completedHints.Count > 0)
        {
            foreach(Hint completedHint in completedHints)
            {
                completedHint.active = false;
                completedHint.completed = false;

                foreach(SubHint subHint in completedHint.subHints)
                {
                    subHint.completed = false;
                    subHint.currentDialogueIndex = 0;
                    subHint.timesTriggered = 0;
                }
            }

            completedHints.Clear();
        }

        currentHint = null;
        currentTimer = 0;
        displayingPrompt = false;
        UIMgr.GetInstance().HidePanel(hintPromptPanelPath);
    }

    public bool GetHintTriggered()
    {
        return hintTriggered;
    }

    public static HintManager GetInstance()
    {
        return instance;
    }

    // Function to trigger a hint through dialogue when requested by the player
    private void TriggerDialogue(bool buttonPressed)
    {
        if(currentHint != null)
        {
            foreach(SubHint subHint in currentHint.subHints)
            {
                if(!subHint.completed)
                {
                    // Set the hint dialogue to whatever subtask hadn't been completed yet
                    if(subHint.hintDialogueNodes.Count > 0)
                    {
                        // Check if dialogue is running
                        if(!DialogueManager.GetInstance().isDialogueRunning)
                        {
                            currentTimer = 0.0f;
                            
                            UIMgr.GetInstance().HidePanel(hintPromptPanelPath);
                            displayingPrompt = false;

                            hintTriggered = true;
                            
                            if(SaveLoadManager.GetInstance() != null)
                            {
                                SaveLoadManager.GetInstance().SetHintTriggered(true);
                            }

                            DialogueManager.GetInstance().TriggerDialogue(subHint.GetCurrentDialogue(buttonPressed));
                        }
                        
                        // complete button prompt
                        if (ControlsUI.Instance) {
                            ControlsUI.Instance.FinishPrompt("Hint");
                        }
                    }

                    break;
                }
            }
        }
    }

    // Function that is triggered by the event listener to remove the current hint after completion
    private void RemoveHint(Hint hint)
    {
        SaveLoadManager saveLoadInstance = SaveLoadManager.GetInstance();

        if(currentHint == hint)
        {
            currentHint = null;
            isTimerRunning = false;
            displayingPrompt = false;
            UIMgr.GetInstance().HidePanel(hintPromptPanelPath);

            if(saveLoadInstance != null)
            {
                saveLoadInstance.AddCurrentHintData(null);
            }
        }

        // Unsubscribe the event listener since the "task" has been completed
        EventCenter.GetInstance().RemoveEventListener<Hint>(hint.name,RemoveHint);
        //hint.OnHintNotRequired -= RemoveHint;

        // Remove task from the list of active hints and put it in the list of completed hints
        activeHints.Remove(hint);

        completedHints.Add(hint);

        if(saveLoadInstance != null)
        {
            saveLoadInstance.RemoveActiveHintData(hint.name);
            saveLoadInstance.AddCompletedHintData(hint.name,hint.hintData);
        }

        if(activeHints.Count > 0)
        {
            currentHint = activeHints[0];
            currentTimer = 0.0f;
            isTimerRunning = true;

            if (saveLoadInstance != null)
            {
                saveLoadInstance.AddCurrentHintData(currentHint.hintData);
            }
        }
    }

    // Functions to prevent the player from continuously spamming the hint button
    private void PressedHint()
    {
        StartCoroutine(PressedHintBlock());
    }

    private IEnumerator PressedHintBlock()
    {
        pressedHintThisFrame = true;
        yield return new WaitForSeconds(buttonPressTimeLimit);
        pressedHintThisFrame = false;
    }

    public void AttemptActivation()
    {
        if(enabledInSettings)
        {
            DisableHintSystem(false);
        }
    }
}