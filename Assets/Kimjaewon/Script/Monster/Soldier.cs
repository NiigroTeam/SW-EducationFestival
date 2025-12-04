using UnityEngine;

public class Soldier : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHealth = 3; // 최대 체력
    private int currentHealth; // 현재 체력

    [Header("이동 설정")]
    public Transform player;
    public float moveSpeed = 3f;
    public float detectionRange = 10f;
    public float stoppingDistance = 1.5f;

    [Header("공격 설정")]
    public int damageAmount = 1; // 1회당 피해량
    public float damageInterval = 1f; // 지속 피해 간격
    private float lastDamageTime;
    private bool isTouchingPlayer = false;
    private GameObject playerObject;

    private Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();

        // Rigidbody2D 설정: Kinematic으로 해서 밀림 방지
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;

        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindWithTag("Player");
            if (foundPlayer != null)
                player = foundPlayer.transform;
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 탐지 범위 내 이동
        if (distanceToPlayer <= detectionRange && distanceToPlayer > stoppingDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        // 접촉 중일 때 지속 피해
        if (isTouchingPlayer && Time.time >= lastDamageTime + damageInterval)
        {
            DealDamageToPlayer();
            lastDamageTime = Time.time;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = true;
            playerObject = collision.gameObject;

            // 닿자마자 즉시 1회 피해
            DealDamageToPlayer();
            lastDamageTime = Time.time;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = false;
        }
    }

    void DealDamageToPlayer()
    {
        if (playerObject != null)
        {
            PlayerHealth playerHealth = playerObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }
}
