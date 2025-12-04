using UnityEngine;
using DG.Tweening; // DOTween 필요

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public Collider2D coll;
    public AudioSource audioSource;
    [Header("Sprite Root (좌우반전용)")]
    public Transform spriteRoot;
    
    // [수정: 효과음] 👈 대쉬 효과음을 설정할 클립 변수 추가
    [Header("Audio Settings")]
    [Tooltip("대쉬 시작 시 재생할 효과음 클립")]
    public AudioClip dashSound;
    
    // --- DOTween 회전 설정 ---
    [Header("DOTween Rotation Settings")]
    [Tooltip("대쉬 중 스프라이트가 회전할 각도 (예: 360 또는 720)")]
    public float dashRotationAmount = 360f;
    
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    
    [Header("Dash Settings")]
    public float dashPower = 15f;
    public float dashDuration = 0.15f;
    // public float dashCooldown = 1f; // 👈 UI 스크립트에서 관리하므로 제거 (주석 처리)

    // --- 무적 설정 (레이어 방식) ---
    [Header("Invulnerability Settings")]
    [Tooltip("무적 상태에서 사용할 Layer 이름 (Enemy와 충돌 해제되어야 함)")]
    public string dashLayerName = "DashingPlayer"; // 유니티에서 설정 필요
    
    // --- UI 연동 설정 ---
    private const int DASH_SKILL_INDEX = 3; // 👈 IndividualSkillCooldown 스크립트에서 대쉬의 인덱스는 3으로 가정

    private bool isDashing = false;
    private float dashTimer = 0f;
    // private float dashCooldownTimer = 0f; // 👈 UI 스크립트에서 관리하므로 제거 (주석 처리)
    private Vector2 movementInput;
    private Vector2 lastMoveDir = Vector2.right;
    
    private Vector3 originalScale;

    private int originalLayer; // 플레이어의 원래 Layer 인덱스
    private int dashLayer;     // DashingPlayer Layer의 인덱스
    
    // ---------------- Start & Update ----------------

    void Awake()
    {
        // Rigidbody2D, Collider2D 컴포넌트 가져오기
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (coll == null) coll = GetComponent<Collider2D>();
        
        // Sprite Root의 최초 스케일 저장
        if (spriteRoot != null)
        {
            originalScale = spriteRoot.localScale;   
        }
        else
        {
            Debug.LogError("Sprite Root(좌우반전용) Transform이 설정되지 않았습니다.");
        }

        // 레이어 인덱스 미리 가져오기
        originalLayer = gameObject.layer;
        dashLayer = LayerMask.NameToLayer(dashLayerName);
        
        if (dashLayer == -1)
        {
            Debug.LogError($"Physics2D 설정에 '{dashLayerName}' 레이어가 존재하지 않습니다! 무적 기능이 작동하지 않습니다.");
        }
    }
    
    void Update()
    {
        HandleInput();
        Move();
        HandleFlip();
        DashInput();
        DashUpdate();
    }
    
    // ---------------- Input ----------------
    void HandleInput()
    {
        if (isDashing) return;
        
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        
        movementInput = new Vector2(x, y).normalized;
        
        // 마지막 이동 방향 저장 (대쉬를 위해)
        if (movementInput.sqrMagnitude > 0.01f)
            lastMoveDir = movementInput;  
    }
    
    // ---------------- Move ----------------
    void Move()
    {
        if (isDashing) return;
        
        transform.Translate(movementInput * moveSpeed * Time.deltaTime);
        
        // 애니메이터 설정
        if (animator != null)
        {
            animator.SetBool("isMoving", movementInput.sqrMagnitude > 0.01f);
        }
    }
    
    // ---------------- Flip ----------------
    void HandleFlip()
    {
        if (spriteRoot == null) return;
        
        // 좌우 반전 처리
        if (movementInput.x < -0.01f)
            spriteRoot.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else if (movementInput.x > 0.01f)
            spriteRoot.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
    }
    
    // ---------------- Dash Input (수정됨) ----------------
    void DashInput()
    {
        if (isDashing) return;

        // 쿨다운 상태를 UI/전역 스크립트를 통해 확인
        bool canDash = IndividualSkillCooldown.instance != null && IndividualSkillCooldown.instance.dashActive;
        
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartDash();
        }
    }
    
    // ---------------- Dash Start (효과음 재생 기능 추가) ----------------
    void StartDash()
    {
        // 이동 입력이 없을 경우 마지막 바라보는 방향으로 대쉬하도록 설정
        if (lastMoveDir == Vector2.zero)
            lastMoveDir = (spriteRoot != null && spriteRoot.localScale.x > 0) ? Vector2.right : Vector2.left; 
        
        isDashing = true;
        dashTimer = dashDuration;
        // dashCooldownTimer = dashCooldown; // 👈 로컬 쿨다운 제거
        
        // 🚨 UI 쿨타임 시작 요청
        if (IndividualSkillCooldown.instance != null)
        {
            IndividualSkillCooldown.instance.StartCooldown(DASH_SKILL_INDEX);
        }
        
        // 🚨 무적 시작: 레이어를 변경하여 적과의 충돌만 무시
        if (dashLayer != -1)
        {
            gameObject.layer = dashLayer;
        }

        // 1. 대쉬 속도 적용
        if (rb != null)
        {
            rb.linearVelocity = lastMoveDir * dashPower; 
        }

        // 2. DOTween 회전 효과 적용
        if (spriteRoot != null)
        {
            spriteRoot.DOKill();
            
            spriteRoot.DOLocalRotate(
                new Vector3(0, 0, dashRotationAmount), // Z축을 기준으로 회전 (2D 시점)
                dashDuration, 
                RotateMode.LocalAxisAdd 
            ).SetEase(Ease.Linear);
        }

        // [수정: 효과음] 👈 대쉬 효과음 재생
        if (audioSource != null && dashSound != null)
        {
            audioSource.PlayOneShot(dashSound);
        }
    }
    
    // ---------------- Dash Update (로컬 쿨다운 제거) ----------------
    void DashUpdate()
    {
        // if (dashCooldownTimer > 0) // 👈 로컬 쿨다운 제거
        //     dashCooldownTimer -= Time.deltaTime; // 👈 로컬 쿨다운 제거
        
        if (!isDashing) return;
        
        // 대쉬 타이머 업데이트
        dashTimer -= Time.deltaTime;
        
        if (dashTimer <= 0)
        {
            isDashing = false;
            
            // 🚨 무적 해제: 원래 레이어로 복귀
            if (dashLayer != -1)
            {
                gameObject.layer = originalLayer;
            }
            
            // 대쉬 종료 시 Rigidbody 속도 초기화
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
            
            // DOTween 회전 효과 즉시 멈추고 원복
            if (spriteRoot != null)
            {
                spriteRoot.DOKill(); 
                spriteRoot.localRotation = Quaternion.identity; 
            }
        }
    }
}