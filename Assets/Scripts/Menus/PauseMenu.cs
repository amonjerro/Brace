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
            SceneTransitionManager.LoadScene(2);
        }

        // Back To Main Menu
        public void ToMainMenu()
        {
            SceneTransitionManager.LoadScene(1);
        }

        // Quit To Desktop
        public void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
    }
}