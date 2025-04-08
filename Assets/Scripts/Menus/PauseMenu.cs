namespace GameMenus
{
    /// <summary>
    /// Class that specifies commands and configuration variables for the pause menu
    /// </summary>
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
        
    }
}