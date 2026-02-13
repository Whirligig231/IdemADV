using UnityEngine;

public class DefaultStartCue : MonoBehaviour
{
    public string startCue;

    private void Start()
    {
        FindAnyObjectByType<Director>().ExecuteCue(startCue);
    }
}
