using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DialogBox : MonoBehaviour, Taskable
{
    public Selector selector;

    public TextMeshProUGUI bodyText;
    public DialogButton left, center, right;

    private UnityEvent[] callbacks;
    private float t = 0.0f;
    private int dir = -1;

    private void Awake()
    {
        callbacks = new UnityEvent[3];

        selector.enabled = false;
        transform.localScale = Vector3.zero;

        // ShowDialogBox("Hello there!", "Hi", "Fuck you");
    }

    private void Update()
    {
        t += dir * Time.deltaTime * 5.0f;
        t = Mathf.Clamp01(t);
        float scale = Mathf.Pow(t - 0.7f, 2.0f) / (-0.4f) + 1.225f;
        if (t < 0.001f)
            scale = 0.0f;
        transform.localScale = new Vector3(scale * 0.75f, scale * 0.75f, 1);
    }

    public void SetCallback(int i, UnityEvent callback)
    {
        callbacks[i] = callback;
    }

    public void ShowDialogBox(string title, string optionL, string optionC, string optionR)
    {
        bodyText.text = title;
        left.SetText(optionL);
        center.SetText(optionC);
        right.SetText(optionR);

        t = 0.0f;
        dir = 1;
        selector.enabled = true;
    }

    public void ShowDialogBox(string title, string optionL, string optionR)
    {
        ShowDialogBox(title, optionL, null, optionR);
    }

    public void ShowDialogBox(string title, string optionC)
    {
        ShowDialogBox(title, null, optionC, null);
    }

    public void SelectCallback(int i)
    {
        if (!selector.enabled)
            return;
        if (callbacks[i] == null)
            Debug.LogWarning("Dialog \"" + bodyText.text + "\" button " + i + " has no callback");
        if (callbacks[i] != null)
            callbacks[i].Invoke();
        dir = -1;
        selector.enabled = false;
    }

    public bool HasFinished()
    {
        return !selector.enabled;
    }
}
