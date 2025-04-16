using System.Collections.Generic;
using InputManagement;
using UnityEngine;
using UnityEngine.UI;

namespace GameMenus
{
    /// <summary>
    /// Character select menu. Allows players to pick their fighter from the game´s roster.
    /// </summary>
    public class CharacterSelect : AbsMenu
    {

        [SerializeField]
        [Tooltip("The colors player's cursors take when they're moving through the select screen")]
        List<Color> playerCursorColors;

        [SerializeField]
        [Tooltip("Central container that holds all the possible characters players can choose from")]
        GameObject characterPortraitContainer;

        [SerializeField]
        [Tooltip("Displays all information for the left player's currently selected character")]
        CharacterPanel leftCharacterPanel;

        [SerializeField]
        [Tooltip("Displays all information for the right player's currently selected character")]
        CharacterPanel rightCharacterPanel;

        [SerializeField]
        [Tooltip("The character roster, which contains all relevant character data")]
        RosterSO roster;

        // Internals
        Dictionary<(int, int), RectTransform> characterPortraits;
        Dictionary<int, int> playerChoices;
        List<CharacterCursor> cursors;
        Dictionary<int, bool> playerReadyStatus;

        #region STATIC_FUNCTIONS
        /// <summary>
        /// Utility method to allow this screen's state machine to calculate movement based on player input. Responds only to movement inputs
        /// </summary>
        /// <param name="input">Input from the player</param>
        /// <returns>A tuple indicating grid traversal</returns>
        public static (int, int) InputToVector(EInput input)
        {
            switch (input)
            {
                case EInput.Left:
                    return (-1, 0);
                case EInput.Right:
                    return (1, 0);
                case EInput.Down:
                    return (0, 1);
                case EInput.Up:
                    return (0, -1);
                default:
                    return (0, 0);
            }
        }

        /// <summary>
        /// Transforms movement player input into the enums that correspond to them
        /// </summary>
        /// <param name="x">The X value</param>
        /// <param name="y">The Y value</param>
        /// <returns>Corresponding movement enum</returns>
        public static EInput VectorToInput(float x, float y)
        {
            if (x < 0) return EInput.Left;
            if (x > 0) return EInput.Right;
            if (y < 0) return EInput.Down;
            if (y > 0) return EInput.Up;
            return EInput.None;
        }
        #endregion


        private void Start()
        {
            characterPortraits = new Dictionary<(int, int), RectTransform>();
            playerChoices = new Dictionary<int, int>();
            cursors = new List<CharacterCursor>();
            playerReadyStatus = new Dictionary<int, bool>();
            Initialize();
        }

        /// <summary>
        /// Initialize and configure this screen
        /// </summary>
        private void Initialize()
        {
            RectTransform[] children = characterPortraitContainer.GetComponentsInChildren<RectTransform>();
            int adjustedI = 1;
            for (int i = 1; i < children.Length; i++)
            {
                adjustedI = i - 1;
                characterPortraits.Add(((adjustedI % 3) - 1, (adjustedI / 3) - 1), children[i]);
            }
        }

        /// <summary>
        /// Updates the location of the character cursor among the portaits to allow players to know which character they are previewing
        /// </summary>
        /// <param name="c">The character cursor that is updating its position</param>
        /// <param name="newLocation">The new coordinates of the location</param>
        public void UpdateCursorImageLocation(CharacterCursor c, (int, int) newLocation)
        {
            RectTransform portraitTransform = characterPortraits[newLocation];
            RectTransform playerCursor = c.GetComponent<RectTransform>();
            playerCursor.position = portraitTransform.position;

            int playerIndex = cursors.IndexOf(c);
            int chosenCharacter = FlattenLocationToIndex(newLocation);
            UpdateCharacterProfile(playerIndex,chosenCharacter);
            playerChoices[playerIndex] = chosenCharacter;
        }

        /// <summary>
        /// Get a players index from its object. Would probably work faster in a Dictionary rather than a List.
        /// </summary>
        /// <param name="c">The Cursor object reference</param>
        /// <returns>The value of the index in the registered players</returns>
        public int GetPlayerIndex(CharacterCursor c)
        {
            return cursors.IndexOf(c);
        }

        /// <summary>
        /// Handle the event of a player joining the game
        /// </summary>
        /// <param name="cursor">The spawned cursor</param>
        public void HandleCursorSpawn(CharacterCursor cursor)
        {
            cursors.Add(cursor);
            cursor.GetComponent<Image>().color = playerCursorColors[cursors.Count-1];
            playerReadyStatus.Add(cursors.Count-1, false);
            playerChoices.Add(cursors.Count - 1, 0);
            UpdateCursorImageLocation(cursors[cursors.Count-1], (0, 0));
        }

        /// <summary>
        /// Toggle whether the player has been set to ready. This should trigger a countdown sequence once all registered players are ready
        /// </summary>
        /// <param name="PlayerIndex">Which player has changed its ready status</param>
        /// <param name="ready">The ready state for this player</param>
        public void SetPlayerReadyStatus(int PlayerIndex, bool ready)
        {
            playerReadyStatus[PlayerIndex] = ready;
            if (TestForGameStart())
            {
                // Populate the game instance with player choices to allow for data to persist through scene transitions
                GameInstance.Player1Character = playerChoices[0] % roster.roster.Count;
                if (playerChoices.ContainsKey(1)){
                    GameInstance.Player2Character = playerChoices[1] % roster.roster.Count;
                }

                // Game Start Process
                // Currently only loads you into next level, we could add a timer that can be interrupted to allow for players to back out
                SceneTransitionManager.LoadScene(2);
            }
        }

        
        /// <summary>
        /// Flatten the two dimensional tuple values into a single dimensional index value for character data lookup
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private int FlattenLocationToIndex((int, int) location)
        {
            int y = (location.Item2 + 1) * 3;
            int x = (location.Item1 + 1);
            return x + y;
        }

        /// <summary>
        /// Update the information being shown on the character profiles
        /// </summary>
        /// <param name="side">Which side to update</param>
        /// <param name="index">The index for the character's info in the roster</param>
        private void UpdateCharacterProfile(int side, int index)
        {
            if (side == 0)
            {
                leftCharacterPanel.UpdateCharacterPanel(roster.roster[index % roster.roster.Count]);
            }
            else if (side == 1) { 
                rightCharacterPanel.UpdateCharacterPanel(roster.roster[index % roster.roster.Count]);
            }
        }

        /// <summary>
        /// Test that all players are ready to start the game
        /// </summary>
        private bool TestForGameStart()
        {
            foreach(bool status in playerReadyStatus.Values)
            {
                if (!status)
                {
                    return status;
                }
            }

            return true;
        }
    }
}