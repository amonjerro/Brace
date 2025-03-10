using System.Collections.Generic;
using UnityEngine;
using InputManagement;

/// <summary>
/// Player states require a little more information than just the abstract state
/// They need to be informed of the inputs the player is pressing in order to transition the player into new states
/// </summary>
public abstract class PlayerState : AbsState<CharacterStates>{
    protected InputMessage activeMessage;
    protected Character characterReference;
    public PlayerState()
    {
        activeMessage = null;
        transitions = new Dictionary<CharacterStates, Transition<CharacterStates>>();
    }

    public void SetMessage(InputMessage m)
    {
        activeMessage = m;
    }

    public virtual void SetCharacter(Character c)
    {
        characterReference = c;
    }

    public void Flush()
    {
        activeMessage = null;
    }
}


// The starting, idle state
public class NeutralState : PlayerState
{
    public NeutralState() : base()
    {
        stateValue = CharacterStates.Neutral;

        // Conditions
        EqualsCondition<EInput> toJumpCondition = new EqualsCondition<EInput>(EInput.Jump);
        EqualsCondition<EInput> toBlockCondition = new EqualsCondition<EInput>(EInput.Block);
        EqualsCondition<EInput> toAttackingCondition = new EqualsCondition<EInput>(EInput.Fireball);

        // Transitions
        Transition<CharacterStates> toJumpTransition = new Transition<CharacterStates>();
        toJumpTransition.SetCondition(toJumpCondition);
        Transition<CharacterStates> toParryTransition = new Transition<CharacterStates>();
        toParryTransition.SetCondition(toBlockCondition);
        Transition<CharacterStates> toAttackTransition = new Transition<CharacterStates>();
        toAttackTransition.SetCondition(toAttackingCondition);

        transitions.Add(CharacterStates.Attacking,toAttackTransition);
        transitions.Add(CharacterStates.Jumping, toJumpTransition);
        transitions.Add(CharacterStates.Parrying, toParryTransition);
    }

    protected override void OnEnter()
    {
        characterReference.ResetHeight();
        Flush();
    }

    protected override void OnExit()
   {
        if (activeMessage != null)
        {
            activeMessage.consumed = true;
        }
        Flush();
    }

    protected override void OnUpdate()
    {
        if (activeMessage == null)
        {
            return;
        }

        foreach (KeyValuePair<CharacterStates,Transition<CharacterStates>> kvp in transitions) {
            EqualsCondition<EInput> condition = (EqualsCondition<EInput>)kvp.Value.GetCondition();
            condition.SetValue(activeMessage.actionType);
        }
    }
}

// The attacking state
public class AttackingState : PlayerState
{
    float timer = 0;
    GreaterThanCondition<float> toNeutral;
    public AttackingState() : base() 
    {
        stateValue = CharacterStates.Attacking;
    }

    public override void SetCharacter(Character c)
    {
        base.SetCharacter(c);
        float attackDuration = c.GetAttackDuration();
        toNeutral = new GreaterThanCondition<float>(attackDuration);
        Transition<CharacterStates> toNeutralTransition = new Transition<CharacterStates>();
        toNeutralTransition.SetCondition(toNeutral);
        transitions.Add(CharacterStates.Neutral, toNeutralTransition);
    }

    protected override void OnEnter()
    {
        Flush();
        timer = 0;
        toNeutral.SetValue(timer);
        characterReference.ThrowFireball();
    }

    protected override void OnExit()
    {
        Flush();
    }

    protected override void OnUpdate()
    {
        timer += Time.deltaTime;
        toNeutral.SetValue(timer);
    }
}

// The state reflecting the character having their guard up but after the parrying window is over
public class BlockingState : PlayerState
{
    EqualsCondition<EInput> inputCondition;
    EqualsCondition<bool> releaseCondition;
    public BlockingState() : base()
    {
        stateValue = CharacterStates.Blocking;
        inputCondition = new EqualsCondition<EInput>(EInput.Block);
        releaseCondition = new EqualsCondition<bool>(true);
        AndCondition condition = new AndCondition(inputCondition, releaseCondition);
        Transition<CharacterStates> backToNeutral = new Transition<CharacterStates>();
        backToNeutral.SetCondition(condition);

        transitions.Add(CharacterStates.Neutral, backToNeutral);
    }
    protected override void OnEnter()
    {
        Flush();
    }

    protected override void OnExit()
    {
        activeMessage.consumed = true;
        Flush();
    }

    protected override void OnUpdate()
    {
        if (activeMessage == null) {
            return;
        }

        inputCondition.SetValue(activeMessage.actionType);
        releaseCondition.SetValue(activeMessage.isRelease);
    }
}

// The state reflecting the parrying window being active
public class ParryingState : PlayerState
{
    EqualsCondition<EInput> inputCondition;
    EqualsCondition<bool> releaseCondition;
    GreaterThanCondition<float> greaterThanCondition;
    float timer = 0;
    public ParryingState() : base()
    {
        // Conditions
        stateValue = CharacterStates.Parrying;
        inputCondition = new EqualsCondition<EInput>(EInput.Block);
        releaseCondition = new EqualsCondition<bool>(true);
        AndCondition andCond = new AndCondition(inputCondition, releaseCondition);
        

        // Transitions
        Transition<CharacterStates> toNeutral = new Transition<CharacterStates>();
        toNeutral.SetCondition(andCond);
        transitions.Add(CharacterStates.Neutral, toNeutral);
    }

    public override void SetCharacter(Character c)
    {
        base.SetCharacter(c);
        greaterThanCondition = new GreaterThanCondition<float>(c.GetParryWindow());
        Transition<CharacterStates> toBlocking = new Transition<CharacterStates>();
        toBlocking.SetCondition(greaterThanCondition);
        transitions.Add(CharacterStates.Blocking, toBlocking);
    }

    protected override void OnEnter()
    {
        // Play some animation

        // Play some sound?
    }

    protected override void OnExit()
    {
        if (activeMessage != null)
        {
            activeMessage.consumed = true;
        }
        Flush();
    }

    protected override void OnUpdate()
    {
        timer += Time.deltaTime;
        greaterThanCondition.SetValue(timer);

        // If no input is being processed return
        if (activeMessage == null) {
            return;
        }

        inputCondition.SetValue(activeMessage.actionType);
        releaseCondition.SetValue(activeMessage.isRelease);
    }
}

// The state reflecting the character jumping
// Can be interrupted by down jumping
public class JumpingState : PlayerState
{
    EqualsCondition<bool> groundCondition;
    EqualsCondition<EInput> jumpCondition;

    float timer;

    public JumpingState() : base()
    {
        stateValue = CharacterStates.Jumping;
        groundCondition = new EqualsCondition<bool>(true);
        jumpCondition = new EqualsCondition<EInput>(EInput.Jump);

        Transition<CharacterStates> toNeutral = new Transition<CharacterStates>();
        toNeutral.SetCondition(groundCondition);
        Transition<CharacterStates> toDownJumping = new Transition<CharacterStates>();
        toDownJumping.SetCondition(jumpCondition);

        transitions.Add(CharacterStates.Neutral, toNeutral);
        transitions.Add(CharacterStates.DownJumping, toDownJumping);

    }
    protected override void OnEnter()
    {
        timer = 0;

        // Call Animation

        // Play sound
    }

    protected override void OnExit()
    {
        if (activeMessage != null)
        {
            activeMessage.consumed = true;
        }
        Flush();

        // Create landing particle system on grounded
    }

    protected override void OnUpdate()
    {
        // Update the characters vertical position
        timer += Time.deltaTime;
        characterReference.HandleJumpUpdate(timer);

        // Check for grounded
        groundCondition.SetValue(characterReference.IsGrounded);

        // If down jumping called, execute it
        if (activeMessage == null) {
            return;
        }
        jumpCondition.SetValue(activeMessage.actionType);

    }
}

// The state indicating the player is trying to fall faster
public class DownJumpingState : PlayerState
{
    EqualsCondition<bool> groundCondition;
    float timer;
    public DownJumpingState(): base()
    {
        stateValue = CharacterStates.DownJumping;
        groundCondition = new EqualsCondition<bool>(true);
        Transition<CharacterStates> transition = new Transition<CharacterStates>();
        transition.SetCondition(groundCondition);
        transitions.Add(CharacterStates.Neutral, transition);
    }
    protected override void OnEnter()
    {
        timer = 0;
    }

    protected override void OnExit()
    {
        // Create a particle system to show landing
    }

    protected override void OnUpdate()
    {
        timer += Time.deltaTime;
        characterReference.HandleDownJumpUpdate(timer);
        groundCondition.SetValue(characterReference.IsGrounded);
    }
}