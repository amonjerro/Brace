using TMPro;
using UnityEngine;

namespace GameMenus
{
    public class CharacterPanel : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI characterName;

        [SerializeField]
        TextMeshProUGUI blockPowerDesc;

        [SerializeField]
        TextMeshProUGUI fireballPowerDesc;

        [SerializeField]
        TextMeshProUGUI jumpPowerDesc;


        public void UpdateCharacterPanel(CharacterSO characterData)
        {
            characterName.text = characterData.characterParameters.characterInfo.characterName;
            blockPowerDesc.text = characterData.characterParameters.characterInfo.blockAbility;
            fireballPowerDesc.text = characterData.characterParameters.characterInfo.fireballAbility;
            jumpPowerDesc.text = characterData.characterParameters.characterInfo.jumpAbility;
        }
    }
}