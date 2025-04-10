using InputManagement;
using UnityEngine;
using UnityEngine.InputSystem;


// AI behavior implemented through a strategy pattern
// Sends inputs to the InputSystem handlers the character object has
public abstract class AbsAIStrategy
{
    Character character;
    protected InputValue defaultInputValue = new InputValue();
    public virtual void SetCharacter(Character character)
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

    protected void Block(bool value)
    {
        if (value)
        {
            character.SendMessage("BeginBlock");
        } else
        {
            character.SendMessage("ReleaseBlock");
        }
        
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
    bool isBlocking = false;
    public override void OnUpdate()
    {
        Block(isBlocking);
        isBlocking = !isBlocking;

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

public class RandomStrategy : AbsAIStrategy
{
    bool isBlocking;
    float timer;
    float inputTickWindow;

    public override void SetCharacter(Character character)
    {
        base.SetCharacter(character);
        inputTickWindow = character.BufferDuration * 6;
    }

    public override void OnUpdate()
    {
        timer += Time.deltaTime;
        if (timer > inputTickWindow) {
            timer = 0.0f;
            if (isBlocking)
            {
                isBlocking = false;
                Block(false);
                return;
            }
            EInput randomInput = (EInput)Random.Range(1, 4);
            switch (randomInput)
            {
                case EInput.Fireball:
                    Attack();
                    break;
                case EInput.Block:
                    Block(true);
                    isBlocking = true;
                    break;
                case EInput.Jump:
                    Jump();
                    break;
                default:
                    return;
            }
        }
    }
}