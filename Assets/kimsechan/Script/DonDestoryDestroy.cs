using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DonDestoryDestroy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 씬 로드 후 즉시 실행되어야 하므로 Start에서 호출
        DestroyAllDontDestroyOnLoad();
        
        // 이 오브젝트는 DDOL 정리가 끝난 후에는 필요 없으므로 파괴
        Destroy(gameObject);
    }

    // DontDestroyOnLoad 씬에 남아있는 불필요한 오브젝트들을 파괴합니다.
    private void DestroyAllDontDestroyOnLoad()
    {
        // 1. Player 파괴 (태그: "Player")
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && player != gameObject) // 자신과 동일한 오브젝트가 아닌 경우만 파괴
        {
            Debug.Log($"[DDOL Cleaner] Player 오브젝트를 파괴했습니다: {player.name}");
            Destroy(player);
        }
        
        // 2. 관리 오브젝트 파괴 (태그: "Don")
        GameObject managerDon = GameObject.FindGameObjectWithTag("Don");
        if (managerDon != null && managerDon != gameObject) // 자신과 동일한 오브젝트가 아닌 경우만 파괴
        {
            Debug.Log($"[DDOL Cleaner] 'Don' 태그 오브젝트를 파괴했습니다: {managerDon.name}");
            Destroy(managerDon);
        }
        // 3. 관리 오브젝트 파괴 (태그: "fade_out")
        GameObject managerFadeOut = GameObject.FindGameObjectWithTag("fade_out");
        if (managerFadeOut != null && managerFadeOut != gameObject) // 자신과 동일한 오브젝트가 아닌 경우만 파괴
        {
            Debug.Log($"[DDOL Cleaner] 'fade_out' 태그 오브젝트를 파괴했습니다: {managerFadeOut.name}");
            Destroy(managerFadeOut);
        }
    }
}