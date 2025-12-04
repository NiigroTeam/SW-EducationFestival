using UnityEngine;

public class Dagger : MonoBehaviour
{
    private const float Q_COOLDOWN_TIME = 1f;
    private const float E_COOLDOWN_TIME = 3f;
    private const float R_COOLDOWN_TIME = 10f;
    
    public AudioSource DashSound;

    [Header("참(베기)")]
    public HitboxDamage slashHitbox;
    public float slashDuration = 0.15f;
    public AudioClip Attack1;

    [Header("지옥참마도")]
    public HitboxDamage ultimateHitbox;
    public float ultimateDuration = 0.4f;
    public AudioClip Attack2;

    [Header("돌진 + 검기 발사")]
    public float dashForce = 8f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 25f;
    public AudioClip Attack3;
    private AudioSource audioSource; 

    [Header("생성 위치")]
    public float spawnDistance = 1.0f;

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
            audioSource = playerObj.GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        qTimer -= Time.deltaTime;
        eTimer -= Time.deltaTime;
        rTimer -= Time.deltaTime;

        if (qTimer < 0) qTimer = 0f;
        if (eTimer < 0) eTimer = 0f;
        if (rTimer < 0) rTimer = 0f;

        if (playerTransform == null) return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mouseWorld - playerTransform.position).normalized;
        mouseRotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

        transform.position = playerTransform.position + localOffset;
        transform.rotation = mouseRotation;

        if (Input.GetMouseButtonDown(0)) UseSlash();
        if (Input.GetKeyDown(KeyCode.E)) UseDashSlash();
        if (Input.GetKeyDown(KeyCode.Q)) UseUltimate();
    }

    void UseSlash()
    {
        if (qTimer > 0 || !IndividualSkillCooldown.instance.qActive) return;
        PlaySound(Attack1);
        
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
        PlaySound(Attack2);

        Vector2 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - playerTransform.position).normalized;

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
        PlaySound(Attack3);

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
    void PlaySound(AudioClip clip)
    {
        if (DashSound != null && clip != null)
        {
            DashSound.PlayOneShot(clip);
        }
    }
}
