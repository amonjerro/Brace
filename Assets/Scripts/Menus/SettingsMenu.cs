using UnityEngine;
using UnityEngine.Audio;

namespace GameMenus
{
    public class SettingsMenu : AbsMenu
    {
        [SerializeField]
        AudioMixer audioMixer;

        public void SetVolume(float volume)
        {
            audioMixer.SetFloat("Volume", volume);
        }

        public void SetFullScreen(bool isFullScreen) { 
            Screen.fullScreen = isFullScreen;
        }

    }
}