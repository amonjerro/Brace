using UnityEngine;
using UnityEngine.InputSystem;
using InputManagement;
using GameMenus;

public class Character : MonoBehaviour
{
    // Dependency Configurations
    [SerializeField]
    InputConfigSO InputConfig;

    [SerializeField]
    CharacterSO CharacterData;

    [SerializeField]
    Shield BlockingField;

    [SerializeField]
    bool isFlipped;

    [SerializeField]
    float _bufferDuration;

    public float BufferDuration { get { return _bufferDuration; } }

    [SerializeField]
    int bufferSize;

    [SerializeField]
    CharacterStates currentState;

    // Internals
    HealthBarController healthBarController;
    InputBuffer inputBuffer;
    InputBufferItem bufferItem;
    StateMachine<CharacterStates> stateMachine;
    DebugInputBuffer inputBufferDebug;

    // Base values
    Vector3 forward3;
    float originalYPosition;
    BulletPool bulletPool;
    bool bInputsEnabled = true;
    bool bCanBeDamaged = true;

    public bool IsGrounded {  get { return transform.position.y <= originalYPosition; } }

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
        Initialize();
    }

    public void Reset()
    {
        currentHealth = MAX_HEALTH;
        healthBarController.UpdateHealth(currentHealth / MAX_HEALTH);
        stateMachine.RestoreInitialState();
        bCanBeDamaged = true;
    }

    public void SetInputStatus(bool status)
    {
        bInputsEnabled = status;
    }

    private void Initialize()
    {
        bulletPool = FindAnyObjectByType<BulletPool>();
        forward3 = transform.right;
        if (isFlipped)
        {
            forward3 *= -1;
        }
        originalYPosition = transform.position.y;
        ConfigureInputBuffer();
        SetupStateMachine();
        MenuManager.PlayInputSwitch += SetInputToGame;
        MenuManager.UIInputSwitch += SetInputToUI;
    }

    private void ConfigureInputBuffer()
    {
        InputBuffer.SetPriorities(InputConfig.MakePriorities());
        inputBuffer = new InputBuffer(_bufferDuration, bufferSize);
        bufferItem = new InputBufferItem();
    }

    // State machine setup
    public void SetupStateMachine()
    {
        stateMachine = new StateMachine<CharacterStates>();
        
        // Create states
        NeutralState neutral = new NeutralState(stateMachine);
        neutral.SetCharacter(this);

        AttackingState attacking = new AttackingState(stateMachine);
        attacking.SetCharacter(this);

        JumpingState jumping = new JumpingState(stateMachine);
        jumping.SetCharacter(this);

        DownJumpingState down = new DownJumpingState(stateMachine);
        down.SetCharacter(this);

        JumpAttackState jumpAttack = new JumpAttackState(stateMachine);
        jumpAttack.SetCharacter(this);

        ParryingState parry = new ParryingState(stateMachine);
        parry.SetCharacter(this);

        BlockingState blocking = new BlockingState(stateMachine);
        blocking.SetCharacter(this);


        // Set up transitions
        neutral.transitions[CharacterStates.Attacking].TargetState = attacking;
        neutral.transitions[CharacterStates.Jumping].TargetState = jumping;
        neutral.transitions[CharacterStates.Parrying].TargetState = parry;

        attacking.transitions[CharacterStates.Neutral].TargetState = neutral;
        
        jumping.transitions[CharacterStates.DownJumping].TargetState = down;
        jumping.transitions[CharacterStates.Neutral].TargetState = neutral;
        jumping.transitions[CharacterStates.JumpAttack].TargetState = jumpAttack;

        down.transitions[CharacterStates.Neutral].TargetState = neutral;
        jumpAttack.transitions[CharacterStates.DownJumping].TargetState = down;

        parry.transitions[CharacterStates.Blocking].TargetState = blocking;

        // Set ground state
        stateMachine.SetStartingState(neutral);
    }

    private void Update()
    {
        currentState = stateMachine.GetCurrentState();
        UpdateTimersAndUI();
        inputBufferDebug.Print(inputBuffer);
        inputBuffer.Update(Time.deltaTime);

        //Input Buffer logic
        if (inputBuffer.PushFlag)
        {
            inputBuffer.Push(bufferItem);
            bufferItem = new InputBufferItem();
        }
        inputBuffer.UpdateActiveMessage();

        // Pass the message to the state machine
        if (inputBuffer.GetActiveMessage() != null)
        {
            PlayerState s = (PlayerState)stateMachine.CurrentState;
            s.SetMessage(inputBuffer.GetActiveMessage());
        }

        stateMachine.Update();
        
    }

    /// <summary>
    /// Allows the state machine to update this object's vertical position
    /// </summary>
    /// <param name="value">The time parameter to pass</param>
    public void HandleJumpUpdate(float jumpTimer)
    {
        transform.position = new Vector3(transform.position.x, ParabolicPosition(jumpTimer), transform.position.z);
    }

    /// <summary>
    /// Allows the state machine to update this object's vertical position with regards to down jumping
    /// </summary>
    /// <param name="newYValue">The new Y value for this object</param>
    public void HandleDownJumpUpdate(float newYValue)
    {
        float adjustedValue = transform.position.y - newYValue;
        transform.position = new Vector3(transform.position.x, adjustedValue, transform.position.z);
    }

    /// <summary>
    /// Resets this object's height to the original value
    /// </summary>
    public void ResetHeight()
    {
        transform.position = new Vector3(transform.position.x, originalYPosition, transform.position.z);
    }

    /// <summary>
    /// Provide access to the duration in seconds of the attack animation
    /// </summary>
    /// <returns></returns>
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
        float t = value / CharacterData.characterParameters.jumpDuration;
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

    public void ThrowFireball()
    {
        Bullet b = bulletPool.GetBullet().GetComponent<Bullet>();
        b.transform.position = transform.position + forward3 + (forward3 * 0.1f);
        b.Launch(CharacterData.characterParameters.bulletParams, forward3);
        attackCooldown = CharacterData.characterParameters.shotCooldown;
        attackCooldownSpeed = 1.0f;
    }

    // Handle Blocking //
    public void EnableBlock()
    {
        BlockingField.gameObject.SetActive(true);
    }

    public void SetBlockToParry(bool value)
    {
        BlockingField.IsParry = value;
    }

    public void DisableBlock()
    {
        BlockingField.gameObject.SetActive(false);
    }


    // Handle Player Input // 
    private void OnAttack(InputValue input)
    {
        if (!bInputsEnabled) return;
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
        if (!bInputsEnabled) return;
        InputMessage im = new InputMessage(EInput.Jump);
        bufferItem.AddInput(im);
    }

    private void OnPause(InputValue input)
    {
        ServiceLocator.Instance.GetService<MenuManager>().OpenMenu(Menus.Pause);
    }

    
    // What to do when the player begins blocking
    private void BeginBlock()
    {
        if (!bInputsEnabled) return;
        InputMessage im = new InputMessage(EInput.Block);
        im.isRelease = false;
        bufferItem.AddInput(im);
    }

    // What to do when the player stops blocking
    private void ReleaseBlock()
    {
        if (!bInputsEnabled) return;
        InputMessage im = new InputMessage(EInput.Block);
        im.isRelease = true;
        bufferItem.AddInput(im);
    }

    private void SetInputToUI()
    {
        PlayerInput pi = GetComponent<PlayerInput>();
        if (pi != null) {
            pi.SwitchCurrentActionMap("UI");
        }
    }
    
    public void SetInputToGame()
    {
        PlayerInput pi = GetComponent<PlayerInput>();
        if (pi != null)
        {
            pi.SwitchCurrentActionMap("Player");
        }
        
    }
}
