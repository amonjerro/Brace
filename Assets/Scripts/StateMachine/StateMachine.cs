using System.Collections.Generic;

/// <summary>
/// Generic state machine for the various game elements that will require one
/// </summary>
/// <typeparam name="EState">The enums that define the types of states this machine will use</typeparam>
public class StateMachine<EState> where EState : System.Enum
{

    AbsState<EState> _currentState;
    AbsState<EState> _startingState;
    public AbsState<EState> CurrentState { get { return _currentState; } }
    public Stack<AbsState<EState>> stateStack;

    public StateMachine()
    {
        stateStack = new Stack<AbsState<EState>>();
    }

    public void Update() {
        _currentState.Update();
        foreach(KeyValuePair<EState,Transition<EState>> kvp in _currentState.transitions)
        {
            if (kvp.Value.IsTriggered())
            {
                _currentState.Exit();
                _currentState = kvp.Value.TargetState;
                _currentState.Enter();
                break;
            }
        }

    }

    /// <summary>
    /// Sets the state that acts as the starting state for this machine
    /// </summary>
    /// <param name="state"></param>
    public void SetStartingState(AbsState<EState> state)
    {
        _startingState = state;
        _currentState = state;
    }

    public EState GetCurrentState()
    {
        return _currentState.GetStateValue();
    }

    public void StackState(AbsState<EState> state)
    {
        stateStack.Push(state);
    }
    public AbsState<EState> UnstackState()
    {
        return stateStack.Pop();
    }

    public void RestoreInitialState()
    {
        _currentState = _startingState;
    }
}