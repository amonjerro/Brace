using System.Collections.Generic;
using System.Collections;
using InputManagement;
using UnityEngine;
using UnityEngine.UI;

namespace GameMenus
{
    /// <summary>
    /// Character select menu. Allows players to pick their fighter from the game�s roster.
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
        [Tooltip("Game Object containing the image of the left character")]
        GameObject leftCharacterDesign;

        [SerializeField]
        [Tooltip("Displays all information for the left player's currently selected character")]
        CharacterPanel leftCharacterPanel;

        [SerializeField]
        [Tooltip("Game Object containing the image of the right character")]
        GameObject rightCharacter;

        [SerializeField]
        [Tooltip("Displays all information for the right player's currently selected character")]
        CharacterPanel rightCharacterPanel;

        [SerializeField]
        [Tooltip("The character roster, which contains all relevant character data")]
        RosterSO roster;

        [SerializeField]
        LevelDataSOs LevelData;

        [SerializeField]
        [Tooltip("Portait animation speed")]
        float animationSpeed = 1.0f;

        // Internals
        Dictionary<(int, int), RectTransform> characterPortraits;
        Dictionary<int, int> playerChoices;
        List<CharacterCursor> cursors;
        Dictionary<int, bool> playerReadyStatus;

        RectTransform leftCharTransform;
        Vector3 leftCharacterOriginalPosition;
        IEnumerator leftPortraitChangeAnimation;
        bool leftCoroutine;
        bool rightCoroutine;

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

        public void Awake()
        {
            //GameInstance.selectedLevel = LevelData.Find(Levels.MainMenu);
        }

        private void Start()
        {
            characterPortraits = new Dictionary<(int, int), RectTransform>();
            playerChoices = new Dictionary<int, int>();
            cursors = new List<CharacterCursor>();
            playerReadyStatus = new Dictionary<int, bool>();
            leftCharTransform = leftCharacterDesign.GetComponent<RectTransform>();
            Quaternion q_i = Quaternion.identity;
            leftCharTransform.GetPositionAndRotation(out leftCharacterOriginalPosition, out q_i);
            Debug.Log(leftCharacterOriginalPosition);
            leftCoroutine = false;
            rightCoroutine = false;
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
                AnimateCharacterSpriteSwap(true);
                leftCharacterPanel.UpdateCharacterPanel(roster.roster[index % roster.roster.Count]);
            }
            else if (side == 1) { 
                rightCharacterPanel.UpdateCharacterPanel(roster.roster[index % roster.roster.Count]);
            }
        }


        private void AnimateCharacterSpriteSwap(bool left)
        {
            if (left)
            {
                if (leftCoroutine)
                {
                    StopCoroutine(leftPortraitChangeAnimation);
                }
                Vector3 v = Vector2.zero;
                Quaternion q = Quaternion.identity;
                leftCharTransform.GetPositionAndRotation(out v, out q);
                leftPortraitChangeAnimation = PortaitSwitchAnimation(v, leftCharacterOriginalPosition.x, -335.0f, leftCharTransform);
                StartCoroutine(leftPortraitChangeAnimation);
            }
        }

        /// <summary>
        /// Animation for the character portrait moving
        /// </summary>
        /// <param name="currentPosition">The current position of the sprite being swapped</param>
        /// <param name="finalX">The final X position that this sprite needs to occupy</param>
        /// <param name="screenOutX">The x location that sits outside the screen for this sprite</param>
        /// <param name="trf">The actual sprite to move</param>
        /// <returns></returns>
        private IEnumerator PortaitSwitchAnimation(Vector3 currentPosition, float finalX, float screenOutX, RectTransform trf)
        {
            leftCoroutine = true;
            float finalDistance = Mathf.Abs(finalX - screenOutX);
            float toOutDistance = Mathf.Abs(currentPosition.x - screenOutX);
            float tolerance = 0.1f;
            float distanceTraveled = 0.0f;
            float sign = Mathf.Sign(screenOutX);
            Quaternion i = Quaternion.identity;

            while ((toOutDistance - distanceTraveled) > tolerance) {
                distanceTraveled += animationSpeed;
                Vector3 newPosition = new Vector3(currentPosition.x + (distanceTraveled * sign), currentPosition.y, currentPosition.z);
                trf.SetPositionAndRotation(newPosition, Quaternion.identity);
                yield return null;
            }

            
            //Sprite swap
            sign = sign * -1;
            distanceTraveled = 0.0f;
            trf.GetPositionAndRotation(out currentPosition, out i);

            while((finalDistance - distanceTraveled) > tolerance)
            {
                distanceTraveled += animationSpeed;
                Vector3 newPosition = new Vector3(currentPosition.x + (distanceTraveled * sign), currentPosition.y, currentPosition.z);
                trf.SetPositionAndRotation(newPosition, Quaternion.identity);
                yield return null;
            }
            leftCoroutine = false;
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