using UnityEngine;

public class Foes : MonoBehaviour
{
    [SerializeField] float foeVelocity;
    [SerializeField] AudioClip clipFoeDie;
    Animator animatorFoe;
    //AudioSource audioFoeDie;
    void Start() => animatorFoe = GetComponent<Animator>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.transform != null && collision.gameObject.transform.parent != null &&
        collision.gameObject.transform.parent.TryGetComponent(out Player player) && player.onAttack)
        // Todo este if para saber si está el objeto Player en modo ataque porque su trigger de ataque es un hijo de ese objeto
        {
            Die();
            GameManager.points += 50;;
        }
        if (collision.gameObject.CompareTag("Fire"))
        {
            Die();
            GameManager.points += 250;
        }       
    }
    private void Die()
    {
        AudioManager.instance.PlaySound(clipFoeDie);
        animatorFoe.SetTrigger("death");
        gameObject.GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 0.8f);
    }
}

