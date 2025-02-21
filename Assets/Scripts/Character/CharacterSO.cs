using UnityEngine;

// Wrapper for the Character Parameters
[CreateAssetMenu(fileName = "CharacterSO", menuName = "Scriptable Objects/CharacterSO")]
public class CharacterSO : ScriptableObject
{
    public CharParams characterParameters;
}
