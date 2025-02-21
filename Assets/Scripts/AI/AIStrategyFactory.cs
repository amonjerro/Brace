public enum AIControlTypes
{
    Static,
    Jumper,
    Blocker,
    Shooter,
    FullAI
}

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
            case AIControlTypes.Static:
            default:
                return new StaticStrategy();
        }
    }
}