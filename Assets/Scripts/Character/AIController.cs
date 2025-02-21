using UnityEngine;

/// <summary>
/// Wrapper for the AI Strategy to allow for CPU controlled characters
/// </summary>
public class AIController : MonoBehaviour
{
    AbsAIStrategy strategy;
    void Update()
    {
        strategy.OnUpdate();
    }

    // Sets the acting strategy for this AI player
    public void SetStrategy(AIControlTypes controlType)
    {
        strategy = StrategyFactory.MakeStrategy(controlType);
        strategy.SetCharacter(GetComponent<Character>());
    }
}
