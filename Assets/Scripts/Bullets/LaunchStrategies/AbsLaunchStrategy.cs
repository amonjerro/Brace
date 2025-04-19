
using UnityEngine;

public enum LaunchStrategies
{
    Normal,
    DelayedLaunch
}

public static class LaunchFactory
{
    public static AbsLaunchStrategy MakeLaunchFactory(LaunchStrategies type, Bullet parent)
    {
        switch (type)
        {
            case LaunchStrategies.DelayedLaunch:
                return new DelayedLaunch(parent);
            default:
                return new RegularLaunch(parent);
        }
    }
}


public abstract class AbsLaunchStrategy
{
    public LaunchStrategies LaunchStrategies { get; protected set; }

    protected Bullet parentBullet;

    public AbsLaunchStrategy(Bullet parentBullet)
    {
        this.parentBullet = parentBullet;
    }

    public abstract void Launch(Vector3 direction, BulletParams parameters);
    public virtual void Update() { }
}

public class RegularLaunch : AbsLaunchStrategy
{

    public RegularLaunch(Bullet parentBullet) : base(parentBullet) {
        LaunchStrategies = LaunchStrategies.Normal;
    }

    public override void Launch(Vector3 direction, BulletParams parameters)
    {
        parentBullet.SetSpeed(parameters.moveSpeed);
        parentBullet.SetDirection(direction);
    }
}

public class DelayedLaunch : AbsLaunchStrategy
{
    private float timer;
    private float triggerTime;
    private float expectedSpeed;
    private bool bTriggered;
    public DelayedLaunch(Bullet parentBullet) : base(parentBullet) {

        LaunchStrategies = LaunchStrategies.DelayedLaunch;
        triggerTime = 0.5f;
        bTriggered = false;
    }

    public override void Launch(Vector3 direction, BulletParams parameters)
    {
        expectedSpeed = parameters.moveSpeed;
        parentBullet.SetDirection(direction);
        timer = 0.0f;
        bTriggered = false;
    }

    public override void Update()
    {
        if (bTriggered) {
            return;
        }
        timer += Time.deltaTime;
        if (timer > triggerTime) { 
            bTriggered = true;
            parentBullet.SetSpeed(expectedSpeed);
        }
    }

}