using UnityEngine;

public class Move_temp : MonoBehaviour
{
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        transform.Translate(
            horizontal,   // X
            0,            // Y
            vertical
            );
    }
}
