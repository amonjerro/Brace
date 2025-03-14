namespace GameMenus
{
    public class MainMenu : AbsMenu
    {
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