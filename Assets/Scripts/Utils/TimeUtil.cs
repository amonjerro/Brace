using UnityEngine;
public static class TimeUtil
{
    private static float previousTime;
    public static float deltaTime;
    public static float timeScale;

    public static void Initialize()
    {
        previousTime = Time.time;
        timeScale = Time.timeScale;
        deltaTime = 0.0f;
    }

    public static void UpdateDeltaTime()
    {
        deltaTime = Time.time - previousTime;
        previousTime = Time.time;
    }

    public static float GetDelta()
    {

        return deltaTime * timeScale;
    }
}