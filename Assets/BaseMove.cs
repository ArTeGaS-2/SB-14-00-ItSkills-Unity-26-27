using UnityEngine;

public class BaseMove : MonoBehaviour
{
    public Vector3 axis = Vector3.right;
    public float speed = 90f;

    private void Update()
    {
        transform.Rotate(axis, speed * Time.deltaTime, Space.Self);
    }
}
