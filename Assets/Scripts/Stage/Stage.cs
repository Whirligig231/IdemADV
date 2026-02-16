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

    private Dictionary<string, Transform> stageMarkers;

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
            personTransform.gameObject.SetActive(true);
        }

        stageMarkers = new Dictionary<string, Transform>();
        foreach (StageMarker marker in FindObjectsByType<StageMarker>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            stageMarkers[marker.name] = marker.transform;
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

    private void SetGameLayerRecursive(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetGameLayerRecursive(child.gameObject, newLayer);
        }
    }

    public void FadeStageIn(string nameL, string nameC, string nameR, string markerL, string markerC, string markerR, bool putInMainLayer)
    {
        int newLayer = LayerMask.NameToLayer("Stage");
        if (putInMainLayer)
            newLayer = LayerMask.NameToLayer("Default");

        if (personL != null)
        {
            personL.localPosition = Vector3.up * 10000.0f;
        }
        if (personC != null)
        {
            personC.localPosition = Vector3.up * 10000.0f;
        }
        if (personR != null)
        {
            personR.localPosition = Vector3.up * 10000.0f;
        }

        if (nameL != "")
        {
            personL = persons[nameL];
            SetGameLayerRecursive(personL.gameObject, newLayer);
            personL.localPosition = Vector3.right * -2.25f;
            personL.localEulerAngles = new Vector3(0, 180.0f, 0);
            if (markerL != "")
            {
                personL.position = stageMarkers[markerL].position;
                personL.rotation = stageMarkers[markerL].rotation;
            }
        }
        if (nameC != "")
        {
            personC = persons[nameC];
            SetGameLayerRecursive(personC.gameObject, newLayer);
            personC.localPosition = Vector3.zero;
            personC.localEulerAngles = new Vector3(0, 180.0f, 0);
            if (markerC != "")
            {
                personC.position = stageMarkers[markerC].position;
                personC.rotation = stageMarkers[markerC].rotation;
            }
        }
        if (nameR != "")
        {
            personR = persons[nameR];
            SetGameLayerRecursive(personR.gameObject, newLayer);
            personR.localPosition = Vector3.right * 2.25f;
            personR.localEulerAngles = new Vector3(0, 180.0f, 0);
            if (markerR != "")
            {
                personR.position = stageMarkers[markerR].position;
                personR.rotation = stageMarkers[markerR].rotation;
            }
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
