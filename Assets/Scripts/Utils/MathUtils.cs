public static class MathUtils
{
    /// <summary>
    /// Calculates the Y value of a parabolic function based on the X value
    /// </summary>
    /// <param name="value">The X value to solve for</param>
    /// <param name="yOffset">A yOffset value to shift the final result by</param>
    /// <param name="totalValueRange">The range by which to constrain X values</param>
    /// <param name="maxY">The maximum possible Y value this parabola can take at is peak</param>
    /// <returns>The Y position that corresponds to the X input</returns>
    public static float ParabolicPosition(float value, float yOffset, float totalValueRange, float maxY)
    {
        // Calculate the normalized value of X within its range
        float t = value / totalValueRange;

        // Calulate the value in the form a * (-4x^2 + 4x) + c
        // Feel free to input into Desmos for demonstration
        return maxY * (-4 * t * t + 4 * t) + yOffset;
    }
}