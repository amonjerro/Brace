using UnityEngine;
using UnityEngine.InputSystem;
using InputManagement;

public class Character : MonoBehaviour
{ 
    [SerializeField]
    CharacterSO CharacterData;

    [SerializeField]
    GameObject BlockingField;

    [SerializeField]
    bool isFlipped;

    HealthBarController healthBarController;
    InputBuffer inputBuffer;
    StateMachine<CharacterStates> stateMachine;
    
    // Base values
    Vector3 forward3;
    float originalYPosition;
    BulletPool bulletPool;
    bool bIsGrounded = true;
    bool bCanBeDamaged = true;
    bool bBlockIsPressed = false;

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
    const float TWO_PI = 2 * Mathf.PI;
    const float MAX_HEALTH = 5.0f;
    float currentHealth = 5.0f;

    // Unity lifecycle // 
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
        UpdateTimersAndUI();

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
    
    // Utility methods //
    float ParabolicPosition()
    {
        float t = jumpTimer / CharacterData.characterParameters.jumpDuration;
        return CharacterData.characterParameters.jumpHeight * ( -4 * t * t + 4 * t) + originalYPosition;
    }

    public void SetHealthBarController(HealthBarController controller)
    {
        healthBarController = controller;
    }

    // Updates all cooldowns and informs the UI of any relevant changes
    public void UpdateTimersAndUI()
    {
        attackCooldown -= Time.deltaTime * attackCooldownSpeed;
        blockTimer += Time.deltaTime * blockUptickSpeed;
        blockCooldownTimer -= Time.deltaTime * blockCooldownSpeed;

        if (attackCooldown <= 0)
        {
            attackCooldownSpeed = 0;
        }
        healthBarController.UpdateCooldown(CooldownType.Shoot, TWO_PI * (attackCooldown / CharacterData.characterParameters.shotCooldown));

        if (blockCooldownTimer <= 0)
        {
            blockCooldownSpeed = 0;
        }
        healthBarController.UpdateCooldown(CooldownType.Block, TWO_PI * (blockCooldownTimer / CharacterData.characterParameters.blockCooldown));
    }

    // Gameplay functions //

    // Test whether the block still counts as a parry
    public bool IsWithinParryWindow()
    {
        return blockTimer <= CharacterData.characterParameters.parryWindow;
    }

    // Change the player's life total based on the damage they've taken.
    // Show feedback to the player that damage has been done
    public void TakeDamage(float value)
    {
        if (!bCanBeDamaged)
        {
            return;
        }
        currentHealth -= value;
        healthBarController.UpdateHealth(currentHealth / MAX_HEALTH);
        if (currentHealth <= 0)
        {
            // End Game
            bCanBeDamaged = false;
            ServiceLocator.Instance.GetService<BattleManager>().SetGameToOver();
        }
    }

    // Handle Player Input // 
    private void OnAttack(InputValue input)
    {
        // Ignore input while cooldown up or is blocking
        if (attackCooldown > 0 || bBlockIsPressed)
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
            if (!bBlockIsPressed)
            {
                return;
            }
            ReleaseBlock();
        }
    }

    // What to do when the player begins blocking
    // TODO - research how to do input buffering to prevent players from feeling that they couldn't block
    private void BeginBlock()
    {
        
        blockUptickSpeed = 1.0f;
        BlockingField.SetActive(true);
        bBlockIsPressed = true;
    }

    // What to do when the player stops blocking
    private void ReleaseBlock()
    {
        blockTimer = 0;
        blockUptickSpeed = 0.0f;
        blockCooldownSpeed = 1.0f;
        blockCooldownTimer = CharacterData.characterParameters.blockCooldown;
        BlockingField.SetActive(false);
        bBlockIsPressed = false;
    }

    private void OnJump(InputValue input)
    {
        if (bIsGrounded) { 
            bIsGrounded = false;
            jumpTimer = 0;
        }
    }


    
}
