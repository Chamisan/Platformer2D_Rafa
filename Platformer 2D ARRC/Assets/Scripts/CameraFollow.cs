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
        Vector3 targetPosition = playerTransform.position + offset; //Primer vector de posici�n con el offset que establec� desde el inicio.
        Vector3 boundPosition = new Vector3(Mathf.Clamp(targetPosition.x, minLimit.x, maxLimit.x),
                                            Mathf.Clamp(targetPosition.y, minLimit.y, maxLimit.y),
                                            targetPosition.z); //Este segundo vector ser� para seguir la c�mara hasta ciertos l�mites establecidos
        Vector3 smoothPosition = Vector3.Lerp(transform.position, boundPosition, smoothFactor * Time.deltaTime); //Y aqu� me permitir� mover la c�mara suavemente
        transform.position = smoothPosition; //La posici�n de la camara finalmente ser� esta
    }
}
