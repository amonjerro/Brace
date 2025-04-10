using System.Collections.Generic;
/// <summary>
/// States for characters
/// </summary>
public enum CharacterStates
{
    Neutral,
    Jumping,
    DownJumping,
    Attacking,
    Blocking,
    Parrying,
    JumpAttack
}
/// <summary>
/// States for the game state machine
/// </summary>
public enum GameStates
{
    Countdown,
    Active,
    Over
}
/// <summary>
/// Class that defines the base behavior for any state in any state machine
/// </summary>
/// <typeparam name="EState"></typeparam>
public abstract class AbsState<EState> where EState : System.Enum
{
    protected StateMachine<EState> machine;
    protected EState stateValue;
    public Dictionary<EState,Transition<EState>> transitions;
    protected abstract void OnUpdate();
    protected abstract void OnEnter();
    protected abstract void OnExit();

    // Wrapper public methods
    public void Update()
    {
        OnUpdate();
    }

    public void Enter()
    {
        OnEnter();
    }

    public void Exit() { 
        OnExit(); 
    }

    // Debug functions
    public EState GetStateValue()
    {
        return stateValue;
    }

    public override string ToString()
    {
        return stateValue.ToString();
    }
}
