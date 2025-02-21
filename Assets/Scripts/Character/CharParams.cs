using System;
using UnityEngine;

[Serializable]
public struct CharParams
{
    // Durations and Timings
    public float jumpDuration;
    public float jumpHeight;
    public float shotCooldown;
    public float blockCooldown;
    public float parryWindow;

    // Graphic config
    public Sprite sprite;

    // Projectile parameters
    public BulletParams bulletParams;

}

[Serializable]
public struct BulletParams
{
    public float moveSpeed;
    public Sprite bulletSprite;
    public float bulletDamage;
}