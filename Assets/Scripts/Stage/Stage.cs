using UnityEngine;

public class Stage : MonoBehaviour, Taskable
{
    public bool followPlayerCamera = true; // Enable so that the lighting matches the lighting in the scene

    public Transform playerCamera;
    public Renderer fadePlane;
    private float fadeT = 0;
    private int fadeDir = -1;

    public bool HasFinished()
    {
        if (fadeDir > 0)
            return (fadeT > 0.9999f);
        if (fadeDir < 0)
            return (fadeT < 0.0001f);
        return true;
    }

    private void Update()
    {
        if (followPlayerCamera)
        {
            transform.position = playerCamera.position + playerCamera.forward * 5.0f;
            transform.rotation = playerCamera.rotation;
        }

        fadeT += fadeDir * Time.deltaTime;
        fadeT = Mathf.Clamp01(fadeT);
        fadePlane.material.SetColor("_Color", new Color(1, 1, 1, fadeT));
    }

    public void FadeStageIn()
    {
        fadeT = 0;
        fadeDir = 1;
    }

    public void FadeStageOut()
    {
        fadeT = 1;
        fadeDir = -1;
    }
}
