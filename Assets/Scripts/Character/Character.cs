using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{ 
    [SerializeField]
    CharacterSO CharacterData;

    [SerializeField]
    GameObject BlockingField;

    [SerializeField]
    bool isFlipped;
    
    // Base values
    Vector3 forward3;
    float originalYPosition;
    BulletPool bulletPool;
    bool bIsGrounded = true;

    // Jump duration modifier
    float jumpTimer = 0;

    // Timers for game actions
    float blockTimer = 0;
    float blockUptickSpeed = 0;
    float blockCooldownTimer = 0;
    float blockCooldownSpeed = 0;
    float attackCooldown = 0;
    float attackCooldownSpeed = 0;
    
    // Constants
    const float HALF_PI = 0.5f * Mathf.PI;
    const float TWO_PI = 2 * Mathf.PI;
    

    private void Start()
    {
        bulletPool = FindAnyObjectByType<BulletPool>();
        forward3 = transform.right;
        if (isFlipped) {
            forward3 *= -1;
        }
        originalYPosition = transform.position.y;
    }

    private void Update()
    {
        attackCooldown -= Time.deltaTime * attackCooldownSpeed;
        blockTimer += Time.deltaTime * blockUptickSpeed;
        blockCooldownTimer -= Time.deltaTime * blockCooldownSpeed;

        if (attackCooldown <= 0)
        {
            attackCooldownSpeed = 0;
        }

        if (blockCooldownTimer <= 0)
        {
            blockCooldownSpeed = 0;
        }

        // Jump Update
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
        // Ignore input while cooldown up
        if (attackCooldown > 0)
        {
            return;
        }
        Bullet b = bulletPool.GetBullet().GetComponent<Bullet>();
        b.transform.position = transform.position + forward3 + (forward3 * 0.1f);
        b.Launch(CharacterData.characterParameters.bulletParams, forward3);
        attackCooldown = CharacterData.characterParameters.shotCooldown;
        attackCooldownSpeed = 1.0f;
    }


    private void OnBlock(InputValue input)
    {
        // Ignore input while cooldown up
        if (blockCooldownTimer > 0)
        {
            return;
        }

        if (input.isPressed)
        {
            BeginBlock();
        } else
        {
            ReleaseBlock();
        }
    }

    private void BeginBlock()
    {
        
        blockUptickSpeed = 1.0f;
        BlockingField.SetActive(true);
    }

    private void ReleaseBlock()
    {
        blockTimer = 0;
        blockUptickSpeed = 0.0f;
        blockCooldownSpeed = 1.0f;
        blockCooldownTimer = CharacterData.characterParameters.blockCooldown;
        BlockingField.SetActive(false);
    }

    private void OnJump(InputValue input)
    {
        if (bIsGrounded) { 
            bIsGrounded = false;
            jumpTimer = 0;
        }
    }

    public bool IsWithinParryWindow()
    {
        return blockTimer <= CharacterData.characterParameters.parryWindow;
    }
}
