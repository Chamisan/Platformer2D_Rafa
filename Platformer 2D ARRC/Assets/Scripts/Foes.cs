using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foes : MonoBehaviour
{
    [SerializeField] float foeVelocity;
    Animator animatorFoe;
    //bool bat;
    //bool skull;
    void Start()
    {
        animatorFoe = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.transform != null && collision.gameObject.transform.parent != null &&
    collision.gameObject.transform.parent.TryGetComponent(out Player player) && player.onAttack)
        //        if (collision.gameObject.transform.parent.TryGetComponent(out Player player) && player.onAttack)
        {
            Die();
            GameManager.points += 50;
        }
            
        if (collision.gameObject.CompareTag("Fire"))
        {
            Die();
            GameManager.points += 250;
        }
            
    }
    private void Die()
    {
        animatorFoe.SetTrigger("death");
        Destroy(gameObject, 0.8f);
    }
}

