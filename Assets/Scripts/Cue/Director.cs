using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour
{
    public TextAsset cueList;

    private Dictionary<string, List<string>> cueData;
    private Dictionary<string, Viewpoint> viewpoints;
    private Dictionary<string, CuedAction> actions;

    private List<string> currentCueData;
    private int currentCueIndex = 0;
    private Taskable currentCueTask;

    private void Awake()
    {
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
            }
            else
            {
                currentCueData.Add(trimmedLine);
            }
        }

        // Load the viewpoints
        viewpoints = new Dictionary<string, Viewpoint>();
        foreach (Viewpoint viewpoint in FindObjectsByType<Viewpoint>(FindObjectsSortMode.None))
        {
            viewpoints[viewpoint.name] = viewpoint;
        }

        // Load the actions
        actions = new Dictionary<string, CuedAction>();
        foreach (CuedAction action in FindObjectsByType<CuedAction>(FindObjectsSortMode.None))
        {
            actions[action.name] = action;
        }
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
                    FindAnyObjectByType<MusicManager>().ChangeMusic(cueParam);
                    currentCueTask = null;
                    break;
                case "Stage":
                    {
                        string[] personNames = cueParam.Split(',');
                        Stage stage = FindAnyObjectByType<Stage>();
                        if (personNames.Length == 0)
                            stage.FadeStageIn("", "", "");
                        else if (personNames.Length == 1)
                            stage.FadeStageIn("", personNames[0].Trim(), "");
                        else if (personNames.Length == 2)
                            stage.FadeStageIn(personNames[0].Trim(), "", personNames[1].Trim());
                        else
                            stage.FadeStageIn(personNames[0].Trim(), personNames[1].Trim(), personNames[2].Trim());
                        currentCueTask = stage;
                        break;
                    }
                case "Unstage":
                    {
                        Stage stage = FindAnyObjectByType<Stage>();
                        stage.FadeStageOut();
                        currentCueTask = stage;
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
