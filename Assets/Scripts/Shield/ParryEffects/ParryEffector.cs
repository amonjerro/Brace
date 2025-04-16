public enum ParryEffect
{
    SpeedUp,
    Harden,
    Absorb
}

public static class ParryFactory{
    public static AbsParryEffector MakeParryFactory(Character c)
    {
        switch (c.GetParryType())
        {
            case ParryEffect.SpeedUp:
                return new SpeedUpEffect(c);
            case ParryEffect.Harden:
                return new HardenEffect(c);
            case ParryEffect.Absorb:
                return new AbsorbEffect(c);
            default:
                return new NoneEffect(c);
        }
    }
}

public abstract class AbsParryEffector
{
    protected Character character;
    public AbsParryEffector(Character c)
    {
        character = c;
    }

    public abstract void Apply(Bullet bullet);
}

public class NoneEffect : AbsParryEffector
{
    public NoneEffect(Character c) : base(c) { }

    public override void Apply(Bullet bullet)
    {
        throw new System.NotImplementedException("No class implemented");
    }
}


public class SpeedUpEffect : AbsParryEffector
{
    public SpeedUpEffect(Character c) : base(c) { }

    public override void Apply(Bullet bullet)
    {
        bullet.MovementComponent.direction *= -1;
        bullet.CheckForSpriteFlip();
        bullet.MovementComponent.speed *= 2;
    }
}

public class HardenEffect : AbsParryEffector
{
    public HardenEffect(Character c) : base(c) { }

    public override void Apply(Bullet bullet)
    {
        bullet.Harden();
        bullet.MovementComponent.direction *= -1;
        bullet.CheckForSpriteFlip();
    }
}

public class AbsorbEffect : AbsParryEffector
{
    public AbsorbEffect(Character c) : base(c) { }

    public override void Apply(Bullet bullet)
    {
        character.AddCharge();
        bullet.ResetBullet();
    }
}
