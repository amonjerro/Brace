using UnityEngine;

public enum CharacterCursorStates
{
    None,
    Selecting,
    Ready
}

public class CharacterCursor : MonoBehaviour
{
    StateMachine<CharacterCursorStates> stateMachine;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {

    }

    public void UpdatePosition(Vector2 positionShift)
    {

    }
}
