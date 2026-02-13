using UnityEngine;

public class CopyClickable : MonoBehaviour, Clickable
{
    public Collider copyFrom;

    public bool IsClickable()
    {
        return copyFrom.GetComponent<Clickable>().IsClickable();
    }

    public bool IsFresh()
    {
        return copyFrom.GetComponent<Clickable>().IsFresh();
    }

    public void OnClick()
    {
        copyFrom.GetComponent<Clickable>().OnClick();
    }
}