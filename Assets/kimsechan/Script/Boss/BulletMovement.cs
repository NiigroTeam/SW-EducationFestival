using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    private Vector3 velocity;
    public int damage = 10; // ✅ damage에 기본값 설정

    public void SetDirection(Vector3 dir)
    {
        velocity = dir;
    }

    private void Update()
    {
        transform.position += velocity * Time.deltaTime;
    }

    private void OnBecameInvisible()
    {
        gameObject.SetActive(false); 
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerManager.instance.take_Damage(damage);
            
            gameObject.SetActive(false); 
        }
    }
}