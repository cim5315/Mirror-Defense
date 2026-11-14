using TMPro;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [Header("Attributes")]
    [SerializeField] private float moveSpeed = 2f;

    public Transform target;
    private int pathIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = LevelManager.main.path[pathIndex];
    }

    // Update is called once per frame
    private void Update()
    {
        if(Vector2.Distance(target.position, transform.position) <= 0f)
        {
            pathIndex++;
            if(pathIndex == LevelManager.main.path.Length)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                target = LevelManager.main.path[pathIndex];
            }
        }
    }
    private void FixedUpdate()
    {
        Vector2 direction = (target.position - transform.position);
        float distanceToTarget = direction.magnitude;

        float rawAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; ;
        float snappedAngle = Mathf.Round(rawAngle / 90f) * 90f;
        transform.rotation = Quaternion.Euler(0, 0, snappedAngle);

        if (distanceToTarget < 0.01f)
        {
            transform.position = target.position;
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            rb.linearVelocity = direction.normalized * moveSpeed;
        }
    }
}
