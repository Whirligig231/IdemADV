using UnityEngine;

public class Viewpoint : MonoBehaviour, Taskable
{
    public new string name;
    public float elasticity = 1.0f;
    public float time = 1.0f;

    private float t = 99999.0f;

    public void StartTimer()
    {
        t = 0.0f;
    }

    private void Update()
    {
        t += Time.deltaTime;
    }

    public bool HasFinished()
    {
        return (t >= time);
    }
}
