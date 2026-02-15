using UnityEngine;

public class FloatingOrb : MonoBehaviour, Personable
{
    public Transform sphereTransform;
    public Renderer sphereRenderer;

    private float t;

    private void Update()
    {
        t += Time.deltaTime;
        sphereTransform.localPosition = Vector3.up * 0.3f * Mathf.Sin(t);
    }

    public string GetName()
    {
        return "Floating Orb";
    }

    public void SetMood(string mood)
    {
        Debug.Log("Floating Orb's mood is now: " + mood);
    }

    public void SetSoundLevel(float level)
    {
        sphereRenderer.material.SetColor("_EmissionColor",
            new Color(0.6f + 0.4f * level, 0.4f + 0.4f * level, 0.2f + 0.4f * level));
    }
}
