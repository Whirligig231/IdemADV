using UnityEngine;

public class DialogBox : MonoBehaviour
{
    public Selector selector;

    private float t = 0.0f;
    private int dir = -1;

    private void Awake()
    {
        selector.enabled = false;
        transform.localScale = Vector3.zero;

        ShowDialogBox("Hello there!", "Hi", "Bye", "Fuck you");
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

    public void ShowDialogBox(string title, string optionL, string optionC, string optionR)
    {
        t = 0.0f;
        dir = 1;
        selector.enabled = true;
    }
}
