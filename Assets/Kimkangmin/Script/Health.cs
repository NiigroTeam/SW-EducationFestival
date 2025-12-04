using System.Collections;
using UnityEngine;
using UnityEngine.UI; 
using DG.Tweening; 

public class Health : MonoBehaviour
{
    public float maxHP = 100f;
    public float currentHP;

    [Tooltip("HP를 표시할 Image 컴포넌트 (Type: Filled)")]
    public Image hpImage;
    public Canvas canvas;
    
    // ⭐ 무적 시간 설정 필드 (0.25초) ⭐
    [Header("무적 설정")]
    public float invincibilityDuration = 0.25f; // 기존 'delay' 변수 대체
    
    public float smoothDuration = 0.5f; // DOTween 지속 시간
    public float Damage = 0;
    
    // ⭐ 무적 상태를 추적하는 플래그 ⭐
    private bool isInvincible = false;
    
    private Coroutine invincibilityCoroutine; // 코루틴 참조 변수

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))TakeDamage(maxHP);
    }

    void Start()
    {
        currentHP = maxHP;
        
        if (hpImage != null) 
        {
            hpImage.fillAmount = currentHP / maxHP;
        }
    }

    public void TakeDamage(float amount)
    {
        // ⭐ HP 변경 로직 ⭐
        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        
        Debug.Log($"✅ 데미지 적용! 오브젝트: {gameObject.name}, New CurrentHP: {currentHP}");
        
        // 🌟 HP 시각화 업데이트 (DOTween 사용) 🌟
        if (hpImage != null)
        {
            float targetFill = currentHP / maxHP;
            
            DOTween.Kill(hpImage);
            DOTween.To(() => hpImage.fillAmount, 
                       x => hpImage.fillAmount = x,
                       targetFill, smoothDuration)
                   .SetEase(Ease.OutCubic);
        }

        // ⭐ 핵심 조건: HP가 0 이하일 때만 사망 처리 ⭐
        if (currentHP <= 0)
            Die(); 
            
        // ⚠️ TakeDamage 함수에서 무적 플래그를 건드리는 로직은 제거해야 합니다.
        //    무적 플래그는 코루틴이 전적으로 관리해야 합니다.
    }

    void Die()
    {
        
        Debug.Log($"{gameObject.name} 사망!");
        
        // 무적 코루틴이 실행 중이라면 멈춥니다.
        if (invincibilityCoroutine != null)
        {
            StopCoroutine(invincibilityCoroutine);
        }
        if (hpImage != null)
        {
            canvas.gameObject.SetActive(false);
        }
        // 1. 꼬리(뱀) 사망 시 BossPhaseManager 호출 (페이즈 전환)
        bool isSnake = gameObject.name.Contains("Snake-slow_0") || gameObject.name.Contains("꼬리");
        if (isSnake) 
        {
            BossPhaseManager manager = FindObjectOfType<BossPhaseManager>(); 
            if (manager != null)
            {
                manager.OnSnakeKilled(); 
            }
        }
        
        // 2. 사망 오브젝트의 모든 스크립트 비활성화
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this) 
            {
                script.enabled = false;
            }
        }
        
        // 3. ⭐ 뱀 오브젝트 즉시 파괴 (지연 시간 제거) ⭐
        if (isSnake)
        {
            Destroy(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        // 🌟 수정: isInvincible이 false일 때만 데미지를 입힙니다.
        if (collision.gameObject.tag == "Weapon" && !isInvincible)
        {
            TakeDamage(Damage);
            
            // 무적 코루틴 시작
            invincibilityCoroutine = StartCoroutine(InvincibilityCoroutine());
        }
    }

    // ----------------------------------------------------
    // ⭐ 코루틴: 무적 상태 관리 ⭐
    // ----------------------------------------------------
    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true; // 무적 상태 시작
        
        // 지정된 시간만큼 대기 (0.25초)
        yield return new WaitForSeconds(invincibilityDuration);
        
        isInvincible = false; // 무적 상태 종료
        invincibilityCoroutine = null;
    }
}