using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firingPointR;
    [SerializeField] private Transform firingPointRU;
    [SerializeField] private Transform firingPointRD;
    [SerializeField] private bool fireR;
    [SerializeField] private bool fireRU;
    [SerializeField] private bool fireRD;
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
        if (fireR)
        {
            GameObject bulletObj = Instantiate(bulletPrefab, firingPointR.transform.position, firingPointR.transform.rotation);
            Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        }

        if (fireRU)
        {
            GameObject bulletObj = Instantiate(bulletPrefab, firingPointRU.transform.position, firingPointRU.transform.rotation);
            Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        }

        if (fireRD)
        {
            GameObject bulletObj = Instantiate(bulletPrefab, firingPointRD.transform.position, firingPointRD.transform.rotation);
            Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        }


    }
}
