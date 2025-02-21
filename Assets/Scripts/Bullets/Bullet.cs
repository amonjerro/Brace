using UnityEngine;

public class Bullet : MonoBehaviour
{
    BulletPool pool;

    Vector3 direction;
    SpriteRenderer spriteRenderer;

    float moveSpeed;

    bool bBulletInPlay;

    [SerializeField]
    float maxX = 12;

    [SerializeField]
    float maxY = 12;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // Get a reference to the pool on start
        pool = GetComponentInParent<BulletPool>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!bBulletInPlay)
        {
            return;
        }

        
        transform.position += direction * moveSpeed * Time.deltaTime;

        if (transform.position.x > maxX || transform.position.x < -maxX || transform.position.y > maxY || transform.position.y < -maxY)
        {
            pool.BulletReset(this);
        }

    }

    public void ResetBullet()
    {
        bBulletInPlay = false;
        direction = Vector3.zero;
        moveSpeed = 0;
    }

    public void Launch(BulletParams bulletParams, Vector3 fireDirection)
    {
        direction = fireDirection;
        moveSpeed = bulletParams.moveSpeed;
        if (bulletParams.bulletSprite != null) {
            spriteRenderer.sprite = bulletParams.bulletSprite;
        }
        spriteRenderer.flipX = direction.x < 0;

        bBulletInPlay = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        pool.BulletReset(this);
    }
}
