using UnityEditor;

namespace GameMenus
{
    public class PauseMenu : AbsMenu
    {
        // Unpause
        public void Unpause()
        {
            menuManagerReference.CloseMenu(Menus.Pause);
        }

        // Settings
        public void OpenOptions()
        {
            menuManagerReference.OpenMenu(Menus.Options);
        }

        // Character Select
        public void ToCharacterScreen()
        {
            SceneTransitionManager.LoadScene(Constants.CharacterScene);
        }

        
    }
}