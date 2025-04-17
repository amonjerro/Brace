using System;

public enum ConditionType
{
    Not,
    And,
    Or,
    Range,
    Greater,
    Equals
}

// Conditions for the state machine transitions
public abstract class Condition
{
    protected static ConditionType conditionType;
    public abstract void Reset();
    public abstract bool Test();

    public override string ToString()
    {
        return conditionType.ToString();
    }

}


// Reverses the value of a condition to perform boolean NOT testing
public class NotCondition : Condition
{
    Condition ConditionA;

    public override bool Test()
    {
        return !ConditionA.Test();
    }

    public override void Reset()
    {
        ConditionA.Reset();
    }
}

// Abstract condition to parent all value evaluation conditions
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

/// <summary>
/// Checks to see if a value falls within a specified range
/// </summary>
/// <typeparam name="T">The type to compare and evaluate</typeparam>
public class WithinRangeCondition<T> : AbsValueCondition<T> where T : IComparable
{
    T MinValue;
    T MaxValue;

    public WithinRangeCondition(T minValue, T maxValue){
        MinValue = minValue;
        MaxValue = maxValue;
        conditionType = ConditionType.Range;
    }

    public override bool Test()
    {
        bool greaterThanMin = ConditionValue.CompareTo(MinValue) >= 0;
        bool lessThanMax = ConditionValue.CompareTo(MaxValue) <= 0;
        return greaterThanMin && lessThanMax;
    }
}

/// <summary>
/// Checks to see if a value is greater than some specified value
/// </summary>
/// <typeparam name="T">The type to compare and evaluate</typeparam>
public class GreaterThanCondition<T> : AbsValueCondition<T> where T : IComparable
{
    T Threshold;

    public GreaterThanCondition(T threshold)
    {
        Threshold = threshold;
        conditionType = ConditionType.Greater;
    }

    public override bool Test()
    {
        return ConditionValue.CompareTo(Threshold) >= 0;
    }
}


/// <summary>
/// Checks to see if a value is equal to some specified value
/// </summary>
/// <typeparam name="T">The type to compare and evaluate</typeparam>
public class EqualsCondition<T> : AbsValueCondition<T> where T : IComparable
{
    T ExpectedValue;

    public EqualsCondition(T expectedValue)
    {
        ExpectedValue = expectedValue;
        conditionType = ConditionType.Equals;
    }

    public override bool Test()
    {
        return ConditionValue.Equals(ExpectedValue);
    }
}

/// <summary>
/// Abstract parent to define conditions that compound other conditions
/// </summary>
public abstract class AbsCompoundCondition : Condition
{
    protected Condition ConditionA;
    protected Condition ConditionB;
    public Condition GetConditionA()
    {
        return ConditionA;
    }

    public Condition GetConditionB() {
        return ConditionB;
    }
    public override void Reset()
    {
        ConditionA.Reset();
        ConditionB.Reset();
    }
}

// Compound conditions types

/// <summary>
/// Performs an AND evaluation between two conditions
/// </summary>
public class AndCondition : AbsCompoundCondition
{
    
    public AndCondition(Condition conditionA, Condition conditionB)
    {
        ConditionA = conditionA;
        ConditionB = conditionB;
        conditionType = ConditionType.And;
    }


    public override bool Test()
    {
        return ConditionA.Test() && ConditionB.Test();
    }

    
}

/// <summary>
/// Performs an OR evaluation between two conditions
/// </summary>
public class OrCondition : AbsCompoundCondition
{

    public override bool Test()
    {
        return ConditionA.Test() || ConditionB.Test();
    }
}