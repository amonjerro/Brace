using UnityEngine;
using UnityEngine.InputSystem;

public enum ControlTypes
{
    Player,
    AI
}


public class CharacterInterface : MonoBehaviour
{
    [SerializeField]
    Vector3 SpawnLocation;

    [SerializeField]
    ControlTypes controlType;

    [SerializeField]
    AIControlTypes aiType;

    [SerializeField]
    Character CharacterPrefab;

    [SerializeField]
    InputActionAsset defaultActionAsset;

    [SerializeField]
    HealthBarController playerHealthBar;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Character createdCharacter = Instantiate(CharacterPrefab, SpawnLocation, Quaternion.identity);
        if (controlType == ControlTypes.Player)
        {
            PlayerInput input = createdCharacter.gameObject.AddComponent<PlayerInput>();
            input.actions = defaultActionAsset;
        } else
        {
            AIController controller = createdCharacter.gameObject.AddComponent<AIController>();
            controller.SetStrategy(aiType);
        }
        createdCharacter.SetHealthBarController(playerHealthBar);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
