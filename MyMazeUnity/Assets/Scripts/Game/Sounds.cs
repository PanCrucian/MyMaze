using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(MyMaze))]
public class Sounds : MonoBehaviour{

    [System.Serializable]
    public class SoundMap
    {
        public string name;
        public SoundNames nameEnum;
        public AudioClip clip;
    }

    public MusicStates state;
    public bool theme;
    public bool sounds;

    public float volume = 1f;
    public SoundMap[] soundsMap;

    private float fadeInTime;

    void Start()
    {
        fadeInTime = 0;
    }

    void Update()
    {
        //VolumeFadeIn();
    }

    /// <summary>
    /// Плавно прибавляем звук
    /// </summary>
    void VolumeFadeIn()
    {
        fadeInTime += Time.deltaTime * 0.5f;
        if (fadeInTime > 1)
            fadeInTime = 1;
        AudioListener.volume = volume * (fadeInTime);
    }

    /// <summary>
    /// Установим громкости
    /// </summary>
    void SetupVolume()
    {
        Sound[] soundsGO = GameObject.FindObjectsOfType<Sound>();
        Theme[] themesGO = GameObject.FindObjectsOfType<Theme>();
        foreach (Sound go in soundsGO)
        {
            if (sounds)
                go.GetComponent<AudioSource>().volume = 1f;
            else
                go.GetComponent<AudioSource>().volume = 0f;
        }
        foreach (Theme go in themesGO)
        {
            if (theme)
                go.GetComponent<AudioSource>().volume = 1f;
            else
                go.GetComponent<AudioSource>().volume = 0f;
        }
    }

    public AudioClip GetAudioClip(SoundNames soundName)
    {
        foreach (SoundMap sMap in soundsMap)
            if (sMap.nameEnum == soundName)
                return sMap.clip;

        Debug.LogWarning("Ссылка на звук " + soundName.ToString() + " не найдена");
        return null;
    }

    /// <summary>
    /// Включает все возможные звуки
    /// </summary>
    public void EnableAllSounds()
    {
        theme = true;
        sounds = true;
        SetupVolume();
    }

    /// <summary>
    /// Выключает все возможные звуки
    /// </summary>
    public void DisableAllSounds()
    {
        theme = false;
        sounds = false;
        SetupVolume();
    }

    /// <summary>
    /// Включает звуки эффектов и выключает остальные звуки
    /// </summary>
    public void SoundsOnly()
    {
        theme = false;
        sounds = true;
        SetupVolume();
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
        SetupVolume();
    }
}
