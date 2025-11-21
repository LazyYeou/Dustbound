using System;
using UnityEngine;

public class AttackFx : MonoBehaviour
{
    [SerializeField] private float damage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, .2f);
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        //Hit player
        if (collision.gameObject.CompareTag("Player"))
        {
            //Player.Instance.TakeDamage(damage);
        }

    }
    void Update()
    {

    }
}