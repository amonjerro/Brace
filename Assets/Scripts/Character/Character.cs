using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{ 
    [SerializeField]
    CharacterSO CharacterData;

    Vector3 forward3;
    float originalYPosition;
    BulletPool bulletPool;

    float jumpTimer = 0;
    const float HALF_PI = 0.5f * Mathf.PI;
    const float TWO_PI = 2 * Mathf.PI;
    bool bIsGrounded = true;

    private void Start()
    {
        bulletPool = FindAnyObjectByType<BulletPool>();
        forward3 = transform.right;
        originalYPosition = transform.position.y;
    }

    private void Update()
    {
        if (bIsGrounded)
        {
            return;
        }

        jumpTimer += Time.deltaTime;
        transform.position = new Vector3(transform.position.x, ParabolicPosition(), transform.position.z);

        if (jumpTimer > CharacterData.characterParameters.jumpDuration)
        {
            bIsGrounded = true;
            transform.position = new Vector3(transform.position.x, originalYPosition, transform.position.z);
        }
    }

    float ParabolicPosition()
    {
        float t = jumpTimer / CharacterData.characterParameters.jumpDuration;
        return CharacterData.characterParameters.jumpHeight * ( -4 * t * t + 4 * t) + originalYPosition;
    }

    // Handle Player Input
    private void OnAttack(InputValue input)
    {
        Bullet b = bulletPool.GetBullet();
        b.transform.position = transform.position + forward3;
        b.Launch(CharacterData.characterParameters.bulletParams, forward3);
    }

    private void OnJump(InputValue input)
    {
        if (bIsGrounded) { 
            bIsGrounded = false;
            jumpTimer = 0;
        }
    }
}
