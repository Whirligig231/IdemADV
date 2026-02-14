using TMPro;
using UnityEngine;

public class TextAppearHandler : MonoBehaviour
{
    private string textLine;
    private int totalLength;
    private int currentIndex;
    private float lengthIncrementTimer;
    private float t;

    private void Start()
    {
        // DisplayText("Hello! This is <i>a test!</i> Yay!", 0.05f);
    }

    public void DisplayText(string textLine, float time, bool timeIsTotal = false)
    {
        this.textLine = textLine;
        ComputeTotalLength();
        currentIndex = 0;
        t = 0;
        if (timeIsTotal)
            lengthIncrementTimer = time / Mathf.Min(1.0f, totalLength);
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
            if (index < textLine.Length && textLine[index] == '<')
            {
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
            if (currentIndex < textLine.Length && textLine[currentIndex] == '<')
            {
                while (textLine[currentIndex - 1] != '>')
                    currentIndex++;
            }

            if (textLine[currentIndex - 1] != ' ')
                playAudio = true;
        }

        if (playAudio)
            GetComponent<AudioSource>().Play();

        TextMeshProUGUI tmpro = GetComponent<TextMeshProUGUI>();
        tmpro.text = textLine.Substring(0, currentIndex) + "<color=#00000000>"
            + textLine.Substring(currentIndex, textLine.Length - currentIndex) + "</color>";
    }
}
