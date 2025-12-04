using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    // 🔴 인스펙터에 검은색 Image UI를 할당하세요.
    public Image blackScreen; 
    public float fadeDuration = 5.0f;
    
    [Header("새 씬 플레이어 목표 위치")]
    public Vector3 playerPos; // 인스펙터에서 설정할 목표 위치

    public static SceneFader Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Awake 시점에서 검은 화면이 존재하면 Alpha를 0으로 초기화
        if (blackScreen != null)
        {
            Color c = blackScreen.color;
            c.a = 0f;
            blackScreen.color = c;
            blackScreen.raycastTarget = false; // 👈 추가: UI가 터치/클릭을 막지 않도록 설정
        }
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // Awake에서 초기화했으므로 Start는 빈 상태로 유지합니다.
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (blackScreen == null)
        {
            Debug.LogError("Black Screen Image가 SceneFader에 할당되지 않아 페이드 인을 시작할 수 없습니다.");
            return;
        }

        // 씬 로드 직후, 혹시 모를 버그 방지를 위해 알파를 1.0으로 강제 설정 후 페이드 인 시작
        Color c = blackScreen.color;
        c.a = 1f;
        blackScreen.color = c;
        blackScreen.raycastTarget = true; // 👈 추가: 페이드 인 중 UI 입력을 막음
        
        StartCoroutine(FadeIn());
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null) {
            // 1. 부모 관계가 있다면 해제하여 월드 좌표를 정확히 따르도록 보장
            player.transform.SetParent(null); 
                
            // 2. 목표 위치 설정 (기존 Y축 오프셋 -50 유지)
            Vector3 finalPos = playerPos;
            finalPos.y += -50f; // 👈 씬 로드 시 Y 위치에 -50을 '더함' (즉, 아래로 50 이동)
            player.transform.position = finalPos;
            
            // 3. Rigidbody가 있다면 잔여 속도를 제거하여 즉시 멈춤
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            { 
                // 🛑 수정: Rigidbody2D는 .velocity를 사용 (linearVelocity 대신)
                rb.linearVelocity = Vector2.zero; 
                rb.angularVelocity = 0f;
            }
            
            Debug.Log($"플레이어 위치를 {scene.name}의 목표 위치 ({finalPos})로 이동 완료.");
        }
    }


    /// <summary>
    /// 외부 스크립트에서 호출되어 페이드 아웃을 시작하고 씬을 로드합니다.
    /// </summary>
    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        float timer = 0f;

        if (blackScreen == null)
        {
            Debug.LogError("Black Screen Image가 SceneFader에 할당되지 않아 즉시 씬 전환합니다.");
            SceneManager.LoadScene(sceneName);
            yield break;
        }

        Color originalColor = blackScreen.color;
        originalColor.a = 0f;
        blackScreen.color = originalColor;
        blackScreen.raycastTarget = true; // 👈 추가: 페이드 아웃 중 UI 입력을 막음

        // Alpha 0 → 1 (투명 → 검은 화면)
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = timer / fadeDuration;

            Color c = originalColor;
            c.a = alpha;
            blackScreen.color = c;

            yield return null;
        }

        // 씬 로드
        SceneManager.LoadScene(sceneName);
    }
    
    private IEnumerator FadeIn()
    {
        float timer = 0f;
        float currentDuration = fadeDuration; 

        // 씬 로드 직후 검은 화면 상태 (Alpha 1)에서 시작
        // OnSceneLoaded에서 이미 설정했으므로, 여기서는 현재 색상으로 시작
        Color targetColor = blackScreen.color;

        // Alpha 1 → 0 (검은 화면 → 투명)
        while (timer < currentDuration)
        {
            timer += Time.deltaTime;
            float alpha = 1f - (timer / currentDuration);

            Color c = targetColor;
            c.a = alpha;
            blackScreen.color = c;

            yield return null;
        }
        
        // 완전히 투명하게 설정
        Color finalColor = targetColor;
        finalColor.a = 0f;
        blackScreen.color = finalColor;
        
        blackScreen.raycastTarget = false; // 👈 추가: 페이드 인 완료 후 UI 입력을 다시 허용
    }
}