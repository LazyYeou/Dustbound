using Unity.Mathematics;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float health;
    [SerializeField] private float range;
    [SerializeField] private int experienceToGive;
    [SerializeField] private GameObject attackFx;
    [SerializeField] private GameObject deathFx;
    [SerializeField] private Animator animator;

    private Vector3 direction;
    private bool isAttacking;
    void Start()
    {
        isAttacking = false;
    }

    void FixedUpdate()
    {
        //Creep
        if (isAttacking == false)
        {
            if (Player.Instance.transform.position.x > transform.position.x)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
            if ((Player.Instance.transform.position - transform.position).magnitude > range)
            {
                direction = (Player.Instance.transform.position - transform.position).normalized;
                rb.linearVelocity = new Vector2(direction.x * moveSpeed, direction.y * moveSpeed);
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
                animator.Play("Attack");
                isAttacking = true;
            }
        }

    }
    public void Attack()
    {
        Instantiate(attackFx, transform.position + (Player.Instance.transform.position - transform.position).normalized * range, transform.rotation);
        isAttacking = false;

    }
    //Take Damage
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
            Instantiate(deathFx, transform.position, transform.rotation);
            //Player.Instance.GetExperience(experienceToGive);
        }
    }
}