namespace GameMenus
{
    public class GameEndBanner : AbsMenu
    {
        public void AnimationOver()
        {
            ServiceLocator.Instance.GetService<MenuManager>().OpenMenu(Menus.GameEnd);
        }
    }
}