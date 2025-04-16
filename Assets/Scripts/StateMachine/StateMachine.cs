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

    /// <summary>
    /// Gets the active state in this state machine
    /// </summary>
    /// <returns></returns>
    public EState GetCurrentState()
    {
        return _currentState.GetStateValue();
    }

    /// <summary>
    /// Stack a state in the state machine. Useful to temporarily create a "save point" for the state machine
    /// </summary>
    /// <param name="state">The state to stack</param>
    public void StackState(AbsState<EState> state)
    {
        stateStack.Push(state);
    }

    /// <summary>
    /// Unstack a state from the state machine
    /// </summary>
    /// <returns>The unstacked state</returns>
    public AbsState<EState> UnstackState()
    {
        return stateStack.Pop();
    }

    /// <summary>
    /// Restore this state machine to its original state.
    /// </summary>
    public void RestoreInitialState()
    {
        _currentState = _startingState;
    }
}