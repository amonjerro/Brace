using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{ 
    [SerializeField]
    CharacterSO CharacterData;

    [SerializeField]
    GameObject BlockingField;
    
    // Base values
    Vector3 forward3;
    float originalYPosition;
    BulletPool bulletPool;
    bool bIsGrounded = true;

    // Jump duration modifier
    float jumpTimer = 0;

    // Timers for game actions
    float blockTimer = 0;
    float cooldownTimer = 0;
    
    // Constants
    const float HALF_PI = 0.5f * Mathf.PI;
    const float TWO_PI = 2 * Mathf.PI;
    

    private void Start()
    {
        bulletPool = FindAnyObjectByType<BulletPool>();
        forward3 = transform.right;
        originalYPosition = transform.position.y;
    }

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;
        // Prevent underflow
        if (cooldownTimer < -1000)
        {
            cooldownTimer = 0;
        }

        blockTimer -= Time.deltaTime;
        // Prevent underflow
        if (blockTimer < -1000) {
            blockTimer = 0;
        }

        if (bIsGrounded)
        {
            return;
        }

        jumpTimer += Time.deltaTime;
        transform.position = new Vector3(transform.position.x, ParabolicPosition(), transform.position.z);

        if (jumpTimer > CharacterData.characterParameters.jumpDuration)
        {
            bIsGrounded = true;
            transform.position = new Vector3(transform.position.x, originalYPosition, transform.position.z);
        }
    }

    float ParabolicPosition()
    {
        float t = jumpTimer / CharacterData.characterParameters.jumpDuration;
        return CharacterData.characterParameters.jumpHeight * ( -4 * t * t + 4 * t) + originalYPosition;
    }

    // Handle Player Input
    private void OnAttack(InputValue input)
    {
        if (cooldownTimer > 0)
        {
            return;
        }
        Bullet b = bulletPool.GetBullet();
        b.transform.position = transform.position + forward3;
        b.Launch(CharacterData.characterParameters.bulletParams, forward3);
        cooldownTimer = CharacterData.characterParameters.shotCooldown;
    }

    private void OnBlock(InputValue input)
    {
        if (blockTimer > 0)
        {
            return;
        }
        BlockingField.SetActive(true);
        blockTimer = CharacterData.characterParameters.blockCooldown;
    }

    private void OnJump(InputValue input)
    {
        if (bIsGrounded) { 
            bIsGrounded = false;
            jumpTimer = 0;
        }
    }
}
