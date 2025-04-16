using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Essentially a list that contains all other CharacterScriptableObjects
/// </summary>
[CreateAssetMenu(menuName = Constants.ScriptableObjectsFolder + "Roster")]
public class RosterSO : ScriptableObject
{
    public List<CharacterSO> roster;
}