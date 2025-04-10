using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameMenus
{
    public enum Menus
    {
        None,
        Main,
        Pause,
        GameEnd,
        CharacterSelect,
        Options,
        EndBanner
    }

    /// <summary>
    /// Game service that manages the state of UI Menus
    /// </summary>
    public class MenuManager : AbsGameService
    {
        public static Action UIInputSwitch;
        public static Action PlayInputSwitch;


        [SerializeField]
        [Tooltip("Canvas that holds all possible menus")]
        Canvas canvas;

        Dictionary<Menus, AbsMenu> menuList;
        Stack<Menus> menuStack;

        // Upon awake, load all available menus and cache references
        public void Awake()
        {
            menuList = new Dictionary<Menus, AbsMenu>();
            menuStack = new Stack<Menus>();

            // Get references to each menu available in this scene
            foreach(Transform child in canvas.transform)
            {
                AbsMenu menu = child.GetComponent<AbsMenu>();
                if (menu != null)
                {
                    menuList.Add(menu.MenuType, menu);
                }
            }
        }

        // Open a menu screen and show it.
        public void OpenMenu(Menus menu)
        {
            if (!menuList.ContainsKey(menu))
            {
                Debug.Log("No such menu can be found configured in this scene");
                return;
            }

            if (menuStack.Count == 0)
            {
                UIInputSwitch?.Invoke();
            } else
            {
                menuList[menuStack.Peek()].Hide();
            }
            menuStack.Push(menu);
            if (!menuList[menu].gameObject.activeInHierarchy)
            {
                menuList[menu].gameObject.SetActive(true);
            }
            menuList[menu].Open();
        }

        // Hide and close a menu screen.
        public void CloseMenu(Menus menu)
        {
            if (!ValidateClose(menu))
            {
                return;
            }

            menuList[menu].Close();
            menuStack.Pop();

            // Open the next screen down the stack or go back to Gaming™
            if (menuStack.Count == 0)
            {
                PlayInputSwitch?.Invoke();
            }
            else
            {
                menuList[menuStack.Peek()].Show();
            }
            
        }

        // Validation function to avoid closing menus in a way that messes up the UI layer stack
        private bool ValidateClose(Menus menu)
        {
            if (!menuList.ContainsKey(menu))
            {
                throw new MissingReferenceException("No reference to such a menu configured");
            }

            if (menuStack.Count == 0)
            {
                throw new InvalidOperationException("Close called on Empty Stack");
            }

            if (menuStack.Peek() != menu)
            {

                throw new ArgumentException("Menu to close not on top of Stack");
            }

            return true;
        }

        // Clean up subscribers to delegate actions
        public override void CleanUp()
        {
            UIInputSwitch = null;
            PlayInputSwitch = null;
        }

    }


    /// <summary>
    /// Abstract class to define a game menu
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public abstract class AbsMenu : MonoBehaviour
    {
        public Menus MenuType;

        [SerializeField]
        protected Animator animator;
        
        protected MenuManager menuManagerReference = null;


        /// <summary>
        /// Opens this menu.
        /// </summary>
        public virtual void Open()
        {
            if (menuManagerReference == null)
            {
                menuManagerReference = ServiceLocator.Instance.GetService<MenuManager>();
            }
            if (gameObject.activeSelf) {
                animator.SetTrigger(Constants.UIAnimationShow);
            }
        }

        public virtual void OnShow() { }

        // Show menu - useful when switching between stack layers
        public virtual void Show()
        {
            gameObject.SetActive(true);
            OnShow();
        }

        // Hide menu - useful when switching between stack layers
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        // Close this menu.
        public virtual void Close()
        {
            animator.SetTrigger(Constants.UIAnimationHide);
        }

        // Back To Main Menu
        public void ToMainMenu()
        {
            SceneTransitionManager.LoadScene(Constants.MainMenu);
        }

        // Back To Main Menu
        public void ToCharacterSelect()
        {
            SceneTransitionManager.LoadScene(Constants.CharacterScene);
        }


        // Quit To Desktop
        public void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
    }
}