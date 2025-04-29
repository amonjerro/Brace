using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = Constants.ScriptableObjectsFolder + "LevelData")]
public class LevelDataSOs : ScriptableObject
{
    public List<LevelData> data;

    public LevelData Find(Levels level)
    {
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].levelType == level)
            {
                return data[i];
            }
        }

        return data[0];
    }
}

public enum Levels
{
    Test,
    MainMenu,
    CharacterSelect
}

[Serializable]
public struct LevelData {
    public string levelName;
    public Levels levelType;
    public AudioClip backgroundMusic;
    public Sprite foreground;
    public Sprite background;
}
