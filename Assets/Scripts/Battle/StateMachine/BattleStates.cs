using UnityEngine;
using System.Collections.Generic;
using GameMenus;

// This class triggers the countdown at the start of the fight
public class CountdownState : AbsState<GameStates>
{
    
    float countdownTimer;
    WithinRangeCondition<float> countdownCondition;
    public CountdownState()
    {
        transitions = new Dictionary<GameStates,Transition<GameStates>>();
        Transition<GameStates> toStart = new Transition<GameStates>();
        countdownCondition = new WithinRangeCondition<float>(2.99f, 3.01f);
        toStart.SetCondition(countdownCondition);
        transitions.Add(GameStates.Active,toStart);
        stateValue = GameStates.Countdown;
    }

    protected override void OnEnter()
    {
        // Trigger countdown UI
        Time.timeScale = 1f;
        // Disable inputs
    }

    protected override void OnExit()
    {
        // Remove countdown UI

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
        transitions = new Dictionary<GameStates, Transition<GameStates>>();
        Transition<GameStates> toEnd = new Transition<GameStates>();
        gameEndCondition = new EqualsCondition<bool>(true);
        toEnd.SetCondition(gameEndCondition);
        transitions.Add(GameStates.Over, toEnd);
        stateValue = GameStates.Active;

    }
    protected override void OnEnter() {
        BattleManager.gameOver += ListenToGameOver;
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
        transitions = new Dictionary<GameStates, Transition<GameStates>>();
        stateValue = GameStates.Over;
    }
    protected override void OnEnter()
    {
        Debug.Log("Game is over!");

        // Show the end-of-game UI
        ServiceLocator.Instance.GetService<MenuManager>().OpenMenu(Menus.EndBanner);
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
