using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField]
    Bullet bulletPrefab;

    // Object pooling
    Queue<Bullet> availableBullets;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        availableBullets = new Queue<Bullet>();
        foreach(Transform child in transform)
        {
            Bullet bullet = child.gameObject.GetComponent<Bullet>();
            availableBullets.Enqueue(bullet);
        }
    }

    public Bullet GetBullet()
    {
        if (availableBullets.Count == 0)
        {
            availableBullets.Enqueue(Instantiate(bulletPrefab, transform));  
        }
        Bullet b = availableBullets.Dequeue();
        return b;
    }
    
    public void BulletReset(Bullet bullet)
    {
        bullet.ResetBullet();
        bullet.transform.position = transform.position;
        availableBullets.Enqueue(bullet);
    }
}
