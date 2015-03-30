using UnityEngine;
using System.Collections;

public class SoundsUI : MonoBehaviour {

    public void EnableAllSounds()
    {
        Game.Instance.Sounds.EnableAllSounds();
    }

    public void DisableAllSounds()
    {
        Game.Instance.Sounds.DisableAllSounds();
    }

    public void SoundsOnly()
    {
        Game.Instance.Sounds.SoundsOnly();
    }
}
