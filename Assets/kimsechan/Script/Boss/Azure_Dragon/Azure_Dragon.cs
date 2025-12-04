using System;
using System.Collections; // Coroutine 사용을 위해 추가
using UnityEngine;
using DG.Tweening; 
using UnityEngine.SceneManagement;

public class Azure_Dragon : BossManager
{
    // ✨ 씬 전환을 위한 변수 (Inspector에서 설정)
    [Header("Scene Transition")]
    [Tooltip("보스 처치 후 로드할 씬의 이름 목록 (Player 이름과 일치하는 씬을 로드)")]
    public string[] nextSceneName; 

    // ✨ 2페이즈 애니메이션 설정
    [Header("Azure Dragon Phase Transitions")]
    [Tooltip("2페이즈 진입 시 보스가 고정적으로 이동할 Y 좌표 (X=0, Y=50.0)")]
    private const float PHASE_TWO_FIXED_Y = 50.0f; 
    private const float PHASE_TWO_FIXED_X = 0f;
    
    [Tooltip("2페이즈 이동 애니메이션 지속 시간")]
    public float animationDuration = 2.0f; 
    public Collider2D collider2d;

    // 2D 충돌 처리
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Weapon"))
        {
            // BossManager의 TakeDamage 메서드를 호출하여 HP 감소와 이펙트 처리를 일임
            // Note: 실제 게임에서는 HitboxDamage 스크립트에서 데미지를 가져와야 합니다.
            TakeDamage(10);
        }
    }

    /// <summary>
    /// BossManager의 OnPhaseTwoStart 메서드를 오버라이드합니다.
    /// 2페이즈 진입 시 청룡이 고정된 위치(0, 50)로 즉시 이동하며, 추가 애니메이션 로직을 제외합니다.
    /// </summary>
    protected override void OnPhaseTwoStart()
    {
        Turn_Change turnChange = GetComponent<Turn_Change>();
        if (turnChange != null)
        {
            turnChange.enabled = false;
        }

        // ✨ 청룡을 고정된 위치(0, 50)로 이동
        Vector3 targetPos = new Vector3(PHASE_TWO_FIXED_X, PHASE_TWO_FIXED_Y, transform.position.z);
        transform.DOMove(targetPos, animationDuration).SetEase(Ease.OutSine);
    }
    
    /// <summary>
    /// **BossManager의 PhaseTwoCinematic을 new로 재정의합니다.**
    /// </summary>
    protected new IEnumerator PhaseTwoCinematic(bool isDying) 
    {
        if (isDying)
        {
            yield break;
        }

        if (skillCoolCoroutine != null)
            StopCoroutine(skillCoolCoroutine);
        
        Turn_Change turnChange = GetComponent<Turn_Change>(); 

        if (turnChange != null)
            turnChange.StopAttackLoop();

        foreach (var skill in bossSkills)
            skill.DeactivateAllObjects();

        OnSkill = true; 
        IsPlayerInputLocked = true; 
        
        // 카메라 이동 및 보스 흔들림 로직을 모두 제거하고 잠시 대기
        yield return new WaitForSeconds(1.0f);

        OnSkill = false;
        IsPlayerInputLocked = false;

        skillCoolCoroutine = StartCoroutine(SkillCool()); 
        
        if (turnChange != null)
        {
            turnChange.enabled = true; 
            turnChange.StartPhaseTwoLoop(); 
        }
    }


    /// <summary>
    /// BossManager의 StartDeathSequence 메서드를 재정의하여 사망 시 즉시 씬 전환을 실행합니다.
    /// </summary>
    public new void StartDeathSequence()
    {
        Debug.Log($"보스 처치 완료! 다음 씬 로드를 시도합니다.");

        // 1. 모든 패턴 및 코루틴 중지
        StopAllCoroutines(); 
        
        // 2. 턴 체인지(위치 추적) 비활성화
        Turn_Change turnChange = GetComponent<Turn_Change>();
        if (turnChange != null)
        {
            turnChange.StopAttackLoop();
            turnChange.enabled = false;
        }
        
        // 3. DO Tween 애니메이션 중지
        transform.DOKill();

        // 4. 오브젝트 파괴 로직

        // 4a. "Player" 태그를 가진 오브젝트를 찾습니다.
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        
        // 5. 씬 전환 실행을 위해 Player 이름 확보
        string targetSceneName = null; // 로드할 씬 이름을 저장할 변수
        string playerName = null;

        if (players.Length > 0 && players[0] != null)
        {
            playerName = players[0].name; // 첫 번째 플레이어의 이름을 사용
            
            // nextSceneName 배열을 순회하며 Player 이름과 일치하는 씬을 찾습니다.
            foreach (string scene in nextSceneName)
            {
                if (playerName.Equals(scene, StringComparison.OrdinalIgnoreCase))
                {
                    targetSceneName = scene; // 일치하는 씬 이름 발견
                    break;
                }
            }
        }

        // 씬 로드 조건이 충족되지 않으면 파괴 로직을 건너뜁니다.
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogWarning($"Player 이름 '{playerName ?? "NULL"}'과 일치하는 씬을 찾지 못했습니다. 씬 전환을 수행하지 않습니다.");
            return; // 씬 로드 실패 시 여기서 종료
        }

        // ----------------------------------------------------
        // 씬 로드 조건 충족 시, 파괴 로직 실행 (재배치)
        // ----------------------------------------------------

        // 4a. (계속) 플레이어 파괴
        foreach (GameObject player in players)
        {
            if (player != null)
            {
                Debug.Log($"플레이어 오브젝트 파괴 (Tag: Player): {player.name}");
                // ⚠️ 주의: 씬 전환 전 Player의 이름 확인이 필요했으므로, 
                // 이 라인은 씬 로드 직전으로 옮겼습니다.
                Destroy(player);
            }
        }
        
        // 4a. (계속) fade_out 오브젝트 파괴 (오류 수정)
        GameObject[] fade_outs = GameObject.FindGameObjectsWithTag("fade_out");
        foreach (GameObject fadeObject in fade_outs) // 올바른 배열 순회
        {
            if (fadeObject != null)
            {
                Debug.Log($"fade_out 오브젝트 파괴: {fadeObject.name}");
                Destroy(fadeObject); // 올바른 요소 파괴
            }
        }
        
        // 4b. "Player" 태그 외의 나머지 DontDestroyOnLoad 오브젝트 파괴
        GameObject[] allActiveObjects = FindObjectsOfType<GameObject>();
        
        for (int i = 0; i < allActiveObjects.Length; i++)
        {
            GameObject obj = allActiveObjects[i];
            
            // 오브젝트가 DontDestroyOnLoad 씬에 있고, 현재 보스 오브젝트가 아닐 때 파괴합니다.
            if (obj != null && obj != gameObject && obj.scene.name == "")
            {
                Debug.Log($"DontDestroyOnLoad 오브젝트 파괴: {obj.name}");
                Destroy(obj);
            }
        }
        
        // 6. 씬 전환 실행
        Debug.Log($"Player 이름과 일치하는 씬 '{targetSceneName}' 로드 시작.");
        SceneManager.LoadScene(targetSceneName); 
    }

    void FixedUpdate()
    {
        // FixedUpdate에서 Hp == 0일 때 사망 시퀀스를 시작합니다.
        if (Hp <= 0) 
        {
             StartDeathSequence();
             // Hp가 0이 된 직후에 StartDeathSequence를 한 번만 호출하도록 막기 위해 스크립트를 비활성화합니다.
             enabled = false; 
        }
        if (Hp <= MaxHp / 2) 
        {
            collider2d.enabled = false;
        }
    }
}