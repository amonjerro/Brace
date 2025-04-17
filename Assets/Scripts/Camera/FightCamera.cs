using UnityEngine;

public class FightCamera : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Y direction shake sensitivity. Higher numbers mean bigger motion through the Y axis")]
    float YSensitivity;

    [SerializeField]
    [Tooltip("X direction shake sensitivity. Higher numbers mean bigger motion through the X axis")]
    float XSensitivity;

    [SerializeField]
    [Tooltip("How fast to cycle through noise values")]
    float cameraShakeSpeed;

    Vector3 originalPosition;
    float cameraShakeDuration;
    float shakeTimer;
    float noiseTimer;
    float ySample;
    float xSample;
    bool bShake;

    private void Awake()
    {
        bShake = false;
        shakeTimer = 0.0f;
        noiseTimer = 0.0f;
        ySample = Random.Range(0, 10);
        xSample = Random.Range(0, 10);
        originalPosition = transform.position;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraShakeDuration = ServiceLocator.Instance.GetService<BattleManager>().HitstopTime;
    }

    // Update is called once per frame
    void Update()
    {
        HandleShake();
    }

    public void InitiateShake()
    {
        shakeTimer = 0.0f;
        noiseTimer = 0.0f;
        bShake = true;

    }

    private void HandleShake()
    {
        if (!bShake)
        {
            return;
        }

        shakeTimer += Time.deltaTime;
        noiseTimer += Time.deltaTime;

        // Shaking time over
        if (shakeTimer > cameraShakeDuration) {
            xSample = Random.Range(0, 10);
            ySample = Random.Range(0, 10);
            transform.position = originalPosition;
            bShake = false;
            return;
        }

        float xChange = XSensitivity * (2 * (Mathf.PerlinNoise1D(xSample + noiseTimer * cameraShakeSpeed) - 1.0f));
        float yChange = YSensitivity * (2 * (Mathf.PerlinNoise1D(ySample + noiseTimer * cameraShakeSpeed) - 1.0f));

        transform.position = new Vector3(xChange + originalPosition.x, yChange + originalPosition.y, originalPosition.z);
    }
}
