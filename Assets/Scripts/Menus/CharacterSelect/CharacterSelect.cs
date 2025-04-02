using System.Collections.Generic;
using InputManagement;
using UnityEngine;
using UnityEngine.UI;

namespace GameMenus
{

    public class CharacterSelect : AbsMenu
    {

        [SerializeField]
        List<Color> playerCursorColors;

        [SerializeField]
        GameObject characterPortraitContainer;

        [SerializeField]
        CharacterPanel leftCharacterPanel;

        [SerializeField]
        CharacterPanel rightCharacterPanel;

        Dictionary<(int, int), RectTransform> characterPortraits;
        List<CharacterCursor> cursors;

        [SerializeField]
        List<CharacterSO> characterData;

        private void Start()
        {
            characterPortraits = new Dictionary<(int, int), RectTransform>();
            cursors = new List<CharacterCursor>();
            Initialize();
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

        public static EInput VectorToInput(float x, float y)
        {
            if (x < 0 ) return EInput.Left;
            if (x > 0 ) return EInput.Right;
            if (y <  0 ) return EInput.Down;
            if (y > 0 ) return EInput.Up;
            return EInput.None;
        }

        public void UpdateCursorImageLocation(CharacterCursor c, (int, int) newLocation)
        {
            RectTransform portraitTransform = characterPortraits[newLocation];
            RectTransform playerCursor = c.GetComponent<RectTransform>();
            playerCursor.position = portraitTransform.position;
            UpdateCharacterProfile(cursors.IndexOf(c), FlattenLocationToIndex(newLocation));
        }

        public void SpawnCursor(CharacterCursor cursor)
        {
            cursors.Add(cursor);
            cursor.GetComponent<Image>().color = playerCursorColors[cursors.Count-1];
            UpdateCursorImageLocation(cursors[cursors.Count-1], (0, 0));
        }

        private void Initialize()
        {
            RectTransform[] children = characterPortraitContainer.GetComponentsInChildren<RectTransform>();
            int adjustedI = 1;
            for (int i = 1; i < children.Length; i++) {
                adjustedI = i - 1;
                characterPortraits.Add(((adjustedI%3)-1, (adjustedI/3)-1), children[i]);
            }
        }

        private int FlattenLocationToIndex((int, int) location)
        {
            int y = (location.Item2 + 1) * 3;
            int x = (location.Item1 + 1);
            return x + y;
        }

        private void UpdateCharacterProfile(int side, int index)
        {
            if (side == 0)
            {
                leftCharacterPanel.UpdateCharacterPanel(characterData[index % characterData.Count]);
            }
            else if (side == 1) { 
                rightCharacterPanel.UpdateCharacterPanel(characterData[index % characterData.Count]);
            }
        }
    }
}