using System.Collections.Generic;
public enum CharacterStates
{
    Neutral,
    Jumping,
    DownJumping,
    Attacking,
    Blocking,
    Parrying
}

public enum GameStates
{
    Countdown,
    Active,
    Over
}

public abstract class AbsState<EState> where EState : System.Enum
{
    protected EState stateValue;
    public Dictionary<EState,Transition<EState>> transitions;
    protected abstract void OnUpdate();
    protected abstract void OnEnter();
    protected abstract void OnExit();

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

    public EState GetStateValue()
    {
        return stateValue;
    }

    public override string ToString()
    {
        return stateValue.ToString();
    }
}
