using System.Collections;
using UnityEngine;

public class SnowDestroy : MonoBehaviour
{
    // ⭐ 플레이어 이동 스크립트 이름 (실제 이름으로 변경 필요)
    private const string PLAYER_MOVEMENT_SCRIPT = "PlayerMovement";
    
    // ⭐ 플레이어가 멈춰야 할 시간 (투사체가 사라지는 시간)
    private const float STOP_DURATION = 1f; 
    
    // ⚠️ isDestory 변수는 이제 필요 없으므로 제거했습니다.
    
    void Start()
    {
        // 4초 후 스스로 파괴되는 코루틴 시작
        StartCoroutine(AutoDestroyDelay());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            // 1. 플레이어를 멈추는 코루틴 시작
            StartCoroutine(StopPlayerTemporarily(other.gameObject));
            
            // 2. 중요: 충돌 후 바로 파괴하는 로직을 제거하고,
            //    코루틴 끝에서 파괴하도록 변경합니다.
            //    Destroy(gameObject); // 이 코드를 제거합니다.
        }
    }

    // ----------------------------------------------------
    // ⭐ 수정된 코루틴: 플레이어와 눈덩이(자신)를 동시에 고정 ⭐
    // ----------------------------------------------------
    IEnumerator StopPlayerTemporarily(GameObject playerObject)
    {
        // 1. 플레이어의 이동 컴포넌트를 찾습니다.
        MonoBehaviour playerMovement = playerObject.GetComponent(PLAYER_MOVEMENT_SCRIPT) as MonoBehaviour;
        
        // 2. 플레이어의 현재 위치(고정할 위치)를 저장합니다.
        Vector3 fixedPosition = playerObject.transform.position;
        
        // 3. 눈덩이(자신)의 Rigidbody2D를 찾습니다.
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        
        // 4. 눈덩이의 움직임을 즉시 멈춥니다.
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true; // 물리 영향 무시 (강제 고정을 위해)
        }

        // 5. 플레이어 이동 스크립트를 비활성화합니다. (선택적)
        bool wasMovementEnabled = playerMovement != null && playerMovement.enabled;
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        float timer = 0f;
        
        // 6. STOP_DURATION(1초) 동안 매 프레임 위치를 고정합니다.
        while (timer < STOP_DURATION)
        {
            // 플레이어와 눈덩이(자신)의 위치를 저장된 위치로 강제 설정
            playerObject.transform.position = fixedPosition;
            this.transform.position = fixedPosition; // ⭐ 눈덩이도 같은 위치로 강제 고정 ⭐

            timer += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }
        
        // 7. 플레이어 이동 스크립트 복구
        if (playerMovement != null && wasMovementEnabled)
        {
            playerMovement.enabled = true;
        }
        
        // 8. 눈덩이(자신)를 파괴합니다.
        Destroy(gameObject);
    }

    // ----------------------------------------------------
    // ⭐ 4초 후 투사체 자동 파괴
    // ----------------------------------------------------
    IEnumerator AutoDestroyDelay()
    {
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }
}