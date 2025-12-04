using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections; 

public class BossController : MonoBehaviour
{
    public GameObject slowProjectilePrefab; 
    public Transform firePoint;             
    public Transform player;                
    public AudioSource audio;
    public AudioClip clip;

    public Animator animator;               
    public string attackTriggerName = "AttackTrigger"; 
    
    // ⭐ HP 바 오브젝트를 이름으로 찾기 위한 필드 추가 ⭐
    public string HpbarName = "BossHpbar_S"; // 인스펙터에서 설정 가능
    
    [Header("공격 애니메이션 길이")]
    public float attackAnimationDuration = 0.5f; 
    
    public float fireInterval = 3f; 
    private bool isAttacking = false;
    public Health bossHealth; 
    
    public Image bossHpImage; 
    
    private Coroutine attackRoutineCoroutine; 
    private Coroutine attackLoopCoroutine;    

    // ----------------------------------------------------
    // 🌟 GameStartTrigger 컴포넌트를 참조하기 위한 변수
    // ----------------------------------------------------
    private Component gameStartTrigger; 


    void Start()
    {
        if (bossHealth == null)
            bossHealth = GetComponent<Health>();

        // ===============================================
        // 🌟 수정: "BossHpbar" 태그 대신 'HpbarName' 변수로 오브젝트를 찾아 Image 연결 🌟
        // ===============================================
        
        // ✨ Health 스크립트에 UI Image 연결
        if (bossHealth != null && bossHpImage != null)
        {
            bossHealth.hpImage = bossHpImage;
            
            Debug.Log("✅ BossController: Health 스크립트와 Image HP 바 연결 완료.");
        }
        // ===============================================

        // 🌟 GameStartTrigger 찾기 및 코루틴 시작
        gameStartTrigger = (Component)FindObjectOfType(typeof(GameStartTrigger)); 
        
        if (gameStartTrigger == null)
        {
            Debug.LogError("⚠️ 씬에서 'GameStartTrigger' 컴포넌트를 찾을 수 없습니다. 공격 루프를 즉시 시작합니다.");
            attackLoopCoroutine = StartCoroutine(AttackLoopCoroutine());
        }
        else
        {
            StartCoroutine(CheckGameStartAndAttackLoop());
        }
    }
    
    // ----------------------------------------------------
    // 🌟 추가된 코루틴: 게임 시작 트리거 체크
    // ----------------------------------------------------
    private IEnumerator CheckGameStartAndAttackLoop()
    {
        GameStartTrigger trigger = gameStartTrigger as GameStartTrigger;

        if (trigger == null) 
        {
            Debug.LogError("GameStartTrigger 컴포넌트를 찾았으나 형 변환에 실패했습니다. 공격 루프를 시작합니다.");
            attackLoopCoroutine = StartCoroutine(AttackLoopCoroutine());
            yield break;
        }

        Debug.Log("Waiting for GameStartTrigger...");
        
        while (!trigger.hasTriggered)
        {
            yield return null; 
        }

        Debug.Log("GameStartTrigger detected! Starting Attack Loop.");
        
        attackLoopCoroutine = StartCoroutine(AttackLoopCoroutine());
    }
    
    // ----------------------------------------------------
    // 🌟 코루틴: 무한 공격 루프 
    // ----------------------------------------------------
    private IEnumerator AttackLoopCoroutine()
    {
        while (true)
        {
            if (!isAttacking)
            {
                Attack();
            }
            yield return new WaitForSeconds(fireInterval);
        }
    }

    void Update()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player")?.transform;
    }

    void Attack()
    {
        if (attackRoutineCoroutine != null)
        {
            StopCoroutine(attackRoutineCoroutine);
        }
        attackRoutineCoroutine = StartCoroutine(AttackRoutine());
    }

    // ----------------------------------------------------
    // ⚔️ 코루틴: 공격 애니메이션 재생 및 상태 관리
    // ----------------------------------------------------
    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        if (animator != null)
        {
            animator.SetTrigger(attackTriggerName);
        }
        yield return new WaitForSeconds(attackAnimationDuration);

        isAttacking = false;
        attackRoutineCoroutine = null;
    }

    public void FireSlowProjectile()
    {
        if (slowProjectilePrefab == null || firePoint == null || player == null)
            return;

        audio.PlayOneShot(clip);
        GameObject proj = Instantiate(slowProjectilePrefab, firePoint.position, Quaternion.identity);
        SlowProjectile sp = proj.GetComponent<SlowProjectile>();
        if (sp != null)
            sp.SetTarget(player.position);
    }
}