using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(MyMaze))]
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

    /// <summary>
    /// Сохраняет в PlayerPrefs информацию о текущем состоянии звуков и музыки
    /// </summary>
    public void Save()
    {
        PlayerPrefs.SetInt("Sounds#theme", Convert.ToInt32(this.theme));
        PlayerPrefs.SetInt("Sounds#sounds", Convert.ToInt32(this.sounds));
        PlayerPrefs.SetFloat("Sounds#volume", Convert.ToInt32(this.volume));
    }

    /// <summary>
    /// Загружает из PlayerPrefs информацию о прошлом состоянии звуков и музыки
    /// </summary>
    public void Load()
    {
        if (PlayerPrefs.HasKey("Sounds#theme"))
            this.theme = Convert.ToBoolean(PlayerPrefs.GetInt("Sounds#theme"));
        if (PlayerPrefs.HasKey("Sounds#sounds"))
            this.sounds = Convert.ToBoolean(PlayerPrefs.GetInt("Sounds#sounds"));
        if (PlayerPrefs.HasKey("Sounds#volume"))
            this.volume = PlayerPrefs.GetFloat("Sounds#volume");
    }
}
