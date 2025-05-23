using UnityEngine;

namespace GameMenus
{
    /// <summary>
    /// The main menu screen. Currently very barebones
    /// </summary>
    public class MainMenu : AbsMenu
    {
        [SerializeField]
        LevelDataSOs LevelData;

        [SerializeField]
        UnityEngine.UI.Selectable defaultSelectable;

        public void Awake()
        {
            GameInstance.selectedLevel = LevelData.Find(Levels.MainMenu);
        }

        public void Start()
        {
            menuManagerReference = ServiceLocator.Instance.GetService<MenuManager>();
            menuManagerReference.StackMenu(MenuType);
        }

        public void ToCharacterScreen()
        {
            SceneTransitionManager.LoadScene(Constants.CharacterScene);
        }

        public void OpenOptions()
        {
            menuManagerReference.OpenMenu(Menus.Options);
        }

        public override void OnShow()
        {
            base.OnShow();
            Debug.Log("MainMenu show");
            defaultSelectable.Select();
        }


    }
}