namespace GameMenus
{
    /// <summary>
    /// The banner that shoots across the screen when game is over.
    /// I have a feeling we can reuse this for more than just game over.
    /// </summary>
    public class GameEndBanner : AbsMenu
    {
        public void AnimationOver()
        {
            ServiceLocator.Instance.GetService<MenuManager>().CloseMenu(Menus.EndBanner);
            ServiceLocator.Instance.GetService<MenuManager>().OpenMenu(Menus.GameEnd);
        }
    }
}