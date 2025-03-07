using UnityEngine;
using UnityEngine.InputSystem;
using InputManagement;
using System;

public class Character : MonoBehaviour
{ 
    [SerializeField]
    CharacterSO CharacterData;

    [SerializeField]
    GameObject BlockingField;

    [SerializeField]
    bool isFlipped;

    [SerializeField]
    float bufferDuration;

    [SerializeField]
    int bufferSize;

    HealthBarController healthBarController;
    InputBuffer inputBuffer;
    InputBufferItem bufferItem;
    StateMachine<CharacterStates> stateMachine;
    DebugInputBuffer inputBufferDebug;

    // Base values
    Vector3 forward3;
    float originalYPosition;
    BulletPool bulletPool;
   
    public bool IsGrounded {  get { return transform.position.y <= originalYPosition; } }
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
        inputBuffer = new InputBuffer(bufferDuration, bufferSize);
        bufferItem = new InputBufferItem();
        SetupStateMachine();
    }

    // State machine setup
    public void SetupStateMachine()
    {
        stateMachine = new StateMachine<CharacterStates>();
        
        // Create states
        NeutralState neutral = new NeutralState();
        neutral.SetCharacter(this);

        AttackingState attacking = new AttackingState();
        attacking.SetCharacter(this);

        JumpingState jumping = new JumpingState();
        jumping.SetCharacter(this);

        DownJumpingState down = new DownJumpingState();
        down.SetCharacter(this);

        ParryingState parry = new ParryingState();
        parry.SetCharacter(this);

        BlockingState blocking = new BlockingState();
        blocking.SetCharacter(this);

        // Set up transitions
        neutral.transitions[CharacterStates.Attacking].TargetState = attacking;
        neutral.transitions[CharacterStates.Jumping].TargetState = jumping;
        neutral.transitions[CharacterStates.Parrying].TargetState = parry;

        attacking.transitions[CharacterStates.Neutral].TargetState = neutral;
        
        jumping.transitions[CharacterStates.DownJumping].TargetState = down;
        jumping.transitions[CharacterStates.Neutral].TargetState = neutral;
        down.transitions[CharacterStates.Neutral].TargetState = neutral;

        parry.transitions[CharacterStates.Blocking].TargetState = blocking;
        parry.transitions[CharacterStates.Neutral].TargetState = neutral;
        blocking.transitions[CharacterStates.Neutral].TargetState = neutral;

        // Set ground state
        stateMachine.SetStartingState(neutral);
    }

    private void Update()
    {
        UpdateTimersAndUI();
        inputBufferDebug.Print(inputBuffer);

        //Input Buffer logic
        inputBuffer.Push(bufferItem);
        PlayerState s = (PlayerState)stateMachine.CurrentState;
        
        // s.SetMessage(); TODO - MAKE THIS WORK
        
        stateMachine.Update();


        bufferItem = new InputBufferItem();
    }

    /// <summary>
    /// Allows the state machine to update this object's vertical position
    /// </summary>
    /// <param name="value">The time parameter to pass</param>
    public void HandleJumpUpdate(float jumpTimer)
    {
        transform.position = new Vector3(transform.position.x, ParabolicPosition(jumpTimer), transform.position.z);
    }

    public void HandleDownJumpUpdate(float newYValue)
    {
        transform.position = new Vector3(transform.position.x, newYValue, transform.position.z);
    }

    public void ResetHeight()
    {
        transform.position = new Vector3(transform.position.x, originalYPosition, transform.position.z);
    }

    public float GetAttackDuration()
    {
        return CharacterData.characterParameters.attackDuration;
    }

    public float GetParryWindow()
    {
        return CharacterData.characterParameters.parryWindow;
    }

    
    
    // Utility methods //
    float ParabolicPosition(float value)
    {
        float t = jumpTimer / CharacterData.characterParameters.jumpDuration;
        return CharacterData.characterParameters.jumpHeight * ( -4 * t * t + 4 * t) + originalYPosition;
    }

    public void SetHealthBarController(HealthBarController controller)
    {
        healthBarController = controller;
    }

    public void SetInputBufferDebugger(DebugInputBuffer debugger)
    {
        inputBufferDebug = debugger;
    }

    // Updates all cooldowns and informs the UI of any relevant changes
    private void UpdateTimersAndUI()
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
        InputMessage im = new InputMessage(EInput.Fireball);
        bufferItem.AddInput(im);
    }
    private void OnBlock(InputValue input)
    {
        if (input.isPressed)
        {
            BeginBlock();
        }
        else
        {
            ReleaseBlock();
        }
    }

    private void OnJump(InputValue input)
    {
        InputMessage im = new InputMessage(EInput.Jump);
        bufferItem.AddInput(im);
        if (IsGrounded)
        {
            jumpTimer = 0;
        }
    }


    // Input specific actions //

    public void ThrowFireball()
    {
        Bullet b = bulletPool.GetBullet().GetComponent<Bullet>();
        b.transform.position = transform.position + forward3 + (forward3 * 0.1f);
        b.Launch(CharacterData.characterParameters.bulletParams, forward3);
        attackCooldown = CharacterData.characterParameters.shotCooldown;
        attackCooldownSpeed = 1.0f;
    }

    // What to do when the player begins blocking
    // TODO - research how to do input buffering to prevent players from feeling that they couldn't block
    private void BeginBlock()
    {
        InputMessage im = new InputMessage(EInput.Block);
        im.isRelease = false;
        bufferItem.AddInput(im);
    }

    // What to do when the player stops blocking
    private void ReleaseBlock()
    {
        InputMessage im = new InputMessage(EInput.Block);
        im.isRelease = true;
        bufferItem.AddInput(im);
    }

    
}
