using UnityEngine;

public class DefaultStartCue : MonoBehaviour
{
    public string startCue;

    private void Awake()
    {
        FindAnyObjectByType<Director>().SetDefaultStartCue(startCue);
    }
}
