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
    void Start()
    {
        BattleManager bm = ServiceLocator.Instance.GetService<BattleManager>();
        Character createdCharacter = Instantiate(CharacterPrefab, SpawnLocation, Quaternion.identity);
        if (SceneTesting)
        {
            createdCharacter.SetCharacterData(roster.roster[CharacterIndex]);
        } else
        {
            createdCharacter.SetCharacterData(roster.roster[GameInstance.GetCharacterByPlayerIndex(PlayerIndex)]);
        }
        

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
        bm.RegisterCharacter(createdCharacter);

    }
}
