using UnityEngine;

public class Move_Test : MonoBehaviour
{
    public float moveSpeed = 0.1f;

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        transform.Translate(
            horizontal * moveSpeed,  // X
            0,                       // Y
            vertical * moveSpeed     // Z
            );
    }
}
