using UnityEngine;
using UnityEngine.Events;

public class CuedAction : MonoBehaviour, Taskable
{
    public new string name;
    public float time = 1.0f;
    public UnityEvent callback;

    private float t = 99999.0f;

    private void Update()
    {
        t += Time.deltaTime;
    }

    public void Invoke()
    {
        t = 0.0f;
        callback.Invoke();
    }

    public bool HasFinished()
    {
        return (t >= time);
    }
}
