using UnityEngine;
using UnityEngine.UIElements;

namespace GameMenus
{
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