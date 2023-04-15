using UnityEngine;

public class Lava : MonoBehaviour
{
    public float speedUp = 1f;
    void Update() => transform.Translate(Vector2.up * speedUp * Time.deltaTime);
}
