using UnityEngine;

public class Sword : MonoBehaviour
{
    private const float Q_COOLDOWN_TIME = 1f;
    private const float E_COOLDOWN_TIME = 3f;
    private const float R_COOLDOWN_TIME = 10f;

    [Header("참(베기)")]
    public HitboxDamage slashHitbox;
    public float slashDuration = 0.15f;
    
    // [수정: 효과음] 👈 참(베기) 효과음
    [Header("Audio Clips")]
    public AudioClip slashSound; 

    [Header("지옥참마도")]
    public HitboxDamage ultimateHitbox;
    public float ultimateDuration = 0.4f;
    
    // [수정: 효과음] 👈 지옥참마도 효과음
    public AudioClip ultimateSound; 

    [Header("돌진 + 검기 발사")]
    public float dashForce = 8f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 25f;

    // [수정: 효과음] 👈 돌진+검기 효과음
    public AudioClip dashSlashSound; 

    [Header("생성 위치")]
    public float spawnDistance = 1.0f;
    
    // [수정: 효과음] 👈 AudioSource 컴포넌트 추가
    private AudioSource audioSource; 

    private Transform playerTransform;
    private Rigidbody2D playerRb;
    private Animator playerAnimator;
    private Quaternion mouseRotation;

    [Header("칼 위치 오프셋")]
    public Vector3 localOffset = new Vector3(0.5f, -0.2f, 0);

    [Header("쿨타임 타이머 (UI용)")]
    public float qTimer { get; private set; }
    public float eTimer { get; private set; }
    public float rTimer { get; private set; }

    void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            playerRb = playerObj.GetComponent<Rigidbody2D>();
            playerAnimator = playerObj.GetComponent<Animator>();
        }
        
        // [수정: 효과음] 👈 AudioSource 컴포넌트 가져오기 시도
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("Sword 스크립트는 AudioSource 컴포넌트가 필요합니다!");
        }
    }

    void Update()
    {
        // 쿨타임 감소
        qTimer -= Time.deltaTime;
        eTimer -= Time.deltaTime;
        rTimer -= Time.deltaTime;

        if (qTimer < 0) qTimer = 0f;
        if (eTimer < 0) eTimer = 0f;
        if (rTimer < 0) rTimer = 0f;

        if (playerTransform == null) return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseWorld - playerTransform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        mouseRotation = Quaternion.Euler(0, 0, angle);

        transform.position = playerTransform.position + localOffset;
        transform.rotation = mouseRotation;

        if (Input.GetMouseButtonDown(0)) UseSlash();
        if (Input.GetKeyDown(KeyCode.E)) UseDashSlash();
        if (Input.GetKeyDown(KeyCode.Q)) UseUltimate();
    }
    
    // [수정: 효과음] 👈 효과음 재생 공통 함수
    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void UseSlash()
    {
        if (qTimer > 0 || !IndividualSkillCooldown.instance.qActive) return;

        // [수정: 효과음] 👈 사운드 재생
        PlaySound(slashSound);

        playerAnimator?.SetTrigger("Slash");
        if (slashHitbox != null)
        {
            Vector2 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - playerTransform.position).normalized;
            Vector3 spawnPos = playerTransform.position + (Vector3)dir * spawnDistance;
            HitboxDamage hitbox = Instantiate(slashHitbox, spawnPos, mouseRotation);
            hitbox.Activate(slashDuration);
            Destroy(hitbox.gameObject, slashDuration + 0.1f);
        }

        IndividualSkillCooldown.instance.StartCooldown(0);
        qTimer = Q_COOLDOWN_TIME;
    }

    void UseDashSlash()
    {
        if (eTimer > 0 || !IndividualSkillCooldown.instance.eActive) return;

        // [수정: 효과음] 👈 사운드 재생
        PlaySound(dashSlashSound);

        Vector2 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - playerTransform.position).normalized;

        // 대시 동작은 기존 코드에 없으므로 (돌진+검기) 검기 발사만 유지합니다.
        // 만약 여기에 대시 동작을 추가하려면 playerRb.AddForce나 Transform.Translate 등을 사용해야 합니다.
        // 현재는 검기 발사만 구현되어 있습니다.

        if (projectilePrefab != null)
        {
            Vector3 spawnPos = playerTransform.position + (Vector3)dir * spawnDistance;
            GameObject proj = Instantiate(projectilePrefab, spawnPos, mouseRotation);
            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = dir * projectileSpeed;
        }

        IndividualSkillCooldown.instance.StartCooldown(1);
        eTimer = E_COOLDOWN_TIME;
    }

    void UseUltimate()
    {
        if (rTimer > 0 || !IndividualSkillCooldown.instance.rActive) return;

        // [수정: 효과음] 👈 사운드 재생
        PlaySound(ultimateSound);

        playerAnimator?.SetTrigger("Ultimate");
        if (ultimateHitbox != null)
        {
            HitboxDamage hitbox = Instantiate(ultimateHitbox, playerTransform.position, Quaternion.identity);
            hitbox.Activate(ultimateDuration);
            Destroy(hitbox.gameObject, ultimateDuration + 0.1f);
        }

        IndividualSkillCooldown.instance.StartCooldown(2);
        rTimer = R_COOLDOWN_TIME;
    }
}