using System;
using UnityEngine;

public class AttackFx : MonoBehaviour
{
    [SerializeField] private float damage;
    // Start is called once bef5ore the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, 5f);
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        //Hit player
        if (collider.CompareTag("Player"))
        {
            Player.Instance.takeDamage(damage);
        }

    }
    void Update()
    {

    }
}