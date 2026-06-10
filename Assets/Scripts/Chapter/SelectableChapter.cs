using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectableChapter : MonoBehaviour, Selectable
{
    public string chapterName = "My Cool Chapter";

    public Image image;
    public TextMeshProUGUI title;

    private bool isDisabled = false;
    private float t = 0.0f;
    private float targetT = 0.0f;

    private void Start()
    {
        if (!ChapterManagement.IsValidChapter(chapterName))
        {
            isDisabled = true;
            return;
        }

        isDisabled = !ChapterManagement.IsChapterEnabled(chapterName);
    }

    private void Update()
    {
        t = Mathf.Lerp(t, targetT, Time.deltaTime * 10.0f);

        if (isDisabled)
        {
            image.transform.localScale = Vector3.one * 0.8f;
            image.color = new Color(0.3f, 0.3f, 0.3f, 1.0f);
            title.text = "";
        }
        else
        {
            image.transform.localScale = Vector3.one * (1.0f + 0.2f * t);
            image.color = new Color(0.8f + 0.2f * t, 0.8f + 0.2f * t, 0.8f + 0.2f * t, 1.0f);
            title.text = (targetT > 0.5f) ? chapterName : "";
        }
    }

    public float GetRadius()
    {
        return 1.0f;
    }

    public bool GetDisabled()
    {
        return isDisabled;
    }

    public void SetSelected(bool selected)
    {
        targetT = selected ? 1 : 0;
    }

    public void Select()
    {
        string sceneName = ChapterManagement.GetChapterScene(chapterName);
        string cueName = ChapterManagement.GetChapterCue(chapterName);
        Director.ExecuteCueInScene(sceneName, cueName);
    }
}
