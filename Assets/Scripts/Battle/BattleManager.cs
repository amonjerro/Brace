using System.Collections.Generic;
using System;

/// <summary>
/// Handles the game state for battle
/// </summary>
public class BattleManager : AbsGameService
{
    StateMachine<GameStates> stateMachine;
    public static Action gameOver;

    List<Character> activeCharacters;


    /** Unity Lifecycle stuff **/
    private void Awake()
    {
        activeCharacters = new List<Character>();
    }


    public void Start()
    {
        SetupStateMachine();
    }

    private void Update()
    {
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
    /// Set up the transitions and state objects for this service´s state machine
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
}