using System;

// Conditions for the state machine transitions
public abstract class Condition
{
    public abstract bool Test();
}

public abstract class AbsValueCondition<T> : Condition where T : IComparable<T>
{
    protected T ConditionValue;
    public void SetValue(T value)
    {
        ConditionValue = value;
    }
}

// Evaluation Conditions
public class WithinRangeCondition<T> : AbsValueCondition<T> where T : IComparable<T>
{
    T MinValue;
    T MaxValue;

    public WithinRangeCondition(T minValue, T maxValue){
        MinValue = minValue;
        MaxValue = maxValue;
    }

    public override bool Test()
    {
        bool greaterThanMin = ConditionValue.CompareTo(MinValue) >= 0;
        bool lessThanMax = ConditionValue.CompareTo(MaxValue) <= 0;
        return greaterThanMin && lessThanMax;
    }
}

public class EqualsCondition<T> : AbsValueCondition<T> where T : IComparable<T>
{
    T ExpectedValue;

    public EqualsCondition(T expectedValue)
    {
        ExpectedValue = expectedValue;
    }

    public override bool Test()
    {
        return ConditionValue.Equals(ExpectedValue);
    }
}

// Compound conditions types
public class AndCondition : Condition
{
    Condition ConditionA;
    Condition ConditionB;

    public override bool Test()
    {
        return ConditionA.Test() && ConditionB.Test();
    }
}

public class NotCondition : Condition
{
    Condition ConditionA;

    public override bool Test()
    {
        return !ConditionA.Test();
    }
}

public class OrCondition : Condition
{
    Condition ConditionA;
    Condition ConditionB;

    public override bool Test()
    {
        return ConditionA.Test() || ConditionB.Test();
    }
}