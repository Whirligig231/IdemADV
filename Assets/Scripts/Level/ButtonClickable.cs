using UnityEngine;
using UnityEngine.Events;

public class ButtonClickable : MonoBehaviour, Clickable
{
    public Vector3 animationOffset;
    public float animationTime;
    public UnityEvent callback;

    private float t = 1.0f;
    private Vector3 originalPosition;
    private bool fresh = true;

    private void Start()
    {
        t = 1.0f;
        originalPosition = transform.localPosition;
    }

    private void Update()
    {
        if (animationTime > 0)
            t += Time.deltaTime / animationTime;
        t = Mathf.Clamp01(t);
        float actualT = t * 2.0f;
        if (t > 0.5f)
            actualT = (1.0f - t) * 2.0f;
        transform.localPosition = originalPosition + animationOffset * actualT;
    }

    public bool IsClickable()
    {
        return true;
    }

    public bool IsFresh()
    {
        return fresh;
    }

    public void SetFresh(bool fresh)
    {
        this.fresh = fresh;
    }

    public void OnClick()
    {
        GetComponent<AudioSource>()?.Play();

        callback.Invoke();
        t = 0.0f;
    }
}
