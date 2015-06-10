using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SoundsUI : MonoBehaviour {

    public Animator soundsOnly;
    public Animator soundsOff;
    public Animator soundsOn;

    void OnEnable()
    {
        StartCoroutine(OnEnableNumerator());
    }

    IEnumerator OnEnableNumerator()
    {
        yield return new WaitForEndOfFrame();
        Sounds sounds = MyMaze.Instance.Sounds;
        if (!sounds.sounds && !sounds.theme)
            SwitchSoindsOn();
        else if (sounds.sounds && sounds.theme)
            SwitchSoundsOnly();
        else
            SwitchSoundsOff();
    }

    void SwitchSoindsOn()
    {
        if (soundsOn.isActiveAndEnabled)
            soundsOn.SetTrigger("FadeIn");
        if (soundsOff.isActiveAndEnabled)
            soundsOff.SetTrigger("FadeOut");
        if (soundsOnly.isActiveAndEnabled)
            soundsOnly.SetTrigger("FadeOut");
    }

    void SwitchSoundsOnly()
    {
        if (soundsOn.isActiveAndEnabled)
            soundsOn.SetTrigger("FadeOut");
        if (soundsOff.isActiveAndEnabled)
            soundsOff.SetTrigger("FadeOut");
        if (soundsOnly.isActiveAndEnabled)
            soundsOnly.SetTrigger("FadeIn");
    }

    void SwitchSoundsOff()
    {
        if (soundsOn.isActiveAndEnabled)
            soundsOn.SetTrigger("FadeOut");
        if (soundsOff.isActiveAndEnabled)
            soundsOff.SetTrigger("FadeIn");
        if (soundsOnly.isActiveAndEnabled)
            soundsOnly.SetTrigger("FadeOut");
    }

    public void OnClick(GameObject buttonObject)
    {
        buttonObject.GetComponent<Button>().interactable = false;
        StartCoroutine(OnClickNumerator(buttonObject));
    }

    IEnumerator OnClickNumerator(GameObject buttonObject)
    {
        yield return new WaitForSeconds(1f);
        buttonObject.GetComponent<Button>().interactable = true;
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
