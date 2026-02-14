using UnityEngine;

public class FadeQuad : MonoBehaviour, Taskable
{
    private float t = 0.0f;
    private int dir = -1;

    private void Update()
    {
        t += Time.deltaTime * dir;
        t = Mathf.Clamp01(t);
        GetComponent<Renderer>().material.color = new Color(0, 0, 0, t);
    }

    public void FadeIn()
    {
        t = 1;
        dir = -1;
    }

    public void FadeOut()
    {
        t = 0;
        dir = 1;
    }

    public void CutIn()
    {
        t = 0;
        dir = -1;
    }

    public void CutOut()
    {
        t = 1;
        dir = 1;
    }

    public bool HasFinished()
    {
        if (dir > 0)
            return (t >= 0.9999f);
        if (dir < 0)
            return (t < 0.0001f);
        return true;
    }
}
