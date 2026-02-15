using UnityEngine;

public class FloatingOrb : MonoBehaviour, Personable
{
    public Transform sphereTransform;
    public Renderer sphereRenderer;

    private float t;
    private Color targetColor;
    private Color color;
    private float soundLevel = 0.0f;

    private void Start()
    {
        targetColor = new Color(0.6f, 0.4f, 0.2f);
        color = targetColor;
    }

    private void Update()
    {
        t += Time.deltaTime;
        sphereTransform.localPosition = Vector3.up * 0.3f * Mathf.Sin(t);
        color = Color.Lerp(color, targetColor, Time.deltaTime);
        sphereRenderer.material.SetColor("_EmissionColor", color + soundLevel * 0.4f * Color.white);
    }

    public string GetName()
    {
        return "Floating Orb";
    }

    public void SetMood(string mood)
    {
        switch (mood)
        {
            case "happy":
                targetColor = new Color(0.6f, 0.4f, 0.2f);
                break;
            case "sad":
                targetColor = new Color(0.2f, 0.4f, 0.6f);
                break;
        }
    }

    public void SetSoundLevel(float level)
    {
        soundLevel = level;
    }
}
