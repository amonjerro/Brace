using InputManagement;
using UnityEngine;

namespace GameMenus
{
    public class CharacterSelect : AbsMenu
    {
        public static Vector2 InputToVector(EInput input)
        {
            switch (input) {
                case EInput.Left:
                    return new Vector2(-1, 0);
                case EInput.Right:
                    return new Vector2(1, 0);
                case EInput.Down:
                    return new Vector2(0, 1);
                case EInput.Up:
                    return new Vector2(0, -1);
                default:
                    return Vector2.zero;
            }
        }
    }
}