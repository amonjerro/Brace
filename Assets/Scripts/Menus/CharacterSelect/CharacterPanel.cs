using TMPro;
using UnityEngine;

namespace GameMenus
{
    /// <summary>
    /// UI Panel that shows players the information about the character they currently have selected on
    /// the character screen
    /// </summary>
    public class CharacterPanel : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Label for the name")]
        TextMeshProUGUI characterName;

        [SerializeField]
        [Tooltip("Label for the block ability description")]
        TextMeshProUGUI blockPowerDesc;

        [SerializeField]
        [Tooltip("Label for the projectile ability description")]
        TextMeshProUGUI fireballPowerDesc;

        [SerializeField]
        [Tooltip("Label for the jump ability description")]
        TextMeshProUGUI jumpPowerDesc;

        // To do: Add the fields for the sprites


        /// <summary>
        /// Update the information in the character panel by accessing the ScriptableObject data
        /// </summary>
        /// <param name="characterData">The ScriptableObject character data</param>
        public void UpdateCharacterPanel(CharacterSO characterData)
        {
            characterName.text = characterData.characterParameters.characterInfo.characterName;
            blockPowerDesc.text = characterData.characterParameters.characterInfo.blockAbility;
            fireballPowerDesc.text = characterData.characterParameters.characterInfo.fireballAbility;
            jumpPowerDesc.text = characterData.characterParameters.characterInfo.jumpAbility;
        }
    }
}