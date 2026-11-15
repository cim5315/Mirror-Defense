using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rigidbody2D;
    [SerializeField] private GameObject[] bulletType = new GameObject[10];
    [SerializeField] private SpriteRenderer enemyBullet;

    [Header("Attributes")]
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private float bulletLifeSpan = 10f;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.linearVelocity = transform.right * bulletSpeed;
        enemyBullet = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        bulletLifeSpan -= Time.deltaTime;
        if (bulletLifeSpan <= 0)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("BasicMirror"))
        {
            enemyBullet.enabled = false;
            bulletType[0].SetActive(true);
            Debug.Log("hit");
        }
        if (collision.gameObject.CompareTag("DamageMirror"))
        {
            enemyBullet.enabled = false;
            bulletType[0].SetActive(true);
            bulletType[1].SetActive(true);
        }
        if (collision.gameObject.CompareTag("EnragingMirror"))
        {
            enemyBullet.enabled = false;
            bulletType[0].SetActive(true);
            bulletType[2].SetActive(true);
        }
        if (collision.gameObject.CompareTag("ExplosiveMirror"))
        {
            enemyBullet.enabled = false;
            bulletType[0].SetActive(true);
            bulletType[3].SetActive(true);
        }
        if (collision.gameObject.CompareTag("MagnetMirror"))
        {
            enemyBullet.enabled = false;
            bulletType[0].SetActive(true);
            bulletType[4].SetActive(true);
        }
        if (collision.gameObject.CompareTag("HomingMirror"))
        {
            enemyBullet.enabled = false;
            bulletType[0].SetActive(true);
            bulletType[5].SetActive(true);
        }
        if (collision.gameObject.CompareTag("PoisonMirror"))
        {
            enemyBullet.enabled = false;
            bulletType[0].SetActive(true);
            bulletType[6].SetActive(true);
        }
        if (collision.gameObject.CompareTag("SlowingMirror"))
        {
            enemyBullet.enabled = false;
            bulletType[0].SetActive(true);
            bulletType[7].SetActive(true);
        }
        if (collision.gameObject.CompareTag("SpeedMirror"))
        {
            enemyBullet.enabled = false;
            bulletType[0].SetActive(true);
            bulletType[8].SetActive(true);
        }
        if (collision.gameObject.CompareTag("SplittingMirror"))
        {
            enemyBullet.enabled = false;
            bulletType[0].SetActive(true);
            bulletType[9].SetActive(true);
        }
    }
}
