using TMPro;
using UnityEngine;

public class CodeEntryManager : MonoBehaviour
{
    public TextMeshPro[] texts;
    public int[] correctCode;
    public ButtonClickable[] buttons;
    public string rightCue, wrongCue, alreadyCue;

    private int[] digits;
    private bool solved = false;

    private void Start()
    {
        digits = new int[] { 0, 0, 0 };
    }

    private void Update()
    {
        for (int i = 0; i < 3; i++)
            texts[i].text = digits[i].ToString();
    }

    public void ChangeDigit(int digitIndex, int offset)
    {
        digits[digitIndex] += offset;
        if (digits[digitIndex] < 0)
            digits[digitIndex] = (digits[digitIndex] + 100) % 10;
        if (digits[digitIndex] > 9)
            digits[digitIndex] = digits[digitIndex] % 10;
    }

    public void IncrementDigit(int digitIndex)
    {
        ChangeDigit(digitIndex, 1);
    }

    public void DecrementDigit(int digitIndex)
    {
        ChangeDigit(digitIndex, -1);
    }

    public void TryCode()
    {
        if (solved)
        {
            FindAnyObjectByType<Director>().ExecuteCue(alreadyCue);
            return;
        }

        bool correct = true;
        for (int i = 0; i < 3; i++)
        {
            if (digits[i] != correctCode[i])
                correct = false;
        }

        if (correct)
        {
            FindAnyObjectByType<Director>().ExecuteCue(rightCue);
            solved = true;
            foreach (ButtonClickable button in buttons)
            {
                button.SetFresh(false);
            }
        }
        else
            FindAnyObjectByType<Director>().ExecuteCue(wrongCue);
    }
}
