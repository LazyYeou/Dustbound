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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isAttacking = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Creep
        if (isAttacking == false)
        {
            //Face towards player
            if (Player.Instance.transform.position.x > transform.position.x)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
            // Move towards player
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
    //Attack (Called at the end of attack animation)
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