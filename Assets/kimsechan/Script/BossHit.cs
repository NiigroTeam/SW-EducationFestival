using UnityEngine;
using DG.Tweening;

public class BossHit : MonoBehaviour
{
    [Header("Damage Effect Settings")]
    public Color damageColor = Color.red;
    public float flashDuration = 0.1f;
    public float bong = 0;

    [Header("Hit Cooldown Settings")]
    public float hitCooldown = 0.3f;   // ← 이 시간 동안 데미지 안 받음
    private float lastHitTime = -999f;

    private BossManager bossManager;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        bossManager = FindObjectOfType<BossManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryApplyDamage(collision.gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryApplyDamage(other.gameObject);
    }

    private void TryApplyDamage(GameObject obj)
    {
        if (Time.time - lastHitTime < hitCooldown) return; // ← 딜레이 적용

        float damageToApply = 0f;
        bool gotDamage = false;

        HitboxDamage hitbox = obj.GetComponent<HitboxDamage>();
        if (hitbox != null && hitbox.isActive)
        {
            damageToApply = hitbox.damage;
            gotDamage = true;
        }

        ProjectileDamage projectile = obj.GetComponent<ProjectileDamage>();
        if (projectile != null && projectile.isActive)
        {
            damageToApply = projectile.damage;
            gotDamage = true;
        }

        if (gotDamage)
        {
            lastHitTime = Time.time;  // ← 마지막 히트 시간 갱신
            bossManager.TakeDamage(damageToApply * (100 / (100 + bong)));
            FlashDamageEffect();
        }
    }

    private void FlashDamageEffect()
    {
        if (spriteRenderer == null) return;

        spriteRenderer.DOKill(true);
        spriteRenderer.color = damageColor;
        spriteRenderer.DOColor(Color.white, flashDuration);
    }
}