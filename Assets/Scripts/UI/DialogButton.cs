using UnityEngine;

public class DialogButton : MonoBehaviour, Selectable
{
    private bool isDisabled = false;
    private float t = 0.0f;
    private float targetT = 0.0f;

    private void Update()
    {
        t = Mathf.Lerp(t, targetT, Time.deltaTime * 10.0f);

        if (isDisabled)
        {
            transform.localScale = Vector3.one * 0.8f;
        }
        else
        {
            transform.localScale = Vector3.one * (1.0f + 0.2f * t);
        }
    }

    public float GetRadius()
    {
        return 0.4f;
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
        Debug.Log("PUSH");
    }
}
