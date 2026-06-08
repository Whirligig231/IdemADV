using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Director : MonoBehaviour
{
    public TextAsset cueList;

    private Dictionary<string, List<string>> cueData;

    private static string startCue;
    private string defaultStartCue;

    private Dictionary<string, Viewpoint> viewpoints;
    private Dictionary<string, CuedAction> actions;
    private Dictionary<string, ToggleObject> toggles;

    private List<string> currentCueData;
    private int currentCueIndex = 0;
    private Taskable currentCueTask;

    private void Awake()
    {
        StringBuilder debugInfoBuilder = new StringBuilder();
        debugInfoBuilder.AppendLine("Director loading!");
        debugInfoBuilder.AppendLine("\nCues:");

        // Load the cue list
        cueData = new Dictionary<string, List<string>>();

        string cueListAll = cueList.text;
        string[] lines = cueListAll.Split('\n');
        List<string> currentCueData = null;
        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();
            if (trimmedLine.Length == 0)
                continue;
            else if (trimmedLine[0] == '#')
            {
                string cueName = trimmedLine.Substring(2);
                if (!cueData.ContainsKey(cueName))
                    cueData[cueName] = new List<string>();
                currentCueData = cueData[cueName];
                debugInfoBuilder.AppendLine("- " + cueName);
            }
            else if (trimmedLine[0] == '/')
                continue;
            else
            {
                currentCueData.Add(trimmedLine);
            }
        }

        // Load the viewpoints
        debugInfoBuilder.AppendLine("\nViewpoints:");
        viewpoints = new Dictionary<string, Viewpoint>();
        foreach (Viewpoint viewpoint in FindObjectsByType<Viewpoint>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            viewpoints[viewpoint.name] = viewpoint;
            debugInfoBuilder.AppendLine("- " + viewpoint.name);
        }

        // Load the actions
        debugInfoBuilder.AppendLine("\nActions:");
        actions = new Dictionary<string, CuedAction>();
        foreach (CuedAction action in FindObjectsByType<CuedAction>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            actions[action.name] = action;
            debugInfoBuilder.AppendLine("- " + action.name);
        }

        // Load the toggles
        debugInfoBuilder.AppendLine("\nToggles:");
        toggles = new Dictionary<string, ToggleObject>();
        foreach (ToggleObject toggle in FindObjectsByType<ToggleObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            toggles[toggle.name] = toggle;
            debugInfoBuilder.AppendLine("- " + toggle.name);
        }

        Debug.Log(debugInfoBuilder.ToString());
    }

    private void Start()
    {
        if (startCue != null)
            ExecuteCue(startCue);
        else if (defaultStartCue != null)
            ExecuteCue(defaultStartCue);

        startCue = null;
    }

    public void SetDefaultStartCue(string startCue)
    {
        defaultStartCue = startCue;
    }

    public void ExecuteCue(string cueName)
    {
        if (cueName == "")
            return;

        currentCueData = cueData[cueName];
        currentCueIndex = 0;
        ProcessCue();
    }

    private void ProcessCue()
    {
        if (currentCueIndex >= currentCueData.Count)
            return;
        string cueLine = currentCueData[currentCueIndex];
        if (cueLine[0] == '%')
        {
            int spacePosition = cueLine.IndexOf(' ');
            if (spacePosition < 0)
                spacePosition = cueLine.Length;
            string cueType = cueLine.Substring(1, spacePosition - 1);
            string cueParam = "";
            if (spacePosition < cueLine.Length)
                cueParam = cueLine.Substring(spacePosition + 1);

            switch (cueType)
            {
                case "Nop":
                    currentCueTask = null;
                    break;
                case "Player":
                    FindAnyObjectByType<PlayerCamera>().SetToPlayer();
                    currentCueTask = null;
                    break;
                case "Viewpoint":
                    Viewpoint viewpoint = viewpoints[cueParam];
                    FindAnyObjectByType<PlayerCamera>().SetToViewpoint(viewpoint);
                    viewpoint.StartTimer();
                    currentCueTask = viewpoint;
                    break;
                case "Action":
                    CuedAction action = actions[cueParam];
                    action.Invoke();
                    currentCueTask = action;
                    break;
                case "FadeIn":
                    {
                        FadeQuad fadeQuad = FindAnyObjectByType<FadeQuad>();
                        fadeQuad.FadeIn();
                        currentCueTask = fadeQuad;
                    }
                    break;
                case "FadeOut":
                    {
                        FadeQuad fadeQuad = FindAnyObjectByType<FadeQuad>();
                        fadeQuad.FadeOut();
                        currentCueTask = fadeQuad;
                    }
                    break;
                case "CutIn":
                    {
                        FadeQuad fadeQuad = FindAnyObjectByType<FadeQuad>();
                        fadeQuad.CutIn();
                        currentCueTask = fadeQuad;
                    }
                    break;
                case "CutOut":
                    {
                        FadeQuad fadeQuad = FindAnyObjectByType<FadeQuad>();
                        fadeQuad.CutOut();
                        currentCueTask = fadeQuad;
                    }
                    break;
                case "Music":
                    FindAnyObjectByType<MusicManager>().ChangeMusic(cueParam, false);
                    currentCueTask = null;
                    break;
                case "MusicCut":
                    FindAnyObjectByType<MusicManager>().ChangeMusic(cueParam, true);
                    currentCueTask = null;
                    break;
                case "Stage":
                    {
                        ProcessStageCue(cueParam, false);
                        break;
                    }
                case "StageInScene":
                    {
                        ProcessStageCue(cueParam, true);
                        break;
                    }
                case "Unstage":
                    {
                        Stage stage = FindAnyObjectByType<Stage>();
                        stage.FadeStageOut();
                        currentCueTask = stage;
                        break;
                    }
                case "Video":
                    {
                        VideoManager video = FindAnyObjectByType<VideoManager>();
                        video.LoadVideo(cueParam);
                        currentCueTask = video;
                        break;
                    }
                case "Next":
                    {
                        string[] names = cueParam.Split('\\');
                        if (names.Length == 1 && cueData.ContainsKey(names[0].Trim()))
                        {
                            ExecuteCue(names[0].Trim());
                            return;
                        }
                        else
                        {
                            if (names.Length >= 2)
                                startCue = names[1].Trim();
                            SceneManager.LoadScene(names[0].Trim());
                            return;
                        }
                    }
                case "ToggleOn":
                    {
                        ToggleObject toggle = toggles[cueParam];
                        toggle.gameObject.SetActive(true);
                        currentCueTask = null;
                        break;
                    }
                case "ToggleOff":
                    {
                        ToggleObject toggle = toggles[cueParam];
                        toggle.gameObject.SetActive(false);
                        currentCueTask = null;
                        break;
                    }
                case "Dialog":
                    {
                        ProcessDialogCue(cueParam);
                        break;
                    }
            }
        }
        else if (cueLine.Contains(':'))
        {
            // Dialogue line
            int colonPosition = cueLine.IndexOf(':');
            string cueNameMood = cueLine.Substring(0, colonPosition);
            string cueText = cueLine.Substring(colonPosition + 1).Trim();

            string cueName = cueNameMood, cueMood = null;
            if (cueNameMood.Contains('('))
            {
                int leftPosition = cueNameMood.IndexOf('(');
                int rightPosition = cueNameMood.IndexOf(')');
                cueName = cueNameMood.Substring(0, leftPosition).Trim();
                cueMood = cueNameMood.Substring(leftPosition + 1, rightPosition - leftPosition - 1).Trim();
            }

            Stage stage = FindAnyObjectByType<Stage>();
            if (cueMood != null)
            {
                stage.SetMood(cueName, cueMood);
            }

            Textbox textbox = FindAnyObjectByType<Textbox>();
            string cueNameDisplay = cueName;
            float cueSpeed = 0.05f;
            if (cueName == "UI")
            {
                cueNameDisplay = "";
                cueSpeed = 0.02f;
            }
            textbox.DisplayText(cueNameDisplay, cueText, cueSpeed); // TODO: More advanced processing
            currentCueTask = textbox;
        }
    }

    private void ProcessDialogCue(string cueParam)
    {
        string[] texts = cueParam.Split('\\');
        string dialogTitle, optionL = null, optionC = null, optionR = null,
            nextL = null, nextC = null, nextR = null;
        if (texts.Length < 5)
        {
            dialogTitle = texts[0].Trim();
            optionC = texts[1].Trim();
            nextC = texts[2].Trim();
        }
        else if (texts.Length < 7)
        {
            dialogTitle = texts[0].Trim();
            optionL = texts[1].Trim();
            nextL = texts[2].Trim();
            optionR = texts[3].Trim();
            nextR = texts[4].Trim();
        }
        else
        {
            dialogTitle = texts[0].Trim();
            optionL = texts[1].Trim();
            nextL = texts[2].Trim();
            optionC = texts[3].Trim();
            nextC = texts[4].Trim();
            optionR = texts[5].Trim();
            nextR = texts[6].Trim();
        }

        DialogBox dialog = FindAnyObjectByType<DialogBox>();
        if (nextL != null)
        {
            UnityEvent eventL = new UnityEvent();
            eventL.AddListener(delegate { ExecuteCue(nextL); });
            dialog.SetCallback(0, eventL);
        }
        if (nextC != null)
        {
            UnityEvent eventC = new UnityEvent();
            eventC.AddListener(delegate { ExecuteCue(nextC); });
            dialog.SetCallback(1, eventC);
        }
        if (nextR != null)
        {
            UnityEvent eventR = new UnityEvent();
            eventR.AddListener(delegate { ExecuteCue(nextR); });
            dialog.SetCallback(2, eventR);
        }

        dialog.ShowDialogBox(dialogTitle, optionL, optionC, optionR);
        currentCueTask = dialog;
    }

    private void ProcessStageCue(string cueParam, bool putInMainLayer)
    {
        string[] personNames = cueParam.Split(',');
        string[][] personNamesMarkers = new string[personNames.Length][];
        for (int i = 0; i < personNames.Length; i++)
        {
            personNamesMarkers[i] = personNames[i].Split('@');
        }

        string nameL = "", markerL = "", nameC = "", markerC = "", nameR = "", markerR = "";

        if (personNames.Length == 1)
        {
            nameC = personNamesMarkers[0][0].Trim();
            if (personNamesMarkers[0].Length > 1)
                markerC = personNamesMarkers[0][1].Trim();
        }
        else if (personNames.Length == 2)
        {
            nameL = personNamesMarkers[0][0].Trim();
            if (personNamesMarkers[0].Length > 1)
                markerL = personNamesMarkers[0][1].Trim();
            nameR = personNamesMarkers[1][0].Trim();
            if (personNamesMarkers[1].Length > 1)
                markerR = personNamesMarkers[1][1].Trim();
        }
        else if (personNames.Length >= 3)
        {
            nameL = personNamesMarkers[0][0].Trim();
            if (personNamesMarkers[0].Length > 1)
                markerL = personNamesMarkers[0][1].Trim();
            nameC = personNamesMarkers[1][0].Trim();
            if (personNamesMarkers[1].Length > 1)
                markerC = personNamesMarkers[1][1].Trim();
            nameR = personNamesMarkers[2][0].Trim();
            if (personNamesMarkers[2].Length > 1)
                markerR = personNamesMarkers[2][1].Trim();
        }

        Stage stage = FindAnyObjectByType<Stage>();
        stage.FadeStageIn(nameL, nameC, nameR, markerL, markerC, markerR, putInMainLayer);
        if (putInMainLayer)
            currentCueTask = null;
        else
            currentCueTask = stage;
    }

    public bool IsRunningCue()
    {
        return currentCueData != null && currentCueIndex < currentCueData.Count;
    }

    private void Update()
    {
        PlayerMovement playerMovement = FindAnyObjectByType<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = !IsRunningCue();
        }

        if (IsRunningCue())
        {
            if (currentCueTask == null || currentCueTask.HasFinished())
            {
                currentCueIndex++;
                ProcessCue();
            }
        }
    }
}
