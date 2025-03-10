using UnityEngine;

// Wrapper for the Character Parameters
[CreateAssetMenu(fileName = Constants.CharacterSOFileName, menuName = Constants.ScriptableObjectsFolder + Constants.CharacterSOFileName)]
public class CharacterSO : ScriptableObject
{
    public CharParams characterParameters;
}
