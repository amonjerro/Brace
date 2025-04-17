using UnityEngine;
using UnityEngine.UI;
public enum CooldownType
{
    Block,
    Jump,
    Shoot
}


/// <summary>
/// Class that shows the health bars at the top of the screen. Currently does not work as a Canvas object and likely should.
/// </summary>
public class HealthBarController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The sprite renderer for the life bar")]
    Image healthBarRenderer;

    [SerializeField]
    [Tooltip("The sprite renderer for the jump action button")]
    Image jumpActionSpriteRenderer;
    
    [SerializeField]
    [Tooltip("The sprite renderer for the attack action button")]
    Image shootActionSpriteRenderer;
    
    [SerializeField]
    [Tooltip("The sprite renderer for the block action button")]
    Image blockActionSpriteRenderer;

    [SerializeField]
    [Range(0f, 0.5f)]
    [Tooltip("How fast health updates over time")]
    float ValueChangeRate;

    // Internals
    float CurrentValue;
    float DesiredValue;

    #region UNITY_LIFECYCLE
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
    #endregion


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
