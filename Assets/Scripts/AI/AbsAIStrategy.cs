using UnityEngine;
using UnityEngine.InputSystem;
public abstract class AbsAIStrategy
{
    Character character;
    InputValue nullInputValue = new InputValue();
    public void SetCharacter(Character character)
    {
        this.character = character;
    }

    protected void Jump()
    {
        character.SendMessage("OnJump", nullInputValue);
    }

    protected void Attack()
    {
        character.SendMessage("OnAttack", nullInputValue);
    }

    protected void Block()
    {
        character.SendMessage("OnBlock", nullInputValue);
    }

    public abstract void OnUpdate();
}


public class JumperStrategy : AbsAIStrategy
{
    public override void OnUpdate()
    {
        Jump();
    }
}

public class BlockerStrategy : AbsAIStrategy
{
    public override void OnUpdate()
    {
        Block();
    }
}

public class ShooterStrategy : AbsAIStrategy
{
    public override void OnUpdate()
    {
        Attack();
    }
}

public class StaticStrategy : AbsAIStrategy
{
    public override void OnUpdate()
    {
        return;
    }
}