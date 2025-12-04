using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI; 
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.SceneManagement; // SceneManager가 필요할 수 있으나, SceneFader가 처리함

public class Health_Die : MonoBehaviour
{
    public float maxHP = 100f;
    public float currentHP;

    [Tooltip("HP를 표시할 Image 컴포넌트 (Type: Filled)")]
    public Image hpImage;
    public Canvas canvas;
    
    // ⭐ 사망 시 이동할 씬 이름 ⭐
    [Header("씬 전환")]
    public string nextSceneName = "NextLevelScene"; // 인스펙터에서 다음 씬 이름 설정
    
    // ⭐ 무적 시간 설정 필드 (0.25초) ⭐
    [Header("무적 설정")]
    public float invincibilityDuration = 0.25f;
    
    public float smoothDuration = 0.5f; 
    public float Damage = 0;
    
    private bool isInvincible = false;
    private Coroutine invincibilityCoroutine;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))TakeDamage(maxHP);
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
        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        
        Debug.Log($"✅ 데미지 적용! 오브젝트: {gameObject.name}, New CurrentHP: {currentHP}");
        
        if (hpImage != null)
        {
            float targetFill = currentHP / maxHP;
            
            DOTween.Kill(hpImage);
            DOTween.To(() => hpImage.fillAmount, 
                       x => hpImage.fillAmount = x,
                       targetFill, smoothDuration)
                   .SetEase(Ease.OutCubic);
        }

        if (currentHP <= 0)
            Die(); 
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} 사망! 씬 전환 시작.");
        
        // 무적 코루틴이 실행 중이라면 멈춥니다.
        if (invincibilityCoroutine != null)
        {
            StopCoroutine(invincibilityCoroutine);
        }

        // HP 바와 캔버스 비활성화 (사망 처리 시 UI 숨김)
        if (canvas != null)
        {
            canvas.gameObject.SetActive(false);
        }

        // ----------------------------------------------------
        // ⭐⭐ 씬 전환 로직 추가 ⭐⭐
        // ----------------------------------------------------
        if (SceneFader.Instance != null && !string.IsNullOrEmpty(nextSceneName))
        {
            // 씬 페이드 아웃을 시작하고 다음 씬으로 이동하도록 SceneFader에 명령합니다.
            SceneFader.Instance.FadeToScene(nextSceneName);
            
            // 씬 전환이 시작되었으므로, 오브젝트 파괴나 스크립트 비활성화는 
            // 씬 전환이 완료될 때까지 잠시 지연하거나 생략하는 것이 좋습니다.
            // 여기서는 스크립트 비활성화 및 파괴를 씬 전환 명령 후에 수행합니다.
        }
        else
        {
            // SceneFader가 없거나 씬 이름이 설정되지 않았을 경우 경고
            Debug.LogError("SceneFader 인스턴스를 찾을 수 없거나 nextSceneName이 설정되지 않아 씬 전환을 할 수 없습니다.");
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
        // (씬 전환 중에도 잔여 동작 방지를 위해 필요)
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this) 
            {
                script.enabled = false;
            }
        }
        
        // 3. ⭐ 오브젝트 파괴 (씬 전환이 시작되었으므로 즉시 파괴해도 무방) ⭐
        Destroy(gameObject); 
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Weapon" && !isInvincible)
        {
            TakeDamage(Damage);
            
            // 무적 코루틴 시작
            invincibilityCoroutine = StartCoroutine(InvincibilityCoroutine());
        }
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        
        yield return new WaitForSeconds(invincibilityDuration);
        
        isInvincible = false;
        invincibilityCoroutine = null;
    }
}