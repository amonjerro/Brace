using System.Collections.Generic;
using UnityEngine;

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

    public GameObject GetBullet()
    {
        if (availableBullets.Count == 0)
        {
            availableBullets.Enqueue(Instantiate(bulletPrefab, transform));  
        }
        GameObject b = availableBullets.Dequeue();
        return b;
    }
    
    public void BulletReset(Bullet bullet)
    {
        bullet.ResetBullet();
        bullet.transform.position = transform.position;
        availableBullets.Enqueue(bullet.gameObject);
    }
}
