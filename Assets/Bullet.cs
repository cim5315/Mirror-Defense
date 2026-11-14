using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rigidbody2D;
    [SerializeField] private int[] enemyType = new int[5];

    [Header("Attributes")]
    [SerializeField] private float bulletSpeed = 5f;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.linearVelocity = transform.right * bulletSpeed;
    }
}
