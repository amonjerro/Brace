
using GameMenus;
using InputManagement;


/// <summary>
/// Abstract player cursor state
/// Processes player input and handles how that processing changes based on context
/// </summary>
public abstract class CursorState : AbsState<CharacterCursorStates> {
    protected CharacterCursor cursor;
    protected InputMessage inputMessage;
    
    /// <summary>
    /// State machine requires a reference to the owning object
    /// </summary>
    /// <param name="cursor"></param>
    public CursorState(CharacterCursor cursor)
    {
        this.cursor = cursor;
        transitions = new System.Collections.Generic.Dictionary<CharacterCursorStates, Transition<CharacterCursorStates>>();
    }

    /// <summary>
    /// Set the input message to evaluate
    /// </summary>
    /// <param name="m"></param>
    public void SetMessage(InputMessage m) {
        inputMessage = m;
    }


    /// <summary>
    /// Resets the state to prevent sticky states
    /// </summary>
    protected void Flush()
    {
        inputMessage = null;
    }

}

/// <summary>
/// State that handles the cursor behavior for when the player is actively choosing a character
/// </summary>
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
        (int, int) inputVector = CharacterSelect.InputToVector(inputMessage.actionType);
        (int, int) currentPosition = cursor.GetCurrentPosition();
        (int, int) newPosition = (inputVector.Item1 + currentPosition.Item1, inputVector.Item2 + currentPosition.Item2);
        if (newPosition.Item1 < -1) { 
            newPosition.Item1 = 1;
        }
        if (newPosition.Item2 < -1)
        {
            newPosition.Item2 = 1;
        }
        if (newPosition.Item1 > 1)
        {
            newPosition.Item1 = -1;
        }
        if (newPosition.Item2 > 1)
        {
            newPosition.Item2 = -1;
        }
        cursor.UpdatePosition(newPosition);
        Flush();

    }

    protected override void OnEnter()
    {
        cursor.SetReadyUp(false);
        readyInput.Reset();
    }

    protected override void OnExit()
    {
    }
}

/// <summary>
/// State that handles the player being ready or backing out of ready status
/// </summary>
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
        cursor.SetReadyUp(true);
        choosingInput.Reset();
    }

    protected override void OnExit()
    {
        Flush();
    }
}