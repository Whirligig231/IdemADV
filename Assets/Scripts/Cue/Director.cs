using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour
{
    public TextAsset cueList;

    private Dictionary<string, List<string>> cueData;
    private Dictionary<string, Viewpoint> viewpoints;

    private List<string> currentCueData;
    private int currentCueIndex = 0;
    private Taskable currentCueTask;

    private void Start()
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
    }

    public void ExecuteCue(string cueName)
    {
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
            }
        }
        else if (cueLine.Contains(':'))
        {
            // Dialogue line
            int colonPosition = cueLine.IndexOf(':');
            string cueName = cueLine.Substring(0, colonPosition);
            string cueText = cueLine.Substring(colonPosition + 1).Trim();

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
