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

    private void Awake()
    {
        activeCharacters = new List<Character>();
    }


    public void Start()
    {
        SetupStateMachine();
    }

    public void Reset()
    {
        foreach (Character c in activeCharacters) { 
            c.Reset();
        }

        stateMachine.RestoreInitialState();
        stateMachine.CurrentState.Enter();
    }

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

    private void Update()
    {
        stateMachine.Update();
    }

    public void SetGameToOver()
    {
        gameOver?.Invoke();
    }

    public override void CleanUp()
    {
        gameOver = null;
    }
}