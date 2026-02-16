using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class TextAppearHandler : MonoBehaviour
{
    private string textLine;
    private int totalLength;
    private int currentIndex;
    private float lengthIncrementTimer;
    private float t;
    private bool playClickSound = true;
    private Dictionary<string, Regex> tagRegexes;

    private void Start()
    {
        tagRegexes = new Dictionary<string, Regex>();
        tagRegexes["delay"] = new Regex("< *delay *= *([0-9.-]+) *>");
    }

    public void SetClickSoundFlag(bool needsClickSound)
    {
        playClickSound = needsClickSound;
    }

    public void DisplayText(string textLine, float time, bool timeIsTotal = false)
    {
        this.textLine = textLine;
        ComputeTotalLength();
        currentIndex = 0;
        t = 0;
        if (timeIsTotal)
            lengthIncrementTimer = time / Mathf.Max(1.0f, totalLength);
        else
            lengthIncrementTimer = time;
    }

    private void ComputeTotalLength()
    {
        totalLength = 0;
        int index = 0;
        while (index < textLine.Length)
        {
            totalLength++;
            index++;
            while (index < textLine.Length && textLine[index] == '<')
            {
                index++;
                while (textLine[index - 1] != '>')
                    index++;
            }
        }
    }

    public void SkipToEnd()
    {
        currentIndex = textLine.Length;
    }

    public bool IsFinished()
    {
        return (currentIndex == textLine.Length);
    }

    private void Update()
    {
        if (textLine == null)
            return;

        bool playAudio = false;
        t += Time.deltaTime;
        while (t >= lengthIncrementTimer && currentIndex < textLine.Length)
        {
            t -= lengthIncrementTimer;
            currentIndex++;
            while (currentIndex < textLine.Length && textLine[currentIndex] == '<')
            {
                int ltPosition = currentIndex;
                currentIndex++;
                while (textLine[currentIndex - 1] != '>')
                    currentIndex++;
                int rtPosition = currentIndex;
                string tag = textLine.Substring(ltPosition, rtPosition - ltPosition);
                foreach (string tagName in tagRegexes.Keys)
                {
                    Regex tagRegex = tagRegexes[tagName];
                    Match match = tagRegex.Match(tag);
                    if (!match.Success)
                        continue;

                    if (tagName == "delay")
                    {
                        float delay = float.Parse(match.Groups[1].Value);
                        t -= delay;
                    }
                }
            }

            if (textLine[currentIndex - 1] != ' ')
                playAudio = true;
        }

        if (playClickSound && playAudio)
            GetComponent<AudioSource>().Play();

        TextMeshProUGUI tmpro = GetComponent<TextMeshProUGUI>();
        tmpro.text = RemoveCustomTags(textLine.Substring(0, currentIndex) + "<color=#00000000>"
            + textLine.Substring(currentIndex, textLine.Length - currentIndex) + "</color>");
    }

    private string RemoveCustomTags(string textLine)
    {
        StringBuilder outputLine = new StringBuilder();
        int index = 0;
        while (index < textLine.Length)
        {
            outputLine.Append(textLine[index]);
            index++;
            while (index < textLine.Length && textLine[index] == '<')
            {
                int ltPosition = index;
                index++;
                while (textLine[index - 1] != '>')
                    index++;
                int rtPosition = index;
                string tag = textLine.Substring(ltPosition, rtPosition - ltPosition);
                bool matched = false;
                foreach (Regex regex in tagRegexes.Values)
                {
                    if (regex.IsMatch(tag))
                    {
                        matched = true;
                        break;
                    }
                }
                if (!matched)
                    outputLine.Append(tag);
            }
        }
        return outputLine.ToString();
    }
}
