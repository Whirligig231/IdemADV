using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public PlayerMovement playerMovement;

    private Viewpoint currentViewpoint;

    private void Start()
    {
        currentViewpoint = playerMovement.playerViewpoint;
        transform.position = currentViewpoint.transform.position;
        transform.rotation = currentViewpoint.transform.rotation;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, currentViewpoint.transform.position, Time.deltaTime * currentViewpoint.elasticity * 5.0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, currentViewpoint.transform.rotation, Time.deltaTime * currentViewpoint.elasticity * 5.0f);
    }

    public void SetToPlayer()
    {
        currentViewpoint = playerMovement.playerViewpoint;
    }

    public void SetToViewpoint(Viewpoint viewpoint)
    {
        currentViewpoint = viewpoint;
    }
}
