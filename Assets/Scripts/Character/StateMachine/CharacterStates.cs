using System.Collections.Generic;
using UnityEngine;
using InputManagement;
using Unity.VisualScripting;

/// <summary>
/// Player states require a little more information than just the abstract state
/// They need to be informed of the inputs the player is pressing in order to transition the player into new states
/// </summary>
public abstract class PlayerState : AbsState<CharacterStates>{
    protected InputMessage activeMessage;
    protected Character characterReference;
    public PlayerState(StateMachine<CharacterStates> stateMachine)
    {
        machine = stateMachine;
        activeMessage = null;
        transitions = new Dictionary<CharacterStates, Transition<CharacterStates>>();
    }

    /// <summary>
    /// Sets the value of the input message to see if any input transitions have been triggered
    /// </summary>
    /// <param name="m">The InputMessage object</param>
    public void SetMessage(InputMessage m)
    {
        activeMessage = m;
    }

    /// <summary>
    /// Sets a reference to the character who owns this state machine and its corresponding states
    /// </summary>
    /// <param name="c"></param>
    public virtual void SetCharacter(Character c)
    {
        characterReference = c;
    }

    /// <summary>
    /// Clean up all transitions to prevent sticky states on reuse.
    /// </summary>
    public void Flush()
    {
        activeMessage = null;
        foreach(Transition<CharacterStates> transition in transitions.Values)
        {
            transition.ResetCondition();
        }
    }
}   


// The starting, idle state
public class NeutralState : PlayerState
{
    EqualsCondition<bool> isPress;
    EqualsCondition<EInput> toBlockCondition;
    public NeutralState(StateMachine<CharacterStates> s) : base(s)
    {
        stateValue = CharacterStates.Neutral;

        // Conditions
        EqualsCondition<EInput> toJumpCondition = new EqualsCondition<EInput>(EInput.Jump);
        toBlockCondition = new EqualsCondition<EInput>(EInput.Block);
        isPress = new EqualsCondition<bool>(true);
        AndCondition blockComposite = new AndCondition(toBlockCondition, isPress);
        EqualsCondition<EInput> toAttackingCondition = new EqualsCondition<EInput>(EInput.Fireball);

        // Transitions
        Transition<CharacterStates> toJumpTransition = new Transition<CharacterStates>();
        toJumpTransition.SetCondition(toJumpCondition);
        Transition<CharacterStates> toParryTransition = new Transition<CharacterStates>();
        toParryTransition.SetCondition(blockComposite);
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


        if (transitions[CharacterStates.Parrying].GetCondition().Test())
        {
            machine.StackState(this);
        }
        Flush();
    }

    protected override void OnUpdate()
    {
        if (activeMessage == null)
        {
            return;
        }

        isPress.SetValue(!activeMessage.isRelease);

        // Not a fan of a O(n) check happening every frame.
        foreach (KeyValuePair<CharacterStates,Transition<CharacterStates>> kvp in transitions) {
            if (kvp.Key == CharacterStates.Parrying)
            {
                toBlockCondition.SetValue(activeMessage.actionType);
                continue;
            }
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
    public AttackingState(StateMachine<CharacterStates> s) : base(s) 
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
        timer += TimeUtil.GetDelta();
        toNeutral.SetValue(timer);
    }
}

// The state reflecting the character having their guard up but after the parrying window is over
public class BlockingState : PlayerState
{
    EqualsCondition<EInput> inputCondition;
    EqualsCondition<bool> releaseCondition;
    public BlockingState(StateMachine<CharacterStates> s) : base(s)
    {
        stateValue = CharacterStates.Blocking;
        inputCondition = new EqualsCondition<EInput>(EInput.Block);
        releaseCondition = new EqualsCondition<bool>(true);
        AndCondition condition = new AndCondition(inputCondition, releaseCondition);
        Transition<CharacterStates> backTransition = new Transition<CharacterStates>();
        backTransition.SetCondition(condition);

        transitions.Add(CharacterStates.Neutral, backTransition);
    }
    protected override void OnEnter()
    {
        characterReference.EnableBlock();
        characterReference.SetBlockToParry(false);
    }

    protected override void OnExit()
    {
        if (activeMessage != null)
        {
            activeMessage.consumed = true;
        }
        characterReference.DisableBlock();
        characterReference.SetCooldown(CooldownType.Block);
        transitions[CharacterStates.Neutral].TargetState = machine.UnstackState();
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
    GreaterThanCondition<float> parryTimeCondition;
    float timer = 0;
    public ParryingState(StateMachine<CharacterStates> s) : base(s)
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
        parryTimeCondition = new GreaterThanCondition<float>(c.GetParryWindow());
        Transition<CharacterStates> toBlocking = new Transition<CharacterStates>();
        toBlocking.SetCondition(parryTimeCondition);
        transitions.Add(CharacterStates.Blocking, toBlocking);
    }

    protected override void OnEnter()
    {
        // Play some animation
        characterReference.EnableBlock();
        characterReference.SetBlockToParry(true);

        // Play some sound?
    }

    protected override void OnExit()
    {
        if (releaseCondition.Test())
        {
            // Block has been released. Return to neutral state
            transitions[CharacterStates.Neutral].TargetState = machine.UnstackState();
            characterReference.SetBlockToParry(false);
            characterReference.DisableBlock();
            characterReference.SetCooldown(CooldownType.Block);
        }

        if (activeMessage != null)
        {
            activeMessage.consumed = true;
        }
        timer = 0;
        Flush();
    }

    protected override void OnUpdate()
    {
        timer += TimeUtil.GetDelta();
        parryTimeCondition.SetValue(timer);

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
    GreaterThanCondition<float> heightRequirement;
    EqualsCondition<EInput> jumpCondition;
    EqualsCondition<EInput> attackCondition;

    float timer;

    public JumpingState(StateMachine<CharacterStates> s) : base(s)
    {
        stateValue = CharacterStates.Jumping;
        groundCondition = new EqualsCondition<bool>(true);
        jumpCondition = new EqualsCondition<EInput>(EInput.Jump);
        attackCondition = new EqualsCondition<EInput>(EInput.Fireball);
        heightRequirement = new GreaterThanCondition<float>(-1.0f);
        AndCondition andCondition = new AndCondition(attackCondition, heightRequirement);

        Transition<CharacterStates> toNeutral = new Transition<CharacterStates>();
        toNeutral.SetCondition(groundCondition);
        Transition<CharacterStates> toDownJumping = new Transition<CharacterStates>();
        toDownJumping.SetCondition(jumpCondition);
        Transition<CharacterStates> toJumpAttack = new Transition<CharacterStates>();
        toJumpAttack.SetCondition(andCondition);

        transitions.Add(CharacterStates.Neutral, toNeutral);
        transitions.Add(CharacterStates.DownJumping, toDownJumping);
        transitions.Add(CharacterStates.JumpAttack, toJumpAttack);
    }
    protected override void OnEnter()
    {
        timer = 0;

        // Call Animation

        // Play sound
    }

    protected override void OnExit()
    {
        // Transitioning to other mid-air jump states
        if (jumpCondition.Test() || attackCondition.Test())
        {
            activeMessage.consumed = true;
            Flush();
            return;
        }

        Flush();
        // Create landing particle system on grounded
        if (characterReference.IsGrounded)
        {
            characterReference.TriggerLandingAnimation();
        }
    }

    protected override void OnUpdate()
    {
        // Update the characters vertical position
        timer += TimeUtil.GetDelta();
        characterReference.HandleJumpUpdate(timer);

        // Check for grounded
        groundCondition.SetValue(characterReference.IsGrounded);
        heightRequirement.SetValue(characterReference.transform.position.y);


        // If down jumping called, execute it
        if (activeMessage == null) {
            return;
        }
        jumpCondition.SetValue(activeMessage.actionType);
        attackCondition.SetValue(activeMessage.actionType);

    }
}

// The state indicating the player is trying to fall faster
public class DownJumpingState : PlayerState
{
    EqualsCondition<bool> groundCondition;
    float timer;
    public DownJumpingState(StateMachine<CharacterStates> s) : base(s)
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
        // Trigger the jump cooldown
        characterReference.SetCooldown(CooldownType.Jump);
        // Create a particle system to show landing
        characterReference.TriggerLandingAnimation();
    }

    protected override void OnUpdate()
    {
        timer += TimeUtil.GetDelta();
        characterReference.HandleDownJumpUpdate(timer);
        groundCondition.SetValue(characterReference.IsGrounded);
    }
}

public class JumpAttackState : PlayerState
{
    float timer = 0;
    GreaterThanCondition<float> toJumpCondition;
    public JumpAttackState(StateMachine<CharacterStates> s) : base(s)
    {
        stateValue = CharacterStates.JumpAttack;
        
    }

    public override void SetCharacter(Character c)
    {
        base.SetCharacter(c);
        float attackDuration = c.GetAttackDuration();
        toJumpCondition = new GreaterThanCondition<float>(attackDuration);
        Transition<CharacterStates> toJumpTransition = new Transition<CharacterStates>();
        toJumpTransition.SetCondition(toJumpCondition);
        transitions.Add(CharacterStates.DownJumping, toJumpTransition);
    }

    protected override void OnEnter()
    {
        timer = 0;
        toJumpCondition.SetValue(timer);
        characterReference.ThrowFireball();
    }

    protected override void OnExit()
    {
        Flush();
        
    }

    protected override void OnUpdate()
    {
        timer += TimeUtil.GetDelta();
        toJumpCondition.SetValue(timer);
    }
}