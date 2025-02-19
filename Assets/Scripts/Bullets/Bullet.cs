using UnityEngine;

public class Bullet : MonoBehaviour
{
    BulletPool pool;

    Vector3 direction;
    SpriteRenderer spriteRenderer;

    BoxCollider2D Player1Collider;
    BoxCollider2D Player2Collider;

    CircleCollider2D myCollider;

    float moveSpeed;

    bool bBulletInPlay;

    [SerializeField]
    float maxX = 12;

    [SerializeField]
    float maxY = 12;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        myCollider = GetComponent<CircleCollider2D>();

        // Get a reference to the pool on start
        pool = GetComponentInParent<BulletPool>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        Player1Collider = GameObject.Find("Player1").GetComponent<BoxCollider2D>();
        Player2Collider = GameObject.Find("Player2").GetComponent<BoxCollider2D>();
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

        if (myCollider.bounds.Intersects(Player1Collider.bounds)) {
            pool.BulletReset(this);
        }

        if (myCollider.bounds.Intersects(Player2Collider.bounds))
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
}
