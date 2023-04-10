using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foes : MonoBehaviour
{
    [SerializeField] float foeVelocity;
    Animator animatorFoe;
    bool bat;
    bool skull;
    void Start()
    {
        animatorFoe = GetComponent<Animator>();
    }

    void Update()
    {
        //if (bat)
           //transform.position += Vector3()
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.transform.parent.TryGetComponent(out Player player) && player.onAttack)
            Die();
        if (collision.gameObject.CompareTag("Fire"))
            Die();
    }
    private void Die()
    {
        animatorFoe.SetTrigger("death");
        Destroy(gameObject, 1.1f);
    }
}

