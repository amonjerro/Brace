using UnityEngine;
using System.Collections.Generic;

// This class triggers the countdown at the start of the fight
public class CountdownState : AbsState<GameStates>
{
    
    float countdownTimer;
    WithinRangeCondition<float> countdownCondition;
    public CountdownState()
    {
        transitions = new List<Transition<GameStates>>();
        Transition<GameStates> toStart = new Transition<GameStates>();
        countdownCondition = new WithinRangeCondition<float>(2.99f, 3.01f);
        toStart.SetCondition(countdownCondition);
        transitions.Add(toStart);
        stateValue = GameStates.Countdown;
    }

    protected override void OnEnter()
    {
        // Trigger countdown UI
        Debug.Log("Timer started");

        // Disable inputs
    }

    protected override void OnExit()
    {
        // Remove countdown UI
        Debug.Log("Timer Ended");

        // Enable inputs
    }

    protected override void OnUpdate()
    {
        countdownTimer += Time.deltaTime;
        countdownCondition.SetValue(countdownTimer);
    }
}

// This state listens for the game over signal in order to transition into the game end phase
public class ActiveGameplayState : AbsState<GameStates>
{
    EqualsCondition<bool> gameEndCondition;
    public ActiveGameplayState()
    {
        transitions = new List<Transition<GameStates>>();
        Transition<GameStates> toEnd = new Transition<GameStates>();
        gameEndCondition = new EqualsCondition<bool>(true);
        toEnd.SetCondition(gameEndCondition);
        transitions.Add(toEnd);
        stateValue = GameStates.Active;

    }
    protected override void OnEnter() {
        BattleManager.gameOver += ListenToGameOver;
        Debug.Log("Entered Active Gameplay State");
    }
    protected override void OnExit()
    {
        BattleManager.gameOver -= ListenToGameOver;
    }
    protected override void OnUpdate()
    {
        // Do nothing during update
    }

    private void ListenToGameOver()
    {
        gameEndCondition.SetValue(true);
    }
}

// This class shows the results UI and signals the game is over
public class GameOverState : AbsState<GameStates>
{
    public GameOverState()
    {
        transitions = new List<Transition<GameStates>>();
        stateValue = GameStates.Over;
    }
    protected override void OnEnter()
    {
        // Pause the game
        Time.timeScale = 0;

        // Show the end-of-game UI
    }
    protected override void OnExit()
    {
        // Unpause the game
        Time.timeScale = 1;
    }

    protected override void OnUpdate()
    {
        // Does nothing on update
    }
}
