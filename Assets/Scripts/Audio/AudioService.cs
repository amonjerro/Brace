using UnityEngine;
using UnityEngine.Rendering.UI;

[RequireComponent(typeof(AudioSource))]
public class AudioService : AbsGameService
{
    [SerializeField]
    AudioSource backgroundMusic;

    [SerializeField]
    AudioSource interruptSource;

    #region PROPERTIES
    // General volume control
    private float _masterVolume = 1.0f;
    public float MasterVolume
    {
        get { return _masterVolume; }
        set { 
            _masterVolume = value;

            // Change the volume of the background music as this property is being edited
            backgroundMusic.volume = value * MusicVolume;
        }
    }

    // BGM volume control
    private float _musicVolume = 1.0f;

    /// <summary>
    /// Use this property to adjust music volume through settings
    /// </summary>
    public float MusicVolume
    {
        get { return _musicVolume; }
        set
        {
            _musicVolume = value;

            // Change the volume of the background music as it is being edited
            backgroundMusic.volume = value * MasterVolume;
        }
    }

    // SFX volume control
    private float _sfxVolume = 1.0f;
    public float SFXVolume
    {
        get { return _sfxVolume; }
        set
        {
            _sfxVolume = value;
        }
    }
    #endregion

    // Internals
    bool _bMusicIsPlaying = false;

    private void Start()
    {
        
    }

    public override void CleanUp()
    {
        // Nothing to do here yet
    }


    public void PlaySFX(AudioClip sound, bool interrupt)
    {
        if (!interrupt && interruptSource.isPlaying) {
            return;
        }

        if (interruptSource.isPlaying) {
            interruptSource.Stop();
        }
        interruptSource.PlayOneShot(sound, _sfxVolume);
    }

}
