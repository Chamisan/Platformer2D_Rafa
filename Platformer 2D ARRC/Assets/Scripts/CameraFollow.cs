using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    [SerializeField] Vector3 offset, minLimit, maxLimit;
    [SerializeField] float smoothFactor;

    private void Start() => playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    void FixedUpdate() => Follow();
    void Follow()
    {
        Vector3 targetPosition = playerTransform.position + offset; //Primer vector de posición con el offset que establecí desde el inicio.
        Vector3 boundPosition = new Vector3(Mathf.Clamp(targetPosition.x, minLimit.x, maxLimit.x),
                                            Mathf.Clamp(targetPosition.y, minLimit.y, maxLimit.y),
                                            targetPosition.z); //Este segundo vector será para seguir la cámara hasta ciertos límites establecidos
        Vector3 smoothPosition = Vector3.Lerp(transform.position, boundPosition, smoothFactor * Time.deltaTime); //Y aquí me permitirá mover la cámara suavemente
        transform.position = smoothPosition; //La posición de la camara finalmente será esta
    }
}
