using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 씬 이동용
using System.Collections;

public class OldTimer : MonoBehaviour
{
    [Header("타이머 UI 이미지")]
    public Image timerImage;     // fillAmount를 이용해 채움 표시
    public float duration = 600f; // 10분 = 600초
    public string sceneToLoad;    // 이동할 씬 이름

    private bool hasLoadedScene = false; // 중복 로딩 방지

    private void Start()
    {
        // 초기 상태: 꽉 찬 상태
        timerImage.fillAmount = 1f;

        // 타이머 시작
        StartCoroutine(TimerRoutine());
    }

    void Update()
    {
        if (!hasLoadedScene && timerImage.fillAmount <= 0f)
        {
            hasLoadedScene = true; // 중복 호출 방지
            SceneManager.LoadScene(sceneToLoad); // 씬 이동
        }
    }

    private IEnumerator TimerRoutine()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // fillAmount = 1 -> 0
            timerImage.fillAmount = Mathf.Clamp01(1f - (elapsed / duration));
            yield return null;
        }

        // 완료 후 확실히 비워둠
        timerImage.fillAmount = 0f;

        Debug.Log("10분 타이머 완료!");
    }
}