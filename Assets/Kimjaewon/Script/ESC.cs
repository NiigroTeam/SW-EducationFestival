using UnityEngine;

public class ESC : MonoBehaviour
{
    // 게임이 멈췄는지 확인하는 거
    [HideInInspector] public bool StopGame = false;

    // 일시정지 메뉴 UI
    public GameObject PauseMenu;  
    
    // 설정창 UI
    public GameObject settingsPanel;

    void Start()
    {
        // PauseMenu 오브젝트가 비어있으면 이름으로 찾음
        if (PauseMenu == null)
        {
            PauseMenu = GameObject.Find("PauseMenu");
        }

        // 게임 시작 시 메뉴는 꺼둠
        if (PauseMenu != null)
        {
            PauseMenu.SetActive(false);
        }

        // 시작할때 설정창 꺼둠
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    void Update()
    {
        // ESC 키를 눌렀을 때 
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 설정창이 켜져 있으면 설정창 먼저 닫기
            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                settingsPanel.SetActive(false);
            }
            // 그렇지 않으면 일시정지/해제 전환
            else if (PauseMenu != null)
            {
                if (PauseMenu.activeSelf)
                {
                    Resume();  // 이미 켜져 있으면 꺼서 게임 계속
                }
                else
                {
                    Pause();   // 꺼져 있으면 켜서 게임 멈춤
                }
            }
        }
    }

    // 게임 다시시작
    public void Resume()
    {
        if (PauseMenu != null)
            PauseMenu.SetActive(false);

        Time.timeScale = 1f;   // 시간이 흐르게
        StopGame = false;
    }

    // 게임 일시정지
    public void Pause()
    {
        if (PauseMenu != null)
            PauseMenu.SetActive(true);

        Time.timeScale = 0f;   // 시간 멈춤
        StopGame = true;
    }
}