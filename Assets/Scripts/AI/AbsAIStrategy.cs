using UnityEngine.InputSystem;

// AI behavior implemented through a strategy pattern
// Sends inputs to the InputSystem handlers the character object has
public abstract class AbsAIStrategy
{
    Character character;
    protected InputValue defaultInputValue = new InputValue();
    public void SetCharacter(Character character)
    {
        this.character = character;
    }

    protected void Jump()
    {
        character.SendMessage("OnJump", defaultInputValue);
    }

    protected void Attack()
    {
        character.SendMessage("OnAttack", defaultInputValue);
    }

    protected void Block()
    {
        character.SendMessage("OnBlock", defaultInputValue);
    }

    public abstract void OnUpdate();
}

// This AI only jumps
public class JumperStrategy : AbsAIStrategy
{
    public override void OnUpdate()
    {
        Jump();
    }
}

// This AI only blocks
public class BlockerStrategy : AbsAIStrategy
{
    public override void OnUpdate()
    {
        Block();
    }
}

// This AI only shoots
public class ShooterStrategy : AbsAIStrategy
{
    public override void OnUpdate()
    {
        Attack();
    }
}

// This AI just sits there
public class StaticStrategy : AbsAIStrategy
{
    public override void OnUpdate()
    {
        return;
    }
}