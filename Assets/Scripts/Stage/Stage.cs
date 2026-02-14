using UnityEngine;

public class Stage : MonoBehaviour
{
    public Transform playerCamera;
    public bool followPlayerCamera = true; // Enable so that the lighting matches the lighting in the scene

    private void Update()
    {
        if (followPlayerCamera)
        {
            transform.position = playerCamera.position + playerCamera.forward * 5.0f;
            transform.rotation = playerCamera.rotation;
        }
    }
}
