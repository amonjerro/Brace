using System.Collections.Generic;
using InputManagement;
using UnityEngine;
using UnityEngine.UI;

namespace GameMenus
{

    public class CharacterSelect : AbsMenu
    {
        [SerializeField]
        GameObject playerCursorPrefab;

        [SerializeField]
        List<Color> playerCursorColors;

        [SerializeField]
        GameObject characterPortraitContainer;


        Dictionary<(int, int), RectTransform> characterPortraits;
        List<RectTransform> cursors;

        private void Start()
        {
            characterPortraits = new Dictionary<(int, int), RectTransform>();
            cursors = new List<RectTransform>();
        }

        public static (int, int) InputToVector(EInput input)
        {
            switch (input) {
                case EInput.Left:
                    return (-1, 0);
                case EInput.Right:
                    return (1, 0);
                case EInput.Down:
                    return (0, 1);
                case EInput.Up:
                    return (0, -1);
                default:
                    return (0,0);
            }
        }

        public void UpdateCursorImageLocation(int playerId, (int, int) newLocation)
        {
            RectTransform portraitTransform = characterPortraits[newLocation];
            RectTransform playerCursor = cursors[playerId];

            playerCursor.localPosition = portraitTransform.localPosition;
        }

        public void SpawnCursor()
        {
            GameObject cursor = Instantiate(characterPortraitContainer);
            cursors.Add(cursor.GetComponent<RectTransform>());
            cursor.GetComponent<Image>().color = playerCursorColors[cursors.Count-1];
            UpdateCursorImageLocation(cursors.Count-1, (cursors.Count - 1, 0));

        }
    }
}