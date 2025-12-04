using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Tire_Hard_2 : Skill_based
{
    [Header("Dependencies")] public BossManager bossManager;

    [Header("Attack Settings")]
    public int skillIndex = 2;
    public float distanceMultiplier = 0.5f;   // Hard_1 거리 비율
    public float chargeDuration = 0.75f;
    public float stopDuration = 3.0f;         // 멈춰있는 시간
    public float fadeDuration = 0.5f;         // 👈 (사용되지 않음)
    public float attackObjectScale = 2.0f;
    public AudioSource audioSource;
    public AudioClip audioClip;

    [Header("Warning Settings")]
    public int warningIndex = 5;

    private Transform playerTarget;
    private Coroutine currentCoroutine;
    
    void Update()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTarget = playerObj.transform;

        if (bossManager == null)
        {
            bossManager = GetComponentInParent<BossManager>();
            if (bossManager == null)
                bossManager = FindObjectOfType<BossManager>();
        }
    }

    public override void Attack()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(SingleDashSequence());
    }

    public override void StopAttack()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
    }

    private IEnumerator SingleDashSequence()
    {
        if (playerTarget == null || bossManager == null)
            yield break;
        
        // 패턴 시작 전 보스 색상 초기화 (이전 요청으로 추가된 부분)
        SpriteRenderer bossSprite = bossManager.GetComponent<SpriteRenderer>();
        if (bossSprite != null)
        {
            bossSprite.color = Color.white;
        } 
        else if (bossManager.gameObject.transform.childCount > 0)
        {
            SpriteRenderer childSprite = bossManager.GetComponentInChildren<SpriteRenderer>();
            if (childSprite != null)
            {
                childSprite.color = Color.white;
            }
        }

        Vector3 playerPos = playerTarget.position;
        playerPos.z = 0;

        // 공격 방향 결정 (랜덤)
        int attackDirection = Random.Range(0, 2); // 0: 우->좌, 1: 좌->우

        Vector3 tireSpawnPos = Vector3.zero;
        Vector3 targetPos = Vector3.zero;
        float targetScaleX = 1f;

        if (attackDirection == 0)
        {
            tireSpawnPos = new Vector3(33f, playerPos.y, 0);
            targetPos = Vector3.Lerp(tireSpawnPos, new Vector3(-33f, playerPos.y, 0), distanceMultiplier);
            targetScaleX = -1f;
        }
        else
        {
            tireSpawnPos = new Vector3(-33f, playerPos.y, 0);
            targetPos = Vector3.Lerp(tireSpawnPos, new Vector3(33f, playerPos.y, 0), distanceMultiplier);
            targetScaleX = 1f;
        }

        // 공격 오브젝트 소환
        GameObject tire = bossManager.UseSkill(skillIndex, tireSpawnPos, Quaternion.identity);
        if (tire != null)
        {
            tire.transform.position = tireSpawnPos;
            tire.transform.DOKill();

            Vector3 currentScale = tire.transform.localScale;
            currentScale.x *= attackObjectScale;
            currentScale.y *= attackObjectScale;
            tire.transform.localScale = new Vector3(targetScaleX * Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);

            audioSource.PlayOneShot(audioClip);
            // 돌진
            yield return tire.transform.DOMove(targetPos, chargeDuration).SetEase(Ease.Linear).WaitForCompletion();

            // 멈춤
            yield return new WaitForSeconds(stopDuration);
            
            tire.SetActive(false);
        }

        currentCoroutine = null;
    }
}