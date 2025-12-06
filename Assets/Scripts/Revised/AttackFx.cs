using System;
using UnityEngine;

public class AttackFx : MonoBehaviour
{
    [SerializeField] private float damage;
    void Start()
    {
        Destroy(gameObject, 5f);
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            Player.Instance.takeDamage(damage);
        }

    }
    void Update()
    {

    }
}