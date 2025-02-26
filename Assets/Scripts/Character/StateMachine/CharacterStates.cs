public class NeutralState : AbsState<CharacterStates>
{
    public NeutralState()
    {
        stateValue = CharacterStates.Neutral;
    }
    protected override void OnEnter()
    {

    }

    protected override void OnExit()
    {

    }

    protected override void OnUpdate()
    {

    }
}

public class AttackingState : AbsState<CharacterStates>
{
    public AttackingState()
    {
        stateValue = CharacterStates.Attacking;
    }

    protected override void OnEnter()
    {

    }

    protected override void OnExit()
    {

    }

    protected override void OnUpdate()
    {

    }
}

public class BlockingState : AbsState<CharacterStates>
{
    public BlockingState()
    {
        stateValue = CharacterStates.Blocking;
    }
    protected override void OnEnter()
    {

    }

    protected override void OnExit()
    {

    }

    protected override void OnUpdate()
    {

    }
}

public class ParryingState : AbsState<CharacterStates>
{
    public ParryingState()
    {
        stateValue = CharacterStates.Parrying;
    }
    protected override void OnEnter()
    {

    }

    protected override void OnExit()
    {

    }

    protected override void OnUpdate()
    {

    }
}

public class JumpingState : AbsState<CharacterStates>
{
    public JumpingState()
    {
        stateValue = CharacterStates.Jumping;
    }
    protected override void OnEnter()
    {

    }

    protected override void OnExit()
    {

    }

    protected override void OnUpdate()
    {

    }
}

public class DownJumpingState : AbsState<CharacterStates>
{
    public DownJumpingState()
    {
        stateValue = CharacterStates.DownJumping;
    }
    protected override void OnEnter()
    {

    }

    protected override void OnExit()
    {

    }

    protected override void OnUpdate()
    {

    }
}