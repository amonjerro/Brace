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

        public void Awake()
        {
            GameInstance.selectedLevel = LevelData.Find(Levels.MainMenu);
        }

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