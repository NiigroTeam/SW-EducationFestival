using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Transform playerTransform;

    [Header("플레이어 기준 상대 위치 Offset")]
    public Vector3 localOffset = new Vector3(0.5f, -0.2f, 0f);

    void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogError("FollowPlayer: Player 태그를 가진 오브젝트를 찾을 수 없음");
            enabled = false;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // -------------------------------
        // ⭐ 플레이어의 좌우반전 체크
        // -------------------------------
        float dir = Mathf.Sign(playerTransform.localScale.x);  
        // dir = +1 → 오른쪽
        // dir = -1 → 왼쪽

        // -------------------------------
        // ⭐ 칼 Offset 반전 처리
        // -------------------------------
        Vector3 appliedOffset = new Vector3(
            localOffset.x * dir,   
            localOffset.y,
            localOffset.z
        );

        // -------------------------------
        // ⭐ 최종 위치 적용
        // -------------------------------
        transform.position = playerTransform.position + appliedOffset;
    }
}