using System.Collections.Generic;
using UnityEngine;

public class CueClickable : MonoBehaviour, Clickable
{
    public string cueName;

    private static HashSet<string> cuesSeen;

    private void Start()
    {
        if (cuesSeen == null)
            cuesSeen = new HashSet<string>();
    }

    public bool IsClickable()
    {
        return true;
    }

    public bool IsFresh()
    {
        if (cuesSeen.Contains(cueName))
            return false;
        return true;
    }

    public void OnClick()
    {
        cuesSeen.Add(cueName);
        FindAnyObjectByType<Director>().ExecuteCue(cueName);
    }
}
