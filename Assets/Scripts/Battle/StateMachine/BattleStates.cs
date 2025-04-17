using UnityEngine;
using System.Collections.Generic;
using GameMenus;

// This class triggers the countdown at the start of the fight
public class CountdownState : AbsState<GameStates>
{
    BattleManager managerReference;
    float countdownTimer;
    WithinRangeCondition<float> countdownCondition;
    public CountdownState(BattleManager managerReference)
    {
        transitions = new Dictionary<GameStates, Transition<GameStates>>();
        Transition<GameStates> toStart = new Transition<GameStates>();
        countdownCondition = new WithinRangeCondition<float>(2.99f, 3.01f);
        toStart.SetCondition(countdownCondition);
        transitions.Add(GameStates.Active, toStart);
        stateValue = GameStates.Countdown;
        this.managerReference = managerReference;
    }

    protected override void OnEnter()
    {
        // Trigger countdown UI
        Time.timeScale = 1f;
        TimeUtil.timeScale = 0.0f;

        // Disable inputs
        managerReference.SetCharacterInputStatus(false);
        managerReference.SetCharacterVulnerabilities(false);
    }

    protected override void OnExit()
    {
        // Reset state
        countdownTimer = 0.0f;
        countdownCondition.Reset();
        TimeUtil.timeScale = 1.0f;

        // Remove countdown UI

        // Enable inputs
        managerReference.SetCharacterInputStatus(true);
        managerReference.SetCharacterVulnerabilities(true);
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
        gameEndCondition.Reset();
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

        // Show the end-of-game UI
        ServiceLocator.Instance.GetService<MenuManager>().OpenMenu(Menus.EndBanner);
        ServiceLocator.Instance.GetService<BattleManager>().SetCharacterInputStatus(false);
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
