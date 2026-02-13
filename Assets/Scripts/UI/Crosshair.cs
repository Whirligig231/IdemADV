using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    public Transform cameraTransform;
    public PlayerMovement playerMovement;
    public Image dot, ring;
    public float maxClickDistance = 8.0f;

    private InputAction interact;

    private void Start()
    {
        interact = InputSystem.actions.FindAction("Interact");
        interact.started += context => ProcessClick();
    }

    private void Update()
    {
        dot.enabled = playerMovement.enabled;

        if (playerMovement.enabled)
        {
            RaycastHit hit;
            if (!Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, maxClickDistance))
                ring.enabled = false;
            else
            {
                Clickable clickable = hit.collider.GetComponent<Clickable>();
                if (clickable == null)
                    ring.enabled = false;
                else if (!clickable.IsClickable())
                    ring.enabled = false;
                else if (!clickable.IsFresh())
                {
                    ring.enabled = true;
                    ring.color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
                }
                else
                {
                    ring.enabled = true;
                    ring.color = new Color(0.4f, 1.0f, 1.0f, 0.9f);
                }
            }
        }
        else
        {
            ring.enabled = false;
        }
    }

    private void ProcessClick()
    {
        if (!playerMovement.enabled)
            return;
        RaycastHit hit;
        if (!Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, maxClickDistance))
            return;
        Clickable clickable = hit.collider.GetComponent<Clickable>();
        if (clickable == null)
            return;
        if (!clickable.IsClickable())
            return;
        clickable.OnClick();
    }
}
