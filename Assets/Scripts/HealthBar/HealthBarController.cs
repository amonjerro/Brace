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
        if (DesiredValue < CurrentValue){
            CurrentValue -= ValueChangeRate * Time.deltaTime;
        }
        healthBarRenderer.material.SetFloat("_HealthRemaining", CurrentValue);
    }

    public void UpdateHealth(float value)
    {
        DesiredValue = value;
    }

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
