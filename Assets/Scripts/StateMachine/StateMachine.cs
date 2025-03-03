using UnityEngine;

/// <summary>
/// Generic state machine for the various game elements that will require one
/// </summary>
/// <typeparam name="EState">The enums that define the types of states this machine will use</typeparam>
public class StateMachine<EState> where EState : System.Enum
{

    AbsState<EState> _currentState;
    public AbsState<EState> CurrentState { get { return _currentState; } }

    public void Update() {
        _currentState.Update();
        foreach(Transition<EState> transition in _currentState.transitions)
        {
            if (transition.IsTriggered())
            {
                _currentState.Exit();
                _currentState = transition.TargetState;
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
        _currentState = state;
    }
}