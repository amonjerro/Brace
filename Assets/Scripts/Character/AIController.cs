using UnityEngine;

public class AIController : MonoBehaviour
{
    AbsAIStrategy strategy;

    
    void Update()
    {
        strategy.OnUpdate();
    }

    public void SetStrategy(AIControlTypes controlType)
    {
        strategy = StrategyFactory.MakeStrategy(controlType);
        strategy.SetCharacter(GetComponent<Character>());
    }
}
