using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameMenus
{
    public enum Menus
    {
        None,
        Main,
        Pause,
        GameEnd,
        CharacterSelect,
        Options
    }

    public class MenuManager : AbsGameService
    {
        public static Action UIInputSwitch;
        public static Action PlayInputSwitch;


        [SerializeField]
        Canvas canvas;

        Dictionary<Menus, AbsMenu> menuList;
        Stack<Menus> menuStack;

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
            menuList[menu].Open();
        }

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

        public override void CleanUp()
        {
            UIInputSwitch = null;
            PlayInputSwitch = null;
        }

    }

    [RequireComponent(typeof(Animator))]
    public abstract class AbsMenu : MonoBehaviour
    {
        public Menus MenuType;

        [SerializeField]
        protected Animator animator;

        protected MenuManager menuManagerReference = null;

        public virtual void Open()
        {
            if (menuManagerReference == null)
            {
                menuManagerReference = ServiceLocator.Instance.GetService<MenuManager>();
            }
            animator.SetTrigger(Constants.UIAnimationShow);
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void Close()
        {
            animator.SetTrigger(Constants.UIAnimationHide);
        }
    }
}