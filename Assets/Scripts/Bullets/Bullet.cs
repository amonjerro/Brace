using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Designer-set properties
    [SerializeField]
    float maxX = 12;

    public float MaxX { get { return maxX; } }

    [SerializeField]
    float maxY = 12;

    // Bullet pool reference
    BulletPool pool;

    // Component that governs how a bullet will move through the screen
    public KinematicComponent MovementComponent { get; private set; }


    // Configuration parameters
    SpriteRenderer spriteRenderer;
    Sprite regularBullet;
    Sprite hardenedBullet;
    float bulletDamage;
    int bulletToughness;
    
    // Lifecycle state
    bool bBulletInPlay;
    AbsLaunchStrategy absLaunchStrategy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // Get a reference to the pool on start
        pool = GetComponentInParent<BulletPool>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        MovementComponent = new KinematicComponent();
    }

    /// <summary>
    /// Increases a bullet's toughness. Should also apply the corresponding hardened sprite one layer up.
    /// </summary>
    /// <param name="sprite">The hardened sprite to use. Character dependent.</param>
    public void Harden()
    {
        bulletToughness++;
        spriteRenderer.sprite = hardenedBullet;
    }

    // Update is called once per frame
    void Update()
    {
        // Exit out of update if this bullet is inactive in the pool
        if (!bBulletInPlay)
        {
            return;
        }

        absLaunchStrategy.Update();

        
        // Update the position based on the movement direction
        transform.position += MovementComponent.direction * MovementComponent.speed * TimeUtil.GetDelta();
        if (transform.position.x > maxX || transform.position.x < -maxX || transform.position.y > maxY || transform.position.y < -maxY)
        {
            pool.BulletReset(this);
        }

    }

    // Reset this bullet to be used in the pool later
    public void ResetBullet()
    {
        bBulletInPlay = false;
    }

    // Ask the pool for a new bullet and get it ready to be gameplay-relevant
    public void Launch(BulletParams bulletParams, Vector3 fireDirection)
    {
        SetBulletGraphics(bulletParams);
        
        bulletDamage = bulletParams.bulletDamage;
        bulletToughness = 1;

        if (absLaunchStrategy == null)
        {
            absLaunchStrategy = LaunchFactory.MakeLaunchFactory(bulletParams.launchType, this);
        } else if (absLaunchStrategy.LaunchStrategies != bulletParams.launchType)
        {
            absLaunchStrategy = LaunchFactory.MakeLaunchFactory(bulletParams.launchType, this);
        }
        
        absLaunchStrategy.Launch(fireDirection, bulletParams);
        CheckForSpriteFlip();
        bBulletInPlay = true;
    }

    public void SetSpeed(float moveSpeed)
    {
        MovementComponent.speed = moveSpeed;
    }

    public void SetDestinationTarget(Vector3 target)
    {
        MovementComponent.direction = (target - transform.position).normalized;
    }

    public void SetDirection(Vector3 direction)
    {
        MovementComponent.direction = direction;
    }
    

    /// <summary>
    /// Check whether for this bullet needs to be flipped.
    /// Checked on launch and on parry.
    /// </summary>
    public void CheckForSpriteFlip()
    {
        spriteRenderer.flipX = MovementComponent.direction.x < 0;
    }

    // Collision handler to determine what happens when this bullet hits something else
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!bBulletInPlay)
        {
            return;
        }
        if (collision.gameObject.CompareTag("Shield"))
        {
            // The bullet gets redirected on parry or reset on block by dealing half damage.
            Shield s = collision.gameObject.GetComponent<Shield>(); 
            if (s.IsParry && bulletToughness == 1) {
                s.effector.Apply(this);
                return;
            } else
            {
                Character chr = s.GetComponentInParent<Character>();
                chr.TakeDamage(0.5f * bulletDamage * bulletToughness);
                bulletToughness = 1;
            }
        }

        // Unblocked shots deal full damage
        if (collision.gameObject.CompareTag("Player"))
        {
            Character chr = collision.gameObject.GetComponent<Character>();
            chr.TakeDamage(bulletDamage * bulletToughness);
            bulletToughness = 1;
        }

        if (bulletToughness <= 1)
        {
            // Most collisions require the bullet to be sent back to the pool.
            pool.BulletReset(this);
        }
        else
        {
            bulletToughness--;
            if (bulletToughness == 1)
            {
                spriteRenderer.sprite = regularBullet;
            }
        }
    }

    private void SetBulletGraphics(BulletParams bulletParams)
    {
        if (bulletParams.bulletSprite != null)
        {
            regularBullet = bulletParams.bulletSprite;
            hardenedBullet = bulletParams.hardenedBullet;
            spriteRenderer.sprite = regularBullet;
            spriteRenderer.color = Color.white;
        }
    }

    
}
