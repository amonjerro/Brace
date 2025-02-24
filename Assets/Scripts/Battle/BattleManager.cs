using System;
using UnityEngine;

/// <summary>
/// Handles the game state for battle
/// </summary>
public class BattleManager : MonoBehaviour
{
    StateMachine<GameStates> stateMachine;
    public static Action gameOver;


    public void Start()
    {
        SetupStateMachine();

        // Start the sequence
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
        countdownState.transitions[0].TargetState = activeGameplayState;
        activeGameplayState.transitions[0].TargetState = gameOverState;
        
        // Set the root
        stateMachine.SetStartingState(countdownState);
    }

    public void SetGameToOver()
    {
        gameOver?.Invoke();
    }

}