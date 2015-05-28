using UnityEngine;
using System.Collections;

public class SoundsUI : MonoBehaviour {

    public Animator soundsOnly;
    public Animator soundsOff;
    public Animator soundsOn;

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        Sounds sounds = MyMaze.Instance.Sounds;
        if (!sounds.sounds && !sounds.theme)
        {
            soundsOn.SetTrigger("FadeIn");
            soundsOff.SetTrigger("FadeOut");
            soundsOnly.SetTrigger("FadeOut");
        }
        else if (sounds.sounds && sounds.theme)
        {
            soundsOn.SetTrigger("FadeOut");
            soundsOff.SetTrigger("FadeOut");
            soundsOnly.SetTrigger("FadeIn");
        }
        else
        {
            soundsOn.SetTrigger("FadeOut");
            soundsOff.SetTrigger("FadeIn");
            soundsOnly.SetTrigger("FadeOut");
        }
    }

    public void EnableAllSounds()
    {
        MyMaze.Instance.Sounds.EnableAllSounds();
        MyMaze.Instance.Sounds.Save();
    }

    public void DisableAllSounds()
    {
        MyMaze.Instance.Sounds.DisableAllSounds();
        MyMaze.Instance.Sounds.Save();
    }

    public void SoundsOnly()
    {
        MyMaze.Instance.Sounds.SoundsOnly();
        MyMaze.Instance.Sounds.Save();
    }
}
