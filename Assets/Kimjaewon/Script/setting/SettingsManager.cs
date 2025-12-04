using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;
    // 💡 [수정] settingsPanel을 private으로 두고, 필요하다면 GetComponentInChildren을 사용해 참조합니다.
    [HideInInspector] public GameObject settingsPanel; 
    
    public AudioSource audio;
    public AudioClip clip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // 이전에 이미 있었습니다.
            
            // 💡 [개선] UI 캔버스에 붙어있을 경우, 루트 오브젝트로 만들기
            if (transform.parent != null)
            {
                transform.SetParent(null);
            }
            DontDestroyOnLoad(gameObject);
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1. 비활성화된 오브젝트를 포함하여 모든 루트 오브젝트 순회
        foreach (GameObject rootObj in scene.GetRootGameObjects())
        {
            if (rootObj.name == "SettingsPanel")
            {
                settingsPanel = rootObj;
                Debug.Log("SettingsPanel을 새 씬에서 찾았습니다: " + settingsPanel.name);
                return;
            }
            // 2. 혹은 루트 오브젝트의 자식들 중에서 찾기 (패널이 Canvas의 자식일 경우)
            Transform childPanel = rootObj.transform.Find("SettingsPanel");
            if (childPanel != null)
            {
                settingsPanel = childPanel.gameObject;
                Debug.Log("SettingsPanel을 새 씬에서 찾았습니다: " + settingsPanel.name);
                return;
            }
        }
        
        // 그래도 찾지 못했다면 경고
        if (settingsPanel == null)
        {
            Debug.LogWarning("새 씬에서 'SettingsPanel'을 찾지 못했습니다. 이름이 정확한지, 씬에 존재하는지 확인하세요.");
        }
    }

    public void OpenSettings()
    {
        if (audio != null && clip != null)
            audio.PlayOneShot(clip);
            
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
        else
            Debug.LogWarning("SettingsPanel이 null입니다. 설정을 열 수 없습니다.");
    }

    public void CloseSettings()
    {
        if (audio != null && clip != null)
            audio.PlayOneShot(clip);
            
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        else
            Debug.LogWarning("SettingsPanel이 null입니다. 설정을 닫을 수 없습니다.");
    }
    
    private void OnDestroy()
    {
        // 메모리 누수 방지를 위해 이벤트 등록 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}