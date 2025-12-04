using System.Collections;
using NUnit.Framework.Internal;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    // 🔴 인스펙터에 검은색 Image UI를 할당하세요.
    public Image blackScreen; 
    public float fadeDuration = 5.0f;
    public TMP_Text fadeText; // 👈 TMP_Text 컴포넌트
    
    [Header("새 씬 플레이어 목표 위치")]
    public Vector3 playerPos; // 인스펙터에서 설정할 목표 위치

    public static SceneFader Instance { get; private set; }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // 이전에 제거되었으므로 그대로 둠
        }
        else
        {
            Destroy(gameObject);
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
        if (blackScreen != null)
        {
            Color c = blackScreen.color;
            c.a = 0f;
            blackScreen.color = c;
        }
        // 💡 [수정] 시작 시 텍스트의 투명도를 36% (0.36f)로 설정합니다.
        if (fadeText != null)
        {
            Color c = fadeText.color;
            c.a = 0.36f; // 👈 36% 투명도 설정
            fadeText.color = c;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 💡 씬 로드 직후 검은 화면과 텍스트의 알파를 1.0으로 강제 설정
        if (blackScreen != null)
        {
            Color c = blackScreen.color;
            c.a = 1f; 
            blackScreen.color = c;
        }
        if (fadeText != null)
        {
            Color c = fadeText.color;
            c.a = 1f; 
            fadeText.color = c;
        }

        StartCoroutine(FadeIn());
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null) {
            // 1. 부모 관계가 있다면 해제하여 월드 좌표를 정확히 따르도록 보장
            player.transform.SetParent(null); 
                
            // 2. 목표 위치 설정 및 Y축 -50 오프셋 적용
            Vector3 finalPos = playerPos;
            finalPos.y += -50f; // 👈 씬 로드 시 Y 위치에 -50을 '더함' (기존 로직 유지)
            player.transform.position = finalPos;
            
            // 3. Rigidbody가 있다면 잔여 속도를 제거하여 즉시 멈춤
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            { 
                // 🛑 [수정] Rigidbody2D는 linearVelocity 대신 velocity를 사용합니다.
                rb.linearVelocity = Vector2.zero; // linearVelocity를 velocity로 변경
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
        // 💡 텍스트가 있다면 텍스트 색상도 가져옵니다.
        Color originalTextColor = fadeText != null ? fadeText.color : Color.clear;

        // Alpha 0 → 1 (투명 → 검은 화면)
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = timer / fadeDuration;

            // 검은 화면 페이드 아웃
            Color screenColor = originalColor;
            screenColor.a = alpha;
            blackScreen.color = screenColor;
            
            // 💡 텍스트 페이드 아웃 (Alpha 0 -> 1)
            if (fadeText != null)
            {
                Color textColor = originalTextColor;
                textColor.a = alpha;
                fadeText.color = textColor;
            }

            yield return null;
        }

        // 씬 로드
        SceneManager.LoadScene(sceneName);
    }
    
    private IEnumerator FadeIn()
    {
        float timer = 0f;
        float currentDuration = fadeDuration; 
        
        if (blackScreen == null) yield break;

        // 씬 로드 직후 검은 화면 상태에서 시작 (Alpha 1)
        Color targetColor = blackScreen.color;
        // 💡 텍스트 색상도 가져옵니다.
        Color targetTextColor = fadeText != null ? fadeText.color : Color.clear;
        
        // OnSceneLoaded에서 이미 Alpha 1로 설정되었으므로 바로 페이드 인 시작

        // Alpha 1 → 0 (검은 화면 → 투명)
        while (timer < currentDuration)
        {
            timer += Time.deltaTime;
            float alpha = 1f - (timer / currentDuration);

            // 검은 화면 페이드 인
            Color screenColor = targetColor;
            screenColor.a = alpha;
            blackScreen.color = screenColor;
            
            // 💡 텍스트 페이드 인 (Alpha 1 -> 0)
            if (fadeText != null)
            {
                Color textColor = targetTextColor;
                textColor.a = alpha;
                fadeText.color = textColor;
            }

            yield return null;
        }
        
        // 완전히 투명하게 설정
        Color finalColor = targetColor;
        finalColor.a = 0f;
        blackScreen.color = finalColor;
        
        if (fadeText != null)
        {
            Color finalTextColor = targetTextColor;
            finalTextColor.a = 0f;
            fadeText.color = finalTextColor;
        }
    }
}