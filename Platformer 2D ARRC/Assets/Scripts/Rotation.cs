using UnityEngine;

public class Rotation : MonoBehaviour
{
    public float velocity = 10f;

    void Update()
    {
        transform.Rotate(0f, 0f, velocity * Time.deltaTime);
    }
}
