using System;
using UnityEngine;

[Serializable]
public struct CharParams
{
    // Durations and Timings
    public float jumpDuration;
    public float jumpHeight;
    public float shotCooldown;
    public float attackDuration;
    public float blockCooldown;
    public float parryWindow;

    // Graphic config
    public Sprite sprite;

    // Projectile parameters
    public BulletParams bulletParams;

    public CharacterInfo characterInfo;

    public ShieldParams shieldParams;
}

// Parameters to govern projectile behavior for each character
[Serializable]
public struct BulletParams
{
    public float moveSpeed;
    public Sprite bulletSprite;
    public float bulletDamage;
    public Sprite hardenedBullet;
}

// Parameters to govern the shield behavior for each character
[Serializable]
public struct ShieldParams {
    public ParryEffect effect;
    public Sprite parrySprite;
}

// Descriptions for the character select screen
[Serializable]
public struct CharacterInfo
{
    public string characterName;
    public string fireballAbility;
    public string blockAbility;
    public string jumpAbility;
}