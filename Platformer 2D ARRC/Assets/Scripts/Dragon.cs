using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : MonoBehaviour
{
    [SerializeField] Transform metaPoint;
    [SerializeField] GameObject dragonEye;
    Animator animatorDragon;
    void Start()
    {
        animatorDragon = GetComponent<Animator>();
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
        animatorDragon.SetTrigger("death");
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        Instantiate(dragonEye, metaPoint.position, metaPoint.rotation);
        Destroy(gameObject, 1);
    }
}
