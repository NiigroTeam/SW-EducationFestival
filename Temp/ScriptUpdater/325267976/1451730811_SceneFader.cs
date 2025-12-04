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
    public TMP_Text fadeText; 
    
    // 💡 [추가] 텍스트의 초기 투명도 (0.0 = 투명, 1.0 = 불투명)를 인스펙터에서 설정
    [Range(0f, 1f)]
    public float initialTextAlpha = 0.36f; 
    
    [Header("새 씬 플레이어 목표 위치")]
    public Vector3 playerPos;

    public static SceneFader Instance { get; private set; }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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
        // 검은 화면은 항상 투명(0)에서 시작
        if (blackScreen != null)
        {
            Color c = blackScreen.color;
            c.a = 0f;
            blackScreen.color = c;
        }
        // 💡 [수정] 텍스트는 인스펙터에서 설정한 투명도로 시작
        if (fadeText != null)
        {
            Color c = fadeText.color;
            c.a = initialTextAlpha; // 👈 인스펙터 값 사용
            fadeText.color = c;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 로드 직후 검은 화면과 텍스트의 알파를 1.0으로 강제 설정 (FadeIn을 위해)
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
            player.transform.SetParent(null); 
                
            Vector3 finalPos = playerPos;
            finalPos.y += -50f;
            player.transform.position = finalPos;
            
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            { 
                // 🛑 Rigidbody2D 속도 설정 수정
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
        
        // 💡 [수정] 텍스트의 시작 알파 값을 initialTextAlpha로 설정
        // blackScreen은 Alpha 0f에서 시작하도록 설정
        Color originalColor = blackScreen.color;
        originalColor.a = 0f;
        
        Color originalTextColor = fadeText != null ? fadeText.color : Color.clear;
        if (fadeText != null) originalTextColor.a = initialTextAlpha;

        // Alpha 0 → 1 (투명 → 검은 화면)
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float timeRatio = timer / fadeDuration; // 0에서 1로 증가

            // 검은 화면 페이드 아웃 (Alpha 0 -> 1)
            Color screenColor = originalColor;
            screenColor.a = timeRatio;
            blackScreen.color = screenColor;
            
            // 💡 [수정] 텍스트 페이드 아웃 (initialTextAlpha -> 1)
            if (fadeText != null)
            {
                // alpha = 시작 알파 + (1 - 시작 알파) * timeRatio
                float currentAlpha = initialTextAlpha + (1f - initialTextAlpha) * timeRatio;
                
                Color textColor = originalTextColor;
                textColor.a = currentAlpha;
                fadeText.color = textColor;
            }

            yield return null;
        }

        // 씬 로드 전에 완전히 불투명하게 설정
        if (blackScreen != null) blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 1f);
        if (fadeText != null) fadeText.color = new Color(fadeText.color.r, fadeText.color.g, fadeText.color.b, 1f);

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
        Color targetTextColor = fadeText != null ? fadeText.color : Color.clear;

        // Alpha 1 → 0 (검은 화면 → 투명)
        while (timer < currentDuration)
        {
            timer += Time.deltaTime;
            float alpha = 1f - (timer / currentDuration);

            // 검은 화면 페이드 인
            Color screenColor = targetColor;
            screenColor.a = alpha;
            blackScreen.color = screenColor;
            
            // 텍스트 페이드 인 (Alpha 1 -> 0)
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