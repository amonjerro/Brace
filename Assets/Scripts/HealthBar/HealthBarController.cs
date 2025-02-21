using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

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
}
