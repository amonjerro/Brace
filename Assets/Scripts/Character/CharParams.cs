using System;
using UnityEngine;

[Serializable]
public struct CharParams
{
    public float jumpDuration;
    public float jumpHeight;
    public Sprite sprite;
    public BulletParams bulletParams;

}

[Serializable]
public struct BulletParams
{
    public float moveSpeed;
    public Sprite bulletSprite;
}