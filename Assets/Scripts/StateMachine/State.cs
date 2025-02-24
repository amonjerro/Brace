using System.Collections.Generic;
public enum CharacterStates
{
    Neutral,
    Jumping,
    DownJumping,
    Attacking,
    Blocking
}

public abstract class AbsState
{
    protected List<Transition> transitions;
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
}
