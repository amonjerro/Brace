using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = Constants.ScriptableObjectsFolder + "Roster")]
public class RosterSO : ScriptableObject
{
    public List<CharacterSO> roster;
}