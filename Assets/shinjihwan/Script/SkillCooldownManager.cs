using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IndividualSkillCooldown : MonoBehaviour
{
    [Header("스킬 UI 이미지")]
    // ⚠️ 인스펙터에서 4개의 이미지를 할당해야 합니다: 0: Q, 1: E, 2: R, 3: Dash(Shift)
    public Image[] skillImages;       
    // ⚠️ 인스펙터에서 4개의 쿨타임을 할당해야 합니다: 0: Q, 1: E, 2: R, 3: Dash 쿨타임
    public float[] cooldownTimes;     
    
    [Header("스킬 사용 가능 상태")]
    public bool qActive = true;
    public bool eActive = true;
    public bool rActive = true;
    public bool dashActive = true; // 👈 대쉬 Active 상태 추가

    private bool[] isCooldownRunning; // 각 스킬별 쿨타임 상태
    
    public static IndividualSkillCooldown instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 배열 크기 확인 (최소 4개 이상이어야 함)
        if (skillImages.Length < 4 || cooldownTimes.Length < 4)
        {
            Debug.LogError("IndividualSkillCooldown: skillImages와 cooldownTimes 배열에 최소 4개의 요소를 할당해야 합니다 (Q, E, R, Dash).");
            return;
        }
        
        isCooldownRunning = new bool[skillImages.Length];
        for (int i = 0; i < skillImages.Length; i++)
        {
            skillImages[i].fillAmount = 1f; // 쿨타임 완료 상태
            isCooldownRunning[i] = false;
        }
    }

    // 스킬 사용 시 호출
    public void StartCooldown(int index)
    {
        if (index < 0 || index >= isCooldownRunning.Length)
        {
            Debug.LogError($"Invalid skill index: {index}");
            return;
        }

        if (!isCooldownRunning[index])
        {
            // 사용 후 Active 끄기
            switch (index)
            {
                case 0: qActive = false; break;
                case 1: eActive = false; break;
                case 2: rActive = false; break;
                case 3: dashActive = false; break; // 👈 대쉬 Active 끄기 (인덱스 3)
            }
            StartCoroutine(CooldownRoutine(index));
        }
    }

    private IEnumerator CooldownRoutine(int index)
    {
        isCooldownRunning[index] = true;
        
        float timer = cooldownTimes[index];
        skillImages[index].fillAmount = 0f; 

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            // 쿨타임 진행률을 계산하여 UI에 반영
            skillImages[index].fillAmount = Mathf.Clamp01(timer / cooldownTimes[index]); 
            yield return null;
        }

        skillImages[index].fillAmount = 1f;
        isCooldownRunning[index] = false;

        // 쿨타임 완료 시 Active 켜기
        switch (index)
        {
            case 0: qActive = true; break;
            case 1: eActive = true; break;
            case 2: rActive = true; break;
            case 3: dashActive = true; break; // 👈 대쉬 Active 켜기 (인덱스 3)
        }
    }
}