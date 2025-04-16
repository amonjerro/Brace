using UnityEngine;
using GameMenus;
using UnityEngine.InputSystem;

public enum CharacterCursorStates
{
    None,
    Selecting,
    Ready
}

public class CharacterCursor : MonoBehaviour
{

    CharacterSelect characterSelect;
    bool initialized = false;
    (int, int) CurrentPosition;
    StateMachine<CharacterCursorStates> stateMachine;
    int PlayerId;

    private void Start()
    {
        PlayerId = -1;
        Initialize();
    }



    private void Initialize()
    {
        CurrentPosition = (0, 0);

        // Character Select referencing
        GameObject go = GameObject.Find("CharacterSelectMenu");
        characterSelect = go.GetComponent<CharacterSelect>();
        transform.SetParent(characterSelect.transform, false);
        characterSelect.HandleCursorSpawn(this);
        PlayerInput pi = gameObject.GetComponent<PlayerInput>();
        pi.SwitchCurrentActionMap("UI");
        PlayerId = characterSelect.GetPlayerIndex(this);


        // StateMachine stuff
        stateMachine = new StateMachine<CharacterCursorStates>();
        ChoosingState choosing = new ChoosingState(this);
        ReadyState ready = new ReadyState(this);
        choosing.transitions[CharacterCursorStates.Ready].TargetState = ready;
        ready.transitions[CharacterCursorStates.Selecting].TargetState = choosing;
        stateMachine.SetStartingState(choosing);


        initialized = true;

    }

    private void Update()
    {
        stateMachine.Update();
    }

    public (int, int) GetCurrentPosition() {  return CurrentPosition; }

    public void UpdatePosition((int, int) newPosition)
    {
        CurrentPosition = newPosition;
        characterSelect.UpdateCursorImageLocation(this,newPosition);
    }

    public void SetReadyUp(bool readyStatus){
        characterSelect.SetPlayerReadyStatus(PlayerId, readyStatus);
    }


    private void OnNavigate(InputValue inputValue)
    {
        if (!initialized)
        {
            return;
        }

        Vector2 v = inputValue.Get<Vector2>();
        if (v.magnitude < 1 )
        {
            return;
        }
        CursorState state = (CursorState)stateMachine.CurrentState;
        state.SetMessage(new InputManagement.InputMessage(CharacterSelect.VectorToInput(v.x, v.y)));
    }

    private void OnSubmit(InputValue inputValue)
    {
        if (!initialized)
        {
            return;
        }

        CursorState state = (CursorState)stateMachine.CurrentState;
        state.SetMessage(new InputManagement.InputMessage(InputManagement.EInput.Fireball));
    }

}
