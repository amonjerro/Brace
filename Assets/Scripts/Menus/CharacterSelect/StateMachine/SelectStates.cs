
using GameMenus;
using InputManagement;

public abstract class CursorState : AbsState<CharacterCursorStates> {
    protected CharacterCursor cursor;
    protected InputMessage inputMessage;

    public CursorState(CharacterCursor cursor)
    {
        this.cursor = cursor;
    }

    public void SetMessage(InputMessage m) {
        inputMessage = m;
    }

    protected void Flush()
    {
        inputMessage = null;
    }

}

public class ChoosingState : CursorState
{
    EqualsCondition<EInput> readyInput;

    public ChoosingState(CharacterCursor cursor) : base(cursor) { 
        Transition<CharacterCursorStates> toReadyTransition = new Transition<CharacterCursorStates>();
        readyInput = new EqualsCondition<EInput>(EInput.Fireball);
        toReadyTransition.SetCondition(readyInput);
        transitions.Add(CharacterCursorStates.Ready,toReadyTransition);
    
    }

    protected override void OnUpdate()
    {
        if (inputMessage == null)
        {
            return;
        }

        readyInput.SetValue(inputMessage.actionType);
        cursor.UpdatePosition(CharacterSelect.InputToVector(inputMessage.actionType));
    }

    protected override void OnEnter()
    {
        readyInput.Reset();
    }

    protected override void OnExit()
    {
        Flush();
    }
}

public class ReadyState : CursorState
{
    EqualsCondition<EInput> choosingInput;
    public ReadyState(CharacterCursor cursor) : base(cursor) { 
        Transition<CharacterCursorStates> toChoosingTransition = new Transition<CharacterCursorStates>();
        choosingInput = new EqualsCondition<EInput>(EInput.Jump);
        toChoosingTransition.SetCondition(choosingInput);
        transitions.Add(CharacterCursorStates.Selecting, toChoosingTransition);
    }

    protected override void OnUpdate()
    {
        if (inputMessage == null)
        {
            return;
        }
        choosingInput.SetValue(inputMessage.actionType);
    }

    protected override void OnEnter()
    {
        choosingInput.Reset();
    }

    protected override void OnExit()
    {
        Flush();
    }
}