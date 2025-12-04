using UnityEngine;

public class RotateToMouse : MonoBehaviour
{
    private Camera _mainCamera;

    void Start()
    {
        // 시작 시 메인 카메라 참조를 가져옴
        _mainCamera = Camera.main; 
    }

    void RotateToCursor()
    {
        // **수정된 20번 줄 근처:** 사용 전에 카메라 객체가 유효한지 확인합니다.
        if (_mainCamera != null) 
        {
            Vector3 mousePos = Input.mousePosition;
            // 이제 안전하게 ScreenToWorldPoint를 호출할 수 있습니다.
            Vector3 worldPoint = _mainCamera.ScreenToWorldPoint(mousePos);
        
            // ... 나머지 회전 로직
        }
        else
        {
            // 디버그용: 카메라가 파괴된 후에도 스크립트가 실행되고 있음을 알림
            Debug.LogWarning("카메라 객체가 파괴되었습니다. 다시 메인 카메라를 찾습니다.");
            // (선택 사항) 다시 Camera.main을 찾아 현재 유효한 카메라를 사용하도록 시도
            _mainCamera = Camera.main;
        }
    }

// Update()는 14번 줄이며 여기서 RotateToCursor()를 호출합니다.
    void Update()
    {
        RotateToCursor();
    }
}