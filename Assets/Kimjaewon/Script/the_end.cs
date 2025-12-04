using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class the_end : MonoBehaviour
{
    public string nextSceneName;   // 실제로 로드할 씬 이름

#if UNITY_EDITOR
    public SceneAsset sceneAsset;

    void OnValidate()
    {
        if (sceneAsset != null)
        {
            nextSceneName = sceneAsset.name; // 씬 이름 자동 저장
        }
    }
#endif

    // 예시: 10초 뒤 이동
    private void Start()
    {
        Invoke("LoadScene", 10f);
    }

    void LoadScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}