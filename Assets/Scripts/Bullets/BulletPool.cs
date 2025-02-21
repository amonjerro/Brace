using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Simple object pool for bullets
/// </summary>
public class BulletPool : MonoBehaviour
{
    [SerializeField]
    GameObject bulletPrefab;

    // Object pooling
    Queue<GameObject> availableBullets;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        availableBullets = new Queue<GameObject>();
        foreach(Transform child in transform)
        {
            Bullet bullet = child.gameObject.GetComponent<Bullet>();
            availableBullets.Enqueue(bullet.gameObject);
        }
    }

    // Provides the next bullet available or creates a new one if all are empty
    public GameObject GetBullet()
    {
        if (availableBullets.Count == 0)
        {
            availableBullets.Enqueue(Instantiate(bulletPrefab, transform));  
        }
        GameObject b = availableBullets.Dequeue();
        return b;
    }
    
    // Resets a bullet back into the pool
    public void BulletReset(Bullet bullet)
    {
        bullet.ResetBullet();
        bullet.transform.position = transform.position;
        availableBullets.Enqueue(bullet.gameObject);
    }
}
