using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Viewpoint playerViewpoint;

    private float walkSpeed = 7.0f;
    private float turnSensitivity = 4.0f;
    private float mouseSensitivity = 0.004f;
    
    private float pitch = 0.0f;
    private float yaw = 0.0f;

    private InputAction move, look, mouselook, sprint;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        move = InputSystem.actions.FindAction("Move");
        look = InputSystem.actions.FindAction("Look");
        mouselook = InputSystem.actions.FindAction("MouseLook");
        sprint = InputSystem.actions.FindAction("Sprint");
    }

    private void Update()
    {
        Vector2 lookVector = look.ReadValue<Vector2>();
        yaw += lookVector.x * turnSensitivity * Time.deltaTime;
        pitch -= lookVector.y * turnSensitivity * Time.deltaTime;

        Vector2 mouselookVector = mouselook.ReadValue<Vector2>();
        yaw += mouselookVector.x * mouseSensitivity;
        pitch -= mouselookVector.y * mouseSensitivity;

        pitch = Mathf.Clamp(pitch, -1.5f, 1.5f);
        playerViewpoint.transform.position = transform.position;
        playerViewpoint.transform.rotation = Quaternion.Euler(Mathf.Rad2Deg * pitch, Mathf.Rad2Deg * yaw, 0.0f);

        Vector2 moveVector = move.ReadValue<Vector2>();
        float controlForward = moveVector.y;
        float controlSide = moveVector.x;
        Vector3 forwardDir = Vector3.ProjectOnPlane(playerViewpoint.transform.forward, Vector3.up).normalized;
        Vector3 sideDir = Vector3.ProjectOnPlane(playerViewpoint.transform.right, Vector3.up).normalized;
        Vector3 movement = controlForward * forwardDir + controlSide * sideDir;

        float running = 1.0f;
        if (sprint.IsPressed())
            running = 3.0f;
        GetComponent<CharacterController>().SimpleMove(movement * walkSpeed * running);
    }
}
