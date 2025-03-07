using System;
using UnityEngine;

// Conditions for the state machine transitions
public abstract class Condition
{
    public abstract void Reset();
    public abstract bool Test();
}

public abstract class AbsValueCondition<T> : Condition where T : IComparable
{
    protected T ConditionValue;
    public void SetValue(T value)
    {
        ConditionValue = value;
    }

    public override void Reset()
    {
        ConditionValue = default(T);
    }
}

// Evaluation Conditions
public class WithinRangeCondition<T> : AbsValueCondition<T> where T : IComparable
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

public class GreaterThanCondition<T> : AbsValueCondition<T> where T : IComparable
{
    T Threshold;

    public GreaterThanCondition(T threshold)
    {
        Threshold = threshold;
    }

    public override bool Test()
    {
        return ConditionValue.CompareTo(Threshold) >= 0;
    }
}

public class EqualsCondition<T> : AbsValueCondition<T> where T : IComparable
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
    public AndCondition(Condition conditionA, Condition conditionB)
    {
        ConditionA = conditionA;
        ConditionB = conditionB;
    }


    public override bool Test()
    {
        return ConditionA.Test() && ConditionB.Test();
    }

    public override void Reset() {
        ConditionA.Reset();
        ConditionB.Reset();
    }
}

public class NotCondition : Condition
{
    Condition ConditionA;

    public override bool Test()
    {
        return !ConditionA.Test();
    }

    public override void Reset() {
        ConditionA.Reset();
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

    public override void Reset() {
        ConditionA.Reset();
        ConditionB.Reset();
    }
}