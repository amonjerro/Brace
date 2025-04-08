using UnityEngine;
using UnityEngine.InputSystem;

public enum ControlTypes
{
    Player,
    AI
}

// Sets up characters based on existing configuration
public class CharacterInterface : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Where will this player spawn")]
    Vector3 SpawnLocation;

    [SerializeField]
    [Tooltip("Who will be in control of this player")]
    ControlTypes controlType;

    [SerializeField]
    [Tooltip("If AI is controlling this character, what strategy should it use?")]
    AIControlTypes aiType;

    [SerializeField]
    [Tooltip("The character prefab object")]
    Character CharacterPrefab;

    [SerializeField]
    [Tooltip("The action asset to assign for player input")]
    InputActionAsset defaultActionAsset;

    [SerializeField]
    [Tooltip("Which health bar to assign to this player")]
    HealthBarController playerHealthBar;

    [SerializeField]
    DebugInputBuffer inputDebugger;


    // On start, this interface sets the status of the players based on incoming information
    void Start()
    {
        Character createdCharacter = Instantiate(CharacterPrefab, SpawnLocation, Quaternion.identity);
        if (controlType == ControlTypes.Player)
        {
            PlayerInput input = createdCharacter.gameObject.AddComponent<PlayerInput>();
            input.actions = defaultActionAsset;
            createdCharacter.SetInputToGame();
        } else
        {
            AIController controller = createdCharacter.gameObject.AddComponent<AIController>();
            controller.SetStrategy(aiType);
        }
        createdCharacter.SetHealthBarController(playerHealthBar);
        createdCharacter.SetInputBufferDebugger(inputDebugger);


    }
}
