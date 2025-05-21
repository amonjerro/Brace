using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace GameMenus
{
    public class SettingsMenu : AbsMenu
    {
        [SerializeField]
        AudioMixer audioMixer;

        [SerializeField]
        UnityEngine.UI.Selectable defaultSelectable;

        public void SetMasterVolume(float volume)
        {
            audioMixer.SetFloat("Volume", volume);
        }

        public void SetMusicVolume(float volume)
        {
            audioMixer.SetFloat("Music", volume);
        }

        public void SetSFXVolume(float volume)
        {
            audioMixer.SetFloat("SFX", volume);
        }

        public void SetFullScreen(bool isFullScreen) { 
            Screen.fullScreen = isFullScreen;
        }

        public void ExitSettings()
        {
            menuManagerReference.CloseMenu(Menus.Options);
        }

        public override void OnShow()
        {
            base.OnShow();
            Debug.Log("Log some stuff");
            defaultSelectable.Select();
        }

    }
}