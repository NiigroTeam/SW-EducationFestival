using UnityEngine;
using DG.Tweening;

public class ProjectileDamage : MonoBehaviour
{
    [Header("데미지 설정")]
    public float damage = 20f;

    [Header("투사체 설정")]
    public float lifetime = 5f; 
    public bool isActive = true;

    [Header("피격 효과 (보스용)")]
    public Color damageColor = Color.red;
    public float flashDuration = 0.1f;

    private Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        if (col == null)
        {
            Debug.LogError("ProjectileDamage 오류: Collider2D가 필요합니다!");
            enabled = false;
            return;
        }
    }

    void Start()
    {
        // 자동 파괴 시간
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive) return;

        // BossManager에 닿았는지 확인
        BossManager boss = collision.GetComponent<BossManager>();

        if (boss != null)
        {
            // 데미지 전달 (보스Hit에서 처리)
            // 여기서는 삭제도, 히트 체크도 없음
        }
    }

    public void Activate()
    {
        isActive = true;
    }

    public void Deactivate()
    {
        isActive = false;
    }
}