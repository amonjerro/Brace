public enum AIControlTypes
{
    Static,
    Jumper,
    Blocker,
    Parrier,
    Shooter,
    Random,
    FullAI
}

/// <summary>
/// Helper class to make new AI Strategies
/// </summary>
public static class StrategyFactory
{
    public static AbsAIStrategy MakeStrategy(AIControlTypes controlType)
    {
        switch (controlType) {
            case AIControlTypes.Blocker:
                return new BlockerStrategy();
            case AIControlTypes.Shooter:
                return new ShooterStrategy();
            case AIControlTypes.Jumper:
                return new JumperStrategy();
            case AIControlTypes.Random:
                return new RandomStrategy();
            case AIControlTypes.FullAI:
                return new WorldStateAnticipationStrategy();
            case AIControlTypes.Static:
            default:
                return new StaticStrategy();
        }
    }
}