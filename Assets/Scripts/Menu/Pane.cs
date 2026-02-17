using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pane : MonoBehaviour
{
    public float maxScale = 5.0f, minScale = 0.2f;

    public Transform boundsPlane;
    public new Camera camera;

    private Transform handle;
    private float targetZoomPower = 0.0f, zoomPower = 0.0f;
    private float screenLocalX, screenLocalY;
    private float boundsLocalX, boundsLocalY;

    private bool rightDragging = false;
    private Vector3 dragMousePosition;

    private Transform targetTransform = null;

    private InputAction point, pan, rightClick, scrollWheel, zoomIn, zoomOut;

    private void Awake()
    {
        if (camera == null)
            camera = Camera.main;
        if (camera == null)
            camera = FindAnyObjectByType<Camera>();

        handle = transform.GetChild(0);

        SetViewportBounds();

        point = InputSystem.actions.FindAction("Point");
        pan = InputSystem.actions.FindAction("Pan");
        rightClick = InputSystem.actions.FindAction("RightClick");
        scrollWheel = InputSystem.actions.FindAction("ScrollWheel");
        zoomIn = InputSystem.actions.FindAction("ZoomIn");
        zoomOut = InputSystem.actions.FindAction("ZoomOut");
    }

    private Vector3 IntersectWithMyPlane(Ray ray)
    {
        float directionNormalDot = Vector3.Dot(ray.direction, transform.forward);
        Vector3 originOffset = transform.position - ray.origin;
        float originNormalDot = Vector3.Dot(originOffset, transform.forward);
        float tValue = originNormalDot / directionNormalDot;
        return ray.origin + ray.direction * tValue;
    }

    private void SetViewportBounds()
    {
        // Find local position maximum Y
        Ray upRay = camera.ViewportPointToRay(new Vector3(0.5f, 1, 0));
        // Intersect with our plane
        Vector3 upPoint = IntersectWithMyPlane(upRay);
        Vector3 upPointLocal = transform.InverseTransformPoint(upPoint);
        screenLocalY = upPointLocal.y;
        screenLocalX = screenLocalY * 5.0f / 4.0f;

        // Now find the bounds plane
        Vector3 boundsCorner = boundsPlane.TransformPoint(new Vector3(0.5f, 0.5f, 0));
        Vector3 boundsCornerLocal = transform.InverseTransformPoint(boundsCorner);
        boundsLocalX = boundsCornerLocal.x;
        boundsLocalY = boundsCornerLocal.y;
    }

    private void MoveBy(Vector2 panVector)
    {
        Vector3 positionDeltaLocal = new Vector3(panVector.x, panVector.y, 0);
        handle.localPosition += positionDeltaLocal;
    }

    private void Update()
    {
        Vector3 oldCenterLocalPosition = handle.InverseTransformPoint(transform.position);

        targetZoomPower += scrollWheel.ReadValue<Vector2>().y;
        if (zoomIn.IsPressed())
            targetZoomPower += Time.deltaTime * 10.0f;
        if (zoomOut.IsPressed())
            targetZoomPower -= Time.deltaTime * 10.0f;

        float minZoomPower = Mathf.Log(minScale, 1.1f);
        float maxZoomPower = Mathf.Log(maxScale, 1.1f);
        targetZoomPower = Mathf.Clamp(targetZoomPower, minZoomPower, maxZoomPower);

        zoomPower = Mathf.Lerp(zoomPower, targetZoomPower, Time.deltaTime * 10.0f);
        handle.localScale = Vector3.one * Mathf.Pow(1.1f, zoomPower);
        Vector3 newCenterLocalPosition = handle.InverseTransformPoint(transform.position);
        handle.localPosition -= (oldCenterLocalPosition - newCenterLocalPosition) * handle.localScale.x;

        Vector2 panVector = -pan.ReadValue<Vector2>() * Time.deltaTime * boundsLocalY * 3.0f;
        if (panVector != Vector2.zero)
            targetTransform = null;

        Vector2 mousePosition = point.ReadValue<Vector2>();
        if (rightClick.IsPressed())
        {
            targetTransform = null;

            Ray mouseRay = camera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, 0));
            Vector3 mousePoint = IntersectWithMyPlane(mouseRay);
            Vector3 mousePointLocal = transform.InverseTransformPoint(mousePoint);

            if (rightDragging)
            {
                Vector3 difference = mousePointLocal - dragMousePosition;
                panVector += new Vector2(difference.x, difference.y);
            }
            else
            {
                rightDragging = true;
            }

            dragMousePosition = mousePointLocal;
        }
        else
        {
            rightDragging = false;
        }

        if (targetTransform != null)
        {
            Vector3 currentLocalPosition = transform.InverseTransformPoint(targetTransform.position);

            // Add tolerance
            float tolerance = screenLocalY / 3.0f;
            if (Mathf.Abs(currentLocalPosition.x) < tolerance)
                currentLocalPosition.x = 0.0f;
            else
                currentLocalPosition.x -= tolerance * Mathf.Sign(currentLocalPosition.x);
            if (Mathf.Abs(currentLocalPosition.y) < tolerance)
                currentLocalPosition.y = 0.0f;
            else
                currentLocalPosition.y -= tolerance * Mathf.Sign(currentLocalPosition.y);

            panVector -= new Vector2(currentLocalPosition.x, currentLocalPosition.y) * Time.deltaTime * 10.0f;
        }

        MoveBy(panVector);
        KeepPositionInBounds();
    }

    private void KeepPositionInBounds()
    {
        float newLocalX = handle.localPosition.x;
        float newLocalY = handle.localPosition.y;

        float boundsLocalXScaled = boundsLocalX * handle.localScale.x;
        float boundsLocalYScaled = boundsLocalY * handle.localScale.y;

        float boundsMarginUp = boundsLocalYScaled + handle.localPosition.y - screenLocalY;
        float boundsMarginDown = boundsLocalYScaled - handle.localPosition.y - screenLocalY;

        if (boundsLocalYScaled < screenLocalY)
            newLocalY = 0;
        else if (boundsMarginUp < 0)
            newLocalY -= boundsMarginUp;
        else if (boundsMarginDown < 0)
            newLocalY += boundsMarginDown;

        float boundsMarginRight = boundsLocalXScaled + handle.localPosition.x - screenLocalX;
        float boundsMarginLeft = boundsLocalXScaled - handle.localPosition.x - screenLocalX;

        if (boundsLocalXScaled < screenLocalX)
            newLocalX = 0;
        else if (boundsMarginRight < 0)
            newLocalX -= boundsMarginRight;
        else if (boundsMarginLeft < 0)
            newLocalX += boundsMarginLeft;

        MoveBy(new Vector2(newLocalX - handle.localPosition.x, newLocalY - handle.localPosition.y));
    }

    public void CenterOnNavigate(Transform navTransform)
    {
        targetTransform = navTransform;
    }
}