using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CapsuleMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private float jumpForce = 6f;

    private Rigidbody capsuleRigidbody;
    private float moveInput;
    private float rotationInput;
    private bool jumpRequested;
    private bool isGrounded;

    private void Awake()
    {
        capsuleRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            moveInput = 0f;
            rotationInput = 0f;
            return;
        }

        moveInput = 0f;
        rotationInput = 0f;

        if (keyboard.wKey.isPressed)
            moveInput += 1f;

        if (keyboard.sKey.isPressed)
            moveInput -= 1f;

        if (keyboard.dKey.isPressed)
            rotationInput += 1f;

        if (keyboard.aKey.isPressed)
            rotationInput -= 1f;

        if (keyboard.spaceKey.wasPressedThisFrame && isGrounded)
            jumpRequested = true;
    }

    private void FixedUpdate()
    {
        Vector3 movement = transform.forward * moveInput * moveSpeed * Time.fixedDeltaTime;
        capsuleRigidbody.MovePosition(capsuleRigidbody.position + movement);

        float rotation = rotationInput * rotationSpeed * Time.fixedDeltaTime;
        Quaternion turn = Quaternion.Euler(0f, rotation, 0f);
        capsuleRigidbody.MoveRotation(capsuleRigidbody.rotation * turn);

        if (jumpRequested)
        {
            capsuleRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpRequested = false;
            isGrounded = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}
