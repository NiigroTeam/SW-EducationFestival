using UnityEngine;

public class Spear : MonoBehaviour
{
    private const float Q_COOLDOWN_TIME = 1f;
    private const float E_COOLDOWN_TIME = 3f;
    private const float R_COOLDOWN_TIME = 10f;
    
    public AudioSource DashSound;

    [Header("Q - Wield")]
    public HitboxDamage wieldHitbox;
    public float wieldDuration = 0.15f;
    public AudioClip Attack1;

    [Header("E - Sting")]
    public HitboxDamage stingHitbox;
    public float stingDuration = 0.25f;
    public AudioClip Attack2;

    [Header("R - Haymaker")]
    public HitboxDamage haymakerHitbox;
    public float haymakerDuration = 0.4f;
    public AudioClip Attack3;

    [Header("히트박스 생성 거리")]
    public float spawnDistance = 1.0f;

    private Transform playerTransform;
    private Rigidbody2D playerRb;
    private Animator playerAnimator;
    private Quaternion mouseRotation;

    [Header("무기 위치 오프셋")]
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

        if (Input.GetMouseButtonDown(0)) UseWield();
        if (Input.GetKeyDown(KeyCode.E)) UseSting();
        if (Input.GetKeyDown(KeyCode.Q)) UseHaymaker();
    }

    void UseWield()
    {
        if (qTimer > 0 || !IndividualSkillCooldown.instance.qActive) return;
        PlaySound(Attack1);
        playerAnimator?.SetTrigger("Wield");
        SpawnHitbox(wieldHitbox, wieldDuration);
        IndividualSkillCooldown.instance.StartCooldown(0);
        qTimer = Q_COOLDOWN_TIME;
    }

    void UseSting()
    {
        if (eTimer > 0 || !IndividualSkillCooldown.instance.eActive) return;
        PlaySound(Attack2);
        playerAnimator?.SetTrigger("Sting");
        SpawnHitbox(stingHitbox, stingDuration);
        IndividualSkillCooldown.instance.StartCooldown(1);
        eTimer = E_COOLDOWN_TIME;
    }

    void UseHaymaker()
    {
        if (rTimer > 0 || !IndividualSkillCooldown.instance.rActive) return;
        PlaySound(Attack3);
        playerAnimator?.SetTrigger("Haymaker");
        SpawnHitbox(haymakerHitbox, haymakerDuration);
        IndividualSkillCooldown.instance.StartCooldown(2);
        rTimer = R_COOLDOWN_TIME;
    }

    void SpawnHitbox(HitboxDamage prefab, float duration)
    {
        if (prefab == null) return;
        Vector2 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - playerTransform.position).normalized;
        Vector3 spawnPos = playerTransform.position + (Vector3)dir * spawnDistance;
        HitboxDamage hitbox = Instantiate(prefab, spawnPos, mouseRotation);
        hitbox.Activate(duration);
        Destroy(hitbox.gameObject, duration + 0.1f);
    }
    void PlaySound(AudioClip clip)
    {
        if (DashSound != null && clip != null)
        {
            DashSound.PlayOneShot(clip);
        }
    }
}
