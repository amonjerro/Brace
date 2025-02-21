using UnityEngine;

public class Shield : MonoBehaviour
{
    Character character;

    private void Start()
    {
        character = GetComponentInParent<Character>();
    }

    public bool IsParry
    {
        get
        {
            return character.IsWithinParryWindow();
        }
    }
}
