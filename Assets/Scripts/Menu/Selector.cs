using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Selector : ControlledMonoBehaviour
{
    public Pane pane;
    public new Camera camera;

    private List<Transform> selectableTransforms;
    private int selectionIndex = -1;
    private int fallbackIndex = 0;

    private Vector2 previousMousePosition = Vector2.zero;
    private Vector2 previousNavigateDir = Vector2.zero;

    private InputAction point, navigate, submit;

    private void AddSelectableTransformsRecursive(Transform root)
    {
        if (root.GetComponent<Selectable>() != null)
            selectableTransforms.Add(root);

        foreach (Transform child in root)
            AddSelectableTransformsRecursive(child);
    }

    private void Awake()
    {
        if (camera == null)
            camera = Camera.main;
        if (camera == null)
            camera = FindAnyObjectByType<Camera>();

        point = InputSystem.actions.FindAction("Point");
        navigate = InputSystem.actions.FindAction("Navigate");
        submit = InputSystem.actions.FindAction("Submit");
        submit.started += ProcessClick;

        selectableTransforms = new List<Transform>();
        AddSelectableTransformsRecursive(transform);
    }

    private void OnDestroy()
    {
        submit.started -= ProcessClick;
    }

    private void ProcessClick(InputAction.CallbackContext context)
    {
        if (!HasPriority())
            return;

        if (selectionIndex < 0)
            selectionIndex = fallbackIndex;
        else
        {
            selectableTransforms[selectionIndex].GetComponent<Selectable>().Select();
        }
    }

    private int GetClosestIndex(Vector3 localPosition, bool considerRadius = false)
    {
        int closestIndex = -1;
        float closestDistance = 99999.0f;
        for (int i = 0; i < selectableTransforms.Count; i++)
        {
            Vector3 thisTransformPosition = transform.InverseTransformPoint(selectableTransforms[i].position);
            float distance = Vector3.Distance(localPosition, thisTransformPosition);
            if (considerRadius)
            {
                if (selectableTransforms[i].GetComponent<Selectable>().GetRadius() < distance)
                    continue;
            }

            if (distance < closestDistance)
            {
                closestIndex = i;
                closestDistance = distance;
            }
        }

        return closestIndex;
    }

    private void Navigate(Vector2 navigateDir)
    {
        Vector3 step = new Vector3(0.01f * navigateDir.x, 0.01f * navigateDir.y, 0);
        Vector3 localPos = transform.InverseTransformPoint(selectableTransforms[fallbackIndex].position);
        for (int i = 0; i < 10000; i++)
        {
            localPos += step;
            int newIndex = GetClosestIndex(localPos);
            if (newIndex != fallbackIndex)
            {
                fallbackIndex = newIndex;
                break;
            }
        }
        selectionIndex = fallbackIndex;

        if (pane != null)
        {
            pane.CenterOnNavigate(selectableTransforms[fallbackIndex]);
        }
    }

    private void Update()
    {
        if (!HasPriority())
            return;

        int previousSelectionIndex = selectionIndex;

        Vector2 mousePosition = point.ReadValue<Vector2>();
        if (previousMousePosition != mousePosition)
        {
            previousMousePosition = mousePosition;
            Ray mouseRay = camera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, 0));

            // Intersect with our plane
            float directionNormalDot = Vector3.Dot(mouseRay.direction, transform.forward);
            Vector3 originOffset = transform.position - mouseRay.origin;
            float originNormalDot = Vector3.Dot(originOffset, transform.forward);
            float tValue = originNormalDot / directionNormalDot;
            Vector3 mouseHitPoint = mouseRay.origin + mouseRay.direction * tValue;
            Vector3 mouseHitPointLocal = transform.InverseTransformPoint(mouseHitPoint);

            // Find the closest point
            selectionIndex = GetClosestIndex(mouseHitPointLocal, true);
            fallbackIndex = GetClosestIndex(mouseHitPointLocal);
        }

        Vector2 navigateDir = navigate.ReadValue<Vector2>();
        if (navigateDir != previousNavigateDir)
        {
            previousNavigateDir = navigateDir;
            Navigate(navigateDir);
        }

        if (selectionIndex != previousSelectionIndex)
        {
            if (previousSelectionIndex >= 0)
                selectableTransforms[previousSelectionIndex].GetComponent<Selectable>().SetSelected(false);
            if (selectionIndex >= 0)
                selectableTransforms[selectionIndex].GetComponent<Selectable>().SetSelected(true);
        }
    }
}
