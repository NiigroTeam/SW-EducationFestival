using UnityEngine;

public class Continue_button : MonoBehaviour
{
    // ESC 스크립트 연결
    public ESC escScript;
    public GameObject canvas;
    public AudioSource clip;
    public AudioClip clip2;

    // 계속하기 버튼 눌렀을 때 시작하게
    public void OnClickContinueButton()
    {
        if (escScript != null)
        {
            clip.PlayOneShot(clip2);
            // 게임 다시 시작
            escScript.Resume();
            canvas.gameObject.SetActive(false);
        }
    }
}
