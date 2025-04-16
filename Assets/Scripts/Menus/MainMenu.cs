namespace GameMenus
{
    /// <summary>
    /// The main menu screen. Currently very barebones
    /// </summary>
    public class MainMenu : AbsMenu
    {
        public void Start()
        {
            menuManagerReference = ServiceLocator.Instance.GetService<MenuManager>();
        }

        public void ToCharacterScreen()
        {
            SceneTransitionManager.LoadScene(Constants.CharacterScene);
        }

        public void OpenOptions()
        {
            menuManagerReference.OpenMenu(Menus.Options);
        }
    }
}