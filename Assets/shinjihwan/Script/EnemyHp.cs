using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public float maxHP = 100f;
    public float currentHP;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log($"{gameObject.name}이(가) {damage} 피해를 받음. 남은 체력: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} 사망");
        Destroy(gameObject);
    }
}