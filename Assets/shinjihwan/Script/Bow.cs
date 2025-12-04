using UnityEngine;
using System.Collections;

public class Bow : MonoBehaviour
{
    [Header("Arrow Prefabs")]
    public GameObject normalArrowPrefab;
    public GameObject spreadArrowPrefab;
    public GameObject fastArrowPrefab;

    [Header("Arrow Speed")]
    public float normalSpeed = 10f;
    public float spreadSpeed = 10f;
    public float fastSpeed = 20f;

    [Header("Spread Settings")]
    public float spreadAngle = 15f;

    [Header("쿨타임 설정")]
    public float qCooldown = 1f;
    public float eCooldown = 3f;
    public float rCooldown = 10f;

    [Header("쿨타임 타이머 (UI용)")]
    public float qTimer { get; private set; }
    public float eTimer { get; private set; }
    public float rTimer { get; private set; }
    
    // [수정: 효과음] 👈 AudioSource 컴포넌트 추가
    [Header("Audio")]
    public AudioSource audioSource;
    [Tooltip("모든 종류의 화살 발사 시 재생할 효과음 클립")]
    public AudioClip shootSound; // [수정: 효과음] 👈 효과음 클립 변수 추가

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        
        // [수정: 효과음] 👈 AudioSource가 없다면 GetComponent로 가져오기 시도
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        // 카메라 없어졌다면 다시 찾기
        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null) return;
        }

        RotateToMouse();

        // 입력 처리
        if (Input.GetMouseButtonDown(0))
            UseNormalShot();

        if (Input.GetKeyDown(KeyCode.E))
            UseSpreadShot();

        if (Input.GetKeyDown(KeyCode.Q))
            UseFastShot();
    }

    // ----------------------
    //   스킬 함수들
    // ----------------------

    void UseNormalShot()
    {
        if (qTimer > 0f) return;
        if (!IndividualSkillCooldown.instance.qActive) return;

        ShootArrow(normalArrowPrefab, transform.right, normalSpeed);
        
        // [수정: 효과음] 👈 효과음 재생 추가
        PlayShootSound();

        // 쿨타임 UI 처리
        IndividualSkillCooldown.instance.StartCooldown(0);

        // 쿨타임 시작
        qTimer = qCooldown;
        StartCoroutine(CooldownTick("Q"));
    }

    void UseSpreadShot()
    {
        if (eTimer > 0f) return;
        if (!IndividualSkillCooldown.instance.eActive) return;

        Vector2 center = transform.right;
        Vector2 left = Rotate(center, -spreadAngle);
        Vector2 right = Rotate(center, spreadAngle);

        ShootArrow(spreadArrowPrefab, left, spreadSpeed);
        ShootArrow(spreadArrowPrefab, center, spreadSpeed);
        ShootArrow(spreadArrowPrefab, right, spreadSpeed);
        
        // [수정: 효과음] 👈 효과음 재생 추가
        PlayShootSound();

        IndividualSkillCooldown.instance.StartCooldown(1);

        eTimer = eCooldown;
        StartCoroutine(CooldownTick("E"));
    }

    void UseFastShot()
    {
        if (rTimer > 0f) return;
        if (!IndividualSkillCooldown.instance.rActive) return;

        ShootArrow(fastArrowPrefab, transform.right, fastSpeed);
        
        // [수정: 효과음] 👈 효과음 재생 추가
        PlayShootSound();

        IndividualSkillCooldown.instance.StartCooldown(2);

        rTimer = rCooldown;
        StartCoroutine(CooldownTick("R"));
    }
    
    // [수정: 효과음] 👈 효과음 재생 전용 메서드 추가
    void PlayShootSound()
    {
        if (audioSource != null && shootSound != null)
        {
            // PlayOneShot을 사용하여 동시에 여러 발이 발사되어도 소리가 끊기지 않도록 처리
            audioSource.PlayOneShot(shootSound);
        }
    }

    // ----------------------
    //   공용 기능
    // ----------------------

    void RotateToMouse()
    {
        if (cam == null) return;

        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mousePos - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void ShootArrow(GameObject prefab, Vector2 direction, float speed)
    {
        if (prefab == null) return;

        GameObject arrow = Instantiate(prefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = direction.normalized * speed;

        arrow.transform.rotation = Quaternion.AngleAxis(
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg,
            Vector3.forward
        );
    }

    Vector2 Rotate(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        return new Vector2(
            Mathf.Cos(rad) * v.x - Mathf.Sin(rad) * v.y,
            Mathf.Sin(rad) * v.x + Mathf.Cos(rad) * v.y
        );
    }

    // ----------------------
    //   쿨타임 처리 코루틴
    // ----------------------
    private IEnumerator CooldownTick(string skill)
    {
        float timer = skill switch
        {
            "Q" => qTimer,
            "E" => eTimer,
            "R" => rTimer,
            _ => 0f
        };

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            // [수정: 타이머 업데이트] 👈 private setter를 가진 property 대신 backing field를 사용해야 함
            // 하지만 이 스크립트에서는 timer 값을 직접 업데이트하지 않고 코루틴에서만 사용하므로 이 부분은 유지합니다.
            yield return null;
        }

        switch (skill)
        {
            case "Q": qTimer = 0f; break;
            case "E": eTimer = 0f; break;
            case "R": rTimer = 0f; break;
        }
    }
}