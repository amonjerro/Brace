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
    [Tooltip("Which player this interface sets up")]
    int PlayerIndex;

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
    [Tooltip("The game's character roster")]
    RosterSO roster;

    [SerializeField]
    DebugInputBuffer inputDebugger;

    [SerializeField]
    [Tooltip("This variable should be turned on when doing testing within the fight scene and turned off for release")]
    bool SceneTesting;

    [SerializeField]
    [Tooltip("When testing, this informs this character select which characters to load")]
    int CharacterIndex;


    // On start, this interface sets the status of the players based on incoming information
    // from the game instance

    // This can be reworked to easily create a demo-mode that runs from the main menu
    void Start()
    {
        BattleManager bm = ServiceLocator.Instance.GetService<BattleManager>();
        Character createdCharacter = Instantiate(CharacterPrefab, SpawnLocation, Quaternion.identity);
        
        // Debug and development logic
        if (SceneTesting)
        {
            createdCharacter.SetCharacterData(roster.roster[CharacterIndex]);
        } else
        {
            createdCharacter.SetCharacterData(roster.roster[GameInstance.GetCharacterByPlayerIndex(PlayerIndex)]);
        }
        
        
        if (controlType == ControlTypes.Player)
        {
            // Player setup
            PlayerInput input = createdCharacter.gameObject.AddComponent<PlayerInput>();
            input.actions = defaultActionAsset;
            createdCharacter.SetInputToGame();
        } else
        {
            // AI setup
            AIController controller = createdCharacter.gameObject.AddComponent<AIController>();
            controller.SetStrategy(aiType);
        }

        // Configure the state for characters
        createdCharacter.SetHealthBarController(playerHealthBar);
        createdCharacter.SetInputBufferDebugger(inputDebugger);
        createdCharacter.SetIndex(PlayerIndex);
        bm.RegisterCharacter(createdCharacter);
    }
}
