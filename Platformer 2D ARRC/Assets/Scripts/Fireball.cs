using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] float fireballSpeed = 5;
    Transform playerTransform;
    float fireDirection;
    bool fire;
    private void Start()
    {
        playerTransform = GameObject.Find("Player").GetComponent<Transform>(); 
        fire = false;
        transform.localScale = new Vector3(playerTransform.localScale.x, transform.localScale.y, transform.localScale.z); //Para la dirección del objeto instanciado
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Foe"))
            Destroy(gameObject);
    }
    private void Update() => FireBehavior();
    private void FireBehavior()
    {
        if (!fire)
            fireDirection = playerTransform.localScale.x;
        transform.Translate(Vector3.right * fireDirection * fireballSpeed * Time.deltaTime);
        fire = true;
    }
}

