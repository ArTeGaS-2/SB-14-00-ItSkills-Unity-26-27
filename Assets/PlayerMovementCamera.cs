using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementCamera : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 12f;
    [SerializeField] private float jumpForce = 6f;

    [Header("Camera")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float mouseSensitivity = 0.12f;
    [SerializeField] private float firstPersonHeight = 0.8f;
    [SerializeField] private float thirdPersonHeight = 1.2f;
    [SerializeField] private float thirdPersonDistance = 4f;
    [SerializeField] private bool startInFirstPerson;

    private readonly HashSet<Collider> groundColliders = new();

    private Rigidbody playerRigidbody;
    private Vector2 moveInput;
    private float cameraYaw;
    private float cameraPitch;
    private bool jumpRequested;
    private bool isFirstPerson;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();

        if (playerCamera == null)
            playerCamera = Camera.main;

        cameraYaw = transform.eulerAngles.y;
        isFirstPerson = startInFirstPerson;
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        groundColliders.Clear();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        ReadMovementInput();
        ReadMouseInput();

        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
            return;

        if (keyboard.spaceKey.wasPressedThisFrame && groundColliders.Count > 0)
            jumpRequested = true;

        if (keyboard.vKey.wasPressedThisFrame)
            isFirstPerson = !isFirstPerson;
    }

    private void FixedUpdate()
    {
        Quaternion cameraDirection = Quaternion.Euler(0f, cameraYaw, 0f);
        Vector3 movement = cameraDirection * new Vector3(moveInput.x, 0f, moveInput.y);

        Vector3 velocity = playerRigidbody.linearVelocity;
        playerRigidbody.linearVelocity = new Vector3(
            movement.x * moveSpeed,
            velocity.y,
            movement.z * moveSpeed
        );

        if (isFirstPerson)
        {
            playerRigidbody.MoveRotation(Quaternion.Euler(0f, cameraYaw, 0f));
        }
        else if (movement.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            Quaternion smoothRotation = Quaternion.Slerp(
                playerRigidbody.rotation,
                targetRotation,
                turnSpeed * Time.fixedDeltaTime
            );
            playerRigidbody.MoveRotation(smoothRotation);
        }

        if (jumpRequested)
        {
            playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpRequested = false;
            groundColliders.Clear();
        }
    }

    private void LateUpdate()
    {
        if (playerCamera == null)
            return;

        Quaternion cameraRotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);

        if (isFirstPerson)
        {
            playerCamera.transform.SetPositionAndRotation(
                transform.position + Vector3.up * firstPersonHeight,
                cameraRotation
            );
        }
        else
        {
            Vector3 lookTarget = transform.position + Vector3.up * thirdPersonHeight;
            Vector3 cameraPosition = lookTarget - cameraRotation * Vector3.forward * thirdPersonDistance;
            playerCamera.transform.SetPositionAndRotation(cameraPosition, cameraRotation);
        }
    }

    private void ReadMovementInput()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            moveInput = Vector2.zero;
            return;
        }

        float horizontal = 0f;
        float vertical = 0f;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            horizontal -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            horizontal += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            vertical -= 1f;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            vertical += 1f;

        moveInput = Vector2.ClampMagnitude(new Vector2(horizontal, vertical), 1f);
    }

    private void ReadMouseInput()
    {
        Mouse mouse = Mouse.current;

        if (mouse == null)
            return;

        Vector2 mouseDelta = mouse.delta.ReadValue() * mouseSensitivity;
        cameraYaw += mouseDelta.x;
        cameraPitch = Mathf.Clamp(cameraPitch - mouseDelta.y, -80f, 80f);
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                groundColliders.Add(collision.collider);
                return;
            }
        }

        groundColliders.Remove(collision.collider);
    }

    private void OnCollisionExit(Collision collision)
    {
        groundColliders.Remove(collision.collider);
    }
}
