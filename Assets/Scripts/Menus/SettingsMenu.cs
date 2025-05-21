using UnityEngine;
using UnityEngine.Audio;

namespace GameMenus
{
    public class SettingsMenu : AbsMenu
    {
        [SerializeField]
        AudioMixer audioMixer;

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

    }
}