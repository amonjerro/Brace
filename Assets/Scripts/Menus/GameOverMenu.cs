using UnityEngine;

namespace GameMenus
{
    public class GameOverMenu : AbsMenu
    {
        public override void OnShow()
        {
            Time.timeScale = 0.0f;
        }
        public void RestartGame()
        {
            ServiceLocator.Instance.GetService<MenuManager>().CloseMenu(Menus.GameEnd);
            ServiceLocator.Instance.GetService<BattleManager>().Reset();
        }
    }
}