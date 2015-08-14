using UnityEngine;
using System.Collections;

public class SoundsPlayer : MonoBehaviour {

    public SoundNames soundName;
    public SoundTypes type;

    private AudioSource lastCreatedAudio;

    /// <summary>
    /// Играет звук 1 раз
    /// </summary>
    public void PlayOneShootSound()
    {
        AudioSource audio = CreateAudio();
        lastCreatedAudio = audio;
        audio.Play();
        DestroyAfterPlay(audio);
    }

    /// <summary>
    /// Играет звук 1 раз
    /// </summary>
    /// <param name="sN">Какой звук играть</param>
    public void PlayOneShootSound(SoundNames sN)
    {
        AudioSource audio = CreateAudio();
        lastCreatedAudio = audio;
        audio.clip = MyMaze.Instance.Sounds.GetAudioClip(sN);
        audio.Play();
        DestroyAfterPlay(audio);
    }

    /// <summary>
    /// Играет звук бесконечно
    /// </summary>
    public void PlayLooped()
    {
        AudioSource audio = CreateAudio();
        lastCreatedAudio = audio;
        audio.loop = true;
        audio.Play();
    }

    /// <summary>
    /// Останавливаем последний воспроизведенный звук
    /// </summary>
    public bool StopSound()
    {
        if (lastCreatedAudio == null)
        {
            Debug.LogWarning("Не найден последний используемый звук");
            return false;
        }
        Destroy(lastCreatedAudio.gameObject);
        return true;
    }

    /// <summary>
    /// Установим громкость звука и его тип
    /// </summary>
    /// <param name="audio"></param>
    void SetupVolumeAndType(AudioSource audio)
    {
        switch (type)
        {
            case SoundTypes.sounds:
                audio.gameObject.AddComponent<Sound>();
                audio.volume = MyMaze.Instance.Sounds.sounds ? 1f : 0f;
                break;
            case SoundTypes.theme:
                audio.gameObject.AddComponent<Theme>();
                audio.volume = MyMaze.Instance.Sounds.theme ? 1f : 0f;
                break;
        }
    }

    /// <summary>
    /// Уничтожает звук после проигрывания
    /// </summary>
    /// <param name="audio">уничтожаемый звук</param>
    void DestroyAfterPlay(AudioSource audio)
    {
        audio.gameObject.AddComponent<SoundDestroyer>();
    }

    /// <summary>
    /// Ищем слушатель звуков
    /// </summary>
    /// <returns></returns>
    AudioListener FindListener()
    {
        AudioListener listener = GameObject.FindObjectOfType<AudioListener>();
        if (listener == null)
        {
            GameObject newListenerGo = new GameObject() { name = "AudioListener" };
            listener = newListenerGo.AddComponent<AudioListener>();
            newListenerGo.AddComponent<DontDestroyOnLoad>();
        }
        return listener;
    }

    /// <summary>
    /// Создаем источник звука как гейм объект сцены, и помещаем в соответсвующий контейнер на сцене
    /// </summary>
    /// <returns></returns>
    AudioSource CreateAudio()
    {
        AudioListener listener = FindListener();
        GameObject sound = new GameObject() { 
            name = soundName.ToString("g") 
        };
        sound.transform.parent = listener.transform;
        AudioSource audio = sound.AddComponent<AudioSource>();
        audio.playOnAwake = false;
        if (soundName != SoundNames.ScriptSolves)
            audio.clip = MyMaze.Instance.Sounds.GetAudioClip(soundName);
        SetupVolumeAndType(audio);
        return audio;
    }
}
