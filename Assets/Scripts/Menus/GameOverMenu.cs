namespace GameMenus
{
    public class GameOverMenu : AbsMenu
    {
        public void RestartGame()
        {
            ServiceLocator.Instance.GetService<MenuManager>().CloseMenu(Menus.GameEnd);
            ServiceLocator.Instance.GetService<BattleManager>().Reset();
        }
    }
}