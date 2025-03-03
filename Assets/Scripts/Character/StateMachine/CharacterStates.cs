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
        transitions = new List<Transition<CharacterStates>>();
    }

    public void SetMessage(InputMessage m)
    {
        activeMessage = m;
    }

    public void SetCharacter(Character c)
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
        Transition<CharacterStates> toBlockTransition = new Transition<CharacterStates>();
        toBlockTransition.SetCondition(toBlockCondition);
        Transition<CharacterStates> toAttackTransition = new Transition<CharacterStates>();
        toAttackTransition.SetCondition(toAttackingCondition);

        transitions.Add(toAttackTransition);
        transitions.Add(toJumpTransition);
        transitions.Add(toBlockTransition);
    }

    protected override void OnEnter()
    {
        characterReference.IsGrounded = true;
        Flush();
    }

    protected override void OnExit()
    {
        Flush();
    }

    protected override void OnUpdate()
    {
        if (activeMessage == null)
        {
            return;
        }

        foreach (Transition<CharacterStates> transition in transitions) {
            EqualsCondition<EInput> condition = (EqualsCondition<EInput>)transition.GetCondition();
            condition.SetValue(activeMessage.actionType);
        }

        Flush();
    }
}

// The attacking state
public class AttackingState : PlayerState
{
    float timer = 0;
    WithinRangeCondition<float> toNeutral;
    public AttackingState(float min, float max) : base() 
    {
        stateValue = CharacterStates.Attacking;

        toNeutral = new WithinRangeCondition<float>(min, max);
        Transition<CharacterStates> toNeutralTransition = new Transition<CharacterStates>();
        toNeutralTransition.SetCondition(toNeutral);

        transitions.Add(toNeutralTransition);
    }

    protected override void OnEnter()
    {
        Flush();
        timer = 0;
        toNeutral.SetValue(timer);
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

        transitions.Add(backToNeutral);
    }
    protected override void OnEnter()
    {
        Flush();
    }

    protected override void OnExit()
    {
        Flush();
    }

    protected override void OnUpdate()
    {
        if (activeMessage == null) {
            return;
        }

        inputCondition.SetValue(activeMessage.actionType);
        releaseCondition.SetValue(activeMessage.isRelease);

        Flush();
    }
}

// The state reflecting the parrying window being active
public class ParryingState : PlayerState
{
    EqualsCondition<EInput> inputCondition;
    EqualsCondition<bool> releaseCondition;
    WithinRangeCondition<float> withinRangeCondition;
    float timer = 0;
    public ParryingState(float parryWindow) : base()
    {
        // Conditions
        stateValue = CharacterStates.Parrying;
        inputCondition = new EqualsCondition<EInput>(EInput.Block);
        releaseCondition = new EqualsCondition<bool>(true);
        AndCondition andCond = new AndCondition(inputCondition, releaseCondition);
        withinRangeCondition = new WithinRangeCondition<float>(parryWindow - 0.01f, parryWindow + 0.01f);

        // Transitions
        Transition<CharacterStates> toNeutral = new Transition<CharacterStates>();
        toNeutral.SetCondition(andCond);
        Transition<CharacterStates> toBlocking = new Transition<CharacterStates>();
        toBlocking.SetCondition(withinRangeCondition);

        transitions.Add(toNeutral);
        transitions.Add(toBlocking);
        
    }
    protected override void OnEnter()
    {

    }

    protected override void OnExit()
    {

    }

    protected override void OnUpdate()
    {
        timer += Time.deltaTime;
        withinRangeCondition.SetValue(timer);

        // If no input is being processed return
        if (activeMessage == null) {
            return;
        }

        inputCondition.SetValue(activeMessage.actionType);
        releaseCondition.SetValue(activeMessage.isRelease);
        
        Flush();
    }
}

// The state reflecting the character jumping
// Can be interrupted by down jumping
public class JumpingState : PlayerState
{
    EqualsCondition<bool> groundCondition;
    EqualsCondition<EInput> jumpCondition;
    public JumpingState() : base()
    {
        stateValue = CharacterStates.Jumping;
        groundCondition = new EqualsCondition<bool>(true);
        jumpCondition = new EqualsCondition<EInput>(EInput.Jump);

        Transition<CharacterStates> toNeutral = new Transition<CharacterStates>();
        toNeutral.SetCondition(groundCondition);
        Transition<CharacterStates> toDownJumping = new Transition<CharacterStates>();
        toDownJumping.SetCondition(jumpCondition);

        transitions.Add(toNeutral);
        transitions.Add(toDownJumping);

    }
    protected override void OnEnter()
    {

    }

    protected override void OnExit()
    {

    }

    protected override void OnUpdate()
    {
        if (activeMessage == null) {
            return;
        }

        groundCondition.SetValue(characterReference.IsGrounded);
        jumpCondition.SetValue(activeMessage.actionType);

        Flush();
    }
}

// The state indicating the player is trying to fall faster
public class DownJumpingState : PlayerState
{
    EqualsCondition<bool> groundCondition;
    public DownJumpingState(): base()
    {
        stateValue = CharacterStates.DownJumping;
        groundCondition = new EqualsCondition<bool>(true);
        Transition<CharacterStates> transition = new Transition<CharacterStates>();
        transition.SetCondition(groundCondition);
        transitions.Add(transition);
    }
    protected override void OnEnter()
    {

    }

    protected override void OnExit()
    {

    }

    protected override void OnUpdate()
    {
        groundCondition.SetValue(characterReference.IsGrounded);
    }
}