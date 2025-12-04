using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyHP hp;

    void Awake()
    {
        hp = GetComponent<EnemyHP>();
        if (hp == null)
        {
            hp = gameObject.AddComponent<EnemyHP>();
        }
    }

    void Update()
    {
        // 여기엔 아무것도 없어도 됨
        // 적 AI 필요한 경우 나중에 채워 넣으면 됨
    }
}