using TMPro;
using UnityEngine;

public class ResizeBoxWidthToText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float scale = 0.01f;
    public float addedWidth = 0.0f;

    private void Update()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        float newWidth = text.preferredWidth * scale + addedWidth;
        float newHeight = rectTransform.sizeDelta.y;
        rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
    }
}
