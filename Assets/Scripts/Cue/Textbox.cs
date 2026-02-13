using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Textbox : MonoBehaviour, Taskable
{
    public Image textboxBackground, triangle;
    public GameObject nameBase;
    public TextMeshProUGUI nameText;
    public TextAppearHandler textAppear;

    private InputAction interact;
    private int state = 0; // 0 = inactive, 1 = printing, 2 = done printing

    private bool clickDisabledForThisFrame = false;

    private string textboxName;

    private void Start()
    {
        interact = InputSystem.actions.FindAction("Interact");
        interact.started += context => ProcessClick();

        textboxBackground.enabled = false;
    }

    private void ProcessClick()
    {
        if (clickDisabledForThisFrame)
        {
            clickDisabledForThisFrame = false;
            return;
        }

        switch (state)
        {
            case 0:
                break;
            case 1:
                state = 2;
                textAppear.SkipToEnd();
                break;
            case 2:
                state = 0;
                textAppear.DisplayText("", 0);
                break;
        }
    }

    private void Update()
    {
        clickDisabledForThisFrame = false;

        switch (state)
        {
            case 0:
                textboxBackground.enabled = false;
                triangle.enabled = false;
                nameBase.SetActive(false);
                break;
            case 1:
                textboxBackground.enabled = true;
                triangle.enabled = false;
                nameBase.SetActive(textboxName != "");
                nameText.text = textboxName;
                if (textAppear.IsFinished())
                    state = 2;
                break;
            case 2:
                textboxBackground.enabled = true;
                triangle.enabled = true;
                nameBase.SetActive(textboxName != "");
                break;
        }
    }

    public void DisplayText(string name, string textLine, float time, bool timeIsTotal = false)
    {
        clickDisabledForThisFrame = true;
        state = 1;
        textboxName = name;
        nameText.text = name;
        textAppear.DisplayText(textLine, time, timeIsTotal);
    }

    public bool HasFinished()
    {
        return (state == 0);
    }
}
