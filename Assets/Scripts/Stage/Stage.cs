using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour, Taskable
{
    public bool followPlayerCamera = true; // Enable so that the lighting matches the lighting in the scene

    public Transform playerCamera;
    public Renderer fadePlane;
    public Transform personsParent;

    private Dictionary<string, Transform> persons;
    private Transform personL, personC, personR;

    private float fadeT = 0;
    private int fadeDir = -1;

    private void Awake()
    {
        persons = new Dictionary<string, Transform>();

        foreach (Transform personTransform in personsParent)
        {
            Personable person = personTransform.GetComponent<Personable>();
            if (person == null)
                continue;
            persons[person.GetName()] = personTransform;
            personTransform.gameObject.SetActive(false);
        }
    }

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

    public void FadeStageIn(string nameL, string nameC, string nameR)
    {
        personL?.gameObject.SetActive(false);
        personC?.gameObject.SetActive(false);
        personR?.gameObject.SetActive(false);

        if (nameL != "")
        {
            personL = persons[nameL];
            personL.gameObject.SetActive(true);
            personL.localPosition = Vector3.right * -2.25f;
        }
        if (nameC != "")
        {
            personC = persons[nameC];
            personC.gameObject.SetActive(true);
            personC.localPosition = Vector3.zero;
        }
        if (nameR != "")
        {
            personR = persons[nameR];
            personR.gameObject.SetActive(true);
            personR.localPosition = Vector3.right * 2.25f;
        }

        fadeT = 0;
        fadeDir = 1;
    }

    public void FadeStageOut()
    {
        fadeT = 1;
        fadeDir = -1;
    }

    public void SetMood(string name, string mood)
    {
        persons[name].GetComponent<Personable>().SetMood(mood);
    }

    public void SetSoundLevel(string name, float level)
    {
        if (name == null || !persons.ContainsKey(name))
            return;
        persons[name].GetComponent<Personable>().SetSoundLevel(level);
    }
}
