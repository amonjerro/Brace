/// <summary>
/// Utility class for state machine transitions.
/// Informs what state should be transitioned into should its conditions be met
/// </summary>
/// <typeparam name="EState"></typeparam>
public class Transition<EState> where EState : System.Enum
{
    Condition condition;
    public AbsState<EState> TargetState {  get; set; }
    public bool IsTriggered() { return condition.Test(); }

    public void SetCondition(Condition c)
    {
        condition = c;
    }

    public Condition GetCondition() { return condition; }

    public void ResetCondition()
    {
        condition.Reset();
    }
}