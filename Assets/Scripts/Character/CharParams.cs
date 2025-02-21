using System;
using UnityEngine;

[Serializable]
public struct CharParams
{
    public float jumpDuration;
    public float jumpHeight;
    public float shotCooldown;

    // Defense
    public float blockCooldown;
    public float parryWindow;

    public Sprite sprite;
    public BulletParams bulletParams;

}

[Serializable]
public struct BulletParams
{
    public float moveSpeed;
    public Sprite bulletSprite;
}