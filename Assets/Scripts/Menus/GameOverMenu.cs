

namespace GameMenus
{
    /// <summary>
    /// The game over menu screen
    /// Allows players to have a rematch, go back to the character select screen or quit to menu
    /// </summary>
    public class GameOverMenu : AbsMenu
    {
        public override void Close()
        {
            gameObject.SetActive(false);
        }

        // Rematch
        public void RestartGame()
        {
            ServiceLocator.Instance.GetService<BattleManager>().Reset();
            ServiceLocator.Instance.GetService<MenuManager>().CloseMenu(Menus.GameEnd);
        }
    }
}