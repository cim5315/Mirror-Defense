using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private float firingSpeed = 1f;

    private float timeUntilFire;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeUntilFire += Time.deltaTime;

        if (timeUntilFire >= 1f / firingSpeed)
        {
            Shoot();
            timeUntilFire = 0f;
        }
    }
    private void Shoot()
    {
        GameObject bulletObj = Instantiate(bulletPrefab, firingPoint.position, firingPoint.rotation);
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
    }
}
