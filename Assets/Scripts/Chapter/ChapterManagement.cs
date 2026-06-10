using System.Collections.Generic;
using UnityEngine;

public static class ChapterManagement
{
    private class ChapterData
    {
        public string name;
        public string sceneName;
        public string cueName;
        public bool enabled = false;
    }

    private static Dictionary<string, ChapterData> chapters;
    private static bool initialized = false;

    private static string currentChapter = null;

    public static string GetCurrentChapter()
    {
        Initialize();
        return currentChapter;
    }

    public static string FindChapterByCue(string sceneName, string cueName)
    {
        Initialize();
        foreach (ChapterData chapter in chapters.Values)
        {
            if (chapter.sceneName == sceneName && chapter.cueName == cueName)
            {
                return chapter.name;
            }
        }

        return null;
    }

    public static void SetChapterByCue(string sceneName, string cueName)
    {
        Initialize();
        string chapterName = FindChapterByCue(sceneName, cueName);
        // Debug.Log(sceneName + " " + cueName + " " + chapterName);
        if (chapterName != null)
        {
            chapters[chapterName].enabled = true;
            currentChapter = chapterName;
        }
    }

    public static string GetChapterScene(string chapterName)
    {
        Initialize();
        return chapters[chapterName].sceneName;
    }

    public static string GetChapterCue(string chapterName)
    {
        Initialize();
        return chapters[chapterName].cueName;
    }

    public static bool IsValidChapter(string chapterName)
    {
        Initialize();
        return chapters.ContainsKey(chapterName);
    }

    public static bool IsChapterEnabled(string chapterName)
    {
        Initialize();
        return chapters[chapterName].enabled;
    }

    public static void SetChapterEnabled(string chapterName, bool enabled)
    {
        Initialize();
        chapters[chapterName].enabled = enabled;
    }

    private static void Initialize()
    {
        if (initialized)
            return;

        chapters = new Dictionary<string, ChapterData>();
        TextAsset chaptersList = Resources.Load<TextAsset>("chapters");
        string chaptersListAll = chaptersList.text;
        string[] lines = chaptersListAll.Split('\n');
        ChapterData currentChapterData = null;
        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();
            if (trimmedLine.Length == 0)
                continue;
            else if (trimmedLine[0] == '#')
            {
                string chapterName = trimmedLine.Substring(2);
                if (!chapters.ContainsKey(chapterName))
                    chapters[chapterName] = new ChapterData();
                currentChapterData = chapters[chapterName];
                currentChapterData.name = chapterName;
            }
            else if (trimmedLine[0] == '/')
                continue;
            else
            {
                int colonPosition = trimmedLine.IndexOf(':');
                string key = trimmedLine.Substring(0, colonPosition);
                string value = trimmedLine.Substring(colonPosition + 1).Trim();
                if (key == "Entry")
                {
                    string[] scenecue = value.Split(",");
                    string sceneName = scenecue[0].Trim();
                    string cueName = scenecue[1].Trim();
                    currentChapterData.sceneName = sceneName;
                    currentChapterData.cueName = cueName;
                }
                else if (key == "StartEnabled")
                {
                    currentChapterData.enabled = bool.Parse(value);
                }
            }
        }

        initialized = true;
    }
}
