using UnityEngine;
using UnityEngine.InputSystem;
using InputManagement;
using GameMenus;
using System;
using UnityEditor.Networking.PlayerConnection;

public class Character : MonoBehaviour
{
    // Dependency Configurations
    [SerializeField]
    [Tooltip("Input Configuration Scriptable Object to inform how priorities are configured")]
    InputConfigSO InputConfig;

    [SerializeField]
    [Tooltip("The game object that corresponds to the shield for this character")]
    Shield BlockingField;

    [SerializeField]
    [Tooltip("Whether this character sprite should be mirrored. Relevant for the right side character.")]
    bool isFlipped;

    [SerializeField]
    [Tooltip("Duration in seconds for each input buffer frame")]
    float _bufferDuration;

    public float BufferDuration { get { return _bufferDuration; } }

    [SerializeField]
    [Tooltip("How many buffer frames should be contained in the input buffer")]
    int bufferSize;

    [SerializeField]
    [Tooltip("Read-only for state machine debugging purposes")]
    CharacterStates currentState;

    public static Action<bool> DamageTaken;

    // Internals
    HealthBarController healthBarController;
    InputBuffer inputBuffer;
    InputBufferItem bufferItem;
    StateMachine<CharacterStates> stateMachine;
    DebugInputBuffer inputBufferDebug;
    CharacterSO CharacterData;

    // Base values
    Vector3 forward3;
    float originalYPosition;
    BulletPool bulletPool;
    bool bInputsEnabled = true;
    bool bCanBeDamaged = true;
    int _playerIndex;

    public bool IsGrounded {  get { return transform.position.y <= originalYPosition; } }
    public bool ChargesAvailable {  get { return charges > 0; } }
    public int PlayerIndex { get { return _playerIndex; } }

    // Timers for game actions
    float blockTimer = 0;
    float blockUptickSpeed = 0;
    float blockCooldownTimer = 0;
    float blockCooldownSpeed = 0;
    float attackCooldown = 0;
    float attackCooldownSpeed = 0;
    int charges = 0;
    
    // Constants
    const float TWO_PI = 2 * Mathf.PI;
    const float MAX_HEALTH = 5.0f;
    float currentHealth = 5.0f;

    // Unity lifecycle // 
    #region UNITY_LIFECYCLE
    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        currentState = stateMachine.GetCurrentState();
        UpdateTimersAndUI();
        inputBufferDebug.Print(inputBuffer);
        inputBuffer.Update(TimeUtil.GetDelta());

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
    /// Resets the state of a player to its initial values
    /// </summary>
    public void Reset()
    {
        currentHealth = MAX_HEALTH;
        healthBarController.UpdateHealth(currentHealth / MAX_HEALTH);
        stateMachine.RestoreInitialState();
        bCanBeDamaged = true;
    }
    #endregion

    #region DEPENDENCY_SETUP
    // Character state access functions //

    public void SetHealthBarController(HealthBarController controller)
    {
        healthBarController = controller;
    }

    public void SetInputBufferDebugger(DebugInputBuffer debugger)
    {
        inputBufferDebug = debugger;
    }

    public void SetCharacterData(CharacterSO data)
    {
        CharacterData = data;
    }
    #endregion

    #region GAME_STATE_HANDLING
    /// <summary>
    /// Sets the state of inputs for the players
    /// </summary>
    /// <param name="status"></param>
    public void SetInputStatus(bool status)
    {
        bInputsEnabled = status;
    }

    /// <summary>
    /// Set the control of player inputs back to game mode
    /// </summary>
    public void SetInputToGame()
    {
        PlayerInput pi = GetComponent<PlayerInput>();
        if (pi != null)
        {
            pi.SwitchCurrentActionMap("Player");
        }

    }

    /// <summary>
    /// Adds a charge counter
    /// </summary>
    public void AddCharge()
    {
        charges++;
    }

    /// <summary>
    /// Spends a charge counter
    /// </summary>
    public void ExpendCharge()
    {
        charges--;
    }

    /// <summary>
    /// Allows the state machine to update this object's vertical position
    /// </summary>
    /// <param name="value">The time parameter to pass</param>
    public void HandleJumpUpdate(float jumpTimer)
    {

        float yPosition = MathUtils.ParabolicPosition(
                                        jumpTimer, originalYPosition, 
                                        CharacterData.characterParameters.jumpDuration, 
                                        CharacterData.characterParameters.jumpHeight
                                        );
        transform.position = new Vector3(transform.position.x, yPosition, transform.position.z);
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

    public void SetIndex(int index)
    {
        _playerIndex = index;
    }
    #endregion

    #region DATA_ACCESSORS
    /// <summary>
    /// Provide access to the duration in seconds of the attack animation
    /// </summary>
    /// <returns></returns>
    public float GetAttackDuration()
    {
        return CharacterData.characterParameters.attackDuration;
    }

    /// <summary>
    /// Get the duration of the parry window for this character
    /// </summary>
    /// <returns>The parry window in seconds</returns>
    public float GetParryWindow()
    {
        return CharacterData.characterParameters.parryWindow;
    }

    /// <summary>
    /// Get the type of parry effect this character has
    /// </summary>
    /// <returns>The type of parry effect they apply</returns>
    public ParryEffect GetParryType()
    {
        return CharacterData.characterParameters.shieldParams.effect;
    }

    /// <summary>
    /// Get the sprite for this character parry animation
    /// </summary>
    /// <returns></returns>
    public Sprite GetParrySprite()
    {
        return CharacterData.characterParameters.shieldParams.parrySprite;
    }
    #endregion

    #region GAMEPLAY_FUNCTIONS
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
        DamageTaken?.Invoke(BlockingField.gameObject.activeInHierarchy);
        if (currentHealth <= 0)
        {
            // End Game
            bCanBeDamaged = false;
            ServiceLocator.Instance.GetService<BattleManager>().EndRound(PlayerIndex);
        }
    }

    /// <summary>
    /// Request a projectile from the pool, initialize it and send it on its way
    /// </summary>
    public void ThrowFireball()
    {
        Bullet b = bulletPool.GetBullet().GetComponent<Bullet>();
        b.transform.position = transform.position + forward3 + (forward3 * 0.1f);
        b.Launch(CharacterData.characterParameters.bulletParams, forward3);
        attackCooldown = CharacterData.characterParameters.shotCooldown;
        attackCooldownSpeed = 1.0f; // To do: Replace this hardcoded number
    }

    // Handle Blocking //
    // i.e. manipulate the Shield object's state
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
    #endregion


    #region PRIVATE_FUNCTIONS
    // Private Functions //

    /// <summary>
    /// Set references, create and populate the state machine, configure the input buffer and suscribe to menu events
    /// Any initialization and resource acquisition should happen here.
    /// Called on Start, not on Awake.
    /// </summary>
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

    /// <summary>
    /// Create and configure the input buffer based on scriptable object configuration settings.
    /// </summary>
    private void ConfigureInputBuffer()
    {
        InputBuffer.SetPriorities(InputConfig.MakePriorities());
        inputBuffer = new InputBuffer(_bufferDuration, bufferSize);
        bufferItem = new InputBufferItem();
    }

    // State machine setup
    private void SetupStateMachine()
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

    // Updates all cooldowns and informs the UI of any relevant changes
    private void UpdateTimersAndUI()
    {
        attackCooldown -= TimeUtil.GetDelta() * attackCooldownSpeed;
        blockTimer += TimeUtil.GetDelta() * blockUptickSpeed;
        blockCooldownTimer -= TimeUtil.GetDelta() * blockCooldownSpeed;

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

    #endregion
}
