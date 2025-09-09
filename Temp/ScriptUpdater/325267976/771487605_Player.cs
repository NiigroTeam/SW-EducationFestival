using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 10f;
    public float jumpForce = 10f;
    public int MaxHp;
    [HideInInspector]public int Hp;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        _Move();
    }
    
    void _Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 movement = new Vector3(h, 0, v).normalized * speed * Time.deltaTime;
        movement.y = rb.linearVelocity.y;

        rb.linearVelocity = movement;
    }
}
