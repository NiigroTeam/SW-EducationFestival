using UnityEngine;

public class Exit : MonoBehaviour
{
    // 종료 버튼을 눌렀을 때 실행
    public void OnClickExitButton()
    {
        // 에디터에서 실행 중일 경우 → 에디터 플레이 모드 종료
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 실제 빌드된 게임일 경우 → 게임 종료
        Application.Quit();
#endif
    }
}