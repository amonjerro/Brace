using UnityEngine;

namespace GameMenus
{
    public class GameOverMenu : AbsMenu
    {
        public override void Close()
        {
            gameObject.SetActive(false);
        }

        public void RestartGame()
        {
            ServiceLocator.Instance.GetService<BattleManager>().Reset();
            ServiceLocator.Instance.GetService<MenuManager>().CloseMenu(Menus.GameEnd);
        }
    }
}