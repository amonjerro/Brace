public static class GameInstance
{
    public static LevelData selectedLevel;
    public static int RoundsToWin = 2;
    public static int Player1Character = 0;
    public static int Player2Character = 0;

    public static bool audioInitialized = false;
    public static float sfxVolume = 1.0f;
    public static float musicVolume = 1.0f;


    public static void Initialize()
    {
        Player1Character = 0;
        Player2Character = 0;
    }

    public static int GetCharacterByPlayerIndex(int index)
    {
        if (index == 0)
        {
            return Player1Character;
        }
        return Player2Character;
    }

    public static void InitializeAudio()
    {
        audioInitialized = true;
    }
}