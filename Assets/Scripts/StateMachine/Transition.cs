using System.Collections.Generic;

public class Transition
{
    Condition condition;
    public AbsState TargetState {  get; private set; }
    public bool IsTriggered() { return condition.Test(); }
}