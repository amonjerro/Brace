using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

/// <summary>
/// Handles the game state for battle
/// </summary>
public class BattleManager : AbsGameService
{
    [SerializeField]
    [Tooltip("The duration of hit stop time freeze in seconds")]
    float _hitStopTime;

    StateMachine<GameStates> stateMachine;
    public static Action gameOver;
    List<Character> activeCharacters;
    FightCamera cameraReference;

    public float HitstopTime { get { return _hitStopTime; } }


    /** Unity Lifecycle stuff **/
    private void Awake()
    {
        activeCharacters = new List<Character>();
        cameraReference = Camera.main.gameObject.GetComponent<FightCamera>();
    }


    public void Start()
    {
        Character.DamageTaken += ReactToDamageTaken;
        TimeUtil.Initialize();
        SetupStateMachine();
    }

    private void Update()
    {
        TimeUtil.UpdateDeltaTime();
        stateMachine.Update();
    }


    /// <summary>
    /// Resets the fight to its initial state to allow for rematches
    /// </summary>
    public void Reset()
    {
        foreach (Character c in activeCharacters) { 
            c.Reset();
        }

        TimeUtil.Initialize();
        stateMachine.RestoreInitialState();
        stateMachine.CurrentState.Enter();
    }

    /// <summary>
    /// Register the currently active characters on screen
    /// </summary>
    /// <param name="character">The character being registered</param>
    public void RegisterCharacter(Character character)
    {
        activeCharacters.Add(character);
        
    }

    /// <summary>
    /// Set up the transitions and state objects for this service�s state machine
    /// </summary>
    public void SetupStateMachine()
    {
        // Create the machine
        stateMachine = new StateMachine<GameStates>();

        // Create the states
        CountdownState countdownState = new CountdownState();
        ActiveGameplayState activeGameplayState = new ActiveGameplayState();
        GameOverState gameOverState = new GameOverState();
        countdownState.transitions[GameStates.Active].TargetState = activeGameplayState;
        activeGameplayState.transitions[GameStates.Over].TargetState = gameOverState;
        
        // Set the root
        stateMachine.SetStartingState(countdownState);
    }

    /// <summary>
    /// Set the game to over - might change accessibilty and functionality of this since this was created
    /// before this class was made a service
    /// </summary>
    public void SetGameToOver()
    {
        gameOver?.Invoke();
    }


    /// <summary>
    /// Perform service clean up
    /// </summary>
    public override void CleanUp()
    {
        Character.DamageTaken -= ReactToDamageTaken;
        gameOver = null;
    }

    /// <summary>
    /// Enable or disable inputs for players, as needed
    /// </summary>
    /// <param name="val">What state to put inputs in</param>
    public void SetCharacterInputStatus(bool val)
    {
        foreach (Character c in activeCharacters)
        {
            c.SetInputStatus(val);
        }
    }

    public void ReactToDamageTaken()
    {
        Debug.Log("Damage Taken!");
        cameraReference.InitiateShake();
        StartCoroutine(RunHitstop());
    }

    // Stops the game for the duration of hit stop to add that juicy feel
    IEnumerator RunHitstop()
    {
        TimeUtil.timeScale = 0.0f;
        yield return new WaitForSeconds(_hitStopTime);
        TimeUtil.timeScale = 1.0f;
    }
}