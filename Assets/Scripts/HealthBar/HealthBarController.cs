using UnityEngine;
public enum CooldownType
{
    Block,
    Jump,
    Shoot
}

public class HealthBarController : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer healthBarRenderer;
    [SerializeField]
    SpriteRenderer jumpActionSpriteRenderer;
    [SerializeField]
    SpriteRenderer shootActionSpriteRenderer;
    [SerializeField]
    SpriteRenderer blockActionSpriteRenderer;

    [SerializeField]
    [Range(0f, 0.5f)]
    float ValueChangeRate;

    float CurrentValue;
    float DesiredValue;

    private void Awake()
    {
        CurrentValue = 1f;
        DesiredValue = 1f;
    }


    public void Update()
    {
        if (DesiredValue > CurrentValue)
        {
            CurrentValue += 2 * ValueChangeRate * Time.deltaTime;
        }
        if (DesiredValue < CurrentValue){
            CurrentValue -= ValueChangeRate * Time.deltaTime;
        }
        healthBarRenderer.material.SetFloat("_HealthRemaining", CurrentValue);
    }

    // Updates the desired value of the health to allow for gradual decrease of health in the UI
    public void UpdateHealth(float value)
    {
        DesiredValue = value;
    }


    /// <summary>
    /// Updates the cooldown user feedback spheres
    /// </summary>
    /// <param name="type">The cooldown sphere to update</param>
    /// <param name="value">The current angle of the arc for the timer. Must be in radians</param>
    public void UpdateCooldown(CooldownType type, float value)
    {
        switch(type) {
            case CooldownType.Jump:
                jumpActionSpriteRenderer.material.SetFloat("_Arc", value);
                break;
            case CooldownType.Shoot:
                shootActionSpriteRenderer.material.SetFloat("_Arc", value);
                break;
            case CooldownType.Block:
                blockActionSpriteRenderer.material.SetFloat("_Arc", value);
                break;
        }
    }
}
