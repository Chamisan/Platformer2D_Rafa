using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMovement : MonoBehaviour
{
    [SerializeField] Vector2 velocity;
    [SerializeField] Rigidbody2D rbPlayer;
    private Vector2 offset;
    private Material material;

    void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
        rbPlayer = GameObject.Find("Player").GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        offset = (rbPlayer.velocity.x * 0.01f) * velocity * Time.deltaTime;
        material.mainTextureOffset += offset;
    }
}
