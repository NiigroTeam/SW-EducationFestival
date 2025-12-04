using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HitboxDamage : MonoBehaviour
{
    public int damage = 10;
    public bool isActive = false;
    private Collider2D col;
    
    // 피해를 입힌 대상을 추적할 HashSet 추가
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>(); 

    void Awake()
    {
        col = GetComponent<Collider2D>();
        
        // ⭐ 안전성 강화: Collider2D가 없는 경우 오류 방지
        if (col == null)
        {
            Debug.LogError("HitboxDamage 스크립트 오류: 이 오브젝트에는 Collider2D 컴포넌트가 필요합니다! 스크립트 비활성화.");
            enabled = false; 
            return;
        }
        
        // 초기에는 히트박스를 비활성화합니다.
        col.enabled = false;
    }

    public void Activate(float duration)
    {
        // Collider2D가 없다면 (Awake에서 오류가 났다면) 실행하지 않습니다.
        if (col == null) return; 

        isActive = true;
        col.enabled = true;
        hitTargets.Clear(); // 활성화 시 HashSet 초기화

        // Invoke를 사용하여 지정된 시간 후에 비활성화
        Invoke(nameof(Deactivate), duration);
    }

    void Deactivate()
    {
        // Collider2D가 없다면 실행하지 않습니다.
        if (col == null) return;
        
        isActive = false;
        col.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive) return;

        GameObject target = collision.gameObject;

        // 이미 피해를 입힌 대상인지 확인하여 단발성 피해를 구현합니다.
        if (hitTargets.Contains(target)) return; 

        EnemyHP enemy = collision.GetComponent<EnemyHP>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            hitTargets.Add(target); // 피해를 입힌 후 기록
        }
    }
}