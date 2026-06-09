using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingCard : MonoBehaviour
{
    public static string endingName;

    public TextMeshProUGUI text;

    private float t;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text.text = "~ " + endingName + " ~";
        text.color = new Color(1, 1, 1, 0);
    }

    private void Update()
    {
        t += Time.deltaTime;
        float alpha = t;
        if (t > 1.0f)
            alpha = 1.0f;
        if (t > 4.0f)
            alpha = 5.0f - t;
        text.color = new Color(1, 1, 1, alpha);

        if (t > 5.0f)
            SceneManager.LoadScene("Flowchart");
    }
}
