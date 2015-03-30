using UnityEngine;
using System.Collections;

public class Sounds : MonoBehaviour{

    public MusicStates state;
    public bool theme;
    public bool sounds;

    public float volume = 1f;
    
    /// <summary>
    /// Включает все возможные звуки
    /// </summary>
    public void EnableAllSounds()
    {
        theme = true;
        sounds = true;
    }

    /// <summary>
    /// Выключает все возможные звуки
    /// </summary>
    public void DisableAllSounds()
    {
        theme = false;
        sounds = false;
    }

    /// <summary>
    /// Включает звуки эффектов и выключает остальные звуки
    /// </summary>
    public void SoundsOnly()
    {
        theme = false;
        sounds = true;
    }
}
